using System;
using System.IO;
using UnityEngine;

namespace TrueSync
{

    /// <summary>
    ///表示Q31.32
    /// </summary>
    [Serializable]
    public partial struct FP : IEquatable<FP>, IComparable<FP>
    {
        public long _serializedValue;

        public const long MAX_VALUE = long.MaxValue;
        public const long MIN_VALUE = long.MinValue;
        public const int NUM_BITS = 64;
        public const int FRACTIONAL_PLACES = 32;
        public const long ONE = 1L << FRACTIONAL_PLACES;
        public const long TWO = 2L << FRACTIONAL_PLACES;
        public const long TEN = 10L << FRACTIONAL_PLACES;
        public const long HALF = 1L << (FRACTIONAL_PLACES - 1);
        public const long PI_TIMES_2 = 0x6487ED511;
        public const long PI = 0x3243F6A88;
        public const long PI_OVER_2 = 0x1921FB544;
        public const long LN2 = 0xB17217F7;
        public const long LOG2MAX = 0x1F00000000;
        public const long LOG2MIN = -0x2000000000;
        public const int LUT_SIZE = (int)(PI_OVER_2 >> 15);

        //二^-32，23283064365389628906225E-10
        public static readonly decimal Precision = (decimal)(new FP(1L));//00000000023283064365389628906225m
        public static readonly FP MaxValue = new FP(MAX_VALUE - 1);
        public static readonly FP MinValue = new FP(MIN_VALUE + 2);
        public static readonly FP One = new FP(ONE);
        public static readonly FP Two = new FP(TWO);
        public static readonly FP Ten = new FP(TEN);
        public static readonly FP Half = new FP(HALF);

        public static readonly FP Zero = new FP();
        public static readonly FP PositiveInfinity = new FP(MAX_VALUE);
        public static readonly FP NegativeInfinity = new FP(MIN_VALUE + 1);
        public static readonly FP NaN = new FP(MIN_VALUE);

        public static readonly FP EN1 = FP.One / 10;
        public static readonly FP EN2 = FP.One / 100;
        public static readonly FP EN3 = FP.One / 1000;
        public static readonly FP EN4 = FP.One / 10000;
        public static readonly FP EN5 = FP.One / 100000;
        public static readonly FP EN6 = FP.One / 1000000;
        public static readonly FP EN7 = FP.One / 10000000;
        public static readonly FP EN8 = FP.One / 100000000;
        public static readonly FP Epsilon = FP.EN3;

        /// <summary>
        ///圆周率
        /// </summary>
        public static readonly FP Pi = new FP(PI);
        public static readonly FP PiOver2 = new FP(PI_OVER_2);
        public static readonly FP PiTimes2 = new FP(PI_TIMES_2);
        public static readonly FP PiInv = (FP)0.3183098861837906715377675267M;
        public static readonly FP PiOver2Inv = (FP)0.6366197723675813430755350535M;

        public static readonly FP Deg2Rad = Pi / new FP(180);

        public static readonly FP Rad2Deg = new FP(180) / Pi;

        public static readonly FP LutInterval = (LUT_SIZE - 1) / PiOver2;

        public static readonly FP Log2Max = new FP(LOG2MAX);
        public static readonly FP Log2Min = new FP(LOG2MIN);
        public static readonly FP Ln2 = new FP(LN2);

        /// <summary>
        ///一、固定装置
        ///如
        /// </summary>
        public static int Sign(FP value)
        {
            return
                value._serializedValue < 0 ? -1 :
                value._serializedValue > 0 ? 1 :
                0;
        }


        /// <summary>
        ///固定装置
        ///固定值=64
        /// </summary>
        public static FP Abs(FP value)
        {
            if (value._serializedValue == MIN_VALUE)
            {
                return MaxValue;
            }

            //无分支实现，请参阅http：
            long mask = value._serializedValue >> 63;
            FP result;
            result._serializedValue = (value._serializedValue + mask) ^ mask;
            return result;
        }

        /// <summary>
        ///固定装置
        ///FastAbs（Fix64.MinValue）公司
        /// </summary>
        public static FP FastAbs(FP value)
        {
            //无分支实现，请参阅http：
            long mask = value._serializedValue >> 63;
            FP result;
            result._serializedValue = (value._serializedValue + mask) ^ mask;
            return result;
        }


        /// <summary>
        ///返回小于或等于指定数字的最大整数。
        /// </summary>
        public static FP Floor(FP value)
        {
            //把小数部分归零
            FP result;
            result._serializedValue = (long)((ulong)value._serializedValue & 0xFFFFFFFF00000000);
            return result;
        }

        /// <summary>
        ///返回大于或等于指定数字的最小整数值。
        /// </summary>
        public static FP Ceiling(FP value)
        {
            bool hasFractionalPart = (value._serializedValue & 0x00000000FFFFFFFF) != 0;
            return hasFractionalPart ? Floor(value) + One : value;
        }

        /// <summary>
        ///将值舍入为最接近的整数值。
        ///如果值介于偶数和不均匀值之间，则返回偶数值。
        /// </summary>
        public static FP Round(FP value)
        {
            long fractionalPart = value._serializedValue & 0x00000000FFFFFFFF;
            FP integralPart = Floor(value);
            if (fractionalPart < 0x80000000)
            {
                return integralPart;
            }
            if (fractionalPart > 0x80000000)
            {
                return integralPart + One;
            }
            //如果数字介于两个值的中间，则四舍五入到最近的偶数
            //这是System.Math.Round().
            return (integralPart._serializedValue & ONE) == 0
                       ? integralPart
                       : integralPart + One;
        }

        /// <summary>
        ///x和y
        ///最大值
        /// </summary>
        public static FP operator +(FP x, FP y)
        {
            FP result;
            result._serializedValue = x._serializedValue + y._serializedValue;
            return result;
        }

        /// <summary>
        ///第三条
        /// </summary>
        public static FP OverflowAdd(FP x, FP y)
        {
            long xl = x._serializedValue;
            long yl = y._serializedValue;
            long sum = xl + yl;
            //工作
            if (((~(xl ^ yl) & (xl ^ sum)) & MIN_VALUE) != 0)
            {
                sum = xl > 0 ? MAX_VALUE : MIN_VALUE;
            }
            FP result;
            result._serializedValue = sum;
            return result;
            //左FP（总和）
        }

        /// <summary>
        ///x x y
        /// </summary>
        public static FP FastAdd(FP x, FP y)
        {
            FP result;
            result._serializedValue = x._serializedValue + y._serializedValue;
            return result;
        }

        /// <summary>
        ///[英]
        ///最大值
        /// </summary>
        public static FP operator -(FP x, FP y)
        {
            FP result;
            result._serializedValue = x._serializedValue - y._serializedValue;
            return result;
        }

        /// <summary>
        ///第三条
        /// </summary>
        public static FP OverflowSub(FP x, FP y)
        {
            long xl = x._serializedValue;
            long yl = y._serializedValue;
            long diff = xl - yl;
            //工作
            if ((((xl ^ yl) & (xl ^ diff)) & MIN_VALUE) != 0)
            {
                diff = xl < 0 ? MIN_VALUE : MAX_VALUE;
            }
            FP result;
            result._serializedValue = diff;
            return result;
            //左FP（差分）
        }

        /// <summary>
        ///第三条
        /// </summary>
        public static FP FastSub(FP x, FP y)
        {
            return new FP(x._serializedValue - y._serializedValue);
        }

        static long AddOverflowHelper(long x, long y, ref bool overflow)
        {
            long sum = x + y;
            //（x）x+y=x
            overflow |= ((x ^ y ^ sum) & MIN_VALUE) != 0;
            return sum;
        }

        public static FP operator *(FP x, FP y)
        {
            long xl = x._serializedValue;
            long yl = y._serializedValue;

            ulong xlo = (ulong)(xl & 0x00000000FFFFFFFF);
            long xhi = xl >> FRACTIONAL_PLACES;
            ulong ylo = (ulong)(yl & 0x00000000FFFFFFFF);
            long yhi = yl >> FRACTIONAL_PLACES;

            ulong lolo = xlo * ylo;
            long lohi = (long)xlo * yhi;
            long hilo = xhi * (long)ylo;
            long hihi = xhi * yhi;

            ulong loResult = lolo >> FRACTIONAL_PLACES;
            long midResult1 = lohi;
            long midResult2 = hilo;
            long hiResult = hihi << FRACTIONAL_PLACES;

            long sum = (long)loResult + midResult1 + midResult2 + hiResult;
            FP result;
            result._serializedValue = sum;
            return result;

            //FP result;
            //result._serializedValue = (long)((float)x._serializedValue / ONE * ((float)y._serializedValue / ONE) * ONE);
            //return result;
        }

        /// <summary>
        ///执行乘法而不检查溢出。
        ///对于保证值不会导致溢出的性能关键代码非常有用
        /// </summary>
        public static FP OverflowMul(FP x, FP y)
        {
            long xl = x._serializedValue;
            long yl = y._serializedValue;

            ulong xlo = (ulong)(xl & 0x00000000FFFFFFFF);
            long xhi = xl >> FRACTIONAL_PLACES;
            ulong ylo = (ulong)(yl & 0x00000000FFFFFFFF);
            long yhi = yl >> FRACTIONAL_PLACES;

            ulong lolo = xlo * ylo;
            long lohi = (long)xlo * yhi;
            long hilo = xhi * (long)ylo;
            long hihi = xhi * yhi;

            ulong loResult = lolo >> FRACTIONAL_PLACES;
            long midResult1 = lohi;
            long midResult2 = hilo;
            long hiResult = hihi << FRACTIONAL_PLACES;

            bool overflow = false;
            long sum = AddOverflowHelper((long)loResult, midResult1, ref overflow);
            sum = AddOverflowHelper(sum, midResult2, ref overflow);
            sum = AddOverflowHelper(sum, hiResult, ref overflow);

            bool opSignsEqual = ((xl ^ yl) & MIN_VALUE) == 0;

            //如果操作数的符号相等而结果的符号是负数，
            //然后正乘法溢出
            //反之亦然
            if (opSignsEqual)
            {
                if (sum < 0 || (overflow && xl > 0))
                {
                    return MaxValue;
                }
            }
            else
            {
                if (sum > 0)
                {
                    return MinValue;
                }
            }

            //[答：]
            //这意味着结果溢出。
            long topCarry = hihi >> FRACTIONAL_PLACES;
            if (topCarry != 0 && topCarry != -1 /*&& xl != -17 && yl != -17*/)
            {
                return opSignsEqual ? MaxValue : MinValue;
            }

            //工作
            //并且结果大于负操作数，则出现负溢出。
            if (!opSignsEqual)
            {
                long posOp, negOp;
                if (xl > yl)
                {
                    posOp = xl;
                    negOp = yl;
                }
                else
                {
                    posOp = yl;
                    negOp = xl;
                }
                if (sum > negOp && negOp < -ONE && posOp > ONE)
                {
                    return MinValue;
                }
            }
            FP result;
            result._serializedValue = sum;
            return result;
            //左FP（总和）
        }

        /// <summary>
        ///执行乘法而不检查溢出。
        ///对于保证值不会导致溢出的性能关键代码非常有用
        /// </summary>
        public static FP FastMul(FP x, FP y)
        {
            long xl = x._serializedValue;
            long yl = y._serializedValue;

            ulong xlo = (ulong)(xl & 0x00000000FFFFFFFF);
            long xhi = xl >> FRACTIONAL_PLACES;
            ulong ylo = (ulong)(yl & 0x00000000FFFFFFFF);
            long yhi = yl >> FRACTIONAL_PLACES;

            ulong lolo = xlo * ylo;
            long lohi = (long)xlo * yhi;
            long hilo = xhi * (long)ylo;
            long hihi = xhi * yhi;

            ulong loResult = lolo >> FRACTIONAL_PLACES;
            long midResult1 = lohi;
            long midResult2 = hilo;
            long hiResult = hihi << FRACTIONAL_PLACES;

            long sum = (long)loResult + midResult1 + midResult2 + hiResult;
            FP result;//=违（FP）
            result._serializedValue = sum;
            return result;
            //左FP（总和）
        }

        //[MethodImplAttribute(MethodImpleOptions.AggressiveInline)]
        public static int CountLeadingZeroes(ulong x)
        {
            int result = 0;
            while ((x & 0xF000000000000000) == 0) { result += 4; x <<= 4; }
            while ((x & 0x8000000000000000) == 0) { result += 1; x <<= 1; }
            return result;
        }

        public static FP operator /(FP x, FP y)
        {
            long xl = x._serializedValue;
            long yl = y._serializedValue;

            if (yl == 0)
            {
                return MAX_VALUE;
                //[DivideByZeroException（）
            }

            ulong remainder = (ulong)(xl >= 0 ? xl : -xl);
            ulong divider = (ulong)(yl >= 0 ? yl : -yl);
            ulong quotient = 0UL;
            int bitPos = NUM_BITS / 2 + 1;


            //如
            while ((divider & 0xF) == 0 && bitPos >= 4)
            {
                divider >>= 4;
                bitPos -= 4;
            }

            while (remainder != 0 && bitPos >= 0)
            {
                int shift = CountLeadingZeroes(remainder);
                if (shift > bitPos)
                {
                    shift = bitPos;
                }
                remainder <<= shift;
                bitPos -= shift;

                ulong div = remainder / divider;
                remainder %= divider;
                quotient += div << bitPos;

                //检测溢出
                if ((div & ~(0xFFFFFFFFFFFFFFFF >> bitPos)) != 0)
                {
                    return ((xl ^ yl) & MIN_VALUE) == 0 ? MaxValue : MinValue;
                }

                remainder <<= 1;
                --bitPos;
            }

            //舍入
            ++quotient;
            long result = (long)(quotient >> 1);
            if (((xl ^ yl) & MIN_VALUE) != 0)
            {
                result = -result;
            }

            FP r;
            r._serializedValue = result;
            return r;

            //FP r;
            //r._serializedValue = (long)(((float)x._serializedValue / ONE) / ((float)y._serializedValue / ONE) * ONE);
            //return r;
        }

        public static FP operator %(FP x, FP y)
        {
            FP result;
            result._serializedValue = x._serializedValue == MIN_VALUE & y._serializedValue == -1 ?
                0 :
                x._serializedValue % y._serializedValue;
            return result;
        }

        /// <summary>
        ///x==最小值y=-1
        ///使用运算符（%）表示更可靠但较慢的模。
        /// </summary>
        public static FP FastMod(FP x, FP y)
        {
            FP result;
            result._serializedValue = x._serializedValue % y._serializedValue;
            return result;
        }

        public static FP operator -(FP x)
        {
            return x._serializedValue == MIN_VALUE ? MaxValue : new FP(-x._serializedValue);
        }

        public static bool operator ==(FP x, FP y)
        {
            return x._serializedValue == y._serializedValue;
        }

        public static bool operator !=(FP x, FP y)
        {
            return x._serializedValue != y._serializedValue;
        }

        public static bool operator >(FP x, FP y)
        {
            return x._serializedValue > y._serializedValue;
        }

        public static bool operator <(FP x, FP y)
        {
            return x._serializedValue < y._serializedValue;
        }

        public static bool operator >=(FP x, FP y)
        {
            return x._serializedValue >= y._serializedValue;
        }

        public static bool operator <=(FP x, FP y)
        {
            return x._serializedValue <= y._serializedValue;
        }


        /// <summary>
        ///返回指定数字的平方根。
        /// </summary>
        ///<exception cref="ArgumentOutOfRangeException">
        ///争论是否定的。
        ///</例外>
        public static FP Sqrt(FP x)
        {
            long xl = x._serializedValue;
            if (xl < 0)
            {
                //斯奎特
                //throw new ArgumentOutOfRangeException("Negative value passed to Sqrt", "x");
                return 0;
            }

            ulong num = (ulong)xl;
            ulong result = 0UL;

            //第二位到第二位
            ulong bit = 1UL << (NUM_BITS - 2);

            while (bit > num)
            {
                bit >>= 2;
            }

            //主要部分执行两次，以避免
            //第一百二十八条
            for (int i = 0; i < 2; ++i)
            {
                //[答：]
                while (bit != 0)
                {
                    if (num >= result + bit)
                    {
                        num -= result + bit;
                        result = (result >> 1) + bit;
                    }
                    else
                    {
                        result >>= 1;
                    }
                    bit >>= 2;
                }

                if (i == 0)
                {
                    //第16条
                    if (num > (1UL << (NUM_BITS / 2)) - 1)
                    {
                        //"数字"法庭
                        //第32页，第1页
                        //"数字"
                        //数字=a-（0.5）^2
                        //=num+
                        //=数值-结-0.5
                        num -= result;
                        num = (num << (NUM_BITS / 2)) - 0x80000000UL;
                        result = (result << (NUM_BITS / 2)) + 0x80000000UL;
                    }
                    else
                    {
                        num <<= NUM_BITS / 2;
                        result <<= NUM_BITS / 2;
                    }

                    bit = 1UL << (NUM_BITS / 2 - 2);
                }
            }
            //一、一、二
            if (num > result)
            {
                ++result;
            }

            FP r;
            r._serializedValue = (long)result;
            return r;
            //左FP（长）
        }

        /// <summary>
        ///x轴
        ///九
        ///x轴，x轴
        ///性：比数学.罪恶（）x64英寸，x86英寸慢200%
        /// </summary>
        public static FP Sin(FP x)
        {
            long clampedL = ClampSinValue(x._serializedValue, out bool flipHorizontal, out bool flipVertical);
            FP clamped = new FP(clampedL);

            //国内外
            //x86-x64型
            FP rawIndex = FastMul(clamped, LutInterval);
            FP roundedIndex = Round(rawIndex);
            int indexError = 0;//FastSub（rawIndex，roundedIndex）

            FP nearestValue = new FP(SinLut[flipHorizontal ?
                SinLut.Length - 1 - (int)roundedIndex :
                (int)roundedIndex]);
            FP secondNearestValue = new FP(SinLut[flipHorizontal ?
                SinLut.Length - 1 - (int)roundedIndex - Sign(indexError) :
                (int)roundedIndex + Sign(indexError)]);

            long delta = FastMul(indexError, FastAbs(FastSub(nearestValue, secondNearestValue)))._serializedValue;
            long interpolatedValue = nearestValue._serializedValue + (flipHorizontal ? -delta : delta);
            long finalValue = flipVertical ? -interpolatedValue : interpolatedValue;

            //FP a2=1磅/平方英尺
            FP a2;
            a2._serializedValue = finalValue;
            return a2;
        }

        /// <summary>
        ///x轴
        ///这，
        ///四、四、五
        /// </summary>
        public static FP FastSin(FP x)
        {
            long clampedL = ClampSinValue(x._serializedValue, out bool flipHorizontal, out bool flipVertical);

            //在中国
            //等
            uint rawIndex = (uint)(clampedL >> 15);
            if (rawIndex >= LUT_SIZE)
            {
                rawIndex = LUT_SIZE - 1;
            }
            long nearestValue = SinLut[flipHorizontal ?
                SinLut.Length - 1 - (int)rawIndex :
                (int)rawIndex];

            FP result;
            result._serializedValue = flipVertical ? -nearestValue : nearestValue;
            return result;
            //FP（flipVertical？-nearest value:
        }



        //[MethodImplAttribute(MethodImpleOptions.AggressiveInline)]
        public static long ClampSinValue(long angle, out bool flipHorizontal, out bool flipVertical)
        {
            //第二条
            long clamped2Pi = angle % PI_TIMES_2;
            if (angle < 0)
            {
                clamped2Pi += PI_TIMES_2;
            }

            //查找表
            //垂直或水平镜像
            flipVertical = clamped2Pi >= PI;
            //从（角度%2PI）获得（角度%PI）-比做一个模得多
            long clampedPi = clamped2Pi;
            while (clampedPi >= PI)
            {
                clampedPi -= PI;
            }
            flipHorizontal = clampedPi >= PI_OVER_2;
            //从（角度%PI）获得（角度%PI）比做另一模得多
            long clampedPiOver2 = clampedPi;
            if (clampedPiOver2 >= PI_OVER_2)
            {
                clampedPiOver2 -= PI_OVER_2;
            }
            return clampedPiOver2;
        }

        /// <summary>
        ///x轴
        ///关详细息，请参见Sin（）
        /// </summary>
        public static FP Cos(FP x)
        {
            long xl = x._serializedValue;
            long rawAngle = xl + (xl > 0 ? -PI - PI_OVER_2 : PI_OVER_2);
            FP a2 = Sin(new FP(rawAngle));
            return a2;
        }

        /// <summary>
        ///x轴
        ///斋戒
        /// </summary>
        public static FP FastCos(FP x)
        {
            long xl = x._serializedValue;
            long rawAngle = xl + (xl > 0 ? -PI - PI_OVER_2 : PI_OVER_2);
            return FastSin(new FP(rawAngle));
        }

        /// <summary>
        ///x轴
        /// </summary>
        ///<备注>
        ///这个功能没有经过很好的测试。这可能非常不准确。
        ///</备注>
        public static FP Tan(FP x)
        {
            long clampedPi = x._serializedValue % PI;
            bool flip = false;
            if (clampedPi < 0)
            {
                clampedPi = -clampedPi;
                flip = true;
            }
            if (clampedPi > PI_OVER_2)
            {
                flip = !flip;
                clampedPi = PI_OVER_2 - (clampedPi - PI_OVER_2);
            }

            FP clamped = new FP(clampedPi);

            //国内外
            FP rawIndex = FastMul(clamped, LutInterval);
            FP roundedIndex = Round(rawIndex);
            FP indexError = FastSub(rawIndex, roundedIndex);

            FP nearestValue = new FP(TanLut[(int)roundedIndex]);
            FP secondNearestValue = new FP(TanLut[(int)roundedIndex + Sign(indexError)]);

            long delta = FastMul(indexError, FastAbs(FastSub(nearestValue, secondNearestValue)))._serializedValue;
            long interpolatedValue = nearestValue._serializedValue + delta;
            long finalValue = flip ? -interpolatedValue : interpolatedValue;
            FP a2 = new FP(finalValue);
            return a2;
        }

        /// <summary>
        ///欧拉
        ///第七条
        /// </summary>
        public static FP Atan(FP z)
        {
            if (z.RawValue == 0) return Zero;

            //参数的强制正值
            //Atan（-z）=-Atan（z）
            bool neg = z.RawValue < 0;
            if (neg)
            {
                z = -z;
            }

            FP result;
            FP two = 2;
            FP three = 3;

            bool invert = z > One;
            if (invert) z = One / z;

            result = One;
            FP term = One;

            FP zSq = z * z;
            FP zSq2 = zSq * two;
            FP zSqPlusOne = zSq + One;
            FP zSq12 = zSqPlusOne * two;
            FP dividend = zSq2;
            FP divisor = zSqPlusOne * three;

            for (int i = 2; i < 30; ++i)
            {
                term *= dividend / divisor;
                result += term;

                dividend += zSq2;
                divisor += zSq12;

                if (term.RawValue == 0) break;
            }

            result = result * z / zSqPlusOne;

            if (invert)
            {
                result = PiOver2 - result;
            }

            if (neg)
            {
                result = -result;
            }
            return result;
        }

        public static FP Atan2(FP y, FP x)
        {
            long yl = y._serializedValue;
            long xl = x._serializedValue;
            if (xl == 0)
            {
                if (yl > 0)
                {
                    return PiOver2;
                }
                if (yl == 0)
                {
                    return Zero;
                }
                return -PiOver2;
            }
            FP atan;
            FP z = y / x;

            FP sm = FP.EN2 * 28;
            //处理溢出
            if (One + sm * z * z == MaxValue)
            {
                return y < Zero ? -PiOver2 : PiOver2;
            }

            if (Abs(z) < One)
            {
                atan = z / (One + sm * z * z);
                if (xl < 0)
                {
                    if (yl < 0)
                    {
                        return atan - Pi;
                    }
                    return atan + Pi;
                }
            }
            else
            {
                atan = PiOver2 - z / (z * z + sm);
                if (yl < 0)
                {
                    return atan - Pi;
                }
            }
            return atan;
        }

        public static FP Asin(FP value)
        {
            return FastSub(PiOver2, Acos(value));
        }

        /// <summary>
        ///阿坦
        ///第七条
        /// </summary>
        public static FP Acos(FP x)
        {
            if (x < -One || x > One)
            {
                throw new ArgumentOutOfRangeException("Must between -FP.One and FP.One", "x");
            }

            if (x.RawValue == 0) return PiOver2;

            FP result = Atan(Sqrt(One - x * x) / x);
            return x.RawValue < 0 ? result + Pi : result;
        }

        public static implicit operator FP(long value)
        {
            FP result;
            result._serializedValue = value * ONE;
            return result;
            //返FP（值*1）
        }

        public static explicit operator long(FP value)
        {
            return value._serializedValue >> FRACTIONAL_PLACES;
        }

        public static implicit operator FP(float value)
        {
            FP result;
            result._serializedValue = (long)(value * ONE);
            return result;
            //左FP（（长）（值*一）
        }

        public static explicit operator float(FP value)
        {
            return (float)value._serializedValue / ONE;
        }

        public static implicit operator FP(double value)
        {
            FP result;
            result._serializedValue = (long)(value * ONE);
            return result;
            //左FP（（长）（值*一）
        }

        public static explicit operator double(FP value)
        {
            return (double)value._serializedValue / ONE;
        }

        public static explicit operator FP(decimal value)
        {
            FP result;
            result._serializedValue = (long)(value * ONE);
            return result;
            //左FP（（长）（值*一）
        }

        public static implicit operator FP(int value)
        {
            FP result;
            result._serializedValue = value * ONE;
            return result;
            //返FP（值*1）
        }

        public static explicit operator decimal(FP value)
        {
            return (decimal)value._serializedValue / ONE;
        }

        public float AsFloat()
        {
            return (float)this;
        }

        public int AsInt()
        {
            return (int)this;
        }

        public long AsLong()
        {
            return (long)this;
        }

        public double AsDouble()
        {
            return (double)this;
        }

        public decimal AsDecimal()
        {
            return (decimal)this;
        }

        public static float ToFloat(FP value)
        {
            return (float)value;
        }

        public static int ToInt(FP value)
        {
            return (int)value;
        }

        public static FP FromFloat(float value)
        {
            return value;
        }

        public static bool IsInfinity(FP value)
        {
            return value == NegativeInfinity || value == PositiveInfinity;
        }

        public static bool IsNaN(FP value)
        {
            return value == NaN;
        }

        public override bool Equals(object obj)
        {
            return obj is FP && ((FP)obj)._serializedValue == _serializedValue;
        }

        public override int GetHashCode()
        {
            return _serializedValue.GetHashCode();
        }

        public bool Equals(FP other)
        {
            return _serializedValue == other._serializedValue;
        }

        public int CompareTo(FP other)
        {
            return _serializedValue.CompareTo(other._serializedValue);
        }

        public override string ToString()
        {
            return ((float)this).ToString();
        }

        public string ToString(IFormatProvider provider)
        {
            return ((float)this).ToString(provider);
        }
        public string ToString(string format)
        {
            return ((float)this).ToString(format);
        }

        public static FP FromRaw(long rawValue)
        {
            return new FP(rawValue);
        }

        public static FP FromRaw(FP rawValue)
        {
            return rawValue;
        }

        internal static void GenerateAcosLut()
        {
            using (StreamWriter writer = new StreamWriter("Fix64AcosLut.cs"))
            {
                writer.Write(
@"namespace TrueSync {
    partial struct FP {
        public static readonly long[] AcosLut = new[] {");
                int lineCounter = 0;
                for (int i = 0; i < LUT_SIZE; ++i)
                {
                    float angle = i / ((float)(LUT_SIZE - 1));
                    if (lineCounter++ % 8 == 0)
                    {
                        writer.WriteLine();
                        writer.Write("            ");
                    }
                    double acos = Math.Acos(angle);
                    long rawValue = ((FP)acos)._serializedValue;
                    writer.Write(string.Format("0x{0:X}L, ", rawValue));
                }
                writer.Write(
@"
        };
    }
}");
            }
        }

        internal static void GenerateSinLut()
        {
            using (StreamWriter writer = new StreamWriter("Fix64SinLut.cs"))
            {
                writer.Write(
@"namespace FixMath.NET {
    partial struct Fix64 {
        public static readonly long[] SinLut = new[] {");
                int lineCounter = 0;
                for (int i = 0; i < LUT_SIZE; ++i)
                {
                    double angle = i * Math.PI * 0.5 / (LUT_SIZE - 1);
                    if (lineCounter++ % 8 == 0)
                    {
                        writer.WriteLine();
                        writer.Write("            ");
                    }
                    double sin = Math.Sin(angle);
                    long rawValue = ((FP)sin)._serializedValue;
                    writer.Write(string.Format("0x{0:X}L, ", rawValue));
                }
                writer.Write(
@"
        };
    }
}");
            }
        }

        internal static void GenerateTanLut()
        {
            using (StreamWriter writer = new StreamWriter("Fix64TanLut.cs"))
            {
                writer.Write(
@"namespace FixMath.NET {
    partial struct Fix64 {
        public static readonly long[] TanLut = new[] {");
                int lineCounter = 0;
                for (int i = 0; i < LUT_SIZE; ++i)
                {
                    double angle = i * Math.PI * 0.5 / (LUT_SIZE - 1);
                    if (lineCounter++ % 8 == 0)
                    {
                        writer.WriteLine();
                        writer.Write("            ");
                    }
                    double tan = Math.Tan(angle);
                    if (tan > (double)MaxValue || tan < 0.0)
                    {
                        tan = (double)MaxValue;
                    }
                    long rawValue = (((decimal)tan > (decimal)MaxValue || tan < 0.0) ? MaxValue : tan)._serializedValue;
                    writer.Write(string.Format("0x{0:X}L, ", rawValue));
                }
                writer.Write(
@"
        };
    }
}");
            }
        }

        /// <summary>
        /// 底层整数表示
        /// </summary>
        public long RawValue { get { return _serializedValue; } }

        /// <summary>
        /// 这是来自原始值的构造函数；它只能在内部使用。
        /// </summary>
        ///<param name="rawValue"></param>
        public FP(long rawValue)
        {
            _serializedValue = rawValue;
        }

        public FP(int value)
        {
            _serializedValue = value * ONE;
        }

        public static FP Mul(FP a, FP b)
        {
            return a * b;
        }

        public long Raw { get { return _serializedValue; } }

        public static FP Max(FP a, FP b)
        {
            return TSMathf.Max(a, b);
        }
        public static FP Min(FP a, FP b)
        {
            return TSMathf.Min(a, b);
        }

        public static FP Clamp(FP value, FP min, FP max)
        {
            return TSMathf.Clamp(value, min, max);
        }
    }
}
