using TrueSync;

namespace GGPhys.Rigid.Forces
{

    ///<summary>
    /// 重力作用力发生器
    ///</summary>
    public class RigidGravityForce : RigidForceArea
    {

        ///<summary>
        /// 重力设置
        ///<summary>
        TSVector3 m_gravity;

        public RigidGravityForce(FP gravity)
        {
            m_gravity = new TSVector3(0, gravity, 0);
        }

        public RigidGravityForce(TSVector3 gravity)
        {
            m_gravity = gravity;
        }

        /// <summary>
        /// 设置重力
        /// </summary>
        /// <param name="gravity"></param>
        public void SetGravity(FP gravity)
        {
            m_gravity = new TSVector3(0, gravity, 0);
        }

        ///<summary>
        /// 运用作用力
        ///</summary>
        public override void UpdateForce(RigidBody body, FP dt)
        {
            if (body.HasInfiniteMass) return;
            body.AddForce(m_gravity * body.GetMass());
        }
    }

}