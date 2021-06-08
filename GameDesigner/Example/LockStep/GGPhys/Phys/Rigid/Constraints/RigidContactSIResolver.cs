using GGPhys.Rigid.Collisions;
using System.Collections.Generic;
using TrueSync;

namespace GGPhys.Rigid.Constraints
{
    /// <summary>
    /// 连续冲量碰撞约束求解器
    /// </summary>
    public class RigidContactSIResolver
    {
        public FP Belta = 0.3; //处理相交部分的程度
        public FP Slop = -0.001; //控制什么范围开始处理相交
        public FP Tolerence = 0.0005; //相交小于该值，则移除对应碰撞
        public int Interations; //迭代次数
        public List<FP> ContactBias = new List<FP>(); //相交部分需要修正的偏置
        public List<RigidContact> EndContacts; //迭代结束后需要结束的碰撞


        public RigidContactSIResolver(int interations = 5)
        {
            Interations = interations;
            EndContacts = new List<RigidContact>();
        }

        public void ResolveContacts(CollisionData collisionData, FP dt)
        {
            PrepareContacts(collisionData.Contacts, dt);
            Integrate(collisionData, Interations, dt);
        }

        /// <summary>
        /// 准备好数据
        /// </summary>
        /// <param name="contacts"></param>
        /// <param name="dt"></param>
        public void PrepareContacts(List<RigidContact> contacts, FP dt)
        {
            //ContactBias.Clear(); //= new FP[contacts.Count];
            if (ContactBias.Count >= contacts.Count)
            {
                for (int i = 0; i < contacts.Count; i++)
                    ContactBias[i] = 0;
            }
            else
            {
                for (int i = 0; i < contacts.Count; i++)
                    ContactBias.Add(0);
            }
        }

        /// <summary>
        /// 迭代
        /// </summary>
        /// <param name="collisionData"></param>
        /// <param name="interations"></param>
        /// <param name="dt"></param>
        void Integrate(CollisionData collisionData, int interations, FP dt)
        {
            List<RigidContact> contacts = collisionData.Contacts;
            for (int i = 0; i < interations; i++)
            {
                for (int j = 0; j < contacts.Count; j++)
                {
                    RigidContact contact = contacts[j];

                    RigidBody body1 = contact.Body[0];
                    RigidBody body2 = contact.Body[1];

                    bool bodyOneStatic = body1.IsStatic || !body1.GetAwake();
                    bool bodyTwoStatic = body2.IsStatic || !body2.GetAwake();

                    TSVector3 oneV = body1.Velocity;
                    TSVector3 twoV = body2.Velocity;
                    TSVector3 oneR = body1.Rotation;
                    TSVector3 twoR = body2.Rotation;

                    FP oneVN = bodyOneStatic ? 0 : TSVector3.Dot(contact.ContactNormal, oneV);
                    FP oneRN = bodyOneStatic ? 0 : TSVector3.Dot(contact.CrossOne, oneR);
                    FP twoVN = bodyTwoStatic ? 0 : TSVector3.Dot(-contact.ContactNormal, twoV);
                    FP twoRN = bodyTwoStatic ? 0 : TSVector3.Dot(contact.CrossTwo, twoR);

                    FP fOneVN = bodyOneStatic ? 0 : TSVector3.Dot(contact.ContactPerpendicular, oneV);
                    FP fOneRN = bodyOneStatic ? 0 : TSVector3.Dot(contact.FCrossOne, oneR);
                    FP fTwoVN = bodyTwoStatic ? 0 : TSVector3.Dot(-contact.ContactPerpendicular, twoV);
                    FP fTwoRN = bodyTwoStatic ? 0 : TSVector3.Dot(contact.FCrossTwo, twoR);

                    FP JV = oneVN + twoVN + oneRN + twoRN;
                    FP FJV = fOneVN + fTwoVN + fOneRN + fTwoRN;

                    if (i == 0)
                    {
                        ContactBias[j] = -(contact.Penetration + Slop) * Belta / dt;
                    }

                    FP Bias = ContactBias[j];
                    FP VR = contact.ContactVR;
                    FP lambda;
                    if (contact.Penetration > -Slop)
                    {
                        lambda = -(JV - VR + Bias) / contact.JMJ;
                    }
                    else
                    {
                        lambda = -(JV - VR) / contact.JMJ;
                    }

                    FP flambda = -FJV / contact.FJMJ;

                    FP oldLambda = contact.Lambda;
                    contact.Lambda += lambda;
                    contact.Lambda = FP.Clamp(contact.Lambda, 0, 10 /*FP.MaxValue*/);
                    FP lambdaDelta = contact.Lambda - oldLambda;

                    FP fOldLambda = contact.FLambda;
                    contact.FLambda += flambda;
                    contact.FLambda = FP.Clamp(contact.FLambda, -contact.Lambda * contact.Friction, contact.Lambda * contact.Friction);
                    FP fLambdaDelta = contact.FLambda - fOldLambda;

                    //if (lambdaDelta > 10f)
                    //    lambdaDelta = 10f;
                    if (lambdaDelta <= 0f)
                        lambdaDelta = 0.0001f;
                    //if (fLambdaDelta > 10f)
                    //    fLambdaDelta = 10f;
                    if (fLambdaDelta <= 0f)
                        fLambdaDelta = 0.0001f;

                    //线性弹跳, 穿模
                    TSVector3 linearImpulse1 = TSVector3.zero;
                    if (!bodyOneStatic | !bodyTwoStatic)
                        linearImpulse1 = lambdaDelta * contact.ContactNormal + fLambdaDelta * contact.ContactPerpendicular;
                    TSVector3 linearImpulse2 = -linearImpulse1;
                    TSVector3 angularImpulse1 = lambdaDelta * contact.CrossOne + fLambdaDelta * contact.FCrossOne;
                    TSVector3 angularImpulse2 = lambdaDelta * contact.CrossTwo + fLambdaDelta * contact.FCrossTwo;

                    if (!bodyOneStatic)
                    {
                        body1.ApplyLinearImpulse(linearImpulse1);
                        body1.ApplyAngularImpulse(angularImpulse1);
                    }

                    if (!bodyTwoStatic)
                    {
                        body2.ApplyLinearImpulse(linearImpulse2);
                        body2.ApplyAngularImpulse(angularImpulse2);
                    }

                    if (i == interations - 1)
                    {
                        contact.IntegrateTimes += 1;
                        contact.Penetration *= TSMathf.Exp(-Belta * contact.IntegrateTimes);

                        if (contact.Penetration < Tolerence | contact.Penetration < -10000 | contact.Penetration > 10000)
                        {
                            contact.Penetration = 0;
                            collisionData.RecycleContact(contact);
                        }
                    }
                }
            }
        }
    }
}


