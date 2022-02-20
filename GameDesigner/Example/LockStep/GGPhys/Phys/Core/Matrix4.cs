using FixPointCS;
using System;
using System.Collections.Generic;
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
    /// 4x3矩阵
    /// </summary>
    public struct Matrix4
    {

        public RawType raw0, raw1, raw2, raw3;
        public RawType raw4, raw5, raw6, raw7;
        public RawType raw8, raw9, raw10, raw11;

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
        public static Matrix4 IdentityOffset(Vector3d offset)
        {
            var m = Identity;
            m.raw3 += offset.x.Raw;
            m.raw7 += offset.y.Raw;
            m.raw11 += offset.z.Raw;
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

        private Matrix4(RawType m0, RawType m1, RawType m2, RawType m3,
                       RawType m4, RawType m5, RawType m6, RawType m7,
                       RawType m8, RawType m9, RawType m10, RawType m11)
        {
            raw0 = m0; raw1 = m1; raw2 = m2; raw3 = m3;
            raw4 = m4; raw5 = m5; raw6 = m6; raw7 = m7;
            raw8 = m8; raw9 = m9; raw10 = m10; raw11 = m11;
        }

        /// <summary>
        /// 下标读写
        /// </summary>
        unsafe public REAL this[int i]
        {
            get
            {
                if ((uint)i >= 12)
                    throw new IndexOutOfRangeException("Matrix4 index out of range.");

                fixed (Matrix4* array = &this) { return REAL.FromRaw(((RawType*)array)[i]); }
            }
            set
            {
                if ((uint)i >= 12)
                    throw new IndexOutOfRangeException("Matrix4 index out of range.");

                fixed (RawType* array = &raw0) { array[i] = value.Raw; }
            }
        }

        /// <summary>
        /// 矩阵乘法
        /// </summary>
        public static Matrix4 operator *(Matrix4 m1, Matrix4 m2)
        {
            Matrix4 result = new Matrix4();
            result.raw0 = FixedType.Mul(m2.raw0, m1.raw0) + FixedType.Mul(m2.raw4, m1.raw1) + FixedType.Mul(m2.raw8, m1.raw2);
            result.raw4 = FixedType.Mul(m2.raw0, m1.raw4) + FixedType.Mul(m2.raw4, m1.raw5) + FixedType.Mul(m2.raw8, m1.raw6);
            result.raw8 = FixedType.Mul(m2.raw0, m1.raw8) + FixedType.Mul(m2.raw4, m1.raw9) + FixedType.Mul(m2.raw8, m1.raw10);

            result.raw1 = FixedType.Mul(m2.raw1, m1.raw0) + FixedType.Mul(m2.raw5, m1.raw1) + FixedType.Mul(m2.raw9, m1.raw2);
            result.raw5 = FixedType.Mul(m2.raw1, m1.raw4) + FixedType.Mul(m2.raw5, m1.raw5) + FixedType.Mul(m2.raw9, m1.raw6);
            result.raw9 = FixedType.Mul(m2.raw1, m1.raw8) + FixedType.Mul(m2.raw5, m1.raw9) + FixedType.Mul(m2.raw9, m1.raw10);

            result.raw2 = FixedType.Mul(m2.raw2, m1.raw0) + FixedType.Mul(m2.raw6, m1.raw1) + FixedType.Mul(m2.raw10, m1.raw2);
            result.raw6 = FixedType.Mul(m2.raw2, m1.raw4) + FixedType.Mul(m2.raw6, m1.raw5) + FixedType.Mul(m2.raw10, m1.raw6);
            result.raw10 = FixedType.Mul(m2.raw2, m1.raw8) + FixedType.Mul(m2.raw6, m1.raw9) + FixedType.Mul(m2.raw10, m1.raw10);

            result.raw3 = FixedType.Mul(m2.raw3, m1.raw0) + FixedType.Mul(m2.raw7, m1.raw1) + FixedType.Mul(m2.raw11, m1.raw2) + m1.raw3;
            result.raw7 = FixedType.Mul(m2.raw3, m1.raw4) + FixedType.Mul(m2.raw7, m1.raw5) + FixedType.Mul(m2.raw11, m1.raw6) + m1.raw7;
            result.raw11 = FixedType.Mul(m2.raw3, m1.raw8) + FixedType.Mul(m2.raw7, m1.raw9) + FixedType.Mul(m2.raw11, m1.raw10) + m1.raw11;

            return result;
        }

        /// <summary>
        /// 矩阵乘向量
        /// </summary>
        public static Vector3d operator *(Matrix4 m, Vector3d vector)
        {
            return Vector3d.FromRaw(FixedType.Mul(vector.x.Raw, m.raw0) + FixedType.Mul(vector.y.Raw, m.raw1) + FixedType.Mul(vector.z.Raw, m.raw2) + m.raw3,
                                     FixedType.Mul(vector.x.Raw, m.raw4) + FixedType.Mul(vector.y.Raw, m.raw5) + FixedType.Mul(vector.z.Raw, m.raw6) + m.raw7,
                                     FixedType.Mul(vector.x.Raw, m.raw8) + FixedType.Mul(vector.y.Raw, m.raw9) + FixedType.Mul(vector.z.Raw, m.raw10) + m.raw11);
        }

        /// <summary>
        /// 转为字符串
        /// </summary>
        public override string ToString()
        {
            return data0 + "," + data1 + "," + data2 + "," + data3 + "\n" +
                   data4 + "," + data5 + "," + data6 + "," + data7 + "\n" +
                   data8 + "," + data9 + "," + data10 + "," + data11;
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
        public Vector3d Transform(Vector3d vector)
        {
            return this * vector;
        }

        /// <summary>
        /// 获取行列式
        /// </summary>
        public RawType GetDeterminant()
        {
            return - FixedType.Mul(raw8, FixedType.Mul(raw5, raw2))
                   + FixedType.Mul(raw4, FixedType.Mul(raw9, raw2))
                   + FixedType.Mul(raw8, FixedType.Mul(raw1, raw6))
                   - FixedType.Mul(raw0, FixedType.Mul(raw9, raw6))
                   - FixedType.Mul(raw4, FixedType.Mul(raw1, raw10))
                   + FixedType.Mul(raw0, FixedType.Mul(raw5, raw10));
        }

        /// <summary>
        /// 赋值为给定向量的逆矩阵
        /// </summary>
        public void SetInverse(Matrix4 m)
        {
            // Make sure the determinant is non-zero.
            RawType det = m.GetDeterminant();
            if (det == REAL.Zero.Raw) return;
            det = FixedType.DivPrecise(REAL.One.Raw, det);

            raw0 = FixedType.Mul(-(FixedType.Mul(raw9, raw6) + FixedType.Mul(raw5, raw10)), det);
            raw4 = FixedType.Mul((FixedType.Mul(raw8, raw6) - FixedType.Mul(raw4, raw10)), det);
            raw8 = FixedType.Mul(-(FixedType.Mul(raw8, raw5) + FixedType.Mul(raw4, raw9)), det);

            raw1 = FixedType.Mul((FixedType.Mul(raw9, raw2) - FixedType.Mul(raw1, raw10)), det);
            raw5 = FixedType.Mul(-(FixedType.Mul(raw8, raw2) + FixedType.Mul(raw0, raw10)), det);
            raw9 = FixedType.Mul((FixedType.Mul(raw8, raw1) - FixedType.Mul(raw0, raw9)), det);

            raw2 = FixedType.Mul(-(FixedType.Mul(raw5, raw2) + FixedType.Mul(raw1, raw6)), det);
            raw6 = FixedType.Mul((FixedType.Mul(raw4, raw2) - FixedType.Mul(raw0, raw6)), det);
            raw10 = FixedType.Mul(-(FixedType.Mul(raw4, raw1) + FixedType.Mul(raw0, raw5)), det);

            raw3 = FixedType.Mul(
                     FixedType.Mul(raw9, FixedType.Mul(raw6, raw3))
                   - FixedType.Mul(raw5, FixedType.Mul(raw10, raw3))
                   - FixedType.Mul(raw9, FixedType.Mul(raw2, raw7))
                   + FixedType.Mul(raw1, FixedType.Mul(raw10, raw7))
                   + FixedType.Mul(raw5, FixedType.Mul(raw2, raw11))
                   - FixedType.Mul(raw1, FixedType.Mul(raw6, raw11)), det);

            raw7 = FixedType.Mul(
                   - FixedType.Mul(raw8, FixedType.Mul(raw6, raw3))
                   + FixedType.Mul(raw4, FixedType.Mul(raw10, raw3))
                   + FixedType.Mul(raw8, FixedType.Mul(raw2, raw7))
                   - FixedType.Mul(raw0, FixedType.Mul(raw10, raw7))
                   - FixedType.Mul(raw4, FixedType.Mul(raw2, raw11))
                   + FixedType.Mul(raw0, FixedType.Mul(raw6, raw11)), det);

            raw11 = FixedType.Mul(
                     FixedType.Mul(raw8, FixedType.Mul(raw5, raw3))
                   - FixedType.Mul(raw4, FixedType.Mul(raw9, raw3))
                   - FixedType.Mul(raw8, FixedType.Mul(raw1, raw7))
                   + FixedType.Mul(raw0, FixedType.Mul(raw9, raw7))
                   + FixedType.Mul(raw4, FixedType.Mul(raw1, raw11))
                   - FixedType.Mul(raw0, FixedType.Mul(raw5, raw11)), det);
        }

        /// <summary>
        /// 返回矩阵的逆矩阵
        /// </summary>
        public Matrix4 Inverse()
        {
            Matrix4 result = Matrix4.Identity;
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
        public Vector3d TransformDirection(Vector3d vector)
        {
            return Vector3d.FromRaw(FixedType.Mul(vector.x.Raw, raw0) + FixedType.Mul(vector.y.Raw, raw1) + FixedType.Mul(vector.z.Raw, raw2),
                                     FixedType.Mul(vector.x.Raw, raw4) + FixedType.Mul(vector.y.Raw, raw5) + FixedType.Mul(vector.z.Raw, raw6),
                                     FixedType.Mul(vector.x.Raw, raw8) + FixedType.Mul(vector.y.Raw, raw9) + FixedType.Mul(vector.z.Raw, raw10));
        }

        /// <summary>
        /// 逆变换给定向量
        /// </summary>
        public Vector3d TransformInverseDirection(Vector3d vector)
        {
            return Vector3d.FromRaw(FixedType.Mul(vector.x.Raw, raw0) + FixedType.Mul(vector.y.Raw, raw4) + FixedType.Mul(vector.z.Raw, raw8),
                                     FixedType.Mul(vector.x.Raw, raw1) + FixedType.Mul(vector.y.Raw, raw5) + FixedType.Mul(vector.z.Raw, raw9),
                                     FixedType.Mul(vector.x.Raw, raw2) + FixedType.Mul(vector.y.Raw, raw6) + FixedType.Mul(vector.z.Raw, raw10));
        }

        /// <summary>
        /// 逆变换点
        /// </summary>
        public Vector3d TransformInverse(Vector3d vector)
        {
            Vector3d tmp = vector;
            tmp.x.Raw -= raw3;
            tmp.y.Raw -= raw7;
            tmp.z.Raw -= raw11;
            return Vector3d.FromRaw(FixedType.Mul(tmp.x.Raw, raw0) + FixedType.Mul(tmp.y.Raw, raw4) + FixedType.Mul(tmp.z.Raw, raw8),
                                     FixedType.Mul(tmp.x.Raw, raw1) + FixedType.Mul(tmp.y.Raw, raw5) + FixedType.Mul(tmp.z.Raw, raw9),
                                     FixedType.Mul(tmp.x.Raw, raw2) + FixedType.Mul(tmp.y.Raw, raw6) + FixedType.Mul(tmp.z.Raw, raw10));
        }

        /// <summary>
        /// 变换给定向量
        /// </summary>
        public Matrix3 TransformMatrix3(Matrix3 mt)
        {
            return Matrix3.FromRaw(FixedType.Mul(mt.raw0, raw0) + FixedType.Mul(mt.raw3, raw1) + FixedType.Mul(mt.raw6, raw2),
                                    FixedType.Mul(mt.raw1, raw0) + FixedType.Mul(mt.raw4, raw1) + FixedType.Mul(mt.raw7, raw2),
                                    FixedType.Mul(mt.raw2, raw0) + FixedType.Mul(mt.raw5, raw1) + FixedType.Mul(mt.raw8, raw2),
                                    FixedType.Mul(mt.raw0, raw4) + FixedType.Mul(mt.raw3, raw5) + FixedType.Mul(mt.raw6, raw6),
                                    FixedType.Mul(mt.raw1, raw4) + FixedType.Mul(mt.raw4, raw5) + FixedType.Mul(mt.raw7, raw6),
                                    FixedType.Mul(mt.raw2, raw4) + FixedType.Mul(mt.raw5, raw5) + FixedType.Mul(mt.raw8, raw6),
                                    FixedType.Mul(mt.raw0, raw8) + FixedType.Mul(mt.raw3, raw9) + FixedType.Mul(mt.raw6, raw10),
                                    FixedType.Mul(mt.raw1, raw8) + FixedType.Mul(mt.raw4, raw9) + FixedType.Mul(mt.raw7, raw10),
                                    FixedType.Mul(mt.raw2, raw8) + FixedType.Mul(mt.raw5, raw9) + FixedType.Mul(mt.raw8, raw10));
        }

        /// <summary>
        /// 获取一列
        /// <summary>
        public Vector3d GetAxisVector(int i)
        {
            return new Vector3d(this[i], this[i + 4], this[i + 8]);
        }

        /// <summary>
        /// 根据四元数旋转和位置进行赋值
        /// </summary>
        /// <param name="q"></param>
        /// <param name="pos"></param>
        public void SetOrientationAndPos(Quaternion q, Vector3d pos)
        {
            RawType one = REAL.One.Raw;
            RawType two = REAL.Two.Raw;

            raw0 = one - (FixedType.Mul(two, FixedType.Mul(q.y.Raw, q.y.Raw)) + FixedType.Mul(two, FixedType.Mul(q.z.Raw, q.z.Raw)));
            raw1 = FixedType.Mul(two, FixedType.Mul(q.x.Raw, q.y.Raw)) - FixedType.Mul(two, FixedType.Mul(q.z.Raw, q.w.Raw));
            raw2 = FixedType.Mul(two, FixedType.Mul(q.x.Raw, q.z.Raw)) + FixedType.Mul(two, FixedType.Mul(q.y.Raw, q.w.Raw));
            raw3 = pos.x.Raw;

            raw4 = FixedType.Mul(two, FixedType.Mul(q.x.Raw, q.y.Raw)) + FixedType.Mul(two, FixedType.Mul(q.z.Raw, q.w.Raw));
            raw5 = one - (FixedType.Mul(two, FixedType.Mul(q.x.Raw, q.x.Raw)) + FixedType.Mul(two, FixedType.Mul(q.z.Raw, q.z.Raw)));
            raw6 = FixedType.Mul(two, FixedType.Mul(q.y.Raw, q.z.Raw)) - FixedType.Mul(two, FixedType.Mul(q.x.Raw, q.w.Raw));
            raw7 = pos.y.Raw;

            raw8 = FixedType.Mul(two, FixedType.Mul(q.x.Raw, q.z.Raw)) - FixedType.Mul(two, FixedType.Mul(q.y.Raw, q.w.Raw));
            raw9 = FixedType.Mul(two, FixedType.Mul(q.y.Raw, q.z.Raw)) + FixedType.Mul(two, FixedType.Mul(q.x.Raw, q.w.Raw));
            raw10 = one - (FixedType.Mul(two, FixedType.Mul(q.x.Raw, q.x.Raw)) + FixedType.Mul(two, FixedType.Mul(q.y.Raw, q.y.Raw)));
            raw11 = pos.z.Raw;
        }
    }

}