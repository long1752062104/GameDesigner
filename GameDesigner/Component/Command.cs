namespace Net.Component
{
    /// <summary>
    /// 网络操作指令 (系统命令使用0-100, 基础网络组件使用100-150) 请从150开始自定义命令
    /// </summary>
    public class Command : Share.NetCmd
    {
        /// <summary>
        /// 客户端输入操作指令
        /// </summary>
        public const byte Input = 100;
        /// <summary>
        /// 玩家运动命令
        /// </summary>
        public const byte Movement = 101;
        /// <summary>
        /// 创建玩家命令
        /// </summary>
        public const byte CreatePlayer = 102;
        /// <summary>
        /// 玩家攻击命令
        /// </summary>
        public const byte Attack = 103;
        /// <summary>
        /// 同步生命值
        /// </summary>
        public const byte SyncHealth = 104;
        /// <summary>
        /// 玩家攻击到敌人
        /// </summary>
        public const byte Damage = 105;
        /// <summary>
        /// 敌人怪物AI同步指令
        /// </summary>
        public const byte EnemySync = 106;
        /// <summary>
        /// 玩家切换状态
        /// </summary>
        public const byte SwitchState = 107;
        /// <summary>
        /// 怪物切换状态
        /// </summary>
        public const byte EnemySwitchState = 108;
        /// <summary>
        /// Transform同步指令
        /// </summary>
        public const byte Transform = 109;
        /// <summary>
        /// NetworkIdentity组件被销毁指令
        /// </summary>
        public const byte Destroy = 110;
        /// <summary>
        /// 当客户端退出游戏, 通知其他客户端删除此客户端所生成的NetworkIdentity物体
        /// </summary>
        public const byte OnPlayerExit = 114;
        /// <summary>
        /// 网络组件生成工具同步指令
        /// </summary>
        public const byte BuildComponent = 115;
    }
}