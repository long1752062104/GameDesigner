using Net.Share;
using Net.System;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECS
{
    /// <summary>
    /// ecs系统, 此系统管理entity和conponent对象池(复用)
    /// </summary>
    [Serializable]
    public class GSystem : IDisposable
    {
        /// <summary>
        /// system的单例对象
        /// </summary>
        public static GSystem Instance = new GSystem();
        private readonly MyDictionary<int, Stack<GObject>> objectPool = new MyDictionary<int, Stack<GObject>>();
        internal readonly ArrayPool<Entity> entities = new ArrayPool<Entity>();
        private bool isDispose;

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity"></param>
        public void AddEntity(Entity entity)
        {
            entity.system = this;
            entity.Awake();
            entities.Add(entity);
        }

        internal void RemoveEntity(Entity entity)
        {
            entities.Remove(entity);
        }

        /// <summary>
        /// 取出实体组件, 复用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Take<T>() where T : GObject, new()
        {
            int type = typeof(T).GetHashCode();
            if (objectPool.TryGetValue(type, out Stack<GObject> queue))
            {
                if (queue.Count > 0)
                    return (T)queue.Pop();
                goto J;
            }
            queue = new Stack<GObject>();
            objectPool.Add(type, queue);
        J: T obj = new T();
            return obj;
        }

        /// <summary>
        /// 压入实体组件, 后面复用
        /// </summary>
        /// <param name="gObject"></param>
        public void Push(GObject gObject)
        {
            int type = gObject.hashCode;
            if (objectPool.TryGetValue(type, out Stack<GObject> queue))
            {
                queue.Push(gObject);
                return;
            }
            queue = new Stack<GObject>();
            objectPool.Add(type, queue);
            queue.Push(gObject);
        }

        public void Push<T>(T[] array) where T : GObject
        {
            int type = typeof(T).GetHashCode();
            if (objectPool.TryGetValue(type, out Stack<GObject> queue))
            {
                queue.CopyTo(array, 0);
                return;
            }
            queue = new Stack<GObject>(array);
            objectPool.Add(type, queue);
        }

        /// <summary>
        /// 创建实体对象, 从对象池进行查询, 如果对象池存在对象, 则返回对象池的对象, 否则创建一个新的T对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Create<T>() where T : Entity, new()
        {
            T entity = Take<T>();
            AddEntity(entity);
            return entity;
        }

        /// <summary>
        /// 创建实体对象, 从对象池进行查询, 如果对象池存在对象, 则返回对象池的对象, 否则创建一个新的T对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Create<T>(T entity) where T : Entity, new()
        {
            AddEntity(entity);
            return entity;
        }

        /// <summary>
        /// 更新ecs系统
        /// </summary>
        /// <param name="worker">线程池并行?</param>
        public void Update(bool worker = false) => Run(worker);

        /// <summary>
        /// 运行ecs
        /// </summary>
        /// <param name="worker">线程池并行?</param>
        public void Run(bool worker = false)
        {
            int count = entities.Count;
            if (!worker)
            {
                for (int i = 0; i < count; i++)
                {
                    if (!entities.buckets[i])
                        continue;
                    if (!entities[i].inactive)
                        continue;
                    entities[i].Execute();
                }
            }
            else
            {
                Parallel.For(0, count, i =>
                {
                    if (!entities.buckets[i])
                        return;
                    if (!entities[i].inactive)
                        return;
                    entities[i].Execute();
                });
            }
        }

        public void Dispose()
        {
            if (isDispose)
                return;
            isDispose = true;
        }

        public T FindObjectOfType<T>() where T : GObject
        {
            Type type = typeof(T);
            var items = entities.ToArray();
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].GetType() == type)
                    return items[i] as T;
                for (int n = 0; n < items[i].components.Count; n++)
                    if (items[i].components[n].GetType() == type)
                        return items[i].components[n] as T;
            }
            return null;
        }

        public T[] FindObjectsOfType<T>() where T : GObject
        {
            Type type = typeof(T);
            var items = entities.ToArray();
            List<T> objs = new List<T>();
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].GetType() == type)
                    objs.Add(items[i] as T);
                for (int n = 0; n < items[i].components.Count; n++)
                    if (items[i].components[n].GetType() == type)
                        objs.Add(items[i].components[n] as T);
            }
            return objs.ToArray();
        }
    }
}