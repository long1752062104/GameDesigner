using System;
using System.Runtime.CompilerServices;
using FixPointCS;

namespace FixMath
{
#if !GGPHYS_FIXPOINT32
    /// <summary>
    /// Signed 32.32 fixed point value struct.
    /// </summary>
    [Serializable]
    public struct FP : IComparable<FP>, IEquatable<FP>
    {
        // Constants
        public static FP Neg1 { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed64.Neg1); } }
        public static FP Zero { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed64.Zero); } }
        public static FP Half { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed64.Half); } }
        public static FP One { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed64.One); } }
        public static FP Two { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed64.Two); } }
        public static FP Pi { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed64.Pi); } }
        public static FP Pi2 { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed64.Pi2); } }
        public static FP PiHalf { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed64.PiHalf); } }
        public static FP E { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed64.E); } }

        public static FP MinValue { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed64.MinValue); } }
        public static FP MaxValue { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed64.MaxValue); } }

        // Raw fixed point value
        public long Raw;

        // Constructors
        public FP(int v) { Raw = Fixed64.FromInt(v); }
        public FP(float v) { Raw = Fixed64.FromFloat(v); }
        public FP(double v) { Raw = Fixed64.FromDouble(v); }
        public FP(long v) { Raw = v; }
        //public FP(F32 v) { Raw = (long)v.Raw << 16; }

        // Conversions
        public static int FloorToInt(FP a) { return Fixed64.FloorToInt(a.Raw); }
        public static int CeilToInt(FP a) { return Fixed64.CeilToInt(a.Raw); }
        public static int RoundToInt(FP a) { return Fixed64.RoundToInt(a.Raw); }
        public float Float { get { return Fixed64.ToFloat(Raw); } }
        public double Double { get { return Fixed64.ToDouble(Raw); } }
        //public F32 F32 { get { return F32.FromRaw((int)(Raw >> 16)); } }

        // Creates the fixed point number that's a divided by b.
        public static FP Ratio(int a, int b) { return FP.FromRaw(((long)a << 32) / b); }
        // Creates the fixed point number that's a divided by 10.
        public static FP Ratio10(int a) { return FP.FromRaw(((long)a << 32) / 10); }
        // Creates the fixed point number that's a divided by 100.
        public static FP Ratio100(int a) { return FP.FromRaw(((long)a << 32) / 100); }
        // Creates the fixed point number that's a divided by 1000.
        public static FP Ratio1000(int a) { return FP.FromRaw(((long)a << 32) / 1000); }

        // Operators
        //public static explicit operator FP(int i) { return new FP(i); }
        //public static explicit operator FP(float f) { return new FP(f); }
        //public static explicit operator FP(double d) { return new FP(d); }
        //public static explicit operator FP(F32 f) { return new FP(f); }

        //public static explicit operator float(FP f) { return f.Float; }
        //public static explicit operator double(FP d) { return d.Double; }

        public static implicit operator FP(int i) { return new FP(i); }
        public static implicit operator FP(float f) { return new FP(f); }
        public static implicit operator FP(double d) { return new FP(d); }
        public static implicit operator FP(long d) { return new FP(d); }
        //public static implicit operator FP(F32 f) { return new FP(f); }

        public static implicit operator float(FP f) { return f.Float; }
        public static implicit operator double(FP d) { return d.Double; }

        public static FP operator -(FP v1) { return FromRaw(-v1.Raw); }
        public static FP operator +(FP v1, FP v2) { return FromRaw(v1.Raw + v2.Raw); }
        public static FP operator -(FP v1, FP v2) { return FromRaw(v1.Raw - v2.Raw); }
        public static FP operator *(FP v1, FP v2) { return FromRaw(Fixed64.Mul(v1.Raw, v2.Raw)); }
        public static FP operator /(FP v1, FP v2) { return FromRaw(Fixed64.DivPrecise(v1.Raw, v2.Raw)); }
        public static FP operator %(FP v1, FP v2) { return FromRaw(Fixed64.Mod(v1.Raw, v2.Raw)); }

        public static FP operator +(FP v1, int v2) { return FromRaw(v1.Raw + Fixed64.FromInt(v2)); }
        public static FP operator +(int v1, FP v2) { return FromRaw(Fixed64.FromInt(v1) + v2.Raw); }
        public static FP operator -(FP v1, int v2) { return FromRaw(v1.Raw - Fixed64.FromInt(v2)); }
        public static FP operator -(int v1, FP v2) { return FromRaw(Fixed64.FromInt(v1) - v2.Raw); }
        public static FP operator *(FP v1, int v2) { return FromRaw(v1.Raw * (long)v2); }
        public static FP operator *(int v1, FP v2) { return FromRaw((long)v1 * v2.Raw); }
        public static FP operator /(FP v1, int v2) { return FromRaw(v1.Raw / (long)v2); }
        public static FP operator /(int v1, FP v2) { return FromRaw(Fixed64.DivPrecise(Fixed64.FromInt(v1), v2.Raw)); }
        public static FP operator %(FP v1, int v2) { return FromRaw(Fixed64.Mod(v1.Raw, Fixed64.FromInt(v2))); }
        public static FP operator %(int v1, FP v2) { return FromRaw(Fixed64.Mod(Fixed64.FromInt(v1), v2.Raw)); }

        public static FP operator ++(FP v1) { return FromRaw(v1.Raw + Fixed64.One); }
        public static FP operator --(FP v1) { return FromRaw(v1.Raw - Fixed64.One); }

        public static bool operator ==(FP v1, FP v2) { return v1.Raw == v2.Raw; }
        public static bool operator !=(FP v1, FP v2) { return v1.Raw != v2.Raw; }
        public static bool operator <(FP v1, FP v2) { return v1.Raw < v2.Raw; }
        public static bool operator <=(FP v1, FP v2) { return v1.Raw <= v2.Raw; }
        public static bool operator >(FP v1, FP v2) { return v1.Raw > v2.Raw; }
        public static bool operator >=(FP v1, FP v2) { return v1.Raw >= v2.Raw; }

        public static bool operator ==(int v1, FP v2) { return Fixed64.FromInt(v1) == v2.Raw; }
        public static bool operator ==(FP v1, int v2) { return v1.Raw == Fixed64.FromInt(v2); }
        public static bool operator !=(int v1, FP v2) { return Fixed64.FromInt(v1) != v2.Raw; }
        public static bool operator !=(FP v1, int v2) { return v1.Raw != Fixed64.FromInt(v2); }
        public static bool operator <(int v1, FP v2) { return Fixed64.FromInt(v1) < v2.Raw; }
        public static bool operator <(FP v1, int v2) { return v1.Raw < Fixed64.FromInt(v2); }
        public static bool operator <=(int v1, FP v2) { return Fixed64.FromInt(v1) <= v2.Raw; }
        public static bool operator <=(FP v1, int v2) { return v1.Raw <= Fixed64.FromInt(v2); }
        public static bool operator >(int v1, FP v2) { return Fixed64.FromInt(v1) > v2.Raw; }
        public static bool operator >(FP v1, int v2) { return v1.Raw > Fixed64.FromInt(v2); }
        public static bool operator >=(int v1, FP v2) { return Fixed64.FromInt(v1) >= v2.Raw; }
        public static bool operator >=(FP v1, int v2) { return v1.Raw >= Fixed64.FromInt(v2); }

        //public static bool operator ==(F32 a, FP b) { return FP.FromF32(a) == b; }
        //public static bool operator ==(FP a, F32 b) { return a == FP.FromF32(b); }
        //public static bool operator !=(F32 a, FP b) { return FP.FromF32(a) != b; }
        //public static bool operator !=(FP a, F32 b) { return a != FP.FromF32(b); }
        //public static bool operator <(F32 a, FP b) { return FP.FromF32(a) < b; }
        //public static bool operator <(FP a, F32 b) { return a < FP.FromF32(b); }
        //public static bool operator <=(F32 a, FP b) { return FP.FromF32(a) <= b; }
        //public static bool operator <=(FP a, F32 b) { return a <= FP.FromF32(b); }
        //public static bool operator >(F32 a, FP b) { return FP.FromF32(a) > b; }
        //public static bool operator >(FP a, F32 b) { return a > FP.FromF32(b); }
        //public static bool operator >=(F32 a, FP b) { return FP.FromF32(a) >= b; }
        //public static bool operator >=(FP a, F32 b) { return a >= FP.FromF32(b); }

        public static FP RadToDeg(FP a) { return FromRaw(Fixed64.Mul(a.Raw, 246083499198)); } // 180 / FP.Pi
        public static FP DegToRad(FP a) { return FromRaw(Fixed64.Mul(a.Raw, 74961320)); }     // FP.Pi / 180

        public static FP Div2(FP a) { return FromRaw(a.Raw >> 1); }
        public static FP Abs(FP a) { return FromRaw(Fixed64.Abs(a.Raw)); }
        public static FP Nabs(FP a) { return FromRaw(Fixed64.Nabs(a.Raw)); }
        public static int Sign(FP a) { return Fixed64.Sign(a.Raw); }
        public static FP Ceil(FP a) { return FromRaw(Fixed64.Ceil(a.Raw)); }
        public static FP Floor(FP a) { return FromRaw(Fixed64.Floor(a.Raw)); }
        public static FP Round(FP a) { return FromRaw(Fixed64.Round(a.Raw)); }
        public static FP Fract(FP a) { return FromRaw(Fixed64.Fract(a.Raw)); }
        public static FP Div(FP a, FP b) { return FromRaw(Fixed64.Div(a.Raw, b.Raw)); }
        public static FP DivFast(FP a, FP b) { return FromRaw(Fixed64.DivFast(a.Raw, b.Raw)); }
        public static FP DivFastest(FP a, FP b) { return FromRaw(Fixed64.DivFastest(a.Raw, b.Raw)); }
        public static FP SqrtPrecise(FP a) { return FromRaw(Fixed64.SqrtPrecise(a.Raw)); }
        public static FP Sqrt(FP a) { return FromRaw(Fixed64.SqrtFastest(a.Raw)); }
        public static FP SqrtFast(FP a) { return FromRaw(Fixed64.SqrtFastest(a.Raw)); }
        public static FP SqrtFastest(FP a) { return FromRaw(Fixed64.SqrtFastest(a.Raw)); }
        public static FP RSqrt(FP a) { return FromRaw(Fixed64.RSqrt(a.Raw)); }
        public static FP RSqrtFast(FP a) { return FromRaw(Fixed64.RSqrtFast(a.Raw)); }
        public static FP RSqrtFastest(FP a) { return FromRaw(Fixed64.RSqrtFastest(a.Raw)); }
        public static FP Rcp(FP a) { return FromRaw(Fixed64.Rcp(a.Raw)); }
        public static FP RcpFast(FP a) { return FromRaw(Fixed64.RcpFast(a.Raw)); }
        public static FP RcpFastest(FP a) { return FromRaw(Fixed64.RcpFastest(a.Raw)); }
        public static FP Exp(FP a) { return FromRaw(Fixed64.Exp(a.Raw)); }
        public static FP ExpFast(FP a) { return FromRaw(Fixed64.ExpFast(a.Raw)); }
        public static FP ExpFastest(FP a) { return FromRaw(Fixed64.ExpFastest(a.Raw)); }
        public static FP Exp2(FP a) { return FromRaw(Fixed64.Exp2(a.Raw)); }
        public static FP Exp2Fast(FP a) { return FromRaw(Fixed64.Exp2Fast(a.Raw)); }
        public static FP Exp2Fastest(FP a) { return FromRaw(Fixed64.Exp2Fastest(a.Raw)); }
        public static FP Log(FP a) { return FromRaw(Fixed64.Log(a.Raw)); }
        public static FP LogFast(FP a) { return FromRaw(Fixed64.LogFast(a.Raw)); }
        public static FP LogFastest(FP a) { return FromRaw(Fixed64.LogFastest(a.Raw)); }
        public static FP Log2(FP a) { return FromRaw(Fixed64.Log2(a.Raw)); }
        public static FP Log2Fast(FP a) { return FromRaw(Fixed64.Log2Fast(a.Raw)); }
        public static FP Log2Fastest(FP a) { return FromRaw(Fixed64.Log2Fastest(a.Raw)); }

        public static FP Sin(FP a) { return FromRaw(Fixed64.SinFastest(a.Raw)); }
        public static FP SinFast(FP a) { return FromRaw(Fixed64.SinFastest(a.Raw)); }
        public static FP SinFastest(FP a) { return FromRaw(Fixed64.SinFastest(a.Raw)); }
        public static FP Cos(FP a) { return FromRaw(Fixed64.CosFastest(a.Raw)); }
        public static FP CosFast(FP a) { return FromRaw(Fixed64.CosFastest(a.Raw)); }
        public static FP CosFastest(FP a) { return FromRaw(Fixed64.CosFastest(a.Raw)); }
        public static FP Tan(FP a) { return FromRaw(Fixed64.Tan(a.Raw)); }
        public static FP TanFast(FP a) { return FromRaw(Fixed64.TanFast(a.Raw)); }
        public static FP TanFastest(FP a) { return FromRaw(Fixed64.TanFastest(a.Raw)); }
        public static FP Asin(FP a) { return FromRaw(Fixed64.AsinFastest(a.Raw)); }
        public static FP AsinFast(FP a) { return FromRaw(Fixed64.AsinFastest(a.Raw)); }
        public static FP AsinFastest(FP a) { return FromRaw(Fixed64.AsinFastest(a.Raw)); }
        public static FP Acos(FP a) { return FromRaw(Fixed64.AcosFastest(a.Raw)); }
        public static FP AcosFast(FP a) { return FromRaw(Fixed64.AcosFastest(a.Raw)); }
        public static FP AcosFastest(FP a) { return FromRaw(Fixed64.AcosFastest(a.Raw)); }
        public static FP Atan(FP a) { return FromRaw(Fixed64.Atan(a.Raw)); }
        public static FP AtanFast(FP a) { return FromRaw(Fixed64.AtanFast(a.Raw)); }
        public static FP AtanFastest(FP a) { return FromRaw(Fixed64.AtanFastest(a.Raw)); }
        public static FP Atan2(FP y, FP x) { return FromRaw(Fixed64.Atan2(y.Raw, x.Raw)); }
        public static FP Atan2Fast(FP y, FP x) { return FromRaw(Fixed64.Atan2Fast(y.Raw, x.Raw)); }
        public static FP Atan2Fastest(FP y, FP x) { return FromRaw(Fixed64.Atan2Fastest(y.Raw, x.Raw)); }
        public static FP Pow(FP a, FP b) { return FromRaw(Fixed64.PowFastest(a.Raw, b.Raw)); }
        public static FP PowFast(FP a, FP b) { return FromRaw(Fixed64.PowFastest(a.Raw, b.Raw)); }
        public static FP PowFastest(FP a, FP b) { return FromRaw(Fixed64.PowFastest(a.Raw, b.Raw)); }

        public static FP Min(FP a, FP b) { return FromRaw(Fixed64.Min(a.Raw, b.Raw)); }
        public static FP Max(FP a, FP b) { return FromRaw(Fixed64.Max(a.Raw, b.Raw)); }
        public static FP Clamp(FP a, FP min, FP max) { return FromRaw(Fixed64.Clamp(a.Raw, min.Raw, max.Raw)); }

        public static FP Lerp(FP a, FP b, FP t)
        {
            long tb = t.Raw;
            long ta = Fixed64.One - tb;
            return FromRaw(Fixed64.Mul(a.Raw, ta) + Fixed64.Mul(b.Raw, tb));
        }

        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static FP FromRaw(long raw)
        {
            FP r;
            r.Raw = raw;
            return r;
        }

        public static FP FromInt(int v) { return new FP(v); }
        public static FP FromFloat(float v) { return new FP(v); }
        public static FP FromDouble(double v) { return new FP(v); }
        //public static FP FromF32(F32 v) { return new FP(v); }

        public bool Equals(FP other)
        {
            return (Raw == other.Raw);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FP))
                return false;
            return ((FP)obj).Raw == Raw;
        }

        public int CompareTo(FP other)
        {
            if (Raw < other.Raw) return -1;
            if (Raw > other.Raw) return +1;
            return 0;
        }

        public override string ToString()
        {
            return Fixed64.ToString(Raw);
        }

        public override int GetHashCode()
        {
            return (int)Raw | (int)(Raw >> 32);
        }
    }
#endif
}
