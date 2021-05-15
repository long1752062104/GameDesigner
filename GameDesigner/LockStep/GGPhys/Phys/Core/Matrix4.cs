using FixPointCS;
using System;
using TrueSync;
using REAL = FixMath.F64;

namespace GGPhys.Core
{

    /// <summary>
    /// 4x3矩阵
    /// </summary>
    public struct Matrix4
    {
        public long raw0, raw1, raw2, raw3;
        public long raw4, raw5, raw6, raw7;
        public long raw8, raw9, raw10, raw11;

        public REAL data0 { get { return REAL.FromRaw(raw0); } set { raw0 = value.Raw; } }
        public REAL data1 { get { return REAL.FromRaw(raw1); } set { raw1 = value.Raw; } }
        public REAL data2 { get { return REAL.FromRaw(raw2); } set { raw2 = value.Raw; } }
        public REAL data3 { get { return REAL.FromRaw(raw3); } set { raw3 = value.Raw; } }
        public REAL data4 { get { return REAL.FromRaw(raw4); } set { raw4 = value.Raw; } }
        public REAL data5 { get { return REAL.FromRaw(raw5); } set { raw5 = value.Raw; } }
        public REAL data6 { get { return REAL.FromRaw(raw6); } set { raw6 = value.Raw; } }
        public REAL data7 { get { return REAL.FromRaw(raw7); } set { raw7 = value.Raw; } }
        public REAL data8 { get { return REAL.FromRaw(raw8); } set { raw8 = value.Raw; } }
        public REAL data9 { get { return REAL.FromRaw(raw9); } set { raw9 = value.Raw; } }
        public REAL data10 { get { return REAL.FromRaw(raw10); } set { raw10 = value.Raw; } }
        public REAL data11 { get { return REAL.FromRaw(raw11); } set { raw11 = value.Raw; } }


        /// <summary>
        /// 构造单位矩阵
        /// </summary>
        static readonly public Matrix4 Identity = new Matrix4(REAL.One, REAL.Zero, REAL.Zero, REAL.Zero,
                                                              REAL.Zero, REAL.One, REAL.Zero, REAL.Zero,
                                                              REAL.Zero, REAL.Zero, REAL.One, REAL.Zero);

        /// <summary>
        /// 构造带偏移量的单位矩阵
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Matrix4 IdentityOffset(TSVector3 offset)
        {
            Matrix4 m = Identity;
            m.raw3 += offset.RawX;
            m.raw7 += offset.RawY;
            m.raw11 += offset.RawZ;
            return m;
        }


        public Matrix4(REAL m0, REAL m1, REAL m2, REAL m3,
                       REAL m4, REAL m5, REAL m6, REAL m7,
                       REAL m8, REAL m9, REAL m10, REAL m11)
        {
            raw0 = m0.Raw; raw1 = m1.Raw; raw2 = m2.Raw; raw3 = m3.Raw;
            raw4 = m4.Raw; raw5 = m5.Raw; raw6 = m6.Raw; raw7 = m7.Raw;
            raw8 = m8.Raw; raw9 = m9.Raw; raw10 = m10.Raw; raw11 = m11.Raw;
        }

        private Matrix4(long m0, long m1, long m2, long m3,
                       long m4, long m5, long m6, long m7,
                       long m8, long m9, long m10, long m11)
        {
            raw0 = m0; raw1 = m1; raw2 = m2; raw3 = m3;
            raw4 = m4; raw5 = m5; raw6 = m6; raw7 = m7;
            raw8 = m8; raw9 = m9; raw10 = m10; raw11 = m11;
        }

        /// <summary>
        /// 下标读写
        /// </summary>
        unsafe public FP this[int i]
        {
            get
            {
                if ((uint)i >= 12)
                    throw new IndexOutOfRangeException("Matrix4 index out of range.");

                fixed (Matrix4* array = &this) { return new FP(((long*)array)[i]); }
            }
            set
            {
                if ((uint)i >= 12)
                    throw new IndexOutOfRangeException("Matrix4 index out of range.");

                fixed (long* array = &raw0) { array[i] = value.Raw; }
            }
        }

        /// <summary>
        /// 矩阵乘法
        /// </summary>
        public static Matrix4 operator *(Matrix4 m1, Matrix4 m2)
        {
            Matrix4 result = new Matrix4
            {
                raw0 = Fixed64.Mul(m2.raw0, m1.raw0) + Fixed64.Mul(m2.raw4, m1.raw1) + Fixed64.Mul(m2.raw8, m1.raw2),
                raw4 = Fixed64.Mul(m2.raw0, m1.raw4) + Fixed64.Mul(m2.raw4, m1.raw5) + Fixed64.Mul(m2.raw8, m1.raw6),
                raw8 = Fixed64.Mul(m2.raw0, m1.raw8) + Fixed64.Mul(m2.raw4, m1.raw9) + Fixed64.Mul(m2.raw8, m1.raw10),

                raw1 = Fixed64.Mul(m2.raw1, m1.raw0) + Fixed64.Mul(m2.raw5, m1.raw1) + Fixed64.Mul(m2.raw9, m1.raw2),
                raw5 = Fixed64.Mul(m2.raw1, m1.raw4) + Fixed64.Mul(m2.raw5, m1.raw5) + Fixed64.Mul(m2.raw9, m1.raw6),
                raw9 = Fixed64.Mul(m2.raw1, m1.raw8) + Fixed64.Mul(m2.raw5, m1.raw9) + Fixed64.Mul(m2.raw9, m1.raw10),

                raw2 = Fixed64.Mul(m2.raw2, m1.raw0) + Fixed64.Mul(m2.raw6, m1.raw1) + Fixed64.Mul(m2.raw10, m1.raw2),
                raw6 = Fixed64.Mul(m2.raw2, m1.raw4) + Fixed64.Mul(m2.raw6, m1.raw5) + Fixed64.Mul(m2.raw10, m1.raw6),
                raw10 = Fixed64.Mul(m2.raw2, m1.raw8) + Fixed64.Mul(m2.raw6, m1.raw9) + Fixed64.Mul(m2.raw10, m1.raw10),

                raw3 = Fixed64.Mul(m2.raw3, m1.raw0) + Fixed64.Mul(m2.raw7, m1.raw1) + Fixed64.Mul(m2.raw11, m1.raw2) + m1.raw3,
                raw7 = Fixed64.Mul(m2.raw3, m1.raw4) + Fixed64.Mul(m2.raw7, m1.raw5) + Fixed64.Mul(m2.raw11, m1.raw6) + m1.raw7,
                raw11 = Fixed64.Mul(m2.raw3, m1.raw8) + Fixed64.Mul(m2.raw7, m1.raw9) + Fixed64.Mul(m2.raw11, m1.raw10) + m1.raw11
            };

            return result;
        }

        /// <summary>
        /// 矩阵乘向量
        /// </summary>
        public static TSVector3 operator *(Matrix4 m, TSVector3 vector)
        {
            return new TSVector3(Fixed64.Mul(vector.RawX, m.raw0) + Fixed64.Mul(vector.RawY, m.raw1) + Fixed64.Mul(vector.RawZ, m.raw2) + m.raw3,
                                     Fixed64.Mul(vector.RawX, m.raw4) + Fixed64.Mul(vector.RawY, m.raw5) + Fixed64.Mul(vector.RawZ, m.raw6) + m.raw7,
                                     Fixed64.Mul(vector.RawX, m.raw8) + Fixed64.Mul(vector.RawY, m.raw9) + Fixed64.Mul(vector.RawZ, m.raw10) + m.raw11);
        }

        /// <summary>
        /// 转为字符串
        /// </summary>
        public override string ToString()
        {
            return "{" + data0 + "," + data1 + "," + data2 + "," + data3 + "},{" +
                   data4 + "," + data5 + "," + data6 + "," + data7 + "},{" +
                   data8 + "," + data9 + "," + data10 + "," + data11 + "}";
        }

        /// <summary>
        /// 对角赋值
        /// </summary>
        public void SetDiagonal(REAL a, REAL b, REAL c)
        {
            raw0 = a.Raw;
            raw5 = b.Raw;
            raw10 = c.Raw;
        }

        /// <summary>
        /// 变换给定点
        /// </summary>
        public TSVector3 Transform(TSVector3 vector)
        {
            return this * vector;
        }

        /// <summary>
        /// 获取行列式
        /// </summary>
        public long GetDeterminant()
        {
            return -Fixed64.Mul(raw8, Fixed64.Mul(raw5, raw2))
                   + Fixed64.Mul(raw4, Fixed64.Mul(raw9, raw2))
                   + Fixed64.Mul(raw8, Fixed64.Mul(raw1, raw6))
                   - Fixed64.Mul(raw0, Fixed64.Mul(raw9, raw6))
                   - Fixed64.Mul(raw4, Fixed64.Mul(raw1, raw10))
                   + Fixed64.Mul(raw0, Fixed64.Mul(raw5, raw10));
        }

        /// <summary>
        /// 赋值为给定向量的逆矩阵
        /// </summary>
        public void SetInverse(Matrix4 m)
        {
            // Make sure the determinant is non-zero.
            long det = m.GetDeterminant();
            if (det == REAL.Zero.Raw) return;
            det = Fixed64.DivPrecise(REAL.One.Raw, det);

            raw0 = Fixed64.Mul(-(Fixed64.Mul(raw9, raw6) + Fixed64.Mul(raw5, raw10)), det);
            raw4 = Fixed64.Mul((Fixed64.Mul(raw8, raw6) - Fixed64.Mul(raw4, raw10)), det);
            raw8 = Fixed64.Mul(-(Fixed64.Mul(raw8, raw5) + Fixed64.Mul(raw4, raw9)), det);

            raw1 = Fixed64.Mul((Fixed64.Mul(raw9, raw2) - Fixed64.Mul(raw1, raw10)), det);
            raw5 = Fixed64.Mul(-(Fixed64.Mul(raw8, raw2) + Fixed64.Mul(raw0, raw10)), det);
            raw9 = Fixed64.Mul((Fixed64.Mul(raw8, raw1) - Fixed64.Mul(raw0, raw9)), det);

            raw2 = Fixed64.Mul(-(Fixed64.Mul(raw5, raw2) + Fixed64.Mul(raw1, raw6)), det);
            raw6 = Fixed64.Mul((Fixed64.Mul(raw4, raw2) - Fixed64.Mul(raw0, raw6)), det);
            raw10 = Fixed64.Mul(-(Fixed64.Mul(raw4, raw1) + Fixed64.Mul(raw0, raw5)), det);

            raw3 = Fixed64.Mul(
                     Fixed64.Mul(raw9, Fixed64.Mul(raw6, raw3))
                   - Fixed64.Mul(raw5, Fixed64.Mul(raw10, raw3))
                   - Fixed64.Mul(raw9, Fixed64.Mul(raw2, raw7))
                   + Fixed64.Mul(raw1, Fixed64.Mul(raw10, raw7))
                   + Fixed64.Mul(raw5, Fixed64.Mul(raw2, raw11))
                   - Fixed64.Mul(raw1, Fixed64.Mul(raw6, raw11)), det);

            raw7 = Fixed64.Mul(
                   -Fixed64.Mul(raw8, Fixed64.Mul(raw6, raw3))
                   + Fixed64.Mul(raw4, Fixed64.Mul(raw10, raw3))
                   + Fixed64.Mul(raw8, Fixed64.Mul(raw2, raw7))
                   - Fixed64.Mul(raw0, Fixed64.Mul(raw10, raw7))
                   - Fixed64.Mul(raw4, Fixed64.Mul(raw2, raw11))
                   + Fixed64.Mul(raw0, Fixed64.Mul(raw6, raw11)), det);

            raw11 = Fixed64.Mul(
                     Fixed64.Mul(raw8, Fixed64.Mul(raw5, raw3))
                   - Fixed64.Mul(raw4, Fixed64.Mul(raw9, raw3))
                   - Fixed64.Mul(raw8, Fixed64.Mul(raw1, raw7))
                   + Fixed64.Mul(raw0, Fixed64.Mul(raw9, raw7))
                   + Fixed64.Mul(raw4, Fixed64.Mul(raw1, raw11))
                   - Fixed64.Mul(raw0, Fixed64.Mul(raw5, raw11)), det);
        }

        /// <summary>
        /// 返回矩阵的逆矩阵
        /// </summary>
        public Matrix4 Inverse()
        {
            Matrix4 result = Identity;
            result.SetInverse(this);
            return result;
        }

        /// <summary>
        /// 变为逆矩阵
        /// </summary>
        public void Invert()
        {
            SetInverse(this);
        }

        /// <summary>
        /// 变换给定向量
        /// </summary>
        public TSVector3 TransformDirection(TSVector3 vector)
        {
            return new TSVector3(Fixed64.Mul(vector.RawX, raw0) + Fixed64.Mul(vector.RawY, raw1) + Fixed64.Mul(vector.RawZ, raw2),
                                     Fixed64.Mul(vector.RawX, raw4) + Fixed64.Mul(vector.RawY, raw5) + Fixed64.Mul(vector.RawZ, raw6),
                                     Fixed64.Mul(vector.RawX, raw8) + Fixed64.Mul(vector.RawY, raw9) + Fixed64.Mul(vector.RawZ, raw10));
        }

        /// <summary>
        /// 逆变换给定向量
        /// </summary>
        public TSVector3 TransformInverseDirection(TSVector3 vector)
        {
            return new TSVector3(Fixed64.Mul(vector.RawX, raw0) + Fixed64.Mul(vector.RawY, raw4) + Fixed64.Mul(vector.RawZ, raw8),
                                     Fixed64.Mul(vector.RawX, raw1) + Fixed64.Mul(vector.RawY, raw5) + Fixed64.Mul(vector.RawZ, raw9),
                                     Fixed64.Mul(vector.RawX, raw2) + Fixed64.Mul(vector.RawY, raw6) + Fixed64.Mul(vector.RawZ, raw10));
        }

        /// <summary>
        /// 逆变换点
        /// </summary>
        public TSVector3 TransformInverse(TSVector3 vector)
        {
            TSVector3 tmp = vector;
            tmp.RawX -= raw3;
            tmp.RawY -= raw7;
            tmp.RawZ -= raw11;
            return new TSVector3(Fixed64.Mul(tmp.RawX, raw0) + Fixed64.Mul(tmp.RawY, raw4) + Fixed64.Mul(tmp.RawZ, raw8),
                                 Fixed64.Mul(tmp.RawX, raw1) + Fixed64.Mul(tmp.RawY, raw5) + Fixed64.Mul(tmp.RawZ, raw9),
                                 Fixed64.Mul(tmp.RawX, raw2) + Fixed64.Mul(tmp.RawY, raw6) + Fixed64.Mul(tmp.RawZ, raw10));
        }

        /// <summary>
        /// 获取一列
        /// <summary>
        public TSVector3 GetAxisVector(int i)
        {
#pragma warning disable IDE0066 // 将 switch 语句转换为表达式
            switch (i)
#pragma warning restore IDE0066 // 将 switch 语句转换为表达式
            {
                case 0:
                    return new TSVector3(raw0, raw4, raw8);
                case 1:
                    return new TSVector3(raw1, raw5, raw9);
                case 2:
                    return new TSVector3(raw2, raw6, raw10);
                case 3:
                    return new TSVector3(raw3, raw7, raw11);
                default:
                    return new TSVector3(this[i], this[i + 4], this[i + 8]);
            }
        }

        /// <summary>
        /// 根据四元数旋转和位置进行赋值
        /// </summary>
        /// <param name="q"></param>
        /// <param name="pos"></param>
        public void SetOrientationAndPos(TSQuaternion q, TSVector3 pos)
        {
            long one = REAL.One.Raw;
            long two = REAL.Two.Raw;

            raw0 = one - (Fixed64.Mul(two, Fixed64.Mul(q.RawY, q.RawY)) + Fixed64.Mul(two, Fixed64.Mul(q.RawZ, q.RawZ)));
            raw1 = Fixed64.Mul(two, Fixed64.Mul(q.RawX, q.RawY)) - Fixed64.Mul(two, Fixed64.Mul(q.RawZ, q.RawW));
            raw2 = Fixed64.Mul(two, Fixed64.Mul(q.RawX, q.RawZ)) + Fixed64.Mul(two, Fixed64.Mul(q.RawY, q.RawW));
            raw3 = pos.RawX;

            raw4 = Fixed64.Mul(two, Fixed64.Mul(q.RawX, q.RawY)) + Fixed64.Mul(two, Fixed64.Mul(q.RawZ, q.RawW));
            raw5 = one - (Fixed64.Mul(two, Fixed64.Mul(q.RawX, q.RawX)) + Fixed64.Mul(two, Fixed64.Mul(q.RawZ, q.RawZ)));
            raw6 = Fixed64.Mul(two, Fixed64.Mul(q.RawY, q.RawZ)) - Fixed64.Mul(two, Fixed64.Mul(q.RawX, q.RawW));
            raw7 = pos.RawY;

            raw8 = Fixed64.Mul(two, Fixed64.Mul(q.RawX, q.RawZ)) - Fixed64.Mul(two, Fixed64.Mul(q.RawY, q.RawW));
            raw9 = Fixed64.Mul(two, Fixed64.Mul(q.RawY, q.RawZ)) + Fixed64.Mul(two, Fixed64.Mul(q.RawX, q.RawW));
            raw10 = one - (Fixed64.Mul(two, Fixed64.Mul(q.RawX, q.RawX)) + Fixed64.Mul(two, Fixed64.Mul(q.RawY, q.RawY)));
            raw11 = pos.RawZ;
        }
    }

}