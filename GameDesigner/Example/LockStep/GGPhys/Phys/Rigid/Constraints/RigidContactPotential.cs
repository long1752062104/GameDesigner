using GGPhys.Core;
using TrueSync;

namespace GGPhys.Rigid.Collisions
{
    /// <summary>
    /// 潜在碰撞数据类
    /// </summary>
    public class RigidContactPotential
    {
        public CollisionPrimitive Primitive1;
        public CollisionPrimitive Primitive2;
        public long Hash; //唯一标识哈希编码

        public int type; //0:未碰撞或相交 1:触发 2:碰撞

        ///<summary>
        /// 摩擦系数
        ///</summary>
        public FP Friction;

        ///<summary>
        /// 回弹系数
        ///</summary>
        public FP Restitution;

        ///<summary>
        /// 碰撞点
        ///</summary>
        public TSVector3 ContactPoint;

        ///<summary>
        /// 碰撞法线
        ///</summary>
        public TSVector3 ContactNormal;

        ///<summary>
        /// 相交深度
        ///</summary>
        public FP Penetration;

        ///<summary>
        /// 闭合速度
        ///</summary>
        public TSVector3 ContactVelocity;

        ///<summary>
        /// 两刚体各自的中心到碰撞点的向量
        ///</summary>
        public TSVector3[] RelativeContactPosition = new TSVector3[2];

        /// <summary>
        /// 碰撞切线
        /// </summary>
        public TSVector3 ContactPerpendicular;

        public FP ContactVR; //在连续冲量求解约束中预先计算好的参数

        public TSVector3 CrossOne; //在连续冲量求解约束中预先计算好的参数

        public TSVector3 CrossTwo; //在连续冲量求解约束中预先计算好的参数

        public TSVector3 FCrossOne; //在连续冲量求解约束中预先计算好的参数

        public TSVector3 FCrossTwo; //在连续冲量求解约束中预先计算好的参数

        public FP JMJ; //在连续冲量求解约束中预先计算好的参数

        public FP FJMJ; //在连续冲量求解约束中预先计算好的参数

        ///<summary>
        /// 计算内部参数
        ///</summary>
        public void CalculateInternals()
        {

            RigidBody body1 = Primitive1.Body;
            RigidBody body2 = Primitive2.Body;

            RelativeContactPosition[0] = ContactPoint - body1.Position;
            RelativeContactPosition[1] = ContactPoint - body2.Position;

            ContactVelocity = CalculateLocalVelocity(0);
            if (!body2.IsStatic)
                ContactVelocity -= CalculateLocalVelocity(1);
            FP normalContactVelocity = TSVector3.Dot(ContactVelocity, -ContactNormal);
            ContactPerpendicular = -(ContactVelocity + normalContactVelocity * ContactNormal).Normalized;

            ContactVR = normalContactVelocity * Restitution;

            CrossOne = TSVector3.Cross(-RelativeContactPosition[0], -ContactNormal);
            CrossTwo = TSVector3.Cross(RelativeContactPosition[1], -ContactNormal);

            FCrossOne = TSVector3.Cross(RelativeContactPosition[0], ContactPerpendicular);
            FCrossTwo = TSVector3.Cross(RelativeContactPosition[1], -ContactPerpendicular);

            FP oneMass = body1.IsStatic ? 0 : body1.InverseMass;
            FP twoMass = body2.IsStatic ? 0 : body2.InverseMass;
            Matrix3 oneTensor = body1.IsStatic ? Matrix3.Zero : body1.InverseInertiaTensorWorld;
            Matrix3 twoTensor = body2.IsStatic ? Matrix3.Zero : body2.InverseInertiaTensorWorld;

            FP linearPart = TSVector3.Dot(ContactNormal, ContactNormal) * (oneMass + twoMass);
            FP angularPart = TSVector3.Dot(CrossOne, oneTensor * CrossOne) + TSVector3.Dot(CrossTwo, twoTensor * CrossTwo);
            JMJ = linearPart + angularPart;

            FP flinearPart = TSVector3.Dot(ContactPerpendicular, ContactPerpendicular) * (oneMass + twoMass);
            FP fangularPart = TSVector3.Dot(FCrossOne, oneTensor * FCrossOne) + TSVector3.Dot(FCrossTwo, twoTensor * FCrossTwo);
            FJMJ = flinearPart + fangularPart;
        }

        ///<summary>
        /// 计算碰撞点的线性速度，包含刚体线性移动产生的部分和刚体旋转产生的部分
        ///</summary>
        public TSVector3 CalculateLocalVelocity(int bodyIndex)
        {
            RigidBody thisBody = bodyIndex == 0 ? Primitive1.Body : Primitive2.Body;

            TSVector3 velocity = TSVector3.Cross(thisBody.Rotation, RelativeContactPosition[bodyIndex]);
            velocity += thisBody.Velocity;

            return velocity;
        }

        /// <summary>
        /// 交换几何体位置
        /// </summary>
        public void Swap()
        {
            CollisionPrimitive p = Primitive1;
            Primitive1 = Primitive2;
            Primitive2 = p;
        }

        public void Clear()
        {
            Primitive1 = null;
            Primitive2 = null;
            type = 0;
        }
    }


}

