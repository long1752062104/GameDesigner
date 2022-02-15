using System;
using System.Collections.Generic;
using GGPhys.Core;
using GGPhys.Rigid;
using REAL = FixMath.FP;

namespace GGPhys.Rigid.Collisions
{
    ///<summary>
    /// 凸多边形碰撞几何体
    ///</summary>
    public class CollisionConvex : CollisionPrimitive
    {
        public Vector3d[] Vertices;
        private Vector3d[] LocalVertices;

        public CollisionConvex(Vector3d[] vertices)
        {
            LocalVertices = vertices;
            Vertices = new Vector3d[vertices.Length];
            BoundingVolum = new BoundingBox();
            BoundingVolum.Init(this);
        }

        public override void CalculateInternals()
        {
            base.CalculateInternals();
            GetVertices();
            BoundingVolum.Update(this);
        }

        void GetVertices()
        {
            for (int i = 0; i < LocalVertices.Length; i++)
            {
                Vertices[i] = Transform * LocalVertices[i];
            }
        }
    }
}
