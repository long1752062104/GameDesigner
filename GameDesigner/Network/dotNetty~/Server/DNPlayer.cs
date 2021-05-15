using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Server
{
    /// <summary>
    /// dotNetty客户端玩家对象
    /// </summary>
    public class DNPlayer : NetPlayer
    {
        /// <summary>
        /// 此客户端的网络通道
        /// </summary>
        internal IChannel channel;

        public override void Dispose()
        {
            if (isDispose)
                return;
            base.Dispose();
            channel?.CloseAsync();
        }
    }
}
