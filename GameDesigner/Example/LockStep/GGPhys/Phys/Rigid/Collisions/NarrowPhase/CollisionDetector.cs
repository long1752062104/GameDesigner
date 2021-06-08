using System;
using TrueSync;

namespace GGPhys.Rigid.Collisions
{
    /// <summary>
    /// 精确碰撞检测方法类
    /// </summary>
    public static class CollisionDetector
    {
        public static void SphereAndSphere(CollisionSphere one, CollisionSphere two, RigidContactPotential potentialContact)
        {
            // 获取位置
            TSVector3 positionOne = one.GetAxis(3);
            TSVector3 positionTwo = two.GetAxis(3);

            TSVector3 midline = positionOne - positionTwo;
            FP size = midline.Magnitude;

            // 看距离是否满足
            if (size <= 0.0f || size >= one.Radius + two.Radius)
                return;

            TSVector3 normal = midline * (1.0 / size);

            FP friction = 0.5 * (one.Body.Friction + two.Body.Friction);
            FP restitution = 0.5 * (one.Body.Restitution + two.Body.Restitution);

            potentialContact.type = 2;
            potentialContact.Friction = friction;
            potentialContact.Restitution = restitution;
            potentialContact.ContactNormal = normal;
            potentialContact.ContactPoint = positionTwo + midline * 0.5;
            potentialContact.Penetration = one.Radius + two.Radius - size;

        }

        public static void BoxAndBox(CollisionBox one, CollisionBox two, RigidContactPotential potentialContact)
        {
            TSVector3 toCentre = two.GetAxis(3) - one.GetAxis(3);

            FP pen = FP.MaxValue; //相交深度
            int best = 0xffffff; //最佳相交轴序号

            // one的三个面分离轴测试
            if (!TryAxis(one, two, one.GetAxis(0), toCentre, 0, ref pen, ref best)) return;
            if (!TryAxis(one, two, one.GetAxis(1), toCentre, 1, ref pen, ref best)) return;
            if (!TryAxis(one, two, one.GetAxis(2), toCentre, 2, ref pen, ref best)) return;

            // two的三个面分离轴测试
            if (!TryAxis(one, two, two.GetAxis(0), toCentre, 3, ref pen, ref best)) return;
            if (!TryAxis(one, two, two.GetAxis(1), toCentre, 4, ref pen, ref best)) return;
            if (!TryAxis(one, two, two.GetAxis(2), toCentre, 5, ref pen, ref best)) return;

            // 9条边边叉积轴分离轴测试
            if (!TryAxis(one, two, TSVector3.Cross(one.GetAxis(0), two.GetAxis(0)), toCentre, 6, ref pen, ref best)) return;
            if (!TryAxis(one, two, TSVector3.Cross(one.GetAxis(0), two.GetAxis(1)), toCentre, 7, ref pen, ref best)) return;
            if (!TryAxis(one, two, TSVector3.Cross(one.GetAxis(0), two.GetAxis(2)), toCentre, 8, ref pen, ref best)) return;
            if (!TryAxis(one, two, TSVector3.Cross(one.GetAxis(1), two.GetAxis(0)), toCentre, 9, ref pen, ref best)) return;
            if (!TryAxis(one, two, TSVector3.Cross(one.GetAxis(1), two.GetAxis(1)), toCentre, 10, ref pen, ref best)) return;
            if (!TryAxis(one, two, TSVector3.Cross(one.GetAxis(1), two.GetAxis(2)), toCentre, 11, ref pen, ref best)) return;
            if (!TryAxis(one, two, TSVector3.Cross(one.GetAxis(2), two.GetAxis(0)), toCentre, 12, ref pen, ref best)) return;
            if (!TryAxis(one, two, TSVector3.Cross(one.GetAxis(2), two.GetAxis(1)), toCentre, 13, ref pen, ref best)) return;
            if (!TryAxis(one, two, TSVector3.Cross(one.GetAxis(2), two.GetAxis(2)), toCentre, 14, ref pen, ref best)) return;

            // 确保有结果
            if (best == 0xffffff)
                throw new Exception("best == 0xffffff");

            // 最佳相交轴为one的面轴
            if (best < 3)
            {
                FillPointFaceBoxBox(one, two, toCentre, potentialContact, best, pen);
            }
            else if (best < 6)
            {
                // 最佳相交轴为two的面轴
                potentialContact.Swap();
                FillPointFaceBoxBox(two, one, toCentre * -1.0, potentialContact, best - 3, pen);
            }
            else
            {
                // 最佳相交轴为边边轴
                best -= 6;
                int oneAxisIndex = best / 3;
                int twoAxisIndex = best % 3;
                TSVector3 oneAxis = one.GetAxis(oneAxisIndex);
                TSVector3 twoAxis = two.GetAxis(twoAxisIndex);
                TSVector3 axis = TSVector3.Cross(oneAxis, twoAxis);
                axis.Normalize();

                if (TSVector3.Dot(axis, toCentre) > 0) axis *= -1.0; //对于one来说normal为two到one,而toCenter默认为one到two

                // 找到两条边
                TSVector3 ptOnOneEdge1 = one.HalfSize;
                TSVector3 ptOnOneEdge2 = one.HalfSize;
                TSVector3 ptOnTwoEdge1 = two.HalfSize;
                TSVector3 ptOnTwoEdge2 = two.HalfSize;
                for (int i = 0; i < 3; i++)
                {
                    if (i == oneAxisIndex)
                    {
                        ptOnOneEdge2[i] = -ptOnOneEdge2[i];
                    }
                    else if (TSVector3.Dot(one.GetAxis(i), axis) > 0)
                    {
                        ptOnOneEdge1[i] = -ptOnOneEdge1[i];
                        ptOnOneEdge2[i] = -ptOnOneEdge2[i];
                    }

                    if (i == twoAxisIndex)
                    {
                        ptOnTwoEdge2[i] = -ptOnTwoEdge2[i];
                    }
                    else if (TSVector3.Dot(two.GetAxis(i), axis) < 0)
                    {
                        ptOnTwoEdge1[i] = -ptOnTwoEdge1[i];
                        ptOnTwoEdge2[i] = -ptOnTwoEdge2[i];
                    }
                }

                // 本地坐标转为世界坐标
                ptOnOneEdge1 = one.Transform * ptOnOneEdge1;
                ptOnOneEdge2 = one.Transform * ptOnOneEdge2;
                ptOnTwoEdge1 = two.Transform * ptOnTwoEdge1;
                ptOnTwoEdge2 = two.Transform * ptOnTwoEdge2;

                // 两条边的最近点作为碰撞点
                TSVector3 contactPoint = ClosestPointOnTwoLines(ptOnOneEdge1, ptOnOneEdge2, ptOnTwoEdge1, ptOnTwoEdge2);

                FP friction = 0.5 * (one.Body.Friction + two.Body.Friction);
                FP restitution = 0.5 * (one.Body.Restitution + two.Body.Restitution);

                // 碰撞数据赋值
                potentialContact.type = 2;
                potentialContact.Friction = friction;
                potentialContact.Restitution = restitution;
                potentialContact.Penetration = pen;
                potentialContact.ContactNormal = axis;
                potentialContact.ContactPoint = contactPoint;
            }
        }


        public static void BoxAndPoint(CollisionBox box, TSVector3 point, RigidContactPotential potentialContact)
        {
            // 把点转为盒体的坐标系
            TSVector3 relPt = box.Transform.TransformInverse(point);

            TSVector3 normal;

            // 找到最浅相交的面轴
            FP min_depth = box.HalfSize.x - FP.Abs(relPt.x);
            if (min_depth < 0) return;
            normal = box.GetAxis(0) * ((relPt.x < 0) ? -1 : 1);

            FP depth = box.HalfSize.y - FP.Abs(relPt.y);
            if (depth < 0) return;
            else if (depth < min_depth)
            {
                min_depth = depth;
                normal = box.GetAxis(1) * ((relPt.y < 0) ? -1 : 1);
            }

            depth = box.HalfSize.z - FP.Abs(relPt.z);
            if (depth < 0) return;
            else if (depth < min_depth)
            {
                min_depth = depth;
                normal = box.GetAxis(2) * ((relPt.z < 0) ? -1 : 1);
            }

            potentialContact.type = 2;
            potentialContact.ContactNormal = normal;
            potentialContact.ContactPoint = point;
            potentialContact.Penetration = min_depth;
            if (potentialContact.Penetration > 10000 | potentialContact.Penetration < -10000)
            {
                UnityEngine.Debug.Log("dsadsadsad");
            }
        }

        public static void BoxAndSphere(CollisionBox box, CollisionSphere sphere, RigidContactPotential potentialContact)
        {
            // 转为盒体坐标系
            TSVector3 centre = sphere.GetAxis(3);
            TSVector3 relCentre = box.Transform.TransformInverse(centre);

            // 提前退出检测
            if (FP.Abs(relCentre.x) - sphere.Radius > box.HalfSize.x ||
                FP.Abs(relCentre.y) - sphere.Radius > box.HalfSize.y ||
                FP.Abs(relCentre.z) - sphere.Radius > box.HalfSize.z)
            {
                return;
            }

            TSVector3 closestPt = new TSVector3(0, 0, 0);
            FP dist;

            // 用盒体的halfsize为relCentre进行clamp
            dist = relCentre.x;
            if (dist > box.HalfSize.x) dist = box.HalfSize.x;
            if (dist < -box.HalfSize.x) dist = -box.HalfSize.x;
            closestPt.x = dist;

            dist = relCentre.y;
            if (dist > box.HalfSize.y) dist = box.HalfSize.y;
            if (dist < -box.HalfSize.y) dist = -box.HalfSize.y;
            closestPt.y = dist;

            dist = relCentre.z;
            if (dist > box.HalfSize.z) dist = box.HalfSize.z;
            if (dist < -box.HalfSize.z) dist = -box.HalfSize.z;
            closestPt.z = dist;

            // 是否碰撞
            dist = (closestPt - relCentre).SqrMagnitude;
            if (dist > sphere.Radius * sphere.Radius) return;

            TSVector3 closestPtWorld = box.Transform.Transform(closestPt);

            FP friction = 0.5 * (box.Body.Friction + sphere.Body.Friction);
            FP restitution = 0.5 * (box.Body.Restitution + sphere.Body.Restitution);

            potentialContact.type = 2;
            potentialContact.Friction = friction;
            potentialContact.Restitution = restitution;
            potentialContact.ContactNormal = (closestPtWorld - centre).Normalized;
            potentialContact.ContactPoint = closestPtWorld;
            potentialContact.Penetration = sphere.Radius - FP.Sqrt(dist);
        }

        public static void CapsuleAndCapsule(CollisionCapsule capsule1, CollisionCapsule capsule2, RigidContactPotential potentialContact)
        {
            //要先把两个胶囊视为两条线段，找到两条线段的最近距离的两个点，然后以两个点做为球心，做球球检测

            TSVector3 bestA = TSVector3.Zero;
            TSVector3 bestB = TSVector3.Zero;
            ClosestPoinsOnTwoLines(capsule1.CenterOne, capsule1.CenterTwo, capsule2.CenterOne, capsule2.CenterTwo, ref bestA, ref bestB);

            //球球检测
            TSVector3 midline = bestA - bestB;
            FP size = midline.Magnitude;
            if (size <= 0.0f || size >= capsule1.Radius + capsule2.Radius)
                return;

            TSVector3 normal = midline * (1.0 / size);

            FP friction = 0.5 * (capsule1.Body.Friction + capsule2.Body.Friction);
            FP restitution = 0.5 * (capsule1.Body.Restitution + capsule2.Body.Restitution);

            potentialContact.type = 2;
            potentialContact.Friction = friction;
            potentialContact.Restitution = restitution;
            potentialContact.ContactNormal = normal;
            potentialContact.ContactPoint = bestA + midline * 0.5;
            potentialContact.Penetration = capsule1.Radius + capsule2.Radius - size;
        }

        public static void CapsuleAndSphere(CollisionCapsule capsule, CollisionSphere sphere, RigidContactPotential potentialContact)
        {

            //视胶囊为线段，找到线段离球心的最近点，做球球检测

            TSVector3 bestB = sphere.GetAxis(3);

            TSVector3 bestA = ClosestPointOnLineSegment(capsule.CenterOne, capsule.CenterTwo, bestB);

            TSVector3 midline = bestA - bestB;
            FP size = midline.Magnitude;

            if (size <= 0.0f || size >= capsule.Radius + sphere.Radius)
                return;

            TSVector3 normal = midline * (1.0 / size);

            FP friction = 0.5 * (capsule.Body.Friction + sphere.Body.Friction);
            FP restitution = 0.5 * (capsule.Body.Restitution + sphere.Body.Restitution);

            potentialContact.type = 2;
            potentialContact.Friction = friction;
            potentialContact.Restitution = restitution;
            potentialContact.ContactNormal = normal;
            potentialContact.ContactPoint = bestB + midline * 0.5;
            potentialContact.Penetration = capsule.Radius + sphere.Radius - size;
        }

        public static void CapsuleAndBox(CollisionCapsule capsule, CollisionBox box, RigidContactPotential potentialContact)
        {
            //用分离轴算法找出最浅相交的特征，然后确定是面碰撞，还是边碰撞，面碰撞做球面检测，边碰撞则找到胶囊离边的最近点，做球盒检测
            TSVector3 toCentre = box.GetAxis(3) - capsule.GetAxis(3);

            FP pen = FP.MaxValue;
            int best = 0xffffff;

            //box的3条对角斜轴
            TSVector3 edgeAxis3 = (box.GetAxis(0) + box.GetAxis(1)).Normalized;
            TSVector3 edgeAxis4 = (box.GetAxis(0) + box.GetAxis(2)).Normalized;
            TSVector3 edgeAxis5 = (box.GetAxis(1) + box.GetAxis(2)).Normalized;

            if (!TryAxis(capsule, box, edgeAxis3, toCentre, 3, ref pen, ref best)) return;
            if (!TryAxis(capsule, box, edgeAxis4, toCentre, 4, ref pen, ref best)) return;
            if (!TryAxis(capsule, box, edgeAxis5, toCentre, 5, ref pen, ref best)) return;

            int bestEdgeAxis = best;

            if (!TryAxis(capsule, box, box.GetAxis(0), toCentre, 0, ref pen, ref best)) return;
            if (!TryAxis(capsule, box, box.GetAxis(1), toCentre, 1, ref pen, ref best)) return;
            if (!TryAxis(capsule, box, box.GetAxis(2), toCentre, 2, ref pen, ref best)) return;


            if (best < 3)
            {
                TSVector3 bestAxis = box.GetAxis(best);
                int sign = FP.Sign(TSVector3.Dot(bestAxis, -toCentre));
                TSVector3 planeDirection = sign * bestAxis;

                //找到离碰撞面较近的一端
                TSVector3 position;
                if (TSVector3.Dot(capsule.CenterOneToTwo, planeDirection) > 0)
                {
                    position = capsule.CenterOne;
                }
                else
                {
                    position = capsule.CenterTwo;
                }

                TSVector3 FPPosition = box.Transform.TransformInverse(position);

                //如果该端点超出碰撞面，则找到最近边，视为与边的碰撞
                if (best == 0)
                {
                    if (FP.Abs(FPPosition.y) > box.HalfSize.y || FP.Abs(FPPosition.z) > box.HalfSize.z)
                        best = bestEdgeAxis;
                }
                if (best == 1)
                {
                    if (FP.Abs(FPPosition.x) > box.HalfSize.x || FP.Abs(FPPosition.z) > box.HalfSize.z)
                        best = bestEdgeAxis;
                }
                if (best == 2)
                {
                    if (FP.Abs(FPPosition.x) > box.HalfSize.x || FP.Abs(FPPosition.y) > box.HalfSize.y)
                        best = bestEdgeAxis;
                }

                if (best < 3)
                {
                    FP planeOffset = TSVector3.Dot(box.GetAxis(3), planeDirection) + box.HalfSize[best];

                    FP dist = TSVector3.Dot(planeDirection, position) - capsule.Radius - planeOffset;

                    if (dist >= 0) return;

                    FP friction = 0.5 * (capsule.Body.Friction + box.Body.Friction);
                    FP restitution = 0.5 * (capsule.Body.Restitution + box.Body.Restitution);

                    potentialContact.type = 2;
                    potentialContact.Friction = friction;
                    potentialContact.Restitution = restitution;
                    potentialContact.ContactNormal = planeDirection;
                    potentialContact.Penetration = -dist;
                    potentialContact.ContactPoint = position - planeDirection * (dist + capsule.Radius);
                }
            }

            if (best >= 3)
            {
                //找到最近边
                TSVector3 linePointA = TSVector3.Zero;
                TSVector3 linePointB = TSVector3.Zero;
                if (best == 3)
                {
                    int sign = FP.Sign(TSVector3.Dot(edgeAxis3, -toCentre));
                    linePointA = sign * box.HalfSize;
                    linePointB = linePointA;
                    linePointB.z *= -1;
                }
                if (best == 4)
                {
                    int sign = FP.Sign(TSVector3.Dot(edgeAxis4, -toCentre));
                    linePointA = sign * box.HalfSize;
                    linePointB = linePointA;
                    linePointB.y *= -1;
                }
                if (best == 5)
                {
                    int sign = FP.Sign(TSVector3.Dot(edgeAxis5, -toCentre));
                    linePointA = sign * box.HalfSize;
                    linePointB = linePointA;
                    linePointB.x *= -1;
                }
                linePointA = box.Transform.Transform(linePointA);
                linePointB = box.Transform.Transform(linePointB);
                TSVector3 bestPoint = ClosestPointFromLineTwo(linePointA, linePointB, capsule.CenterOne, capsule.CenterTwo);

                potentialContact.Swap();
                BoxAndSphere(box, bestPoint, capsule.Radius, capsule.Body, potentialContact);
            }
        }

        public static void ConvexAndConvex(CollisionConvex convex1, CollisionConvex convex2, RigidContactPotential potentialContact)
        {
            TSVector3 normal = TSVector3.Zero;
            TSVector3 contactPoint = TSVector3.Zero;
            FP pen = 0;
            GJKDetecotor detector = new GJKDetecotor();
            bool intersect = detector.GJK(convex1.Vertices, convex2.Vertices, ref normal, ref contactPoint, ref pen);

            if (!intersect) return;

            FP friction = 0.5 * (convex1.Body.Friction + convex2.Body.Friction);
            FP restitution = 0.5 * (convex1.Body.Restitution + convex2.Body.Restitution);

            // 创建碰撞对象并赋值数据
            potentialContact.type = 2;
            potentialContact.Friction = friction;
            potentialContact.Restitution = restitution;
            potentialContact.ContactNormal = normal;
            potentialContact.Penetration = pen;
            potentialContact.ContactPoint = contactPoint;
        }

        public static void ConvexAndBox(CollisionConvex convex, CollisionBox box, RigidContactPotential potentialContact)
        {
            TSVector3 normal = TSVector3.Zero;
            TSVector3 contactPoint = TSVector3.Zero;
            FP pen = 0;
            GJKDetecotor detector = new GJKDetecotor();
            bool intersect = detector.GJK(convex.Vertices, box.Vertices, ref normal, ref contactPoint, ref pen);

            if (!intersect) return;

            FP friction = 0.5 * (convex.Body.Friction + box.Body.Friction);
            FP restitution = 0.5 * (convex.Body.Restitution + box.Body.Restitution);

            // 创建碰撞对象并赋值数据
            potentialContact.type = 2;
            potentialContact.Friction = friction;
            potentialContact.Restitution = restitution;
            potentialContact.ContactNormal = normal;
            potentialContact.Penetration = pen;
            potentialContact.ContactPoint = contactPoint;
        }

        public static void ConvexAndSphere(CollisionConvex convex, CollisionSphere sphere, RigidContactPotential potentialContact)
        {
            TSVector3 normal = TSVector3.Zero;
            TSVector3 contactPoint = TSVector3.Zero;
            FP pen = 0;
            GJKDetecotor detector = new GJKDetecotor();
            bool intersect = detector.GJKDist(convex.Vertices, new TSVector3[] { sphere.GetAxis(3) }, ref normal, ref contactPoint, ref pen);

            if (!intersect || pen > sphere.Radius) return;

            pen = sphere.Radius - pen;
            FP friction = 0.5 * (convex.Body.Friction + sphere.Body.Friction);
            FP restitution = 0.5 * (convex.Body.Restitution + sphere.Body.Restitution);

            // 创建碰撞对象并赋值数据
            potentialContact.type = 2;
            potentialContact.Friction = friction;
            potentialContact.Restitution = restitution;
            potentialContact.ContactNormal = normal;
            potentialContact.Penetration = pen;
            potentialContact.ContactPoint = contactPoint;
        }

        public static void ConvexAndCapsule(CollisionConvex convex, CollisionCapsule capsule, RigidContactPotential potentialContact)
        {
            TSVector3 normal = TSVector3.Zero;
            TSVector3 contactPoint = TSVector3.Zero;
            FP pen = 0;
            GJKDetecotor detector = new GJKDetecotor();
            bool intersect = detector.GJKDist(convex.Vertices, new TSVector3[] { capsule.CenterOne, capsule.CenterTwo }, ref normal, ref contactPoint, ref pen);

            if (!intersect || pen > capsule.Radius) return;

            pen = capsule.Radius - pen;
            FP friction = 0.5 * (convex.Body.Friction + capsule.Body.Friction);
            FP restitution = 0.5 * (convex.Body.Restitution + capsule.Body.Restitution);

            // 创建碰撞对象并赋值数据
            potentialContact.type = 2;
            potentialContact.Friction = friction;
            potentialContact.Restitution = restitution;
            potentialContact.ContactNormal = normal;
            potentialContact.Penetration = pen;
            potentialContact.ContactPoint = contactPoint;
        }

        public static void TriangleAndSphere(CollisionTriangle triangle, CollisionSphere sphere, RigidContactPotential potentialContact)
        {
            TSVector3 A = triangle.Vertices[0];
            TSVector3 B = triangle.Vertices[1];
            TSVector3 C = triangle.Vertices[2];
            TSVector3 center = sphere.GetAxis(3);
            FP projection = TSVector3.Dot(center - A, triangle.Normal);
            if (projection <= 0 || projection > sphere.Radius) return; //在三角形法线另一侧则视为不碰撞

            //找到球心与三角形的最近点，然后视为球点碰撞
            int index = 0;
            TSVector3 closetPoint = ClosestPointOnTriangle(center, A, B, C, triangle.Normal, ref index);

            TSVector3 midline = center - closetPoint;
            FP pen = sphere.Radius - midline.Magnitude;
            if (pen < 0) return;

            FP friction = 0.5 * (triangle.Body.Friction + sphere.Body.Friction);
            FP restitution = 0.5 * (triangle.Body.Restitution + sphere.Body.Restitution);

            potentialContact.type = 2;
            potentialContact.Friction = friction;
            potentialContact.Restitution = restitution;
            potentialContact.ContactNormal = -midline.Normalized;
            potentialContact.Penetration = pen;
            potentialContact.ContactPoint = closetPoint;
        }

        public static void TriangleAndCapsule(CollisionTriangle triangle, CollisionCapsule capsule, RigidContactPotential potentialContact)
        {
            //把胶囊视为线段，找到线段与三角形的双方的最近点，然后转为球点碰撞

            TSVector3 A = triangle.Vertices[0];
            TSVector3 B = triangle.Vertices[1];
            TSVector3 C = triangle.Vertices[2];
            TSVector3 center1 = capsule.CenterOne;
            TSVector3 center2 = capsule.CenterTwo;
            FP projection1 = TSVector3.Dot(center1 - A, triangle.Normal);
            FP projection2 = TSVector3.Dot(center2 - A, triangle.Normal);
            if (projection1 > capsule.Radius && projection2 > capsule.Radius) return;

            TSVector3 closetP = TSVector3.Zero;
            TSVector3 closetQ = TSVector3.Zero;
            ClosestPointOnLineAndTriangle(center1, center2, A, B, C, triangle.Normal, ref closetP, ref closetQ);

            TSVector3 midline = closetP - closetQ;
            FP pen = capsule.Radius - midline.Magnitude;
            if (pen < 0) return;

            TSVector3 normal = -midline.Normalized;
            if (TSVector3.Dot(normal, triangle.Normal) > 0)
            {
                normal *= -1;
            }

            FP friction = 0.5 * (triangle.Body.Friction + capsule.Body.Friction);
            FP restitution = 0.5 * (triangle.Body.Restitution + capsule.Body.Restitution);


            potentialContact.type = 2;
            potentialContact.Friction = friction;
            potentialContact.Restitution = restitution;
            potentialContact.ContactNormal = normal;
            potentialContact.Penetration = pen;
            potentialContact.ContactPoint = closetQ;
        }

        public static void TriangleAndBox(CollisionTriangle triangle, CollisionBox box, RigidContactPotential potentialContact)
        {
            TSVector3 A = triangle.Vertices[0];
            TSVector3 B = triangle.Vertices[1];
            TSVector3 C = triangle.Vertices[2];
            TSVector3 center = box.GetAxis(3);

            int best = 0;
            TSVector3 normal = TSVector3.Zero;
            TSVector3 contactPoint = TSVector3.Zero;

            //1、与三角形法线面的分离轴检测
            FP projection1 = box.HalfSize.x * TSVector3.AbsDot(triangle.Normal, box.GetAxis(0)) +
            box.HalfSize.y * TSVector3.AbsDot(triangle.Normal, box.GetAxis(1)) +
            box.HalfSize.z * TSVector3.AbsDot(triangle.Normal, box.GetAxis(2));
            FP projection2 = TSVector3.Dot(center - A, triangle.Normal);
            FP pen1 = projection1 - projection2;
            if (projection2 < 0 || pen1 < 0) return; //在法线反面或者，投影小于中心到三角的法线投影
            FP minPen = pen1;

            //2、三角形点与与盒体6个面的分离检测，转为盒体坐标，然后AABB检测
            //由于三角形点与盒体面的碰撞情况占比很小，只做提前退出，不做碰撞
            TSVector3 relA = box.Transform.TransformInverse(A);
            TSVector3 relB = box.Transform.TransformInverse(B);
            TSVector3 relC = box.Transform.TransformInverse(C);
            TSVector3[] relVertices = new TSVector3[3] { relA, relB, relC };
            if (!BoxIntersectPoint(box, relVertices)) return;

            //3、三角形三条边与盒体三个方向的边的9条叉积轴分离轴检测
            TSVector3 f1 = relB - relA;
            TSVector3 f2 = relC - relB;
            TSVector3 f3 = relA - relC;
            TSVector3[] f = new TSVector3[3] { f1, f2, f3 };
            int boxIndex = -1;
            int triangleIndex = -1;
            TSVector3 normal3 = TSVector3.Zero;
            FP pen3 = 0;
            MinDepthEdges(box, relVertices, f, ref pen3, ref normal3, ref boxIndex, ref triangleIndex);
            if (boxIndex == -1 || triangleIndex == -1) return;
            best = pen3 < minPen ? 2 : best;
            minPen = best == 2 ? pen3 : minPen;

            //与三角形面的碰撞
            if (best == 0)
            {
                normal = -triangle.Normal;
                contactPoint = box.HalfSize;
                if (TSVector3.Dot(box.GetAxis(0), normal) < 0) contactPoint.x = -contactPoint.x;
                if (TSVector3.Dot(box.GetAxis(1), normal) < 0) contactPoint.y = -contactPoint.y;
                if (TSVector3.Dot(box.GetAxis(2), normal) < 0) contactPoint.z = -contactPoint.z;
                contactPoint = box.Transform.Transform(contactPoint);
            }
            if (best == 2)
            {
                //边边碰撞
                normal = box.Transform.TransformDirection(normal3);
                TSVector3 ptOnOneEdge1 = TSVector3.Zero;
                TSVector3 ptOnOneEdge2 = TSVector3.Zero;
                if (triangleIndex == 0)
                {
                    ptOnOneEdge1 = triangle.Vertices[0];
                    ptOnOneEdge2 = triangle.Vertices[1];
                }
                if (triangleIndex == 1)
                {
                    ptOnOneEdge1 = triangle.Vertices[1];
                    ptOnOneEdge2 = triangle.Vertices[2];
                }
                if (triangleIndex == 2)
                {
                    ptOnOneEdge1 = triangle.Vertices[2];
                    ptOnOneEdge2 = triangle.Vertices[0];
                }

                TSVector3 ptOnTwoEdge1 = box.HalfSize;
                TSVector3 ptOnTwoEdge2 = box.HalfSize;
                for (int i = 0; i < 3; i++)
                {
                    if (i == boxIndex)
                    {
                        ptOnTwoEdge2[i] = -ptOnTwoEdge2[i];
                    }
                    else if (TSVector3.Dot(box.GetAxis(i), normal) < 0)
                    {
                        ptOnTwoEdge1[i] = -ptOnTwoEdge1[i];
                        ptOnTwoEdge2[i] = -ptOnTwoEdge2[i];
                    }
                }
                ptOnTwoEdge1 = box.Transform.Transform(ptOnTwoEdge1);
                ptOnTwoEdge2 = box.Transform.Transform(ptOnTwoEdge2);
                contactPoint = ClosestPointOnTwoLines(ptOnOneEdge1, ptOnOneEdge2, ptOnTwoEdge1, ptOnTwoEdge2);
            }

            FP friction = 0.5 * (triangle.Body.Friction + box.Body.Friction);
            FP restitution = 0.5 * (triangle.Body.Restitution + box.Body.Restitution);

            potentialContact.type = 2;
            potentialContact.Friction = friction;
            potentialContact.Restitution = restitution;
            potentialContact.ContactNormal = normal;
            potentialContact.Penetration = minPen;
            potentialContact.ContactPoint = contactPoint;
        }

        public static void TriangleAndConvex(CollisionTriangle triangle, CollisionConvex convex, RigidContactPotential potentialContact)
        {
            TSVector3 A = triangle.Vertices[0];
            TSVector3 center = convex.GetAxis(3);
            FP projection = TSVector3.Dot(center - A, triangle.Normal);
            if (projection <= 0) return;

            //GJK算法
            TSVector3 normal = TSVector3.Zero;
            TSVector3 contactPoint = TSVector3.Zero;
            FP pen = 0;
            GJKDetecotor detector = new GJKDetecotor();
            bool intersect = detector.GJK(triangle.Vertices, convex.Vertices, ref normal, ref contactPoint, ref pen);
            if (!intersect) return;

            FP friction = 0.5 * (triangle.Body.Friction + convex.Body.Friction);
            FP restitution = 0.5 * (triangle.Body.Restitution + convex.Body.Restitution);

            potentialContact.type = 2;
            potentialContact.Friction = friction;
            potentialContact.Restitution = restitution;
            potentialContact.ContactNormal = normal;
            potentialContact.Penetration = pen;
            potentialContact.ContactPoint = contactPoint;
        }

        public static void BoxAndSphere(CollisionBox box, TSVector3 center, FP radius, RigidBody body, RigidContactPotential potentialContact)
        {
            TSVector3 relCentre = box.Transform.TransformInverse(center);

            // 提前退出检测
            if (FP.Abs(relCentre.x) - radius > box.HalfSize.x ||
                FP.Abs(relCentre.y) - radius > box.HalfSize.y ||
                FP.Abs(relCentre.z) - radius > box.HalfSize.z)
            {
                return;
            }

            TSVector3 closestPt = new TSVector3(0, 0, 0);
            FP dist;

            // Clamp closestPt 到 halfsize
            dist = relCentre.x;
            if (dist > box.HalfSize.x) dist = box.HalfSize.x;
            if (dist < -box.HalfSize.x) dist = -box.HalfSize.x;
            closestPt.x = dist;

            dist = relCentre.y;
            if (dist > box.HalfSize.y) dist = box.HalfSize.y;
            if (dist < -box.HalfSize.y) dist = -box.HalfSize.y;
            closestPt.y = dist;

            dist = relCentre.z;
            if (dist > box.HalfSize.z) dist = box.HalfSize.z;
            if (dist < -box.HalfSize.z) dist = -box.HalfSize.z;
            closestPt.z = dist;

            dist = (closestPt - relCentre).SqrMagnitude;
            if (dist > radius * radius) return;

            TSVector3 closestPtWorld = box.Transform.Transform(closestPt);

            FP friction = 0.5 * (box.Body.Friction + body.Friction);
            FP restitution = 0.5 * (box.Body.Restitution + body.Restitution);

            potentialContact.type = 2;
            potentialContact.Friction = friction;
            potentialContact.Restitution = restitution;
            potentialContact.ContactNormal = (closestPtWorld - center).Normalized;
            potentialContact.ContactPoint = closestPtWorld;
            potentialContact.Penetration = radius - FP.Sqrt(dist);
        }

        public static bool RayAndSphere(CollisionRay ray, CollisionSphere sphere, ref FP distance)
        {
            return false;
        }

        public static bool RayAndBox(CollisionRay ray, CollisionBox box, ref FP distance)
        {
            return false;
        }

        public static bool RayAndCapsule(CollisionRay ray, CollisionCapsule capsule, ref FP distance)
        {
            return false;
        }

        public static bool RayAndConvex(CollisionRay ray, CollisionConvex convex, ref FP distance)
        {
            return false;
        }

        public static bool RayAndTriangle(CollisionRay ray, CollisionTriangle triangle, ref FP distance)
        {
            return false;
        }

        /// <summary>
        /// 第二条线段离第一条线段的最近点
        /// </summary>
        /// <param name="lineOnePointA"></param>
        /// <param name="lineOnePointB"></param>
        /// <param name="lineTwoPointA"></param>
        /// <param name="lineTwoPointB"></param>
        /// <returns></returns>
        public static TSVector3 ClosestPointFromLineTwo(TSVector3 lineOnePointA, TSVector3 lineOnePointB, TSVector3 lineTwoPointA, TSVector3 lineTwoPointB)
        {
            TSVector3 bestA = TSVector3.Zero;
            TSVector3 bestB = TSVector3.Zero;
            ClosestPoinsOnTwoLines(lineOnePointA, lineOnePointB, lineTwoPointA, lineTwoPointB, ref bestA, ref bestB);
            return bestB;
        }

        /// <summary>
        /// 两条线段的两个最近点
        /// </summary>
        /// <param name="lineOnePointA"></param>
        /// <param name="lineOnePointB"></param>
        /// <param name="lineTwoPointA"></param>
        /// <param name="lineTwoPointB"></param>
        /// <param name="bestA"></param>
        /// <param name="bestB"></param>
        public static void ClosestPoinsOnTwoLines(TSVector3 lineOnePointA, TSVector3 lineOnePointB, TSVector3 lineTwoPointA, TSVector3 lineTwoPointB, ref TSVector3 bestA, ref TSVector3 bestB)
        {
            FP episolon = 0.0001;
            TSVector3 d1 = lineOnePointB - lineOnePointA;
            TSVector3 d2 = lineTwoPointB - lineTwoPointA;
            TSVector3 r = lineOnePointA - lineTwoPointA;
            FP a = TSVector3.Dot(d1, d1);
            FP e = TSVector3.Dot(d2, d2);
            FP f = TSVector3.Dot(d2, r);

            FP s;
            FP t;
            if (a <= episolon && e <= episolon)
            {
                bestA = lineOnePointA;
                bestB = lineTwoPointA;
                return;
            }

            if (a <= episolon)
            {
                s = 0;
                t = f / e;
                t = FP.Clamp(t, 0, 1);
            }
            else
            {
                FP c = TSVector3.Dot(d1, r);
                if (e <= episolon)
                {
                    t = 0;
                    s = FP.Clamp(-c / a, 0, 1);
                }
                else
                {
                    FP b = TSVector3.Dot(d1, d2);
                    FP denom = a * e - b * b;
                    s = denom == 0 ? 0 : FP.Clamp((b * f - c * e) / denom, 0, 1);

                    FP tnom = b * s + f;
                    if (tnom < 0)
                    {
                        t = 0;
                        s = FP.Clamp(-c / a, 0, 1);
                    }
                    else if (tnom > e)
                    {
                        t = 1;
                        s = FP.Clamp((b - c) / a, 0, 1);
                    }
                    else
                    {
                        t = tnom / e;
                    }
                }
            }

            bestA = lineOnePointA + d1 * s;
            bestB = lineTwoPointA + d2 * t;
        }

        /// <summary>
        /// 两条线段最近距离点
        /// </summary>
        /// <param name="lineOnePointA"></param>
        /// <param name="lineOnePointB"></param>
        /// <param name="lineTwoPointA"></param>
        /// <param name="lineTwoPointB"></param>
        /// <returns></returns>
        public static TSVector3 ClosestPointOnTwoLines(TSVector3 lineOnePointA, TSVector3 lineOnePointB, TSVector3 lineTwoPointA, TSVector3 lineTwoPointB)
        {
            TSVector3 bestA = TSVector3.Zero;
            TSVector3 bestB = TSVector3.Zero;
            ClosestPoinsOnTwoLines(lineOnePointA, lineOnePointB, lineTwoPointA, lineTwoPointB, ref bestA, ref bestB);
            return bestA + (bestB - bestA) * 0.5;
        }

        /// <summary>
        /// 线段到点的最近点
        /// </summary>
        /// <param name="linePointA"></param>
        /// <param name="linePointB"></param>
        /// <param name="Point"></param>
        /// <returns></returns>
        public static TSVector3 ClosestPointOnLineSegment(TSVector3 linePointA, TSVector3 linePointB, TSVector3 Point)
        {
            TSVector3 AB = linePointB - linePointA;
            FP t = TSVector3.Dot(Point - linePointA, AB) / TSVector3.Dot(AB, AB);
            return linePointA + FP.Clamp(t, 0, 1) * AB;
        }

        /// <summary>
        /// 三角形到一点的最近点
        /// </summary>
        /// <param name="P"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="normal"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static TSVector3 ClosestPointOnTriangle(TSVector3 P, TSVector3 A, TSVector3 B, TSVector3 C, TSVector3 normal, ref int index)
        {
            TSVector3 foot = FootPointOnPlane(A, B, C, P);

            TSVector3 AF = foot - A;
            TSVector3 BF = foot - B;
            FP signFAB = FP.Sign(TSVector3.Dot(normal, TSVector3.Cross(B - A, AF)));
            FP signFAC = FP.Sign(TSVector3.Dot(normal, TSVector3.Cross(AF, C - A)));
            FP signFBC = FP.Sign(TSVector3.Dot(normal, TSVector3.Cross(C - B, BF)));

            TSVector3 closetPoint;
            if (signFAB <= 0 && signFAC <= 0) //最近点为三角形顶点
            {
                closetPoint = A;
                index = 0;
            }
            else if (signFAB <= 0 && signFBC <= 0)
            {
                closetPoint = B;
                index = 1;
            }
            else if (signFAC <= 0 && signFBC <= 0)
            {
                closetPoint = C;
                index = 2;
            }
            else if (signFAB <= 0) // 最近点在三条边上
            {
                closetPoint = ClosestPointOnLineSegment(A, B, P);
                index = 3;
            }
            else if (signFAC <= 0)
            {
                closetPoint = ClosestPointOnLineSegment(A, C, P);
                index = 4;
            }
            else if (signFBC <= 0)
            {
                closetPoint = ClosestPointOnLineSegment(B, C, P);
                index = 5;
            }
            else // 最近点在面上，为垂足
            {
                closetPoint = foot;
                index = 6;
            }

            return closetPoint;
        }

        /// <summary>
        /// 线段和三角形双方的最近距离点
        /// </summary>
        /// <param name="P"></param>
        /// <param name="Q"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="normal"></param>
        /// <param name="closetA"></param>
        /// <param name="closetB"></param>
        public static void ClosestPointOnLineAndTriangle(TSVector3 P, TSVector3 Q, TSVector3 A, TSVector3 B, TSVector3 C, TSVector3 normal, ref TSVector3 closetA, ref TSVector3 closetB)
        {
            //两个端点到三角形的最近点和线段到三条边的边边距离比较，取最近的

            //1、线段两端分别到三角形最近点
            int featureIndex1 = 0;
            int featureIndex2 = 0;

            TSVector3 bestA = ClosestPointOnTriangle(P, A, B, C, normal, ref featureIndex1);
            FP sqrDist1 = (P - bestA).SqrMagnitude;
            TSVector3 bestB = ClosestPointOnTriangle(Q, A, B, C, normal, ref featureIndex2);
            FP sqrDist2 = (Q - bestB).SqrMagnitude;
            bool lessthan = sqrDist1 < sqrDist2;
            int minIndex = lessthan ? 1 : 2;
            FP minSqrdist = lessthan ? sqrDist1 : sqrDist2;
            if (featureIndex1 == 6 && featureIndex2 == 6) //两最近点为垂足，则两点取最近即可
            {
                if (minIndex == 1)
                {
                    closetA = P;
                    closetB = bestA;
                    return;
                }
                if (minIndex == 2)
                {
                    closetA = Q;
                    closetB = bestB;
                    return;
                }
            }

            //2、线段分别和三角形三边比较
            TSVector3 bestAB1 = TSVector3.Zero;
            TSVector3 bestAB2 = TSVector3.Zero; ;
            ClosestPoinsOnTwoLines(P, Q, A, B, ref bestAB1, ref bestAB2);
            FP sqrDist3 = (bestAB1 - bestAB2).SqrMagnitude;
            lessthan = sqrDist3 < minSqrdist;
            minIndex = lessthan ? 3 : minIndex;
            minSqrdist = lessthan ? sqrDist3 : minSqrdist;

            TSVector3 bestBC1 = TSVector3.Zero;
            TSVector3 bestBC2 = TSVector3.Zero; ;
            ClosestPoinsOnTwoLines(P, Q, B, C, ref bestBC1, ref bestBC2);
            FP sqrDist4 = (bestBC1 - bestBC2).SqrMagnitude;
            lessthan = sqrDist4 < minSqrdist;
            minIndex = lessthan ? 4 : minIndex;
            minSqrdist = lessthan ? sqrDist4 : minSqrdist;

            TSVector3 bestAC1 = TSVector3.Zero;
            TSVector3 bestAC2 = TSVector3.Zero; ;
            ClosestPoinsOnTwoLines(P, Q, A, C, ref bestAC1, ref bestAC2);
            FP sqrDist5 = (bestAC1 - bestAC2).SqrMagnitude;
            lessthan = sqrDist5 < minSqrdist;
            minIndex = lessthan ? 5 : minIndex;

            //取最近
            if (minIndex == 1)
            {
                closetA = P;
                closetB = bestA;
                return;
            }
            if (minIndex == 2)
            {
                closetA = Q;
                closetB = bestB;
                return;
            }
            if (minIndex == 3)
            {
                closetA = bestAB1;
                closetB = bestAB2;
                return;
            }
            if (minIndex == 4)
            {
                closetA = bestBC1;
                closetB = bestBC2;
                return;
            }
            if (minIndex == 5)
            {
                closetA = bestAC1;
                closetB = bestAC2;
                return;
            }
        }

        /// <summary>
        /// 点到三角形的垂足
        /// </summary>
        /// <param name="planePointA"></param>
        /// <param name="planePointB"></param>
        /// <param name="planePointC"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static TSVector3 FootPointOnPlane(TSVector3 planePointA, TSVector3 planePointB, TSVector3 planePointC, TSVector3 point)
        {
            TSVector3 normal = TSVector3.Cross(planePointB - planePointA, planePointC - planePointA);// not normalized
            if (normal == TSVector3.Zero) return TSVector3.Zero;

            TSVector3 PA = planePointA - point;
            FP t = TSVector3.Dot(PA, normal) / TSVector3.Dot(normal, normal);
            return point + t * normal;
        }

        /// <summary>
        /// 盒体和盒体的分离轴测试
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <param name="axis"></param>
        /// <param name="toCentre"></param>
        /// <param name="index"></param>
        /// <param name="smallestPenetration"></param>
        /// <param name="smallestCase"></param>
        /// <returns></returns>
        private static bool TryAxis(CollisionBox one, CollisionBox two, TSVector3 axis, TSVector3 toCentre, int index, ref FP smallestPenetration, ref int smallestCase)
        {
            // 两条边叉积接近0，为平行轴，不检测
            if (axis.SqrMagnitude < 0.0001)
                return true;
            axis.Normalize();

            FP penetration = PenetrationOnAxis(one, two, axis, toCentre);

            if (penetration < 0) return false;
            if (penetration < smallestPenetration)
            {
                smallestPenetration = penetration;
                smallestCase = index;
            }
            return true;
        }

        /// <summary>
        /// 胶囊和盒体的分离轴测试
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <param name="axis"></param>
        /// <param name="toCentre"></param>
        /// <param name="index"></param>
        /// <param name="smallestPenetration"></param>
        /// <param name="smallestCase"></param>
        /// <returns></returns>
        private static bool TryAxis(CollisionCapsule one, CollisionBox two, TSVector3 axis, TSVector3 toCentre, int index, ref FP smallestPenetration, ref int smallestCase)
        {
            // 两条边叉积接近0，为平行轴，不检测
            if (axis.SqrMagnitude < 0.0001f)
                return true;
            axis.Normalize();

            FP penetration = PenetrationOnAxis
                (one, two, axis, toCentre);

            if (penetration < 0) return false;
            if (penetration < smallestPenetration)
            {
                smallestPenetration = penetration;
                smallestCase = index;
            }
            return true;
        }

        /// <summary>
        /// 盒体和盒体在一轴上的相交部分的投影
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <param name="axis"></param>
        /// <param name="toCentre"></param>
        /// <returns></returns>
        private static FP PenetrationOnAxis(CollisionBox one, CollisionBox two, TSVector3 axis, TSVector3 toCentre)
        {
            FP oneProject = TransformToAxis(one, axis);
            FP twoProject = TransformToAxis(two, axis);

            FP distance = TSVector3.AbsDot(toCentre, axis); // 中心投影

            return oneProject + twoProject - distance;
        }

        /// <summary>
        /// 胶囊和盒体在一轴上的相交部分的投影
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <param name="axis"></param>
        /// <param name="toCentre"></param>
        /// <returns></returns>
        private static FP PenetrationOnAxis(CollisionCapsule one, CollisionBox two, TSVector3 axis, TSVector3 toCentre)
        {
            FP oneProject = TransformToAxis(one, axis);
            FP twoProject = TransformToAxis(two, axis);

            FP distance = TSVector3.AbsDot(toCentre, axis); // 中心投影

            return oneProject + twoProject - distance;
        }

        /// <summary>
        /// 盒体在一轴上的半投影
        /// </summary>
        /// <param name="box"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        private static FP TransformToAxis(CollisionBox box, TSVector3 axis)
        {
            return
            box.HalfSize.x * TSVector3.AbsDot(axis, box.GetAxis(0)) +
            box.HalfSize.y * TSVector3.AbsDot(axis, box.GetAxis(1)) +
            box.HalfSize.z * TSVector3.AbsDot(axis, box.GetAxis(2));
        }

        /// <summary>
        /// 胶囊在一轴上的半投影
        /// </summary>
        /// <param name="capsule"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        private static FP TransformToAxis(CollisionCapsule capsule, TSVector3 axis)
        {
            return
            capsule.Radius * TSVector3.AbsDot(axis, capsule.GetAxis(0)) +
            (capsule.HalfHeight.y + capsule.Radius) * TSVector3.AbsDot(axis, capsule.GetAxis(1)) +
            capsule.Radius * TSVector3.AbsDot(axis, capsule.GetAxis(2));
        }

        /// <summary>
        /// 盒体和盒体的点面碰撞
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <param name="toCentre"></param>
        /// <param name="data"></param>
        /// <param name="best"></param>
        /// <param name="pen"></param>
        private static void FillPointFaceBoxBox(CollisionBox one, CollisionBox two, TSVector3 toCentre, RigidContactPotential potentialContact, int best, FP pen)
        {
            TSVector3 normal = one.GetAxis(best);
            if (TSVector3.Dot(one.GetAxis(best), toCentre) > 0)
            {
                normal *= -1.0;
            }

            // 找到碰撞顶点
            TSVector3 vertex = two.HalfSize;
            if (TSVector3.Dot(two.GetAxis(0), normal) < 0) vertex.x = -vertex.x;
            if (TSVector3.Dot(two.GetAxis(1), normal) < 0) vertex.y = -vertex.y;
            if (TSVector3.Dot(two.GetAxis(2), normal) < 0) vertex.z = -vertex.z;

            FP friction = 0.5 * (one.Body.Friction + two.Body.Friction);
            FP restitution = 0.5 * (one.Body.Restitution + two.Body.Restitution);

            potentialContact.type = 2;
            potentialContact.Friction = friction;
            potentialContact.Restitution = restitution;
            potentialContact.ContactNormal = normal;
            potentialContact.Penetration = pen;
            potentialContact.ContactPoint = two.Transform * vertex;
        }

        /// <summary>
        /// 盒体和一组顶点是否相交 AABB
        /// </summary>
        /// <param name="box"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        public static bool BoxIntersectPoint(CollisionBox box, TSVector3[] points)
        {
            FP minX = FP.MaxValue;
            FP maxX = FP.MinValue;
            FP minY = FP.MaxValue;
            FP maxY = FP.MinValue;
            FP minZ = FP.MaxValue;
            FP maxZ = FP.MinValue;
            for (int i = 0; i < points.Length; ++i)
            {
                TSVector3 vertice = points[i];
                if (vertice.x > maxX) maxX = vertice.x;
                if (vertice.x < minX) minX = vertice.x;
                if (vertice.y > maxY) maxY = vertice.y;
                if (vertice.y < minY) minY = vertice.y;
                if (vertice.z > maxZ) maxZ = vertice.z;
                if (vertice.z < minZ) minZ = vertice.z;
            }

            if (minX > box.HalfSize.x || maxX < -box.HalfSize.x) return false;
            if (minY > box.HalfSize.y || maxY < -box.HalfSize.y) return false;
            if (minZ > box.HalfSize.z || maxZ < -box.HalfSize.z) return false;

            return true;
        }

        private static void MinDepthPoint(CollisionBox box, TSVector3[] points, ref int minIndex, ref FP minDepth, ref TSVector3 normal)
        {
            minIndex = 0;
            minDepth = FP.MaxValue;
            for (int i = 0; i < points.Length; i++)
            {
                FP min_depth = minDepth;
                TSVector3 min_normal = TSVector3.Zero;
                TSVector3 point = points[i];
                FP depth = box.HalfSize.x - FP.Abs(point.x);
                if (depth < 0) continue;
                else if (depth < min_depth)
                {
                    min_depth = depth;
                    min_normal = box.GetAxis(0) * ((point.x < 0) ? -1 : 1);
                }

                depth = box.HalfSize.y - FP.Abs(point.y);
                if (depth < 0) continue;
                else if (depth < min_depth)
                {
                    min_depth = depth;
                    min_normal = box.GetAxis(1) * ((point.y < 0) ? -1 : 1);
                }

                depth = box.HalfSize.z - FP.Abs(point.z);
                if (depth < 0) continue;
                else if (depth < min_depth)
                {
                    min_depth = depth;
                    min_normal = box.GetAxis(2) * ((point.z < 0) ? -1 : 1);
                }

                if (min_depth < minDepth)
                {
                    minDepth = min_depth;
                    normal = min_normal;
                    minIndex = i;
                }
            }
        }

        /// <summary>
        /// 在盒体坐标系内，9叉积方向，找到盒体和三角形的两条最浅相交边
        /// </summary>
        /// <param name="box"></param>
        /// <param name="points"></param>
        /// <param name="f"></param>
        /// <param name="minDepth"></param>
        /// <param name="normal"></param>
        /// <param name="boxIndex"></param>
        /// <param name="triangleIndex"></param>
        public static void MinDepthEdges(CollisionBox box, TSVector3[] points, TSVector3[] f, ref FP minDepth, ref TSVector3 normal, ref int boxIndex, ref int triangleIndex)
        {
            minDepth = FP.MaxValue;
            TSVector3 u0 = TSVector3.right;
            TSVector3 u1 = TSVector3.up;
            TSVector3 u2 = TSVector3.forward;
            TSVector3 center = (points[0] + points[1] + points[2]) / 3;
            for (int i = 0; i < 9; i++)
            {
                FP min_depth = minDepth;
                int index1 = i / 3;
                int index2 = i % 3;
                TSVector3 boxAxis = TSVector3.Zero;
                boxAxis[index1] = 1;
                TSVector3 axis = TSVector3.Cross(f[index2], boxAxis);
                if (axis.SqrMagnitude < 0.0001f) continue; //两边平行
                axis.Normalize();

                //三个顶点在轴上的投影
                FP p0 = TSVector3.Dot(axis, points[0]);
                FP p1 = TSVector3.Dot(axis, points[1]);
                FP p2 = TSVector3.Dot(axis, points[2]);

                //盒体在轴上的半投影
                FP r = box.HalfSize.x * TSVector3.AbsDot(axis, u0) +
                         box.HalfSize.y * TSVector3.AbsDot(axis, u1) +
                         box.HalfSize.z * TSVector3.AbsDot(axis, u2);

                //用投影做分离检测
                FP min = FP.Min(FP.Min(p0, p1), p2);
                FP max = FP.Max(FP.Max(p0, p1), p2);
                if (min > r || max < -r)
                {
                    boxIndex = -1;
                    triangleIndex = -1;
                    return;
                }
                else
                {
                    FP depth = FP.Min(r - min, max + r);
                    if (depth < min_depth)
                    {
                        boxIndex = index1;
                        triangleIndex = index2;
                        minDepth = depth;
                        normal = axis.Normalized;
                        if (TSVector3.Dot(normal, center) < 0)
                        {
                            normal *= -1;
                        }
                    }
                }
            }
        }
    }
}
