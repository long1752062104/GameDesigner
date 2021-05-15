namespace Net.Share
{
    /// <summary>
    /// 传输模式设置
    /// </summary>
    public enum TransportMode
    {
        /// <summary>
        /// 正常模式, 保证可靠数据接近正常得到应答, 降低多次重传的情况
        /// </summary>
        Normal,
        /// <summary>
        /// 快速模式, 速度飞起来, 用流量来换取快速应答
        /// </summary>
        Quick,
        /// <summary>
        /// 双倍加速模式, 重传和应答会以2倍速以上响应
        /// </summary>
        DoubleSpeed,
        /// <summary>
        /// 极快模式, 双倍速度重传与应答
        /// </summary>
        VeryFast,
        /// <summary>
        /// 超速模式, 无法形容的极限速度模式, 流量翻100倍
        /// </summary>
        Speeding
    }
}