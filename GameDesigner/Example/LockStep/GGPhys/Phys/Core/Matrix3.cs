using System;
using System.Collections.Generic;
using FixPointCS;
using REAL = FixMath.FP;

#if GGPHYS_FIXPOINT32
using RawType = System.Int32;
using FixedType = FixPointCS.Fixed32;
#else
using RawType = System.Int64;
using FixedType = FixPointCS.Fixed64;
#endif

namespace GGPhys.Core
{
    /// <summary>
    /// 3x3矩阵
    /// </summary>
    public struct Matrix3
    {
        public RawType raw0, raw1, raw2;
        public RawType raw3, raw4, raw5;
        public RawType raw6, raw7, raw8;

        public REAL data0 { get { return REAL.FromRaw(raw0); } set { raw0 = value.Raw; } }
        public REAL data1 { get { return REAL.FromRaw(raw1); } set { raw1 = value.Raw; } }
        public REAL data2 { get { return REAL.FromRaw(raw2); } set { raw2 = value.Raw; } }
        public REAL data3 { get { return REAL.FromRaw(raw3); } set { raw3 = value.Raw; } }
        public REAL data4 { get { return REAL.FromRaw(raw4); } set { raw4 = value.Raw; } }
        public REAL data5 { get { return REAL.FromRaw(raw5); } set { raw5 = value.Raw; } }
        public REAL data6 { get { return REAL.FromRaw(raw6); } set { raw6 = value.Raw; } }
        public REAL data7 { get { return REAL.FromRaw(raw7); } set { raw7 = value.Raw; } }
        public REAL data8 { get { return REAL.FromRaw(raw8); } set { raw8 = value.Raw; } }


        /// <summary>
        /// 获得一个单位矩阵
        /// </summary>
        static readonly public Matrix3 Identity = new Matrix3(REAL.One, REAL.Zero, REAL.Zero,
                                                              REAL.Zero, REAL.One, REAL.Zero,
                                                              REAL.Zero, REAL.Zero, REAL.One);

        /// <summary>
        /// 获得一个全0矩阵
        /// </summary>
        static readonly public Matrix3 Zero = new Matrix3(REAL.Zero, REAL.Zero, REAL.Zero,
                                                          REAL.Zero, REAL.Zero, REAL.Zero,
                                                          REAL.Zero, REAL.Zero, REAL.Zero);


        public Matrix3(REAL m0, REAL m1, REAL m2,
                       REAL m3, REAL m4, REAL m5,
                       REAL m6, REAL m7, REAL m8)
        {
            raw0 = m0.Raw; raw1 = m1.Raw; raw2 = m2.Raw;
            raw3 = m3.Raw; raw4 = m4.Raw; raw5 = m5.Raw;
            raw6 = m6.Raw; raw7 = m7.Raw; raw8 = m8.Raw;
        }

        private Matrix3(RawType m0, RawType m1, RawType m2,
                       RawType m3, RawType m4, RawType m5,
                       RawType m6, RawType m7, RawType m8)
        {
            raw0 = m0; raw1 = m1; raw2 = m2;
            raw3 = m3; raw4 = m4; raw5 = m5;
            raw6 = m6; raw7 = m7; raw8 = m8;
        }

        /// <summary>
        /// 根据每一列数据构造新矩阵
        /// </summary>
        public Matrix3(Vector3d compOne, Vector3d compTwo, Vector3d compThree)
        {
            raw0 = compOne.x.Raw; raw1 = compTwo.x.Raw; raw2 = compThree.x.Raw;
            raw3 = compOne.y.Raw; raw4 = compTwo.y.Raw; raw5 = compThree.y.Raw;
            raw6 = compOne.z.Raw; raw7 = compTwo.z.Raw; raw8 = compThree.z.Raw;
        }

        /// <summary>
        /// 下标读写
        /// </summary>
        unsafe public REAL this[RawType i]
        {
            get
            {
                if ((uint)i >= 9)
                    throw new IndexOutOfRangeException("Matrix3 index out of range.");

                fixed (Matrix3* array = &this) { return REAL.FromRaw(((RawType*)array)[i]); }
            }
            set
            {
                if ((uint)i >= 9)
                    throw new IndexOutOfRangeException("Matrix3 index out of range.");

                fixed (RawType* array = &raw0) { array[i] = value.Raw; }
            }
        }

        public static Matrix3 FromRaw(RawType m0, RawType m1, RawType m2,
                                       RawType m3, RawType m4, RawType m5,
                                       RawType m6, RawType m7, RawType m8)
        {
            Matrix3 m = new Matrix3();
            m.raw0 = m0; m.raw1 = m1; m.raw2 = m2;
            m.raw3 = m3; m.raw4 = m4; m.raw5 = m5;
            m.raw6 = m6; m.raw7 = m7; m.raw8 = m8;
            return m;
        }

        /// <summary>
        /// 用矩阵对向量进行变换
        /// </summary>
        public static Vector3d operator *(Matrix3 m, Vector3d vector)
        {
            return Vector3d.FromRaw(
                FixedType.Mul(vector.x.Raw, m.raw0) + FixedType.Mul(vector.y.Raw, m.raw1) + FixedType.Mul(vector.z.Raw, m.raw2),
                FixedType.Mul(vector.x.Raw, m.raw3) + FixedType.Mul(vector.y.Raw, m.raw4) + FixedType.Mul(vector.z.Raw, m.raw5),
                FixedType.Mul(vector.x.Raw, m.raw6) + FixedType.Mul(vector.y.Raw, m.raw7) + FixedType.Mul(vector.z.Raw, m.raw8)
            );
        }

        /// <summary>
        /// 矩阵相乘
        /// </summary>
        public static Matrix3 operator *(Matrix3 m1, Matrix3 m2)
        {
            return new Matrix3(
                FixedType.Mul(m1.raw0, m2.raw0) + FixedType.Mul(m1.raw1, m2.raw3) + FixedType.Mul(m1.raw2, m2.raw6),
                FixedType.Mul(m1.raw0, m2.raw1) + FixedType.Mul(m1.raw1, m2.raw4) + FixedType.Mul(m1.raw2, m2.raw7),
                FixedType.Mul(m1.raw0, m2.raw2) + FixedType.Mul(m1.raw1, m2.raw5) + FixedType.Mul(m1.raw2, m2.raw8),

                FixedType.Mul(m1.raw3, m2.raw0) + FixedType.Mul(m1.raw4, m2.raw3) + FixedType.Mul(m1.raw5, m2.raw6),
                FixedType.Mul(m1.raw3, m2.raw1) + FixedType.Mul(m1.raw4, m2.raw4) + FixedType.Mul(m1.raw5, m2.raw7),
                FixedType.Mul(m1.raw3, m2.raw2) + FixedType.Mul(m1.raw4, m2.raw5) + FixedType.Mul(m1.raw5, m2.raw8),

                FixedType.Mul(m1.raw6, m2.raw0) + FixedType.Mul(m1.raw7, m2.raw3) + FixedType.Mul(m1.raw8, m2.raw6),
                FixedType.Mul(m1.raw6, m2.raw1) + FixedType.Mul(m1.raw7, m2.raw4) + FixedType.Mul(m1.raw8, m2.raw7),
                FixedType.Mul(m1.raw6, m2.raw2) + FixedType.Mul(m1.raw7, m2.raw5) + FixedType.Mul(m1.raw8, m2.raw8)
                );
        }

        /// <summary>
        /// 矩阵乘以一个数
        /// </summary>
        public static Matrix3 operator *(Matrix3 m1, REAL scalar)
        {
            return new Matrix3(
             FixedType.Mul(m1.raw0, scalar.Raw), FixedType.Mul(m1.raw1, scalar.Raw), FixedType.Mul(m1.raw2, scalar.Raw),
             FixedType.Mul(m1.raw3, scalar.Raw), FixedType.Mul(m1.raw4, scalar.Raw), FixedType.Mul(m1.raw5, scalar.Raw),
             FixedType.Mul(m1.raw6, scalar.Raw), FixedType.Mul(m1.raw7, scalar.Raw), FixedType.Mul(m1.raw8, scalar.Raw));
        }

        /// <summary>
        /// 矩阵加法
        /// </summary>
        public static Matrix3 operator +(Matrix3 m1, Matrix3 m2)
        {
            return new Matrix3(
            m1.raw0 + m2.raw0, m1.raw1 + m2.raw1, m1.raw2 + m2.raw2,
            m1.raw3 + m2.raw3, m1.raw4 + m2.raw4, m1.raw5 + m2.raw5,
            m1.raw6 + m2.raw6, m1.raw7 + m2.raw7, m1.raw8 + m2.raw8);
        }

        /// <summary>
        /// 矩阵减法
        /// </summary>
        public static Matrix3 operator -(Matrix3 m1, Matrix3 m2)
        {
            return new Matrix3(
            m1.raw0 - m2.raw0, m1.raw1 - m2.raw1, m1.raw2 - m2.raw2,
            m1.raw3 - m2.raw3, m1.raw4 - m2.raw4, m1.raw5 - m2.raw5,
            m1.raw6 - m2.raw6, m1.raw7 - m2.raw7, m1.raw8 - m2.raw8);
        }

        /// <summary>
        /// 转为字符串
        /// </summary>
        public override string ToString()
        {
            return data0 + "," + data1 + "," + data2 + "\n" +
                   data3 + "," + data4 + "," + data5 + "\n" +
                   data6 + "," + data7 + "," + data8;
        }

        /// <summary>
        /// 惯性张量赋值
        /// </summary>
        public void SetInertiaTensorCoeffs(REAL ix, REAL iy, REAL iz, REAL ixy, REAL ixz, REAL iyz)
        {
            raw0 = ix.Raw;
            raw1 = raw3 = -ixy.Raw;
            raw2 = raw6 = -ixz.Raw;
            raw4 = iy.Raw;
            raw5 = raw7 = -iyz.Raw;
            raw8 = iz.Raw;
        }

        /// <summary>
        /// 斜矩阵赋值
        /// </summary>
        public void SetSkewSymmetric(Vector3d vector)
        {
            raw0 = raw4 = raw8 = REAL.Zero.Raw;
            raw1 = -vector.z.Raw;
            raw2 = vector.y.Raw;
            raw3 = vector.z.Raw;
            raw5 = -vector.x.Raw;
            raw6 = -vector.y.Raw;
            raw7 = vector.x.Raw;
        }

        /// <summary>
        /// 为每一列赋值
        /// </summary>
        public void SetComponents(Vector3d compOne, Vector3d compTwo, Vector3d compThree)
        {
            raw0 = compOne.x.Raw; raw1 = compTwo.x.Raw; raw2 = compThree.x.Raw;
            raw3 = compOne.y.Raw; raw4 = compTwo.y.Raw; raw5 = compThree.y.Raw;
            raw6 = compOne.z.Raw; raw7 = compTwo.z.Raw; raw8 = compThree.z.Raw;
        }

        /// <summary>
        /// 变换向量
        /// </summary>
        public Vector3d Transform(Vector3d vector)
        {
            return this * vector;
        }

        /// <summary>
        /// 逆变换向量
        /// </summary>
        public Vector3d TransformTranspose(Vector3d vector)
        {
            return Vector3d.FromRaw(
                FixedType.Mul(vector.x.Raw, raw0) + FixedType.Mul(vector.y.Raw, raw3) + FixedType.Mul(vector.z.Raw, raw6),
                FixedType.Mul(vector.x.Raw, raw1) + FixedType.Mul(vector.y.Raw, raw4) + FixedType.Mul(vector.z.Raw, raw7),
                FixedType.Mul(vector.x.Raw, raw2) + FixedType.Mul(vector.y.Raw, raw5) + FixedType.Mul(vector.z.Raw, raw8)
            );
        }

        /// <summary>
        /// 获取矩阵一行
        /// </summary>
        /// <param name="i">行</param>
        public Vector3d GetRowVector(RawType i)
        {
            return new Vector3d(this[i * 3], this[i * 3 + 1], this[i * 3 + 2]);
        }

        /// <summary>
        /// 获取矩阵一列
        /// </summary>
        /// <param name="i">列</param>
        public Vector3d GetAxisVector(RawType i)
        {
            return new Vector3d(this[i], this[i + 3], this[i + 6]);
        }

        /// <summary>
        /// 变为给定矩阵的逆矩阵
        /// </summary>
        public void SetInverse(Matrix3 m)
        {
            if (m.raw1 == REAL.Zero.Raw
                && m.raw2 == REAL.Zero.Raw
                && m.raw3 == REAL.Zero.Raw
                && m.raw5 == REAL.Zero.Raw
                && m.raw6 == REAL.Zero.Raw
                && m.raw7 == REAL.Zero.Raw)
            {
                data0 = m.data0 == 0 ? 0 : 1 / m.data0;
                data4 = m.data4 == 0 ? 0 : 1 / m.data4;
                data8 = m.data8 == 0 ? 0 : 1 / m.data8;
                return;
            }

            RawType t4 = FixedType.Mul(m.raw0, m.raw4);
            RawType t6 = FixedType.Mul(m.raw0, m.raw5);
            RawType t8 = FixedType.Mul(m.raw1, m.raw3);
            RawType t10 = FixedType.Mul(m.raw2, m.raw3);
            RawType t12 = FixedType.Mul(m.raw1, m.raw6);
            RawType t14 = FixedType.Mul(m.raw2, m.raw6);

            // 计算行列式
            RawType t16 = (FixedType.Mul(t4, m.raw8) - FixedType.Mul(t6, m.raw7) - FixedType.Mul(t4, m.raw8) +
                        FixedType.Mul(t10, m.raw7) + FixedType.Mul(t12, m.raw5) - FixedType.Mul(t14, m.raw4));

            // 确保行列式非0
            if (t16 == REAL.Zero.Raw) return;
            RawType t17 = FixedType.DivPrecise(REAL.One.Raw, t16);

            raw0 = FixedType.Mul((FixedType.Mul(m.raw4, m.raw8) - FixedType.Mul(m.raw5, m.raw7)), t17);
            raw1 = -FixedType.Mul((FixedType.Mul(m.raw1, m.raw8) - FixedType.Mul(m.raw2, m.raw7)), t17);
            raw2 = FixedType.Mul((FixedType.Mul(m.raw1, m.raw5) - FixedType.Mul(m.raw2, m.raw4)), t17); ;
            raw3 = -FixedType.Mul((FixedType.Mul(m.raw3, m.raw8) - FixedType.Mul(m.raw5, m.raw6)), t17); ;
            raw4 = FixedType.Mul((FixedType.Mul(m.raw0, m.raw8) - t14), t17);
            raw5 = -FixedType.Mul((t6 - t10), t17);
            raw6 = FixedType.Mul((FixedType.Mul(m.raw3, m.raw7) - FixedType.Mul(m.raw4, m.raw6)), t17); ;
            raw7 = -FixedType.Mul((FixedType.Mul(m.raw0, m.raw7) - t12), t17);
            raw8 = FixedType.Mul((t4 - t8), t17);
        }

        /// <summary>
        /// 获取逆矩阵
        /// </summary>
        public Matrix3 Inverse()
        {
            Matrix3 result = Identity;
            result.SetInverse(this);
            return result;
        }

        /// <summary>
        /// 转换为逆矩阵
        /// </summary>
        public void Invert()
        {
            SetInverse(this);
        }

        /// <summary>
        /// 赋值为给定矩阵的转置矩阵
        /// </summary>
        public void SetTranspose(Matrix3 m)
        {
            raw0 = m.raw0;
            raw1 = m.raw3;
            raw2 = m.raw6;
            raw3 = m.raw1;
            raw4 = m.raw4;
            raw5 = m.raw7;
            raw6 = m.raw2;
            raw7 = m.raw5;
            raw8 = m.raw8;
        }

        /// <summary>
        /// 获取转置矩阵
        /// </summary>
        public Matrix3 Transpose()
        {
            Matrix3 result = Identity;
            result.SetTranspose(this);
            return result;
        }

        /// <summary>
        /// 根据四元数赋值
        /// </summary>
        public void SetOrientation(Quaternion q)
        {
            RawType one = REAL.One.Raw;
            RawType two = REAL.Two.Raw;

            raw0 = one - (FixedType.Mul(two, FixedType.Mul(q.y.Raw, q.y.Raw)) + FixedType.Mul(two, FixedType.Mul(q.z.Raw, q.z.Raw)));
            raw1 = FixedType.Mul(two, FixedType.Mul(q.x.Raw, q.y.Raw)) - FixedType.Mul(two, FixedType.Mul(q.z.Raw, q.w.Raw));
            raw2 = FixedType.Mul(two, FixedType.Mul(q.x.Raw, q.y.Raw)) + FixedType.Mul(two, FixedType.Mul(q.y.Raw, q.w.Raw));

            raw3 = FixedType.Mul(two, FixedType.Mul(q.x.Raw, q.y.Raw)) + FixedType.Mul(two, FixedType.Mul(q.z.Raw, q.w.Raw));
            raw4 = one - (FixedType.Mul(two, FixedType.Mul(q.x.Raw, q.x.Raw)) + FixedType.Mul(two, FixedType.Mul(q.z.Raw, q.z.Raw)));
            raw5 = FixedType.Mul(two, FixedType.Mul(q.y.Raw, q.z.Raw)) - FixedType.Mul(two, FixedType.Mul(q.x.Raw, q.w.Raw));

            raw6 = FixedType.Mul(two, FixedType.Mul(q.x.Raw, q.z.Raw)) - FixedType.Mul(two, FixedType.Mul(q.y.Raw, q.w.Raw));
            raw7 = FixedType.Mul(two, FixedType.Mul(q.y.Raw, q.z.Raw)) + FixedType.Mul(two, FixedType.Mul(q.x.Raw, q.w.Raw));
            raw8 = one - (FixedType.Mul(two, FixedType.Mul(q.x.Raw, q.x.Raw)) + FixedType.Mul(two, FixedType.Mul(q.y.Raw, q.y.Raw)));
        }
    }
}