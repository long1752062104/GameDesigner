using System.Collections;
using System.Collections.Generic;
using GGPhys.Core;

namespace GGPhys.Rigid.Collisions
{
    /// <summary>
    /// 三角形碰撞几何体
    /// </summary>
    public class CollisionTriangle : CollisionPrimitive
    {
        public Vector3d[] Vertices;
        public Vector3d Normal;

        public CollisionTriangle(Vector3d a, Vector3d b, Vector3d c)
        {
            Vertices = new Vector3d[3] { a, b, c };
            BoundingVolum = new BoundingBox();
            BoundingVolum.Init(this);
        }

        public override void CalculateInternals()
        {
            base.CalculateInternals();
            Vertices[0] = Transform.Transform(Vertices[0]);
            Vertices[1] = Transform.Transform(Vertices[1]);
            Vertices[2] = Transform.Transform(Vertices[2]);
            Normal = Vector3d.Cross(Vertices[1] - Vertices[0], Vertices[2] - Vertices[1]).Normalized;
            BoundingVolum.Update(this);
        }
    }
}

