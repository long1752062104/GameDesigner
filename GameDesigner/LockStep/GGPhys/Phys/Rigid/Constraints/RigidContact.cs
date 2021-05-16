using GGPhys.Core;
using GGPhys.Rigid.Collisions;
using System;
using TrueSync;

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

        public FP Lambda; //碰撞发现方向拉格朗日乘子

        public FP FLambda; //摩擦力方向拉格朗日乘子

        public int IntegrateTimes = 0; //该碰撞参与迭代过的次数

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


        ///<summary>
        /// 赋值
        ///</summary>
        public void SetData(RigidContactPotential pContact)
        {
            RigidBody one = pContact.Primitive1.Body;
            RigidBody two = pContact.Primitive2.Body;
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

            RigidBody body1 = Body[0];
            RigidBody body2 = Body[1];

            RelativeContactPosition[0] = ContactPoint - body1.Position;
            RelativeContactPosition[1] = ContactPoint - body2.Position;

            ContactVelocity = CalculateLocalVelocity(0);
            if (!Body[1].IsStatic)
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
            RigidBody thisBody = Body[bodyIndex];

            TSVector3 velocity = TSVector3.Cross(thisBody.Rotation, RelativeContactPosition[bodyIndex]);
            velocity += thisBody.Velocity;

            return velocity;
        }

        ///<summary>
        /// 判断刚体是否从休眠中苏醒
        ///</summary>
        public void MatchAwakeState()
        {
            RigidBody body0 = Body[0];
            RigidBody body1 = Body[1];

            bool body0awake = body0.GetAwake();
            bool body1awake = body1.GetAwake();
            bool body0static = body0.IsStatic;
            bool body1static = body1.IsStatic;

            if (body0awake ^ body1awake)
            {
                if (body0static || body1static) return;
                if (body0awake && ContactVelocity.SqrMagnitude > body1.AwakeVelocityLimit)
                    body1.SetAwake();
                if (body1awake && ContactVelocity.SqrMagnitude > body0.AwakeVelocityLimit)
                    body0.SetAwake();
            }

            MatchedAwake = true;
        }
    }
}