using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;

namespace Udx
{
    public delegate void UDXPRC(UDXEVENT_TYPE eventtype, int erro, IntPtr s, IntPtr pData, int len);
    public delegate int UInputUdxData(EndPoint from, byte[] pData, int len);//填充from和data,len为pData提供的缓冲长度
    public delegate void UDXP2PPRC(string user1, string user2, long dwuser);
    public delegate int UOutputUdxData(EndPoint adrrto, byte[] pData, int len);//发送地址及待发送的数据及长度

    public enum UDXEVENT_TYPE
    {
        E_CONNECT,//联接
        E_MSGREAD,//读消息
        E_MSGWRITE,//写消息
        E_DATAREAD,//数据到来
        E_DATAWRITE,//发送数据成功
        E_LINKBROKEN,//联接断开
        E_LINKTIMER,//每个联接，约50MS调用一次的timer
        E_FILE_BEGIN,//有新文件要发过来
        E_FILE_END,//文件传输完成
        E_FILE_CANCEL,//文件传输被取消
        E_LINK_NOTIFY,//拥赛消息
    };

    public struct UFrameType
    {
        public byte type;//音视频，数据// AUDIOFRAME_A~DATAFRAME_I 0~3可填
        public byte subtype;//子类型，type2 = FTypeVer_1时帧头变成FrameType2 0~6 可填
        public byte modestream;//是否是流式传输0~1可填
        public byte nuse;//0~1可填
        public int sid;//流ID
        public int sbid;//子流ID,short类型
    };

    public struct UFileInfo
    {
        public long len;
        public string name;
    };

    public interface IUDX { }

    public class UdxLib
    {
#if __IOS__ || UNITY_IOS && !UNITY_EDITOR
		public const string nativeLibrary = "__Internal";
#elif UNITY_ANDROID && !UNITY_EDITOR//宏编译 在安卓编译就是这里
		public const string nativeLibrary = "udxapi";
#else //在win, unity编辑器
        public const string nativeLibrary = "FastUdxApi.dll";
#endif
        public const int AUDIOFRAME_A = 0;//音频帧
        public const int VIDEOFRAME_I = 1;//I帧
        public const int VIDEOFRAME_P = 2;//P帧
        public const int DATAFRAME_I = 3;//数据帧

        public static bool INIT;
        public static HashSet<IUDX> UDXS = new HashSet<IUDX>();

        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern void UInit(int threadcout);//UDXAPI初始化，threadcout UDX事件回调事件线程数，如果是客户端，一般设置为2，服务器时CPU*2+2

        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr UCreateFUObj(); //创建一个UDXOBJ对象	

        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern int UBind(IntPtr fu, string ip, int port);//给UDXOBJ对象，设置绑定的本地IP,PORT	

        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern void UDestroyFUObj(IntPtr fu);     //删除这个udxobj对象

        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern void USetFUCB(IntPtr fu, UDXPRC cb);//设置udxobj对象的回调函授，包括了，联接，收发，断开事件

        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr UConnect(IntPtr fu, string ip, int port, long dwUser, bool bSync, int linktype, string strChannel = null);   //向远程主机发起一条连接

        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern void UGetLocalAddr(IntPtr fu, byte[] ip, ref int port, ref int ntype);

        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static unsafe extern int USend(IntPtr s, byte* pData, int len);//对建立的一条连接s发起一个，发送动作

        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern int UMsgSend(IntPtr s, IntPtr pData, int len);//对建立的一条连接s发起一个，发送动作

        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern int UDSend(IntPtr s, IPAddress pAddr, IntPtr pData, int len);//直接向指定地址发送非可靠数据

        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UIsConnect(IntPtr s);       //判断，当前的联接s对象，是否是在联接状态

        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UIsFullBuffs(IntPtr s); //判断，当前联接,是否已经缓冲满，如果满了，请调用wait或sleep，等待成为非满状态

        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern void USetUserData(IntPtr s, long i64User);  //与联接，关联一个用户自定义的数据对象

        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern long UGetUserData(IntPtr s);//获取，之前与这个联接关联的，用户自定义的数据对象

        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern int UWait(IntPtr s, int ms); //等联接对象到可发状态

        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern long UDump(IntPtr s);//复制这个联接对象，增加引用。

        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern long UUndump(IntPtr s);//复制这个联接对象，减小引用。
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern void UClose(IntPtr s);   //关掉当前联接，断开
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr UGetFUObj(IntPtr s);//得到根UDX对象
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern void UGetRemoteAddr(IntPtr s, byte[] ip, ref int port, ref int ntype);//得到远程地址及IPV4 OR 6
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern void UGetSpeedStr(IntPtr s, string buff, bool bSend = true, bool bCurrent = false);           //得到实时/平均速度,字符串
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UIsP2p(IntPtr s);
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UIsIPV6(IntPtr fu);
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern int UGetLinkType(IntPtr s);//得到联接类型
                                                        //UDX侦模式发送音视频及其他数据
                                                        //每一侦数据，被 UDX增加了一个UdxFrameType包头，用于丢侦处理，及包的识别
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern void USendFrame(IntPtr s, uint sid, ushort sbid, byte[] pData, int len, int type1/* AUDIOFRAME_A or VIDEOFRAME_I or VIDEOFRAME_P */, int type2, byte JumpFrameMode = 0);

        //   =========================== 发送文件接口 ========================
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern void USendFile(IntPtr s, string szFileName, long expectbew);//发送文件
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UIsSendDone(IntPtr s, bool bSend);
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern float UGetPerCenter(IntPtr s, bool bSend);
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern void USetSaveDIR(IntPtr s, string szDir);
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern void USetSaveFileName(IntPtr s, string szFile);
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern void UCancel(IntPtr s, bool bSend);


        //   =========================== p2p接口 ========================
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr UGetP2pClient(IntPtr fu); //通过已经绑定的接口得到P2P接口
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern void USetP2PCB(IntPtr p2pu, UDXP2PPRC cb);//设置udxobj对象的回调函授，包括了，联接，收发，断开事件
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern void USetNatServer(IntPtr p2pu, byte[] mip, long mport);
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UP2pConnectToDesUser(IntPtr p2pu, string user1, string user2, long dwUser, bool bCaller, int linktype);
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern void UP2PSetTimeOut(IntPtr p2pu, int ms);

        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern void UP2PMsgConnectRegister(IntPtr fu, string reg_ip, int reg_port, string selfSN, string desSN, string p2p_ip, int p2p_port, string ts_ip, int ts_port, ushort index);
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern void UP2PTo(IntPtr s, string desSN);


        //中转接口
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern void UOpenChannel(IntPtr fu, string ip, short port, string strCName);        //打开一个中转通道,通道名不得等于或超过25个字符
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern void UCloseChannel(IntPtr fu, string ip, short port, string strCName);       //关掉一个中转通道

        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern int UGetStreamID(IntPtr s);//本地唯一标识
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern int UGetDesStreamID(IntPtr s);//远程标识

        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern int UGetHead(byte[] pData, ref UFrameType pHead);//通过缓冲得到包头
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern void UUnInit();

        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern int UGetTSSessionCount(IntPtr fu);


        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern void USetFUIOBase(IntPtr fu, UInputUdxData cbin, UOutputUdxData cbout);//设置udxobj对象的收发接口函数
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern int UGetLinkIndex(IntPtr s);//得到联接序号
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern void USetConnectTimeout(IntPtr fu, int linkConnectTime);
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern void USetLinkTimeout(IntPtr s, int con, int hardbeat, int contimeout);
        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern int UGetLinkRtt(IntPtr s);

        [DllImport(nativeLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern void USetGameMode(IntPtr s, bool bGameMode);
    }
}
