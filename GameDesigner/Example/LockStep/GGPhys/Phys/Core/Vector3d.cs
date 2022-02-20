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
        /// x�ᵥλ����
        /// </summary>
        public readonly static Vector3d UnitX = new Vector3d(REAL.One, REAL.Zero, REAL.Zero);

        /// <summary>
        /// y�ᵥλ����
        /// </summary>
	    public readonly static Vector3d UnitY = new Vector3d(REAL.Zero, REAL.One, REAL.Zero);

        /// <summary>
        /// z�ᵥλ����
        /// </summary>
	    public readonly static Vector3d UnitZ = new Vector3d(REAL.Zero, REAL.Zero, REAL.One);

        /// <summary>
        /// ȫ0����
        /// </summary>
	    public readonly static Vector3d Zero = new Vector3d(REAL.Zero);

        /// <summary>
        /// ȫ1����
        /// </summary>
        public readonly static Vector3d One = new Vector3d(REAL.One);

        /// <summary>
        /// ȫ0.5����
        /// </summary>
        public readonly static Vector3d Half = new Vector3d(REAL.Half);

        /// <summary>
        /// ����������
        /// </summary>
        public readonly static Vector3d PositiveInfinity = new Vector3d(REAL.MaxValue);

        /// <summary>
        /// �������
        /// </summary>
        public readonly static Vector3d NegativeInfinity = new Vector3d(REAL.MinValue);

        /// <summary>
        /// ����һ��ֵ��������
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3d(REAL v)
        {
            this.x.Raw = v.Raw;
            this.y.Raw = v.Raw;
            this.z.Raw = v.Raw;
        }

        /// <summary>
        /// ��������ֵ��������
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3d(REAL x, REAL y, REAL z)
        {
            this.x.Raw = x.Raw;
            this.y.Raw = y.Raw;
            this.z.Raw = z.Raw;
        }

        /// <summary>
        /// ��������
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
        /// �±��д
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
        /// ����ģ
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
        /// ����ƽ������
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
        /// ������λ��
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
        /// ��RawType����
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
        /// �����ӷ�
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator +(Vector3d v1, Vector3d v2)
        {
            return new Vector3d(v1.x.Raw + v2.x.Raw, v1.y.Raw + v2.y.Raw, v1.z.Raw + v2.z.Raw);
        }

        /// <summary>
        /// ������һ����
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator +(Vector3d v1, REAL s)
        {
            return new Vector3d(v1.x.Raw + s.Raw, v1.y.Raw + s.Raw, v1.z.Raw + s.Raw);
        }

        /// <summary>
        /// ��������
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator +(REAL s, Vector3d v1)
        {
            return new Vector3d(v1.x.Raw + s.Raw, v1.y.Raw + s.Raw, v1.z.Raw + s.Raw);
        }

        /// <summary>
        /// ����ȡ��
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator -(Vector3d v)
        {
            return new Vector3d(-v.x.Raw, -v.y.Raw, -v.z.Raw);
        }

        /// <summary>
        /// ��������
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator -(Vector3d v1, Vector3d v2)
        {
            return new Vector3d(v1.x.Raw - v2.x.Raw, v1.y.Raw - v2.y.Raw, v1.z.Raw - v2.z.Raw);
        }

        /// <summary>
        /// ������һ����
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator -(Vector3d v1, REAL s)
        {
            return new Vector3d(v1.x.Raw - s.Raw, v1.y.Raw - s.Raw, v1.z.Raw - s.Raw);
        }

        /// <summary>
        /// һ����������
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator -(REAL s, Vector3d v1)
        {
            return new Vector3d(s.Raw - v1.x.Raw, s.Raw - v1.y.Raw, s.Raw - v1.z.Raw);
        }

        /// <summary>
        /// �����˷�
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator *(Vector3d v1, Vector3d v2)
        {
            return new Vector3d(FixedType.Mul(v1.x.Raw, v2.x.Raw), FixedType.Mul(v1.y.Raw, v2.y.Raw), FixedType.Mul(v1.z.Raw, v2.z.Raw));
        }

        /// <summary>
        /// ������һ����
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator *(Vector3d v, REAL s)
        {
            return new Vector3d(FixedType.Mul(v.x.Raw, s.Raw), FixedType.Mul(v.y.Raw, s.Raw), FixedType.Mul(v.z.Raw, s.Raw));
        }

        /// <summary>
        /// һ����������
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator *(REAL s, Vector3d v)
        {
            return new Vector3d(FixedType.Mul(v.x.Raw, s.Raw), FixedType.Mul(v.y.Raw, s.Raw), FixedType.Mul(v.z.Raw, s.Raw));
        }

        /// <summary>
        /// �������
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator /(Vector3d v1, Vector3d v2)
        {
            return new Vector3d(FixedType.DivPrecise(v1.x.Raw, v2.x.Raw), FixedType.DivPrecise(v1.y.Raw, v2.y.Raw), FixedType.DivPrecise(v1.z.Raw, v2.z.Raw));
        }

        /// <summary>
        /// ��������һ����
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d operator /(Vector3d v, REAL s)
        {
            return new Vector3d(FixedType.DivPrecise(v.x.Raw, s.Raw), FixedType.DivPrecise(v.y.Raw, s.Raw), FixedType.DivPrecise(v.z.Raw, s.Raw));
        }

        /// <summary>
        /// �����Ƿ����
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector3d v1, Vector3d v2)
        {
            return (v1.x.Raw == v2.x.Raw && v1.y.Raw == v2.y.Raw && v1.z.Raw == v2.z.Raw);
        }

        /// <summary>
        /// �����Ƿ񲻵�
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
        /// ���
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (!(obj is Vector3d)) return false;
            Vector3d v = (Vector3d)obj;
            return this == v;
        }


        /// <summary>
        /// ���
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector3d v)
        {
            return this == v;
        }

        /// <summary>
        /// hash����
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
        /// ÿһ����Ա�
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
        /// תΪ�ַ���
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return string.Format("{0},{1},{2}", x, y, z);
        }

        /// <summary>
        /// תΪ�ַ���
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string f)
        {
            return string.Format("{0},{1},{2}", ((double)x).ToString(f), ((double)y).ToString(f), ((double)z).ToString(f));
        }

        /// <summary>
        /// ���ַ�����ֵ
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
        /// �������
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static REAL Dot(Vector3d v0, Vector3d v1)
        {
            return REAL.FromRaw(FixedType.Mul(v0.x.Raw, v1.x.Raw) + FixedType.Mul(v0.y.Raw, v1.y.Raw) + FixedType.Mul(v0.z.Raw, v1.z.Raw));
        }

        /// <summary>
        /// �������Ե��
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static REAL AbsDot(Vector3d v0, Vector3d v1)
        {
            return REAL.Abs(Dot(v0, v1));
        }

        /// <summary>
        /// ������λ��
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
        /// �����нǣ�180����
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static REAL Angle180(Vector3d a, Vector3d b)
        {
            REAL dp = Vector3d.Dot(a, b);
            REAL m = a.Magnitude * b.Magnitude;
            return DMath.SafeAcos(DMath.SafeDiv(dp, m)) * DMath.Rad2Deg;
        }

        /// <summary>
        /// �������
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d Cross(Vector3d v0, Vector3d v1)
        {
            return new Vector3d(FixedType.Mul(v0.y.Raw, v1.z.Raw) - FixedType.Mul(v0.z.Raw, v1.y.Raw),
                                FixedType.Mul(v0.z.Raw, v1.x.Raw) - FixedType.Mul(v0.x.Raw, v1.z.Raw),
                                FixedType.Mul(v0.x.Raw, v1.y.Raw) - FixedType.Mul(v0.y.Raw, v1.x.Raw));
        }

        /// <summary>
        /// �������
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
        /// ��������һ��λ������ͶӰ����
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d Project(Vector3d v, Vector3d n)
        {
            return Dot(v, n) * n;
        }

        /// <summary>
        /// ��������
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d Reflect(Vector3d i, Vector3d n)
        {
            return i - 2 * n * Dot(i, n);
        }

        /// <summary>
        /// ��������
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d Refract(Vector3d i, Vector3d n, REAL eta)
        {
            REAL ni = Dot(n, i);
            REAL k = 1.0f - eta * eta * (1.0f - ni * ni);

            return (k >= 0) ? eta * i - (eta * ni + DMath.SafeSqrt(k)) * n : Zero;
        }

        /// <summary>
        /// ��׼������
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
        /// ȡ��С
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d Min(Vector3d v, REAL s)
        {
            return new Vector3d(FixedType.Min(v.x.Raw, s.Raw), FixedType.Min(v.y.Raw, s.Raw), FixedType.Min(v.z.Raw, s.Raw));
        }

        /// <summary>
        /// ȡ��С
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d Min(Vector3d v0, Vector3d v1)
        {
            return new Vector3d(FixedType.Min(v0.x.Raw, v1.x.Raw), FixedType.Min(v0.y.Raw, v1.y.Raw), FixedType.Min(v0.z.Raw, v1.z.Raw));
        }

        /// <summary>
        /// ȡ���
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d Max(Vector3d v, REAL s)
        {
            return new Vector3d(FixedType.Max(v.x.Raw, s.Raw), FixedType.Max(v.y.Raw, s.Raw), FixedType.Max(v.z.Raw, s.Raw));
        }

        /// <summary>
        /// ȡ���
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3d Max(Vector3d v0, Vector3d v1)
        {
            return new Vector3d(FixedType.Max(v0.x.Raw, v1.x.Raw), FixedType.Max(v0.y.Raw, v1.y.Raw), FixedType.Max(v0.z.Raw, v1.z.Raw));
        }

        /// <summary>
        /// ��������
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
        /// Round����
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


































