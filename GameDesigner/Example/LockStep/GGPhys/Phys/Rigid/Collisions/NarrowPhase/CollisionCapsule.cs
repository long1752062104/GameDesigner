using System.Collections;
using System.Collections.Generic;
using GGPhys.Core;
using GGPhys.Rigid;
using REAL = FixMath.FP;

namespace GGPhys.Rigid.Collisions
{
    ///<summary>
    /// 胶囊形状碰撞几何体
    ///</summary>
    public class CollisionCapsule : CollisionPrimitive
    {
        public REAL Radius; //半径
        public REAL Height; //两端圆心距离
        public Vector3d HalfHeight; // 半高向量
        public Vector3d CenterOne; //一端圆心
        public Vector3d CenterTwo; //另一端圆心
        public Vector3d CenterOneToTwo; //圆心一到圆心二向量
        public Vector3d CenterTwoToOne; //圆心二到圆心一向量

        public CollisionCapsule(REAL radius, REAL height)
        {
            Radius = radius;
            Height = height;
            HalfHeight = new Vector3d(0, 0.5 * height, 0);
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
