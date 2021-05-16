using ILRuntime.CLR.Method;
using ILRuntime.CLR.Utils;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ILRuntime.Runtime.Debugger
{
    public class DebugService
    {
        BreakPointContext curBreakpoint;
        DebuggerServer server;
        Runtime.Enviorment.AppDomain domain;
        Dictionary<int, LinkedList<BreakpointInfo>> activeBreakpoints = new Dictionary<int, LinkedList<BreakpointInfo>>();
        Dictionary<int, BreakpointInfo> breakpointMapping = new Dictionary<int, BreakpointInfo>();
        Queue<KeyValuePair<int, VariableReference>> pendingReferences = new Queue<KeyValuePair<int, VariableReference>>();
        Queue<KeyValuePair<int, VariableReference>> pendingEnuming = new Queue<KeyValuePair<int, VariableReference>>();
        Queue<KeyValuePair<int, KeyValuePair<VariableReference, VariableReference>>> pendingIndexing = new Queue<KeyValuePair<int, KeyValuePair<VariableReference, VariableReference>>>();
        AutoResetEvent evt = new AutoResetEvent(false);

        public Action<string> OnBreakPoint;

        public Enviorment.AppDomain AppDomain { get { return domain; } }

        public AutoResetEvent BlockEvent { get { return evt; } }

        public bool IsDebuggerAttached
        {
            get
            {
#if DEBUG && !DISABLE_ILRUNTIME_DEBUG
                return (server != null && server.IsAttached);
#else
                return false;
#endif
            }
        }

        public DebugService(Runtime.Enviorment.AppDomain domain)
        {
            this.domain = domain;
        }

        /// <summary>
        /// Start Debugger Server
        /// </summary>
        /// <param name="port">Port to listen on</param>
        public void StartDebugService(int port)
        {
#if DEBUG && !DISABLE_ILRUNTIME_DEBUG
            server = new Debugger.DebuggerServer(this);
            server.Port = port;
            server.Start();
#endif
        }

        /// <summary>
        /// Stop Debugger Server
        /// </summary>
        public void StopDebugService()
        {
#if DEBUG && !DISABLE_ILRUNTIME_DEBUG
            server.Stop();
            server = null;
#endif
        }

        /// <summary>
        /// 中断运行
        /// </summary>
        /// <param name="intpreter"></param>
        /// <param name="ex"></param>
        /// <returns>如果挂的有调试器则返回true</returns>
        internal bool Break(ILIntepreter intpreter, Exception ex = null)
        {
            BreakPointContext ctx = new BreakPointContext();
            ctx.Interpreter = intpreter;
            ctx.Exception = ex;

            curBreakpoint = ctx;

            if (OnBreakPoint != null)
            {
                OnBreakPoint(ctx.DumpContext());
                return true;
            }
            return false;
        }

        public string GetStackTrace(ILIntepreter intepreper)
        {
            StringBuilder sb = new StringBuilder();
            ILRuntime.CLR.Method.ILMethod m;
            StackFrame[] frames = intepreper.Stack.Frames.ToArray();
            Mono.Cecil.Cil.Instruction ins = null;
            if (frames[0].Address != null)
            {
                ins = frames[0].Method.Definition.Body.Instructions[frames[0].Address.Value];
                sb.AppendLine(ins.ToString());
            }
            for (int i = 0; i < frames.Length; i++)
            {
                StackFrame f = frames[i];
                m = f.Method;
                string document = "";
                if (f.Address != null)
                {
                    ins = m.Definition.Body.Instructions[f.Address.Value];

                    Mono.Cecil.Cil.SequencePoint seq = FindSequencePoint(ins, m.Definition.DebugInformation.GetSequencePointMapping());
                    if (seq != null)
                    {
                        string path = seq.Document.Url.Replace("\\", "/");
                        document = string.Format("(at {0}:{1})", path, seq.StartLine);
                    }
                }
                sb.AppendFormat("at {0} {1}\r\n", m, document);
            }

            return sb.ToString();
        }

        public unsafe string GetThisInfo(ILIntepreter intepreter)
        {
            StackFrame topFrame = intepreter.Stack.Frames.Peek();
            StackObject* arg = Minus(topFrame.LocalVarPointer, topFrame.Method.ParameterCount);
            if (topFrame.Method.HasThis)
                arg--;
            if (arg->ObjectType == ObjectTypes.StackObjectReference)
            {
                long addr = *(long*)&arg->Value;
                arg = (StackObject*)addr;
            }
            ILTypeInstance instance = arg->ObjectType != ObjectTypes.Null ? intepreter.Stack.ManagedStack[arg->Value] as ILTypeInstance : null;
            if (instance == null)
                return "null";
            Mono.Collections.Generic.Collection<Mono.Cecil.FieldDefinition> fields = instance.Type.TypeDefinition.Fields;
            int idx = 0;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < fields.Count; i++)
            {
                try
                {
                    Mono.Cecil.FieldDefinition f = fields[i];
                    if (f.IsStatic)
                        continue;
                    StackObject field = instance.Fields[idx];
                    object v = StackObject.ToObject(&field, intepreter.AppDomain, instance.ManagedObjects);
                    if (v == null)
                        v = "null";
                    string name = f.Name;
                    sb.AppendFormat("{0} {1} = {2}", f.FieldType.Name, name, v);
                    if ((idx % 3 == 0 && idx != 0) || idx == instance.Fields.Length - 1)
                        sb.AppendLine();
                    else
                        sb.Append(", ");
                    idx++;
                }
                catch
                {

                }
            }
            return sb.ToString();
        }

        public unsafe string GetLocalVariableInfo(ILIntepreter intepreter)
        {
            StackFrame topFrame = intepreter.Stack.Frames.Peek();
            ILMethod m = topFrame.Method;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < m.LocalVariableCount; i++)
            {
                try
                {
                    Mono.Cecil.Cil.VariableDefinition lv = m.Definition.Body.Variables[i];
                    StackObject* val = Add(topFrame.LocalVarPointer, i);
                    object v = StackObject.ToObject(val, intepreter.AppDomain, intepreter.Stack.ManagedStack);
                    if (v == null)
                        v = "null";
                    m.Definition.DebugInformation.TryGetName(lv, out string vName);
                    string name = string.IsNullOrEmpty(vName) ? "v" + lv.Index : vName;
                    sb.AppendFormat("{0} {1} = {2}", lv.VariableType.Name, name, v);
                    if ((i % 3 == 0 && i != 0) || i == m.LocalVariableCount - 1)
                        sb.AppendLine();
                    else
                        sb.Append(", ");
                }
                catch
                {

                }
            }
            return sb.ToString();
        }

        internal static Mono.Cecil.Cil.SequencePoint FindSequencePoint(Mono.Cecil.Cil.Instruction ins, IDictionary<Mono.Cecil.Cil.Instruction, Mono.Cecil.Cil.SequencePoint> seqMapping)
        {
            Mono.Cecil.Cil.Instruction cur = ins;
            Mono.Cecil.Cil.SequencePoint sp;
            while (!seqMapping.TryGetValue(cur, out sp) && cur.Previous != null)
                cur = cur.Previous;

            return sp;
        }

        unsafe StackObject* Add(StackObject* a, int b)
        {
            return (StackObject*)((long)a + sizeof(StackObject) * b);
        }

        unsafe StackObject* Minus(StackObject* a, int b)
        {
            return (StackObject*)((long)a - sizeof(StackObject) * b);
        }

        internal void NotifyModuleLoaded(string moduleName)
        {
            if (server != null && server.IsAttached)
                server.NotifyModuleLoaded(moduleName);
        }

        internal void SetBreakPoint(int methodHash, int bpHash, int startLine)
        {
            lock (activeBreakpoints)
            {
                if (!activeBreakpoints.TryGetValue(methodHash, out LinkedList<BreakpointInfo> lst))
                {
                    lst = new LinkedList<Debugger.BreakpointInfo>();
                    activeBreakpoints[methodHash] = lst;
                }

                BreakpointInfo bpInfo = new BreakpointInfo();
                bpInfo.BreakpointHashCode = bpHash;
                bpInfo.MethodHashCode = methodHash;
                bpInfo.StartLine = startLine;

                lst.AddLast(bpInfo);
                breakpointMapping[bpHash] = bpInfo;
            }
        }

        internal void DeleteBreakpoint(int bpHash)
        {
            lock (activeBreakpoints)
            {
                if (breakpointMapping.TryGetValue(bpHash, out BreakpointInfo bpInfo))
                {
                    if (activeBreakpoints.TryGetValue(bpInfo.MethodHashCode, out LinkedList<BreakpointInfo> lst))
                    {
                        lst.Remove(bpInfo);
                    }
                    breakpointMapping.Remove(bpHash);
                }
            }
        }

        internal void ExecuteThread(int threadHash)
        {
            lock (AppDomain.FreeIntepreters)
            {
                foreach (KeyValuePair<int, ILIntepreter> i in AppDomain.Intepreters)
                {
                    //We should resume all threads on execute
                    i.Value.ClearDebugState();
                    i.Value.Resume();
                }
            }
        }

        internal unsafe void StepThread(int threadHash, StepTypes type)
        {
            lock (AppDomain.FreeIntepreters)
            {
                if (AppDomain.Intepreters.TryGetValue(threadHash, out ILIntepreter intp))
                {
                    intp.ClearDebugState();
                    intp.CurrentStepType = type;
                    intp.LastStepFrameBase = intp.Stack.Frames.Count > 0 ? intp.Stack.Frames.Peek().BasePointer : (StackObject*)0;
                    intp.LastStepInstructionIndex = intp.Stack.Frames.Count > 0 ? intp.Stack.Frames.Peek().Address.Value : 0;

                    intp.Resume();
                }
            }
        }

        unsafe internal void CheckShouldBreak(ILMethod method, ILIntepreter intp, int ip)
        {
            if (server != null && server.IsAttached)
            {
                int methodHash = method.GetHashCode();
                BreakpointInfo[] lst = null;

                lock (activeBreakpoints)
                {
                    if (activeBreakpoints.TryGetValue(methodHash, out LinkedList<BreakpointInfo> bps))
                        lst = bps.ToArray();
                }
                bool bpHit = false;

                if (lst != null)
                {
                    Mono.Cecil.Cil.SequencePoint sp = method.Definition.DebugInformation.GetSequencePoint(method.Definition.Body.Instructions[ip]);
                    if (sp != null)
                    {
                        foreach (BreakpointInfo i in lst)
                        {
                            if ((i.StartLine + 1) == sp.StartLine)
                            {
                                DoBreak(intp, i.BreakpointHashCode, false);
                                bpHit = true;
                                break;
                            }
                        }
                    }
                }

                if (!bpHit)
                {
                    Mono.Cecil.Cil.SequencePoint sp = method.Definition.DebugInformation.GetSequencePoint(method.Definition.Body.Instructions[ip]);//.SequencePoint;
                    if (sp != null && IsSequenceValid(sp))
                    {
                        switch (intp.CurrentStepType)
                        {
                            case StepTypes.Into:
                                DoBreak(intp, 0, true);
                                break;
                            case StepTypes.Over:
                                if (intp.Stack.Frames.Peek().BasePointer <= intp.LastStepFrameBase && ip != intp.LastStepInstructionIndex)
                                {
                                    DoBreak(intp, 0, true);
                                }
                                break;
                            case StepTypes.Out:
                                {
                                    if (intp.Stack.Frames.Count > 0 && intp.Stack.Frames.Peek().BasePointer < intp.LastStepFrameBase)
                                    {
                                        DoBreak(intp, 0, true);
                                    }
                                }
                                break;
                        }
                    }
                }
            }
        }

        bool IsSequenceValid(Mono.Cecil.Cil.SequencePoint sp)
        {
            return sp.StartLine != sp.EndLine || sp.StartColumn != sp.EndColumn;
        }

        void DoBreak(ILIntepreter intp, int bpHash, bool isStep)
        {
            KeyValuePair<int, StackFrameInfo[]>[] frames = new KeyValuePair<int, StackFrameInfo[]>[AppDomain.Intepreters.Count];
            frames[0] = new KeyValuePair<int, StackFrameInfo[]>(intp.GetHashCode(), GetStackFrameInfo(intp));
            int idx = 1;
            foreach (KeyValuePair<int, ILIntepreter> j in AppDomain.Intepreters)
            {
                if (j.Value != intp)
                {
                    j.Value.ShouldBreak = true;
                    try
                    {
                        frames[idx++] = new KeyValuePair<int, Debugger.StackFrameInfo[]>(j.Value.GetHashCode(), GetStackFrameInfo(j.Value));
                    }
                    catch
                    {
                        frames[idx++] = new KeyValuePair<int, Debugger.StackFrameInfo[]>(j.Value.GetHashCode(), new StackFrameInfo[0]);
                    }
                }
            }
            if (!isStep)
                server.SendSCBreakpointHit(intp.GetHashCode(), bpHash, frames);
            else
                server.SendSCStepComplete(intp.GetHashCode(), frames);
            //Breakpoint hit
            intp.Break();
        }

        unsafe StackFrameInfo[] GetStackFrameInfo(ILIntepreter intp)
        {
            StackFrame[] frames = intp.Stack.Frames.ToArray();
            Mono.Cecil.Cil.Instruction ins = null;
            ILMethod m;
            List<StackFrameInfo> frameInfos = new List<StackFrameInfo>();

            for (int j = 0; j < frames.Length; j++)
            {
                StackFrameInfo info = new Debugger.StackFrameInfo();
                StackFrame f = frames[j];
                m = f.Method;
                info.MethodName = m.ToString();

                if (f.Address != null)
                {
                    ins = m.Definition.Body.Instructions[f.Address.Value];

                    Mono.Cecil.Cil.SequencePoint seq = FindSequencePoint(ins, m.Definition.DebugInformation.GetSequencePointMapping());
                    if (seq != null)
                    {
                        info.DocumentName = seq.Document.Url;
                        info.StartLine = seq.StartLine - 1;
                        info.StartColumn = seq.StartColumn - 1;
                        info.EndLine = seq.EndLine - 1;
                        info.EndColumn = seq.EndColumn - 1;
                    }
                    else
                        continue;
                }
                StackFrame topFrame = f;
                m = topFrame.Method;
                int argumentCount = m.ParameterCount;
                if (m.HasThis)
                    argumentCount++;
                info.LocalVariables = new VariableInfo[argumentCount + m.LocalVariableCount];
                for (int i = 0; i < argumentCount; i++)
                {
                    int argIdx = m.HasThis ? i - 1 : i;
                    StackObject* arg = Minus(topFrame.LocalVarPointer, argumentCount);
                    string name = null;
                    object v = null;
                    string typeName = null;
                    StackObject* val = Add(arg, i);
                    v = StackObject.ToObject(val, intp.AppDomain, intp.Stack.ManagedStack);
                    if (argIdx >= 0)
                    {
                        Mono.Cecil.ParameterDefinition lv = m.Definition.Parameters[argIdx];
                        name = string.IsNullOrEmpty(lv.Name) ? "arg" + lv.Index : lv.Name;
                        typeName = lv.ParameterType.FullName;
                    }
                    else
                    {
                        name = "this";
                        typeName = m.DeclearingType.FullName;
                    }

                    VariableInfo vinfo = VariableInfo.FromObject(v);
                    vinfo.Address = (long)val;
                    vinfo.Name = name;
                    vinfo.TypeName = typeName;
                    vinfo.Expandable = GetValueExpandable(val, intp.Stack.ManagedStack);

                    info.LocalVariables[i] = vinfo;
                }
                for (int i = argumentCount; i < info.LocalVariables.Length; i++)
                {
                    int locIdx = i - argumentCount;
                    Mono.Cecil.Cil.VariableDefinition lv = m.Definition.Body.Variables[locIdx];
                    StackObject* val = Add(topFrame.LocalVarPointer, locIdx);
                    object v = StackObject.ToObject(val, intp.AppDomain, intp.Stack.ManagedStack);
                    CLR.TypeSystem.IType type = intp.AppDomain.GetType(lv.VariableType, m.DeclearingType, m);
                    m.Definition.DebugInformation.TryGetName(lv, out string vName);
                    string name = string.IsNullOrEmpty(vName) ? "v" + lv.Index : vName;
                    VariableInfo vinfo = VariableInfo.FromObject(v);
                    vinfo.Address = (long)val;
                    vinfo.Name = name;
                    vinfo.TypeName = lv.VariableType.FullName;
                    vinfo.Expandable = GetValueExpandable(val, intp.Stack.ManagedStack);
                    info.LocalVariables[i] = vinfo;
                }
                frameInfos.Add(info);
            }
            return frameInfos.ToArray();
        }

        internal unsafe VariableInfo[] EnumChildren(int threadHashCode, VariableReference parent)
        {
            if (AppDomain.Intepreters.TryGetValue(threadHashCode, out ILIntepreter intepreter))
            {
#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
                if (domain.IsNotUnityMainThread())
                {
                    lock (pendingEnuming)
                    {
                        pendingEnuming.Enqueue(new KeyValuePair<int, VariableReference>(threadHashCode, parent));
                    }
                    return null;
                }
#endif
                VariableInfo info = ResolveVariable(threadHashCode, parent, out object obj);
                if (obj != null)
                {
                    if (obj is Array)
                    {
                        return EnumArray((Array)obj, intepreter);
                    }
                    else if (obj is IList)
                    {
                        return EnumList((IList)obj, intepreter);
                    }
                    else if (obj is IDictionary)
                    {
                        return EnumDictionary((IDictionary)obj, intepreter);
                    }
                    else if (obj is ILTypeInstance)
                    {
                        return EnumILTypeInstance((ILTypeInstance)obj, intepreter);
                    }
                    else if (obj is ILRuntime.Runtime.Enviorment.CrossBindingAdaptorType)
                    {
                        return EnumILTypeInstance(((Enviorment.CrossBindingAdaptorType)obj).ILInstance, intepreter);
                    }
                    else
                    {
                        return EnumCLRObject(obj, intepreter);
                    }
                }
                else
                    return new VariableInfo[] { VariableInfo.NullReferenceExeption };
            }
            else
                return new VariableInfo[] { VariableInfo.NullReferenceExeption };
        }

        VariableInfo[] EnumArray(Array arr, ILIntepreter intepreter)
        {
            VariableInfo[] res = new VariableInfo[arr.Length];

            for (int i = 0; i < arr.Length; i++)
            {
                try
                {
                    object obj = arr.GetValue(i);

                    VariableInfo info = VariableInfo.FromObject(obj, true);
                    info.Name = string.Format("[{0}]", i);
                    info.Offset = i;
                    info.Type = VariableTypes.IndexAccess;
                    res[i] = info;
                }
                catch (Exception ex)
                {
                    VariableInfo info = VariableInfo.GetException(ex);
                    info.Name = string.Format("[{0}]", i);
                    res[i] = info;
                }
            }

            return res;
        }

        VariableInfo[] EnumList(IList lst, ILIntepreter intepreter)
        {
            VariableInfo[] res = new VariableInfo[lst.Count];

            for (int i = 0; i < lst.Count; i++)
            {
                try
                {
                    object obj = lst[i];

                    VariableInfo info = VariableInfo.FromObject(obj, true);
                    info.Name = string.Format("[{0}]", i);
                    info.Offset = i;
                    info.Type = VariableTypes.IndexAccess;

                    res[i] = info;
                }
                catch (Exception ex)
                {
                    VariableInfo info = VariableInfo.GetException(ex);
                    info.Name = string.Format("[{0}]", i);
                    res[i] = info;
                }
            }

            return res;
        }

        VariableInfo[] EnumDictionary(IDictionary lst, ILIntepreter intepreter)
        {
            VariableInfo[] res = new VariableInfo[lst.Count];

            object[] keys = GetArray(lst.Keys);
            object[] values = GetArray(lst.Values);
            for (int i = 0; i < lst.Count; i++)
            {
                try
                {
                    object obj = values[i];
                    VariableInfo info = VariableInfo.FromObject(obj, true);
                    info.Name = string.Format("[{0}]", i);
                    info.Type = VariableTypes.IndexAccess;
                    info.Offset = i;
                    info.Value = string.Format("{0},{1}", SafeToString(keys[i]), SafeToString(values[i]));
                    info.Expandable = true;
                    res[i] = info;
                }
                catch (Exception ex)
                {
                    VariableInfo info = VariableInfo.GetException(ex);
                    info.Name = string.Format("[{0}]", i);
                    res[i] = info;
                }
            }
            return res;
        }

        string SafeToString(object obj)
        {
            if (obj != null)
                return obj.ToString();
            else
                return "null";
        }
        object[] GetArray(ICollection lst)
        {
            object[] res = new object[lst.Count];
            int idx = 0;
            foreach (object i in lst)
            {
                res[idx++] = i;
            }
            return res;
        }

        VariableInfo[] EnumILTypeInstance(ILTypeInstance obj, ILIntepreter intepreter)
        {
            return EnumObject(obj, obj.Type.ReflectionType);
        }

        VariableInfo[] EnumCLRObject(object obj, ILIntepreter intepreter)
        {
            return EnumObject(obj, obj.GetType());
        }

        VariableInfo[] EnumObject(object obj, Type t)
        {
            List<VariableInfo> lst = new List<VariableInfo>();
            foreach (System.Reflection.FieldInfo i in t.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
            {
                try
                {
                    if (i.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false).Length > 0)
                        continue;
                    object val = i.GetValue(obj);
                    VariableInfo info = VariableInfo.FromObject(val);
                    info.Type = VariableTypes.FieldReference;
                    info.TypeName = i.FieldType.FullName;
                    info.Name = i.Name;
                    info.Expandable = !i.FieldType.IsPrimitive && val != null;
                    info.IsPrivate = i.IsPrivate;
                    info.IsProtected = i.IsFamily;

                    lst.Add(info);
                }
                catch (Exception ex)
                {
                    VariableInfo info = VariableInfo.GetException(ex);
                    info.Name = i.Name;
                    lst.Add(info);
                }
            }

            foreach (System.Reflection.PropertyInfo i in t.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
            {
                try
                {
                    if (i.GetIndexParameters().Length > 0)
                        continue;
                    if (i.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length > 0)
                        continue;
                    object val = i.GetValue(obj, null);
                    VariableInfo info = VariableInfo.FromObject(val);
                    info.Type = VariableTypes.PropertyReference;
                    info.TypeName = i.PropertyType.FullName;
                    info.Name = i.Name;
                    info.Expandable = !i.PropertyType.IsPrimitive && val != null;
                    info.IsPrivate = i.GetGetMethod(true).IsPrivate;
                    info.IsProtected = i.GetGetMethod(true).IsFamily;

                    lst.Add(info);
                }
                catch (Exception ex)
                {
                    VariableInfo info = VariableInfo.GetException(ex);
                    info.Name = i.Name;
                    lst.Add(info);
                }
            }

            return lst.ToArray();
        }

        internal unsafe VariableInfo ResolveIndexAccess(int threadHashCode, VariableReference body, VariableReference idx, out object res)
        {
            res = null;
            if (AppDomain.Intepreters.TryGetValue(threadHashCode, out ILIntepreter intepreter))
            {
#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
                if (domain.IsNotUnityMainThread())
                {
                    lock (pendingIndexing)
                    {
                        pendingIndexing.Enqueue(new KeyValuePair<int, KeyValuePair<VariableReference, VariableReference>>(threadHashCode, new KeyValuePair<VariableReference, VariableReference>(body, idx)));
                    }
                    res = null;
                    return new VariableInfo() { Type = VariableTypes.Pending };
                }
#endif
                VariableInfo info = ResolveVariable(threadHashCode, body, out object obj);
                if (obj != null)
                {
                    info = ResolveVariable(threadHashCode, idx, out object idxObj);
                    if (obj is Array)
                    {
                        res = ((Array)obj).GetValue((int)idxObj);
                        info = VariableInfo.FromObject(res);
                        info.Type = VariableTypes.IndexAccess;
                        info.TypeName = obj.GetType().GetElementType().FullName;
                        info.Expandable = res != null && !obj.GetType().GetElementType().IsPrimitive;

                        return info;
                    }
                    else
                    {
                        if (obj is ILTypeInstance)
                        {
                            IMethod m = ((ILTypeInstance)obj).Type.GetMethod("get_Item");
                            if (m != null)
                            {
                                res = intepreter.AppDomain.Invoke(m, obj, idxObj);
                                info = VariableInfo.FromObject(res);
                                info.Type = VariableTypes.IndexAccess;
                                info.TypeName = m.ReturnType.FullName;
                                info.Expandable = res != null && !m.ReturnType.IsPrimitive;

                                return info;
                            }
                            else
                                return VariableInfo.NullReferenceExeption;
                        }
                        else
                        {
                            if (obj is ILRuntime.Runtime.Enviorment.CrossBindingAdaptorType)
                            {
                                throw new NotImplementedException();
                            }
                            else
                            {
                                if (obj is IDictionary && idxObj is int)
                                {
                                    IDictionary dic = (IDictionary)obj;
                                    object[] keys = GetArray(dic.Keys);
                                    if (keys[0].GetType() != typeof(int))
                                    {
                                        int index = (int)idxObj;
                                        object[] values = GetArray(dic.Values);
                                        Type t = typeof(KeyValuePair<,>).MakeGenericType(keys[index].GetType(), values[index].GetType());
                                        System.Reflection.ConstructorInfo ctor = t.GetConstructor(new Type[] { keys[index].GetType(), values[index].GetType() });
                                        res = ctor.Invoke(new object[] { keys[index], values[index] });
                                        info = VariableInfo.FromObject(res);
                                        info.Type = VariableTypes.IndexAccess;
                                        info.Offset = index;
                                        info.TypeName = t.FullName;
                                        info.Expandable = true;

                                        return info;
                                    }
                                }
                                System.Reflection.PropertyInfo pi = obj.GetType().GetProperty("Item");
                                if (pi != null)
                                {
                                    res = pi.GetValue(obj, new object[] { idxObj });
                                    info = VariableInfo.FromObject(res);
                                    info.Type = VariableTypes.IndexAccess;
                                    info.TypeName = pi.PropertyType.FullName;
                                    info.Expandable = res != null && !pi.PropertyType.IsPrimitive;

                                    return info;
                                }
                                else
                                    return VariableInfo.NullReferenceExeption;
                            }
                        }
                    }
                }
                else
                    return VariableInfo.NullReferenceExeption;
            }
            else
                return VariableInfo.NullReferenceExeption;
        }

        internal void ResolvePendingRequests()
        {
            lock (pendingReferences)
            {
                while (pendingReferences.Count > 0)
                {
                    VariableInfo info;
                    KeyValuePair<int, VariableReference> r = pendingReferences.Dequeue();
                    try
                    {
                        info = ResolveVariable(r.Key, r.Value, out object res);
                    }
                    catch (Exception ex)
                    {
                        info = VariableInfo.GetException(ex);
                    }
                    server.SendSCResolveVariableResult(info);
                }
            }
            lock (pendingEnuming)
            {
                while (pendingEnuming.Count > 0)
                {
                    VariableInfo[] info;
                    KeyValuePair<int, VariableReference> r = pendingEnuming.Dequeue();
                    try
                    {
                        info = EnumChildren(r.Key, r.Value);
                    }
                    catch (Exception ex)
                    {
                        info = new VariableInfo[] { VariableInfo.GetException(ex) };
                    }
                    server.SendSCEnumChildrenResult(info);
                }
            }
            lock (pendingIndexing)
            {
                while (pendingIndexing.Count > 0)
                {
                    VariableInfo info;
                    KeyValuePair<int, KeyValuePair<VariableReference, VariableReference>> r = pendingIndexing.Dequeue();
                    try
                    {
                        info = ResolveIndexAccess(r.Key, r.Value.Key, r.Value.Value, out object res);
                    }
                    catch (Exception ex)
                    {
                        info = VariableInfo.GetException(ex);
                    }
                    server.SendSCResolveVariableResult(info);
                }
            }
        }

        internal unsafe VariableInfo ResolveVariable(int threadHashCode, VariableReference variable, out object res)
        {
            res = null;
            if (AppDomain.Intepreters.TryGetValue(threadHashCode, out ILIntepreter intepreter))
            {
                if (variable != null)
                {
#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
                    if (domain.IsNotUnityMainThread())
                    {
                        lock (pendingReferences)
                        {
                            pendingReferences.Enqueue(new KeyValuePair<int, VariableReference>(threadHashCode, variable));
                        }
                        res = null;
                        return new VariableInfo() { Type = VariableTypes.Pending };
                    }
#endif
                    switch (variable.Type)
                    {
                        case VariableTypes.Normal:
                            {
                                StackObject* ptr = (StackObject*)variable.Address;
                                object obj = StackObject.ToObject(ptr, AppDomain, intepreter.Stack.ManagedStack);
                                if (obj != null)
                                {
                                    //return ResolveMember(obj, name, out res);   
                                    res = obj;
                                    return null;
                                }
                                else
                                {
                                    return VariableInfo.Null;
                                }
                            }
                        case VariableTypes.FieldReference:
                        case VariableTypes.PropertyReference:
                            {
                                if (variable.Parent != null)
                                {
                                    VariableInfo info = ResolveVariable(threadHashCode, variable.Parent, out object obj);
                                    if (obj != null)
                                    {
                                        return ResolveMember(obj, variable.Name, out res);
                                    }
                                    else
                                    {
                                        return VariableInfo.NullReferenceExeption;
                                    }
                                }
                                else
                                {
                                    StackFrame frame = intepreter.Stack.Frames.Peek();
                                    ILMethod m = frame.Method;
                                    if (m.HasThis)
                                    {
                                        StackObject* addr = Minus(frame.LocalVarPointer, m.ParameterCount + 1);
                                        object v = StackObject.ToObject(addr, intepreter.AppDomain, intepreter.Stack.ManagedStack);
                                        VariableInfo result = ResolveMember(v, variable.Name, out res);
                                        if (result.Type == VariableTypes.NotFound)
                                        {
                                            ILTypeInstance ins = v as ILTypeInstance;
                                            if (ins != null)
                                            {
                                                Type ilType = ins.Type.ReflectionType;
                                                System.Reflection.FieldInfo[] fields = ilType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                                                foreach (System.Reflection.FieldInfo f in fields)
                                                {
                                                    if (f.Name.Contains("_this"))
                                                    {
                                                        result = ResolveMember(f.GetValue(v), variable.Name, out res);
                                                        if (result.Type != VariableTypes.NotFound)
                                                            return result;
                                                    }
                                                }
                                            }
                                        }
                                        return result;
                                    }
                                    else
                                    {
                                        return VariableInfo.GetCannotFind(variable.Name);
                                    }
                                }
                            }
                        case VariableTypes.IndexAccess:
                            {
                                return ResolveIndexAccess(threadHashCode, variable.Parent, variable.Parameters[0], out res);
                            }
                        case VariableTypes.Integer:
                            {
                                res = variable.Offset;
                                return VariableInfo.GetInteger(variable.Offset);
                            }
                        case VariableTypes.String:
                            {
                                res = variable.Name;
                                return VariableInfo.GetString(variable.Name);
                            }
                        case VariableTypes.Boolean:
                            {
                                if (variable.Offset == 1)
                                {
                                    res = true;
                                    return VariableInfo.True;
                                }
                                else
                                {
                                    res = false;
                                    return VariableInfo.False;
                                }
                            }
                        case VariableTypes.Null:
                            {
                                res = null;
                                return VariableInfo.Null;
                            }
                        default:
                            throw new NotImplementedException();
                    }
                }
                else
                {
                    return VariableInfo.NullReferenceExeption;
                }
            }
            else
                return VariableInfo.NullReferenceExeption;
        }

        VariableInfo ResolveMember(object obj, string name, out object res)
        {
            res = null;
            Type type = null;
            if (obj is ILTypeInstance)
            {
                type = ((ILTypeInstance)obj).Type.ReflectionType;
            }
            else if (obj is Enviorment.CrossBindingAdaptorType)
                type = ((Enviorment.CrossBindingAdaptorType)obj).ILInstance.Type.ReflectionType;
            else
                type = obj.GetType();
            System.Reflection.FieldInfo fi = type.GetField(name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (fi != null)
            {
                res = fi.GetValue(obj);
                VariableInfo info = VariableInfo.FromObject(res);

                info.Address = 0;
                info.Name = name;
                info.Type = VariableTypes.FieldReference;
                info.TypeName = fi.FieldType.FullName;
                info.IsPrivate = fi.IsPrivate;
                info.IsProtected = fi.IsFamily;
                info.Expandable = res != null && !fi.FieldType.IsPrimitive;

                return info;
            }
            else
            {
                System.Reflection.FieldInfo[] fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                string match = string.Format("<{0}>", name);
                foreach (System.Reflection.FieldInfo f in fields)
                {
                    if (f.Name.Contains(match))
                    {
                        res = f.GetValue(obj);
                        VariableInfo info = VariableInfo.FromObject(res);

                        info.Address = 0;
                        info.Name = name;
                        info.Type = VariableTypes.FieldReference;
                        info.TypeName = f.FieldType.FullName;
                        info.IsPrivate = f.IsPrivate;
                        info.IsProtected = f.IsFamily;
                        info.Expandable = res != null && !f.FieldType.IsPrimitive;

                        return info;
                    }
                }
            }

            System.Reflection.PropertyInfo pi = type.GetProperty(name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (pi != null)
            {
                res = pi.GetValue(obj, null);
                VariableInfo info = VariableInfo.FromObject(res);

                info.Address = 0;
                info.Name = name;
                info.Type = VariableTypes.PropertyReference;
                info.TypeName = pi.PropertyType.FullName;
                info.IsPrivate = pi.GetGetMethod(true).IsPrivate;
                info.IsProtected = pi.GetGetMethod(true).IsFamily;
                info.Expandable = res != null && !pi.PropertyType.IsPrimitive;
                return info;
            }

            return VariableInfo.GetCannotFind(name);
        }

        unsafe bool GetValueExpandable(StackObject* esp, IList<object> mStack)
        {
            if (esp->ObjectType < ObjectTypes.Object)
                return false;
            else
            {
                object obj = mStack[esp->Value];
                if (obj == null)
                    return false;
                if (obj is ILTypeInstance)
                    return true;
                else if (obj.GetType().IsPrimitive)
                    return false;
                else
                    return true;

            }
        }

        internal void ThreadStarted(ILIntepreter intp)
        {
            if (server != null && server.IsAttached)
            {
                server.SendSCThreadStarted(intp.GetHashCode());
            }
        }

        internal void ThreadEnded(ILIntepreter intp)
        {
            if (server != null && server.IsAttached)
            {
                server.SendSCThreadEnded(intp.GetHashCode());
            }
        }

        internal void Detach()
        {
            activeBreakpoints.Clear();
            breakpointMapping.Clear();
            pendingEnuming.Clear();
            pendingReferences.Clear();
            pendingIndexing.Clear();
            foreach (KeyValuePair<int, ILIntepreter> j in AppDomain.Intepreters)
            {
                j.Value.ClearDebugState();
                j.Value.Resume();
            }
        }

        internal unsafe void DumpStack(StackObject* esp, RuntimeStack stack)
        {
            StackObject* start = stack.StackBase;
            StackObject* end = esp + 10;
            UncheckedStack<StackFrame> frames = stack.Frames;
            IList<object> mStack = stack.ManagedStack;
            StackObject* valuePointerEnd = stack.ValueTypeStackPointer;
            StringBuilder final = new StringBuilder();
            HashSet<long> leakVObj = new HashSet<long>();
            for (StackObject* i = stack.ValueTypeStackBase; i > stack.ValueTypeStackPointer;)
            {
                leakVObj.Add((long)i);
                i = Minus(i, i->ValueLow + 1);
            }
            for (StackObject* i = start; i <= end; i++)
            {
                StringBuilder sb = new StringBuilder();
                ILMethod localMethod = null, baseMethod = null;
                bool isLocal = false;
                bool isBase = false;
                int localIdx = 0;
                if (i == esp)
                    sb.Append("->");
                foreach (StackFrame j in frames)
                {
                    if (i >= j.LocalVarPointer && i < j.BasePointer)
                    {
                        isLocal = true;
                        localIdx = (int)(i - j.LocalVarPointer);
                        localMethod = j.Method;
                    }
                    else if (i == j.BasePointer)
                    {
                        isBase = true;
                        baseMethod = j.Method;
                    }
                }
                sb.Append(string.Format("(0x{0:X8}) Type:{1} ", (long)i, i->ObjectType));
                try
                {
                    GetStackObjectText(sb, i, mStack, valuePointerEnd);
                }
                catch
                {
                    sb.Append(" Cannot Fetch Object Info");
                }
                if (i < esp)
                {
                    if (i->ObjectType == ObjectTypes.ValueTypeObjectReference)
                        VisitValueTypeReference(ILIntepreter.ResolveReference(i), leakVObj);
                }
                if (isLocal)
                {
                    sb.Append(string.Format("|Loc:{0}", localIdx));
                    if (localIdx == 0)
                    {
                        sb.Append(" Method:");
                        sb.Append(localMethod.ToString());
                    }
                }
                if (isBase)
                {
                    sb.Append("|Base");
                    sb.Append(" Method:");
                    sb.Append(baseMethod.ToString());
                }

                final.AppendLine(sb.ToString());
            }

            for (StackObject* i = stack.ValueTypeStackBase; i > stack.ValueTypeStackPointer;)
            {
                CLR.TypeSystem.IType vt = domain.GetType(i->Value);
                int cnt = i->ValueLow;
                bool leak = leakVObj.Contains((long)i);
                final.AppendLine("----------------------------------------------");
                final.AppendLine(string.Format("{2}(0x{0:X8}){1}", (long)i, vt, leak ? "*" : ""));
                for (int j = 0; j < cnt; j++)
                {
                    StringBuilder sb = new StringBuilder();
                    StackObject* ptr = Minus(i, j + 1);
                    sb.Append(string.Format("(0x{0:X8}) Type:{1} ", (long)ptr, ptr->ObjectType));
                    GetStackObjectText(sb, ptr, mStack, valuePointerEnd);
                    final.AppendLine(sb.ToString());
                }
                i = Minus(i, i->ValueLow + 1);
            }
            final.AppendLine("Managed Objects:");
            for (int i = 0; i < mStack.Count; i++)
            {
                final.AppendLine(string.Format("({0}){1}", i, mStack[i]));
            }
#if !UNITY_5 && !UNITY_2017_1_OR_NEWER && !UNITY_4
            System.Diagnostics.Debug.Print(final.ToString());
#else
            UnityEngine.Debug.LogWarning(final.ToString());
#endif
        }

        unsafe void GetStackObjectText(StringBuilder sb, StackObject* esp, IList<object> mStack, StackObject* valueTypeEnd)
        {
            string text = "null";
            switch (esp->ObjectType)
            {
                case ObjectTypes.StackObjectReference:
                    {
                        sb.Append(string.Format("Value:0x{0:X8}", (long)ILIntepreter.ResolveReference(esp)));
                    }
                    break;
                case ObjectTypes.ValueTypeObjectReference:
                    {
                        object obj = null;
                        StackObject* dst = ILIntepreter.ResolveReference(esp);
                        try
                        {
                            if (dst > valueTypeEnd)
                                obj = StackObject.ToObject(esp, domain, mStack);
                            if (obj != null)
                                text = obj.ToString();
                        }
                        catch
                        {
                            text = "Invalid Object";
                        }
                        text += string.Format("({0})", domain.GetType(dst->Value));
                    }
                    sb.Append(string.Format("Value:0x{0:X8} Text:{1} ", (long)ILIntepreter.ResolveReference(esp), text));
                    break;
                default:
                    {
                        if (esp->ObjectType >= ObjectTypes.Null && esp->ObjectType <= ObjectTypes.ArrayReference)
                        {
                            if (esp->ObjectType < ObjectTypes.Object || esp->Value < mStack.Count)
                            {
                                object obj = StackObject.ToObject(esp, domain, mStack);
                                if (obj != null)
                                    text = obj.ToString();
                            }
                        }

                        sb.Append(string.Format("Value:{0} ValueLow:{1} Text:{2} ", esp->Value, esp->ValueLow, text));
                    }
                    break;

            }
        }

        unsafe void VisitValueTypeReference(StackObject* esp, HashSet<long> leak)
        {
            leak.Remove((long)esp);
            for (int i = 0; i < esp->ValueLow; i++)
            {
                StackObject* ptr = Minus(esp, i + 1);
                if (ptr->ObjectType == ObjectTypes.ValueTypeObjectReference)
                {
                    VisitValueTypeReference(ILIntepreter.ResolveReference(ptr), leak);
                }
            }
        }
    }
}
