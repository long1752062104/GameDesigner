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

using System;

namespace TrueSync
{
    /// <summary>
    ///矢量结构。
    /// </summary>
    [Serializable]
    public struct TSVector4
    {

        private static FP ZeroEpsilonSq = TSMathf.Epsilon;
        internal static TSVector4 InternalZero;

        /// <summary>The X component of the vector.</summary>
        public FP x;
        /// <summary>The Y component of the vector.</summary>
        public FP y;
        /// <summary>The Z component of the vector.</summary>
        public FP z;
        /// <summary>The W component of the vector.</summary>
        public FP w;

        #region Static readonly variables
        /// <summary>
        ///（0，0，0）
        /// </summary>
        public static readonly TSVector4 zero;
        /// <summary>
        ///（1,1,1,1）
        /// </summary>
        public static readonly TSVector4 one;
        /// <summary>
        ///有分量的向量
        /// (浮点最小值,浮点最小值,浮点最小值);
        /// </summary>
        public static readonly TSVector4 MinValue;
        /// <summary>
        ///有分量的向量
        /// (浮点最大值,浮点最大值,浮点最大值);
        /// </summary>
        public static readonly TSVector4 MaxValue;
        #endregion

        #region Private static constructor
        static TSVector4()
        {
            one = new TSVector4(1, 1, 1, 1);
            zero = new TSVector4(0, 0, 0, 0);
            MinValue = new TSVector4(FP.MinValue);
            MaxValue = new TSVector4(FP.MaxValue);
            InternalZero = zero;
        }
        #endregion

        public static TSVector4 Abs(TSVector4 other)
        {
            return new TSVector4(FP.Abs(other.x), FP.Abs(other.y), FP.Abs(other.z), FP.Abs(other.z));
        }

        /// <summary>
        ///获取向量的平方长度。
        /// </summary>
        ///<returns>返回
        public FP sqrMagnitude
        {
            get
            {
                return (((x * x) + (y * y)) + (z * z) + (w * w));
            }
        }

        /// <summary>
        ///获取向量的长度。
        /// </summary>
        ///<returns>
        public FP magnitude
        {
            get
            {
                FP num = sqrMagnitude;
                return FP.Sqrt(num);
            }
        }

        public static TSVector4 ClampMagnitude(TSVector4 vector, FP maxLength)
        {
            return Normalize(vector) * maxLength;
        }

        /// <summary>
        ///获取向量的规范化版本。
        /// </summary>
        ///<returns>
        public TSVector4 normalized
        {
            get
            {
                TSVector4 result = new TSVector4(x, y, z, w);
                result.Normalize();

                return result;
            }
        }

        /// <summary>
        ///构造函数初始化结构的新实例
        /// </summary>
        ///<param name="x">
        ///<param name="y">
        ///<param name="z">
        ///<param name="w">
        public TSVector4(int x, int y, int z, int w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public TSVector4(FP x, FP y, FP z, FP w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        /// <summary>
        ///将向量的每个分量乘以所提供向量的相同分量。
        /// </summary>
        public void Scale(TSVector4 other)
        {
            x = x * other.x;
            y = y * other.y;
            z = z * other.z;
            w = w * other.w;
        }

        /// <summary>
        ///将所有矢量分量设置为特定值。
        /// </summary>
        ///<param name="x">
        ///<param name="y">
        ///<param name="z">
        ///<param name="w">
        public void Set(FP x, FP y, FP z, FP w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        /// <summary>
        ///构造函数初始化结构的新实例
        /// </summary>
        ///xyz</param>
        public TSVector4(FP xyzw)
        {
            x = xyzw;
            y = xyzw;
            z = xyzw;
            w = xyzw;
        }

        public static TSVector4 Lerp(TSVector4 from, TSVector4 to, FP percent)
        {
            return from + (to - from) * percent;
        }

        /// <summary>
        ///从JVector第一章
        /// </summary>
        ///<returns>返回
        #region public override string ToString()
        public override string ToString()
        {
            return string.Format("({0:f1}, {1:f1}, {2:f1}, {3:f1})", x.AsFloat(), y.AsFloat(), z.AsFloat(), w.AsFloat());
        }
        #endregion

        /// <summary>
        ///测试对象是否等于此向量。
        /// </summary>
        ///<param name="obj">回报率
        ///<returns>
        #region public override bool Equals(object obj)
        public override bool Equals(object obj)
        {
            if (!(obj is TSVector4)) return false;
            TSVector4 other = (TSVector4)obj;

            return (((x == other.x) && (y == other.y)) && (z == other.z) && (w == other.w));
        }
        #endregion

        /// <summary>
        ///将向量的每个分量乘以所提供向量的相同分量。
        /// </summary>
        public static TSVector4 Scale(TSVector4 vecA, TSVector4 vecB)
        {
            TSVector4 result;
            result.x = vecA.x * vecB.x;
            result.y = vecA.y * vecB.y;
            result.z = vecA.z * vecB.z;
            result.w = vecA.w * vecB.w;

            return result;
        }

        /// <summary>
        ///试两个JVector retired是componedi等
        /// </summary>
        ///<param name="value1">一级。</param>
        ///<param name="value2">
        ///<returns>
        #region public static bool operator ==(JVector value1, JVector value2)
        public static bool operator ==(TSVector4 value1, TSVector4 value2)
        {
            return (((value1.x == value2.x) && (value1.y == value2.y)) && (value1.z == value2.z) && (value1.w == value2.w));
        }
        #endregion

        /// <summary>
        ///试两个JVector return是不coordinable等
        /// </summary>
        ///<param name="value1">一级。</param>
        ///<param name="value2">
        ///<returns>
        #region public static bool operator !=(JVector value1, JVector value2)
        public static bool operator !=(TSVector4 value1, TSVector4 value2)
        {
            if ((value1.x == value2.x) && (value1.y == value2.y) && (value1.z == value2.z))
            {
                return (value1.w != value2.w);
            }
            return true;
        }
        #endregion

        /// <summary>
        ///x、y z
        /// </summary>
        ///<param name="value1">一级。</param>
        ///<param name="value2">
        ///返回向量的最小值
        #region public static JVector Min(JVector value1, JVector value2)

        public static TSVector4 Min(TSVector4 value1, TSVector4 value2)
        {
            TSVector4.Min(ref value1, ref value2, out TSVector4 result);
            return result;
        }

        /// <summary>
        ///x、y z
        /// </summary>
        ///<param name="value1">一级。</param>
        ///<param name="value2">
        ///<param name="result">
        public static void Min(ref TSVector4 value1, ref TSVector4 value2, out TSVector4 result)
        {
            result.x = (value1.x < value2.x) ? value1.x : value2.x;
            result.y = (value1.y < value2.y) ? value1.y : value2.y;
            result.z = (value1.z < value2.z) ? value1.z : value2.z;
            result.w = (value1.w < value2.w) ? value1.w : value2.w;
        }
        #endregion

        /// <summary>
        ///x、y、z
        /// </summary>
        ///<param name="value1">一级。</param>
        ///<param name="value2">
        ///返回
        #region public static JVector Max(JVector value1, JVector value2)
        public static TSVector4 Max(TSVector4 value1, TSVector4 value2)
        {
            TSVector4.Max(ref value1, ref value2, out TSVector4 result);
            return result;
        }

        public static FP Distance(TSVector4 v1, TSVector4 v2)
        {
            return FP.Sqrt((v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y) + (v1.z - v2.z) * (v1.z - v2.z) + (v1.w - v2.w) * (v1.w - v2.w));
        }

        /// <summary>
        ///x、y、z
        /// </summary>
        ///<param name="value1">一级。</param>
        ///<param name="value2">
        ///<param name="result">
        public static void Max(ref TSVector4 value1, ref TSVector4 value2, out TSVector4 result)
        {
            result.x = (value1.x > value2.x) ? value1.x : value2.x;
            result.y = (value1.y > value2.y) ? value1.y : value2.y;
            result.z = (value1.z > value2.z) ? value1.z : value2.z;
            result.w = (value1.w > value2.w) ? value1.w : value2.w;
        }
        #endregion

        /// <summary>
        ///将向量的长度设置为零。
        /// </summary>
        #region public void MakeZero()
        public void MakeZero()
        {
            x = FP.Zero;
            y = FP.Zero;
            z = FP.Zero;
            w = FP.Zero;
        }
        #endregion

        /// <summary>
        ///检查矢量的长度是否为零。
        /// </summary>
        ///<returns>
        #region public bool IsZero()
        public bool IsZero()
        {
            return (sqrMagnitude == FP.Zero);
        }

        /// <summary>
        ///检查矢量的长度是否接近零。
        /// </summary>
        ///<returns>
        public bool IsNearlyZero()
        {
            return (sqrMagnitude < ZeroEpsilonSq);
        }
        #endregion

        /// <summary>
        ///用给定的矩阵变换向量。
        /// </summary>
        ///<param name="position">
        ///<param name="matrix">
        ///<returns>返回
        #region public static JVector Transform(JVector position, JMatrix matrix)
        public static TSVector4 Transform(TSVector4 position, TSMatrix4 matrix)
        {
            TSVector4.Transform(ref position, ref matrix, out TSVector4 result);
            return result;
        }

        public static TSVector4 Transform(TSVector3 position, TSMatrix4 matrix)
        {
            TSVector4.Transform(ref position, ref matrix, out TSVector4 result);
            return result;
        }

        /// <summary>
        ///用给定的矩阵变换向量。
        /// </summary>
        ///<param name="vector">
        ///<param name="matrix">
        ///<param name="result">
        public static void Transform(ref TSVector3 vector, ref TSMatrix4 matrix, out TSVector4 result)
        {
            result.x = vector.x * matrix.M11 + vector.y * matrix.M12 + vector.z * matrix.M13 + matrix.M14;
            result.y = vector.x * matrix.M21 + vector.y * matrix.M22 + vector.z * matrix.M23 + matrix.M24;
            result.z = vector.x * matrix.M31 + vector.y * matrix.M32 + vector.z * matrix.M33 + matrix.M34;
            result.w = vector.x * matrix.M41 + vector.y * matrix.M42 + vector.z * matrix.M43 + matrix.M44;
        }

        public static void Transform(ref TSVector4 vector, ref TSMatrix4 matrix, out TSVector4 result)
        {
            result.x = vector.x * matrix.M11 + vector.y * matrix.M12 + vector.z * matrix.M13 + vector.w * matrix.M14;
            result.y = vector.x * matrix.M21 + vector.y * matrix.M22 + vector.z * matrix.M23 + vector.w * matrix.M24;
            result.z = vector.x * matrix.M31 + vector.y * matrix.M32 + vector.z * matrix.M33 + vector.w * matrix.M34;
            result.w = vector.x * matrix.M41 + vector.y * matrix.M42 + vector.z * matrix.M43 + vector.w * matrix.M44;
        }
        #endregion

        /// <summary>
        ///计算两个向量的点积。
        /// </summary>
        ///<param name="vector1">
        ///΋
        ///<returns>
        #region public static FP Dot(JVector vector1, JVector vector2)
        public static FP Dot(TSVector4 vector1, TSVector4 vector2)
        {
            return TSVector4.Dot(ref vector1, ref vector2);
        }


        /// <summary>
        ///计算两个向量的点积。
        /// </summary>
        ///<param name="vector1">
        ///΋
        ///<returns>
        public static FP Dot(ref TSVector4 vector1, ref TSVector4 vector2)
        {
            return ((vector1.x * vector2.x) + (vector1.y * vector2.y)) + (vector1.z * vector2.z) + (vector1.w * vector2.w);
        }
        #endregion

        /// <summary>
        ///添加两个向量。
        /// </summary>
        ///<param name="value1">
        ///<param name="value2">
        ///返回
        #region public static void Add(JVector value1, JVector value2)
        public static TSVector4 Add(TSVector4 value1, TSVector4 value2)
        {
            TSVector4.Add(ref value1, ref value2, out TSVector4 result);
            return result;
        }

        /// <summary>
        ///添加到向量。
        /// </summary>
        ///<param name="value1">
        ///<param name="value2">
        ///<param name="result">
        public static void Add(ref TSVector4 value1, ref TSVector4 value2, out TSVector4 result)
        {
            result.x = value1.x + value2.x;
            result.y = value1.y + value2.y;
            result.z = value1.z + value2.z;
            result.w = value1.w + value2.w;
        }
        #endregion

        /// <summary>
        ///将向量除以因子。
        /// </summary>
        ///<param name="value1">
        ///<param name="scaleFactor">
        ///<returns>
        public static TSVector4 Divide(TSVector4 value1, FP scaleFactor)
        {
            TSVector4.Divide(ref value1, scaleFactor, out TSVector4 result);
            return result;
        }

        /// <summary>
        ///将向量除以因子。
        /// </summary>
        ///<param name="value1">
        ///<param name="scaleFactor">
        ///Ň
        public static void Divide(ref TSVector4 value1, FP scaleFactor, out TSVector4 result)
        {
            result.x = value1.x / scaleFactor;
            result.y = value1.y / scaleFactor;
            result.z = value1.z / scaleFactor;
            result.w = value1.w / scaleFactor;
        }

        /// <summary>
        ///减去两个向量。
        /// </summary>
        ///<param name="value1">
        ///<param name="value2">
        ///<returns>
        #region public static JVector Subtract(JVector value1, JVector value2)
        public static TSVector4 Subtract(TSVector4 value1, TSVector4 value2)
        {
            TSVector4.Subtract(ref value1, ref value2, out TSVector4 result);
            return result;
        }

        /// <summary>
        ///减去向量。
        /// </summary>
        ///<param name="value1">
        ///<param name="value2">
        ///<param name="result">
        public static void Subtract(ref TSVector4 value1, ref TSVector4 value2, out TSVector4 result)
        {
            result.x = value1.x - value2.x;
            result.y = value1.y - value2.y;
            result.z = value1.z - value2.z;
            result.w = value1.w - value2.w;
        }
        #endregion

        /// <summary>
        ///获取向量的哈希代码。
        /// </summary>
        ///<returns>
        #region public override int GetHashCode()
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode() ^ w.GetHashCode();
        }
        #endregion

        /// <summary>
        ///反转矢量的方向。
        /// </summary>
        #region public static JVector Negate(JVector value)
        public void Negate()
        {
            x = -x;
            y = -y;
            z = -z;
            w = -w;
        }

        /// <summary>
        ///反转向量的方向。
        /// </summary>
        ///<param name="value">
        ///<returns>
        public static TSVector4 Negate(TSVector4 value)
        {
            TSVector4.Negate(ref value, out TSVector4 result);
            return result;
        }

        /// <summary>
        ///反转向量的方向。
        /// </summary>
        ///<param name="value">
        ///<param name="result">
        public static void Negate(ref TSVector4 value, out TSVector4 result)
        {
            result.x = -value.x;
            result.y = -value.y;
            result.z = -value.z;
            result.w = -value.w;
        }
        #endregion

        /// <summary>
        ///规范化给定向量。
        /// </summary>
        ///<param name="value">
        ///<returns>
        #region public static JVector Normalize(JVector value)
        public static TSVector4 Normalize(TSVector4 value)
        {
            TSVector4.Normalize(ref value, out TSVector4 result);
            return result;
        }

        /// <summary>
        ///规范化此向量。
        /// </summary>
        public void Normalize()
        {
            FP num2 = ((x * x) + (y * y)) + (z * z) + (w * w);
            FP num = FP.One / FP.Sqrt(num2);
            x *= num;
            y *= num;
            z *= num;
            w *= num;
        }

        /// <summary>
        ///规范化给定向量。
        /// </summary>
        ///<param name="value">
        ///<param name="result">
        public static void Normalize(ref TSVector4 value, out TSVector4 result)
        {
            FP num2 = ((value.x * value.x) + (value.y * value.y)) + (value.z * value.z) + (value.w * value.w);
            FP num = FP.One / FP.Sqrt(num2);
            result.x = value.x * num;
            result.y = value.y * num;
            result.z = value.z * num;
            result.w = value.w * num;
        }
        #endregion

        #region public static void Swap(ref JVector vector1, ref JVector vector2)

        /// <summary>
        ///交换两个向量的组件。
        /// </summary>
        ///<param name="vector1">
        ///<param name="vector2">
        public static void Swap(ref TSVector4 vector1, ref TSVector4 vector2)
        {
            FP temp;

            temp = vector1.x;
            vector1.x = vector2.x;
            vector2.x = temp;

            temp = vector1.y;
            vector1.y = vector2.y;
            vector2.y = temp;

            temp = vector1.z;
            vector1.z = vector2.z;
            vector2.z = temp;

            temp = vector1.w;
            vector1.w = vector2.w;
            vector2.w = temp;
        }
        #endregion

        /// <summary>
        ///用一个因子乘以一个向量。
        /// </summary>
        ///<param name="value1">
        ///<param name="scaleFactor">
        ///<returns>
        #region public static JVector Multiply(JVector value1, FP scaleFactor)
        public static TSVector4 Multiply(TSVector4 value1, FP scaleFactor)
        {
            TSVector4.Multiply(ref value1, scaleFactor, out TSVector4 result);
            return result;
        }

        /// <summary>
        ///用一个因子乘以一个向量。
        /// </summary>
        ///<param name="value1">
        ///<param name="scaleFactor">
        ///<param name="result">
        public static void Multiply(ref TSVector4 value1, FP scaleFactor, out TSVector4 result)
        {
            result.x = value1.x * scaleFactor;
            result.y = value1.y * scaleFactor;
            result.z = value1.z * scaleFactor;
            result.w = value1.w * scaleFactor;
        }
        #endregion

        /// <summary>
        ///计算两个向量的点积。
        /// </summary>
        ///<param name="value1">
        ///<param name="value2">
        ///返回
        #region public static FP operator *(JVector value1, JVector value2)
        public static FP operator *(TSVector4 value1, TSVector4 value2)
        {
            return TSVector4.Dot(ref value1, ref value2);
        }
        #endregion

        /// <summary>
        ///将矢量乘以比例因子。
        /// </summary>
        ///<param name="value1">
        ///<param name="value2">
        ///<returns>
        #region public static JVector operator *(JVector value1, FP value2)
        public static TSVector4 operator *(TSVector4 value1, FP value2)
        {
            TSVector4.Multiply(ref value1, value2, out TSVector4 result);
            return result;
        }
        #endregion

        /// <summary>
        ///将矢量乘以比例因子。
        /// </summary>
        ///<param name="value2">
        ///<param name="value1">
        ///<returns>
        #region public static JVector operator *(FP value1, JVector value2)
        public static TSVector4 operator *(FP value1, TSVector4 value2)
        {
            TSVector4.Multiply(ref value2, value1, out TSVector4 result);
            return result;
        }
        #endregion

        /// <summary>
        ///减去两个向量。
        /// </summary>
        ///<param name="value1">
        ///<param name="value2">
        ///<returns>
        #region public static JVector operator -(JVector value1, JVector value2)
        public static TSVector4 operator -(TSVector4 value1, TSVector4 value2)
        {
            TSVector4.Subtract(ref value1, ref value2, out TSVector4 result);
            return result;
        }
        #endregion

        /// <summary>
        ///添加两个向量。
        /// </summary>
        ///<param name="value1">
        ///<param name="value2">
        ///返回
        #region public static JVector operator +(JVector value1, JVector value2)
        public static TSVector4 operator +(TSVector4 value1, TSVector4 value2)
        {
            TSVector4.Add(ref value1, ref value2, out TSVector4 result);
            return result;
        }
        #endregion

        /// <summary>
        ///将向量除以因子。
        /// </summary>
        ///<param name="value1">
        ///<param name="scaleFactor">
        ///<returns>
        public static TSVector4 operator /(TSVector4 value1, FP value2)
        {
            TSVector4.Divide(ref value1, value2, out TSVector4 result);
            return result;
        }

        public TSVector2 ToTSVector2()
        {
            return new TSVector2(x, y);
        }

        public TSVector3 ToTSVector()
        {
            return new TSVector3(x, y, z);
        }
    }

}