using GGPhys.Core;
using GGPhys.Rigid.Collisions;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;

namespace GGPhysUnity
{
    public class BMeshCollider : BCollider
    {
        public Mesh mesh;
        public int maxThreadCount = 32;
        private BTriangle[] triangles;
        public List<CollisionPrimitive> Primitives;

        public override void AddToEngine(BRigidBody bBody)
        {
            Primitives = new List<CollisionPrimitive>();
            AddTriangles(bBody);
        }

        public override Matrix3 CalculateInertiaTensor(FP mass)
        {
            return Matrix3.Zero;
        }

        private void OnValidate()
        {
            UpdateMesh();
        }

        void AddTriangles(BRigidBody bBody)
        {
            if (mesh == null) return;
            int size = mesh.triangles.Length;
            if (size % 3 != 0) return;
            int triangleCount = size / 3;
            triangles = new BTriangle[triangleCount];
            int[] meshTriangles = mesh.triangles;
            Vector3[] meshVertices = mesh.vertices;
            Vector3 lossyScale = gameObject.transform.lossyScale;

            for (int i = 0; i < size; i += 3)
            {
                int verticeIndexA = meshTriangles[i];
                int verticeIndexB = meshTriangles[i + 1];
                int verticeIndexC = meshTriangles[i + 2];
                Vector3 a = meshVertices[verticeIndexA].Multiply(lossyScale);
                Vector3 b = meshVertices[verticeIndexB].Multiply(lossyScale);
                Vector3 c = meshVertices[verticeIndexC].Multiply(lossyScale);

                BTriangle triangle = new BTriangle
                {
                    A = a,
                    B = b,
                    C = c
                };
                triangles[i / 3] = triangle;
            }

            for (int i = 0; i < triangles.Length; i++)
            {
                BTriangle triangle = triangles[i];
                CollisionTriangle shape = new CollisionTriangle(triangle.A, triangle.B, triangle.C)
                {
                    Body = bBody.Body,
                    Offset = Matrix4.IdentityOffset(CenterOffset /*- bBody.CenterOfMassOffset*/),
                    IsTrigger = IsTrigger,
                    CollisionLayer = (uint)bBody.collsionLayer,
                    CollisionMask = (uint)bBody.collsionMask
                };
                bBody.Body.Offset = CenterOffset;
                Primitives.Add(shape);
                RigidPhysicsEngine.Instance.Collisions.AddPrimitive(shape);
            }

            triangles = null;
        }

        void UpdateMesh()
        {
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                mesh = meshFilter.sharedMesh;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (mesh == null)
                UpdateMesh();
            if (mesh == null)
                return;
            if (transform == null)
                transform = GetComponent<TSTransform>();
            if (transform == null)
                return;
            Gizmos.color = new Color(0, 128, 255);
            Gizmos.DrawWireMesh(mesh, transform.position + transform.TransformDirection(CenterOffset), transform.rotation, transform.localScale);
        }
    }

    public struct BTriangle
    {
        public TSVector3 A;
        public TSVector3 B;
        public TSVector3 C;
    }
}

