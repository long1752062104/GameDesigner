using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGPhys.Core;
using GGPhys.Rigid.Collisions;
using REAL = FixMath.FP;

namespace GGPhysUnity
{

    public class BCapsuleCollider : BCollider
    {
        public float radius;
        public float height;

        public override void AddToEngine(BRigidBody bBody)
        {
            base.AddToEngine(bBody);

            var shape = new CollisionCapsule(radius, height);
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
            REAL fHeight = height;
            REAL fRadius = radius;
            var inertiaTensor = Matrix3.Identity;
            //REAL Ixx = mass * ((3 * fRadius + 2 * fHeight) * 0.125) * fHeight;
            //inertiaTensor.data0 *= Ixx;
            //inertiaTensor.data4 *= 0.4 * mass * (fRadius * fRadius);
            //inertiaTensor.data8 *= Ixx;
            return inertiaTensor;
        }


        private void OnDrawGizmosSelected()
        {
            var offsetOne = new Vector3d(CenterOffset.x, CenterOffset.y + 0.5 * height + radius, CenterOffset.z);
            var offsetTwo = new Vector3d(CenterOffset.x, CenterOffset.y - 0.5 * height - radius, CenterOffset.z);
            var centerOneWorld = transform.position + transform.TransformDirection(offsetOne.ToVector3());
            var centerTwoWorld = transform.position + transform.TransformDirection(offsetTwo.ToVector3());
            DebugExtension.DrawCapsule(centerOneWorld, centerTwoWorld, new Color(0, 128, 255), radius);
            if (Primitive == null)
                return;
            var bounds = Primitive.BoundingVolum;
            if (bounds == null)
                return;
            DebugExtension.DrawBounds(new Bounds() { 
                min = new Vector3(bounds.minX, bounds.minY, bounds.minZ),
                max = new Vector3(bounds.maxX, bounds.maxY, bounds.maxZ),
            });
        }



    }

}
