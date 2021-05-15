using FixMath;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;

namespace GGPhys.Rigid.Collisions
{
    public struct SupportInfo
    {
        public TSVector3 vertice1;
        public TSVector3 vertice2;

        public SupportInfo(TSVector3 one, TSVector3 two)
        {
            vertice1 = one;
            vertice2 = two;
        }
    }

    public class Simplex
    {
        public struct SimplexPlane
        {
            public SimplexPlane(TSVector3 normal, FP distance, TSVector3 A, TSVector3 B, TSVector3 C)
            {
                this.normal = normal;
                sqrDistance = distance;
                this.A = A;
                this.B = B;
                this.C = C;
                originInPlane = OriginInPlane(normal, A, B, C);
            }

            public TSVector3 normal;
            public FP sqrDistance;
            public TSVector3 A;
            public TSVector3 B;
            public TSVector3 C;
            public bool originInPlane;

            public static bool OriginInPlane(TSVector3 normal, TSVector3 A, TSVector3 B, TSVector3 C)
            {
                TSVector3 AF = normal - A;
                FP signPAB = FP.Sign(TSVector3.Dot(normal, TSVector3.Cross(B - A, AF)));
                FP signPAC = FP.Sign(TSVector3.Dot(normal, TSVector3.Cross(AF, C - A)));
                if ((signPAB * signPAC) == -1) return false;
                FP signPBC = FP.Sign(TSVector3.Dot(normal, TSVector3.Cross(C - B, normal - B)));
                if ((signPBC * signPAB) == -1 || (signPBC * signPAC) == -1) return false;

                return true;
            }

        }

        public List<TSVector3> points;
        public List<SimplexPlane> planes;


        public TSVector3 A { get => points[0]; }
        public TSVector3 B { get => points[1]; }
        public TSVector3 C { get => points[2]; }
        public TSVector3 D { get => points[3]; }

        public int Count { get => points.Count; }

        public Simplex()
        {
            points = new List<TSVector3>();
            planes = new List<SimplexPlane>();
        }

        public void Clear()
        {
            points.Clear();
            planes.Clear();
        }

        public void Add(TSVector3 point)
        {
            points.Add(point);
        }

        public void RemoveAt(int index)
        {
            points.RemoveAt(index);
        }

        public bool ContainsPoint(TSVector3 point)
        {
            if (Count < 4) return false;

            TSVector3 AB = B - A;
            TSVector3 BC = C - B;
            TSVector3 AC = C - A;
            TSVector3 CD = D - C;
            TSVector3 AD = D - A;
            TSVector3 DB = B - D;
            TSVector3 BD = D - B;
            TSVector3 DC = C - D;

            TSVector3 NormalABC = TSVector3.Cross(AB, BC);
            TSVector3 NormalACD = TSVector3.Cross(AC, CD);
            TSVector3 NormalADB = TSVector3.Cross(AD, DB);
            TSVector3 NormalBDC = TSVector3.Cross(BD, DC);

            TSVector3 AP = point - A;
            TSVector3 BP = point - B;

            //可能为0
            int signABC = FP.Sign(TSVector3.Dot(NormalABC, AP));
            if (signABC == 0) return false;
            int signACD = FP.Sign(TSVector3.Dot(NormalACD, AP));
            if (signACD == 0 || signACD * signABC == -1) return false;
            int signADB = FP.Sign(TSVector3.Dot(NormalADB, AP));
            if (signADB == 0 || signADB * signABC == -1) return false;
            int signBDC = FP.Sign(TSVector3.Dot(NormalBDC, BP));
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
            TSVector3 NormalABC = GJKDetecotor.FootPointOnPlane(A, B, C, TSVector3.Zero);
            TSVector3 NormalACD = GJKDetecotor.FootPointOnPlane(A, C, D, TSVector3.Zero);
            TSVector3 NormalADB = GJKDetecotor.FootPointOnPlane(A, D, B, TSVector3.Zero);
            TSVector3 NormalBDC = GJKDetecotor.FootPointOnPlane(B, D, C, TSVector3.Zero);

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
                TSVector3 NormalABC = GJKDetecotor.FootPointOnPlane(A, B, C, TSVector3.Zero);
                SimplexPlane planeABC = new SimplexPlane(NormalABC, NormalABC.SqrMagnitude, A, B, C);
                planes.Add(planeABC);
            }
            else
            {
                TSVector3 NormalABC = GJKDetecotor.FootPointOnPlane(A, B, C, TSVector3.Zero);
                TSVector3 NormalACD = GJKDetecotor.FootPointOnPlane(A, C, D, TSVector3.Zero);
                TSVector3 NormalADB = GJKDetecotor.FootPointOnPlane(A, D, B, TSVector3.Zero);
                TSVector3 NormalBDC = GJKDetecotor.FootPointOnPlane(B, D, C, TSVector3.Zero);

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


        public void InsertPlanePoint(TSVector3 point, SimplexPlane plane)
        {
            TSVector3 NormalPAB = GJKDetecotor.FootPointOnPlane(point, plane.A, plane.B, TSVector3.Zero);
            TSVector3 NormalPAC = GJKDetecotor.FootPointOnPlane(point, plane.A, plane.C, TSVector3.Zero);
            TSVector3 NormalPBC = GJKDetecotor.FootPointOnPlane(point, plane.B, plane.C, TSVector3.Zero);

            SimplexPlane planePAB = new SimplexPlane(NormalPAB, NormalPAB.SqrMagnitude, point, plane.A, plane.B);
            SimplexPlane planePAC = new SimplexPlane(NormalPAC, NormalPAC.SqrMagnitude, point, plane.A, plane.C);
            SimplexPlane planePBC = new SimplexPlane(NormalPBC, NormalPBC.SqrMagnitude, point, plane.B, plane.C);

            RemovePlane(plane);


            if (planePAB.originInPlane)
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
                SimplexPlane p = planes[i];
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
        private Dictionary<TSVector3, SupportInfo> supports;


        public GJKDetecotor()
        {
            simplex = new Simplex();
            supports = new Dictionary<TSVector3, SupportInfo>();
        }

        public void Clear()
        {
            simplex.Clear();
            supports.Clear();
        }

        public bool GJKTest(TSVector3[] vertices1, TSVector3[] vertices2)
        {
            simplex.Clear();
            supports.Clear();
            // 得到初始的方向
            TSVector3 direction = FindFirstDirection(vertices1, vertices2);
            // 得到首个support点
            simplex.Add(Support(vertices1, vertices2, direction));
            // 得到第二个方向
            direction = -direction;

            int maxIterations = vertices1.Length + vertices2.Length;
            for (int i = 0; i < maxIterations; i++)
            {
                TSVector3 p = Support(vertices1, vertices2, direction);
                // 沿着dir的方向，已经找不到能够跨越原点的support点了。
                if (TSVector3.Dot(p, direction) < 0)
                    return false;

                simplex.Add(p);

                // 单形体包含原点了
                if (simplex.ContainsPoint(TSVector3.Zero))
                {
                    return true;
                }

                direction = FindNextDirection();
            }

            Debug.Log("MaxIterations----" + "GJK");
            return false;
        }

        public bool GJK(TSVector3[] vertices1, TSVector3[] vertices2, ref TSVector3 normal, ref TSVector3 contactPoint, ref FP penetration)
        {
            simplex.Clear();
            supports.Clear();
            // 得到初始的方向
            TSVector3 direction = FindFirstDirection(vertices1, vertices2);
            // 得到首个support点
            simplex.Add(Support(vertices1, vertices2, direction));
            // 得到第二个方向
            direction = -direction;

            int maxIterations = vertices1.Length + vertices2.Length;
            for (int i = 0; i < maxIterations; i++)
            {
                TSVector3 p = Support(vertices1, vertices2, direction);
                // 沿着dir的方向，已经找不到能够跨越原点的support点了。
                if (TSVector3.Dot(p, direction) < 0)
                    return false;

                simplex.Add(p);

                // 单形体包含原点了
                if (simplex.ContainsPoint(TSVector3.Zero))
                {
                    if (EPA(vertices1, vertices2, ref normal, ref contactPoint, ref penetration))
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

        public bool GJKDist(TSVector3[] vertices1, TSVector3[] vertices2, ref TSVector3 normal, ref TSVector3 contactPoint, ref FP penetration)
        {
            simplex.Clear();
            supports.Clear();
            // 得到初始的方向
            TSVector3 direction = FindFirstDirection(vertices1, vertices2);
            // 得到首个support点
            simplex.Add(Support(vertices1, vertices2, direction));
            // 得到第二个方向
            direction = -direction;

            int maxIterations = vertices1.Length + vertices2.Length;
            for (int i = 0; i < maxIterations; i++)
            {
                TSVector3 p = Support(vertices1, vertices2, direction);
                // 沿着dir的方向，已经找不到能够跨越原点的support点了。
                if (TSVector3.Dot(p, direction) < 0)
                {
                    simplex.GeneratePlanes();
                    Simplex.SimplexPlane? plane = simplex.FindClosestPlane();
                    if (plane == null) return false;
                    Simplex.SimplexPlane simplexPlane = (Simplex.SimplexPlane)plane;
                    if (!simplexPlane.originInPlane) return false;
                    normal = simplexPlane.normal.Normalized;
                    penetration = F64.SqrtFastest(simplexPlane.sqrDistance);
                    contactPoint = DistContactPoint(simplexPlane.A, simplexPlane.B, simplexPlane.C, simplexPlane.normal);
                    return true;
                }
                simplex.Add(p);
                direction = FindNextDirection();
            }

            Debug.Log("MaxIterations----" + "GJK");
            return false;
        }

        public bool EPA(TSVector3[] vertices1, TSVector3[] vertices2, ref TSVector3 normal, ref TSVector3 contactPoint, ref FP penetration)
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
                TSVector3 point = Support(vertices1, vertices2, plane.normal);
                // 无法找到能够跨越该边的support点了。也就是说，该边就是差集最近边
                FP distance = TSVector3.Dot(point, plane.normal);
                if (distance - plane.sqrDistance <= 0.001)
                {
                    // 返回碰撞信息
                    normal = -plane.normal.Normalized;
                    penetration = F64.SqrtFastest(plane.sqrDistance);
                    contactPoint = ContactPoint(plane.A, plane.B, plane.C, plane.normal);
                    return true;
                }

                simplex.InsertPlanePoint(point, plane);
            }
            Debug.Log("MaxIterations----" + "EPA");
            return false;
        }

        TSVector3 ContactPoint(TSVector3 sp1, TSVector3 sp2, TSVector3 sp3, TSVector3 normal)
        {
            SupportInfo si1 = supports[sp1];
            SupportInfo si2 = supports[sp2];
            SupportInfo si3 = supports[sp3];

            if ((si1.vertice1 == si2.vertice1) && (si2.vertice1 == si3.vertice1))
            {
                return si1.vertice1;
            }

            if ((si1.vertice2 == si2.vertice2) && (si2.vertice2 == si3.vertice2))
            {
                return si1.vertice2;
            }

            TSVector3 one1 = si1.vertice1;
            TSVector3 one2 = si2.vertice1 == one1 ? si3.vertice1 : si2.vertice1;
            TSVector3 two1 = si1.vertice2;
            TSVector3 two2 = si2.vertice2 == two1 ? si3.vertice2 : si2.vertice2;

            return ClosestPointOnTwoLines(one1, one2, two1, two2);

        }

        TSVector3 DistContactPoint(TSVector3 sp1, TSVector3 sp2, TSVector3 sp3, TSVector3 normal)
        {
            SupportInfo si1 = supports[sp1];
            SupportInfo si2 = supports[sp2];
            SupportInfo si3 = supports[sp3];

            if ((si1.vertice1 == si2.vertice1) && (si2.vertice1 == si3.vertice1))
            {
                return si1.vertice1 + normal;
            }

            if ((si1.vertice2 == si2.vertice2) && (si2.vertice2 == si3.vertice2))
            {
                return si1.vertice2 + normal;
            }

            TSVector3 one1 = si1.vertice1;
            TSVector3 one2 = si2.vertice1 == one1 ? si3.vertice1 : si2.vertice1;
            TSVector3 two1 = si1.vertice2;
            TSVector3 two2 = si2.vertice2 == two1 ? si3.vertice2 : si2.vertice2;

            return ClosestPointOnTwoLines(one1, one2, two1, two2);

        }

        TSVector3 FindFirstDirection(TSVector3[] vertices1, TSVector3[] vertices2, int startIndex = 0)
        {
            if ((vertices1.Length <= startIndex) || (vertices2.Length <= startIndex)) return TSVector3.one;
            TSVector3 direction = vertices1[startIndex] - vertices2[startIndex];
            if (direction == TSVector3.Zero)
            {
                int index = startIndex;
                index++;
                return FindFirstDirection(vertices1, vertices2, index);
            }
            return direction;

        }

        TSVector3 Support(TSVector3[] vertices1, TSVector3[] vertices2, TSVector3 dir)
        {
            TSVector3 a = GetFarthestPointInDirection(vertices1, dir);
            TSVector3 b = GetFarthestPointInDirection(vertices2, -dir);
            TSVector3 support = a - b;
            CacheSupport(support, a, b);
            return support;
        }

        void CacheSupport(TSVector3 support, TSVector3 vertice1, TSVector3 vertice2)
        {
            if (supports.ContainsKey(support)) return;
            SupportInfo si = new SupportInfo(vertice1, vertice2);
            supports.Add(support, si);
        }

        TSVector3 GetFarthestPointInDirection(TSVector3[] vertices, TSVector3 direction)
        {
            FP maxDistance = FP.MinValue;
            int maxIndex = 0;
            for (int i = 0; i < vertices.Length; ++i)
            {
                FP distance = TSVector3.Dot(vertices[i], direction);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    maxIndex = i;
                }
            }
            return vertices[maxIndex];
        }

        TSVector3 FindNextDirection()
        {
            int count = simplex.Count;
            if (count == 2)
            {
                // 计算原点到直线01的垂足
                TSVector3 crossPoint = ClosestPointOnLine(simplex.A, simplex.B, TSVector3.Zero);
                // 取靠近原点方向的向量
                return TSVector3.Zero - crossPoint;
            }
            else if (count == 3)
            {
                // 计算原点到面012的垂足
                TSVector3 crossPoint = FootPointOnPlane(simplex.A, simplex.B, simplex.C, TSVector3.Zero);
                return TSVector3.Zero - crossPoint;
            }
            else if (count == 4)
            {
                // 计算原点到面301的垂足
                TSVector3 crossOnDAB = FootPointOnPlane(simplex.D, simplex.A, simplex.B, TSVector3.Zero);
                // 计算原点到面302的垂足
                TSVector3 crossOnDAC = FootPointOnPlane(simplex.D, simplex.A, simplex.C, TSVector3.Zero);
                // 计算原点到面312的垂足
                TSVector3 crossOnDBC = FootPointOnPlane(simplex.D, simplex.B, simplex.C, TSVector3.Zero);

                FP originToDAB = crossOnDAB.SqrMagnitude;
                FP originToDAC = crossOnDAC.SqrMagnitude;
                FP originToDBC = crossOnDBC.SqrMagnitude;

                int minIndex = MinIndex(originToDAB, originToDAC, originToDBC);

                // 保留距离原点最近的一个面
                if (minIndex == 1)
                {
                    simplex.RemoveAt(2);
                    return TSVector3.Zero - crossOnDAB;
                }
                if (minIndex == 2)
                {
                    simplex.RemoveAt(1);
                    return TSVector3.Zero - crossOnDAC;
                }
                else
                {
                    simplex.RemoveAt(0);
                    return TSVector3.Zero - crossOnDBC;
                }
            }
            else
            {
                // 不应该执行到这里
                return TSVector3.Zero;
            }
        }

        public static TSVector3 ClosestPointOnLine(TSVector3 linePointA, TSVector3 linePointB, TSVector3 point)
        {
            TSVector3 AB = linePointB - linePointA;
            FP t = TSVector3.Dot(point - linePointA, AB) / TSVector3.Dot(AB, AB);
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

            FP s = 0;
            FP t = 0;

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

        public static TSVector3 FootPointOnPlane(TSVector3 planePointA, TSVector3 planePointB, TSVector3 planePointC, TSVector3 point)
        {
            TSVector3 normal = TSVector3.Cross(planePointB - planePointA, planePointC - planePointA);// not normalized
            if (normal == TSVector3.Zero) return TSVector3.Zero;

            TSVector3 PA = planePointA - point;
            FP t = TSVector3.Dot(PA, normal) / TSVector3.Dot(normal, normal);
            return point + t * normal;
        }

        public static FP PlaneCenterDistance(TSVector3 planePointA, TSVector3 planePointB, TSVector3 planePointC)
        {
            FP x = (planePointA.x + planePointB.x + planePointC.x) * 0.33333;
            FP y = (planePointA.y + planePointB.y + planePointC.y) * 0.33333;
            FP z = (planePointA.z + planePointB.z + planePointC.z) * 0.33333;
            return x * x + y * y + z * z;
        }


        public static int MinIndex(FP one, FP two, FP three)
        {
            int index = one < two ? 1 : 2;
            if (index == 1)
            {
                index = one < three ? 1 : 3;
            }
            else
            {
                index = two < three ? 2 : 3;
            }
            return index;
        }

        public static int MaxIndex(FP one, FP two, FP three)
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


