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
    ///3x3矩阵
    /// </summary>
    public struct TSMatrix3
    {
        /// <summary>
        ///M11
        /// </summary>
        public FP M11; //第一行向量
        /// <summary>
        ///M12
        /// </summary>
        public FP M12;
        /// <summary>
        ///M13
        /// </summary>
        public FP M13;
        /// <summary>
        ///M21
        /// </summary>
        public FP M21; //第二行向量
        /// <summary>
        ///M22英寸
        /// </summary>
        public FP M22;
        /// <summary>
        ///第23页
        /// </summary>
        public FP M23;
        /// <summary>
        ///M31型
        /// </summary>
        public FP M31; //第三行向量
        /// <summary>
        ///M32型
        /// </summary>
        public FP M32;
        /// <summary>
        ///M33型
        /// </summary>
        public FP M33;

        internal static TSMatrix3 InternalIdentity;

        /// <summary>
        ///单位矩阵。
        /// </summary>
        public static readonly TSMatrix3 Identity;
        public static readonly TSMatrix3 Zero;

        static TSMatrix3()
        {
            Zero = new TSMatrix3();

            Identity = new TSMatrix3();
            Identity.M11 = FP.One;
            Identity.M22 = FP.One;
            Identity.M33 = FP.One;

            InternalIdentity = Identity;
        }

        public TSVector3 eulerAngles
        {
            get
            {
                TSVector3 result = new TSVector3();

                result.x = TSMathf.Atan2(M32, M33) * FP.Rad2Deg;
                result.y = TSMathf.Atan2(-M31, TSMathf.Sqrt(M32 * M32 + M33 * M33)) * FP.Rad2Deg;
                result.z = TSMathf.Atan2(M21, M11) * FP.Rad2Deg;

                return result * -1;
            }
        }

        public static TSMatrix3 CreateFromYawPitchRoll(FP yaw, FP pitch, FP roll)
        {
            TSQuaternion.CreateFromYawPitchRoll(yaw, pitch, roll, out TSQuaternion quaternion);
            CreateFromQuaternion(ref quaternion, out TSMatrix3 matrix);
            return matrix;
        }

        public static TSMatrix3 CreateRotationX(FP radians)
        {
            TSMatrix3 matrix;
            FP num2 = FP.Cos(radians);
            FP num = FP.Sin(radians);
            matrix.M11 = FP.One;
            matrix.M12 = FP.Zero;
            matrix.M13 = FP.Zero;
            matrix.M21 = FP.Zero;
            matrix.M22 = num2;
            matrix.M23 = num;
            matrix.M31 = FP.Zero;
            matrix.M32 = -num;
            matrix.M33 = num2;
            return matrix;
        }

        public static void CreateRotationX(FP radians, out TSMatrix3 result)
        {
            FP num2 = FP.Cos(radians);
            FP num = FP.Sin(radians);
            result.M11 = FP.One;
            result.M12 = FP.Zero;
            result.M13 = FP.Zero;
            result.M21 = FP.Zero;
            result.M22 = num2;
            result.M23 = num;
            result.M31 = FP.Zero;
            result.M32 = -num;
            result.M33 = num2;
        }

        public static TSMatrix3 CreateRotationY(FP radians)
        {
            TSMatrix3 matrix;
            FP num2 = FP.Cos(radians);
            FP num = FP.Sin(radians);
            matrix.M11 = num2;
            matrix.M12 = FP.Zero;
            matrix.M13 = -num;
            matrix.M21 = FP.Zero;
            matrix.M22 = FP.One;
            matrix.M23 = FP.Zero;
            matrix.M31 = num;
            matrix.M32 = FP.Zero;
            matrix.M33 = num2;
            return matrix;
        }

        public static void CreateRotationY(FP radians, out TSMatrix3 result)
        {
            FP num2 = FP.Cos(radians);
            FP num = FP.Sin(radians);
            result.M11 = num2;
            result.M12 = FP.Zero;
            result.M13 = -num;
            result.M21 = FP.Zero;
            result.M22 = FP.One;
            result.M23 = FP.Zero;
            result.M31 = num;
            result.M32 = FP.Zero;
            result.M33 = num2;
        }

        public static TSMatrix3 CreateRotationZ(FP radians)
        {
            TSMatrix3 matrix;
            FP num2 = FP.Cos(radians);
            FP num = FP.Sin(radians);
            matrix.M11 = num2;
            matrix.M12 = num;
            matrix.M13 = FP.Zero;
            matrix.M21 = -num;
            matrix.M22 = num2;
            matrix.M23 = FP.Zero;
            matrix.M31 = FP.Zero;
            matrix.M32 = FP.Zero;
            matrix.M33 = FP.One;
            return matrix;
        }


        public static void CreateRotationZ(FP radians, out TSMatrix3 result)
        {
            FP num2 = FP.Cos(radians);
            FP num = FP.Sin(radians);
            result.M11 = num2;
            result.M12 = num;
            result.M13 = FP.Zero;
            result.M21 = -num;
            result.M22 = num2;
            result.M23 = FP.Zero;
            result.M31 = FP.Zero;
            result.M32 = FP.Zero;
            result.M33 = FP.One;
        }

        /// <summary>
        ///初始化矩阵结构的新实例。
        /// </summary>
        ///<param name="m11">m11</param>
        ///<param name="m12">m12</param>
        ///<param name="m13">m13</param>
        ///<param name="m21">m21</param>
        ///<param name="m22">m22</param>
        ///<param name="m23">m23</param>
        ///<param name="m31">m31</param>
        ///<param name="m32">m32</param>
        ///<param name="m33">m33</param>
        #region public JMatrix(FP m11, FP m12, FP m13, FP m21, FP m22, FP m23,FP m31, FP m32, FP m33)
        public TSMatrix3(FP m11, FP m12, FP m13, FP m21, FP m22, FP m23, FP m31, FP m32, FP m33)
        {
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M21 = m21;
            M22 = m22;
            M23 = m23;
            M31 = m31;
            M32 = m32;
            M33 = m33;
        }
        #endregion

        /// <summary>
        ///获取矩阵的行列式。
        /// </summary>
        ///方法
        #region public FP Determinant()
        //金融资产负债表
        //{
        //返回M11*M22*M33-M11*M23*M32-M12*M21*M33+M12*M23*M31+M13*M21*M32-M13*M22*M31
        //}
        #endregion

        /// <summary>
        ///将两个矩阵相乘。注意：矩阵乘法不是交换的。
        /// </summary>
        ///<param name="matrix1">
        ///<param name="matrix2">
        ///<returns>
        #region public static JMatrix Multiply(JMatrix matrix1, JMatrix matrix2)
        public static TSMatrix3 Multiply(TSMatrix3 matrix1, TSMatrix3 matrix2)
        {
            TSMatrix3.Multiply(ref matrix1, ref matrix2, out TSMatrix3 result);
            return result;
        }

        /// <summary>
        ///将两个矩阵相乘。注意：矩阵乘法不是交换的。
        /// </summary>
        ///<param name="matrix1">
        ///<param name="matrix2">
        ///<param name="result">
        public static void Multiply(ref TSMatrix3 matrix1, ref TSMatrix3 matrix2, out TSMatrix3 result)
        {
            FP num0 = ((matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21)) + (matrix1.M13 * matrix2.M31);
            FP num1 = ((matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22)) + (matrix1.M13 * matrix2.M32);
            FP num2 = ((matrix1.M11 * matrix2.M13) + (matrix1.M12 * matrix2.M23)) + (matrix1.M13 * matrix2.M33);
            FP num3 = ((matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21)) + (matrix1.M23 * matrix2.M31);
            FP num4 = ((matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22)) + (matrix1.M23 * matrix2.M32);
            FP num5 = ((matrix1.M21 * matrix2.M13) + (matrix1.M22 * matrix2.M23)) + (matrix1.M23 * matrix2.M33);
            FP num6 = ((matrix1.M31 * matrix2.M11) + (matrix1.M32 * matrix2.M21)) + (matrix1.M33 * matrix2.M31);
            FP num7 = ((matrix1.M31 * matrix2.M12) + (matrix1.M32 * matrix2.M22)) + (matrix1.M33 * matrix2.M32);
            FP num8 = ((matrix1.M31 * matrix2.M13) + (matrix1.M32 * matrix2.M23)) + (matrix1.M33 * matrix2.M33);

            result.M11 = num0;
            result.M12 = num1;
            result.M13 = num2;
            result.M21 = num3;
            result.M22 = num4;
            result.M23 = num5;
            result.M31 = num6;
            result.M32 = num7;
            result.M33 = num8;
        }
        #endregion

        /// <summary>
        ///添加矩阵。
        /// </summary>
        ///<param name="matrix1">
        ///<param name="matrix2">
        ///返回
        #region public static JMatrix Add(JMatrix matrix1, JMatrix matrix2)
        public static TSMatrix3 Add(TSMatrix3 matrix1, TSMatrix3 matrix2)
        {
            TSMatrix3.Add(ref matrix1, ref matrix2, out TSMatrix3 result);
            return result;
        }

        /// <summary>
        ///添加矩阵。
        /// </summary>
        ///<param name="matrix1">
        ///<param name="matrix2">
        ///<param name="result">
        public static void Add(ref TSMatrix3 matrix1, ref TSMatrix3 matrix2, out TSMatrix3 result)
        {
            result.M11 = matrix1.M11 + matrix2.M11;
            result.M12 = matrix1.M12 + matrix2.M12;
            result.M13 = matrix1.M13 + matrix2.M13;
            result.M21 = matrix1.M21 + matrix2.M21;
            result.M22 = matrix1.M22 + matrix2.M22;
            result.M23 = matrix1.M23 + matrix2.M23;
            result.M31 = matrix1.M31 + matrix2.M31;
            result.M32 = matrix1.M32 + matrix2.M32;
            result.M33 = matrix1.M33 + matrix2.M33;
        }
        #endregion

        /// <summary>
        ///计算给定矩阵的逆矩阵。
        /// </summary>
        ///<param name="matrix">
        ///返回
        #region public static JMatrix Inverse(JMatrix matrix)
        public static TSMatrix3 Inverse(TSMatrix3 matrix)
        {
            TSMatrix3.Inverse(ref matrix, out TSMatrix3 result);
            return result;
        }

        public FP Determinant()
        {
            return M11 * M22 * M33 + M12 * M23 * M31 + M13 * M21 * M32 -
                   M31 * M22 * M13 - M32 * M23 * M11 - M33 * M21 * M12;
        }

        public static void Invert(ref TSMatrix3 matrix, out TSMatrix3 result)
        {
            FP determinantInverse = 1 / matrix.Determinant();
            FP m11 = (matrix.M22 * matrix.M33 - matrix.M23 * matrix.M32) * determinantInverse;
            FP m12 = (matrix.M13 * matrix.M32 - matrix.M33 * matrix.M12) * determinantInverse;
            FP m13 = (matrix.M12 * matrix.M23 - matrix.M22 * matrix.M13) * determinantInverse;

            FP m21 = (matrix.M23 * matrix.M31 - matrix.M21 * matrix.M33) * determinantInverse;
            FP m22 = (matrix.M11 * matrix.M33 - matrix.M13 * matrix.M31) * determinantInverse;
            FP m23 = (matrix.M13 * matrix.M21 - matrix.M11 * matrix.M23) * determinantInverse;

            FP m31 = (matrix.M21 * matrix.M32 - matrix.M22 * matrix.M31) * determinantInverse;
            FP m32 = (matrix.M12 * matrix.M31 - matrix.M11 * matrix.M32) * determinantInverse;
            FP m33 = (matrix.M11 * matrix.M22 - matrix.M12 * matrix.M21) * determinantInverse;

            result.M11 = m11;
            result.M12 = m12;
            result.M13 = m13;

            result.M21 = m21;
            result.M22 = m22;
            result.M23 = m23;

            result.M31 = m31;
            result.M32 = m32;
            result.M33 = m33;
        }

        /// <summary>
        ///计算给定矩阵的逆矩阵。
        /// </summary>
        ///<param name="matrix">
        ///<param name="result">JMatrix的
        public static void Inverse(ref TSMatrix3 matrix, out TSMatrix3 result)
        {
            FP det = 1024 * matrix.M11 * matrix.M22 * matrix.M33 -
                1024 * matrix.M11 * matrix.M23 * matrix.M32 -
                1024 * matrix.M12 * matrix.M21 * matrix.M33 +
                1024 * matrix.M12 * matrix.M23 * matrix.M31 +
                1024 * matrix.M13 * matrix.M21 * matrix.M32 -
                1024 * matrix.M13 * matrix.M22 * matrix.M31;

            FP num11 = 1024 * matrix.M22 * matrix.M33 - 1024 * matrix.M23 * matrix.M32;
            FP num12 = 1024 * matrix.M13 * matrix.M32 - 1024 * matrix.M12 * matrix.M33;
            FP num13 = 1024 * matrix.M12 * matrix.M23 - 1024 * matrix.M22 * matrix.M13;

            FP num21 = 1024 * matrix.M23 * matrix.M31 - 1024 * matrix.M33 * matrix.M21;
            FP num22 = 1024 * matrix.M11 * matrix.M33 - 1024 * matrix.M31 * matrix.M13;
            FP num23 = 1024 * matrix.M13 * matrix.M21 - 1024 * matrix.M23 * matrix.M11;

            FP num31 = 1024 * matrix.M21 * matrix.M32 - 1024 * matrix.M31 * matrix.M22;
            FP num32 = 1024 * matrix.M12 * matrix.M31 - 1024 * matrix.M32 * matrix.M11;
            FP num33 = 1024 * matrix.M11 * matrix.M22 - 1024 * matrix.M21 * matrix.M12;

            if (det == 0)
            {
                result.M11 = FP.PositiveInfinity;
                result.M12 = FP.PositiveInfinity;
                result.M13 = FP.PositiveInfinity;
                result.M21 = FP.PositiveInfinity;
                result.M22 = FP.PositiveInfinity;
                result.M23 = FP.PositiveInfinity;
                result.M31 = FP.PositiveInfinity;
                result.M32 = FP.PositiveInfinity;
                result.M33 = FP.PositiveInfinity;
            }
            else
            {
                result.M11 = num11 / det;
                result.M12 = num12 / det;
                result.M13 = num13 / det;
                result.M21 = num21 / det;
                result.M22 = num22 / det;
                result.M23 = num23 / det;
                result.M31 = num31 / det;
                result.M32 = num32 / det;
                result.M33 = num33 / det;
            }

        }
        #endregion

        /// <summary>
        ///将矩阵乘以比例因子。
        /// </summary>
        ///<param name="matrix1">
        ///<param name="scaleFactor">
        ///<返回>JMatrix
        #region public static JMatrix Multiply(JMatrix matrix1, FP scaleFactor)
        public static TSMatrix3 Multiply(TSMatrix3 matrix1, FP scaleFactor)
        {
            TSMatrix3.Multiply(ref matrix1, scaleFactor, out TSMatrix3 result);
            return result;
        }

        /// <summary>
        ///将矩阵乘以比例因子。
        /// </summary>
        ///<param name="matrix1">
        ///<param name="scaleFactor">
        ///<param name="result">JMatrix公司
        public static void Multiply(ref TSMatrix3 matrix1, FP scaleFactor, out TSMatrix3 result)
        {
            FP num = scaleFactor;
            result.M11 = matrix1.M11 * num;
            result.M12 = matrix1.M12 * num;
            result.M13 = matrix1.M13 * num;
            result.M21 = matrix1.M21 * num;
            result.M22 = matrix1.M22 * num;
            result.M23 = matrix1.M23 * num;
            result.M31 = matrix1.M31 * num;
            result.M32 = matrix1.M32 * num;
            result.M33 = matrix1.M33 * num;
        }
        #endregion

        /// <summary>
        ///马蒂克斯
        /// </summary>
        ///<param name="quaternion">公司
        ///<returns>JMatrix公司。</returns>
        #region public static JMatrix CreateFromQuaternion(JQuaternion quaternion)

        public static TSMatrix3 CreateFromLookAt(TSVector3 position, TSVector3 target)
        {
            LookAt(target - position, TSVector3.up, out TSMatrix3 result);
            return result;
        }

        public static TSMatrix3 LookAt(TSVector3 forward, TSVector3 upwards)
        {
            LookAt(forward, upwards, out TSMatrix3 result);

            return result;
        }

        public static void LookAt(TSVector3 forward, TSVector3 upwards, out TSMatrix3 result)
        {
            TSVector3 zaxis = forward; zaxis.Normalize();
            TSVector3 xaxis = TSVector3.Cross(upwards, zaxis); xaxis.Normalize();
            TSVector3 yaxis = TSVector3.Cross(zaxis, xaxis);

            result.M11 = xaxis.x;
            result.M21 = yaxis.x;
            result.M31 = zaxis.x;
            result.M12 = xaxis.y;
            result.M22 = yaxis.y;
            result.M32 = zaxis.y;
            result.M13 = xaxis.z;
            result.M23 = yaxis.z;
            result.M33 = zaxis.z;
        }

        public static TSMatrix3 CreateFromQuaternion(TSQuaternion quaternion)
        {
            CreateFromQuaternion(ref quaternion, out TSMatrix3 result);
            return result;
        }

        /// <summary>
        ///马蒂克斯
        /// </summary>
        ///<param name="quaternion">公司
        ///<param name="result">J矩阵</param>
        public static void CreateFromQuaternion(ref TSQuaternion quaternion, out TSMatrix3 result)
        {
            FP num9 = quaternion.x * quaternion.x;
            FP num8 = quaternion.y * quaternion.y;
            FP num7 = quaternion.z * quaternion.z;
            FP num6 = quaternion.x * quaternion.y;
            FP num5 = quaternion.z * quaternion.w;
            FP num4 = quaternion.z * quaternion.x;
            FP num3 = quaternion.y * quaternion.w;
            FP num2 = quaternion.y * quaternion.z;
            FP num = quaternion.x * quaternion.w;
            result.M11 = FP.One - (2 * (num8 + num7));
            result.M12 = 2 * (num6 + num5);
            result.M13 = 2 * (num4 - num3);
            result.M21 = 2 * (num6 - num5);
            result.M22 = FP.One - (2 * (num7 + num9));
            result.M23 = 2 * (num2 + num);
            result.M31 = 2 * (num4 + num3);
            result.M32 = 2 * (num2 - num);
            result.M33 = FP.One - (2 * (num8 + num9));
        }
        #endregion

        /// <summary>
        ///创建转置矩阵。
        /// </summary>
        ///<param name="matrix">
        ///<returns>JMatrix.</returns>
        #region public static JMatrix Transpose(JMatrix matrix)
        public static TSMatrix3 Transpose(TSMatrix3 matrix)
        {
            TSMatrix3.Transpose(ref matrix, out TSMatrix3 result);
            return result;
        }

        /// <summary>
        ///创建转置矩阵。
        /// </summary>
        ///<param name="matrix">
        ///</param>
        public static void Transpose(ref TSMatrix3 matrix, out TSMatrix3 result)
        {
            result.M11 = matrix.M11;
            result.M12 = matrix.M21;
            result.M13 = matrix.M31;
            result.M21 = matrix.M12;
            result.M22 = matrix.M22;
            result.M23 = matrix.M32;
            result.M31 = matrix.M13;
            result.M32 = matrix.M23;
            result.M33 = matrix.M33;
        }
        #endregion

        /// <summary>
        ///将两个矩阵相乘。
        /// </summary>
        ///<param name="value1">
        ///<param name="value2">
        ///<returns>
        #region public static JMatrix operator *(JMatrix value1,JMatrix value2)
        public static TSMatrix3 operator *(TSMatrix3 value1, TSMatrix3 value2)
        {
            TSMatrix3.Multiply(ref value1, ref value2, out TSMatrix3 result);
            return result;
        }
        #endregion


        public FP Trace()
        {
            return M11 + M22 + M33;
        }

        /// <summary>
        ///将两个矩阵相加。
        /// </summary>
        ///<param name="value1">
        ///<param name="value2">
        ///返回
        #region public static JMatrix operator +(JMatrix value1, JMatrix value2)
        public static TSMatrix3 operator +(TSMatrix3 value1, TSMatrix3 value2)
        {
            TSMatrix3.Add(ref value1, ref value2, out TSMatrix3 result);
            return result;
        }
        #endregion

        /// <summary>
        ///减去两个矩阵。
        /// </summary>
        ///<param name="value1">
        ///<param name="value2">
        ///返回
        #region public static JMatrix operator -(JMatrix value1, JMatrix value2)
        public static TSMatrix3 operator -(TSMatrix3 value1, TSMatrix3 value2)
        {
            TSMatrix3.Multiply(ref value2, -FP.One, out value2);
            TSMatrix3.Add(ref value1, ref value2, out TSMatrix3 result);
            return result;
        }
        #endregion

        public static bool operator ==(TSMatrix3 value1, TSMatrix3 value2)
        {
            return value1.M11 == value2.M11 &&
                value1.M12 == value2.M12 &&
                value1.M13 == value2.M13 &&
                value1.M21 == value2.M21 &&
                value1.M22 == value2.M22 &&
                value1.M23 == value2.M23 &&
                value1.M31 == value2.M31 &&
                value1.M32 == value2.M32 &&
                value1.M33 == value2.M33;
        }

        public static bool operator !=(TSMatrix3 value1, TSMatrix3 value2)
        {
            return value1.M11 != value2.M11 ||
                value1.M12 != value2.M12 ||
                value1.M13 != value2.M13 ||
                value1.M21 != value2.M21 ||
                value1.M22 != value2.M22 ||
                value1.M23 != value2.M23 ||
                value1.M31 != value2.M31 ||
                value1.M32 != value2.M32 ||
                value1.M33 != value2.M33;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TSMatrix3)) return false;
            TSMatrix3 other = (TSMatrix3)obj;

            return M11 == other.M11 &&
                M12 == other.M12 &&
                M13 == other.M13 &&
                M21 == other.M21 &&
                M22 == other.M22 &&
                M23 == other.M23 &&
                M31 == other.M31 &&
                M32 == other.M32 &&
                M33 == other.M33;
        }

        public override int GetHashCode()
        {
            return M11.GetHashCode() ^
                M12.GetHashCode() ^
                M13.GetHashCode() ^
                M21.GetHashCode() ^
                M22.GetHashCode() ^
                M23.GetHashCode() ^
                M31.GetHashCode() ^
                M32.GetHashCode() ^
                M33.GetHashCode();
        }

        /// <summary>
        ///创建绕给定轴旋转给定角度的矩阵。
        /// </summary>
        ///<param name="axis">毛边。</param>
        ///<param name="angle">
        ///<param name="result">
        #region public static void CreateFromAxisAngle(ref JVector axis, FP angle, out JMatrix result)
        public static void CreateFromAxisAngle(ref TSVector3 axis, FP angle, out TSMatrix3 result)
        {
            FP x = axis.x;
            FP y = axis.y;
            FP z = axis.z;
            FP num2 = FP.Sin(angle);
            FP num = FP.Cos(angle);
            FP num11 = x * x;
            FP num10 = y * y;
            FP num9 = z * z;
            FP num8 = x * y;
            FP num7 = x * z;
            FP num6 = y * z;
            result.M11 = num11 + (num * (FP.One - num11));
            result.M12 = (num8 - (num * num8)) + (num2 * z);
            result.M13 = (num7 - (num * num7)) - (num2 * y);
            result.M21 = (num8 - (num * num8)) - (num2 * z);
            result.M22 = num10 + (num * (FP.One - num10));
            result.M23 = (num6 - (num * num6)) + (num2 * x);
            result.M31 = (num7 - (num * num7)) + (num2 * y);
            result.M32 = (num6 - (num * num6)) - (num2 * x);
            result.M33 = num9 + (num * (FP.One - num9));
        }

        /// <summary>
        ///创建绕给定轴旋转给定角度的矩阵。
        /// </summary>
        ///<param name="axis">毛边。</param>
        ///<param name="angle">
        ///<returns>返回
        public static TSMatrix3 AngleAxis(FP angle, TSVector3 axis)
        {
            CreateFromAxisAngle(ref axis, angle, out TSMatrix3 result);
            return result;
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}", M11.RawValue, M12.RawValue, M13.RawValue, M21.RawValue, M22.RawValue, M23.RawValue, M31.RawValue, M32.RawValue, M33.RawValue);
        }

    }

}
