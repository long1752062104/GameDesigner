using GGPhys.Core;
using GGPhys.Rigid.Collisions;
using TrueSync;
using UnityEngine;

namespace GGPhysUnity
{
    public class BBoxCollider : BCollider
    {
        public TSVector3 halfSize;

        public override void AddToEngine(BRigidBody bBody)
        {
            CollisionBox shape = new CollisionBox(halfSize)
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
            TSVector3 fHalfSize = halfSize;
            Matrix3 inertiaTensor = Matrix3.Identity;
            inertiaTensor.data0 = 1.5 * mass * (fHalfSize.y * fHalfSize.y + fHalfSize.z * fHalfSize.z);
            inertiaTensor.data4 = 1.5 * mass * (fHalfSize.x * fHalfSize.x + fHalfSize.z * fHalfSize.z);
            inertiaTensor.data8 = 1.5 * mass * (fHalfSize.x * fHalfSize.x + fHalfSize.y * fHalfSize.y);
            return inertiaTensor;
        }

        private void OnDrawGizmos()
        {
            if (!showGizmos)
                return;
            if (transform == null)
                transform = GetComponent<TSTransform>();
            Gizmos.color = Color.green;
            //int[,] mults = new int[,]
            //{
            //    {1,1,1},{-1,1,1},{1,-1,1},{-1,-1,1},
            //    {1,1,-1},{-1,1,-1},{1,-1,-1},{-1,-1,-1}
            //};
            //TSVector3[] vertexPosArray = new TSVector3[8];
            //for (int i = 0; i < 8; i++)
            //{
            //    TSVector3 vertexPos = new TSVector3(mults[i, 0], mults[i, 1], mults[i, 2]);
            //    vertexPos = TSVector3.Multiply(vertexPos, halfSize);
            //    vertexPos = transform.position + transform.TransformDirection(CenterOffset + vertexPos);
            //    vertexPosArray[i] = vertexPos;
            //}
            //Gizmos.DrawLine(vertexPosArray[0], vertexPosArray[1]);
            //Gizmos.DrawLine(vertexPosArray[0], vertexPosArray[2]);
            //Gizmos.DrawLine(vertexPosArray[0], vertexPosArray[4]);
            //Gizmos.DrawLine(vertexPosArray[1], vertexPosArray[3]);
            //Gizmos.DrawLine(vertexPosArray[1], vertexPosArray[5]);
            //Gizmos.DrawLine(vertexPosArray[2], vertexPosArray[3]);
            //Gizmos.DrawLine(vertexPosArray[2], vertexPosArray[6]);
            //Gizmos.DrawLine(vertexPosArray[3], vertexPosArray[7]);
            //Gizmos.DrawLine(vertexPosArray[4], vertexPosArray[5]);
            //Gizmos.DrawLine(vertexPosArray[4], vertexPosArray[6]);
            //Gizmos.DrawLine(vertexPosArray[5], vertexPosArray[7]);
            //Gizmos.DrawLine(vertexPosArray[6], vertexPosArray[7]);
            if (mesh == null)
            {
                MeshFilter mf = GetComponent<MeshFilter>();
                if (mf == null)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Mesh mesh1 = cube.GetComponent<MeshFilter>().sharedMesh;
                    mesh = Instantiate(mesh1);
                    DestroyImmediate(cube, true);
                }
                else mesh = mf.sharedMesh;
            }
            Gizmos.DrawWireMesh(mesh, transform.TransformPoint(CenterOffset), transform.rotation, halfSize * 2f);
        }

        private Mesh mesh;

        private void Reset()
        {
            BoxCollider box = GetComponent<BoxCollider>();
            if (box == null)
            {
                box = gameObject.AddComponent<BoxCollider>();
                CenterOffset = box.center;
                halfSize = (gameObject.transform.localScale / 2).Multiply(box.size);
                DestroyImmediate(box, true);
            }
            else
            {
                CenterOffset = box.center;
                halfSize = (gameObject.transform.localScale / 2).Multiply(box.size);
            }
        }
    }

}
