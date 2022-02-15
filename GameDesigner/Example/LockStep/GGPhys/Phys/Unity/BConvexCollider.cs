using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGPhys.Rigid.Collisions;
using GGPhys.Core;
using REAL = FixMath.FP;

namespace GGPhysUnity
{

    public class BConvexCollider : BCollider
    {
        public Vector3[] vertices;

        public override void AddToEngine(BRigidBody bBody)
        {
            base.AddToEngine(bBody);
            var shape = new CollisionConvex(ToVector3dList(vertices));
            shape.Body = bBody.Body;
            shape.Offset = Matrix4.IdentityOffset(CenterOffset.ToVector3d() - bBody.CenterOfMassOffset);
            shape.IsTrigger = IsTrigger;
            shape.CollisionLayer = (uint)bBody.collsionLayer;
            shape.CollisionMask = (uint)bBody.collsionMask;
            Primitive = shape;
            RigidPhysicsEngine.Instance.Collisions.AddPrimitive(shape);
        }

        public override Matrix3 CalculateInertiaTensor(REAL mass)
        {
            var inertiaTensor = Matrix3.Identity;
            inertiaTensor.data0 *= mass;
            inertiaTensor.data4 *= mass;
            inertiaTensor.data8 *= mass;
            return inertiaTensor;
        }


        Vector3d[] ToVector3dList(Vector3[] list)
        {
            Vector3d[] v3ds = new Vector3d[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                v3ds[i] = list[i].ToVector3d();
            }
            return v3ds;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 128, 255);
            if (vertices == null || vertices.Length <= 0) return;
            foreach (var p1 in vertices)
            {
                foreach (var p2 in vertices)
                {
                    if(p1 != p2)
                    {
                        Gizmos.DrawLine(transform.TransformPoint(p1), transform.TransformPoint(p2));
                    }
                }
            }
        }
    }
}
