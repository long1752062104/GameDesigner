using System;
using System.Runtime.CompilerServices;
using REAL = FixMath.FP;

#if GGPHYS_FIXPOINT32
using RawType = System.Int32;
#else
using RawType = System.Int64;
#endif

namespace GGPhys.Core
{
    public class DMath
    {

        public static REAL EPS = 1e-9;

        public static REAL PI = REAL.Pi;

        public static REAL SQRT2 = 1.414213562373095;

        public static REAL Rad2Deg = 180.0 / PI;

        public static REAL Deg2Rad = PI / 180.0;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static REAL SafeAcos(REAL r)
        {
            return REAL.Acos(REAL.Clamp(r, -1, 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static REAL SafeAsin(REAL r)
        {
            return REAL.Asin(REAL.Min(1.0, REAL.Max((- 1.0), r)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static REAL SafeSqrt(REAL v)
        {
            if (v <= 0.0) return 0.0;
            return REAL.SqrtFastest(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static REAL SafeLog(REAL v)
        {
            if (v <= 0.0) return 0.0;
            return REAL.Log(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static REAL SafeInvSqrt(REAL n, REAL d)
        {
            if (d <= 0) return 0;
            d = REAL.Sqrt(d);
            if (d < EPS) return 0;
            return n / d;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static REAL SafeDiv(REAL n, REAL d)
        {
            if (REAL.Abs(d) < EPS) return 0;
            return n / d;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(REAL v)
        {
            return REAL.Abs(v) < EPS;
        }
    }
}




















