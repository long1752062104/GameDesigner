#if !CLOSE_ILR
using ILRuntime.Runtime.Intepreter;
using Net.Share;
using Net.System;
using System.Collections.Generic;
using System.Reflection;

namespace Net.Client
{
    public static class ClientBaseExtensions
    {
        /// <summary>
        /// 添加网络Rpc ILRuntime
        /// </summary>
        /// <param name="target">注册的对象实例</param>
        public static void Add_ILR_RpcHandle(this ClientBase self, object target)
        {
            Add_ILR_RpcHandle(self, target, false);
        }

        /// <summary>
        /// 添加网络Rpc ILRuntime
        /// </summary>
        /// <param name="target">注册的对象实例</param>
        /// <param name="append">一个Rpc方法是否可以多次添加到Rpcs里面？</param>
        public static void Add_ILR_RpcHandle(this ClientBase self, object target, bool append)
        {
            if (!append)
            {
                foreach (List<RPCMethod> rpcs in self.RPCsDic.Values)
                {
                    foreach (RPCMethod o in rpcs)
                        if (o.target == target)
                            return;
                }
            }
            ILTypeInstance ilInstace = target as ILTypeInstance;
            global::System.Type type = ilInstace.Type.ReflectionType;
            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (MethodInfo info in methods)
            {
                object[] rpcs = info.GetCustomAttributes(typeof(RPCFun), true);
                if (rpcs.Length > 0)
                {
                    RPCFun rpc = rpcs[0] as RPCFun;
                    RPCMethod item = new RPCMethod(target, info, rpc.cmd);
                    AddRpc(self.RPCsDic, self.RPCs, item);
                }
            }
        }

        private static void AddRpc(MyDictionary<string, List<RPCMethod>> rpcs, List<RPCMethod> rpcsList, RPCMethod item)
        {
            if (!rpcs.ContainsKey(item.method.Name))
                rpcs.Add(item.method.Name, new List<RPCMethod>());
            rpcs[item.method.Name].Add(item);
            rpcsList.Add(item);
        }
    }
}
#endif