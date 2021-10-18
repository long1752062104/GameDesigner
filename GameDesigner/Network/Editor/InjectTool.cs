#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Net.Share;
using UnityEditor.Compilation;
using System.IO;
using System.Reflection;
using ILRuntime.Mono.Cecil;
using ILRuntime.Mono.Cecil.Mdb;
using ILRuntime.Mono.Cecil.Cil;

public static class InjectTool
{
    private const string AssemblyPath = "./Library/ScriptAssemblies/Assembly-CSharp.dll";
    public static AssemblyDefinition m_ScriptDef;

    [InitializeOnLoadMethod]
    static void OnInitializeOnLoad()
    {
        int open = 0;
        if (PlayerPrefs.HasKey("RpcInject"))
            open = PlayerPrefs.GetInt("RpcInject", 0);
        if(open == 0)
            CompilationPipeline.assemblyCompilationFinished += OnCompilationFinished;
    }

    [MenuItem("GameDesigner/Network/openRpcInject")]
    static void Init1()
    {
        PlayerPrefs.SetInt("RpcInject", 0);
        Debug.Log("rpc注入开启");
    }

    [MenuItem("GameDesigner/Network/closeRpcInject")]
    static void Init()
    {
        PlayerPrefs.SetInt("RpcInject", 1);
        Debug.Log("rpc注入关闭");
    }

    internal static void OnCompilationFinished(string targetAssembly, CompilerMessage[] messages)
    {
        if (messages.Length > 0)
            foreach (var msg in messages)
                if (msg.type == CompilerMessageType.Error)
                    return;
        if (targetAssembly.Contains("-Editor") || targetAssembly.Contains(".Editor"))
            return;
        if (targetAssembly.Contains("com.unity") || Path.GetFileName(targetAssembly).StartsWith("Unity"))
            return;
        var readerParameters = new ReaderParameters { ReadWrite = true };
        m_ScriptDef = AssemblyDefinition.ReadAssembly(AssemblyPath, readerParameters);
        if (m_ScriptDef == null)
        {
            Debug.LogError(string.Format("InjectTool Inject Load assembly failed: {0}", AssemblyPath));
            return;
        }
        try
        {
            var module = m_ScriptDef.MainModule;
            var needInjectAttr = typeof(Rpc).FullName;
            foreach (var type in module.Types)
            {
                List<MethodDefinition> mets = new List<MethodDefinition>();
                foreach (var method in type.Methods)
                {
                    foreach (var item in method.CustomAttributes)
                    {
                        var name = item.AttributeType.Name;
                        if (name == "Rpc" | name == "rpc" | name == "RPCFun")
                        {
                            var met = InjectMethod(module, method, item);
                            mets.Add(met);
                            break;
                        }
                    }
                }
                foreach (var item in mets)
                {
                    type.Methods.Add(item);
                }
            }
            m_ScriptDef.Write(new WriterParameters() { SymbolWriterProvider = new MdbWriterProvider() });
        }
        catch (Exception ex)
        {
            Debug.LogError(string.Format("Rpc注入失败: {0}", ex));
        }
        finally
        {
            if (m_ScriptDef.MainModule.SymbolReader != null)
                m_ScriptDef.MainModule.SymbolReader.Dispose();
            var pdbToDelete = Path.ChangeExtension(AssemblyPath,  ".pdb");
            if (pdbToDelete != null)
                File.Delete(pdbToDelete);
            m_ScriptDef.Dispose();
        }
    }

    private static MethodDefinition InjectMethod(ModuleDefinition module, MethodDefinition method, CustomAttribute attribute)
    {
        var name = method.Name;
        method.Name = "Call" + name;
        MethodDefinition met = new MethodDefinition(name, method.Attributes, method.ReturnType);
        foreach (var item in method.CustomAttributes)
            met.CustomAttributes.Add(item);
        method.CustomAttributes.Clear();
        foreach (var item in method.Parameters)
            met.Parameters.Add(item);
        foreach (var item in method.Body.Variables)
            met.Body.Variables.Add(item);
        met.Body.MaxStackSize = method.Body.MaxStackSize;

        ILProcessor cmdWorker = met.Body.GetILProcessor();

        var clientBaseType = module.GetType("Net.Client.ClientBase");
        var clientBaseDef = clientBaseType.Resolve();
        MethodDefinition instance = clientBaseDef.Methods[6];
        MethodDefinition send;
        int hasCmd = 0;
        ushort hasMask = 0;
        if (attribute.Fields.Count == 0)
            send = clientBaseDef.Methods[178];
        else if (attribute.Fields[0].Argument.Value == null)
            send = clientBaseDef.Methods[178];
        else
        {
            foreach (var item in attribute.Fields)
            {
                if (item.Name == "cmd")
                    hasCmd = (byte)item.Argument.Value;
                if (item.Name == "mask")
                    hasMask = (ushort)item.Argument.Value;
            }
            if(hasMask != 0 & hasCmd != 0)
                send = clientBaseDef.Methods[181];
            else if(hasMask != 0)
                send = clientBaseDef.Methods[180];
            else
                send = clientBaseDef.Methods[179];
        }

        var objectType = module.ImportReference(typeof(object));
      
        cmdWorker.Append(cmdWorker.Create(OpCodes.Nop));
        cmdWorker.Append(cmdWorker.Create(OpCodes.Call, instance));
        if(hasCmd != 0)
            cmdWorker.Append(cmdWorker.Create(OpCodes.Ldc_I4_S, (sbyte)hasCmd));
        if(hasMask != 0 & hasMask < 128)
            cmdWorker.Append(cmdWorker.Create(OpCodes.Ldc_I4_S, (sbyte)hasMask));
        else if(hasMask != 0)
            cmdWorker.Append(cmdWorker.Create(OpCodes.Ldc_I4, hasMask));
        else
            cmdWorker.Append(cmdWorker.Create(OpCodes.Ldstr, name));
        if (method.Parameters.Count < 8)
        {
            var Ldc_I4_x = typeof(OpCodes).GetField("Ldc_I4_" + method.Parameters.Count, BindingFlags.Public | BindingFlags.Static);
            OpCode opCode = (OpCode)Ldc_I4_x.GetValue(null);
            cmdWorker.Append(cmdWorker.Create(opCode));
        }
        else 
        {
            cmdWorker.Append(cmdWorker.Create(OpCodes.Ldc_I4_S, (sbyte)method.Parameters.Count));
        }
        cmdWorker.Append(cmdWorker.Create(OpCodes.Newarr, objectType));
        cmdWorker.Append(cmdWorker.Create(OpCodes.Dup));

        for (int i = 0; i < method.Parameters.Count; i++)
        {
            OpCode Ldc_I4_x_OpCode;
            OpCode Ldarg_x_OpCode;
            bool ldc_s = false;
            if (i >= 9)
            {
                Ldc_I4_x_OpCode = OpCodes.Ldc_I4_S;
                ldc_s = true;
            }
            else 
            {
                var Ldc_I4_x = typeof(OpCodes).GetField("Ldc_I4_" + i, BindingFlags.Public | BindingFlags.Static);
                Ldc_I4_x_OpCode = (OpCode)Ldc_I4_x.GetValue(null);
            }
            bool arg_s = false;
            if (i >= 3)
            {
                Ldarg_x_OpCode = OpCodes.Ldarg_S;
                arg_s = true;
            }
            else
            {
                var Ldarg_x = typeof(OpCodes).GetField("Ldarg_" + (i + 1), BindingFlags.Public | BindingFlags.Static);
                Ldarg_x_OpCode = (OpCode)Ldarg_x.GetValue(null);
            }
            var parType = method.Parameters[i].ParameterType;

            if(!ldc_s)
                cmdWorker.Append(cmdWorker.Create(Ldc_I4_x_OpCode));
            else
                cmdWorker.Append(cmdWorker.Create(Ldc_I4_x_OpCode, (sbyte)i));
            if (!arg_s)
                cmdWorker.Append(cmdWorker.Create(Ldarg_x_OpCode));
            else
                cmdWorker.Append(cmdWorker.Create(Ldarg_x_OpCode, method.Parameters[i]));
            cmdWorker.Append(cmdWorker.Create(OpCodes.Box, parType));
            cmdWorker.Append(cmdWorker.Create(OpCodes.Stelem_Ref));

            cmdWorker.Append(cmdWorker.Create(OpCodes.Dup));
        }

        if (cmdWorker.Body.Instructions[cmdWorker.Body.Instructions.Count - 1].OpCode == OpCodes.Dup)
            cmdWorker.Body.Instructions.RemoveAt(cmdWorker.Body.Instructions.Count - 1);

        cmdWorker.Append(cmdWorker.Create(OpCodes.Callvirt, send));
        cmdWorker.Append(cmdWorker.Create(OpCodes.Nop));
        cmdWorker.Append(cmdWorker.Create(OpCodes.Ret));

        var instructions1 = new List<Instruction>(method.Body.Instructions);
        var instructions2 = new List<Instruction>(met.Body.Instructions);
        method.Body.Instructions.Clear();
        met.Body.Instructions.Clear();
        foreach (var item in instructions1)
            met.Body.Instructions.Add(item);
        foreach (var item in instructions2)
            method.Body.Instructions.Add(item);
        return met;
    }
}
#endif