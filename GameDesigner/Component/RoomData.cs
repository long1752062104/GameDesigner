using Net.Share;

namespace Net.Component
{
    /// <summary>
    /// 房间数据信息
    /// </summary>
    public class RoomData
    {
        /// <summary>
        /// 房间名称
        /// </summary>
        public string name;
        /// <summary>
        /// 房间可以组队人数
        /// </summary>
        public int num;
        /// <summary>
        /// 当前加入房间人数
        /// </summary>
        public int currNum;
        /// <summary>
        /// 房间的状态
        /// </summary>
        public NetState state;
        /// <summary>
        /// 竞技模式 1:个人 2:团队
        /// </summary>
        public int mode;
    }

    public class JoinData
    {
        public string name;//玩家名称
        public bool ready;//是否准备
        public bool teamTag;//true:为红队 false:为绿队
        public string iconName;
    }
}