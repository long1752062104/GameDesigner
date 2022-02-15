using System;
using System.Collections.Generic;
using System.Text;
using REAL = FixMath.FP;

namespace GGPhys.Rigid.Forces
{
    /// <summary>
    /// A force that applies to any body that enters the forces area.
    /// </summary>
    public abstract class RigidForceArea
    {
        /// <summary>
        /// Overload this in implementations of the interface to calculate
        /// and update the force applied to the body.
        /// </summary>
        public abstract void UpdateForce(RigidBody body, REAL dt);

    }
}
