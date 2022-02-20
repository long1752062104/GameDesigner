using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGPhys.Rigid.Collisions;
using REAL = FixMath.FP;
using GGPhys.Core;

namespace GGPhysUnity
{

    public class BBoxCollider : BCollider
    {
        public Vector3 halfSize;

        public override void AddToEngine(BRigidBody bBody)
        {
            base.AddToEngine(bBody);

            var shape = new CollisionBox(halfSize.ToVector3d());
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
            Vector3d fHalfSize = halfSize.ToVector3d();
            var inertiaTensor = Matrix3.Identity;
            inertiaTensor.data0 = 1.5 * mass * (fHalfSize.y * fHalfSize.y + fHalfSize.z * fHalfSize.z);
            inertiaTensor.data4 = 1.5 * mass * (fHalfSize.x * fHalfSize.x + fHalfSize.z * fHalfSize.z);
            inertiaTensor.data8 = 1.5 * mass * (fHalfSize.x * fHalfSize.x + fHalfSize.y * fHalfSize.y);
            return inertiaTensor;
        }

        private void OnValidate()
        {
            if (halfSize.ToVector3d() != Vector3d.Zero) return;
            halfSize = transform.TransformDirection(transform.localScale * 0.5f);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0, 128, 255);
            int[,] mults = new int[,]
            {
                {1,1,1},{-1,1,1},{1,-1,1},{-1,-1,1},
                {1,1,-1},{-1,1,-1},{1,-1,-1},{-1,-1,-1}
            };
            Vector3[] vertexPosArray = new Vector3[8];
            for (int i = 0; i < 8; i++)
            {
                Vector3d vertexPos = new Vector3d(mults[i, 0], mults[i, 1], mults[i, 2]);
                vertexPos *= halfSize.ToVector3d();
                vertexPos = transform.position.ToVector3d() + transform.TransformDirection((CenterOffset.ToVector3d() + vertexPos).ToVector3()).ToVector3d();
                vertexPosArray[i] = vertexPos.ToVector3();
            }
            Gizmos.DrawLine(vertexPosArray[0], vertexPosArray[1]);
            Gizmos.DrawLine(vertexPosArray[0], vertexPosArray[2]);
            Gizmos.DrawLine(vertexPosArray[0], vertexPosArray[4]);
            Gizmos.DrawLine(vertexPosArray[1], vertexPosArray[3]);
            Gizmos.DrawLine(vertexPosArray[1], vertexPosArray[5]);
            Gizmos.DrawLine(vertexPosArray[2], vertexPosArray[3]);
            Gizmos.DrawLine(vertexPosArray[2], vertexPosArray[6]);
            Gizmos.DrawLine(vertexPosArray[3], vertexPosArray[7]);
            Gizmos.DrawLine(vertexPosArray[4], vertexPosArray[5]);
            Gizmos.DrawLine(vertexPosArray[4], vertexPosArray[6]);
            Gizmos.DrawLine(vertexPosArray[5], vertexPosArray[7]);
            Gizmos.DrawLine(vertexPosArray[6], vertexPosArray[7]);
            if (Primitive == null)
                return;
            var bounds = Primitive.BoundingVolum;
            if (bounds == null)
                return;
            var rigidBody = GetComponent<BRigidBody>();
            var isAwake = rigidBody ? rigidBody.Body.GetAwake() : false;
            var color = isAwake ? Color.green : Color.white;
            DebugExtension.DrawBounds(new Bounds()
            {
                min = new Vector3(bounds.minX, bounds.minY, bounds.minZ),
                max = new Vector3(bounds.maxX, bounds.maxY, bounds.maxZ),
            }, color);
        }
    }

}
