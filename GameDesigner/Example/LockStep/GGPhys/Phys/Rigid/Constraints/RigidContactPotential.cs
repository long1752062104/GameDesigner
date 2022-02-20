using System;
using System.Collections;
using System.Collections.Generic;
using GGPhys.Core;
using REAL = FixMath.FP;

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
        public REAL Friction;

        ///<summary>
        /// 回弹系数
        ///</summary>
        public REAL Restitution;

        ///<summary>
        /// 碰撞点
        ///</summary>
        public Vector3d ContactPoint;

        ///<summary>
        /// 碰撞法线
        ///</summary>
        public Vector3d ContactNormal;

        ///<summary>
        /// 相交深度
        ///</summary>
        public REAL Penetration;

        ///<summary>
        /// 闭合速度
        ///</summary>
        public Vector3d ContactVelocity;

        ///<summary>
        /// 两刚体各自的中心到碰撞点的向量
        ///</summary>
        public Vector3d[] RelativeContactPosition = new Vector3d[2];

        /// <summary>
        /// 碰撞切线
        /// </summary>
        public Vector3d ContactPerpendicular;

        public REAL ContactVR; //在连续冲量求解约束中预先计算好的参数

        public Vector3d CrossOne; //在连续冲量求解约束中预先计算好的参数

        public Vector3d CrossTwo; //在连续冲量求解约束中预先计算好的参数

        public Vector3d FCrossOne; //在连续冲量求解约束中预先计算好的参数

        public Vector3d FCrossTwo; //在连续冲量求解约束中预先计算好的参数

        public REAL JMJ; //在连续冲量求解约束中预先计算好的参数

        public REAL FJMJ; //在连续冲量求解约束中预先计算好的参数

        ///<summary>
        /// 计算内部参数
        ///</summary>
        public void CalculateInternals()
        {
            var body1 = Primitive1.Body;
            var body2 = Primitive2.Body;

            //初始化向量, 没有这个会反弹的离谱
            if (body1.Velocity.x > body1.VelocityLimit.x)
                body1.Velocity.x = body1.VelocityLimit.x;
            if (body1.Velocity.y > body1.VelocityLimit.y)
                body1.Velocity.y = body1.VelocityLimit.y;
            if (body1.Velocity.z > body1.VelocityLimit.z)
                body1.Velocity.z = body1.VelocityLimit.z;

            if (body1.Velocity.x < -body1.VelocityLimit.x)
                body1.Velocity.x = -body1.VelocityLimit.x;
            if (body1.Velocity.y < -body1.VelocityLimit.y)
                body1.Velocity.y = -body1.VelocityLimit.y;
            if (body1.Velocity.z < -body1.VelocityLimit.z)
                body1.Velocity.z = -body1.VelocityLimit.z;

            if (body2.Velocity.x > body2.VelocityLimit.x)
                body2.Velocity.x = body2.VelocityLimit.x;
            if (body2.Velocity.y > body2.VelocityLimit.y)
                body2.Velocity.y = body2.VelocityLimit.y;
            if (body2.Velocity.z > body2.VelocityLimit.z)
                body2.Velocity.z = body2.VelocityLimit.z;

            if (body2.Velocity.x < -body2.VelocityLimit.x)
                body2.Velocity.x = -body2.VelocityLimit.x;
            if (body2.Velocity.y < -body2.VelocityLimit.y)
                body2.Velocity.y = -body2.VelocityLimit.y;
            if (body2.Velocity.z < -body2.VelocityLimit.z)
                body2.Velocity.z = -body2.VelocityLimit.z;

            RelativeContactPosition[0] = ContactPoint - body1.Position;
            RelativeContactPosition[1] = ContactPoint - body2.Position;

            ContactVelocity = CalculateLocalVelocity(0);
            if (!body2.IsStatic)
                ContactVelocity -= CalculateLocalVelocity(1);
            var normalContactVelocity = Vector3d.Dot(ContactVelocity, -ContactNormal);

            ContactPerpendicular = -(ContactVelocity + normalContactVelocity * ContactNormal).Normalized;

            if (Vector3d.AbsDot(ContactPerpendicular, ContactNormal) > 0.99)
                ContactPerpendicular = Vector3d.Zero;

            ContactVR = normalContactVelocity * Restitution;

            CrossOne = Vector3d.Cross(-RelativeContactPosition[0], -ContactNormal);
            CrossTwo = Vector3d.Cross(RelativeContactPosition[1], -ContactNormal);

            FCrossOne = Vector3d.Cross(RelativeContactPosition[0], ContactPerpendicular);
            FCrossTwo = Vector3d.Cross(RelativeContactPosition[1], -ContactPerpendicular);

            var oneMass = body1.IsStatic ? 0 : body1.InverseMass;
            var twoMass = body2.IsStatic ? 0 : body2.InverseMass;
            var oneTensor = body1.IsStatic ? Matrix3.Zero : body1.InverseInertiaTensorWorld;
            var twoTensor = body2.IsStatic ? Matrix3.Zero : body2.InverseInertiaTensorWorld;

            REAL linearPart = Vector3d.Dot(ContactNormal, ContactNormal) * (oneMass + twoMass);
            REAL angularPart = Vector3d.Dot(CrossOne, oneTensor * CrossOne) + Vector3d.Dot(CrossTwo, twoTensor * CrossTwo);
            JMJ = linearPart + angularPart;

            REAL flinearPart = Vector3d.Dot(ContactPerpendicular, ContactPerpendicular) * (oneMass + twoMass);
            REAL fangularPart = Vector3d.Dot(FCrossOne, oneTensor * FCrossOne) + Vector3d.Dot(FCrossTwo, twoTensor * FCrossTwo);
            FJMJ = flinearPart + fangularPart;
        }

        ///<summary>
        /// 计算碰撞点的线性速度，包含刚体线性移动产生的部分和刚体旋转产生的部分
        ///</summary>
        public Vector3d CalculateLocalVelocity(int bodyIndex)
        {
            RigidBody thisBody = bodyIndex == 0 ? Primitive1.Body : Primitive2.Body;

            Vector3d velocity = Vector3d.Cross(thisBody.Rotation, RelativeContactPosition[bodyIndex]);
            velocity += thisBody.Velocity;

            return velocity;
        }

        /// <summary>
        /// 交换几何体位置
        /// </summary>
        public void Swap()
        {
            var p = Primitive1;
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

