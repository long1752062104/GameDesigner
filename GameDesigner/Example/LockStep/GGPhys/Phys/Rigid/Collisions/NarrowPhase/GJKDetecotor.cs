using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using GGPhys.Core;
using UnityEngine;
using REAL = FixMath.FP;

namespace GGPhys.Rigid.Collisions
{
    public struct SupportInfo
    {
        public Vector3d vertice1;
        public Vector3d vertice2;

        public SupportInfo(Vector3d one, Vector3d two)
        {
            vertice1 = one;
            vertice2 = two;
        }
    }

    public class Simplex
    {
        public struct SimplexPlane
        {
            public SimplexPlane(Vector3d normal, REAL distance, Vector3d A, Vector3d B, Vector3d C)
            {
                this.normal = normal;
                this.sqrDistance = distance;
                this.A = A;
                this.B = B;
                this.C = C;
                this.originInPlane = OriginInPlane(normal, A, B, C);
            }

            public Vector3d normal;
            public REAL sqrDistance;
            public Vector3d A;
            public Vector3d B;
            public Vector3d C;
            public bool originInPlane;

            public static bool OriginInPlane(Vector3d normal, Vector3d A, Vector3d B, Vector3d C)
            {
                Vector3d AF = normal - A;
                REAL signPAB = REAL.Sign(Vector3d.Dot(normal, Vector3d.Cross(B - A, AF)));
                REAL signPAC = REAL.Sign(Vector3d.Dot(normal, Vector3d.Cross(AF, C - A)));
                if ((signPAB * signPAC) == -1) return false;
                REAL signPBC = REAL.Sign(Vector3d.Dot(normal, Vector3d.Cross(C - B, normal - B)));
                if ((signPBC * signPAB) == -1 || (signPBC * signPAC) == -1) return false;

                return true;
            }

        }

        public List<Vector3d> points;
        public List<SimplexPlane> planes;


        public Vector3d A { get => points[0]; }
        public Vector3d B { get => points[1]; }
        public Vector3d C { get => points[2]; }
        public Vector3d D { get => points[3]; }

        public int Count { get => points.Count; }

        public Simplex()
        {
            points = new List<Vector3d>();
            planes = new List<SimplexPlane>();
        }

        public void Clear()
        {
            points.Clear();
            planes.Clear();
        }

        public void Add(Vector3d point)
        {
            points.Add(point);
        }

        public void RemoveAt(int index)
        {
            points.RemoveAt(index);
        }

        public bool ContainsPoint(Vector3d point)
        {
            if (Count < 4) return false;

            Vector3d AB = B - A;
            Vector3d BC = C - B;
            Vector3d AC = C - A;
            Vector3d CD = D - C;
            Vector3d AD = D - A;
            Vector3d DB = B - D;
            Vector3d BD = D - B;
            Vector3d DC = C - D;

            Vector3d NormalABC = Vector3d.Cross(AB, BC);
            Vector3d NormalACD = Vector3d.Cross(AC, CD);
            Vector3d NormalADB = Vector3d.Cross(AD, DB);
            Vector3d NormalBDC = Vector3d.Cross(BD, DC);

            Vector3d AP = point - A;
            Vector3d BP = point - B;

            //可能为0
            int signABC = REAL.Sign(Vector3d.Dot(NormalABC, AP));
            if (signABC == 0) return false;
            int signACD = REAL.Sign(Vector3d.Dot(NormalACD, AP));
            if (signACD == 0 || signACD * signABC == -1) return false;
            int signADB = REAL.Sign(Vector3d.Dot(NormalADB, AP));
            if (signADB == 0 || signADB * signABC == -1) return false;
            int signBDC = REAL.Sign(Vector3d.Dot(NormalBDC, BP));
            if (signBDC == 0 || signBDC * signABC == -1) return false;

            return true;
        }

        public SimplexPlane? FindClosestPlane()
        {
            if (planes.Count == 0) return null;
            int index = 0;
            for (int i = 1; i < planes.Count; i++)
            {
                index = planes[index].sqrDistance < planes[i].sqrDistance ? index : i;
            }
            return planes[index];
        }

        public void InitPlanes()
        {
            if (points.Count < 4) return;
            planes.Clear();
            Vector3d NormalABC = GJKDetecotor.FootPointOnPlane(A, B, C, Vector3d.Zero);
            Vector3d NormalACD = GJKDetecotor.FootPointOnPlane(A, C, D, Vector3d.Zero);
            Vector3d NormalADB = GJKDetecotor.FootPointOnPlane(A, D, B, Vector3d.Zero);
            Vector3d NormalBDC = GJKDetecotor.FootPointOnPlane(B, D, C, Vector3d.Zero);

            SimplexPlane planeABC = new SimplexPlane(NormalABC, NormalABC.SqrMagnitude, A, B, C);
            SimplexPlane planeACD = new SimplexPlane(NormalACD, NormalACD.SqrMagnitude, A, C, D);
            SimplexPlane planeADB = new SimplexPlane(NormalADB, NormalADB.SqrMagnitude, A, D, B);
            SimplexPlane planeBDC = new SimplexPlane(NormalBDC, NormalBDC.SqrMagnitude, B, D, C);

            planes.Add(planeABC);
            planes.Add(planeACD);
            planes.Add(planeADB);
            planes.Add(planeBDC);
        }

        public void GeneratePlanes()
        {
            if (points.Count < 3) return;
            planes.Clear();
            if (points.Count == 3)
            {
                Vector3d NormalABC = GJKDetecotor.FootPointOnPlane(A, B, C, Vector3d.Zero);
                SimplexPlane planeABC = new SimplexPlane(NormalABC, NormalABC.SqrMagnitude, A, B, C);
                planes.Add(planeABC);
            }
            else
            {
                Vector3d NormalABC = GJKDetecotor.FootPointOnPlane(A, B, C, Vector3d.Zero);
                Vector3d NormalACD = GJKDetecotor.FootPointOnPlane(A, C, D, Vector3d.Zero);
                Vector3d NormalADB = GJKDetecotor.FootPointOnPlane(A, D, B, Vector3d.Zero);
                Vector3d NormalBDC = GJKDetecotor.FootPointOnPlane(B, D, C, Vector3d.Zero);

                SimplexPlane planeABC = new SimplexPlane(NormalABC, NormalABC.SqrMagnitude, A, B, C);
                SimplexPlane planeACD = new SimplexPlane(NormalACD, NormalACD.SqrMagnitude, A, C, D);
                SimplexPlane planeADB = new SimplexPlane(NormalADB, NormalADB.SqrMagnitude, A, D, B);
                SimplexPlane planeBDC = new SimplexPlane(NormalBDC, NormalBDC.SqrMagnitude, B, D, C);

                planes.Add(planeABC);
                planes.Add(planeACD);
                planes.Add(planeADB);
                planes.Add(planeBDC);
            } 
        }


        public void InsertPlanePoint(Vector3d point, SimplexPlane plane)
        {
            Vector3d NormalPAB = GJKDetecotor.FootPointOnPlane(point, plane.A, plane.B, Vector3d.Zero);
            Vector3d NormalPAC = GJKDetecotor.FootPointOnPlane(point, plane.A, plane.C, Vector3d.Zero);
            Vector3d NormalPBC = GJKDetecotor.FootPointOnPlane(point, plane.B, plane.C, Vector3d.Zero);

            SimplexPlane planePAB = new SimplexPlane(NormalPAB, NormalPAB.SqrMagnitude, point, plane.A, plane.B);
            SimplexPlane planePAC = new SimplexPlane(NormalPAC, NormalPAC.SqrMagnitude, point, plane.A, plane.C);
            SimplexPlane planePBC = new SimplexPlane(NormalPBC, NormalPBC.SqrMagnitude, point, plane.B, plane.C);

            RemovePlane(plane);


            if(planePAB.originInPlane)
                planes.Add(planePAB);
            if (planePAC.originInPlane)
                planes.Add(planePAC);
            if (planePBC.originInPlane)
                planes.Add(planePBC);

            Add(point);
        }

        void RemovePlane(SimplexPlane plane)
        {
            for (int i = planes.Count - 1; i >= 0; i--)
            {
                var p = planes[i];
                if (p.normal == plane.normal
                    && p.sqrDistance == plane.sqrDistance
                    && p.A == plane.A
                    && p.B == plane.B
                    && p.C == plane.C)
                    planes.Remove(plane);
            }
        }
    }

    public class GJKDetecotor
    {
        public Simplex simplex;
        private Dictionary<Vector3d, SupportInfo> supports;


        public GJKDetecotor()
        {
            simplex = new Simplex();
            supports = new Dictionary<Vector3d, SupportInfo>();
        }

        public void Clear()
        {
            simplex.Clear();
            supports.Clear();
        }

        public bool GJKTest(Vector3d[] vertices1, Vector3d[] vertices2)
        {
            simplex.Clear();
            supports.Clear();
            // 得到初始的方向
            Vector3d direction = FindFirstDirection(vertices1, vertices2);
            // 得到首个support点
            simplex.Add(Support(vertices1, vertices2, direction));
            // 得到第二个方向
            direction = -direction;

            var maxIterations = vertices1.Length + vertices2.Length;
            for (int i = 0; i < maxIterations; i++)
            {
                Vector3d p = Support(vertices1, vertices2, direction);
                // 沿着dir的方向，已经找不到能够跨越原点的support点了。
                if (Vector3d.Dot(p, direction) < 0)
                    return false;

                simplex.Add(p);

                // 单形体包含原点了
                if (simplex.ContainsPoint(Vector3d.Zero))
                {
                    return true;
                }

                direction = FindNextDirection();
            }

            Debug.Log("MaxIterations----" + "GJK");
            return false;
        }

        public bool GJK(Vector3d[] vertices1, Vector3d[] vertices2, ref Vector3d normal, ref Vector3d contactPoint, ref REAL penetration)
        {
            simplex.Clear();
            supports.Clear();
            // 得到初始的方向
            Vector3d direction = FindFirstDirection(vertices1, vertices2);
            // 得到首个support点
            simplex.Add(Support(vertices1, vertices2, direction));
            // 得到第二个方向
            direction = -direction;

            var maxIterations = vertices1.Length + vertices2.Length;
            for (int i = 0; i < maxIterations; i++)
            {
                Vector3d p = Support(vertices1, vertices2, direction);
                // 沿着dir的方向，已经找不到能够跨越原点的support点了。
                if (Vector3d.Dot(p, direction) < 0)
                    return false;

                simplex.Add(p);

                // 单形体包含原点了
                if (simplex.ContainsPoint(Vector3d.Zero))
                {
                    if(EPA(vertices1, vertices2, ref normal, ref contactPoint, ref penetration))
                    {
                        return true;
                    }
                    return false;
                }

                direction = FindNextDirection();
            }

            Debug.Log("MaxIterations----" + "GJK");
            return false;
        }

        public bool GJKDist(Vector3d[] vertices1, Vector3d[] vertices2, ref Vector3d normal, ref Vector3d contactPoint, ref REAL penetration)
        {
            simplex.Clear();
            supports.Clear();
            // 得到初始的方向
            Vector3d direction = FindFirstDirection(vertices1, vertices2);
            // 得到首个support点
            simplex.Add(Support(vertices1, vertices2, direction));
            // 得到第二个方向
            direction = -direction;

            var maxIterations = vertices1.Length + vertices2.Length;
            for (int i = 0; i < maxIterations; i++)
            {
                Vector3d p = Support(vertices1, vertices2, direction);
                // 沿着dir的方向，已经找不到能够跨越原点的support点了。
                if (Vector3d.Dot(p, direction) < 0)
                {
                    simplex.GeneratePlanes();
                    var plane = simplex.FindClosestPlane();
                    if (plane == null) return false;
                    Simplex.SimplexPlane simplexPlane = (Simplex.SimplexPlane)plane;
                    if (!simplexPlane.originInPlane) return false;
                    normal = simplexPlane.normal.Normalized;
                    penetration = REAL.SqrtFastest(simplexPlane.sqrDistance);
                    contactPoint = DistContactPoint(simplexPlane.A, simplexPlane.B, simplexPlane.C, simplexPlane.normal);
                    return true;
                }
                simplex.Add(p);
                direction = FindNextDirection();
            }

            Debug.Log("MaxIterations----" + "GJK");
            return false;
        }

        public bool EPA(Vector3d[] vertices1, Vector3d[] vertices2, ref Vector3d normal, ref Vector3d contactPoint, ref REAL penetration)
        {
            int maxIterations = vertices1.Length + vertices2.Length;
            simplex.InitPlanes();
            for (int i = 0; i < maxIterations; i++)
            {
                // 找到距离原点最近的边
                Simplex.SimplexPlane? p = simplex.FindClosestPlane();
                if (p == null) break;
                Simplex.SimplexPlane plane = (Simplex.SimplexPlane)p;
                // 沿着边的法线方向，尝试找一个新的support点
                Vector3d point = Support(vertices1, vertices2, plane.normal);
                // 无法找到能够跨越该边的support点了。也就是说，该边就是差集最近边
                REAL distance = Vector3d.Dot(point, plane.normal);
                if (distance - plane.sqrDistance <= 0.001)
                {
                    // 返回碰撞信息
                    normal = - plane.normal.Normalized;
                    penetration = REAL.SqrtFastest(plane.sqrDistance);
                    contactPoint = ContactPoint(plane.A, plane.B, plane.C, plane.normal);
                    return true;
                }

                simplex.InsertPlanePoint(point, plane);
            }
            Debug.Log("MaxIterations----" + "EPA");
            return false;
        }

        Vector3d ContactPoint(Vector3d sp1, Vector3d sp2, Vector3d sp3, Vector3d normal)
        {
            var si1 = supports[sp1];
            var si2 = supports[sp2];
            var si3 = supports[sp3];

            if ((si1.vertice1 == si2.vertice1) && (si2.vertice1 == si3.vertice1))
            {
                return si1.vertice1;
            }

            if ((si1.vertice2 == si2.vertice2) && (si2.vertice2 == si3.vertice2))
            {
                return si1.vertice2;
            }

            Vector3d one1 = si1.vertice1;
            Vector3d one2 = si2.vertice1 == one1 ? si3.vertice1 : si2.vertice1;
            Vector3d two1 = si1.vertice2;
            Vector3d two2 = si2.vertice2 == two1 ? si3.vertice2 : si2.vertice2;

            return ClosestPointOnTwoLines(one1, one2, two1, two2);

        }

        Vector3d DistContactPoint(Vector3d sp1, Vector3d sp2, Vector3d sp3, Vector3d normal)
        {
            var si1 = supports[sp1];
            var si2 = supports[sp2];
            var si3 = supports[sp3];

            if ((si1.vertice1 == si2.vertice1) && (si2.vertice1 == si3.vertice1))
            {
                return si1.vertice1 + normal;
            }

            if ((si1.vertice2 == si2.vertice2) && (si2.vertice2 == si3.vertice2))
            {
                return si1.vertice2 + normal;
            }

            Vector3d one1 = si1.vertice1;
            Vector3d one2 = si2.vertice1 == one1 ? si3.vertice1 : si2.vertice1;
            Vector3d two1 = si1.vertice2;
            Vector3d two2 = si2.vertice2 == two1 ? si3.vertice2 : si2.vertice2;

            return ClosestPointOnTwoLines(one1, one2, two1, two2);

        }

        Vector3d FindFirstDirection(Vector3d[] vertices1, Vector3d[] vertices2, int startIndex = 0)
        {
            if ((vertices1.Length <= startIndex) || (vertices2.Length <= startIndex)) return Vector3d.One;
            Vector3d direction = vertices1[startIndex] - vertices2[startIndex];
            if(direction == Vector3d.Zero)
            {
                int index = startIndex;
                index++;
                return FindFirstDirection(vertices1, vertices2, index);
            }
            return direction;

        }

        Vector3d Support(Vector3d[] vertices1, Vector3d[] vertices2, Vector3d dir)
        {
            Vector3d a = GetFarthestPointInDirection(vertices1, dir);
            Vector3d b = GetFarthestPointInDirection(vertices2, -dir);
            Vector3d support = a - b;
            CacheSupport(support, a, b);
            return support;
        }

        void CacheSupport(Vector3d support,Vector3d vertice1, Vector3d vertice2)
        {
            if (supports.ContainsKey(support)) return;
            var si = new SupportInfo(vertice1, vertice2);
            supports.Add(support, si);
        }

        Vector3d GetFarthestPointInDirection(Vector3d[] vertices, Vector3d direction)
        {
            REAL maxDistance = REAL.MinValue;
            int maxIndex = 0;
            for (int i = 0; i < vertices.Length; ++i)
            {
                REAL distance = Vector3d.Dot(vertices[i], direction);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    maxIndex = i;
                }
            }
            return vertices[maxIndex];
        }

        Vector3d FindNextDirection()
        {
            int count = simplex.Count;
            if (count == 2)
            {
                // 计算原点到直线01的垂足
                Vector3d crossPoint = ClosestPointOnLine(simplex.A, simplex.B, Vector3d.Zero);
                // 取靠近原点方向的向量
                return Vector3d.Zero - crossPoint;
            }
            else if(count == 3)
            {
                // 计算原点到面012的垂足
                Vector3d crossPoint = FootPointOnPlane(simplex.A, simplex.B, simplex.C, Vector3d.Zero);
                return Vector3d.Zero - crossPoint;
            }
            else if (count == 4)
            {
                // 计算原点到面301的垂足
                Vector3d crossOnDAB = FootPointOnPlane(simplex.D, simplex.A, simplex.B, Vector3d.Zero);
                // 计算原点到面302的垂足
                Vector3d crossOnDAC = FootPointOnPlane(simplex.D, simplex.A, simplex.C, Vector3d.Zero);
                // 计算原点到面312的垂足
                Vector3d crossOnDBC = FootPointOnPlane(simplex.D, simplex.B, simplex.C, Vector3d.Zero);

                REAL originToDAB = crossOnDAB.SqrMagnitude;
                REAL originToDAC = crossOnDAC.SqrMagnitude;
                REAL originToDBC = crossOnDBC.SqrMagnitude;

                int minIndex = MinIndex(originToDAB, originToDAC, originToDBC);

                // 保留距离原点最近的一个面
                if (minIndex == 1)
                {
                    simplex.RemoveAt(2);
                    return Vector3d.Zero - crossOnDAB;
                }
                if (minIndex == 2)
                {
                    simplex.RemoveAt(1);
                    return Vector3d.Zero - crossOnDAC;
                }
                else
                {
                    simplex.RemoveAt(0);
                    return Vector3d.Zero - crossOnDBC;
                }
            }
            else
            {
                // 不应该执行到这里
                return Vector3d.Zero;
            }
        }

        public static Vector3d ClosestPointOnLine(Vector3d linePointA, Vector3d linePointB, Vector3d point)
        {
            Vector3d AB = linePointB - linePointA;
            REAL t = Vector3d.Dot(point - linePointA, AB) / Vector3d.Dot(AB, AB); 
            return linePointA + t * AB;
        }

        /// <summary>
        /// 第二条线段离第一条线段的最近点
        /// </summary>
        /// <param name="lineOnePointA"></param>
        /// <param name="lineOnePointB"></param>
        /// <param name="lineTwoPointA"></param>
        /// <param name="lineTwoPointB"></param>
        /// <returns></returns>
        public static Vector3d ClosestPointFromLineTwo(Vector3d lineOnePointA, Vector3d lineOnePointB, Vector3d lineTwoPointA, Vector3d lineTwoPointB)
        {
            Vector3d bestA = Vector3d.Zero;
            Vector3d bestB = Vector3d.Zero;
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
        public static void ClosestPoinsOnTwoLines(Vector3d lineOnePointA, Vector3d lineOnePointB, Vector3d lineTwoPointA, Vector3d lineTwoPointB, ref Vector3d bestA, ref Vector3d bestB)
        {
            REAL episolon = 0.0001;
            Vector3d d1 = lineOnePointB - lineOnePointA;
            Vector3d d2 = lineTwoPointB - lineTwoPointA;
            Vector3d r = lineOnePointA - lineTwoPointA;
            REAL a = Vector3d.Dot(d1, d1);
            REAL e = Vector3d.Dot(d2, d2);
            REAL f = Vector3d.Dot(d2, r);

            REAL s = 0;
            REAL t = 0;

            if (a <= episolon && e <= episolon)
            {
                s = t = 0;
                bestA = lineOnePointA;
                bestB = lineTwoPointA;
                return;
            }

            if (a <= episolon)
            {
                s = 0;
                t = f / e;
                t = REAL.Clamp(t, 0, 1);
            }
            else
            {
                REAL c = Vector3d.Dot(d1, r);
                if (e <= episolon)
                {
                    t = 0;
                    s = REAL.Clamp(-c / a, 0, 1);
                }
                else
                {
                    REAL b = Vector3d.Dot(d1, d2);
                    REAL denom = a * e - b * b;
                    s = denom == 0 ? 0 : REAL.Clamp((b * f - c * e) / denom, 0, 1);

                    REAL tnom = b * s + f;
                    if (tnom < 0)
                    {
                        t = 0;
                        s = REAL.Clamp(-c / a, 0, 1);
                    }
                    else if (tnom > e)
                    {
                        t = 1;
                        s = REAL.Clamp((b - c) / a, 0, 1);
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
        public static Vector3d ClosestPointOnTwoLines(Vector3d lineOnePointA, Vector3d lineOnePointB, Vector3d lineTwoPointA, Vector3d lineTwoPointB)
        {
            Vector3d bestA = Vector3d.Zero;
            Vector3d bestB = Vector3d.Zero;
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
        public static Vector3d ClosestPointOnLineSegment(Vector3d linePointA, Vector3d linePointB, Vector3d Point)
        {
            Vector3d AB = linePointB - linePointA;
            REAL t = Vector3d.Dot(Point - linePointA, AB) / Vector3d.Dot(AB, AB);
            return linePointA + REAL.Clamp(t, 0, 1) * AB;
        }

        public static Vector3d FootPointOnPlane(Vector3d planePointA, Vector3d planePointB, Vector3d planePointC, Vector3d point)
        {
            Vector3d normal = Vector3d.Cross(planePointB - planePointA, planePointC - planePointA);// not normalized
            if (normal == Vector3d.Zero) return Vector3d.Zero;

            Vector3d PA= planePointA - point;
            REAL t = Vector3d.Dot(PA, normal) / Vector3d.Dot(normal, normal);
            return point + t * normal;
        }

        public static REAL PlaneCenterDistance(Vector3d planePointA, Vector3d planePointB, Vector3d planePointC)
        {
            REAL x = (planePointA.x + planePointB.x + planePointC.x) * 0.33333;
            REAL y = (planePointA.y + planePointB.y + planePointC.y) * 0.33333;
            REAL z = (planePointA.z + planePointB.z + planePointC.z) * 0.33333;
            return x * x + y * y + z * z;
        }


        public static int MinIndex(REAL one, REAL two, REAL three)
        {
            int index = one < two ? 1 : 2;
            if(index == 1)
            {
                index = one < three ? 1 : 3;
            }
            else
            {
                index = two < three ? 2 : 3;
            }
            return index;
        }

        public static int MaxIndex(REAL one, REAL two, REAL three)
        {
            int index = one > two ? 1 : 2;
            if (index == 1)
            {
                index = one > three ? 1 : 3;
            }
            else
            {
                index = two > three ? 2 : 3;
            }
            return index;
        }
    }
}


