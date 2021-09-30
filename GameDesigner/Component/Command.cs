namespace Net.Component
{
    /// <summary>
    /// 网络操作指令
    /// </summary>
    public class Command : Share.NetCmd
    {
        /// <summary>
        /// 帧同步操作命令
        /// </summary>
        public const byte SyncOperations = 30;
        /// <summary>
        /// 客户端输入操作指令
        /// </summary>
        public const byte Input = 31;
        /// <summary>
        /// 玩家运动命令
        /// </summary>
        public const byte Movement = 32;
        /// <summary>
        /// 创建玩家命令
        /// </summary>
        public const byte CreatePlayer = 33;
        /// <summary>
        /// 玩家攻击命令
        /// </summary>
        public const byte Attack = 34;
        /// <summary>
        /// 同步生命值
        /// </summary>
        public const byte SyncHealth = 35;
        /// <summary>
        /// 玩家攻击到敌人
        /// </summary>
        public const byte Damage = 36;
        /// <summary>
        /// 敌人怪物AI同步指令
        /// </summary>
        public const byte EnemySync = 37;
        /// <summary>
        /// 玩家切换状态
        /// </summary>
        public const byte SwitchState = 38;
        /// <summary>
        /// 怪物切换状态
        /// </summary>
        public const byte EnemySwitchState = 39;
        /// <summary>
        /// TransformComponent组件测试指令
        /// </summary>
        public const byte Transform = 40;
        /// <summary>
        /// TransformComponent组件被销毁指令
        /// </summary>
        public const byte Destroy = 41;
        /// <summary>
        /// 新版动画同步命令
        /// </summary>
        public const byte Animator = 42;
        /// <summary>
        /// 新版动画参数同步命令
        /// </summary>
        public const byte AnimatorParameter = 43;
        /// <summary>
        /// 旧版动画同步命令
        /// </summary>
        public const byte Animation = 44;
    }
}