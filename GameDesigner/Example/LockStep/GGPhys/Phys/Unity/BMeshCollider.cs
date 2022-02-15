using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GGPhys.Core;
using GGPhys.Rigid.Collisions;
using UnityEngine;
using REAL = FixMath.FP;

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
            base.AddToEngine(bBody);
            Primitives = new List<CollisionPrimitive>();
            AddTriangles(bBody);
            
        }

        public override Matrix3 CalculateInertiaTensor(REAL mass)
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
            var size = mesh.triangles.Length;
            if (size % 3 != 0) return;
            int triangleCount = size / 3;
            triangles = new BTriangle[triangleCount];
            var meshTriangles = mesh.triangles;
            var meshVertices = mesh.vertices;
            var lossyScale = transform.lossyScale.ToVector3d();
            int batchCount = (size / (maxThreadCount * 3)) * 3;
            if(batchCount == 0)
            {
                for (int i = 0; i < size; i += 3)
                {
                    var verticeIndexA = meshTriangles[i];
                    var verticeIndexB = meshTriangles[i + 1];
                    var verticeIndexC = meshTriangles[i + 2];
                    var a = meshVertices[verticeIndexA].ToVector3d() * lossyScale;
                    var b = meshVertices[verticeIndexB].ToVector3d() * lossyScale;
                    var c = meshVertices[verticeIndexC].ToVector3d() * lossyScale;

                    var triangle = new BTriangle();
                    triangle.A = a;
                    triangle.B = b;
                    triangle.C = c;
                    triangles[i / 3] = triangle;
                }
            }
            else //多线程加速
            {
                int taskCount = size % batchCount == 0 ? size / batchCount : size / batchCount + 1;
                Task[] tasks = new Task[taskCount];
                for (int k = 0; k < size; k += batchCount)
                {
                    int start = k;
                    int end = start + batchCount;
                    tasks[start / batchCount] = Task.Factory.StartNew(() =>
                    {
                        for (int i = start; (i < size && i < end); i += 3)
                        {
                            var verticeIndexA = meshTriangles[i];
                            var verticeIndexB = meshTriangles[i + 1];
                            var verticeIndexC = meshTriangles[i + 2];
                            var a = meshVertices[verticeIndexA].ToVector3d() * lossyScale;
                            var b = meshVertices[verticeIndexB].ToVector3d() * lossyScale;
                            var c = meshVertices[verticeIndexC].ToVector3d() * lossyScale;

                            var triangle = new BTriangle();
                            triangle.A = a;
                            triangle.B = b;
                            triangle.C = c;
                            triangles[i / 3] = triangle;
                        }
                    });
                }

                Task.WaitAll(tasks);
            }
            

            for (int i = 0; i < triangles.Length; i++)
            {
                var triangle = triangles[i];
                var shape = new CollisionTriangle(triangle.A, triangle.B, triangle.C);
                shape.Body = bBody.Body;
                shape.Offset = Matrix4.IdentityOffset(CenterOffset.ToVector3d() - bBody.CenterOfMassOffset);
                shape.IsTrigger = IsTrigger;
                shape.CollisionLayer = (uint)bBody.collsionLayer;
                shape.CollisionMask = (uint)bBody.collsionMask;
                Primitives.Add(shape);
                RigidPhysicsEngine.Instance.Collisions.AddPrimitive(shape);
            }

            triangles = null;
        }

        void UpdateMesh()
        {
            var meshFilter = gameObject.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                mesh = meshFilter.sharedMesh;
            }
        }
    }

    public struct BTriangle
    {
        public Vector3d A;
        public Vector3d B;
        public Vector3d C;
    }
}

