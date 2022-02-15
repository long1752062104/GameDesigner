using System;
using System.Collections.Generic;
using GGPhys.Core;
using REAL = FixMath.FP;

namespace GGPhys.Rigid.Collisions
{
    /// <summary>
    /// 相交测试类，参照CollisionDetector
    /// </summary>
    public static class IntersectionTests
    {
        public static bool SphereAndSphere(CollisionSphere one, CollisionSphere two, RigidContactPotential potentialContact)
        {
            Vector3d positionOne = one.GetAxis(3);
            Vector3d positionTwo = two.GetAxis(3);

            Vector3d midline = positionOne - positionTwo;
            REAL size = midline.Magnitude;

            if (size <= 0.0f || size >= one.Radius + two.Radius)
            {
                return false;
            }

            potentialContact.type = 1;
            return true;
        }

        public static bool BoxAndBox(CollisionBox one, CollisionBox two, RigidContactPotential potentialContact)
        {
            Vector3d toCentre = two.GetAxis(3) - one.GetAxis(3);

            if (!TryAxis(one, two, one.GetAxis(0), toCentre) 
                || !TryAxis(one, two, one.GetAxis(1), toCentre)
                || !TryAxis(one, two, one.GetAxis(2), toCentre)
                || !TryAxis(one, two, two.GetAxis(0), toCentre)
                || !TryAxis(one, two, two.GetAxis(1), toCentre)
                || !TryAxis(one, two, two.GetAxis(2), toCentre)
                || !TryAxis(one, two, Vector3d.Cross(one.GetAxis(0), two.GetAxis(0)), toCentre)
                || !TryAxis(one, two, Vector3d.Cross(one.GetAxis(0), two.GetAxis(1)), toCentre)
                || !TryAxis(one, two, Vector3d.Cross(one.GetAxis(0), two.GetAxis(2)), toCentre)
                || !TryAxis(one, two, Vector3d.Cross(one.GetAxis(1), two.GetAxis(0)), toCentre)
                || !TryAxis(one, two, Vector3d.Cross(one.GetAxis(1), two.GetAxis(1)), toCentre)
                || !TryAxis(one, two, Vector3d.Cross(one.GetAxis(1), two.GetAxis(2)), toCentre)
                || !TryAxis(one, two, Vector3d.Cross(one.GetAxis(2), two.GetAxis(0)), toCentre)
                || !TryAxis(one, two, Vector3d.Cross(one.GetAxis(2), two.GetAxis(1)), toCentre)
                || !TryAxis(one, two, Vector3d.Cross(one.GetAxis(2), two.GetAxis(2)), toCentre))
            {
                return false;
            }

            potentialContact.type = 1;
            return true;
        }


        public static bool BoxAndSphere(CollisionBox box, CollisionSphere sphere, RigidContactPotential potentialContact)
        {
            Vector3d centre = sphere.GetAxis(3);
            Vector3d relCentre = box.Transform.TransformInverse(centre);

            if (REAL.Abs(relCentre.x) - sphere.Radius > box.HalfSize.x ||
                REAL.Abs(relCentre.y) - sphere.Radius > box.HalfSize.y ||
                REAL.Abs(relCentre.z) - sphere.Radius > box.HalfSize.z)
            {
                return false;
            }

            Vector3d closestPt = new Vector3d(0, 0, 0);
            REAL dist;

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
            Vector3d v0 = capsule2.CenterOne - capsule1.CenterOne;
            Vector3d v1 = capsule2.CenterTwo - capsule1.CenterOne;
            Vector3d v2 = capsule2.CenterOne - capsule1.CenterTwo;
            Vector3d v3 = capsule2.CenterTwo - capsule1.CenterTwo;

            REAL d0 = Vector3d.Dot(v0, v0);
            REAL d1 = Vector3d.Dot(v1, v1);
            REAL d2 = Vector3d.Dot(v2, v2);
            REAL d3 = Vector3d.Dot(v3, v3);

            Vector3d bestA = Vector3d.Zero; ;
            if (d2 < d0 || d2 < d1 || d3 < d0 || d3 < d1)
            {
                bestA = capsule1.CenterTwo;
            }
            else
            {
                bestA = capsule1.CenterOne;
            }

            Vector3d bestB = ClosestPointOnLineSegment(capsule2.CenterOne, capsule2.CenterTwo, bestA);

            bestA = ClosestPointOnLineSegment(capsule1.CenterOne, capsule1.CenterTwo, bestB);

            Vector3d midline = bestA - bestB;
            REAL size = midline.Magnitude;

            if (size <= 0.0f || size >= capsule1.Radius + capsule2.Radius)
            {
                return false;
            }

            potentialContact.type = 1;
            return true;
        }

        public static bool CapsuleAndSphere(CollisionCapsule capsule, CollisionSphere sphere, RigidContactPotential potentialContact)
        {
            Vector3d bestB = sphere.GetAxis(3);

            Vector3d bestA = ClosestPointOnLineSegment(capsule.CenterOne, capsule.CenterTwo, bestB);

            Vector3d midline = bestA - bestB;
            REAL size = midline.Magnitude;

            if (size <= 0.0f || size >= capsule.Radius + sphere.Radius)
            {
                return false;
            }

            potentialContact.type = 1;
            return true;
        }

        public static bool CapsuleAndBox(CollisionCapsule capsule, CollisionBox box, RigidContactPotential potentialContact)
        {
            Vector3d toCentre = box.GetAxis(3) - capsule.GetAxis(3);

            Vector3d edgeAxis3 = (box.GetAxis(0) + box.GetAxis(1)).Normalized;
            Vector3d edgeAxis4 = (box.GetAxis(0) + box.GetAxis(2)).Normalized;
            Vector3d edgeAxis5 = (box.GetAxis(1) + box.GetAxis(2)).Normalized;

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
            var detector = new GJKDetecotor();
            bool intersect = detector.GJKTest(convex1.Vertices, convex2.Vertices);
            potentialContact.type = intersect ? 1 : 0;
            return intersect;
        }

        public static bool ConvexAndBox(CollisionConvex convex, CollisionBox box, RigidContactPotential potentialContact)
        {
            var detector = new GJKDetecotor();
            bool intersect = detector.GJKTest(convex.Vertices, box.Vertices);
            potentialContact.type = intersect ? 1 : 0;
            return intersect;
        }

        public static bool ConvexAndSphere(CollisionConvex convex, CollisionSphere sphere, RigidContactPotential potentialContact)
        {
            Vector3d normal = Vector3d.Zero;
            Vector3d contactPoint = Vector3d.Zero;
            REAL pen = 0;
            var detector = new GJKDetecotor();
            bool intersect = detector.GJKDist(convex.Vertices, new Vector3d[] { sphere.GetAxis(3) }, ref normal, ref contactPoint, ref pen);
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
            Vector3d normal = Vector3d.Zero;
            Vector3d contactPoint = Vector3d.Zero;
            REAL pen = 0;
            var detector = new GJKDetecotor();
            bool intersect = detector.GJKDist(convex.Vertices, new Vector3d[] { capsule.CenterOne, capsule.CenterTwo }, ref normal, ref contactPoint, ref pen);
            if(!intersect || pen > capsule.Radius)
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
            Vector3d A = triangle.Vertices[0];
            Vector3d B = triangle.Vertices[1];
            Vector3d C = triangle.Vertices[2];
            Vector3d center = sphere.GetAxis(3);
            REAL projection = Vector3d.Dot(center - A, triangle.Normal);
            if (projection > sphere.Radius)
            {
                return false;
            }

            int index = 0;
            Vector3d closetP = CollisionDetector.ClosestPointOnTriangle(center, A, B, C, triangle.Normal, ref index);
            REAL dist = (closetP - center).Magnitude;
            if (dist > sphere.Radius)
            {
                return false;
            }

            potentialContact.type = 1;
            return true;
        }

        public static bool TriangleAndCapsule(CollisionTriangle triangle, CollisionCapsule capsule, RigidContactPotential potentialContact)
        {
            Vector3d A = triangle.Vertices[0];
            Vector3d B = triangle.Vertices[1];
            Vector3d C = triangle.Vertices[2];
            Vector3d center1 = capsule.CenterOne;
            Vector3d center2 = capsule.CenterTwo;
            REAL projection1 = Vector3d.Dot(center1 - A, triangle.Normal);
            REAL projection2 = Vector3d.Dot(center2 - A, triangle.Normal);
            if ((projection1 < -capsule.Radius && projection2 < -capsule.Radius) || (projection1 > capsule.Radius && projection2 > capsule.Radius))
            {
                return false;
            }

            Vector3d closetP = Vector3d.Zero;
            Vector3d closetQ = Vector3d.Zero;
            int featureIndex = 0;
            CollisionDetector.ClosestPointOnLineAndTriangle(center1, center2, A, B, C, triangle.Normal, ref closetP, ref closetQ, ref featureIndex);

            Vector3d midline = closetP - closetQ;
            REAL dist = midline.Magnitude;
            if (featureIndex != 1 && featureIndex != 2 && dist > capsule.Radius)
            {
                return false;
            }

            potentialContact.type = 1;
            return true;
        }

        public static bool TriangleAndBox(CollisionTriangle triangle, CollisionBox box, RigidContactPotential potentialContact)
        {
            Vector3d A = triangle.Vertices[0];
            Vector3d B = triangle.Vertices[1];
            Vector3d C = triangle.Vertices[2];
            Vector3d center = box.GetAxis(3);

            REAL projection1 = box.HalfSize.x * Vector3d.AbsDot(triangle.Normal, box.GetAxis(0)) +
            box.HalfSize.y * Vector3d.AbsDot(triangle.Normal, box.GetAxis(1)) +
            box.HalfSize.z * Vector3d.AbsDot(triangle.Normal, box.GetAxis(2));
            REAL projection2 = Vector3d.Dot(center - A, triangle.Normal);
            REAL pen1 = projection1 - projection2;
            if (projection2 < 0 || pen1 < 0)
            {
                return false;
            }

            Vector3d relA = box.Transform.TransformInverse(A);
            Vector3d relB = box.Transform.TransformInverse(B);
            Vector3d relC = box.Transform.TransformInverse(C);
            Vector3d[] relVertices = new Vector3d[3] { relA, relB, relC };
            if (!CollisionDetector.BoxIntersectPoint(box, relVertices))
            {
                return false;
            }


            Vector3d f1 = relB - relA;
            Vector3d f2 = relC - relB;
            Vector3d f3 = relA - relC;
            Vector3d[] f = new Vector3d[3] { f1, f2, f3 };
            int boxIndex = -1;
            int triangleIndex = -1;
            Vector3d normal3 = Vector3d.Zero;
            REAL pen3 = 0;
            CollisionDetector.MinDepthEdges(box, relVertices, f, ref pen3, ref normal3, ref boxIndex, ref triangleIndex);
            if(boxIndex == -1 || triangleIndex == -1)
            {
                return false;
            }

            potentialContact.type = 1;
            return true;
        }

        public static bool TriangleAndConvex(CollisionTriangle triangle, CollisionConvex convex, RigidContactPotential potentialContact)
        {
            Vector3d A = triangle.Vertices[0];
            Vector3d center = convex.GetAxis(3);
            REAL projection = Vector3d.Dot(center - A, triangle.Normal);
            if (projection <= 0)
            {
                return false;
            }
            var detector = new GJKDetecotor();
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

        public static Vector3d ClosestPointOnLineSegment(Vector3d linePointA, Vector3d linePointB, Vector3d Point)
        {
            Vector3d AB = linePointB - linePointA;
            float t = Vector3d.Dot(Point - linePointA, AB) / Vector3d.Dot(AB, AB); //
            return linePointA + Saturate(t) * AB;
        }

        public static REAL Saturate(REAL t)
        {
            return REAL.Min(REAL.Max(t, 0), 1);
        }

        private static bool TryAxis(CollisionBox one, CollisionBox two, Vector3d axis, Vector3d toCentre)
        {
            if (axis.SqrMagnitude < 0.0001) return true;
            axis.Normalize();

            REAL penetration = PenetrationOnAxis
                (one, two, axis, toCentre);

            if (penetration < 0) return false;

            return true;
        }

        private static bool TryAxis(CollisionCapsule one, CollisionBox two, Vector3d axis, Vector3d toCentre)
        {
            if (axis.SqrMagnitude < 0.0001) return true;
            axis.Normalize();

            REAL penetration = PenetrationOnAxis
                (one, two, axis, toCentre);

            if (penetration < 0) return false;

            return true;
        }

        private static REAL PenetrationOnAxis(CollisionBox one, CollisionBox two, Vector3d axis, Vector3d toCentre)
        {
            REAL oneProject = TransformToAxis(one, axis);
            REAL twoProject = TransformToAxis(two, axis);

            REAL distance = Vector3d.AbsDot(toCentre, axis);

            return oneProject + twoProject - distance;
        }

        private static REAL PenetrationOnAxis(CollisionCapsule one, CollisionBox two, Vector3d axis, Vector3d toCentre)
        {
            REAL oneProject = TransformToAxis(one, axis);
            REAL twoProject = TransformToAxis(two, axis);

            REAL distance = Vector3d.AbsDot(toCentre, axis);

            return oneProject + twoProject - distance;
        }

        private static REAL TransformToAxis(CollisionBox box, Vector3d axis)
        {
            return
            box.HalfSize.x * Vector3d.AbsDot(axis, box.GetAxis(0)) +
            box.HalfSize.y * Vector3d.AbsDot(axis, box.GetAxis(1)) +
            box.HalfSize.z * Vector3d.AbsDot(axis, box.GetAxis(2));
        }

        private static REAL TransformToAxis(CollisionCapsule capsule, Vector3d axis)
        {
            return
            capsule.Radius * Vector3d.AbsDot(axis, capsule.GetAxis(0)) +
            (capsule.HalfHeight.y + capsule.Radius) * Vector3d.AbsDot(axis, capsule.GetAxis(1)) +
            capsule.Radius * Vector3d.AbsDot(axis, capsule.GetAxis(2));
        }
    }
}
