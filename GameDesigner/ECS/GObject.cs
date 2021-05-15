namespace ECS
{
    public class GObject
    {
        internal int hashCode;

        public GObject()
        {
            hashCode = GetType().GetHashCode();
        }

        public virtual void Awake() { }
        public virtual void OnDestroy() { }

        /// <summary>
        /// 移除实体或组件, 并将对象扔进system对象池, 当AddComponent或Create时可以复用
        /// </summary>
        /// <param name="gObject"></param>
        /// <param name="reuse">此对象可以复用? 复用会将对象丢入system对象池, 等待再次使用. 如果是entity对象, 并且不复用, 则把entity所使用的组件也同样不再复用</param>
        public static void Destroy(GObject gObject, bool reuse = true)
        {
            if (gObject is Component component)
            {
                component.entity.components.Remove(component);
                if (component is IUpdate update)
                    component.entity.updates.Remove(update);
                if (reuse) component.entity.system.Push(component);
                gObject.OnDestroy();
                return;
            }
            else if (gObject is Entity entity)
            {
                entity.system.RemoveEntity(entity);
                entity.OnDestroy();
                while (entity.components.Count > 0)
                {
                    Component component1 = entity.components[0];
                    entity.components.RemoveAt(0);
                    component1.OnDestroy();
                    if (reuse) entity.system.Push(component1);
                }
                entity.updates.Clear();
                if (reuse) entity.system.Push(entity);
            }
        }
    }
}
