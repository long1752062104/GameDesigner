using TrueSync;

namespace GGPhys.Rigid.Forces
{

    ///<summary>
    /// 弹力作用力发生器
    ///</summary>
    public class RigidSpringForce : RigidForce
    {

        ///<summary>
        /// 作用力产生作用的两个刚体
        ///</summary>
        private RigidBody m_bodyA, m_bodyB;

        ///<summary>
        /// 本地坐标系中的两个连接点
        ///</summary>
        private TSVector3 m_connectionA, m_connectionB;

        ///<summary>
        /// 弹性系数
        ///</summary>
        private FP m_springConstant;

        ///<summary>
        /// 弹簧正常长度
        ///</summary>
        private FP m_restLength;

        public RigidSpringForce(RigidBody a, RigidBody b, TSVector3 connectionA, TSVector3 connectionB, FP springConstant, FP restLength)
        {
            m_bodyA = a;
            m_bodyB = b;
            m_connectionA = connectionA;
            m_connectionB = connectionB;
            m_springConstant = springConstant;
            m_restLength = restLength;
        }

        public override void UpdateForce(FP dt)
        {
            UpdateForce(m_bodyA, m_bodyB, m_connectionA, m_connectionB, dt);
            UpdateForce(m_bodyB, m_bodyA, m_connectionB, m_connectionA, dt);
        }

        private void UpdateForce(RigidBody body, RigidBody other, TSVector3 connection, TSVector3 otherConnection, FP dt)
        {
            if (body.HasInfiniteMass) return;

            TSVector3 lws = body.GetPointInWorldSpace(connection);
            TSVector3 ows = other.GetPointInWorldSpace(otherConnection);

            TSVector3 force = lws - ows;

            FP magnitude = force.Magnitude;

            magnitude = (m_restLength - magnitude) * m_springConstant;

            force.Normalize();
            force *= magnitude;
            body.AddForceAtPoint(force, lws);

        }
    }

}