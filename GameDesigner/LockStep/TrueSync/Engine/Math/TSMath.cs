using FixPointCS;
using System;
/* Copyright (C) <2009-2011> <Thorben Linneweber, Jitter Physics>
* 
*  This software is provided 'as-is', without any express or implied
*  warranty.  In no event will the authors be held liable for any damages
*  arising from the use of this software.
*
*  Permission is granted to anyone to use this software for any purpose,
*  including commercial applications, and to alter it and redistribute it
*  freely, subject to the following restrictions:
*
*  1. The origin of this software must not be misrepresented; you must not
*      claim that you wrote the original software. If you use this software
*      in a product, an acknowledgment in the product documentation would be
*      appreciated but is not required.
*  2. Altered source versions must be plainly marked as such, and must not be
*      misrepresented as being the original software.
*  3. This notice may not be removed or altered from any source distribution. 
*/

namespace TrueSync
{

    /// <summary>
    ///包含常见的数学运算。
    /// </summary>
    public sealed class TSMathf
    {

        /// <summary>
        ///π常数。
        /// </summary>
        public static FP Pi = FP.Pi;

        /**
        *  @brief PI over 2 constant.
        **/
        public static FP PiOver2 = FP.PiOver2;

        /// <summary>
        ///一个小值，通常用来决定是否是数字
        ///结果为零。
        /// </summary>
        public static FP Epsilon = FP.Epsilon;

        /**
        *  @brief Degree to radians constant.
        **/
        public static FP Deg2Rad = FP.Deg2Rad;

        /**
        *  @brief Radians to degree constant.
        **/
        public static FP Rad2Deg = FP.Rad2Deg;


        /**
         * @brief FP infinity.
         * */
        public static FP Infinity = FP.MaxValue;

        /// <summary>
        ///得到平方根。
        /// </summary>
        ///<param name="number">
        ///<returns></returns>
        #region public static FP Sqrt(FP number)
        public static FP Sqrt(FP number)
        {
            return FP.Sqrt(number);
        }
        #endregion

        /// <summary>
        ///获取两个值的最大数目。
        /// </summary>
        ///<param name="val1">
        ///<param name="val2">
        ///<returns>返回
        #region public static FP Max(FP val1, FP val2)
        public static FP Max(FP val1, FP val2)
        {
            return (val1 > val2) ? val1 : val2;
        }
        #endregion

        /// <summary>
        ///获取两个值的最小数目。
        /// </summary>
        ///<param name="val1">
        ///<param name="val2">
        ///<returns>返回
        #region public static FP Min(FP val1, FP val2)
        public static FP Min(FP val1, FP val2)
        {
            return (val1 < val2) ? val1 : val2;
        }
        #endregion

        /// <summary>
        ///获取三个值的最大数目。
        /// </summary>
        ///<param name="val1">
        ///<param name="val2">
        ///<param name="val3">
        ///<returns>返回
        #region public static FP Max(FP val1, FP val2,FP val3)
        public static FP Max(FP val1, FP val2, FP val3)
        {
            FP max12 = (val1 > val2) ? val1 : val2;
            return (max12 > val3) ? max12 : val3;
        }
        #endregion

        /// <summary>
        ///一[最小值，最大值]
        /// </summary>
        ///<param name="value">
        ///<param name="min">
        ///<param name="max">
        ///<returns>
        #region public static FP Clamp(FP value, FP min, FP max)
        public static FP Clamp(FP value, FP min, FP max)
        {
            if (value < min)
            {
                value = min;
                return value;
            }
            if (value > max)
            {
                value = max;
            }
            return value;
        }
        #endregion

        /// <summary>
        ///返回一个在[浮点零, 一楼]
        /// </summary>
        ///<param name="value">
        ///<returns>
        public static FP Clamp01(FP value)
        {
            if (value < FP.Zero)
                return FP.Zero;

            if (value > FP.One)
                return FP.One;

            return value;
        }

        /// <summary>
        ///将矩阵项的每个符号改为"+"
        /// </summary>
        ///<param name="matrix">
        ///<param name="result">
        #region public static void Absolute(ref JMatrix matrix,out JMatrix result)
        public static void Absolute(ref TSMatrix3 matrix, out TSMatrix3 result)
        {
            result.M11 = FP.Abs(matrix.M11);
            result.M12 = FP.Abs(matrix.M12);
            result.M13 = FP.Abs(matrix.M13);
            result.M21 = FP.Abs(matrix.M21);
            result.M22 = FP.Abs(matrix.M22);
            result.M23 = FP.Abs(matrix.M23);
            result.M31 = FP.Abs(matrix.M31);
            result.M32 = FP.Abs(matrix.M32);
            result.M33 = FP.Abs(matrix.M33);
        }
        #endregion

        /// <summary>
        ///返回值的正弦值。
        /// </summary>
        public static FP Sin(FP value)
        {
            return FP.Sin(value);
        }

        /// <summary>
        ///返回值的余弦值。
        /// </summary>
        public static FP Cos(FP value)
        {
            return FP.Cos(value);
        }

        /// <summary>
        ///谭
        /// </summary>
        public static FP Tan(FP value)
        {
            return FP.Tan(value);
        }

        /// <summary>
        ///返回值的弧正弦值。
        /// </summary>
        public static FP Asin(FP value)
        {
            return FP.Asin(value);
        }

        public static FP Exp(FP a)
        {
            return Fixed64.Exp(a.RawValue);
        }

        /// <summary>
        ///返回值的反余弦值。
        /// </summary>
        public static FP Acos(FP value)
        {
            return FP.Acos(value);
        }

        /// <summary>
        ///谭
        /// </summary>
        public static FP Atan(FP value)
        {
            return FP.Atan(value);
        }

        /// <summary>
        ///黄褐色
        /// </summary>
        public static FP Atan2(FP y, FP x)
        {
            return FP.Atan2(y, x);
        }

        /// <summary>
        ///返回小于或等于指定数字的最大整数。
        /// </summary>
        public static FP Floor(FP value)
        {
            return FP.Floor(value);
        }

        /// <summary>
        ///返回大于或等于指定数字的最小整数值。
        /// </summary>
        public static FP Ceiling(FP value)
        {
            return value;
        }

        /// <summary>
        ///将值舍入为最接近的整数值。
        ///如果值介于偶数和不均匀值之间，则返回偶数值。
        /// </summary>
        public static FP Round(FP value)
        {
            return FP.Round(value);
        }

        /// <summary>
        ///一、固定装置
        ///如
        /// </summary>
        public static int Sign(FP value)
        {
            return FP.Sign(value);
        }

        /// <summary>
        ///固定装置
        ///固定值=64
        /// </summary>
        public static FP Abs(FP value)
        {
            return FP.Abs(value);
        }

        public static FP Barycentric(FP value1, FP value2, FP value3, FP amount1, FP amount2)
        {
            return value1 + (value2 - value1) * amount1 + (value3 - value1) * amount2;
        }

        public static FP CatmullRom(FP value1, FP value2, FP value3, FP value4, FP amount)
        {
            //方法：
            //FPs，FPs
            FP amountSquared = amount * amount;
            FP amountCubed = amountSquared * amount;
            return 0.5 * (2.0 * value2 +
                                 (value3 - value1) * amount +
                                 (2.0 * value1 - 5.0 * value2 + 4.0 * value3 - value4) * amountSquared +
                                 (3.0 * value2 - value1 - 3.0 * value3 + value4) * amountCubed);
        }

        public static FP Distance(FP value1, FP value2)
        {
            return FP.Abs(value1 - value2);
        }

        public static FP Hermite(FP value1, FP tangent1, FP value2, FP tangent2, FP amount)
        {
            //全部转换不失精度
            //数量数：数量结是不是穷
            FP v1 = value1, v2 = value2, t1 = tangent1, t2 = tangent2, s = amount, result;
            FP sCubed = s * s * s;
            FP sSquared = s * s;

            if (amount == 0f)
                result = value1;
            else if (amount == 1f)
                result = value2;
            else
                result = (2 * v1 - 2 * v2 + t2 + t1) * sCubed +
                         (3 * v2 - 3 * v1 - 2 * t1 - t2) * sSquared +
                         t1 * s +
                         v1;
            return result;
        }

        public static FP Lerp(FP value1, FP value2, FP amount)
        {
            return value1 + (value2 - value1) * Clamp01(amount);
        }

        public static FP InverseLerp(FP value1, FP value2, FP amount)
        {
            if (value1 != value2)
                return Clamp01((amount - value1) / (value2 - value1));
            return FP.Zero;
        }

        public static FP SmoothStep(FP value1, FP value2, FP amount)
        {
            //预测0<金额<1
            //<0，1
            //二
            FP result = Clamp(amount, 0f, 1f);
            result = Hermite(value1, 0f, value2, 0f, result);
            return result;
        }


        /// <summary>
        ///第二条
        ///供至少6位小数精度
        /// </summary>
        internal static FP Pow2(FP x)
        {
            if (x.RawValue == 0)
            {
                return FP.One;
            }

            //通过参exp（-x）=1/经验（x）来避免负参数
            bool neg = x.RawValue < 0;
            if (neg)
            {
                x = -x;
            }

            if (x == FP.One)
            {
                return neg ? FP.One / 2 : 2;
            }
            if (x >= FP.Log2Max)
            {
                return neg ? FP.One / FP.MaxValue : FP.MaxValue;
            }
            if (x <= FP.Log2Min)
            {
                return neg ? FP.MaxValue : FP.Zero;
            }

            /* The algorithm is based on the power series for exp(x):
             * http://en.wikipedia.org/wiki/index#函数形式化定义
             * 
             * From term n, we get term n+1 by multiplying with x/n.
             * When the sum term drops to zero, we can stop summing.
             */

            int integerPart = (int)Floor(x);
            //取指数的小数部分
            x = FP.FromRaw(x.RawValue & 0x00000000FFFFFFFF);

            FP result = FP.One;
            FP term = FP.One;
            int i = 1;
            while (term.RawValue != 0)
            {
                term = FP.FastMul(FP.FastMul(x, term), FP.Ln2) / i;
                result += term;
                i++;
            }

            result = FP.FromRaw(result.RawValue << integerPart);
            if (neg)
            {
                result = FP.One / result;
            }

            return result;
        }

        /// <summary>
        ///二
        ///供至少9位小数精度
        /// </summary>
        ///<exception cref="ArgumentOutOfRangeException">
        ///这场争论是非正面的
        ///</例外>
        internal static FP Log2(FP x)
        {
            if (x.RawValue <= 0)
            {
                throw new ArgumentOutOfRangeException("Non-positive value passed to Ln", "x");
            }

            //粘土
            //特纳（C.S.Turner），《电气与电子工程师协会》（IEEE）
            //2010年第124140页第9页）

            long b = 1U << (FP.FRACTIONAL_PLACES - 1);
            long y = 0;

            long rawX = x.RawValue;
            while (rawX < FP.ONE)
            {
                rawX <<= 1;
                y -= FP.ONE;
            }

            while (rawX >= (FP.ONE << 1))
            {
                rawX >>= 1;
                y += FP.ONE;
            }

            FP z = FP.FromRaw(rawX);

            for (int i = 0; i < FP.FRACTIONAL_PLACES; i++)
            {
                z = FP.FastMul(z, z);
                if (z.RawValue >= (FP.ONE << 1))
                {
                    z = FP.FromRaw(z.RawValue >> 1);
                    y += b;
                }
                b >>= 1;
            }

            return FP.FromRaw(y);
        }

        /// <summary>
        ///返回指定数字的自然对数。
        ///供至少7位小数精度
        /// </summary>
        ///<exception cref="ArgumentOutOfRangeException">
        ///这场争论是非正面的
        ///</例外>
        public static FP Ln(FP x)
        {
            return FP.FastMul(Log2(x), FP.Ln2);
        }

        /// <summary>
        ///返回提升到指定幂的指定数字。
        ///第五章
        /// </summary>
        ///<exception cref="DivideByZeroException">
        ///基数为零，指数为负
        ///</例外>
        ///<exception cref="ArgumentOutOfRangeException">
        ///基数为负，指数为非零
        ///</例外>
        public static FP Pow(FP b, FP exp)
        {
            if (b == FP.One)
            {
                return FP.One;
            }

            if (exp.RawValue == 0)
            {
                return FP.One;
            }

            if (b.RawValue == 0)
            {
                if (exp.RawValue < 0)
                {
                    //[DivideByZeroException（）
                    return FP.MaxValue;
                }
                return FP.Zero;
            }

            FP log2 = Log2(b);
            return Pow2(exp * log2);
        }

        public static FP MoveTowards(FP current, FP target, FP maxDelta)
        {
            if (Abs(target - current) <= maxDelta)
                return target;
            return (current + (Sign(target - current)) * maxDelta);
        }

        public static FP Repeat(FP t, FP length)
        {
            return (t - (Floor(t / length) * length));
        }

        public static FP DeltaAngle(FP current, FP target)
        {
            FP num = Repeat(target - current, 360f);
            if (num > 180f)
            {
                num -= (FP)360f;
            }
            return num;
        }

        public static FP MoveTowardsAngle(FP current, FP target, float maxDelta)
        {
            target = current + DeltaAngle(current, target);
            return MoveTowards(current, target, maxDelta);
        }

        public static FP SmoothDamp(FP current, FP target, ref FP currentVelocity, FP smoothTime, FP maxSpeed)
        {
            FP deltaTime = FP.EN2;
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        public static FP SmoothDamp(FP current, FP target, ref FP currentVelocity, FP smoothTime)
        {
            FP deltaTime = FP.EN2;
            FP positiveInfinity = -FP.MaxValue;
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, positiveInfinity, deltaTime);
        }

        public static FP SmoothDamp(FP current, FP target, ref FP currentVelocity, FP smoothTime, FP maxSpeed, FP deltaTime)
        {
            smoothTime = Max(FP.EN4, smoothTime);
            FP num = 2f / smoothTime;
            FP num2 = num * deltaTime;
            FP num3 = FP.One / (((FP.One + num2) + ((0.48f * num2) * num2)) + (((0.235f * num2) * num2) * num2));
            FP num4 = current - target;
            FP num5 = target;
            FP max = maxSpeed * smoothTime;
            num4 = Clamp(num4, -max, max);
            target = current - num4;
            FP num7 = (currentVelocity + (num * num4)) * deltaTime;
            currentVelocity = (currentVelocity - (num * num7)) * num3;
            FP num8 = target + ((num4 + num7) * num3);
            if (((num5 - current) > FP.Zero) == (num8 > num5))
            {
                num8 = num5;
                currentVelocity = (num8 - num5) / deltaTime;
            }
            return num8;
        }
    }
}
