using System;
using System.Runtime.CompilerServices;
using FixPointCS;

namespace FixMath
{
#if GGPHYS_FIXPOINT32
    /// <summary>
    /// Signed 16.16 fixed point value struct.
    /// </summary>
    [Serializable]
    public struct FP : IComparable<FP>, IEquatable<FP>
    {
        // Constants
        public static FP Neg1 { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed32.Neg1); } }
        public static FP Zero { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed32.Zero); } }
        public static FP Half { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed32.Half); } }
        public static FP One { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed32.One); } }
        public static FP Two { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed32.Two); } }
        public static FP Pi { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed32.Pi); } }
        public static FP Pi2 { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed32.Pi2); } }
        public static FP PiHalf { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed32.PiHalf); } }
        public static FP E { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed32.E); } }

        public static FP MinValue { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed32.MinValue); } }
        public static FP MaxValue { [MethodImpl(FixedUtil.AggressiveInlining)] get { return FromRaw(Fixed32.MaxValue); } }

        // Raw fixed point value
        public int Raw;

        // Constructors
        public FP(int v) { Raw = Fixed32.FromInt(v); }
        public FP(float v) { Raw = Fixed32.FromFloat(v); }
        public FP(double v) { Raw = Fixed32.FromDouble(v); }
        public FP(FP v) { Raw = (int)(v.Raw >> 16); }

        // Conversions
        public static int FloorToInt(FP a) { return Fixed32.FloorToInt(a.Raw); }
        public static int CeilToInt(FP a) { return Fixed32.CeilToInt(a.Raw); }
        public static int RoundToInt(FP a) { return Fixed32.RoundToInt(a.Raw); }
        public float Float { get { return Fixed32.ToFloat(Raw); } }
        public double Double { get { return Fixed32.ToDouble(Raw); } }

        // Creates the fixed point number that's a divided by b.
        public static FP Ratio(int a, int b) { return FP.FromRaw((int)(((long)a << 16) / b)); }
        // Creates the fixed point number that's a divided by 10.
        public static FP Ratio10(int a) { return FP.FromRaw((int)(((long)a << 16) / 10)); }
        // Creates the fixed point number that's a divided by 100.
        public static FP Ratio100(int a) { return FP.FromRaw((int)(((long)a << 16) / 100)); }
        // Creates the fixed point number that's a divided by 1000.
        public static FP Ratio1000(int a) { return FP.FromRaw((int)(((long)a << 16) / 1000)); }

        public static implicit operator FP(int i) { return new FP(i); }
        public static implicit operator FP(float f) { return new FP(f); }
        public static implicit operator FP(double d) { return new FP(d); }
        //public static implicit operator FP(FP f) { return new FP(f); }

        public static implicit operator float(FP f) { return f.Float; }
        public static implicit operator double(FP d) { return d.Double; }

        // Operators
        public static FP operator -(FP v1) { return FromRaw(-v1.Raw); }

        //public static FP operator +(FP v1, FP v2) { FP r; r.raw = v1.raw + v2.raw; return r; }
        public static FP operator +(FP v1, FP v2) { return FromRaw(v1.Raw + v2.Raw); }
        public static FP operator -(FP v1, FP v2) { return FromRaw(v1.Raw - v2.Raw); }
        public static FP operator *(FP v1, FP v2) { return FromRaw(Fixed32.Mul(v1.Raw, v2.Raw)); }
        public static FP operator /(FP v1, FP v2) { return FromRaw(Fixed32.DivPrecise(v1.Raw, v2.Raw)); }
        public static FP operator %(FP v1, FP v2) { return FromRaw(Fixed32.Mod(v1.Raw, v2.Raw)); }

        public static FP operator +(FP v1, int v2) { return FromRaw(v1.Raw + Fixed32.FromInt(v2)); }
        public static FP operator +(int v1, FP v2) { return FromRaw(Fixed32.FromInt(v1) + v2.Raw); }
        public static FP operator -(FP v1, int v2) { return FromRaw(v1.Raw - Fixed32.FromInt(v2)); }
        public static FP operator -(int v1, FP v2) { return FromRaw(Fixed32.FromInt(v1) - v2.Raw); }
        public static FP operator *(FP v1, int v2) { return FromRaw(v1.Raw * (int)v2); }
        public static FP operator *(int v1, FP v2) { return FromRaw((int)v1 * v2.Raw); }
        public static FP operator /(FP v1, int v2) { return FromRaw(v1.Raw / (int)v2); }
        public static FP operator /(int v1, FP v2) { return FromRaw(Fixed32.DivPrecise(Fixed32.FromInt(v1), v2.Raw)); }
        public static FP operator %(FP v1, int v2) { return FromRaw(Fixed32.Mod(v1.Raw, Fixed32.FromInt(v2))); }
        public static FP operator %(int v1, FP v2) { return FromRaw(Fixed32.Mod(Fixed32.FromInt(v1), v2.Raw)); }

        public static FP operator ++(FP v1) { return FromRaw(v1.Raw + Fixed32.One); }
        public static FP operator --(FP v1) { return FromRaw(v1.Raw - Fixed32.One); }

        public static bool operator ==(FP v1, FP v2) { return v1.Raw == v2.Raw; }
        public static bool operator !=(FP v1, FP v2) { return v1.Raw != v2.Raw; }
        public static bool operator <(FP v1, FP v2) { return v1.Raw < v2.Raw; }
        public static bool operator <=(FP v1, FP v2) { return v1.Raw <= v2.Raw; }
        public static bool operator >(FP v1, FP v2) { return v1.Raw > v2.Raw; }
        public static bool operator >=(FP v1, FP v2) { return v1.Raw >= v2.Raw; }

        public static bool operator ==(int v1, FP v2) { return Fixed32.FromInt(v1) == v2.Raw; }
        public static bool operator ==(FP v1, int v2) { return v1.Raw == Fixed32.FromInt(v2); }
        public static bool operator !=(int v1, FP v2) { return Fixed32.FromInt(v1) != v2.Raw; }
        public static bool operator !=(FP v1, int v2) { return v1.Raw != Fixed32.FromInt(v2); }
        public static bool operator <(int v1, FP v2) { return Fixed32.FromInt(v1) < v2.Raw; }
        public static bool operator <(FP v1, int v2) { return v1.Raw < Fixed32.FromInt(v2); }
        public static bool operator <=(int v1, FP v2) { return Fixed32.FromInt(v1) <= v2.Raw; }
        public static bool operator <=(FP v1, int v2) { return v1.Raw <= Fixed32.FromInt(v2); }
        public static bool operator >(int v1, FP v2) { return Fixed32.FromInt(v1) > v2.Raw; }
        public static bool operator >(FP v1, int v2) { return v1.Raw > Fixed32.FromInt(v2); }
        public static bool operator >=(int v1, FP v2) { return Fixed32.FromInt(v1) >= v2.Raw; }
        public static bool operator >=(FP v1, int v2) { return v1.Raw >= Fixed32.FromInt(v2); }

        public static FP RadToDeg(FP a) { return FromRaw(Fixed32.Mul(a.Raw, 3754943)); }  // 180 / FP.Pi
        public static FP DegToRad(FP a) { return FromRaw(Fixed32.Mul(a.Raw, 1143)); }     // FP.Pi / 180

        public static FP Div2(FP a) { return FromRaw(a.Raw >> 1); }
        public static FP Abs(FP a) { return FromRaw(Fixed32.Abs(a.Raw)); }
        public static FP Nabs(FP a) { return FromRaw(Fixed32.Nabs(a.Raw)); }
        public static int Sign(FP a) { return Fixed32.Sign(a.Raw); }
        public static FP Ceil(FP a) { return FromRaw(Fixed32.Ceil(a.Raw)); }
        public static FP Floor(FP a) { return FromRaw(Fixed32.Floor(a.Raw)); }
        public static FP Round(FP a) { return FromRaw(Fixed32.Round(a.Raw)); }
        public static FP Fract(FP a) { return FromRaw(Fixed32.Fract(a.Raw)); }
        public static FP Div(FP a, FP b) { return FromRaw(Fixed32.Div(a.Raw, b.Raw)); }
        public static FP DivFast(FP a, FP b) { return FromRaw(Fixed32.DivFast(a.Raw, b.Raw)); }
        public static FP DivFastest(FP a, FP b) { return FromRaw(Fixed32.DivFastest(a.Raw, b.Raw)); }
        public static FP SqrtPrecise(FP a) { return FromRaw(Fixed32.SqrtPrecise(a.Raw)); }
        public static FP Sqrt(FP a) { return FromRaw(Fixed32.Sqrt(a.Raw)); }
        public static FP SqrtFast(FP a) { return FromRaw(Fixed32.SqrtFast(a.Raw)); }
        public static FP SqrtFastest(FP a) { return FromRaw(Fixed32.SqrtFastest(a.Raw)); }
        public static FP RSqrt(FP a) { return FromRaw(Fixed32.RSqrt(a.Raw)); }
        public static FP RSqrtFast(FP a) { return FromRaw(Fixed32.RSqrtFast(a.Raw)); }
        public static FP RSqrtFastest(FP a) { return FromRaw(Fixed32.RSqrtFastest(a.Raw)); }
        public static FP Rcp(FP a) { return FromRaw(Fixed32.Rcp(a.Raw)); }
        public static FP RcpFast(FP a) { return FromRaw(Fixed32.RcpFast(a.Raw)); }
        public static FP RcpFastest(FP a) { return FromRaw(Fixed32.RcpFastest(a.Raw)); }
        public static FP Exp(FP a) { return FromRaw(Fixed32.Exp(a.Raw)); }
        public static FP ExpFast(FP a) { return FromRaw(Fixed32.ExpFast(a.Raw)); }
        public static FP ExpFastest(FP a) { return FromRaw(Fixed32.ExpFastest(a.Raw)); }
        public static FP Exp2(FP a) { return FromRaw(Fixed32.Exp2(a.Raw)); }
        public static FP Exp2Fast(FP a) { return FromRaw(Fixed32.Exp2Fast(a.Raw)); }
        public static FP Exp2Fastest(FP a) { return FromRaw(Fixed32.Exp2Fastest(a.Raw)); }
        public static FP Log(FP a) { return FromRaw(Fixed32.Log(a.Raw)); }
        public static FP LogFast(FP a) { return FromRaw(Fixed32.LogFast(a.Raw)); }
        public static FP LogFastest(FP a) { return FromRaw(Fixed32.LogFastest(a.Raw)); }
        public static FP Log2(FP a) { return FromRaw(Fixed32.Log2(a.Raw)); }
        public static FP Log2Fast(FP a) { return FromRaw(Fixed32.Log2Fast(a.Raw)); }
        public static FP Log2Fastest(FP a) { return FromRaw(Fixed32.Log2Fastest(a.Raw)); }

        public static FP Sin(FP a) { return FromRaw(Fixed32.Sin(a.Raw)); }
        public static FP SinFast(FP a) { return FromRaw(Fixed32.SinFast(a.Raw)); }
        public static FP SinFastest(FP a) { return FromRaw(Fixed32.SinFastest(a.Raw)); }
        public static FP Cos(FP a) { return FromRaw(Fixed32.Cos(a.Raw)); }
        public static FP CosFast(FP a) { return FromRaw(Fixed32.CosFast(a.Raw)); }
        public static FP CosFastest(FP a) { return FromRaw(Fixed32.CosFastest(a.Raw)); }
        public static FP Tan(FP a) { return FromRaw(Fixed32.Tan(a.Raw)); }
        public static FP TanFast(FP a) { return FromRaw(Fixed32.TanFast(a.Raw)); }
        public static FP TanFastest(FP a) { return FromRaw(Fixed32.TanFastest(a.Raw)); }
        public static FP Asin(FP a) { return FromRaw(Fixed32.Asin(a.Raw)); }
        public static FP AsinFast(FP a) { return FromRaw(Fixed32.AsinFast(a.Raw)); }
        public static FP AsinFastest(FP a) { return FromRaw(Fixed32.AsinFastest(a.Raw)); }
        public static FP Acos(FP a) { return FromRaw(Fixed32.Acos(a.Raw)); }
        public static FP AcosFast(FP a) { return FromRaw(Fixed32.AcosFast(a.Raw)); }
        public static FP AcosFastest(FP a) { return FromRaw(Fixed32.AcosFastest(a.Raw)); }
        public static FP Atan(FP a) { return FromRaw(Fixed32.Atan(a.Raw)); }
        public static FP AtanFast(FP a) { return FromRaw(Fixed32.AtanFast(a.Raw)); }
        public static FP AtanFastest(FP a) { return FromRaw(Fixed32.AtanFastest(a.Raw)); }
        public static FP Atan2(FP y, FP x) { return FromRaw(Fixed32.Atan2(y.Raw, x.Raw)); }
        public static FP Atan2Fast(FP y, FP x) { return FromRaw(Fixed32.Atan2Fast(y.Raw, x.Raw)); }
        public static FP Atan2Fastest(FP y, FP x) { return FromRaw(Fixed32.Atan2Fastest(y.Raw, x.Raw)); }
        public static FP Pow(FP a, FP b) { return FromRaw(Fixed32.Pow(a.Raw, b.Raw)); }
        public static FP PowFast(FP a, FP b) { return FromRaw(Fixed32.PowFast(a.Raw, b.Raw)); }
        public static FP PowFastest(FP a, FP b) { return FromRaw(Fixed32.PowFastest(a.Raw, b.Raw)); }

        public static FP Min(FP a, FP b) { return FromRaw(Fixed32.Min(a.Raw, b.Raw)); }
        public static FP Max(FP a, FP b) { return FromRaw(Fixed32.Max(a.Raw, b.Raw)); }
        public static FP Clamp(FP a, FP min, FP max) { return FromRaw(Fixed32.Clamp(a.Raw, min.Raw, max.Raw)); }

        public static FP Lerp(FP a, FP b, FP t)
        {
            int tb = t.Raw;
            int ta = Fixed32.One - tb;
            return FromRaw(Fixed32.Mul(a.Raw, ta) + Fixed32.Mul(b.Raw, tb));
        }

        [MethodImpl(FixedUtil.AggressiveInlining)]
        public static FP FromRaw(int raw)
        {
            FP r;
            r.Raw = raw;
            return r;
        }

        public static FP FromInt(int v) { return new FP(v); }
        public static FP FromFloat(float v) { return new FP(v); }
        public static FP FromDouble(double v) { return new FP(v); }
        public static FP FromFP(FP v) { return new FP(v); }

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
            return Fixed32.ToString(Raw);
        }

        public override int GetHashCode()
        {
            return (int)Raw | (int)(Raw >> 32);
        }
    }
#endif
}
