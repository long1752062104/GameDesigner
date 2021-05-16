using Net.Share;
using System;
using System.Collections.Generic;
using System.Reflection;
using Net.Server;
using System.Threading;

namespace Net.Adapter
{
    internal class RPCPTR
    {
        internal object target;
        internal byte cmd;
        public virtual void Invoke(object[] pars) {}
    }
    internal class RPCPTRNull : RPCPTR
    {
        internal Action ptr;
        public override void Invoke(object[] pars)
        {
            ptr();
        }
    }
    internal class RPCPTR<T> : RPCPTR
    {
        internal Action<T> ptr;
        public override void Invoke(object[] pars)
        {
            ptr((T)pars[0]);
        }
    }
    internal class RPCPTR<T, T1> : RPCPTR
    {
        internal Action<T, T1> ptr;
        public override void Invoke(object[] pars)
        {
            ptr((T)pars[0], (T1)pars[1]);
        }
    }
    internal class RPCPTR<T, T1, T2> : RPCPTR
    {
        internal Action<T, T1, T2> ptr;
        public override void Invoke(object[] pars)
        {
            ptr((T)pars[0], (T1)pars[1], (T2)pars[2]);
        }
    }
    internal class RPCPTR<T, T1, T2, T3> : RPCPTR
    {
        internal Action<T, T1, T2, T3> ptr;
        public override void Invoke(object[] pars)
        {
            ptr((T)pars[0], (T1)pars[1], (T2)pars[2], (T3)pars[3]);
        }
    }
    internal class RPCPTR<T, T1, T2, T3, T4> : RPCPTR
    {
        internal Action<T, T1, T2, T3, T4> ptr;
        public override void Invoke(object[] pars)
        {
            ptr((T)pars[0], (T1)pars[1], (T2)pars[2], (T3)pars[3], (T4)pars[4]);
        }
    }
    internal class RPCPTR<T, T1, T2, T3, T4, T5> : RPCPTR
    {
        internal Action<T, T1, T2, T3, T4, T5> ptr;
        public override void Invoke(object[] pars)
        {
            ptr((T)pars[0], (T1)pars[1], (T2)pars[2], (T3)pars[3], (T4)pars[4], (T5)pars[5]);
        }
    }
    internal class RPCPTR<T, T1, T2, T3, T4, T5, T6> : RPCPTR
    {
        internal Action<T, T1, T2, T3, T4, T5, T6> ptr;
        public override void Invoke(object[] pars)
        {
            ptr((T)pars[0], (T1)pars[1], (T2)pars[2], (T3)pars[3], (T4)pars[4], (T5)pars[5], (T6)pars[6]);
        }
    }
    internal class RPCPTR<T, T1, T2, T3, T4, T5, T6, T7> : RPCPTR
    {
        internal Action<T, T1, T2, T3, T4, T5, T6, T7> ptr;
        public override void Invoke(object[] pars)
        {
            ptr((T)pars[0], (T1)pars[1], (T2)pars[2], (T3)pars[3], (T4)pars[4], (T5)pars[5], (T6)pars[6], (T7)pars[7]);
        }
    }
    internal class RPCPTR<T, T1, T2, T3, T4, T5, T6, T7, T8> : RPCPTR
    {
        internal Action<T, T1, T2, T3, T4, T5, T6, T7, T8> ptr;
        public override void Invoke(object[] pars)
        {
            ptr((T)pars[0], (T1)pars[1], (T2)pars[2], (T3)pars[3], (T4)pars[4], (T5)pars[5], (T6)pars[6], (T7)pars[7], (T8)pars[8]);
        }
    }
    internal class RPCPTR<T, T1, T2, T3, T4, T5, T6, T7, T8, T9> : RPCPTR
    {
        internal Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9> ptr;
        public override void Invoke(object[] pars)
        {
            ptr((T)pars[0], (T1)pars[1], (T2)pars[2], (T3)pars[3], (T4)pars[4], (T5)pars[5], (T6)pars[6], (T7)pars[7], (T8)pars[8], (T9)pars[9]);
        }
    }
    internal class RPCPTR<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : RPCPTR
    {
        internal Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> ptr;
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

        public CallSiteRpcAdapter() { Context = SynchronizationContext.Current; }

        public void AddRpcHandle(object target, bool append)
        {
            Type type = target.GetType();
            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (MethodInfo info in methods)
            {
                RPCFun rpc = info.GetCustomAttribute<RPCFun>();
                if (rpc != null)
                {
                    if (info.ReturnType != typeof(void))
                        throw new Exception("rpc函数不允许有返回值，也没必要!");
                    var pars = info.GetParameters();
                    RPCPTR metPtr;
                    if (pars.Length > 0)
                    {
                        List<Type> parTypes = new List<Type>();
                        foreach (var par in pars) parTypes.Add(par.ParameterType);
                        var type2 = Type.GetType($"Net.Adapter.RPCPTR`{parTypes.Count}");
                        var gt = type2.MakeGenericType(parTypes.ToArray());
                        metPtr = (RPCPTR)Activator.CreateInstance(gt);
                    }
                    else metPtr = new RPCPTRNull();
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
            if (string.IsNullOrEmpty(model.func))
                return;
            if (RPCS.TryGetValue(model.func, out RPCPTR model1))
            {
                if (Context != null)
                    Context.Post((obj) => { model1.Invoke(model.pars); }, null);
                else
                    model1.Invoke(model.pars);
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