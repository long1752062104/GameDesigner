namespace Net.Share
{
    using global::System;
    using global::System.Net.Sockets;
    using global::System.Runtime.InteropServices;

    /// <summary>
    /// 系统时钟间隔是个很少被关心到的系统标量，
    /// 它反映了系统产生时钟中断的频率，间隔越小频率越高，反之亦然。
    /// 每当时钟中断产生，系统相关的中断函数将会处理这个中断。
    /// 时钟中断处理函数会更新系统时间，检查内核调试信息等。
    /// </summary>
    public static class Win32KernelAPI
    {
        /// <summary>
        /// 设置应用程序或驱动程序使用的最小定时器分辨率  
        /// </summary>
        /// <param name="uMilliseconds"></param>
        /// <returns></returns>
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern uint timeBeginPeriod(uint uMilliseconds);

        /// <summary>
        /// 清除应用程序或驱动程序使用的最小定时器分辨率  
        /// </summary>
        /// <param name="uMilliseconds"></param>
        /// <returns></returns>
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern uint timeEndPeriod(uint uMilliseconds);

        [DllImport("ws2_32.dll", SetLastError = true)]
        public unsafe static extern int sendto([In] IntPtr socketHandle, [In] byte* pinnedBuffer, [In] int len, [In] SocketFlags socketFlags, [In] byte[] socketAddress, [In] int socketAddressSize);

        [DllImport("ws2_32.dll", SetLastError = true)]
        public unsafe static extern int send([In] IntPtr socketHandle, [In] byte* pinnedBuffer, [In] int len, [In] SocketFlags socketFlags);
    }
}
