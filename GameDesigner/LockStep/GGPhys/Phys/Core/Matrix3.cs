using FixPointCS;
using System;
using TrueSync;

namespace GGPhys.Core
{
    /// <summary>
    /// 3x3矩阵
    /// </summary>
    public struct Matrix3
    {
        public long raw0, raw1, raw2;
        public long raw3, raw4, raw5;
        public long raw6, raw7, raw8;

        public FP data0 { get { return FP.FromRaw(raw0); } set { raw0 = value.Raw; } }
        public FP data1 { get { return FP.FromRaw(raw1); } set { raw1 = value.Raw; } }
        public FP data2 { get { return FP.FromRaw(raw2); } set { raw2 = value.Raw; } }
        public FP data3 { get { return FP.FromRaw(raw3); } set { raw3 = value.Raw; } }
        public FP data4 { get { return FP.FromRaw(raw4); } set { raw4 = value.Raw; } }
        public FP data5 { get { return FP.FromRaw(raw5); } set { raw5 = value.Raw; } }
        public FP data6 { get { return FP.FromRaw(raw6); } set { raw6 = value.Raw; } }
        public FP data7 { get { return FP.FromRaw(raw7); } set { raw7 = value.Raw; } }
        public FP data8 { get { return FP.FromRaw(raw8); } set { raw8 = value.Raw; } }


        /// <summary>
        /// 获得一个单位矩阵
        /// </summary>
        static readonly public Matrix3 Identity = new Matrix3(FP.One, FP.Zero, FP.Zero,
                                                              FP.Zero, FP.One, FP.Zero,
                                                              FP.Zero, FP.Zero, FP.One);

        /// <summary>
        /// 获得一个全0矩阵
        /// </summary>
        static readonly public Matrix3 Zero = new Matrix3(FP.Zero, FP.Zero, FP.Zero,
                                                          FP.Zero, FP.Zero, FP.Zero,
                                                          FP.Zero, FP.Zero, FP.Zero);


        public Matrix3(FP m0, FP m1, FP m2,
                       FP m3, FP m4, FP m5,
                       FP m6, FP m7, FP m8)
        {
            raw0 = m0.Raw; raw1 = m1.Raw; raw2 = m2.Raw;
            raw3 = m3.Raw; raw4 = m4.Raw; raw5 = m5.Raw;
            raw6 = m6.Raw; raw7 = m7.Raw; raw8 = m8.Raw;
        }

        private Matrix3(long m0, long m1, long m2,
                       long m3, long m4, long m5,
                       long m6, long m7, long m8)
        {
            raw0 = m0; raw1 = m1; raw2 = m2;
            raw3 = m3; raw4 = m4; raw5 = m5;
            raw6 = m6; raw7 = m7; raw8 = m8;
        }

        /// <summary>
        /// 根据每一列数据构造新矩阵
        /// </summary>
        public Matrix3(TSVector3 compOne, TSVector3 compTwo, TSVector3 compThree)
        {
            raw0 = compOne.RawX; raw1 = compTwo.RawX; raw2 = compThree.RawX;
            raw3 = compOne.RawY; raw4 = compTwo.RawY; raw5 = compThree.RawY;
            raw6 = compOne.RawZ; raw7 = compTwo.RawZ; raw8 = compThree.RawZ;
        }

        /// <summary>
        /// 下标读写
        /// </summary>
        unsafe public FP this[int i]
        {
            get
            {
                if ((uint)i >= 9)
                    throw new IndexOutOfRangeException("Matrix3 index out of range.");

                fixed (Matrix3* array = &this) { return FP.FromRaw(((long*)array)[i]); }
            }
            set
            {
                if ((uint)i >= 9)
                    throw new IndexOutOfRangeException("Matrix3 index out of range.");

                fixed (long* array = &raw0) { array[i] = value.Raw; }
            }
        }

        public static Matrix3 FromLong(long m0, long m1, long m2,
                                       long m3, long m4, long m5,
                                       long m6, long m7, long m8)
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
        public static TSVector3 operator *(Matrix3 m, TSVector3 vector)
        {
            return new TSVector3(
                Fixed64.Mul(vector.RawX, m.raw0) + Fixed64.Mul(vector.RawY, m.raw1) + Fixed64.Mul(vector.RawZ, m.raw2),
                Fixed64.Mul(vector.RawX, m.raw3) + Fixed64.Mul(vector.RawY, m.raw4) + Fixed64.Mul(vector.RawZ, m.raw5),
                Fixed64.Mul(vector.RawX, m.raw6) + Fixed64.Mul(vector.RawY, m.raw7) + Fixed64.Mul(vector.RawZ, m.raw8)
            );
        }

        /// <summary>
        /// 矩阵相乘
        /// </summary>
        public static Matrix3 operator *(Matrix3 m1, Matrix3 m2)
        {
            return new Matrix3(
                Fixed64.Mul(m1.raw0, m2.raw0) + Fixed64.Mul(m1.raw1, m2.raw3) + Fixed64.Mul(m1.raw2, m2.raw6),
                Fixed64.Mul(m1.raw0, m2.raw1) + Fixed64.Mul(m1.raw1, m2.raw4) + Fixed64.Mul(m1.raw2, m2.raw7),
                Fixed64.Mul(m1.raw0, m2.raw2) + Fixed64.Mul(m1.raw1, m2.raw5) + Fixed64.Mul(m1.raw2, m2.raw8),

                Fixed64.Mul(m1.raw3, m2.raw0) + Fixed64.Mul(m1.raw4, m2.raw3) + Fixed64.Mul(m1.raw5, m2.raw6),
                Fixed64.Mul(m1.raw3, m2.raw1) + Fixed64.Mul(m1.raw4, m2.raw4) + Fixed64.Mul(m1.raw5, m2.raw7),
                Fixed64.Mul(m1.raw3, m2.raw2) + Fixed64.Mul(m1.raw4, m2.raw5) + Fixed64.Mul(m1.raw5, m2.raw8),

                Fixed64.Mul(m1.raw6, m2.raw0) + Fixed64.Mul(m1.raw7, m2.raw3) + Fixed64.Mul(m1.raw8, m2.raw6),
                Fixed64.Mul(m1.raw6, m2.raw1) + Fixed64.Mul(m1.raw7, m2.raw4) + Fixed64.Mul(m1.raw8, m2.raw7),
                Fixed64.Mul(m1.raw6, m2.raw2) + Fixed64.Mul(m1.raw7, m2.raw5) + Fixed64.Mul(m1.raw8, m2.raw8)
                );
        }

        /// <summary>
        /// 矩阵乘以一个数
        /// </summary>
        public static Matrix3 operator *(Matrix3 m1, FP scalar)
        {
            return new Matrix3(
             Fixed64.Mul(m1.raw0, scalar.Raw), Fixed64.Mul(m1.raw1, scalar.Raw), Fixed64.Mul(m1.raw2, scalar.Raw),
             Fixed64.Mul(m1.raw3, scalar.Raw), Fixed64.Mul(m1.raw4, scalar.Raw), Fixed64.Mul(m1.raw5, scalar.Raw),
             Fixed64.Mul(m1.raw6, scalar.Raw), Fixed64.Mul(m1.raw7, scalar.Raw), Fixed64.Mul(m1.raw8, scalar.Raw));
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
        public void SetInertiaTensorCoeffs(FP ix, FP iy, FP iz, FP ixy, FP ixz, FP iyz)
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
        public void SetSkewSymmetric(TSVector3 vector)
        {
            raw0 = raw4 = raw8 = FP.Zero.Raw;
            raw1 = -vector.RawZ;
            raw2 = vector.RawY;
            raw3 = vector.RawZ;
            raw5 = -vector.RawX;
            raw6 = -vector.RawY;
            raw7 = vector.RawX;
        }

        /// <summary>
        /// 为每一列赋值
        /// </summary>
        public void SetComponents(TSVector3 compOne, TSVector3 compTwo, TSVector3 compThree)
        {
            raw0 = compOne.RawX; raw1 = compTwo.RawX; raw2 = compThree.RawX;
            raw3 = compOne.RawY; raw4 = compTwo.RawY; raw5 = compThree.RawY;
            raw6 = compOne.RawZ; raw7 = compTwo.RawZ; raw8 = compThree.RawZ;
        }

        /// <summary>
        /// 变换向量
        /// </summary>
        public TSVector3 Transform(TSVector3 vector)
        {
            return this * vector;
        }

        /// <summary>
        /// 逆变换向量
        /// </summary>
        public TSVector3 TransformTranspose(TSVector3 vector)
        {
            return new TSVector3(
                Fixed64.Mul(vector.RawX, raw0) + Fixed64.Mul(vector.RawY, raw3) + Fixed64.Mul(vector.RawZ, raw6),
                Fixed64.Mul(vector.RawX, raw1) + Fixed64.Mul(vector.RawY, raw4) + Fixed64.Mul(vector.RawZ, raw7),
                Fixed64.Mul(vector.RawX, raw2) + Fixed64.Mul(vector.RawY, raw5) + Fixed64.Mul(vector.RawZ, raw8)
            );
        }

        /// <summary>
        /// 获取矩阵一行
        /// </summary>
        /// <param name="i">行</param>
        public TSVector3 GetRowVector(int i)
        {
            return new TSVector3(this[i * 3], this[i * 3 + 1], this[i * 3 + 2]);
        }

        /// <summary>
        /// 获取矩阵一列
        /// </summary>
        /// <param name="i">列</param>
        public TSVector3 GetAxisVector(int i)
        {
            return new TSVector3(this[i], this[i + 3], this[i + 6]);
        }

        /// <summary>
        /// 变为给定矩阵的逆矩阵
        /// </summary>
        public void SetInverse(Matrix3 m)
        {
            if (m.raw1 == FP.Zero.Raw
                && m.raw2 == FP.Zero.Raw
                && m.raw3 == FP.Zero.Raw
                && m.raw5 == FP.Zero.Raw
                && m.raw6 == FP.Zero.Raw
                && m.raw7 == FP.Zero.Raw)
            {
                data0 = m.data0 == 0 ? 0 : 1 / m.data0;
                data4 = m.data4 == 0 ? 0 : 1 / m.data4;
                data8 = m.data8 == 0 ? 0 : 1 / m.data8;
                return;
            }

            long t4 = Fixed64.Mul(m.raw0, m.raw4);
            long t6 = Fixed64.Mul(m.raw0, m.raw5);
            long t8 = Fixed64.Mul(m.raw1, m.raw3);
            long t10 = Fixed64.Mul(m.raw2, m.raw3);
            long t12 = Fixed64.Mul(m.raw1, m.raw6);
            long t14 = Fixed64.Mul(m.raw2, m.raw6);

            // 计算行列式
            long t16 = Fixed64.Mul(t4, m.raw8) - Fixed64.Mul(t6, m.raw7) - Fixed64.Mul(t4, m.raw8) +
                        Fixed64.Mul(t10, m.raw7) + Fixed64.Mul(t12, m.raw5) - Fixed64.Mul(t14, m.raw4);

            // 确保行列式非0
            if (t16 == FP.Zero.Raw) return;
            long t17 = Fixed64.DivPrecise(FP.One.Raw, t16);

            raw0 = Fixed64.Mul((Fixed64.Mul(m.raw4, m.raw8) - Fixed64.Mul(m.raw5, m.raw7)), t17);
            raw1 = -Fixed64.Mul((Fixed64.Mul(m.raw1, m.raw8) - Fixed64.Mul(m.raw2, m.raw7)), t17);
            raw2 = Fixed64.Mul((Fixed64.Mul(m.raw1, m.raw5) - Fixed64.Mul(m.raw2, m.raw4)), t17); ;
            raw3 = -Fixed64.Mul((Fixed64.Mul(m.raw3, m.raw8) - Fixed64.Mul(m.raw5, m.raw6)), t17); ;
            raw4 = Fixed64.Mul((Fixed64.Mul(m.raw0, m.raw8) - t14), t17);
            raw5 = -Fixed64.Mul((t6 - t10), t17);
            raw6 = Fixed64.Mul((Fixed64.Mul(m.raw3, m.raw7) - Fixed64.Mul(m.raw4, m.raw6)), t17); ;
            raw7 = -Fixed64.Mul((Fixed64.Mul(m.raw0, m.raw7) - t12), t17);
            raw8 = Fixed64.Mul((t4 - t8), t17);
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
        public void SetOrientation(TSQuaternion q)
        {
            long one = FP.One.Raw;
            long two = FP.Two.Raw;

            raw0 = one - (Fixed64.Mul(two, Fixed64.Mul(q.RawY, q.RawY)) + Fixed64.Mul(two, Fixed64.Mul(q.RawZ, q.RawZ)));
            raw1 = Fixed64.Mul(two, Fixed64.Mul(q.RawX, q.RawY)) - Fixed64.Mul(two, Fixed64.Mul(q.RawZ, q.RawW));
            raw2 = Fixed64.Mul(two, Fixed64.Mul(q.RawX, q.RawY)) + Fixed64.Mul(two, Fixed64.Mul(q.RawY, q.RawW));

            raw3 = Fixed64.Mul(two, Fixed64.Mul(q.RawX, q.RawY)) + Fixed64.Mul(two, Fixed64.Mul(q.RawZ, q.RawW));
            raw4 = one - (Fixed64.Mul(two, Fixed64.Mul(q.RawX, q.RawX)) + Fixed64.Mul(two, Fixed64.Mul(q.RawZ, q.RawZ)));
            raw5 = Fixed64.Mul(two, Fixed64.Mul(q.RawY, q.RawZ)) - Fixed64.Mul(two, Fixed64.Mul(q.RawX, q.RawW));

            raw6 = Fixed64.Mul(two, Fixed64.Mul(q.RawX, q.RawZ)) - Fixed64.Mul(two, Fixed64.Mul(q.RawY, q.RawW));
            raw7 = Fixed64.Mul(two, Fixed64.Mul(q.RawY, q.RawZ)) + Fixed64.Mul(two, Fixed64.Mul(q.RawX, q.RawW));
            raw8 = one - (Fixed64.Mul(two, Fixed64.Mul(q.RawX, q.RawX)) + Fixed64.Mul(two, Fixed64.Mul(q.RawY, q.RawY)));
        }
    }
}