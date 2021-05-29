namespace ECS
{
    /// <summary>
    /// 热更层借助此组件进行更新, ilr不支持多继承, 接口继承
    /// </summary>
    public class UpdateComponent : Component, IUpdate
    {
        public virtual void Update()
        {
        }
    }
}
