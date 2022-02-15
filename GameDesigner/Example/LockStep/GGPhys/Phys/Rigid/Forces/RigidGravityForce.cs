using System;
using System.Collections.Generic;
using GGPhys.Core;
using REAL = FixMath.FP;

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
        Vector3d m_gravity;

        public RigidGravityForce(REAL gravity)
        {
            m_gravity = new Vector3d(0, gravity, 0);
        }

        public RigidGravityForce(Vector3d gravity)
        {
            m_gravity = gravity;
        }

        /// <summary>
        /// 设置重力
        /// </summary>
        /// <param name="gravity"></param>
        public void SetGravity(REAL gravity)
        {
            m_gravity = new Vector3d(0, gravity, 0);
        }

        ///<summary>
        /// 运用作用力
        ///</summary>
        public override void UpdateForce(RigidBody body, REAL dt)
        {
            if (body.HasInfiniteMass) return;
            body.AddForce(m_gravity * body.GetMass());
        }
    }

}