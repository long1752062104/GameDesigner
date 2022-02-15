using System;
using System.Collections.Generic;
using GGPhys.Core;
using REAL = FixMath.FP;

namespace GGPhys.Rigid.Forces
{

    ///<summary>
    /// ����������������
    ///</summary>
    public class RigidSpringForce : RigidForce
    {

        ///<summary>
        /// �������������õ���������
        ///</summary>
        private RigidBody m_bodyA, m_bodyB;

        ///<summary>
        /// ��������ϵ�е��������ӵ�
        ///</summary>
        private Vector3d m_connectionA, m_connectionB;

        ///<summary>
        /// ����ϵ��
        ///</summary>
        private REAL m_springConstant;

        ///<summary>
        /// ������������
        ///</summary>
        private REAL m_restLength;

        public RigidSpringForce(RigidBody a, RigidBody b, Vector3d connectionA, Vector3d connectionB, REAL springConstant, REAL restLength)
        {
            m_bodyA = a;
            m_bodyB = b;
            m_connectionA = connectionA;
            m_connectionB = connectionB;
            m_springConstant = springConstant;
            m_restLength = restLength;
        }

        public override void UpdateForce(REAL dt)
        {
            UpdateForce(m_bodyA, m_bodyB, m_connectionA, m_connectionB, dt);
            UpdateForce(m_bodyB, m_bodyA, m_connectionB, m_connectionA, dt);
        }

        private void UpdateForce(RigidBody body, RigidBody other, Vector3d connection, Vector3d otherConnection, REAL dt)
        {
            if (body.HasInfiniteMass) return;

            Vector3d lws = body.GetPointInWorldSpace(connection);
            Vector3d ows = other.GetPointInWorldSpace(otherConnection);

            Vector3d force = lws - ows;

            REAL magnitude = force.Magnitude;

            magnitude = (m_restLength - magnitude) * m_springConstant;

            force.Normalize();
            force *= magnitude;
            body.AddForceAtPoint(force, lws);
            
        }
    }

}