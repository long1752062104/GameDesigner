namespace Net.Share
{
    /// <summary>
    /// 网络状态
    /// </summary>
    public enum NetState
    {
        /// <summary>
        /// 待机状态
        /// </summary>
        Idle,
        /// <summary>
        /// 等待其他玩家加入状态
        /// </summary>
        WaitTeam,
        /// <summary>
        /// 组队状态
        /// </summary>
        Team,
        /// <summary>
        /// 作战中，或在游戏中
        /// </summary>
        InCombat,
        /// <summary>
        /// 玩家掉线
        /// </summary>
        Disconnected,
        /// <summary>
        /// 离开状态
        /// </summary>
        AwayStatus,
        /// <summary>
        /// 在游戏厅内，或在场景内
        /// </summary>
        InGameHall,
        /// <summary>
        /// 在游戏厅外，或在场景外
        /// </summary>
        OutGameHall,
    }
}
