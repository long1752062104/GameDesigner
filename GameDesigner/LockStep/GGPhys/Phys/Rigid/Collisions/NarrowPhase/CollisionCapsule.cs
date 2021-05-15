using GGPhys.Core;
using TrueSync;

namespace GGPhys.Rigid.Collisions
{
    ///<summary>
    /// 胶囊形状碰撞几何体
    ///</summary>
    public class CollisionCapsule : CollisionPrimitive
    {
        public FP Radius; //半径
        public FP Height; //两端圆心距离
        public TSVector3 HalfHeight; // 半高向量
        public TSVector3 CenterOne; //一端圆心
        public TSVector3 CenterTwo; //另一端圆心
        public TSVector3 CenterOneToTwo; //圆心一到圆心二向量
        public TSVector3 CenterTwoToOne; //圆心二到圆心一向量

        public CollisionCapsule(FP radius, FP height)
        {
            Radius = radius;
            Height = height;
            HalfHeight = new TSVector3(0, 0.5 * height, 0);
            BoundingVolum = new BoundingBox();
            BoundingVolum.Init(this);
        }

        public override void CalculateInternals()
        {
            base.CalculateInternals();
            Matrix3 rotMt = new Matrix3(GetAxis(0), GetAxis(1), GetAxis(2));
            CenterOne = GetAxis(3) + rotMt.Transform(HalfHeight);
            CenterTwo = GetAxis(3) - rotMt.Transform(HalfHeight);
            CenterOneToTwo = CenterTwo - CenterOne;
            CenterTwoToOne = CenterOne - CenterTwo;
            BoundingVolum.Update(this);
        }
    }
}
