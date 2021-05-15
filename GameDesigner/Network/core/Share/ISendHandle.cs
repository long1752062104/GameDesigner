using System;
using System.Threading;

namespace Net.Share
{
    /// <summary>
    /// 发送处理程序接口 
    /// 2019.9.23
    /// </summary>
    public interface ISendHandle
    {
        /// <summary>
        /// 发送自定义网络数据
        /// </summary>
        /// <param name="buffer">数据缓冲区</param>
        void Send(byte[] buffer);

        /// <summary>
        /// 发送自定义网络数据
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="buffer">发送字节数组缓冲区</param>
        void Send(byte cmd, byte[] buffer);

        /// <summary>
        /// 发送自定义协议类型, 使用protobuf序列化obj对象
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="obj">使用protobuf序列化的对象</param>
        void Send(byte cmd, object obj);

        /// <summary>
        /// 发送远程过程调用函数数据
        /// </summary>
        /// <param name="func">RPCFun函数</param>
        /// <param name="pars">RPCFun参数</param>
        void Send(string func, params object[] pars);

        /// <summary>
        /// 发送带有网络命令的远程过程调用数据
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="func">RPCFun函数</param>
        /// <param name="pars">RPCFun参数</param>
        void Send(byte cmd, string func, params object[] pars);

        /// <summary>
        /// 远程过程调用 同Send方法
        /// </summary>
        /// <param name="func">Call名</param>
        /// <param name="pars">Call函数</param>
        void CallRpc(string func, params object[] pars);

        /// <summary>
        /// 远程过程调用 同Send方法
        /// </summary>
        /// <param name="cmd">网络命令，请看NetCmd类定义</param>
        /// <param name="func">Call名</param>
        /// <param name="pars">Call函数</param>
        void CallRpc(byte cmd, string func, params object[] pars);

        /// <summary>
        /// 网络请求 同Send方法
        /// </summary>
        /// <param name="func">Call名</param>
        /// <param name="pars">Call函数</param>
        void Request(string func, params object[] pars);

        /// <summary>
        /// 网络请求 同Send方法
        /// </summary>
        /// <param name="cmd">网络命令，请看NetCmd类定义</param>
        /// <param name="func">Call名</param>
        /// <param name="pars">Call函数</param>
        void Request(byte cmd, string func, params object[] pars);

        /// <summary>
        /// 发送网络可靠传输数据, 可以发送大型文件数据
        /// 调用此方法通常情况下是一定把数据发送成功为止
        /// </summary>
        /// <param name="func">函数名</param>
        /// <param name="pars">参数</param>
        void SendRT(string func, params object[] pars);

        /// <summary>
        /// 发送可靠网络传输, 可以发送大型文件数据
        /// 调用此方法通常情况下是一定把数据发送成功为止
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="func">函数名</param>
        /// <param name="pars">参数</param>
        void SendRT(byte cmd, string func, params object[] pars);

        /// <summary>
        /// 发送可靠网络传输, 可发送大数据流
        /// 调用此方法通常情况下是一定把数据发送成功为止
        /// </summary>
        /// <param name="buffer"></param>
        void SendRT(byte[] buffer);

        /// <summary>
        /// 发送可靠网络传输, 可发送大数据流
        /// 调用此方法通常情况下是一定把数据发送成功为止
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="buffer"></param>
        void SendRT(byte cmd, byte[] buffer);

        /// <summary>
        /// 发送自定义协议类型, 使用protobuf序列化obj对象
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="obj">使用protobuf序列化的对象</param>
        void SendRT(byte cmd, object obj);

        /// <summary>
        /// 发送请求, 并且监听服务端的回调请求, 服务器回调请求要对应上发送时的回调匿名, 异步回调, 并且在millisecondsDelay时间内要响应, 否则调用outTimeAct
        /// </summary>
        /// <param name="func">服务器函数名</param>
        /// <param name="funcCB">服务器回调函数名</param>
        /// <param name="callback">回调接收委托</param>
        /// <param name="pars">远程参数</param>
        void Send(string func, string funcCB, Delegate callback, params object[] pars);

        /// <summary>
        /// 发送请求, 并且监听服务端的回调请求, 服务器回调请求要对应上发送时的回调匿名, 异步回调, 并且在millisecondsDelay时间内要响应, 否则调用outTimeAct
        /// </summary>
        /// <param name="func">服务器函数名</param>
        /// <param name="funcCB">服务器回调函数名</param>
        /// <param name="callback">回调接收委托</param>
        /// <param name="millisecondsDelay">异步时间</param>
        /// <param name="pars">远程参数</param>
        void Send(string func, string funcCB, Delegate callback, int millisecondsDelay, params object[] pars);

        /// <summary>
        /// 发送请求, 并且监听服务端的回调请求, 服务器回调请求要对应上发送时的回调匿名, 异步回调, 并且在millisecondsDelay时间内要响应, 否则调用outTimeAct
        /// </summary>
        /// <param name="func">服务器函数名</param>
        /// <param name="funcCB">服务器回调函数名</param>
        /// <param name="callback">回调接收委托</param>
        /// <param name="millisecondsDelay">异步时间</param>
        /// <param name="outTimeAct">异步超时调用</param>
        /// <param name="pars">远程参数</param>
        void Send(string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, params object[] pars);

        /// <summary>
        /// 发送请求, 并且监听服务端的回调请求, 服务器回调请求要对应上发送时的回调匿名, 异步回调, 并且在millisecondsDelay时间内要响应, 否则调用outTimeAct
        /// </summary>
        /// <param name="cmd">指令</param>
        /// <param name="func">服务器函数名</param>
        /// <param name="funcCB">服务器回调函数名</param>
        /// <param name="callback">回调接收委托</param>
        /// <param name="millisecondsDelay">异步时间</param>
        /// <param name="outTimeAct">异步超时调用</param>
        /// <param name="pars">远程参数</param>
        void Send(byte cmd, string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, params object[] pars);

        /// <summary>
        /// 发送请求, 异步回调
        /// </summary>
        /// <param name="cmd">指令</param>
        /// <param name="func">服务器函数名</param>
        /// <param name="funcCB">服务器回调函数名</param>
        /// <param name="callback">回调接收委托</param>
        /// <param name="millisecondsDelay">异步时间</param>
        /// <param name="outTimeAct">异步超时调用</param>
        /// <param name="context">调用上下文线程对象</param>
        /// <param name="pars">远程参数</param>
        void Send(byte cmd, string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, SynchronizationContext context, params object[] pars);

        /// <summary>
        /// 发送请求, 并且监听服务端的回调请求, 服务器回调请求要对应上发送时的回调匿名, 异步回调, 并且在millisecondsDelay时间内要响应, 否则调用outTimeAct
        /// </summary>
        /// <param name="func">服务器函数名</param>
        /// <param name="funcCB">服务器回调函数名</param>
        /// <param name="callback">回调接收委托</param>
        /// <param name="pars">远程参数</param>
        void SendRT(string func, string funcCB, Delegate callback, params object[] pars);

        /// <summary>
        /// 发送请求, 并且监听服务端的回调请求, 服务器回调请求要对应上发送时的回调匿名, 异步回调, 并且在millisecondsDelay时间内要响应, 否则调用outTimeAct
        /// </summary>
        /// <param name="func">服务器函数名</param>
        /// <param name="funcCB">服务器回调函数名</param>
        /// <param name="callback">回调接收委托</param>
        /// <param name="millisecondsDelay">异步时间</param>
        /// <param name="pars">远程参数</param>
        void SendRT(string func, string funcCB, Delegate callback, int millisecondsDelay, params object[] pars);

        /// <summary>
        /// 发送请求, 并且监听服务端的回调请求, 服务器回调请求要对应上发送时的回调匿名, 异步回调, 并且在millisecondsDelay时间内要响应, 否则调用outTimeAct
        /// </summary>
        /// <param name="func">服务器函数名</param>
        /// <param name="funcCB">服务器回调函数名</param>
        /// <param name="callback">回调接收委托</param>
        /// <param name="millisecondsDelay">异步时间</param>
        /// <param name="outTimeAct">异步超时调用</param>
        /// <param name="pars">远程参数</param>
        void SendRT(string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, params object[] pars);

        /// <summary>
        /// 发送请求, 并且监听服务端的回调请求, 服务器回调请求要对应上发送时的回调匿名, 异步回调, 并且在millisecondsDelay时间内要响应, 否则调用outTimeAct
        /// </summary>
        /// <param name="cmd">指令</param>
        /// <param name="func">服务器函数名</param>
        /// <param name="funcCB">服务器回调函数名</param>
        /// <param name="callback">回调接收委托</param>
        /// <param name="millisecondsDelay">异步时间</param>
        /// <param name="outTimeAct">异步超时调用</param>
        /// <param name="pars">远程参数</param>
        void SendRT(byte cmd, string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, params object[] pars);

        /// <summary>
        /// 发送请求, 并且监听服务端的回调请求, 服务器回调请求要对应上发送时的回调匿名, 异步回调, 并且在millisecondsDelay时间内要响应, 否则调用outTimeAct
        /// </summary>
        /// <param name="cmd">指令</param>
        /// <param name="func">服务器函数名</param>
        /// <param name="funcCB">服务器回调函数名</param>
        /// <param name="callback">回调接收委托</param>
        /// <param name="millisecondsDelay">异步时间</param>
        /// <param name="outTimeAct">异步超时调用</param>
        /// <param name="context">调用上下文线程</param>
        /// <param name="pars">远程参数</param>
        void SendRT(byte cmd, string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, SynchronizationContext context, params object[] pars);
    }
}