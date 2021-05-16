using System;
using System.Runtime.InteropServices;

namespace Kcp
{
    public unsafe delegate int outputCallback(byte* buf, int len, IntPtr kcp, IntPtr user);

    public unsafe delegate IntPtr ikcp_malloc_hook(int size);

    public unsafe delegate void ikcp_free_hook(IntPtr ptr);

    public unsafe class KcpLib
    {
        public const string nativeLibrary = "kcp.dll";

        // create a new kcp control object, 'conv' must equal in two endpoint
        // from the same connection. 'user' will be passed to the output callback
        // output callback can be setup like this: 'kcp->output = my_udp_output'
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ikcp_create(uint conv, IntPtr user);

        // release kcp control object
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ikcp_release(IntPtr kcp);

        // set output callback, which will be invoked by kcp
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ikcp_setoutput(IntPtr kcp, IntPtr output);

        // user/upper level recv: returns size, returns below zero for EAGAIN
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ikcp_recv(IntPtr kcp, byte* buffer, int len);

        // user/upper level send, returns below zero for error
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ikcp_send(IntPtr kcp, byte* buffer, int len);

        // update state (call it repeatedly, every 10ms-100ms), or you can ask 
        // ikcp_check when to call it again (without ikcp_input/_send calling).
        // 'current' - current timestamp in millisec. 
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ikcp_update(IntPtr kcp, uint current);

        // Determine when should you invoke ikcp_update:
        // returns when you should invoke ikcp_update in millisec, if there 
        // is no ikcp_input/_send calling. you can call ikcp_update in that
        // time, instead of call update repeatly.
        // Important to reduce unnacessary ikcp_update invoking. use it to 
        // schedule ikcp_update (eg. implementing an epoll-like mechanism, 
        // or optimize ikcp_update when handling massive kcp connections)
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ikcp_check(IntPtr kcp, uint current);

        // when you received a low level packet (eg. UDP packet), call it
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ikcp_input(IntPtr kcp, byte* data, long size);

        // flush pending data
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ikcp_flush(IntPtr kcp);

        // check the size of next message in the recv queue
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)] 
        public static extern int ikcp_peeksize(IntPtr kcp);

        // change MTU size, default is 1400
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ikcp_setmtu(IntPtr kcp, int mtu);

        // set maximum window size: sndwnd=32, rcvwnd=32 by default
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)] 
        public static extern int ikcp_wndsize(IntPtr kcp, int sndwnd, int rcvwnd);

        // get how many packet is waiting to be sent
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)] 
        public static extern int ikcp_waitsnd(IntPtr kcp);

        // fastest: ikcp_nodelay(kcp, 1, 20, 2, 1)
        // nodelay: 0:disable(default), 1:enable
        // interval: internal update timer interval in millisec, default is 100ms 
        // resend: 0:disable fast resend(default), 1:enable fast resend
        // nc: 0:normal congestion control(default), 1:disable congestion control
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ikcp_nodelay(IntPtr kcp, int nodelay, int interval, int resend, int nc);

        [DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ikcp_log(IntPtr kcp, int mask, byte* fmt, params object[] p);

        // setup allocator
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ikcp_allocator(IntPtr new_malloc, IntPtr new_free);

        // read conv
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ikcp_getconv(IntPtr ptr);
    }
}
