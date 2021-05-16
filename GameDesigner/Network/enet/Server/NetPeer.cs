using ENet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Server
{
    /// <summary>
    /// enet客户端对象
    /// </summary>
    public class NetPeer : NetPlayer
    {
        /// <summary>
        /// ENet客户端对象
        /// </summary>
        public Peer EClient { get; set; }
        /// <summary>
        /// ENet网络通道ID
        /// </summary>
        public byte ChannelID { get; set; }
    }
}
