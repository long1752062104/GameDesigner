using GGPhys.Core;
using TrueSync;

namespace GGPhys.Rigid.Collisions
{
    ///<summary>
    /// 碰撞形状几何体
    ///</summary>
    public abstract class CollisionPrimitive
    {
        /// <summary>
        /// 所属碰撞层级
        /// </summary>
        public uint CollisionLayer = 0x00000001;

        /// <summary>
        /// 和哪些层级进行碰撞，位运算
        /// </summary>
        public uint CollisionMask = 0xffffffff;

        ///<summary>
        /// 所属刚体
        ///</summary>
        public RigidBody Body;

        /// <summary>
        /// 是否为触发器
        /// </summary>
        public bool IsTrigger;

        ///<summary>
        /// 相对于刚体的偏移矩阵
        ///</summary>
        public Matrix4 Offset;

        ///<summary>
        /// 几何体变换矩阵
        ///</summary>
        public Matrix4 Transform;

        /// <summary>
        /// 集合体包围盒，用于粗略碰撞检测
        /// </summary>
        public BoundingVolum BoundingVolum;

        /// <summary>
        /// hash编码顺序号
        /// </summary>
        public int HashOrder;


        public CollisionPrimitive()
        {
            Transform = Matrix4.Identity;
        }

        ///<summary>
        /// 更新计算各状态数据
        ///</summary>
        public virtual void CalculateInternals()
        {
            Transform = Body.Transform;// * Offset;
        }

        ///<summary>
        /// 获取某方向轴向世界坐标方向
        ///</summary>
        public TSVector3 GetAxis(int index)
        {
            return Transform.GetAxisVector(index);
        }

        public override string ToString()
        {
            return Body.ToString();
        }
    }
}
