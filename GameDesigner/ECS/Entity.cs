using System;
using System.Collections.Generic;

namespace ECS
{
    /// <summary>
    /// ecs实体类, 实体类管理组件的集合 当不使用的时候会丢入system对象池, 给之后创建实体时再次复用
    /// </summary>
    public class Entity : GObject, IIndex
    {
        internal List<Component> components = new List<Component>();
        internal List<IUpdate> updates = new List<IUpdate>();
        internal bool inactive;
        public int Index { get; set; } = -1;

        /// <summary>
        /// 创建实体, 默认是GSystem的单例
        /// </summary>
        public Entity()
        {
            system = GSystem.Instance;
        }

        internal void Execute()
        {
            for (int i = 0; i < updates.Count; i++)
                updates[i].Update();
        }

        /// <summary>
        /// 添加组件, 从system对象池取出对象, 并添加到entity组件列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AddComponent<T>() where T : Component, new()
        {
            T com = system.Take<T>();
            com.entity = this;
            com.system = system;
            components.Add(com);
            com.Awake();
            if (com is IUpdate update)
                updates.Add(update);
            inactive = updates.Count > 0;
            return com;
        }

        public Component AddComponent(Component com)
        {
            com.entity = this;
            com.system = system;
            components.Add(com);
            com.Awake();
            if (com is IUpdate update)
                updates.Add(update);
            inactive = updates.Count > 0;
            return com;
        }

        public T GetComponent<T>() where T : Component
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is T t)
                    return t;
            }
            return null;
        }

        public Component GetComponent(Type comType)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].GetType() == comType)
                    return components[i];
            }
            return null;
        }

        public T[] GetComponents<T>() where T : Component
        {
            List<T> ts = new List<T>();
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is T t)
                    ts.Add(t);
            }
            return ts.ToArray();
        }

        public Component[] GetComponents(Type comType)
        {
            List<Component> ts = new List<Component>();
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].GetType() == comType)
                    ts.Add(components[i]);
            }
            return ts.ToArray();
        }

        public override string ToString()
        {
            string str = "[";
            for (int i = 0; i < components.Count; i++)
            {
                str += components[i].GetType().Name + ",";
            }
            str = str.TrimEnd(',');
            str += "]";
            return $"{GetType().Name} components: {str}";
        }
    }
}
