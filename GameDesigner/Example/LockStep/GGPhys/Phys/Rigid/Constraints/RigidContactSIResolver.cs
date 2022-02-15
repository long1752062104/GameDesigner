using System.Collections;
using System.Collections.Generic;
using GGPhys.Core;
using GGPhys.Rigid.Collisions;
using UnityEngine;
using REAL = FixMath.FP;

namespace GGPhys.Rigid.Constraints
{
    /// <summary>
    /// 连续冲量碰撞约束求解器
    /// </summary>
    public class RigidContactSIResolver
    {
        public REAL Belta = 0.3; //处理相交部分的程度
        public REAL Slop = - 0.001; //控制什么范围开始处理相交
        public REAL Tolerence = 0.0005; //相交小于该值，则移除对应碰撞
        public int Interations; //迭代次数
        public REAL[] ContactBias; //相交部分需要修正的偏置
        public List<RigidContact> EndContacts; //迭代结束后需要结束的碰撞


        public RigidContactSIResolver(int interations = 5)
        {
            Interations = interations;
            EndContacts = new List<RigidContact>();
        }

        public void Resolve(CollisionData collisionData, REAL dt)
        {
            PrepareContacts(collisionData.Contacts, dt);
            Integrate(collisionData, Interations, dt);
        }

        /// <summary>
        /// 准备好数据
        /// </summary>
        /// <param name="contacts"></param>
        /// <param name="dt"></param>
        public void PrepareContacts(List<RigidContact> contacts, REAL dt)
        {
            ContactBias = new REAL[contacts.Count];
        }

        /// <summary>
        /// 迭代
        /// </summary>
        /// <param name="collisionData"></param>
        /// <param name="interations"></param>
        /// <param name="dt"></param>
        void Integrate(CollisionData collisionData, int interations, REAL dt)
        {
            var contacts = collisionData.Contacts;
            for (int i = 0; i < interations; i++)
            {
                IntegrateContacts(contacts, dt, i);
            }
            KillContacts(collisionData);
        }

        /// <summary>
        /// 碰撞求解迭代
        /// </summary>
        /// <param name="contacts"></param>
        /// <param name="dt"></param>
        /// <param name="interation"></param>
        void IntegrateContacts(List<RigidContact> contacts, REAL dt, int interation)
        {
            for (int i = 0; i < contacts.Count; i++)
            {
                var contact = contacts[i];

                var body1 = contact.Body[0];
                var body2 = contact.Body[1];

                bool bodyOneStatic = body1.IsStatic || !body1.GetAwake();
                bool bodyTwoStatic = body2.IsStatic || !body2.GetAwake();

                var oneV = body1.Velocity;
                var twoV = body2.Velocity;
                var oneR = body1.Rotation;
                var twoR = body2.Rotation;

                var oneVN = bodyOneStatic ? 0 : Vector3d.Dot(contact.ContactNormal, oneV);
                var oneRN = bodyOneStatic ? 0 : Vector3d.Dot(contact.CrossOne, oneR);
                var twoVN = bodyTwoStatic ? 0 : Vector3d.Dot(-contact.ContactNormal, twoV);
                var twoRN = bodyTwoStatic ? 0 : Vector3d.Dot(contact.CrossTwo, twoR);

                var fOneVN = bodyOneStatic ? 0 : Vector3d.Dot(contact.ContactPerpendicular, oneV);
                var fOneRN = bodyOneStatic ? 0 : Vector3d.Dot(contact.FCrossOne, oneR);
                var fTwoVN = bodyTwoStatic ? 0 : Vector3d.Dot(-contact.ContactPerpendicular, twoV);
                var fTwoRN = bodyTwoStatic ? 0 : Vector3d.Dot(contact.FCrossTwo, twoR);

                REAL JV = oneVN + twoVN + oneRN + twoRN;
                REAL FJV = fOneVN + fTwoVN + fOneRN + fTwoRN;

                if (interation == 0)
                {
                    ContactBias[i] = -(contact.Penetration + Slop) * Belta / dt;
                }

                var Bias = ContactBias[i];
                var VR = contact.ContactVR;
                REAL lambda;
                REAL flambda;
                if (contact.Penetration > -Slop)
                {
                    lambda = -(JV - VR + Bias) / contact.JMJ;
                }
                else
                {
                    lambda = -(JV - VR) / contact.JMJ;
                }

                flambda = -FJV / contact.FJMJ;

                var oldLambda = contact.Lambda;
                contact.Lambda += lambda;
                contact.Lambda = REAL.Clamp(contact.Lambda, 0, REAL.MaxValue);
                var lambdaDelta = contact.Lambda - oldLambda;

                var fOldLambda = contact.FLambda;
                contact.FLambda += flambda;
                contact.FLambda = REAL.Clamp(contact.FLambda, -contact.Lambda * contact.Friction, contact.Lambda * contact.Friction);
                var fLambdaDelta = contact.FLambda - fOldLambda;
                
                Vector3d linearImpulse1 = lambdaDelta * contact.ContactNormal + fLambdaDelta * contact.ContactPerpendicular;
                Vector3d linearImpulse2 = -linearImpulse1;
                Vector3d angularImpulse1 = lambdaDelta * contact.CrossOne + fLambdaDelta * contact.FCrossOne;
                Vector3d angularImpulse2 = lambdaDelta * contact.CrossTwo + fLambdaDelta * contact.FCrossTwo;

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

                if (interation == Interations - 1)
                {
                    contact.IntegrateTimes += 1;
                    contact.Penetration *= REAL.Exp(-Belta * contact.IntegrateTimes);

                    if (contact.Penetration < Tolerence || !contact.HasMultiContacts)
                    {
                        EndContacts.Add(contact);
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// 清除结束的碰撞
        /// </summary>
        void KillContacts(CollisionData collisionData)
        {
            foreach (var contact in EndContacts)
            {
                collisionData.RecycleContact(contact);
            }
            EndContacts.Clear();
        }
    }
}


