using TrueSync;

namespace GGPhys.Rigid.Collisions
{
    /// <summary>
    /// 相交测试类，参照CollisionDetector
    /// </summary>
    public static class IntersectionTests
    {
        public static bool SphereAndSphere(CollisionSphere one, CollisionSphere two, RigidContactPotential potentialContact)
        {
            TSVector3 positionOne = one.GetAxis(3);
            TSVector3 positionTwo = two.GetAxis(3);

            TSVector3 midline = positionOne - positionTwo;
            FP size = midline.Magnitude;

            if (size <= 0.0f || size >= one.Radius + two.Radius)
            {
                return false;
            }

            potentialContact.type = 1;
            return true;
        }

        public static bool BoxAndBox(CollisionBox one, CollisionBox two, RigidContactPotential potentialContact)
        {
            TSVector3 toCentre = two.GetAxis(3) - one.GetAxis(3);

            if (!TryAxis(one, two, one.GetAxis(0), toCentre)
                || !TryAxis(one, two, one.GetAxis(1), toCentre)
                || !TryAxis(one, two, one.GetAxis(2), toCentre)
                || !TryAxis(one, two, two.GetAxis(0), toCentre)
                || !TryAxis(one, two, two.GetAxis(1), toCentre)
                || !TryAxis(one, two, two.GetAxis(2), toCentre)
                || !TryAxis(one, two, TSVector3.Cross(one.GetAxis(0), two.GetAxis(0)), toCentre)
                || !TryAxis(one, two, TSVector3.Cross(one.GetAxis(0), two.GetAxis(1)), toCentre)
                || !TryAxis(one, two, TSVector3.Cross(one.GetAxis(0), two.GetAxis(2)), toCentre)
                || !TryAxis(one, two, TSVector3.Cross(one.GetAxis(1), two.GetAxis(0)), toCentre)
                || !TryAxis(one, two, TSVector3.Cross(one.GetAxis(1), two.GetAxis(1)), toCentre)
                || !TryAxis(one, two, TSVector3.Cross(one.GetAxis(1), two.GetAxis(2)), toCentre)
                || !TryAxis(one, two, TSVector3.Cross(one.GetAxis(2), two.GetAxis(0)), toCentre)
                || !TryAxis(one, two, TSVector3.Cross(one.GetAxis(2), two.GetAxis(1)), toCentre)
                || !TryAxis(one, two, TSVector3.Cross(one.GetAxis(2), two.GetAxis(2)), toCentre))
            {
                return false;
            }

            potentialContact.type = 1;
            return true;
        }


        public static bool BoxAndSphere(CollisionBox box, CollisionSphere sphere, RigidContactPotential potentialContact)
        {
            TSVector3 centre = sphere.GetAxis(3);
            TSVector3 relCentre = box.Transform.TransformInverse(centre);

            if (FP.Abs(relCentre.x) - sphere.Radius > box.HalfSize.x ||
                FP.Abs(relCentre.y) - sphere.Radius > box.HalfSize.y ||
                FP.Abs(relCentre.z) - sphere.Radius > box.HalfSize.z)
            {
                return false;
            }

            TSVector3 closestPt = new TSVector3(0, 0, 0);
            FP dist;

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
            if (dist > sphere.Radius * sphere.Radius)
            {
                return false;
            }

            potentialContact.type = 1;
            return true;
        }


        public static bool CapsuleAndCapsule(CollisionCapsule capsule1, CollisionCapsule capsule2, RigidContactPotential potentialContact)
        {
            TSVector3 v0 = capsule2.CenterOne - capsule1.CenterOne;
            TSVector3 v1 = capsule2.CenterTwo - capsule1.CenterOne;
            TSVector3 v2 = capsule2.CenterOne - capsule1.CenterTwo;
            TSVector3 v3 = capsule2.CenterTwo - capsule1.CenterTwo;

            FP d0 = TSVector3.Dot(v0, v0);
            FP d1 = TSVector3.Dot(v1, v1);
            FP d2 = TSVector3.Dot(v2, v2);
            FP d3 = TSVector3.Dot(v3, v3);

            TSVector3 bestA = TSVector3.Zero; ;
            if (d2 < d0 || d2 < d1 || d3 < d0 || d3 < d1)
            {
                bestA = capsule1.CenterTwo;
            }
            else
            {
                bestA = capsule1.CenterOne;
            }

            TSVector3 bestB = ClosestPointOnLineSegment(capsule2.CenterOne, capsule2.CenterTwo, bestA);

            bestA = ClosestPointOnLineSegment(capsule1.CenterOne, capsule1.CenterTwo, bestB);

            TSVector3 midline = bestA - bestB;
            FP size = midline.Magnitude;

            if (size <= 0.0f || size >= capsule1.Radius + capsule2.Radius)
            {
                return false;
            }

            potentialContact.type = 1;
            return true;
        }

        public static bool CapsuleAndSphere(CollisionCapsule capsule, CollisionSphere sphere, RigidContactPotential potentialContact)
        {
            TSVector3 bestB = sphere.GetAxis(3);

            TSVector3 bestA = ClosestPointOnLineSegment(capsule.CenterOne, capsule.CenterTwo, bestB);

            TSVector3 midline = bestA - bestB;
            FP size = midline.Magnitude;

            if (size <= 0.0f || size >= capsule.Radius + sphere.Radius)
            {
                return false;
            }

            potentialContact.type = 1;
            return true;
        }

        public static bool CapsuleAndBox(CollisionCapsule capsule, CollisionBox box, RigidContactPotential potentialContact)
        {
            TSVector3 toCentre = box.GetAxis(3) - capsule.GetAxis(3);

            TSVector3 edgeAxis3 = (box.GetAxis(0) + box.GetAxis(1)).Normalized;
            TSVector3 edgeAxis4 = (box.GetAxis(0) + box.GetAxis(2)).Normalized;
            TSVector3 edgeAxis5 = (box.GetAxis(1) + box.GetAxis(2)).Normalized;

            if (!TryAxis(capsule, box, edgeAxis3, toCentre)
                || !TryAxis(capsule, box, edgeAxis4, toCentre)
                || !TryAxis(capsule, box, edgeAxis5, toCentre)
                || !TryAxis(capsule, box, box.GetAxis(0), toCentre)
                || !TryAxis(capsule, box, box.GetAxis(1), toCentre)
                || !TryAxis(capsule, box, box.GetAxis(2), toCentre))
            {
                return false;
            }

            potentialContact.type = 1;
            return true;
        }

        public static bool ConvexAndConvex(CollisionConvex convex1, CollisionConvex convex2, RigidContactPotential potentialContact)
        {
            GJKDetecotor detector = new GJKDetecotor();
            bool intersect = detector.GJKTest(convex1.Vertices, convex2.Vertices);
            potentialContact.type = intersect ? 1 : 0;
            return intersect;
        }

        public static bool ConvexAndBox(CollisionConvex convex, CollisionBox box, RigidContactPotential potentialContact)
        {
            GJKDetecotor detector = new GJKDetecotor();
            bool intersect = detector.GJKTest(convex.Vertices, box.Vertices);
            potentialContact.type = intersect ? 1 : 0;
            return intersect;
        }

        public static bool ConvexAndSphere(CollisionConvex convex, CollisionSphere sphere, RigidContactPotential potentialContact)
        {
            TSVector3 normal = TSVector3.Zero;
            TSVector3 contactPoint = TSVector3.Zero;
            FP pen = 0;
            GJKDetecotor detector = new GJKDetecotor();
            bool intersect = detector.GJKDist(convex.Vertices, new TSVector3[] { sphere.GetAxis(3) }, ref normal, ref contactPoint, ref pen);
            if (!intersect || pen > sphere.Radius)
            {
                return false;
            }
            else
            {
                potentialContact.type = 1;
                return true;
            }
        }

        public static bool ConvexAndCapsule(CollisionConvex convex, CollisionCapsule capsule, RigidContactPotential potentialContact)
        {
            TSVector3 normal = TSVector3.Zero;
            TSVector3 contactPoint = TSVector3.Zero;
            FP pen = 0;
            GJKDetecotor detector = new GJKDetecotor();
            bool intersect = detector.GJKDist(convex.Vertices, new TSVector3[] { capsule.CenterOne, capsule.CenterTwo }, ref normal, ref contactPoint, ref pen);
            if (!intersect || pen > capsule.Radius)
            {
                return false;
            }
            else
            {
                potentialContact.type = 1;
                return true;
            }
        }

        public static bool TriangleAndSphere(CollisionTriangle triangle, CollisionSphere sphere, RigidContactPotential potentialContact)
        {
            TSVector3 A = triangle.Vertices[0];
            TSVector3 B = triangle.Vertices[1];
            TSVector3 C = triangle.Vertices[2];
            TSVector3 center = sphere.GetAxis(3);
            FP projection = TSVector3.Dot(center - A, triangle.Normal);
            if (projection <= 0 || projection > sphere.Radius)
            {
                return false;
            }

            int index = 0;
            TSVector3 closetP = CollisionDetector.ClosestPointOnTriangle(center, A, B, C, triangle.Normal, ref index);
            FP dist = (closetP - center).Magnitude;
            if (dist > sphere.Radius)
            {
                return false;
            }

            potentialContact.type = 1;
            return true;
        }

        public static bool TriangleAndCapsule(CollisionTriangle triangle, CollisionCapsule capsule, RigidContactPotential potentialContact)
        {
            TSVector3 A = triangle.Vertices[0];
            TSVector3 B = triangle.Vertices[1];
            TSVector3 C = triangle.Vertices[2];
            TSVector3 center1 = capsule.CenterOne;
            TSVector3 center2 = capsule.CenterTwo;
            FP projection1 = TSVector3.Dot(center1 - A, triangle.Normal);
            FP projection2 = TSVector3.Dot(center2 - A, triangle.Normal);
            if ((projection1 <= 0 && projection2 <= 0) || (projection1 > capsule.Radius && projection2 > capsule.Radius))
            {
                return false;
            }

            TSVector3 closetP = TSVector3.Zero;
            TSVector3 closetQ = TSVector3.Zero;
            CollisionDetector.ClosestPointOnLineAndTriangle(center1, center2, A, B, C, triangle.Normal, ref closetP, ref closetQ);
            TSVector3 midline = closetP - closetQ;
            FP dist = midline.Magnitude;
            if (dist > capsule.Radius)
            {
                return false;
            }

            potentialContact.type = 1;
            return true;
        }

        public static bool TriangleAndBox(CollisionTriangle triangle, CollisionBox box, RigidContactPotential potentialContact)
        {
            TSVector3 A = triangle.Vertices[0];
            TSVector3 B = triangle.Vertices[1];
            TSVector3 C = triangle.Vertices[2];
            TSVector3 center = box.GetAxis(3);

            FP projection1 = box.HalfSize.x * TSVector3.AbsDot(triangle.Normal, box.GetAxis(0)) +
            box.HalfSize.y * TSVector3.AbsDot(triangle.Normal, box.GetAxis(1)) +
            box.HalfSize.z * TSVector3.AbsDot(triangle.Normal, box.GetAxis(2));
            FP projection2 = TSVector3.Dot(center - A, triangle.Normal);
            FP pen1 = projection1 - projection2;
            if (projection2 < 0 || pen1 < 0)
            {
                return false;
            }

            TSVector3 relA = box.Transform.TransformInverse(A);
            TSVector3 relB = box.Transform.TransformInverse(B);
            TSVector3 relC = box.Transform.TransformInverse(C);
            TSVector3[] relVertices = new TSVector3[3] { relA, relB, relC };
            if (!CollisionDetector.BoxIntersectPoint(box, relVertices))
            {
                return false;
            }


            TSVector3 f1 = relB - relA;
            TSVector3 f2 = relC - relB;
            TSVector3 f3 = relA - relC;
            TSVector3[] f = new TSVector3[3] { f1, f2, f3 };
            int boxIndex = -1;
            int triangleIndex = -1;
            TSVector3 normal3 = TSVector3.Zero;
            FP pen3 = 0;
            CollisionDetector.MinDepthEdges(box, relVertices, f, ref pen3, ref normal3, ref boxIndex, ref triangleIndex);
            if (boxIndex == -1 || triangleIndex == -1)
            {
                return false;
            }

            potentialContact.type = 1;
            return true;
        }

        public static bool TriangleAndConvex(CollisionTriangle triangle, CollisionConvex convex, RigidContactPotential potentialContact)
        {
            TSVector3 A = triangle.Vertices[0];
            TSVector3 center = convex.GetAxis(3);
            FP projection = TSVector3.Dot(center - A, triangle.Normal);
            if (projection <= 0)
            {
                return false;
            }
            GJKDetecotor detector = new GJKDetecotor();
            if (detector.GJKTest(triangle.Vertices, convex.Vertices))
            {
                potentialContact.type = 1;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static TSVector3 ClosestPointOnLineSegment(TSVector3 linePointA, TSVector3 linePointB, TSVector3 Point)
        {
            TSVector3 AB = linePointB - linePointA;
            FP t = TSVector3.Dot(Point - linePointA, AB) / TSVector3.Dot(AB, AB); //
            return linePointA + Saturate(t) * AB;
        }

        public static FP Saturate(FP t)
        {
            return FP.Min(FP.Max(t, 0), 1);
        }

        private static bool TryAxis(CollisionBox one, CollisionBox two, TSVector3 axis, TSVector3 toCentre)
        {
            if (axis.SqrMagnitude < 0.0001) return true;
            axis.Normalize();

            FP penetration = PenetrationOnAxis(one, two, axis, toCentre);

            if (penetration < 0) return false;

            return true;
        }

        private static bool TryAxis(CollisionCapsule one, CollisionBox two, TSVector3 axis, TSVector3 toCentre)
        {
            if (axis.SqrMagnitude < 0.0001) return true;
            axis.Normalize();

            FP penetration = PenetrationOnAxis
                (one, two, axis, toCentre);

            if (penetration < 0) return false;

            return true;
        }

        private static FP PenetrationOnAxis(CollisionBox one, CollisionBox two, TSVector3 axis, TSVector3 toCentre)
        {
            FP oneProject = TransformToAxis(one, axis);
            FP twoProject = TransformToAxis(two, axis);

            FP distance = TSVector3.AbsDot(toCentre, axis);

            return oneProject + twoProject - distance;
        }

        private static FP PenetrationOnAxis(CollisionCapsule one, CollisionBox two, TSVector3 axis, TSVector3 toCentre)
        {
            FP oneProject = TransformToAxis(one, axis);
            FP twoProject = TransformToAxis(two, axis);

            FP distance = TSVector3.AbsDot(toCentre, axis);

            return oneProject + twoProject - distance;
        }

        private static FP TransformToAxis(CollisionBox box, TSVector3 axis)
        {
            return
            box.HalfSize.x * TSVector3.AbsDot(axis, box.GetAxis(0)) +
            box.HalfSize.y * TSVector3.AbsDot(axis, box.GetAxis(1)) +
            box.HalfSize.z * TSVector3.AbsDot(axis, box.GetAxis(2));
        }

        private static FP TransformToAxis(CollisionCapsule capsule, TSVector3 axis)
        {
            return
            capsule.Radius * TSVector3.AbsDot(axis, capsule.GetAxis(0)) +
            (capsule.HalfHeight.y + capsule.Radius) * TSVector3.AbsDot(axis, capsule.GetAxis(1)) +
            capsule.Radius * TSVector3.AbsDot(axis, capsule.GetAxis(2));
        }
    }
}
