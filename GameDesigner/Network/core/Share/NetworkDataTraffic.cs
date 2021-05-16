namespace Net.Share
{
    /// <summary>
    /// 网络流量数据统计
    /// </summary>
    /// <param name="sendNumber">一秒发送数据次数</param>
    /// <param name="sendCount">一秒发送字节长度</param>
    /// <param name="receiveNumber">一秒接收数据次数</param>
    /// <param name="receiveCount">一秒接收到的字节大小</param>
    /// <param name="resolveNumber">解析RPC函数次数</param>
    /// <param name="sendLoopNum">发送线程循环次数 并发数,类似fps</param>
    /// <param name="revdLoopNum">接收线程循环次数</param>
    public delegate void NetworkDataTraffic(int sendNumber, int sendCount, int receiveNumber, int receiveCount, int resolveNumber, int sendLoopNum, int revdLoopNum);

    /// <summary>
    /// 当处理缓冲区数据
    /// </summary>
    /// <param name="client">处理此客户端的数据请求</param>
    /// <param name="model"></param>
    public delegate void RevdBufferHandle<Player>(Player client, RPCModel model);

    /// <summary>
    /// webSocket当处理缓冲区数据
    /// </summary>
    /// <param name="client">处理此客户端的数据请求</param>
    /// <param name="model"></param>
    public delegate void WSRevdBufferHandle<Player>(Player client, MessageModel model);
}