using GGPhys.Core;
using GGPhys.Rigid.Collisions;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;

namespace GGPhysUnity
{

    public class BConvexCollider : BCollider
    {
        public List<TSVector3> vertices = new List<TSVector3>();

        public override void AddToEngine(BRigidBody bBody)
        {
            CollisionConvex shape = new CollisionConvex(vertices.ToArray())
            {
                Body = bBody.Body,
                Offset = Matrix4.IdentityOffset(CenterOffset/* - bBody.CenterOfMassOffset*/),
                IsTrigger = IsTrigger,
                CollisionLayer = (uint)bBody.collsionLayer,
                CollisionMask = (uint)bBody.collsionMask
            };
            bBody.Body.Offset = CenterOffset;
            Primitive = shape;
            RigidPhysicsEngine.Instance.Collisions.AddPrimitive(shape);
        }

        public override Matrix3 CalculateInertiaTensor(FP mass)
        {
            Matrix3 inertiaTensor = Matrix3.Identity;
            inertiaTensor.data0 *= mass;
            inertiaTensor.data4 *= mass;
            inertiaTensor.data8 *= mass;
            return inertiaTensor;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 128, 255);
            if (vertices.Count <= 0) return;
            foreach (TSVector3 p1 in vertices)
            {
                foreach (TSVector3 p2 in vertices)
                {
                    if (p1 != p2)
                    {
                        Gizmos.DrawLine(transform.TransformPoint(p1), transform.TransformPoint(p2));
                    }
                }
            }
        }

        private void Reset()
        {
            vertices.Clear();
            if (transform == null)
                transform = GetComponent<TSTransform>();
            TSVector3 size = transform.localScale;
            transform.localScale = new TSVector3(TSMathf.Abs(size.x), TSMathf.Abs(size.y), TSMathf.Abs(size.z));
            BoxCollider box = GetComponent<BoxCollider>();
            if (box == null)
            {
                box = gameObject.AddComponent<BoxCollider>();
                size = (size / 2).Multiply(box.size);
                DestroyImmediate(box, true);
            }
            else size = (size / 2).Multiply(box.size);
            //size /= 2;
            vertices.Add(new TSVector3(size.x, -size.y, -size.z));
            vertices.Add(new TSVector3(size.x, -size.y, size.z));
            vertices.Add(new TSVector3(size.x, size.y, -size.z));
            vertices.Add(new TSVector3(size.x, size.y, size.z));

            vertices.Add(new TSVector3(-size.x, -size.y, -size.z));
            vertices.Add(new TSVector3(-size.x, -size.y, size.z));
            vertices.Add(new TSVector3(-size.x, size.y, -size.z));
            vertices.Add(new TSVector3(-size.x, size.y, size.z));
        }
    }
}
