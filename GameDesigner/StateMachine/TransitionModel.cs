namespace GameDesigner
{
    /// <summary>
    /// 状态过渡事件选项模式
    /// </summary>

    public enum TransitionModel
    {
        /// <summary>
        /// 用时间来跳转状态
        /// </summary>
        ExitTime = 0,
        /// <summary>
        /// 代码控制条件跳转
        /// </summary>
		ScriptControl = 1,
    }
}