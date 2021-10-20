namespace Net.Share
{
    using global::System;

    /// <summary>
    /// 标注为远程过程调用函数
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RPCFun : Attribute
    {
        /// <summary>
        /// 网络命令
        /// </summary>
        public byte cmd;
        /// <summary>
        /// 远程方法的掩饰
        /// </summary>
        public ushort mask;
        /// <summary>
        /// <code>如果在unity编译为il2cpp后端脚本并使用CallSiteRpcAdapter快速调用rpc适配器，则需要先声明类型出来，因为编译后，类型将无法在创建出来</code>
        /// 例子: void Test(int num, string str); 则需要使用 [Rpc(il2cpp = typeof(RPCPTR&lt;int, string&gt;))]
        /// </summary>
        public Type il2cpp;

        /// <summary>
        /// 构造RPCFun函数特性
        /// </summary>
        public RPCFun() { }

        /// <summary>
        /// 构造RPCFun函数特性
        /// </summary>
        /// <param name="cmd">自定义的网络命令</param>
        public RPCFun(byte cmd)
        {
            this.cmd = cmd;
        }
    }

    /// <summary>
    /// 标注为远程过程调用函数 (简型)
    /// </summary>
    public class Rpc : RPCFun
    {
        /// <summary>
        /// 构造Rpc函数特性
        /// </summary>
        public Rpc() { }

        /// <summary>
        /// 构造Rpc函数特性
        /// </summary>
        /// <param name="cmd">自定义的网络命令</param>
        public Rpc(byte cmd)
        {
            this.cmd = cmd;
        }
    }

    /// <summary>
    /// 标注为远程过程调用函数 (偷懒型) 安卓上有出现找不到问题, 出现此问题请使用 Rpc(第一个大写) 标签 
    /// </summary>
    public class rpc : RPCFun
    {
        /// <summary>
        /// 构造rpc函数特性
        /// </summary>
        public rpc() { }

        /// <summary>
        /// 构造rpc函数特性
        /// </summary>
        /// <param name="cmd">自定义的网络命令</param>
        public rpc(byte cmd)
        {
            this.cmd = cmd;
        }
    }
}