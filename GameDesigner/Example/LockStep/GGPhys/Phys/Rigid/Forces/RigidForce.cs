using System;
using System.Collections.Generic;
using GGPhys.Core;
using GGPhys.Rigid;
using REAL = FixMath.FP;

namespace GGPhys.Rigid.Forces
{

    /// <summary>
    /// ������������
    /// </summary>
    public abstract class RigidForce
    {
        /// <summary>
        /// Ϊ����������
        /// </summary>
        public abstract void UpdateForce(REAL dt);

    }
}
