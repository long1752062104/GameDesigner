using System;
using System.Collections.Generic;
using GGPhys.Core;
using GGPhys.Rigid;
using REAL = FixMath.FP;

namespace GGPhys.Rigid.Forces
{

    /// <summary>
    /// 刚体作用力类
    /// </summary>
    public abstract class RigidForce
    {
        /// <summary>
        /// 为刚体生成力
        /// </summary>
        public abstract void UpdateForce(REAL dt);

    }
}
