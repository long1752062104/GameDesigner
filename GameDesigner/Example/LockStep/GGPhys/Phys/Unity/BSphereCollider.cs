using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGPhys.Rigid.Collisions;
using GGPhys.Core;
using REAL = FixMath.FP;

namespace GGPhysUnity
{

    public class BSphereCollider : BCollider
    {
        public float radius;

        public override void AddToEngine(BRigidBody bBody)
        {
            base.AddToEngine(bBody);

            var shape = new CollisionSphere(radius);
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
            REAL fRadius = radius;
            var inertiaTensor = Matrix3.Identity;
            inertiaTensor.data0 *= 0.4 * mass * (fRadius * fRadius);
            inertiaTensor.data4 *= 0.4 * mass * (fRadius * fRadius);
            inertiaTensor.data8 *= 0.4 * mass * (fRadius * fRadius);
            return inertiaTensor;
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0, 128 ,255);
            Gizmos.DrawWireSphere(transform.position + transform.TransformDirection(CenterOffset), radius);
        }
    }

}
