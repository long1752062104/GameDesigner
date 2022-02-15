using System;
using System.Collections.Generic;
using GGPhys.Core;
using GGPhys.Rigid;
using REAL = FixMath.FP;

namespace GGPhys.Rigid.Collisions
{
    /// <summary>
    /// ������ײ������
    /// </summary>
    public class CollisionSphere : CollisionPrimitive
    {
        public REAL Radius;

        public CollisionSphere(REAL radius)
        {
            Radius = radius;
            BoundingVolum = new BoundingSphere();
            BoundingVolum.Init(this);
        }

        public override void CalculateInternals()
        {
            base.CalculateInternals();
            BoundingVolum.Update(this);
        }
    }
}
