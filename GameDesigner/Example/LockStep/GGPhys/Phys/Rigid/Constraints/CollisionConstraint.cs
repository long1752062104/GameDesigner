using GGPhys.Rigid.Collisions;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrueSync;

namespace GGPhys.Rigid.Constraints
{
    /// <summary>
    /// 碰撞约束类
    /// </summary>
    public class CollisionConstraint : RigidConstraint
    {
        public CollisionData CollisionData;
        private List<CollisionPrimitive> m_Primatives;
        private GridManager m_GridManager;

        public int MaxThread = 32;
        public TSVector3 GridSize;
        public TSVector3 GridCellSize;
        public int GridGroupScale = 4;

        public CollisionConstraint(TSVector3 gridSize, TSVector3 gridCellSize, TSVector3 gridCenterOffset)
        {
            GridCellSize = gridCellSize;
            CollisionData = new CollisionData();
            m_Primatives = new List<CollisionPrimitive>();
            m_GridManager = new GridManager((int)gridSize.x, (int)gridSize.y, (int)gridSize.z, gridCellSize, gridCenterOffset, GridGroupScale);
            m_GridManager.InitGrids();
        }

        /// <summary>
        /// 加入受约束的几何体
        /// </summary>
        /// <param name="primitive"></param>
        public void AddPrimitive(CollisionPrimitive primitive)
        {
            primitive.CalculateInternals();//计算包围盒大小和世界顶点
            if (primitive.Body.IsStatic)
            {
                m_GridManager.AddStaticPrimitive(primitive);
            }
            else
            {
                m_Primatives.Add(primitive);
            }
        }

        /// <summary>
        /// 移除受约束的几何体
        /// </summary>
        /// <param name="primitive"></param>
        public void RemovePrimitive(CollisionPrimitive primitive)
        {
            if (primitive.Body.IsStatic)
            {
                m_GridManager.RemoveStaticPrimitive(primitive);
            }
            else
            {
                m_Primatives.Remove(primitive);
            }
        }

        /// <summary>
        /// 更新几何体信息
        /// </summary>
        public void UpdatePrimitives()
        {
            for (int i = 0; i < m_Primatives.Count; i++)
            {
                CollisionPrimitive primitive = m_Primatives[i];
                if (!primitive.Body.GetAwake() || primitive.Body.IsStatic)
                    continue;
                primitive.CalculateInternals();
                primitive.HashOrder = i;
                m_GridManager.AddPrimitive(primitive);
            }
        }

        /// <summary>
        /// 生成碰撞数据
        /// </summary>
        public override void GenerateContacts()
        {
            UpdatePrimitives();//更新几何体数据
            BroadPhase(); //粗略检测
            NarrowPhase(); //精确检测 GC
        }

        /// <summary>
        /// 射线检测
        /// </summary>
        /// <param name="contactBody">第一个碰撞到的刚体</param>
        /// <param name="contactPoint">碰撞点</param>
        /// <returns></returns>
        public bool RayCastStatic(TSVector3 start, TSVector3 direction, FP distance, uint layerMask, ref RigidBody contactBody, ref TSVector3 contactPoint)
        {
            CollisionRay ray = new CollisionRay();
            TSVector3 dirNormal = direction.Normalized;
            ray.Init(start, start + dirNormal * distance, dirNormal, layerMask);

            while (true)
            {
                Grid grid = m_GridManager.NextRayGrid(ray);
                if (grid == null)
                {
                    return false;
                }
                if (DetectRayInGrid(ray, grid, ref contactBody, ref contactPoint))
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// 粗略检测阶段
        /// </summary>
        void BroadPhase()
        {
            m_GridManager.GeneratePotentialContacts(CollisionData);
        }

        /// <summary>
        /// 精确检测阶段
        /// </summary>
        void NarrowPhase()
        {
            while (CollisionData.PotentialContacts.Count > 0)
            {
                RigidContactPotential pContact = CollisionData.PotentialContacts[0];
                DetectCollisions(pContact);
                if (pContact.type == 1)
                {
                    CollisionData.BodiesTrigger(pContact.Primitive1.Body, pContact.Primitive2.Body);
                }
                if (pContact.type == 2)
                {
                    pContact.CalculateInternals();
                    RigidContact contact = CollisionData.GetContact();
                    contact.SetData(pContact);
                }
                CollisionData.PotentialContactsMap.Remove(pContact.Hash);
                CollisionData.RecyclePotentialContact(pContact);
                CollisionData.PotentialContacts.RemoveAt(0);
            }
        }

        void DetectCollisions(RigidContactPotential potentialContact)
        {
            switch (potentialContact.Primitive1)
            {
                case CollisionSphere sphere:
                    DetectCollisions(sphere, potentialContact.Primitive2, potentialContact);
                    break;
                case CollisionBox box:
                    DetectCollisions(box, potentialContact.Primitive2, potentialContact);
                    break;
                case CollisionCapsule capsule:
                    DetectCollisions(capsule, potentialContact.Primitive2, potentialContact);
                    break;
                case CollisionConvex convex:
                    DetectCollisions(convex, potentialContact.Primitive2, potentialContact);
                    break;
                case CollisionTriangle triangle:
                    DetectCollisions(triangle, potentialContact.Primitive2, potentialContact);
                    break;
                default:
                    break;
            }
        }


        private void DetectCollisions(CollisionSphere sphere, CollisionPrimitive primitive, RigidContactPotential potentialContact)
        {
            switch (primitive)
            {
                case CollisionSphere sphere2:
                    if (sphere.IsTrigger || sphere2.IsTrigger)
                        IntersectionTests.SphereAndSphere(sphere, sphere2, potentialContact);
                    else
                        CollisionDetector.SphereAndSphere(sphere, sphere2, potentialContact);
                    break;

                case CollisionBox box:
                    potentialContact.Swap();
                    if (sphere.IsTrigger || box.IsTrigger)
                        IntersectionTests.BoxAndSphere(box, sphere, potentialContact);
                    else
                        CollisionDetector.BoxAndSphere(box, sphere, potentialContact);
                    break;

                case CollisionCapsule capsule:
                    potentialContact.Swap();
                    if (sphere.IsTrigger || capsule.IsTrigger)
                        IntersectionTests.CapsuleAndSphere(capsule, sphere, potentialContact);
                    else
                        CollisionDetector.CapsuleAndSphere(capsule, sphere, potentialContact);
                    break;
                case CollisionConvex convex:
                    potentialContact.Swap();
                    if (sphere.IsTrigger || convex.IsTrigger)
                        IntersectionTests.ConvexAndSphere(convex, sphere, potentialContact);
                    else
                        CollisionDetector.ConvexAndSphere(convex, sphere, potentialContact);
                    break;
                case CollisionTriangle triangle:
                    potentialContact.Swap();
                    if (sphere.IsTrigger || triangle.IsTrigger)
                        IntersectionTests.TriangleAndSphere(triangle, sphere, potentialContact);
                    else
                        CollisionDetector.TriangleAndSphere(triangle, sphere, potentialContact);
                    break;
            }
        }

        private void DetectCollisions(CollisionBox box, CollisionPrimitive primitive, RigidContactPotential potentialContact)
        {
            switch (primitive)
            {
                case CollisionSphere sphere:
                    if (box.IsTrigger || sphere.IsTrigger)
                        IntersectionTests.BoxAndSphere(box, sphere, potentialContact);
                    else
                        CollisionDetector.BoxAndSphere(box, sphere, potentialContact);
                    break;

                case CollisionBox box2:
                    if (box.IsTrigger || box2.IsTrigger)
                        IntersectionTests.BoxAndBox(box, box2, potentialContact);
                    else
                        CollisionDetector.BoxAndBox(box, box2, potentialContact);
                    break;

                case CollisionCapsule capsule:
                    potentialContact.Swap();
                    if (box.IsTrigger || capsule.IsTrigger)
                        IntersectionTests.CapsuleAndBox(capsule, box, potentialContact);
                    else
                        CollisionDetector.CapsuleAndBox(capsule, box, potentialContact);
                    break;
                case CollisionConvex convex:
                    potentialContact.Swap();
                    if (box.IsTrigger || convex.IsTrigger)
                        IntersectionTests.ConvexAndBox(convex, box, potentialContact);
                    else
                        CollisionDetector.ConvexAndBox(convex, box, potentialContact);
                    break;
                case CollisionTriangle triangle:
                    potentialContact.Swap();
                    if (box.IsTrigger || triangle.IsTrigger)
                        IntersectionTests.TriangleAndBox(triangle, box, potentialContact);
                    else
                        CollisionDetector.TriangleAndBox(triangle, box, potentialContact);
                    break;
            }
        }

        private void DetectCollisions(CollisionCapsule capsule, CollisionPrimitive primitive, RigidContactPotential potentialContact)
        {
            switch (primitive)
            {
                case CollisionSphere sphere:
                    if (capsule.IsTrigger || sphere.IsTrigger)
                        IntersectionTests.CapsuleAndSphere(capsule, sphere, potentialContact);
                    else
                        CollisionDetector.CapsuleAndSphere(capsule, sphere, potentialContact);
                    break;

                case CollisionBox box:
                    if (capsule.IsTrigger || box.IsTrigger)
                        IntersectionTests.CapsuleAndBox(capsule, box, potentialContact);
                    else
                        CollisionDetector.CapsuleAndBox(capsule, box, potentialContact);
                    break;

                case CollisionCapsule capsule2:
                    if (capsule.IsTrigger || capsule2.IsTrigger)
                        IntersectionTests.CapsuleAndCapsule(capsule, capsule2, potentialContact);
                    else
                        CollisionDetector.CapsuleAndCapsule(capsule, capsule2, potentialContact);
                    break;
                case CollisionConvex convex:
                    potentialContact.Swap();
                    if (capsule.IsTrigger || convex.IsTrigger)
                        IntersectionTests.ConvexAndCapsule(convex, capsule, potentialContact);
                    else
                        CollisionDetector.ConvexAndCapsule(convex, capsule, potentialContact);
                    break;
                case CollisionTriangle triangle:
                    potentialContact.Swap();
                    if (capsule.IsTrigger || triangle.IsTrigger)
                        IntersectionTests.TriangleAndCapsule(triangle, capsule, potentialContact);
                    else
                        CollisionDetector.TriangleAndCapsule(triangle, capsule, potentialContact);
                    break;
            }
        }

        private void DetectCollisions(CollisionConvex convex, CollisionPrimitive primitive, RigidContactPotential potentialContact)
        {
            switch (primitive)
            {
                case CollisionSphere sphere:
                    if (convex.IsTrigger || sphere.IsTrigger)
                        IntersectionTests.ConvexAndSphere(convex, sphere, potentialContact);
                    else
                        CollisionDetector.ConvexAndSphere(convex, sphere, potentialContact);
                    break;
                case CollisionBox box:
                    if (convex.IsTrigger || box.IsTrigger)
                        IntersectionTests.ConvexAndBox(convex, box, potentialContact);
                    else
                        CollisionDetector.ConvexAndBox(convex, box, potentialContact);
                    break;
                case CollisionCapsule capsule:
                    if (convex.IsTrigger || capsule.IsTrigger)
                        IntersectionTests.ConvexAndCapsule(convex, capsule, potentialContact);
                    else
                        CollisionDetector.ConvexAndCapsule(convex, capsule, potentialContact);
                    break;
                case CollisionConvex convex2:
                    if (convex.IsTrigger || convex2.IsTrigger)
                        IntersectionTests.ConvexAndConvex(convex, convex2, potentialContact);
                    else
                        CollisionDetector.ConvexAndConvex(convex, convex2, potentialContact);
                    break;
                case CollisionTriangle triangle:
                    potentialContact.Swap();
                    if (convex.IsTrigger || triangle.IsTrigger)
                        IntersectionTests.TriangleAndConvex(triangle, convex, potentialContact);
                    else
                        CollisionDetector.TriangleAndConvex(triangle, convex, potentialContact);
                    break;
            }
        }

        private void DetectCollisions(CollisionTriangle triangle, CollisionPrimitive primitive, RigidContactPotential potentialContact)
        {
            switch (primitive)
            {
                case CollisionSphere sphere:
                    if (triangle.IsTrigger || sphere.IsTrigger)
                        IntersectionTests.TriangleAndSphere(triangle, sphere, potentialContact);
                    else
                        CollisionDetector.TriangleAndSphere(triangle, sphere, potentialContact);
                    break;
                case CollisionBox box:
                    if (triangle.IsTrigger || box.IsTrigger)
                        IntersectionTests.TriangleAndBox(triangle, box, potentialContact);
                    else
                        CollisionDetector.TriangleAndBox(triangle, box, potentialContact);
                    break;

                case CollisionCapsule capsule:
                    if (triangle.IsTrigger || capsule.IsTrigger)
                        IntersectionTests.TriangleAndCapsule(triangle, capsule, potentialContact);
                    else
                        CollisionDetector.TriangleAndCapsule(triangle, capsule, potentialContact);
                    break;
                case CollisionConvex convex:
                    if (triangle.IsTrigger || convex.IsTrigger)
                        IntersectionTests.TriangleAndConvex(triangle, convex, potentialContact);
                    else
                        CollisionDetector.TriangleAndConvex(triangle, convex, potentialContact);
                    break;
            }
        }


        /// <summary>
        /// 射线在整个格子中进行检测
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="grid"></param>
        /// <param name="contactBody"></param>
        /// <param name="contactPoint"></param>
        /// <returns></returns>
        private bool DetectRayInGrid(CollisionRay ray, Grid grid, ref RigidBody contactBody, ref TSVector3 contactPoint)
        {
            FP minDist = FP.MaxValue;
            RigidBody body = null;

            LinkedNode<CollisionPrimitive> node = grid.HeadPrimitive;
            while (node != null)
            {
                FP distance = FP.MaxValue;
                switch (node.obj)
                {
                    case CollisionSphere sphere:
                        CollisionDetector.RayAndSphere(ray, sphere, ref distance);
                        break;
                    case CollisionBox box:
                        CollisionDetector.RayAndBox(ray, box, ref distance);
                        break;
                    case CollisionCapsule capsule:
                        CollisionDetector.RayAndCapsule(ray, capsule, ref distance);
                        break;
                    case CollisionConvex convex:
                        CollisionDetector.RayAndConvex(ray, convex, ref distance);
                        break;
                    case CollisionTriangle triangle:
                        CollisionDetector.RayAndTriangle(ray, triangle, ref distance);
                        break;
                    default:
                        break;
                }
                if (distance < minDist)
                {
                    minDist = distance;
                    body = node.obj.Body;
                }
                node = node.next;
            }

            if (minDist != FP.MaxValue)
            {
                contactBody = body;
                contactPoint = ray.start + ray.direction * minDist;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
