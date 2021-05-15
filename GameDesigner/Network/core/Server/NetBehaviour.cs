using Net.Share;
using System.Collections.Generic;
using System.Reflection;

namespace Net.Server
{
    /// <summary>
    /// 服务器网络行为
    /// </summary>
    public class NetBehaviour<Player, Scene> where Player : NetPlayer, new() where Scene : NetScene<Player>, new()
    {
        /// <summary>
        /// 构造网络行为函数
        /// </summary>
        public NetBehaviour() { }

        /// <summary>
        /// 添加所有带有RPCFun特性的方法
        /// </summary>
        /// <param name="target">要收集RPCFun特性的对象</param>
        public static void AddRpcs(object target)
        {
            AddRpcs(ServerBase<Player, Scene>.Instance, false);
        }

        /// <summary>
        /// 添加所有带有RPCFun特性的方法
        /// </summary>
        /// <param name="server"></param>
        /// <param name="target"></param>
        /// <param name="append">追加rpc，如果target类型已经存在还可以追加到rpcs？</param>
        public static void AddRpcs(ServerBase<Player, Scene> server, object target, bool append = false)
        {
            server.AddRpcHandle(target, append);
        }

        /// <summary>
        /// 获取带有RPCFun特性的所有方法
        /// </summary>
        /// <param name="target">要获取RPCFun特性的对象</param>
        /// <returns>返回获取到带有RPCFun特性的所有公开方法</returns>
        public static List<RPCMethod> GetRpcs(object target)
        {
            List<RPCMethod> rpcs = new List<RPCMethod>();
            foreach (MethodInfo info in target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                RPCFun rpc = info.GetCustomAttribute<RPCFun>();
                if (rpc != null)
                    rpcs.Add(new RPCMethod(target, info, rpc.cmd));
            }
            return rpcs;
        }

        /// <summary>
        /// 移除网络远程过程调用函数
        /// </summary>
        /// <param name="server">指定的服务器rpcs</param>
        /// <param name="target">移除的rpc对象</param>
        public static void RemoveRpc(ServerBase<Player, Scene> server, object target)
        {
            server.RemoveRpc(target);
        }
    }
}