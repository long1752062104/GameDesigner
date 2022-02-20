using System;
using System.Collections.Generic;
using GGPhys.Core;
using REAL = FixMath.FP;
using GGPhys.Rigid.Collisions;

namespace GGPhys.Rigid.Constraints
{
    /// <summary>
    /// 碰撞数据类
    /// </summary>
    public class RigidContact
    {

        ///<summary>
        /// 相互碰撞的两个刚体
        ///</summary>
        public RigidBody[] Body = new RigidBody[2];

        /// <summary>
        /// 是否计算过内部参数并检测过唤醒
        /// </summary>
        public bool MatchedAwake = false;

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

        public REAL Lambda; //碰撞发现方向拉格朗日乘子

        public REAL FLambda; //摩擦力方向拉格朗日乘子

        public int IntegrateTimes = 0; //该碰撞参与迭代过的次数

        public bool HasMultiContacts = false; //是否允许同时求解多个碰撞

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


        ///<summary>
        /// 赋值
        ///</summary>
        public void SetData(RigidContactPotential pContact)
        {
            var one = pContact.Primitive1.Body;
            var two = pContact.Primitive2.Body;
            one.AddContactBody(two, pContact.ContactPoint);
            two.AddContactBody(one, pContact.ContactPoint);
            Body[0] = one;
            Body[1] = two;
            Friction = pContact.Friction;
            Restitution = pContact.Restitution;
            ContactNormal = pContact.ContactNormal;
            ContactPoint = pContact.ContactPoint;
            Penetration = pContact.Penetration;
            ContactVelocity = pContact.ContactVelocity;
            ContactPerpendicular = pContact.ContactPerpendicular;
            ContactVR = pContact.ContactVR;
            CrossOne = pContact.CrossOne;
            CrossTwo = pContact.CrossTwo;
            FCrossOne = pContact.FCrossOne;
            FCrossTwo = pContact.FCrossTwo;
            JMJ = pContact.JMJ;
            FJMJ = pContact.FJMJ;
            RelativeContactPosition[0] = pContact.RelativeContactPosition[0];
            RelativeContactPosition[1] = pContact.RelativeContactPosition[1];

            if((pContact.Primitive1 is CollisionBox || pContact.Primitive2 is CollisionBox || pContact.Primitive1 is CollisionConvex || pContact.Primitive2 is CollisionConvex)
                && !(pContact.Primitive1 is CollisionCapsule || pContact.Primitive2 is CollisionCapsule)
                && !(pContact.Primitive1 is CollisionSphere || pContact.Primitive2 is CollisionSphere))
            {
                HasMultiContacts = true;
            }
            else
            {
                HasMultiContacts = false;
            }

            MatchAwakeState();
        }

        /// <summary>
        /// 清除复位，准备回收
        /// </summary>
        public void Clear()
        {
            Body[0] = null;
            Body[1] = null;
            Lambda = 0;
            FLambda = 0;
            IntegrateTimes = 0;
            MatchedAwake = false;
        }

        ///<summary>
        /// 计算内部参数
        ///</summary>
        public void CalculateInternals()
        {
            if (Body[0] == null || Body[1] == null)
                throw new NullReferenceException("Body null");

            var body1 = Body[0];
            var body2 = Body[1];

            RelativeContactPosition[0] = ContactPoint - body1.Position;
            RelativeContactPosition[1] = ContactPoint - body2.Position;

            ContactVelocity = CalculateLocalVelocity(0);
            if (!Body[1].IsStatic)
                ContactVelocity -= CalculateLocalVelocity(1);
            var normalContactVelocity = Vector3d.Dot(ContactVelocity, -ContactNormal);
            ContactPerpendicular = -(ContactVelocity + normalContactVelocity * ContactNormal).Normalized;

            ContactVR = normalContactVelocity * Restitution;

            CrossOne = Vector3d.Cross(-RelativeContactPosition[0], -ContactNormal);
            CrossTwo = Vector3d.Cross(RelativeContactPosition[1], -ContactNormal);

            FCrossOne = Vector3d.Cross(RelativeContactPosition[0], ContactPerpendicular);
            FCrossTwo = Vector3d.Cross(RelativeContactPosition[1], - ContactPerpendicular);

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
            RigidBody thisBody = Body[bodyIndex];

            Vector3d velocity = Vector3d.Cross(thisBody.Rotation, RelativeContactPosition[bodyIndex]);
            velocity += thisBody.Velocity;

            return velocity;
        }

        ///<summary>
        /// 判断刚体是否从休眠中苏醒
        ///</summary>
        public void MatchAwakeState()
        {
            var body0 = Body[0];
            var body1 = Body[1];

            bool body0awake = body0.GetAwake();
            bool body1awake = body1.GetAwake();
            bool body0static = body0.IsStatic;
            bool body1static = body1.IsStatic;

            if (body0awake ^ body1awake)
            {
                if (body0static || body1static) return;
                if (body0awake && ContactVelocity.SqrMagnitude >= body1.AwakeVelocityLimit)
                    body1.SetAwake();
                if (body1awake && ContactVelocity.SqrMagnitude >= body0.AwakeVelocityLimit)
                    body0.SetAwake();
            }

            MatchedAwake = true;
        }
    }
}