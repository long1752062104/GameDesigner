namespace Net.Share
{
    using global::System;

    /// <summary>
    /// 序列化处理程序
    /// </summary>
    public delegate bool SerializeHandle(Type type, object value, out byte[] buffer);

    /// <summary>
    /// 反序列化处理程序
    /// </summary>
    /// <param name="buffer">数据缓冲区</param>
    /// <param name="index">当前解析数据的索引</param>
    /// <param name="type">解析的类型</param>
    /// <returns>返回解析后的对象实例</returns>
    public delegate bool DeserializeHandle(byte[] buffer, ref int index, Type type, out object value);
}
