namespace GGPhys.Rigid.Collisions
{
    /// <summary>
    /// 网格类
    /// </summary>
    public class Grid
    {
        public static ClassObjectPool<LinkedNode<CollisionPrimitive>> PrimitiveNodePool; //几何体节点池
        public LinkedNode<CollisionPrimitive> HeadPrimitive; //几何体头节点
        public LinkedNode<CollisionPrimitive> HeadStaticPrimitive; //静态几何体尾节点

        public Grid ParentGrid; //父格子

        /// <summary>
        /// 添加几何体
        /// </summary>
        /// <param name="primitive"></param>
        public void AddPrimitive(CollisionPrimitive primitive)
        {
            if (HeadPrimitive == null)
            {
                HeadPrimitive = PrimitiveNodePool.Spawn();
                HeadPrimitive.obj = primitive;
                return;
            }
            LinkedNode<CollisionPrimitive> newHead = PrimitiveNodePool.Spawn();
            newHead.obj = primitive;
            newHead.next = HeadPrimitive;
            HeadPrimitive = newHead;
        }

        /// <summary>
        /// 添加静态几何体
        /// </summary>
        /// <param name="primitive"></param>
        public void AddStaticPrimitive(CollisionPrimitive primitive)
        {
            if (HeadStaticPrimitive == null)
            {
                HeadStaticPrimitive = PrimitiveNodePool.Spawn();
                HeadStaticPrimitive.obj = primitive;
                return;
            }
            LinkedNode<CollisionPrimitive> newHead = PrimitiveNodePool.Spawn();
            newHead.obj = primitive;
            newHead.next = HeadStaticPrimitive;
            HeadStaticPrimitive = newHead;
        }

        /// <summary>
        /// 移除静态几何体
        /// </summary>
        /// <param name="primitive"></param>
        public void RemoveStaticPrimitive(CollisionPrimitive primitive)
        {
            LinkedNode<CollisionPrimitive> node = HeadStaticPrimitive;
            if (node.obj == primitive)
            {
                HeadStaticPrimitive = node.next;
                ClearNode(node);
                return;
            }
            while (node.next != null)
            {
                if (node.next.obj == primitive)
                {
                    LinkedNode<CollisionPrimitive> next = node.next;
                    node.next = next.next;
                    ClearNode(next);
                    return;
                }
                node = node.next;
            }
        }

        /// <summary>
        /// 从头部清除节点
        /// </summary>
        /// <param name="node"></param>
        public void ClearNode(LinkedNode<CollisionPrimitive> node)
        {
            node.obj = null;
            node.next = null;
            PrimitiveNodePool.Recycle(node);
        }


        /// <summary>
        /// 清除全部几何体节点
        /// </summary>
        public void Clear()
        {
            while (HeadPrimitive != null)
            {
                LinkedNode<CollisionPrimitive> next = HeadPrimitive.next;
                HeadPrimitive.obj = null;
                HeadPrimitive.next = null;
                PrimitiveNodePool.Recycle(HeadPrimitive);
                HeadPrimitive = next;
            }
        }



        /// <summary>
        /// 是否存在潜在碰撞
        /// </summary>
        /// <returns></returns>
        public bool HasContact()
        {
            return HeadPrimitive != null;
        }
    }

    /// <summary>
    /// 几何体节点
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LinkedNode<T>
    {
        public T obj;
        public LinkedNode<T> next;
    }
}

