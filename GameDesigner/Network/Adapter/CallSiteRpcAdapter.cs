using Net.Share;
using System;
using System.Reflection;
using Net.Server;
using System.Threading;
using Net.Event;
using Net.System;

namespace Net.Adapter
{
    public class RPCPTR
    {
        public object target;
        public byte cmd;
        public virtual void Invoke(object[] pars) {}
    }
    public class RPCPTRMethod : RPCPTR
    {
        public MethodInfo method;
        public override void Invoke(object[] pars)
        {
            method.Invoke(target, pars);
        }
    }
    public class RPCPTRNull : RPCPTR
    {
        public Action ptr;
        public override void Invoke(object[] pars)
        {
            ptr();
        }
    }
    public class RPCPTR<T> : RPCPTR
    {
        public Action<T> ptr;
        public override void Invoke(object[] pars)
        {
            ptr((T)pars[0]);
        }
    }
    public class RPCPTR<T, T1> : RPCPTR
    {
        public Action<T, T1> ptr;
        public override void Invoke(object[] pars)
        {
            ptr((T)pars[0], (T1)pars[1]);
        }
    }
    public class RPCPTR<T, T1, T2> : RPCPTR
    {
        public Action<T, T1, T2> ptr;
        public override void Invoke(object[] pars)
        {
            ptr((T)pars[0], (T1)pars[1], (T2)pars[2]);
        }
    }
    public class RPCPTR<T, T1, T2, T3> : RPCPTR
    {
        public Action<T, T1, T2, T3> ptr;
        public override void Invoke(object[] pars)
        {
            ptr((T)pars[0], (T1)pars[1], (T2)pars[2], (T3)pars[3]);
        }
    }
    public class RPCPTR<T, T1, T2, T3, T4> : RPCPTR
    {
        public Action<T, T1, T2, T3, T4> ptr;
        public override void Invoke(object[] pars)
        {
            ptr((T)pars[0], (T1)pars[1], (T2)pars[2], (T3)pars[3], (T4)pars[4]);
        }
    }
    public class RPCPTR<T, T1, T2, T3, T4, T5> : RPCPTR
    {
        public Action<T, T1, T2, T3, T4, T5> ptr;
        public override void Invoke(object[] pars)
        {
            ptr((T)pars[0], (T1)pars[1], (T2)pars[2], (T3)pars[3], (T4)pars[4], (T5)pars[5]);
        }
    }
    public class RPCPTR<T, T1, T2, T3, T4, T5, T6> : RPCPTR
    {
        public Action<T, T1, T2, T3, T4, T5, T6> ptr;
        public override void Invoke(object[] pars)
        {
            ptr((T)pars[0], (T1)pars[1], (T2)pars[2], (T3)pars[3], (T4)pars[4], (T5)pars[5], (T6)pars[6]);
        }
    }
    public class RPCPTR<T, T1, T2, T3, T4, T5, T6, T7> : RPCPTR
    {
        public Action<T, T1, T2, T3, T4, T5, T6, T7> ptr;
        public override void Invoke(object[] pars)
        {
            ptr((T)pars[0], (T1)pars[1], (T2)pars[2], (T3)pars[3], (T4)pars[4], (T5)pars[5], (T6)pars[6], (T7)pars[7]);
        }
    }
    public class RPCPTR<T, T1, T2, T3, T4, T5, T6, T7, T8> : RPCPTR
    {
        public Action<T, T1, T2, T3, T4, T5, T6, T7, T8> ptr;
        public override void Invoke(object[] pars)
        {
            ptr((T)pars[0], (T1)pars[1], (T2)pars[2], (T3)pars[3], (T4)pars[4], (T5)pars[5], (T6)pars[6], (T7)pars[7], (T8)pars[8]);
        }
    }
    public class RPCPTR<T, T1, T2, T3, T4, T5, T6, T7, T8, T9> : RPCPTR
    {
        public Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9> ptr;
        public override void Invoke(object[] pars)
        {
            ptr((T)pars[0], (T1)pars[1], (T2)pars[2], (T3)pars[3], (T4)pars[4], (T5)pars[5], (T6)pars[6], (T7)pars[7], (T8)pars[8], (T9)pars[9]);
        }
    }
    public class RPCPTR<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : RPCPTR
    {
        public Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> ptr;
        public override void Invoke(object[] pars)
        {
            ptr((T)pars[0], (T1)pars[1], (T2)pars[2], (T3)pars[3], (T4)pars[4], (T5)pars[5], (T6)pars[6], (T7)pars[7], (T8)pars[8], (T9)pars[9], (T10)pars[10]);
        }
    }

    /// <summary>
    /// 服务器远程过程调用适配器
    /// </summary>
    /// <typeparam name="Player"></typeparam>
    public class CallSiteRpcAdapter<Player> : CallSiteRpcAdapter, IRPCAdapter<Player> where Player : NetPlayer
    {
        public void OnRpcExecute(Player client, RPCModel model)
        {
            if (model.methodMask != 0)
                if (!RpcMask.TryGetValue(model.methodMask, out model.func)) model.func = $"[mask:{model.methodMask}]";
            if (string.IsNullOrEmpty(model.func))
                return;
            if (RPCS.TryGetValue(model.func, out RPCPTR model1))
            {
                if (model1.cmd == NetCmd.SafeCall)
                {
                    object[] pars = new object[model.pars.Length + 1];
                    pars[0] = client;
                    Array.Copy(model.pars, 0, pars, 1, model.pars.Length);
                    model1.Invoke(pars);
                }
                else model1.Invoke(model.pars);
            }
        }
    }

    /// <summary>
    /// 客户端远程过程调用适配器
    /// </summary>
    public class CallSiteRpcAdapter : IRPCAdapter
    {
        internal SynchronizationContext Context;
        internal MyDictionary<string, RPCPTR> RPCS = new MyDictionary<string, RPCPTR>();
        internal MyDictionary<ushort, string> RpcMask = new MyDictionary<ushort, string>();
        internal MyDictionary<string, RPCModelTask> rpcTasks = new MyDictionary<string, RPCModelTask>();
#if UNITY_EDITOR
        private readonly bool useIL2CPP;
#endif
        public CallSiteRpcAdapter()
        {
            Context = SynchronizationContext.Current;
#if UNITY_EDITOR
#pragma warning disable CS0618 // 类型或成员已过时
            useIL2CPP = UnityEditor.PlayerSettings.GetPropertyInt("ScriptingBackend", UnityEditor.BuildTargetGroup.Standalone) == 1;
#pragma warning restore CS0618 // 类型或成员已过时
#endif
        }

        public void AddRpcHandle(object target, bool append)
        {
            Type type = target.GetType();
            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (MethodInfo info in methods)
            {
                RPCFun rpc = info.GetCustomAttribute<RPCFun>();
                if (rpc != null)
                {
                    if (rpc.mask != 0)
                    {
                        if (!RpcMask.TryGetValue(rpc.mask, out string func))
                            RpcMask.Add(rpc.mask, info.Name);
                        else if (func != info.Name)
                            NDebug.LogError($"错误! 请修改Rpc方法{info.Name}或{func}的mask值, mask值必须是唯一的!");
                    }
                    if (info.ReturnType != typeof(void))
                    {
                        RPCPTRMethod met1 = new RPCPTRMethod
                        {
                            target = target,
                            method = info,
                            cmd = rpc.cmd
                        };
                        RPCS.Add(info.Name, met1);
                        continue;
                    }
                    var pars = info.GetParameters();
#if UNITY_EDITOR
                    if (rpc.il2cpp == null & useIL2CPP)
                        throw new Exception("如果在unity编译为il2cpp后端脚本，则需要先声明类型出来，因为编译后，类型被固定，将无法创建出来! 例子: void Test(int num, string str); 则需要这样添加 [Rpc(il2cpp = typeof(RPCPTR<int, string>))]");
                    if (useIL2CPP) 
                    {
                        var pars1 = rpc.il2cpp.GetGenericArguments();
                        if (pars.Length != pars1.Length)
                            throw new Exception($"{type}类的:{info.Name}方法定义Rpc的参数长度不一致!");
                        for (int i = 0; i < pars.Length; i++)
                            if(pars[i].ParameterType != pars1[i])
                                throw new Exception($"{type}类的:{info.Name}方法定义Rpc的参数类型不一致!");
                    }
#endif
                    Type[] parTypes = new Type[pars.Length];
                    for (int i = 0; i < pars.Length; i++)
                        parTypes[i] = pars[i].ParameterType;
                    Type gt = null;
                    switch (parTypes.Length)
                    {
                        case 0:
                            gt = typeof(RPCPTRNull);
                            break;
                        case 1:
                            gt = typeof(RPCPTR<>).MakeGenericType(parTypes);
                            break;
                        case 2:
                            gt = typeof(RPCPTR<,>).MakeGenericType(parTypes);
                            break;
                        case 3:
                            gt = typeof(RPCPTR<,,>).MakeGenericType(parTypes);
                            break;
                        case 4:
                            gt = typeof(RPCPTR<,,,>).MakeGenericType(parTypes);
                            break;
                        case 5:
                            gt = typeof(RPCPTR<,,,,>).MakeGenericType(parTypes);
                            break;
                        case 6:
                            gt = typeof(RPCPTR<,,,,,>).MakeGenericType(parTypes);
                            break;
                        case 7:
                            gt = typeof(RPCPTR<,,,,,,>).MakeGenericType(parTypes);
                            break;
                        case 8:
                            gt = typeof(RPCPTR<,,,,,,,>).MakeGenericType(parTypes);
                            break;
                        case 9:
                            gt = typeof(RPCPTR<,,,,,,,,>).MakeGenericType(parTypes);
                            break;
                        case 10:
                            gt = typeof(RPCPTR<,,,,,,,,,>).MakeGenericType(parTypes);
                            break;
                        case 11:
                            gt = typeof(RPCPTR<,,,,,,,,,,>).MakeGenericType(parTypes);
                            break;
                    }
                    RPCPTR metPtr = (RPCPTR)Activator.CreateInstance(gt);
                    var ptr = metPtr.GetType().GetField("ptr", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                    var met = Delegate.CreateDelegate(ptr.FieldType, target, info);
                    ptr.SetValue(metPtr, met);
                    metPtr.target = target;
                    metPtr.cmd = rpc.cmd;
                    RPCS.Add(info.Name, metPtr);
                }
            }
        }

        public void OnRpcExecute(RPCModel model)
        {
            if (model.methodMask != 0)
                if (!RpcMask.TryGetValue(model.methodMask, out model.func)) model.func = $"[mask:{model.methodMask}]";
            if (string.IsNullOrEmpty(model.func))
                return;
            if (rpcTasks.TryGetValue(model.func, out RPCModelTask model1))
            {
                model1.model = model;
                model1.IsCompleted = true;
                rpcTasks.Remove(model.func);
                return;
            }
            if (RPCS.TryGetValue(model.func, out RPCPTR model2))
            {
                if (Context != null)
                    Context.Post((obj) => { model2.Invoke(model.pars); }, null);
                else
                    model2.Invoke(model.pars);
            }
        }

        public void RemoveRpc(object target)
        {
            if (target is string key)
            {
                if (RPCS.ContainsKey(key))
                    RPCS.Remove(key);
                return;
            }
            var entries = RPCS.entries;
            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i].hashCode == 0)
                    continue;
                var rpc = entries[i].value;
                if (rpc == null)
                    continue;
                if (rpc.target == null | rpc.target == target)
                {
                    RPCS.Remove(entries[i].key);
                    continue;
                }
                if (rpc.target.Equals(null) | rpc.target.Equals(target))
                {
                    RPCS.Remove(entries[i].key);
                }
            }
        }

        public void CheckRpcUpdate()
        {
            var entries = RPCS.entries;
            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i].hashCode == 0)
                    continue;
                var rpc = entries[i].value;
                if (rpc == null)
                    continue;
                if (rpc.target == null)
                {
                    RPCS.Remove(entries[i].key);
                    continue;
                }
                if (rpc.target.Equals(null))
                {
                    RPCS.Remove(entries[i].key);
                }
            }
        }
    }
}