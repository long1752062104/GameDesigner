using GGPhys.Core;
using GGPhys.Rigid.Collisions;
using TrueSync;
using UnityEngine;

namespace GGPhysUnity
{

    public class BSphereCollider : BCollider
    {
        public FP radius;

        public override void AddToEngine(BRigidBody bBody)
        {
            CollisionSphere shape = new CollisionSphere(radius)
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
            FP fRadius = radius;
            Matrix3 inertiaTensor = Matrix3.Identity;
            inertiaTensor.data0 *= 0.4f * mass * (fRadius * fRadius);
            inertiaTensor.data4 *= 0.4f * mass * (fRadius * fRadius);
            inertiaTensor.data8 *= 0.4f * mass * (fRadius * fRadius);
            return inertiaTensor;
        }


        private void OnDrawGizmos()
        {
            if (transform == null)
                transform = GetComponent<TSTransform>();
            Gizmos.color = new Color(0, 128, 255);
            Gizmos.DrawWireSphere(transform.position + transform.TransformDirection(CenterOffset), (float)radius);
        }

        public void Reset()
        {
            if (transform == null)
                transform = GetComponent<TSTransform>();
            Vector3 size = gameObject.transform.localScale;
            size = new TSVector3(TSMathf.Abs(size.x), TSMathf.Abs(size.y), TSMathf.Abs(size.z));
            transform.localScale = size;
            SphereCollider sc = GetComponent<SphereCollider>();
            if (sc == null)
            {
                sc = gameObject.AddComponent<SphereCollider>();
                radius = sc.radius * ((size.x + size.y + size.z) / 3f);
                DestroyImmediate(sc, true);
            }
            else
            {
                radius = sc.radius * ((size.x + size.y + size.z) / 3f);
            }
        }
    }

}
