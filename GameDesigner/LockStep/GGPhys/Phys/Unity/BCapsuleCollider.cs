using GGPhys.Core;
using GGPhys.Rigid.Collisions;
using TrueSync;
using UnityEngine;

namespace GGPhysUnity
{
    public class BCapsuleCollider : BCollider
    {
        public FP radius = 1f;
        public FP height = 1f;

        public override void AddToEngine(BRigidBody bBody)
        {
            CollisionCapsule shape = new CollisionCapsule(radius, height)
            {
                Body = bBody.Body,
                Offset = Matrix4.IdentityOffset(CenterOffset /*- bBody.CenterOfMassOffset*/),
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
            FP fHeight = height;
            FP fRadius = radius;
            Matrix3 inertiaTensor = Matrix3.Identity;
            FP Ixx = mass * ((3 * fRadius + 2 * fHeight) * 0.125) * fHeight;
            inertiaTensor.data0 *= Ixx;
            inertiaTensor.data4 *= 0.4 * mass * (fRadius * fRadius);
            inertiaTensor.data8 *= Ixx;
            return inertiaTensor;
        }

        private void OnDrawGizmos()
        {
            if (transform == null)
                transform = GetComponent<TSTransform>();
            TSVector3 offsetOne = new TSVector3(CenterOffset.x, CenterOffset.y + 0.5 * height + radius, CenterOffset.z);
            TSVector3 offsetTwo = new TSVector3(CenterOffset.x, CenterOffset.y - 0.5 * height - radius, CenterOffset.z);
            TSVector3 centerOneWorld = transform.position + transform.TransformDirection(offsetOne);
            TSVector3 centerTwoWorld = transform.position + transform.TransformDirection(offsetTwo);
            DebugExtension.DrawCapsule(centerOneWorld, centerTwoWorld, Color.green, (float)radius);
        }

        public void Reset()
        {
            CapsuleCollider box = GetComponent<CapsuleCollider>();
            if (box == null)
            {
                box = gameObject.AddComponent<CapsuleCollider>();
                CenterOffset = box.center;
                radius = box.radius;
                height = box.height / 2f;
                DestroyImmediate(box, true);
            }
            else
            {
                CenterOffset = box.center;
                radius = box.radius;
                height = box.height / 2f;
            }
        }
    }
}
