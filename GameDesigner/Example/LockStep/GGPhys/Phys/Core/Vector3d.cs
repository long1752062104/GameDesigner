using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using REAL = FixMath.FP;
using UnityEngine;

#if GGPHYS_FIXPOINT32
using RawType = System.Int32;
using FixedType = FixPointCS.Fixed32;
#else
using RawType = System.Int64;
using FixedType = FixPointCS.Fixed64;
#endif

namespace GGPhys.Core
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3d : IEquatable<Vector3d>, IComparable<Vector3d>
    {

        //public RawType x.Raw;
        //public RawType y.Raw;
        //public RawType z.Raw;

        public REAL x;//{ get { return REAL.FromRaw(x.Raw); } set { x.Raw = value.Raw; } }
        public REAL y;//{ get { return REAL.FromRaw(y.Raw); } set { y.Raw = value.Raw; } }
        public REAL z;//{ get { return REAL.FromRaw(z.Raw); } set { z.Raw = value.Raw; } }

        /// <summary>
        /// x轴单位向量
        /// </summary>
        public readonly static Vector3d UnitX = new Vector3d(REAL.One, REAL.Zero, REAL.Zero);

        /// <summary>
        /// y轴单位向量
        /// </summary>
	    public readonly static Vector3d UnitY = new Vector3d(REAL.Zero, REAL.One, REAL.Zero);

        /// <summary>
        /// z轴单位向量
        /// </summary>
	    public readonly static Vector3d UnitZ = new Vector3d(REAL.Zero, REAL.Zero, REAL.One);

        /// <summary>
        /// 全0向量
        /// </summary>
	    public readonly static Vector3d Zero = new Vector3d(REAL.Zero);

        /// <summary>
        /// 全1向量
        /// </summary>
        public readonly static Vector3d One = new Vector3d(REAL.One);

        /// <summary>
        /// 全0.5向量
        /// </summary>
        public readonly static Vector3d Half = new Vector3d(REAL.Half);

        /// <summary>
        /// 无穷正向量
        /// </summary>
        public readonly static Vector3d PositiveInfinity = new Vector3d(REAL.MaxValue);

        /// <summary>
        /// 无穷负向量
        /// </summary>
        public readonly static Vector3d NegativeInfinity = new Vector3d(REAL.MinValue);

        /// <summary>
        /// 根据一个值构造向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3d(REAL v)
        {
            this.x.Raw = v.Raw;
            this.y.Raw = v.Raw;
            this.z.Raw = v.Raw;
        }

        /// <summary>
        /// 根据三个值构造向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3d(REAL x, REAL y, REAL z)
        {
            this.x.Raw = x.Raw;
            this.y.Raw = y.Raw;
            this.z.Raw = z.Raw;
        }

        /// <summary>
        /// 构造向量
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vector3d(RawType x, RawType y, RawType z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3d(REAL x, REAL y)
        {
            this.x.Raw = x.Raw;
            this.y.Raw = y.Raw;
            this.z.Raw = REAL.Zero.Raw;
        }

        /// <summary>
        /// 下标读写
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        unsafe public REAL this[int i]
        {
            get
            {
                if ((uint)i >= 3)
                    throw new IndexOutOfRangeException("Vector3d index out of range.");

                fixed (Vector3d* array = &this) { return REAL.FromRaw(((RawType*)array)[i]); }
            }
            set
            {
                if ((uint)i >= 3)
                    throw new IndexOutOfRangeException("Vector3d index out of range.");

                fixed (RawType* array = &x.Raw) { array[i] = value.Raw; }
            }
        }

        /// <summary>
        /// 向量模
        /// </summary>
        public REAL Magnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return REAL.FromRaw(FixedType.SqrtFastest(FixedType.Mul(x.Raw, x.Raw) + FixedType.Mul(y.Raw, y.Raw) + FixedType.Mul(z.Raw, z.Raw)));
            }
        }

        /// <summary>
        /// 向量平方长度
        /// </summary>
		public REAL SqrMagnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return REAL.FromRaw(FixedType.Mul(x.Raw, x.Raw) + FixedType.Mul(y.Raw, y.Raw) + FixedType.Mul(z.Raw, z.Raw));
            }
        }

        /// <summary>
        /// 向量单位化
        /// </summary>
        public Vector3d Normalized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                RawType invLength = FixedType.RSqrtFastest(FixedType.Mul(x.Raw, x.Raw) + FixedType.Mul(y.Raw, y.Raw) + FixedType.Mul(z.Raw, z.Raw));
                return new Vector3d(FixedType.Mul(x.Raw, invLength), FixedType.Mul(y.Raw, invLength), FixedType.Mul(z.Raw, invLength));
            }
        }

        /// <summary>
        /// 从RawType构造
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3d FromRaw(RawType x, RawType y, RawType z)
        {
            return new Vector3d(x, y, z);
        }

        /// <summary>
        /// 向量加法
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator +(Vector3d v1, Vector3d v2)
        {
            return new Vector3d(v1.x.Raw + v2.x.Raw, v1.y.Raw + v2.y.Raw, v1.z.Raw + v2.z.Raw);
        }

        /// <summary>
        /// 向量加一个数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator +(Vector3d v1, REAL s)
        {
            return new Vector3d(v1.x.Raw + s.Raw, v1.y.Raw + s.Raw, v1.z.Raw + s.Raw);
        }

        /// <summary>
        /// 数加向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator +(REAL s, Vector3d v1)
        {
            return new Vector3d(v1.x.Raw + s.Raw, v1.y.Raw + s.Raw, v1.z.Raw + s.Raw);
        }

        /// <summary>
        /// 向量取反
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator -(Vector3d v)
        {
            return new Vector3d(-v.x.Raw, -v.y.Raw, -v.z.Raw);
        }

        /// <summary>
        /// 向量减法
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator -(Vector3d v1, Vector3d v2)
        {
            return new Vector3d(v1.x.Raw - v2.x.Raw, v1.y.Raw - v2.y.Raw, v1.z.Raw - v2.z.Raw);
        }

        /// <summary>
        /// 向量减一个数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator -(Vector3d v1, REAL s)
        {
            return new Vector3d(v1.x.Raw - s.Raw, v1.y.Raw - s.Raw, v1.z.Raw - s.Raw);
        }

        /// <summary>
        /// 一个数减向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator -(REAL s, Vector3d v1)
        {
            return new Vector3d(s.Raw - v1.x.Raw, s.Raw - v1.y.Raw, s.Raw - v1.z.Raw);
        }

        /// <summary>
        /// 向量乘法
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator *(Vector3d v1, Vector3d v2)
        {
            return new Vector3d(FixedType.Mul(v1.x.Raw, v2.x.Raw), FixedType.Mul(v1.y.Raw, v2.y.Raw), FixedType.Mul(v1.z.Raw, v2.z.Raw));
        }

        /// <summary>
        /// 向量乘一个数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator *(Vector3d v, REAL s)
        {
            return new Vector3d(FixedType.Mul(v.x.Raw, s.Raw), FixedType.Mul(v.y.Raw, s.Raw), FixedType.Mul(v.z.Raw, s.Raw));
        }

        /// <summary>
        /// 一个数乘向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator *(REAL s, Vector3d v)
        {
            return new Vector3d(FixedType.Mul(v.x.Raw, s.Raw), FixedType.Mul(v.y.Raw, s.Raw), FixedType.Mul(v.z.Raw, s.Raw));
        }

        /// <summary>
        /// 向量相除
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator /(Vector3d v1, Vector3d v2)
        {
            return new Vector3d(FixedType.DivPrecise(v1.x.Raw, v2.x.Raw), FixedType.DivPrecise(v1.y.Raw, v2.y.Raw), FixedType.DivPrecise(v1.z.Raw, v2.z.Raw));
        }

        /// <summary>
        /// 向量除以一个数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator /(Vector3d v, REAL s)
        {
            return new Vector3d(FixedType.DivPrecise(v.x.Raw, s.Raw), FixedType.DivPrecise(v.y.Raw, s.Raw), FixedType.DivPrecise(v.z.Raw, s.Raw));
        }

        /// <summary>
        /// 向量是否相等
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector3d v1, Vector3d v2)
        {
            return (v1.x.Raw == v2.x.Raw && v1.y.Raw == v2.y.Raw && v1.z.Raw == v2.z.Raw);
        }

        /// <summary>
        /// 向量是否不等
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector3d v1, Vector3d v2)
        {
            return (v1.x.Raw != v2.x.Raw || v1.y.Raw != v2.y.Raw || v1.z.Raw != v2.z.Raw);
        }

        public static implicit operator Vector3d(Vector3 v)
        {
            return new Vector3d(v.x, v.y, v.z);
        }

        /// <summary>
        /// 相等
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (!(obj is Vector3d)) return false;
            Vector3d v = (Vector3d)obj;
            return this == v;
        }


        /// <summary>
        /// 相等
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector3d v)
        {
            return this == v;
        }

        /// <summary>
        /// hash编码
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = (hash * 16777619) ^ x.Raw.GetHashCode();
                hash = (hash * 16777619) ^ y.Raw.GetHashCode();
                hash = (hash * 16777619) ^ z.Raw.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// 每一个轴对比
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(Vector3d other)
        {
            if (x != other.x)
                return x < other.x ? -1 : 1;
            else if (y != other.y)
                return y < other.y ? -1 : 1;
            else if (z != other.z)
                return z < other.z ? -1 : 1;
            return 0;
        }

        /// <summary>
        /// 转为字符串
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return string.Format("{0},{1},{2}", x, y, z);
        }

        /// <summary>
        /// 转为字符串
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string f)
        {
            return string.Format("{0},{1},{2}", ((double)x).ToString(f), ((double)y).ToString(f), ((double)z).ToString(f));
        }

        /// <summary>
        /// 从字符串赋值
        /// </summary>
        static public Vector3d FromString(string s)
        {
            Vector3d v = new Vector3d();

            try
            {
                string[] separators = new string[] { "," };
                string[] result = s.Split(separators, StringSplitOptions.None);

                v.x = double.Parse(result[0]);
                v.y = double.Parse(result[1]);
                v.z = double.Parse(result[2]);
            }
            catch { }

            return v;
        }

        /// <summary>
        /// 向量点积
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static REAL Dot(Vector3d v0, Vector3d v1)
        {
            return REAL.FromRaw(FixedType.Mul(v0.x.Raw, v1.x.Raw) + FixedType.Mul(v0.y.Raw, v1.y.Raw) + FixedType.Mul(v0.z.Raw, v1.z.Raw));
        }

        /// <summary>
        /// 向量绝对点积
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static REAL AbsDot(Vector3d v0, Vector3d v1)
        {
            return REAL.Abs(Dot(v0, v1));
        }

        /// <summary>
        /// 向量单位化
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Normalize()
        {
            RawType invLength = FixedType.RSqrtFastest(FixedType.Mul(x.Raw, x.Raw) + FixedType.Mul(y.Raw, y.Raw) + FixedType.Mul(z.Raw, z.Raw));
            x.Raw = FixedType.Mul(x.Raw, invLength);
            y.Raw = FixedType.Mul(y.Raw, invLength);
            z.Raw = FixedType.Mul(z.Raw, invLength);
        }

        /// <summary>
        /// 向量夹角，180度内
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static REAL Angle180(Vector3d a, Vector3d b)
        {
            REAL dp = Vector3d.Dot(a, b);
            REAL m = a.Magnitude * b.Magnitude;
            return DMath.SafeAcos(DMath.SafeDiv(dp, m)) * DMath.Rad2Deg;
        }

        /// <summary>
        /// 向量叉积
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d Cross(Vector3d v0, Vector3d v1)
        {
            return new Vector3d(FixedType.Mul(v0.y.Raw, v1.z.Raw) - FixedType.Mul(v0.z.Raw, v1.y.Raw),
                                FixedType.Mul(v0.z.Raw, v1.x.Raw) - FixedType.Mul(v0.x.Raw, v1.z.Raw),
                                FixedType.Mul(v0.x.Raw, v1.y.Raw) - FixedType.Mul(v0.y.Raw, v1.x.Raw));
        }

        /// <summary>
        /// 两点距离
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static REAL Distance(Vector3d v0, Vector3d v1)
        {
            return DMath.SafeSqrt(SqrDistance(v0, v1));
        }

        /// <summary>
        /// Square distance between two vectors.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static REAL SqrDistance(Vector3d v0, Vector3d v1)
        {
            Vector3d v = v0 - v1;
            return REAL.FromRaw(FixedType.Mul(v.x.Raw, v.x.Raw) + FixedType.Mul(v.y.Raw, v.y.Raw) + FixedType.Mul(v.z.Raw, v.z.Raw));
        }

        /// <summary>
        /// 向量在另一单位向量的投影向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d Project(Vector3d v, Vector3d n)
        {
            return Dot(v, n) * n;
        }

        /// <summary>
        /// 反射向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d Reflect(Vector3d i, Vector3d n)
        {
            return i - 2 * n * Dot(i, n);
        }

        /// <summary>
        /// 折射向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d Refract(Vector3d i, Vector3d n, REAL eta)
        {
            REAL ni = Dot(n, i);
            REAL k = 1.0f - eta * eta * (1.0f - ni * ni);

            return (k >= 0) ? eta * i - (eta * ni + DMath.SafeSqrt(k)) * n : Zero;
        }

        /// <summary>
        /// 标准正交基
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Orthonormal(ref Vector3d a, ref Vector3d b, out Vector3d c)
        {
            a.Normalize();
            c = Cross(a, b);

            if (c.SqrMagnitude == 0)
                throw new ArgumentException("a and b are parallel");

            c.Normalize();
            b = Cross(c, a);
        }

        /// <summary>
        /// 取最小
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d Min(Vector3d v, REAL s)
        {
            return new Vector3d(FixedType.Min(v.x.Raw, s.Raw), FixedType.Min(v.y.Raw, s.Raw), FixedType.Min(v.z.Raw, s.Raw));
        }

        /// <summary>
        /// 取最小
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d Min(Vector3d v0, Vector3d v1)
        {
            return new Vector3d(FixedType.Min(v0.x.Raw, v1.x.Raw), FixedType.Min(v0.y.Raw, v1.y.Raw), FixedType.Min(v0.z.Raw, v1.z.Raw));
        }

        /// <summary>
        /// 取最大
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d Max(Vector3d v, REAL s)
        {
            return new Vector3d(FixedType.Max(v.x.Raw, s.Raw), FixedType.Max(v.y.Raw, s.Raw), FixedType.Max(v.z.Raw, s.Raw));
        }

        /// <summary>
        /// 取最大
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d Max(Vector3d v0, Vector3d v1)
        {
            return new Vector3d(FixedType.Max(v0.x.Raw, v1.x.Raw), FixedType.Max(v0.y.Raw, v1.y.Raw), FixedType.Max(v0.z.Raw, v1.z.Raw));
        }

        /// <summary>
        /// 绝对向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Abs()
        {
            x.Raw = FixedType.Abs(x.Raw);
            y.Raw = FixedType.Abs(y.Raw);
            z.Raw = FixedType.Abs(z.Raw);
        }

        /// <summary>
        /// Clamp
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clamp(REAL min, REAL max)
        {
            x.Raw = FixedType.Clamp(x.Raw, min.Raw, max.Raw);
            y.Raw = FixedType.Clamp(y.Raw, min.Raw, max.Raw);
            z.Raw = FixedType.Clamp(z.Raw, min.Raw, max.Raw);
        }

        /// <summary>
        /// Clamp
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clamp(Vector3d min, Vector3d max)
        {
            new Vector3d(FixedType.Clamp(x.Raw, min.x.Raw, max.x.Raw),
                         FixedType.Clamp(y.Raw, min.y.Raw, max.y.Raw),
                         FixedType.Clamp(z.Raw, min.z.Raw, max.z.Raw));
        }

        /// <summary>
        /// Lerp
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d Lerp(Vector3d from, Vector3d to, REAL t)
        {
            if (t < 0) t = 0;
            if (t > 1.0) t = 1.0;

            if (t == 0) return from;
            if (t == 1.0) return to;

            RawType tr = t.Raw;
            return new Vector3d(
                FixedType.Lerp(from.x.Raw, to.x.Raw, tr),
                FixedType.Lerp(from.y.Raw, to.y.Raw, tr),
                FixedType.Lerp(from.z.Raw, to.z.Raw, tr));
        }

        /// <summary>
        /// Slerp
        /// </summary>
        public static Vector3d Slerp(Vector3d from, Vector3d to, REAL t)
        {
            if (t < 0) t = 0;
            if (t > 1.0f) t = 1.0f;

            if (t == 0) return from;
            if (t == 1.0f) return to;
            if (to.x == from.x && to.y == from.y && to.z == from.z) return to;

            REAL m = from.Magnitude * to.Magnitude;
            if (DMath.IsZero(m)) return Vector3d.Zero;

            REAL theta = REAL.Acos(Dot(from, to) / m);

            if (theta == 0) return to;

            REAL sRawTypeheta = REAL.Sin(theta);
            REAL st1 = REAL.Sin((1.0 - t) * theta) / sRawTypeheta;
            REAL st = REAL.Sin(t * theta) / sRawTypeheta;

            Vector3d v = new Vector3d(FixedType.Mul(from.x.Raw, st1.Raw) + FixedType.Mul(to.x.Raw, st.Raw),
                                      FixedType.Mul(from.y.Raw, st1.Raw) + FixedType.Mul(to.y.Raw, st.Raw),
                                      FixedType.Mul(from.z.Raw, st1.Raw) + FixedType.Mul(to.z.Raw, st.Raw));
            return v;
        }

        /// <summary>
        /// Round舍入
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Round()
        {
            x.Raw = FixedType.Round(x.Raw);
            y.Raw = FixedType.Round(y.Raw);
            z.Raw = FixedType.Round(z.Raw);
        }

    }

}


































