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
    public struct TSMatrix4
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
        ///M14型
        /// </summary>
        public FP M14;
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
        ///二十四
        /// </summary>
        public FP M24;
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
        /// <summary>
        ///M34型
        /// </summary>
        public FP M34;
        /// <summary>
        ///M41
        /// </summary>
        public FP M41; //第四行向量
        /// <summary>
        ///M42
        /// </summary>
        public FP M42;
        /// <summary>
        ///M43
        /// </summary>
        public FP M43;
        /// <summary>
        ///M44号
        /// </summary>
        public FP M44;

        internal static TSMatrix4 InternalIdentity;

        /// <summary>
        ///单位矩阵。
        /// </summary>
        public static readonly TSMatrix4 Identity;
        public static readonly TSMatrix4 Zero;

        static TSMatrix4()
        {
            Zero = new TSMatrix4();

            Identity = new TSMatrix4();
            Identity.M11 = FP.One;
            Identity.M22 = FP.One;
            Identity.M33 = FP.One;
            Identity.M44 = FP.One;

            InternalIdentity = Identity;
        }
        /// <summary>
        ///初始化矩阵结构的新实例。
        /// </summary>
        ///<param name="m11">m11</param>
        ///<param name="m12">m12</param>
        ///<param name="m13">m13</param>
        ///<param name="m14">m14</param>
        ///<param name="m21">m21</param>
        ///<param name="m22">m22</param>
        ///<param name="m23">m23</param>
        ///<param name="m24">m24</param>
        ///<param name="m31">m31</param>
        ///<param name="m32">m32</param>
        ///<param name="m33">m33</param>
        ///<param name="m34">m34</param>
        ///<param name="m41">m41</param>
        ///<param name="m42">m42</param>
        ///<param name="m43">m43</param>
        ///<param name="m44">m44</param>
        public TSMatrix4(FP m11, FP m12, FP m13, FP m14,
            FP m21, FP m22, FP m23, FP m24,
            FP m31, FP m32, FP m33, FP m34,
            FP m41, FP m42, FP m43, FP m44)
        {
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M14 = m14;
            M21 = m21;
            M22 = m22;
            M23 = m23;
            M24 = m24;
            M31 = m31;
            M32 = m32;
            M33 = m33;
            M34 = m34;
            M41 = m41;
            M42 = m42;
            M43 = m43;
            M44 = m44;
        }

        /// <summary>
        ///将两个矩阵相乘。注意：矩阵乘法不是交换的。
        /// </summary>
        ///<param name="matrix1">
        ///<param name="matrix2">
        ///<returns>
        public static TSMatrix4 Multiply(TSMatrix4 matrix1, TSMatrix4 matrix2)
        {
            Multiply(ref matrix1, ref matrix2, out TSMatrix4 result);
            return result;
        }

        /// <summary>
        ///将两个矩阵相乘。注意：矩阵乘法不是交换的。
        /// </summary>
        ///<param name="matrix1">
        ///<param name="matrix2">
        ///<param name="result">
        public static void Multiply(ref TSMatrix4 matrix1, ref TSMatrix4 matrix2, out TSMatrix4 result)
        {
            //第一排
            result.M11 = matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21 + matrix1.M13 * matrix2.M31 + matrix1.M14 * matrix2.M41;
            result.M12 = matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22 + matrix1.M13 * matrix2.M32 + matrix1.M14 * matrix2.M42;
            result.M13 = matrix1.M11 * matrix2.M13 + matrix1.M12 * matrix2.M23 + matrix1.M13 * matrix2.M33 + matrix1.M14 * matrix2.M43;
            result.M14 = matrix1.M11 * matrix2.M14 + matrix1.M12 * matrix2.M24 + matrix1.M13 * matrix2.M34 + matrix1.M14 * matrix2.M44;

            //第二排
            result.M21 = matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21 + matrix1.M23 * matrix2.M31 + matrix1.M24 * matrix2.M41;
            result.M22 = matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22 + matrix1.M23 * matrix2.M32 + matrix1.M24 * matrix2.M42;
            result.M23 = matrix1.M21 * matrix2.M13 + matrix1.M22 * matrix2.M23 + matrix1.M23 * matrix2.M33 + matrix1.M24 * matrix2.M43;
            result.M24 = matrix1.M21 * matrix2.M14 + matrix1.M22 * matrix2.M24 + matrix1.M23 * matrix2.M34 + matrix1.M24 * matrix2.M44;

            //第三排
            result.M31 = matrix1.M31 * matrix2.M11 + matrix1.M32 * matrix2.M21 + matrix1.M33 * matrix2.M31 + matrix1.M34 * matrix2.M41;
            result.M32 = matrix1.M31 * matrix2.M12 + matrix1.M32 * matrix2.M22 + matrix1.M33 * matrix2.M32 + matrix1.M34 * matrix2.M42;
            result.M33 = matrix1.M31 * matrix2.M13 + matrix1.M32 * matrix2.M23 + matrix1.M33 * matrix2.M33 + matrix1.M34 * matrix2.M43;
            result.M34 = matrix1.M31 * matrix2.M14 + matrix1.M32 * matrix2.M24 + matrix1.M33 * matrix2.M34 + matrix1.M34 * matrix2.M44;

            //第四排
            result.M41 = matrix1.M41 * matrix2.M11 + matrix1.M42 * matrix2.M21 + matrix1.M43 * matrix2.M31 + matrix1.M44 * matrix2.M41;
            result.M42 = matrix1.M41 * matrix2.M12 + matrix1.M42 * matrix2.M22 + matrix1.M43 * matrix2.M32 + matrix1.M44 * matrix2.M42;
            result.M43 = matrix1.M41 * matrix2.M13 + matrix1.M42 * matrix2.M23 + matrix1.M43 * matrix2.M33 + matrix1.M44 * matrix2.M43;
            result.M44 = matrix1.M41 * matrix2.M14 + matrix1.M42 * matrix2.M24 + matrix1.M43 * matrix2.M34 + matrix1.M44 * matrix2.M44;
        }

        /// <summary>
        ///添加矩阵。
        /// </summary>
        ///<param name="matrix1">
        ///<param name="matrix2">
        ///返回
        public static TSMatrix4 Add(TSMatrix4 matrix1, TSMatrix4 matrix2)
        {
            Add(ref matrix1, ref matrix2, out TSMatrix4 result);
            return result;
        }

        /// <summary>
        ///添加矩阵。
        /// </summary>
        ///<param name="matrix1">
        ///<param name="matrix2">
        ///<param name="result">
        public static void Add(ref TSMatrix4 matrix1, ref TSMatrix4 matrix2, out TSMatrix4 result)
        {
            result.M11 = matrix1.M11 + matrix2.M11;
            result.M12 = matrix1.M12 + matrix2.M12;
            result.M13 = matrix1.M13 + matrix2.M13;
            result.M14 = matrix1.M14 + matrix2.M14;

            result.M21 = matrix1.M21 + matrix2.M21;
            result.M22 = matrix1.M22 + matrix2.M22;
            result.M23 = matrix1.M23 + matrix2.M23;
            result.M24 = matrix1.M24 + matrix2.M24;

            result.M31 = matrix1.M31 + matrix2.M31;
            result.M32 = matrix1.M32 + matrix2.M32;
            result.M33 = matrix1.M33 + matrix2.M33;
            result.M34 = matrix1.M34 + matrix2.M34;

            result.M41 = matrix1.M41 + matrix2.M41;
            result.M42 = matrix1.M42 + matrix2.M42;
            result.M43 = matrix1.M43 + matrix2.M43;
            result.M44 = matrix1.M44 + matrix2.M44;
        }

        /// <summary>
        ///计算给定矩阵的逆矩阵。
        /// </summary>
        ///<param name="matrix">
        ///返回
        public static TSMatrix4 Inverse(TSMatrix4 matrix)
        {
            Inverse(ref matrix, out TSMatrix4 result);
            return result;
        }

        public FP determinant
        {
            get
            {
                //|a b c d | | | | e f h | e f h | e f g g g g g|
                //|例如h |=a | j k l |-b | i k l |+c | i j l |-d | i j k k|
                //|我知道了，我知道了|
                //|无p|
                //
                //|f g h|
                //a | j k l |=a（f（kp lo）-g（jp ln）+h（jo kn））
                //|否|
                //
                //|香港|
                //b | i k l |=b（e（kp lo）-g（ip lm）+h（io公里））
                //|营业执照|
                //
                //|e f h|
                //c | i j l |=c（e（jp ln）-f（ip lm）+h（以jm计）
                //|中英文对照|
                //
                //|英联邦政府|
                //d | i j k |=d（e（jo kn）-f（io-km）+g（单位：jm））
                //|m n|
                //
                //运营成本
                //穆尔
                //
                //加：6+8+3=17
                //mul:12+16=28

                FP a = M11, b = M12, c = M13, d = M14;
                FP e = M21, f = M22, g = M23, h = M24;
                FP i = M31, j = M32, k = M33, l = M34;
                FP m = M41, n = M42, o = M43, p = M44;

                FP kp_lo = k * p - l * o;
                FP jp_ln = j * p - l * n;
                FP jo_kn = j * o - k * n;
                FP ip_lm = i * p - l * m;
                FP io_km = i * o - k * m;
                FP in_jm = i * n - j * m;

                return a * (f * kp_lo - g * jp_ln + h * jo_kn) -
                       b * (e * kp_lo - g * ip_lm + h * io_km) +
                       c * (e * jp_ln - f * ip_lm + h * in_jm) -
                       d * (e * jo_kn - f * io_km + g * in_jm);
            }
        }

        /// <summary>
        ///计算给定矩阵的逆矩阵。
        /// </summary>
        ///<param name="matrix">
        ///<param name="result">JMatrix的
        public static void Inverse(ref TSMatrix4 matrix, out TSMatrix4 result)
        {
            //-1
            //妠
            //
            //-十一
            //M=—————————————————————————————————————————————————————
            //det（米）
            //
            //一个
            //
            //T
            //A=C
            //
            //C，M
            //本人
            //C=（-1）*det（米）
            //日本
            //
            //【a、b、c、d】
            //M=【e f g h】
            //[我会的]
            //[无p]
            //
            //第一排
            //2 | f g h|
            //C=（-1）| j k l |=+（f（kp lo）-g（jp ln）+h（jo kn））
            //11 ||
            //
            //3 | e g h|
            //C=（-1）| i k l |=-（e（kp lo）-g（ip lm）+h（io-里））
            //十二|
            //
            //4 |英|
            //C=（-1）| i j l |=+（e（jp ln）-f（ip lm）+h（以jm计）
            //13 |百百牛顿|
            //
            //5 | e f g|
            //C=（-1）| i j k |=-（e（jo kn）-f（io-km）+g（以jm计））
            //14 |无|
            //
            //第二排
            //三|
            //C=（-1）| j k l |=-（b（kp lo）-C（jp ln）+d（jo kn））
            //21 |无|
            //
            //四|
            //C=（-1）| i k l |=+（a（kp-lo）-C（ip-lm）+d（io-里））
            //二十二|
            //
            //五|
            //（英寸-1英寸）
            //二十三|
            //
            //6 | a b c|
            //C=（-1）| i j k |=+（a（jo kn）-b（io-km）+C（以jm计））
            //24度|
            //
            //第三排
            //四|
            //C=（-1）| f g h |=+（b（gp ho）-C（fp hn）+d（fo gn））
            //31 ||
            //
            //五|
            //C=（-1）|例如h |=-（a（gp ho）-C（ep-hm）+d（eo-gm））
            //三十二|
            //
            //六|
            //C=（-1）| e f h |=+（a（fp hn）-b（ep-hm）+d（英语-fm））
            //33 |百百牛顿|
            //
            //7 | a b c|
            //欧-欧（欧-欧-欧-欧-英-英-英-英-英-英-英-英-英-英-英-英-英-英-英-英-英-英-英）
            //34 |无|
            //
            //第四排
            //五|
            //C=（-1）| f g h |=-（b（gl香港）-C（fl hj）+d（fk gj））
            //41 |焦耳|
            //
            //六|
            //C=（-1）|例如h |=+（a（gl-hk）-C（el-hi）+d（ek-gi））
            //第四十二条|
            //
            //七|
            //C=（-1）| e f h |=-（a（fl hj）-b（el-hi）+d（ej-fi））
            //第四十三条|
            //
            //8 | a b c|
            //C=（-1）| e f g |=+（a（fk-gj）-b（ek-gi）+C（ej-fi））
            //44 ||
            //
            //运营成本
            //第53章，第104节
            FP a = matrix.M11, b = matrix.M12, c = matrix.M13, d = matrix.M14;
            FP e = matrix.M21, f = matrix.M22, g = matrix.M23, h = matrix.M24;
            FP i = matrix.M31, j = matrix.M32, k = matrix.M33, l = matrix.M34;
            FP m = matrix.M41, n = matrix.M42, o = matrix.M43, p = matrix.M44;

            FP kp_lo = k * p - l * o;
            FP jp_ln = j * p - l * n;
            FP jo_kn = j * o - k * n;
            FP ip_lm = i * p - l * m;
            FP io_km = i * o - k * m;
            FP in_jm = i * n - j * m;

            FP a11 = (f * kp_lo - g * jp_ln + h * jo_kn);
            FP a12 = -(e * kp_lo - g * ip_lm + h * io_km);
            FP a13 = (e * jp_ln - f * ip_lm + h * in_jm);
            FP a14 = -(e * jo_kn - f * io_km + g * in_jm);

            FP det = a * a11 + b * a12 + c * a13 + d * a14;

            if (det == FP.Zero)
            {
                result.M11 = FP.PositiveInfinity;
                result.M12 = FP.PositiveInfinity;
                result.M13 = FP.PositiveInfinity;
                result.M14 = FP.PositiveInfinity;
                result.M21 = FP.PositiveInfinity;
                result.M22 = FP.PositiveInfinity;
                result.M23 = FP.PositiveInfinity;
                result.M24 = FP.PositiveInfinity;
                result.M31 = FP.PositiveInfinity;
                result.M32 = FP.PositiveInfinity;
                result.M33 = FP.PositiveInfinity;
                result.M34 = FP.PositiveInfinity;
                result.M41 = FP.PositiveInfinity;
                result.M42 = FP.PositiveInfinity;
                result.M43 = FP.PositiveInfinity;
                result.M44 = FP.PositiveInfinity;

            }
            else
            {
                FP invDet = FP.One / det;

                result.M11 = a11 * invDet;
                result.M21 = a12 * invDet;
                result.M31 = a13 * invDet;
                result.M41 = a14 * invDet;

                result.M12 = -(b * kp_lo - c * jp_ln + d * jo_kn) * invDet;
                result.M22 = (a * kp_lo - c * ip_lm + d * io_km) * invDet;
                result.M32 = -(a * jp_ln - b * ip_lm + d * in_jm) * invDet;
                result.M42 = (a * jo_kn - b * io_km + c * in_jm) * invDet;

                FP gp_ho = g * p - h * o;
                FP fp_hn = f * p - h * n;
                FP fo_gn = f * o - g * n;
                FP ep_hm = e * p - h * m;
                FP eo_gm = e * o - g * m;
                FP en_fm = e * n - f * m;

                result.M13 = (b * gp_ho - c * fp_hn + d * fo_gn) * invDet;
                result.M23 = -(a * gp_ho - c * ep_hm + d * eo_gm) * invDet;
                result.M33 = (a * fp_hn - b * ep_hm + d * en_fm) * invDet;
                result.M43 = -(a * fo_gn - b * eo_gm + c * en_fm) * invDet;

                FP gl_hk = g * l - h * k;
                FP fl_hj = f * l - h * j;
                FP fk_gj = f * k - g * j;
                FP el_hi = e * l - h * i;
                FP ek_gi = e * k - g * i;
                FP ej_fi = e * j - f * i;

                result.M14 = -(b * gl_hk - c * fl_hj + d * fk_gj) * invDet;
                result.M24 = (a * gl_hk - c * el_hi + d * ek_gi) * invDet;
                result.M34 = -(a * fl_hj - b * el_hi + d * ej_fi) * invDet;
                result.M44 = (a * fk_gj - b * ek_gi + c * ej_fi) * invDet;
            }
        }

        /// <summary>
        ///将矩阵乘以比例因子。
        /// </summary>
        ///<param name="matrix1">
        ///<param name="scaleFactor">
        ///<返回>JMatrix
        public static TSMatrix4 Multiply(TSMatrix4 matrix1, FP scaleFactor)
        {
            Multiply(ref matrix1, scaleFactor, out TSMatrix4 result);
            return result;
        }

        /// <summary>
        ///将矩阵乘以比例因子。
        /// </summary>
        ///<param name="matrix1">
        ///<param name="scaleFactor">
        ///<param name="result">JMatrix公司
        public static void Multiply(ref TSMatrix4 matrix1, FP scaleFactor, out TSMatrix4 result)
        {
            FP num = scaleFactor;
            result.M11 = matrix1.M11 * num;
            result.M12 = matrix1.M12 * num;
            result.M13 = matrix1.M13 * num;
            result.M14 = matrix1.M14 * num;

            result.M21 = matrix1.M21 * num;
            result.M22 = matrix1.M22 * num;
            result.M23 = matrix1.M23 * num;
            result.M24 = matrix1.M24 * num;

            result.M31 = matrix1.M31 * num;
            result.M32 = matrix1.M32 * num;
            result.M33 = matrix1.M33 * num;
            result.M34 = matrix1.M34 * num;

            result.M41 = matrix1.M41 * num;
            result.M42 = matrix1.M42 * num;
            result.M43 = matrix1.M43 * num;
            result.M44 = matrix1.M44 * num;
        }


        public static TSMatrix4 Rotate(TSQuaternion quaternion)
        {
            Rotate(ref quaternion, out TSMatrix4 result);
            return result;
        }

        /// <summary>
        ///马蒂克斯
        /// </summary>
        ///<param name="quaternion">公司
        ///<param name="result">J矩阵</param>
        public static void Rotate(ref TSQuaternion quaternion, out TSMatrix4 result)
        {
            //预计算坐标积
            FP x = quaternion.x * 2;
            FP y = quaternion.y * 2;
            FP z = quaternion.z * 2;
            FP xx = quaternion.x * x;
            FP yy = quaternion.y * y;
            FP zz = quaternion.z * z;
            FP xy = quaternion.x * y;
            FP xz = quaternion.x * z;
            FP yz = quaternion.y * z;
            FP wx = quaternion.w * x;
            FP wy = quaternion.w * y;
            FP wz = quaternion.w * z;

            //从正基
            result.M11 = FP.One - (yy + zz);
            result.M21 = xy + wz;
            result.M31 = xz - wy;
            result.M41 = FP.Zero;
            result.M12 = xy - wz;
            result.M22 = FP.One - (xx + zz);
            result.M32 = yz + wx;
            result.M42 = FP.Zero;
            result.M13 = xz + wy;
            result.M23 = yz - wx;
            result.M33 = FP.One - (xx + yy);
            result.M43 = FP.Zero;
            result.M14 = FP.Zero;
            result.M24 = FP.Zero;
            result.M34 = FP.Zero;
            result.M44 = FP.One;
        }

        /// <summary>
        ///创建转置矩阵。
        /// </summary>
        ///<param name="matrix">
        ///<returns>JMatrix.</returns>
        public static TSMatrix4 Transpose(TSMatrix4 matrix)
        {
            Transpose(ref matrix, out TSMatrix4 result);
            return result;
        }

        /// <summary>
        ///创建转置矩阵。
        /// </summary>
        ///<param name="matrix">
        ///</param>
        public static void Transpose(ref TSMatrix4 matrix, out TSMatrix4 result)
        {
            result.M11 = matrix.M11;
            result.M12 = matrix.M21;
            result.M13 = matrix.M31;
            result.M14 = matrix.M41;
            result.M21 = matrix.M12;
            result.M22 = matrix.M22;
            result.M23 = matrix.M32;
            result.M24 = matrix.M42;
            result.M31 = matrix.M13;
            result.M32 = matrix.M23;
            result.M33 = matrix.M33;
            result.M34 = matrix.M43;
            result.M41 = matrix.M14;
            result.M42 = matrix.M24;
            result.M43 = matrix.M34;
            result.M44 = matrix.M44;
        }


        /// <summary>
        ///将两个矩阵相乘。
        /// </summary>
        ///<param name="value1">
        ///<param name="value2">
        ///<returns>
        public static TSMatrix4 operator *(TSMatrix4 value1, TSMatrix4 value2)
        {
            Multiply(ref value1, ref value2, out TSMatrix4 result);
            return result;
        }


        public FP Trace()
        {
            return M11 + M22 + M33 + M44;
        }

        /// <summary>
        ///将两个矩阵相加。
        /// </summary>
        ///<param name="value1">
        ///<param name="value2">
        ///返回
        public static TSMatrix4 operator +(TSMatrix4 value1, TSMatrix4 value2)
        {
            Add(ref value1, ref value2, out TSMatrix4 result);
            return result;
        }

        /// <summary>
        ///返回一个新的矩阵，其中包含给定矩阵的否定元素。
        /// </summary>
        ///<3name="参数">
        ///<returns>
        public static TSMatrix4 operator -(TSMatrix4 value)
        {
            TSMatrix4 result;

            result.M11 = -value.M11;
            result.M12 = -value.M12;
            result.M13 = -value.M13;
            result.M14 = -value.M14;
            result.M21 = -value.M21;
            result.M22 = -value.M22;
            result.M23 = -value.M23;
            result.M24 = -value.M24;
            result.M31 = -value.M31;
            result.M32 = -value.M32;
            result.M33 = -value.M33;
            result.M34 = -value.M34;
            result.M41 = -value.M41;
            result.M42 = -value.M42;
            result.M43 = -value.M43;
            result.M44 = -value.M44;

            return result;
        }

        /// <summary>
        ///减去两个矩阵。
        /// </summary>
        ///<param name="value1">
        ///<param name="value2">
        ///返回
        public static TSMatrix4 operator -(TSMatrix4 value1, TSMatrix4 value2)
        {
            Multiply(ref value2, -FP.One, out value2);
            Add(ref value1, ref value2, out TSMatrix4 result);
            return result;
        }

        public static bool operator ==(TSMatrix4 value1, TSMatrix4 value2)
        {
            return value1.M11 == value2.M11 &&
                value1.M12 == value2.M12 &&
                value1.M13 == value2.M13 &&
                value1.M14 == value2.M14 &&
                value1.M21 == value2.M21 &&
                value1.M22 == value2.M22 &&
                value1.M23 == value2.M23 &&
                value1.M24 == value2.M24 &&
                value1.M31 == value2.M31 &&
                value1.M32 == value2.M32 &&
                value1.M33 == value2.M33 &&
                value1.M34 == value2.M34 &&
                value1.M41 == value2.M41 &&
                value1.M42 == value2.M42 &&
                value1.M43 == value2.M43 &&
                value1.M44 == value2.M44;
        }

        public static bool operator !=(TSMatrix4 value1, TSMatrix4 value2)
        {
            return value1.M11 != value2.M11 ||
                value1.M12 != value2.M12 ||
                value1.M13 != value2.M13 ||
                value1.M14 != value2.M14 ||
                value1.M21 != value2.M21 ||
                value1.M22 != value2.M22 ||
                value1.M23 != value2.M23 ||
                value1.M24 != value2.M24 ||
                value1.M31 != value2.M31 ||
                value1.M32 != value2.M32 ||
                value1.M33 != value2.M33 ||
                value1.M34 != value2.M34 ||
                value1.M41 != value2.M41 ||
                value1.M42 != value2.M42 ||
                value1.M43 != value2.M43 ||
                value1.M44 != value2.M44;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TSMatrix4)) return false;
            TSMatrix4 other = (TSMatrix4)obj;

            return M11 == other.M11 &&
                M12 == other.M12 &&
                M13 == other.M13 &&
                M14 == other.M14 &&
                M21 == other.M21 &&
                M22 == other.M22 &&
                M23 == other.M23 &&
                M24 == other.M24 &&
                M31 == other.M31 &&
                M32 == other.M32 &&
                M33 == other.M33 &&
                M34 == other.M44 &&
                M41 == other.M41 &&
                M42 == other.M42 &&
                M43 == other.M43 &&
                M44 == other.M44;
        }

        public override int GetHashCode()
        {
            return M11.GetHashCode() ^
                M12.GetHashCode() ^
                M13.GetHashCode() ^
                M14.GetHashCode() ^
                M21.GetHashCode() ^
                M22.GetHashCode() ^
                M23.GetHashCode() ^
                M24.GetHashCode() ^
                M31.GetHashCode() ^
                M32.GetHashCode() ^
                M33.GetHashCode() ^
                M34.GetHashCode() ^
                M41.GetHashCode() ^
                M42.GetHashCode() ^
                M43.GetHashCode() ^
                M44.GetHashCode();
        }

        /// <summary>
        ///创建转换矩阵。
        /// </summary>
        ///</param>
        ///</param>
        ///</param>
        ///<returns>
        public static TSMatrix4 Translate(FP xPosition, FP yPosition, FP zPosition)
        {
            TSMatrix4 result;

            result.M11 = FP.One; result.M12 = FP.Zero; result.M13 = FP.Zero; result.M14 = xPosition;
            result.M21 = FP.Zero; result.M22 = FP.One; result.M23 = FP.Zero; result.M24 = yPosition;
            result.M31 = FP.Zero; result.M32 = FP.Zero; result.M33 = FP.One; result.M34 = zPosition;
            result.M41 = FP.Zero; result.M42 = FP.Zero; result.M43 = FP.Zero; result.M44 = FP.One;

            return result;
        }

        public static TSMatrix4 Translate(TSVector3 translation)
        {
            return Translate(translation.x, translation.y, translation.z);
        }

        /// <summary>
        ///创建缩放矩阵。
        /// </summary>
        ///<param name="xScale">X X轴
        ///<param name="yScale">Y型
        ///<param name="zScale">Z轴
        ///<returns>
        public static TSMatrix4 Scale(FP xScale, FP yScale, FP zScale)
        {
            TSMatrix4 result;

            result.M11 = xScale; result.M12 = FP.Zero; result.M13 = FP.Zero; result.M14 = FP.Zero;
            result.M21 = FP.Zero; result.M22 = yScale; result.M23 = FP.Zero; result.M24 = FP.Zero;
            result.M31 = FP.Zero; result.M32 = FP.Zero; result.M33 = zScale; result.M34 = FP.Zero;
            result.M41 = FP.Zero; result.M42 = FP.Zero; result.M43 = FP.Zero; result.M44 = FP.One;

            return result;
        }

        /// <summary>
        ///创建具有中心点的缩放矩阵。
        /// </summary>
        ///<param name="xScale">X X轴
        ///<param name="yScale">Y型
        ///<param name="zScale">Z轴
        ///<param name="centerPoint">
        ///<returns>
        public static TSMatrix4 Scale(FP xScale, FP yScale, FP zScale, TSVector3 centerPoint)
        {
            TSMatrix4 result;

            FP tx = centerPoint.x * (FP.One - xScale);
            FP ty = centerPoint.y * (FP.One - yScale);
            FP tz = centerPoint.z * (FP.One - zScale);

            result.M11 = xScale; result.M12 = FP.Zero; result.M13 = FP.Zero; result.M14 = FP.Zero;
            result.M21 = FP.Zero; result.M22 = yScale; result.M23 = FP.Zero; result.M24 = FP.Zero;
            result.M31 = FP.Zero; result.M32 = FP.Zero; result.M33 = zScale; result.M34 = FP.Zero;
            result.M41 = tx; result.M42 = ty; result.M43 = tz; result.M44 = FP.One;

            return result;
        }

        /// <summary>
        ///创建缩放矩阵。
        /// </summary>
        ///<param name="scales">
        ///<returns>
        public static TSMatrix4 Scale(TSVector3 scales)
        {
            return Scale(scales.x, scales.y, scales.z);
        }

        /// <summary>
        ///创建具有中心点的缩放矩阵。
        /// </summary>
        ///<param name="scales">
        ///<param name="centerPoint">
        ///<returns>
        public static TSMatrix4 Scale(TSVector3 scales, TSVector3 centerPoint)
        {
            return Scale(scales.x, scales.y, scales.z, centerPoint);
        }

        /// <summary>
        ///创建在每个轴上均匀缩放的统一缩放矩阵。
        /// </summary>
        ///<param name="scale">
        ///<returns>
        public static TSMatrix4 Scale(FP scale)
        {
            return Scale(scale, scale, scale);
        }

        /// <summary>
        ///创建一个统一的缩放矩阵，该矩阵在每个轴上以一个中心点相等地缩放。
        /// </summary>
        ///<param name="scale">
        ///<param name="centerPoint">
        ///<returns>
        public static TSMatrix4 Scale(FP scale, TSVector3 centerPoint)
        {
            return Scale(scale, scale, scale, centerPoint);
        }

        /// <summary>
        ///是的
        /// </summary>
        ///<param name="radians">X X cuplexcuplex，X二次开发
        ///<returns>
        public static TSMatrix4 RotateX(FP radians)
        {
            TSMatrix4 result;

            FP c = TSMathf.Cos(radians);
            FP s = TSMathf.Sin(radians);

            //[1 0 0 0]
            //[0摄氏度0]
            //[0-秒0]
            //[0 0 0 1]
            result.M11 = FP.One;
            result.M12 = FP.Zero;
            result.M13 = FP.Zero;
            result.M14 = FP.Zero;
            result.M21 = FP.Zero;
            result.M22 = c;
            result.M23 = s;
            result.M24 = FP.Zero;
            result.M31 = FP.Zero;
            result.M32 = -s;
            result.M33 = c;
            result.M34 = FP.Zero;
            result.M41 = FP.Zero;
            result.M42 = FP.Zero;
            result.M43 = FP.Zero;
            result.M44 = FP.One;

            return result;
        }

        /// <summary>
        ///我是说
        /// </summary>
        ///<param name="radians">X X cuplexcuplex，X二次开发
        ///<param name="centerPoint">
        ///<returns>
        public static TSMatrix4 RotateX(FP radians, TSVector3 centerPoint)
        {
            TSMatrix4 result;

            FP c = TSMathf.Cos(radians);
            FP s = TSMathf.Sin(radians);

            FP y = centerPoint.y * (FP.One - c) + centerPoint.z * s;
            FP z = centerPoint.z * (FP.One - c) - centerPoint.y * s;

            //[1 0 0 0]
            //[0摄氏度0]
            //[0-秒0]
            //[0 y z 1]
            result.M11 = FP.One;
            result.M12 = FP.Zero;
            result.M13 = FP.Zero;
            result.M14 = FP.Zero;
            result.M21 = FP.Zero;
            result.M22 = c;
            result.M23 = s;
            result.M24 = FP.Zero;
            result.M31 = FP.Zero;
            result.M32 = -s;
            result.M33 = c;
            result.M34 = FP.Zero;
            result.M41 = FP.Zero;
            result.M42 = y;
            result.M43 = z;
            result.M44 = FP.One;

            return result;
        }

        /// <summary>
        ///是的
        /// </summary>
        ///<param name="radians">Y超视距
        ///<returns>
        public static TSMatrix4 RotateY(FP radians)
        {
            TSMatrix4 result;

            FP c = TSMathf.Cos(radians);
            FP s = TSMathf.Sin(radians);

            //[c 0-s 0]
            //[0 1 0 0]
            //【s 0 c 0】
            //[0 0 0 1]
            result.M11 = c;
            result.M12 = FP.Zero;
            result.M13 = -s;
            result.M14 = FP.Zero;
            result.M21 = FP.Zero;
            result.M22 = FP.One;
            result.M23 = FP.Zero;
            result.M24 = FP.Zero;
            result.M31 = s;
            result.M32 = FP.Zero;
            result.M33 = c;
            result.M34 = FP.Zero;
            result.M41 = FP.Zero;
            result.M42 = FP.Zero;
            result.M43 = FP.Zero;
            result.M44 = FP.One;

            return result;
        }

        /// <summary>
        ///是的
        /// </summary>
        ///<param name="radians">Y超视距
        ///<param name="centerPoint">
        ///<returns>
        public static TSMatrix4 RotateY(FP radians, TSVector3 centerPoint)
        {
            TSMatrix4 result;

            FP c = TSMathf.Cos(radians);
            FP s = TSMathf.Sin(radians);

            FP x = centerPoint.x * (FP.One - c) - centerPoint.z * s;
            FP z = centerPoint.x * (FP.One - c) + centerPoint.x * s;

            //[c 0-s 0]
            //[0 1 0 0]
            //【s 0 c 0】
            //[x 0 z 1]
            result.M11 = c;
            result.M12 = FP.Zero;
            result.M13 = -s;
            result.M14 = FP.Zero;
            result.M21 = FP.Zero;
            result.M22 = FP.One;
            result.M23 = FP.Zero;
            result.M24 = FP.Zero;
            result.M31 = s;
            result.M32 = FP.Zero;
            result.M33 = c;
            result.M34 = FP.Zero;
            result.M41 = x;
            result.M42 = FP.Zero;
            result.M43 = z;
            result.M44 = FP.One;

            return result;
        }

        /// <summary>
        ///是的
        /// </summary>
        ///<param name="radians">Z超光速
        ///<returns>
        public static TSMatrix4 RotateZ(FP radians)
        {
            TSMatrix4 result;

            FP c = TSMathf.Cos(radians);
            FP s = TSMathf.Sin(radians);

            //[秒0 0]
            //[-s c 0 0]
            //[0 0 1 0]
            //[0 0 0 1]
            result.M11 = c;
            result.M12 = s;
            result.M13 = FP.Zero;
            result.M14 = FP.Zero;
            result.M21 = -s;
            result.M22 = c;
            result.M23 = FP.Zero;
            result.M24 = FP.Zero;
            result.M31 = FP.Zero;
            result.M32 = FP.Zero;
            result.M33 = FP.One;
            result.M34 = FP.Zero;
            result.M41 = FP.Zero;
            result.M42 = FP.Zero;
            result.M43 = FP.Zero;
            result.M44 = FP.One;

            return result;
        }

        /// <summary>
        ///这是一个非常重要的问题
        /// </summary>
        ///<param name="radians">Z超光速
        ///<param name="centerPoint">
        ///<returns>
        public static TSMatrix4 RotateZ(FP radians, TSVector3 centerPoint)
        {
            TSMatrix4 result;

            FP c = TSMathf.Cos(radians);
            FP s = TSMathf.Sin(radians);

            FP x = centerPoint.x * (1 - c) + centerPoint.y * s;
            FP y = centerPoint.y * (1 - c) - centerPoint.x * s;

            //[秒0 0]
            //[-s c 0 0]
            //[0 0 1 0]
            //[x y 0 1]
            result.M11 = c;
            result.M12 = s;
            result.M13 = FP.Zero;
            result.M14 = FP.Zero;
            result.M21 = -s;
            result.M22 = c;
            result.M23 = FP.Zero;
            result.M24 = FP.Zero;
            result.M31 = FP.Zero;
            result.M32 = FP.Zero;
            result.M33 = FP.One;
            result.M34 = FP.Zero;
            result.M41 = FP.Zero;
            result.M42 = FP.Zero;
            result.M43 = FP.Zero;
            result.M44 = FP.One;

            return result;
        }

        /// <summary>
        ///创建绕给定轴旋转给定角度的矩阵。
        /// </summary>
        ///<param name="axis">毛边。</param>
        ///<param name="angle">
        ///<param name="result">
        public static void AxisAngle(ref TSVector3 axis, FP angle, out TSMatrix4 result)
        {
            //a： 角度
            //x、 是，z：是的
            //
            //旋转矩阵M可以下方式
            //
            //T T T T
            //M=uu+（cos a）（I-uu）+（新浪）S
            //
            //哪里：
            //
            //u=（x，y，z）
            //
            //[0-z y]
            //S=[z 0-x]
            //[-y x 0]
            //
            //[1 0 0]
            //我=[0 1 0]
            //[0 0 1]
            //
            //
            //[xx+cosa*（1-xx）yx cosa*yx-浪*z zx cosa*xz+新浪*y]
            //M=[xy cosa*yx+北段浪*z yy+cosa（1-yy）yz cosa*yz-北段浪*x]
            //[zx cosa*zx SENTARY浪*y zy cosa*zy+北段浪*x zz+cosa*（1-zz）]
            //
            FP x = axis.x, y = axis.y, z = axis.z;
            FP sa = TSMathf.Sin(angle), ca = TSMathf.Cos(angle);
            FP xx = x * x, yy = y * y, zz = z * z;
            FP xy = x * y, xz = x * z, yz = y * z;

            result.M11 = xx + ca * (FP.One - xx);
            result.M12 = xy - ca * xy + sa * z;
            result.M13 = xz - ca * xz - sa * y;
            result.M14 = FP.Zero;
            result.M21 = xy - ca * xy - sa * z;
            result.M22 = yy + ca * (FP.One - yy);
            result.M23 = yz - ca * yz + sa * x;
            result.M24 = FP.Zero;
            result.M31 = xz - ca * xz + sa * y;
            result.M32 = yz - ca * yz - sa * x;
            result.M33 = zz + ca * (FP.One - zz);
            result.M34 = FP.Zero;
            result.M41 = FP.Zero;
            result.M42 = FP.Zero;
            result.M43 = FP.Zero;
            result.M44 = FP.One;
        }

        /// <summary>
        ///创建绕给定轴旋转给定角度的矩阵。
        /// </summary>
        ///<param name="axis">毛边。</param>
        ///<param name="angle">
        ///<returns>返回
        public static TSMatrix4 AngleAxis(FP angle, TSVector3 axis)
        {
            AxisAngle(ref axis, angle, out TSMatrix4 result);
            return result;
        }

        public override string ToString()
        {
            return string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}",
                M11, M12, M13, M14,
                M21, M22, M23, M24,
                M31, M32, M33, M34,
                M41, M42, M43, M44);
        }

        public static void TRS(TSVector3 translation, TSQuaternion rotation, TSVector3 scale, out TSMatrix4 matrix)
        {
            matrix = Translate(translation) * Rotate(rotation) * Scale(scale);
        }

        public static TSMatrix4 TRS(TSVector3 translation, TSQuaternion rotation, TSVector3 scale)
        {
            TRS(translation, rotation, scale, out TSMatrix4 result);
            return result;
        }

        public static TSMatrix4 TransformToMatrix(ref TSTransform transform)
        {
            TRS(transform.localPosition, transform.localRotation, transform.localScale, out TSMatrix4 result);
            return result;
        }

        public static TSMatrix4 TransformToMatrix1(ref TSTransform transform)
        {
            TRS(transform.position, transform.rotation, transform.localScale, out TSMatrix4 result);
            return result;
        }
    }

}
