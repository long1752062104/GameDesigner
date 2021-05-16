using System;

namespace Net
{
    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
    [Serializable]
    public struct Matrix4x4 : IEquatable<Matrix4x4>
    {
        #region 源码
        public Matrix4x4(Vector4 column0, Vector4 column1, Vector4 column2, Vector4 column3)
        {
            m00 = column0.x;
            m01 = column1.x;
            m02 = column2.x;
            m03 = column3.x;
            m10 = column0.y;
            m11 = column1.y;
            m12 = column2.y;
            m13 = column3.y;
            m20 = column0.z;
            m21 = column1.z;
            m22 = column2.z;
            m23 = column3.z;
            m30 = column0.w;
            m31 = column1.w;
            m32 = column2.w;
            m33 = column3.w;
        }

        // Token: 0x170010FE RID: 4350
        public float this[int row, int column]
        {
            get
            {
                return this[row + column * 4];
            }
            set
            {
                this[row + column * 4] = value;
            }
        }

        // Token: 0x170010FF RID: 4351
        public float this[int index]
        {
            get
            {
                float result;
                switch (index)
                {
                    case 0:
                        result = m00;
                        break;
                    case 1:
                        result = m10;
                        break;
                    case 2:
                        result = m20;
                        break;
                    case 3:
                        result = m30;
                        break;
                    case 4:
                        result = m01;
                        break;
                    case 5:
                        result = m11;
                        break;
                    case 6:
                        result = m21;
                        break;
                    case 7:
                        result = m31;
                        break;
                    case 8:
                        result = m02;
                        break;
                    case 9:
                        result = m12;
                        break;
                    case 10:
                        result = m22;
                        break;
                    case 11:
                        result = m32;
                        break;
                    case 12:
                        result = m03;
                        break;
                    case 13:
                        result = m13;
                        break;
                    case 14:
                        result = m23;
                        break;
                    case 15:
                        result = m33;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid matrix index!");
                }
                return result;
            }
            set
            {
                switch (index)
                {
                    case 0:
                        m00 = value;
                        break;
                    case 1:
                        m10 = value;
                        break;
                    case 2:
                        m20 = value;
                        break;
                    case 3:
                        m30 = value;
                        break;
                    case 4:
                        m01 = value;
                        break;
                    case 5:
                        m11 = value;
                        break;
                    case 6:
                        m21 = value;
                        break;
                    case 7:
                        m31 = value;
                        break;
                    case 8:
                        m02 = value;
                        break;
                    case 9:
                        m12 = value;
                        break;
                    case 10:
                        m22 = value;
                        break;
                    case 11:
                        m32 = value;
                        break;
                    case 12:
                        m03 = value;
                        break;
                    case 13:
                        m13 = value;
                        break;
                    case 14:
                        m23 = value;
                        break;
                    case 15:
                        m33 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid matrix index!");
                }
            }
        }

        // Token: 0x060048B7 RID: 18615 RVA: 0x0007C1D8 File Offset: 0x0007A3D8
        public override int GetHashCode()
        {
            return GetColumn(0).GetHashCode() ^ GetColumn(1).GetHashCode() << 2 ^ GetColumn(2).GetHashCode() >> 2 ^ GetColumn(3).GetHashCode() >> 1;
        }

        // Token: 0x060048B8 RID: 18616 RVA: 0x0007C24C File Offset: 0x0007A44C
        public override bool Equals(object other)
        {
            return other is Matrix4x4 && Equals((Matrix4x4)other);
        }

        // Token: 0x060048B9 RID: 18617 RVA: 0x0007C280 File Offset: 0x0007A480
        public bool Equals(Matrix4x4 other)
        {
            return GetColumn(0).Equals(other.GetColumn(0)) && GetColumn(1).Equals(other.GetColumn(1)) && GetColumn(2).Equals(other.GetColumn(2)) && GetColumn(3).Equals(other.GetColumn(3));
        }

        // Token: 0x060048BA RID: 18618 RVA: 0x0007C308 File Offset: 0x0007A508
        public static Matrix4x4 operator *(Matrix4x4 lhs, Matrix4x4 rhs)
        {
            Matrix4x4 result;
            result.m00 = lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20 + lhs.m03 * rhs.m30;
            result.m01 = lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21 + lhs.m03 * rhs.m31;
            result.m02 = lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22 + lhs.m03 * rhs.m32;
            result.m03 = lhs.m00 * rhs.m03 + lhs.m01 * rhs.m13 + lhs.m02 * rhs.m23 + lhs.m03 * rhs.m33;
            result.m10 = lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20 + lhs.m13 * rhs.m30;
            result.m11 = lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21 + lhs.m13 * rhs.m31;
            result.m12 = lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22 + lhs.m13 * rhs.m32;
            result.m13 = lhs.m10 * rhs.m03 + lhs.m11 * rhs.m13 + lhs.m12 * rhs.m23 + lhs.m13 * rhs.m33;
            result.m20 = lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20 + lhs.m23 * rhs.m30;
            result.m21 = lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21 + lhs.m23 * rhs.m31;
            result.m22 = lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22 + lhs.m23 * rhs.m32;
            result.m23 = lhs.m20 * rhs.m03 + lhs.m21 * rhs.m13 + lhs.m22 * rhs.m23 + lhs.m23 * rhs.m33;
            result.m30 = lhs.m30 * rhs.m00 + lhs.m31 * rhs.m10 + lhs.m32 * rhs.m20 + lhs.m33 * rhs.m30;
            result.m31 = lhs.m30 * rhs.m01 + lhs.m31 * rhs.m11 + lhs.m32 * rhs.m21 + lhs.m33 * rhs.m31;
            result.m32 = lhs.m30 * rhs.m02 + lhs.m31 * rhs.m12 + lhs.m32 * rhs.m22 + lhs.m33 * rhs.m32;
            result.m33 = lhs.m30 * rhs.m03 + lhs.m31 * rhs.m13 + lhs.m32 * rhs.m23 + lhs.m33 * rhs.m33;
            return result;
        }

        // Token: 0x060048BB RID: 18619 RVA: 0x0007C780 File Offset: 0x0007A980
        public static Vector4 operator *(Matrix4x4 lhs, Vector4 vector)
        {
            Vector4 result;
            result.x = lhs.m00 * vector.x + lhs.m01 * vector.y + lhs.m02 * vector.z + lhs.m03 * vector.w;
            result.y = lhs.m10 * vector.x + lhs.m11 * vector.y + lhs.m12 * vector.z + lhs.m13 * vector.w;
            result.z = lhs.m20 * vector.x + lhs.m21 * vector.y + lhs.m22 * vector.z + lhs.m23 * vector.w;
            result.w = lhs.m30 * vector.x + lhs.m31 * vector.y + lhs.m32 * vector.z + lhs.m33 * vector.w;
            return result;
        }

        // Token: 0x060048BC RID: 18620 RVA: 0x0007C8B0 File Offset: 0x0007AAB0
        public static bool operator ==(Matrix4x4 lhs, Matrix4x4 rhs)
        {
            return lhs.GetColumn(0) == rhs.GetColumn(0) && lhs.GetColumn(1) == rhs.GetColumn(1) && lhs.GetColumn(2) == rhs.GetColumn(2) && lhs.GetColumn(3) == rhs.GetColumn(3);
        }

        // Token: 0x060048BD RID: 18621 RVA: 0x0007C92C File Offset: 0x0007AB2C
        public static bool operator !=(Matrix4x4 lhs, Matrix4x4 rhs)
        {
            return !(lhs == rhs);
        }

        // Token: 0x060048BE RID: 18622 RVA: 0x0007C94C File Offset: 0x0007AB4C
        public Vector4 GetColumn(int index)
        {
            Vector4 result;
            switch (index)
            {
                case 0:
                    result = new Vector4(m00, m10, m20, m30);
                    break;
                case 1:
                    result = new Vector4(m01, m11, m21, m31);
                    break;
                case 2:
                    result = new Vector4(m02, m12, m22, m32);
                    break;
                case 3:
                    result = new Vector4(m03, m13, m23, m33);
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid column index!");
            }
            return result;
        }

        // Token: 0x060048BF RID: 18623 RVA: 0x0007CA10 File Offset: 0x0007AC10
        public Vector4 GetRow(int index)
        {
            Vector4 result;
            switch (index)
            {
                case 0:
                    result = new Vector4(m00, m01, m02, m03);
                    break;
                case 1:
                    result = new Vector4(m10, m11, m12, m13);
                    break;
                case 2:
                    result = new Vector4(m20, m21, m22, m23);
                    break;
                case 3:
                    result = new Vector4(m30, m31, m32, m33);
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid row index!");
            }
            return result;
        }

        // Token: 0x060048C0 RID: 18624 RVA: 0x0007CAD1 File Offset: 0x0007ACD1
        public void SetColumn(int index, Vector4 column)
        {
            this[0, index] = column.x;
            this[1, index] = column.y;
            this[2, index] = column.z;
            this[3, index] = column.w;
        }

        // Token: 0x060048C1 RID: 18625 RVA: 0x0007CB10 File Offset: 0x0007AD10
        public void SetRow(int index, Vector4 row)
        {
            this[index, 0] = row.x;
            this[index, 1] = row.y;
            this[index, 2] = row.z;
            this[index, 3] = row.w;
        }

        // Token: 0x060048C2 RID: 18626 RVA: 0x0007CB50 File Offset: 0x0007AD50
        public Vector3 MultiplyPoint(Vector3 point)
        {
            Vector3 result;
            result.x = m00 * point.x + m01 * point.y + m02 * point.z + m03;
            result.y = m10 * point.x + m11 * point.y + m12 * point.z + m13;
            result.z = m20 * point.x + m21 * point.y + m22 * point.z + m23;
            float num = m30 * point.x + m31 * point.y + m32 * point.z + m33;
            num = 1f / num;
            result.x *= num;
            result.y *= num;
            result.z *= num;
            return result;
        }

        // Token: 0x060048C3 RID: 18627 RVA: 0x0007CC80 File Offset: 0x0007AE80
        public Vector3 MultiplyPoint3x4(Vector3 point)
        {
            Vector3 result;
            result.x = m00 * point.x + m01 * point.y + m02 * point.z + m03;
            result.y = m10 * point.x + m11 * point.y + m12 * point.z + m13;
            result.z = m20 * point.x + m21 * point.y + m22 * point.z + m23;
            return result;
        }

        // Token: 0x060048C4 RID: 18628 RVA: 0x0007CD44 File Offset: 0x0007AF44
        public Vector3 MultiplyVector(Vector3 vector)
        {
            Vector3 result;
            result.x = m00 * vector.x + m01 * vector.y + m02 * vector.z;
            result.y = m10 * vector.x + m11 * vector.y + m12 * vector.z;
            result.z = m20 * vector.x + m21 * vector.y + m22 * vector.z;
            return result;
        }

        // Token: 0x060048C6 RID: 18630 RVA: 0x0007CF0C File Offset: 0x0007B10C
        public static Matrix4x4 Scale(Vector3 vector)
        {
            Matrix4x4 result;
            result.m00 = vector.x;
            result.m01 = 0f;
            result.m02 = 0f;
            result.m03 = 0f;
            result.m10 = 0f;
            result.m11 = vector.y;
            result.m12 = 0f;
            result.m13 = 0f;
            result.m20 = 0f;
            result.m21 = 0f;
            result.m22 = vector.z;
            result.m23 = 0f;
            result.m30 = 0f;
            result.m31 = 0f;
            result.m32 = 0f;
            result.m33 = 1f;
            return result;
        }

        // Token: 0x060048C7 RID: 18631 RVA: 0x0007CFE8 File Offset: 0x0007B1E8
        public static Matrix4x4 Translate(Vector3 vector)
        {
            Matrix4x4 result;
            result.m00 = 1f;
            result.m01 = 0f;
            result.m02 = 0f;
            result.m03 = vector.x;
            result.m10 = 0f;
            result.m11 = 1f;
            result.m12 = 0f;
            result.m13 = vector.y;
            result.m20 = 0f;
            result.m21 = 0f;
            result.m22 = 1f;
            result.m23 = vector.z;
            result.m30 = 0f;
            result.m31 = 0f;
            result.m32 = 0f;
            result.m33 = 1f;
            return result;
        }

        // Token: 0x060048C8 RID: 18632 RVA: 0x0007D0C4 File Offset: 0x0007B2C4
        public static Matrix4x4 Rotate(Quaternion q)
        {
            float num = q.x * 2f;
            float num2 = q.y * 2f;
            float num3 = q.z * 2f;
            float num4 = q.x * num;
            float num5 = q.y * num2;
            float num6 = q.z * num3;
            float num7 = q.x * num2;
            float num8 = q.x * num3;
            float num9 = q.y * num3;
            float num10 = q.w * num;
            float num11 = q.w * num2;
            float num12 = q.w * num3;
            Matrix4x4 result;
            result.m00 = 1f - (num5 + num6);
            result.m10 = num7 + num12;
            result.m20 = num8 - num11;
            result.m30 = 0f;
            result.m01 = num7 - num12;
            result.m11 = 1f - (num4 + num6);
            result.m21 = num9 + num10;
            result.m31 = 0f;
            result.m02 = num8 + num11;
            result.m12 = num9 - num10;
            result.m22 = 1f - (num4 + num5);
            result.m32 = 0f;
            result.m03 = 0f;
            result.m13 = 0f;
            result.m23 = 0f;
            result.m33 = 1f;
            return result;
        }

        // Token: 0x17001100 RID: 4352
        // (get) Token: 0x060048C9 RID: 18633 RVA: 0x0007D23C File Offset: 0x0007B43C
        public static Matrix4x4 zero
        {
            get
            {
                return Matrix4x4.zeroMatrix;
            }
        }

        // Token: 0x17001101 RID: 4353
        // (get) Token: 0x060048CA RID: 18634 RVA: 0x0007D258 File Offset: 0x0007B458
        public static Matrix4x4 identity
        {
            get
            {
                return Matrix4x4.identityMatrix;
            }
        }

        // Token: 0x060048CB RID: 18635 RVA: 0x0007D274 File Offset: 0x0007B474
        public override string ToString()
        {
            return string.Format("{0:F5}\t{1:F5}\t{2:F5}\t{3:F5}\n{4:F5}\t{5:F5}\t{6:F5}\t{7:F5}\n{8:F5}\t{9:F5}\t{10:F5}\t{11:F5}\n{12:F5}\t{13:F5}\t{14:F5}\t{15:F5}\n", new object[]
            {
                m00,
                m01,
                m02,
                m03,
                m10,
                m11,
                m12,
                m13,
                m20,
                m21,
                m22,
                m23,
                m30,
                m31,
                m32,
                m33
            });
        }

        // Token: 0x060048CC RID: 18636 RVA: 0x0007D384 File Offset: 0x0007B584
        public string ToString(string format)
        {
            return string.Format("{0}\t{1}\t{2}\t{3}\n{4}\t{5}\t{6}\t{7}\n{8}\t{9}\t{10}\t{11}\n{12}\t{13}\t{14}\t{15}\n", new object[]
            {
                m00.ToString(format),
                m01.ToString(format),
                m02.ToString(format),
                m03.ToString(format),
                m10.ToString(format),
                m11.ToString(format),
                m12.ToString(format),
                m13.ToString(format),
                m20.ToString(format),
                m21.ToString(format),
                m22.ToString(format),
                m23.ToString(format),
                m30.ToString(format),
                m31.ToString(format),
                m32.ToString(format),
                m33.ToString(format)
            });
        }

        public float m00;

        public float m10;

        public float m20;

        public float m30;

        public float m01;

        public float m11;

        public float m21;

        public float m31;

        public float m02;

        public float m12;

        public float m22;

        public float m32;

        public float m03;

        public float m13;

        public float m23;

        public float m33;

        // Token: 0x04001914 RID: 6420
        private static readonly Matrix4x4 zeroMatrix = new Matrix4x4(new Vector4(0f, 0f, 0f, 0f), new Vector4(0f, 0f, 0f, 0f), new Vector4(0f, 0f, 0f, 0f), new Vector4(0f, 0f, 0f, 0f));

        // Token: 0x04001915 RID: 6421
        private static readonly Matrix4x4 identityMatrix = new Matrix4x4(new Vector4(1f, 0f, 0f, 0f), new Vector4(0f, 1f, 0f, 0f), new Vector4(0f, 0f, 1f, 0f), new Vector4(0f, 0f, 0f, 1f));
        #endregion

        #region 网上代码

        public bool isIdentity
        {
            get
            {
                return m00 == 1f && m11 == 1f && m22 == 1f && m33 == 1f && // Check diagonal element first for early out.
                m12 == 0.0f && m13 == 0.0f && m13 == 0.0f && m21 == 0.0f && m23 == 0.0f && m23 == 0.0f && m31 == 0.0f && m32 == 0.0f && m33 == 0.0f;
            }
        }

        public Vector3 up
        {
            get
            {
                Vector3 vector3;
                vector3.x = m01;
                vector3.y = m11;
                vector3.z = m21;
                return vector3;
            }
            set
            {
                m01 = value.x;
                m11 = value.y;
                m21 = value.z;
            }
        }

        public Vector3 down
        {
            get
            {
                Vector3 vector3;
                vector3.x = -m01;
                vector3.y = -m11;
                vector3.z = -m21;
                return vector3;
            }
            set
            {
                m01 = -value.x;
                m11 = -value.y;
                m21 = -value.z;
            }
        }

        public Vector3 right
        {
            get
            {
                Vector3 vector3;
                vector3.x = m00;
                vector3.y = m10;
                vector3.z = m20;
                return vector3;
            }
            set
            {
                m00 = value.x;
                m10 = value.y;
                m20 = value.z;
            }
        }

        public Vector3 left
        {
            get
            {
                Vector3 vector3;
                vector3.x = -m00;
                vector3.y = -m10;
                vector3.z = -m20;
                return vector3;
            }
            set
            {
                m00 = -value.x;
                m10 = -value.y;
                m20 = -value.z;
            }
        }

        public Vector3 forward
        {
            get
            {
                Vector3 vector3;
                vector3.x = -m02;
                vector3.y = -m12;
                vector3.z = -m22;
                return vector3;
            }
            set
            {
                m02 = -value.x;
                m12 = -value.y;
                m22 = -value.z;
            }
        }

        public Vector3 back
        {
            get
            {
                Vector3 vector3;
                vector3.x = m02;
                vector3.y = m12;
                vector3.z = m22;
                return vector3;
            }
            set
            {
                m02 = value.x;
                m12 = value.y;
                m22 = value.z;
            }
        }

        public Matrix4x4(
            float m00, float m01, float m02, float m03, float m10, float m11, float m12, float m13, float m20, float m21, float m22, float m23,
            float m30, float m31, float m32, float m33)
        {
            this.m00 = m00;
            this.m01 = m01;
            this.m02 = m02;
            this.m03 = m03;
            this.m10 = m10;
            this.m11 = m11;
            this.m12 = m12;
            this.m13 = m13;
            this.m20 = m20;
            this.m21 = m21;
            this.m22 = m22;
            this.m23 = m23;
            this.m30 = m30;
            this.m31 = m31;
            this.m32 = m32;
            this.m33 = m33;
        }

        public static Matrix4x4 CreateTranslation(Vector3 position)
        {
            Matrix4x4 matrix44;
            matrix44.m00 = 1f;
            matrix44.m01 = 0.0f;
            matrix44.m02 = 0.0f;
            matrix44.m03 = position.x;
            matrix44.m10 = 0.0f;
            matrix44.m11 = 1f;
            matrix44.m12 = 0.0f;
            matrix44.m13 = position.y;
            matrix44.m20 = 0.0f;
            matrix44.m21 = 0.0f;
            matrix44.m22 = 1f;
            matrix44.m23 = position.z;
            matrix44.m30 = 0.0f;
            matrix44.m31 = 0.0f;
            matrix44.m32 = 0.0f;
            matrix44.m33 = 1f;
            return matrix44;
        }

        public Matrix4x4 inverse
        {
            get
            {
                return Matrix4x4.Invert(this);
            }
        }

        public static void CreateTranslation(ref Vector3 position, out Matrix4x4 matrix)
        {
            matrix.m00 = 1f;
            matrix.m01 = 0.0f;
            matrix.m02 = 0.0f;
            matrix.m03 = position.x;
            matrix.m10 = 0.0f;
            matrix.m11 = 1f;
            matrix.m12 = 0.0f;
            matrix.m13 = position.y;
            matrix.m20 = 0.0f;
            matrix.m21 = 0.0f;
            matrix.m22 = 1f;
            matrix.m23 = position.z;
            matrix.m30 = 0.0f;
            matrix.m31 = 0.0f;
            matrix.m32 = 0.0f;
            matrix.m33 = 1f;
        }

        public static Matrix4x4 CreateScale(Vector3 scales)
        {
            Matrix4x4 matrix44;
            matrix44.m00 = scales.x;
            matrix44.m01 = 0.0f;
            matrix44.m02 = 0.0f;
            matrix44.m03 = 0.0f;
            matrix44.m10 = 0.0f;
            matrix44.m11 = scales.y;
            matrix44.m12 = 0.0f;
            matrix44.m13 = 0.0f;
            matrix44.m20 = 0.0f;
            matrix44.m21 = 0.0f;
            matrix44.m22 = scales.z;
            matrix44.m23 = 0.0f;
            matrix44.m30 = 0.0f;
            matrix44.m31 = 0.0f;
            matrix44.m32 = 0.0f;
            matrix44.m33 = 1f;
            return matrix44;
        }

        public static Matrix4x4 TRS(Vector3 pos, Quaternion q, Vector3 s)
        {
            Matrix4x4 m1 = CreateTranslation(pos);
            Matrix4x4 m2 = CreateFromQuaternion(q);
            Matrix4x4 m3 = CreateScale(s);
            return m1 * m2 * m3;
        }

        public static void CreateScale(ref Vector3 scales, out Matrix4x4 matrix)
        {
            matrix.m00 = scales.x;
            matrix.m01 = 0.0f;
            matrix.m02 = 0.0f;
            matrix.m03 = 0.0f;
            matrix.m10 = 0.0f;
            matrix.m11 = scales.y;
            matrix.m12 = 0.0f;
            matrix.m13 = 0.0f;
            matrix.m20 = 0.0f;
            matrix.m21 = 0.0f;
            matrix.m22 = scales.z;
            matrix.m23 = 0.0f;
            matrix.m30 = 0.0f;
            matrix.m31 = 0.0f;
            matrix.m32 = 0.0f;
            matrix.m33 = 1f;
        }

        public static Matrix4x4 CreateOrthographic(float width, float height, float zNearPlane, float zFarPlane)
        {
            Matrix4x4 matrix44;
            matrix44.m00 = 2f / width;
            matrix44.m10 = matrix44.m20 = matrix44.m30 = 0.0f;
            matrix44.m11 = 2f / height;
            matrix44.m01 = matrix44.m21 = matrix44.m31 = 0.0f;
            matrix44.m22 = (float)(1.0 / (zNearPlane - (double)zFarPlane));
            matrix44.m02 = matrix44.m12 = matrix44.m32 = 0.0f;
            matrix44.m03 = matrix44.m13 = 0.0f;
            matrix44.m23 = zNearPlane / (zNearPlane - zFarPlane);
            matrix44.m33 = 1f;
            return matrix44;
        }

        public static void CreateOrthographic(float width, float height, float zNearPlane, float zFarPlane, out Matrix4x4 matrix)
        {
            matrix.m00 = 2f / width;
            matrix.m10 = matrix.m20 = matrix.m30 = 0.0f;
            matrix.m11 = 2f / height;
            matrix.m01 = matrix.m21 = matrix.m31 = 0.0f;
            matrix.m22 = (float)(1.0 / (zNearPlane - (double)zFarPlane));
            matrix.m02 = matrix.m12 = matrix.m32 = 0.0f;
            matrix.m03 = matrix.m13 = 0.0f;
            matrix.m23 = zNearPlane / (zNearPlane - zFarPlane);
            matrix.m33 = 1f;
        }

        public static Matrix4x4 CreateLookAt(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUpVector)
        {
            Vector3 vector3_1 = Vector3.Normalize(cameraPosition - cameraTarget);
            Vector3 vector3_2 = Vector3.Normalize(Vector3.Cross(cameraUpVector, vector3_1));
            Vector3 vector1 = Vector3.Cross(vector3_1, vector3_2);
            Matrix4x4 matrix44;
            matrix44.m00 = vector3_2.x;
            matrix44.m10 = vector1.x;
            matrix44.m20 = vector3_1.x;
            matrix44.m30 = 0.0f;
            matrix44.m01 = vector3_2.y;
            matrix44.m11 = vector1.y;
            matrix44.m21 = vector3_1.y;
            matrix44.m31 = 0.0f;
            matrix44.m02 = vector3_2.z;
            matrix44.m12 = vector1.z;
            matrix44.m22 = vector3_1.z;
            matrix44.m32 = 0.0f;
            matrix44.m03 = -Vector3.Dot(vector3_2, cameraPosition);
            matrix44.m13 = -Vector3.Dot(vector1, cameraPosition);
            matrix44.m23 = -Vector3.Dot(vector3_1, cameraPosition);
            matrix44.m33 = 1f;
            return matrix44;
        }

        public static void CreateLookAt(ref Vector3 cameraPosition, ref Vector3 cameraTarget, ref Vector3 cameraUpVector, out Matrix4x4 matrix)
        {
            Vector3 vector3_1 = Vector3.Normalize(cameraPosition - cameraTarget);
            Vector3 vector3_2 = Vector3.Normalize(Vector3.Cross(cameraUpVector, vector3_1));
            Vector3 vector1 = Vector3.Cross(vector3_1, vector3_2);
            matrix.m00 = vector3_2.x;
            matrix.m10 = vector1.x;
            matrix.m20 = vector3_1.x;
            matrix.m30 = 0.0f;
            matrix.m01 = vector3_2.y;
            matrix.m11 = vector1.y;
            matrix.m21 = vector3_1.y;
            matrix.m31 = 0.0f;
            matrix.m02 = vector3_2.z;
            matrix.m12 = vector1.z;
            matrix.m22 = vector3_1.z;
            matrix.m32 = 0.0f;
            matrix.m03 = -Vector3.Dot(vector3_2, cameraPosition);
            matrix.m13 = -Vector3.Dot(vector1, cameraPosition);
            matrix.m23 = -Vector3.Dot(vector3_1, cameraPosition);
            matrix.m33 = 1f;
        }

        public static Matrix4x4 CreateFromQuaternion(Quaternion quaternion)
        {
            float num1 = quaternion.x * quaternion.x;
            float num2 = quaternion.y * quaternion.y;
            float num3 = quaternion.z * quaternion.z;
            float num4 = quaternion.x * quaternion.y;
            float num5 = quaternion.z * quaternion.w;
            float num6 = quaternion.z * quaternion.x;
            float num7 = quaternion.y * quaternion.w;
            float num8 = quaternion.y * quaternion.z;
            float num9 = quaternion.x * quaternion.w;
            Matrix4x4 matrix44;
            matrix44.m00 = (float)(1.0 - 2.0 * (num2 + (double)num3));
            matrix44.m10 = (float)(2.0 * (num4 + (double)num5));
            matrix44.m20 = (float)(2.0 * (num6 - (double)num7));
            matrix44.m30 = 0.0f;
            matrix44.m01 = (float)(2.0 * (num4 - (double)num5));
            matrix44.m11 = (float)(1.0 - 2.0 * (num3 + (double)num1));
            matrix44.m21 = (float)(2.0 * (num8 + (double)num9));
            matrix44.m31 = 0.0f;
            matrix44.m02 = (float)(2.0 * (num6 + (double)num7));
            matrix44.m12 = (float)(2.0 * (num8 - (double)num9));
            matrix44.m22 = (float)(1.0 - 2.0 * (num2 + (double)num1));
            matrix44.m32 = 0.0f;
            matrix44.m03 = 0.0f;
            matrix44.m13 = 0.0f;
            matrix44.m23 = 0.0f;
            matrix44.m33 = 1f;
            return matrix44;
        }

        public static void CreateFromQuaternion(ref Quaternion quaternion, out Matrix4x4 matrix)
        {
            float num1 = quaternion.x * quaternion.x;
            float num2 = quaternion.y * quaternion.y;
            float num3 = quaternion.z * quaternion.z;
            float num4 = quaternion.x * quaternion.y;
            float num5 = quaternion.z * quaternion.w;
            float num6 = quaternion.z * quaternion.x;
            float num7 = quaternion.y * quaternion.w;
            float num8 = quaternion.y * quaternion.z;
            float num9 = quaternion.x * quaternion.w;
            matrix.m00 = (float)(1.0 - 2.0 * (num2 + (double)num3));
            matrix.m10 = (float)(2.0 * (num4 + (double)num5));
            matrix.m20 = (float)(2.0 * (num6 - (double)num7));
            matrix.m30 = 0.0f;
            matrix.m01 = (float)(2.0 * (num4 - (double)num5));
            matrix.m11 = (float)(1.0 - 2.0 * (num3 + (double)num1));
            matrix.m21 = (float)(2.0 * (num8 + (double)num9));
            matrix.m31 = 0.0f;
            matrix.m02 = (float)(2.0 * (num6 + (double)num7));
            matrix.m12 = (float)(2.0 * (num8 - (double)num9));
            matrix.m22 = (float)(1.0 - 2.0 * (num2 + (double)num1));
            matrix.m32 = 0.0f;
            matrix.m03 = 0.0f;
            matrix.m13 = 0.0f;
            matrix.m23 = 0.0f;
            matrix.m33 = 1f;
        }

        public static Matrix4x4 CreateFromYawPitchRoll(float yaw, float pitch, float roll)
        {
            Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll, out Quaternion result);
            return Matrix4x4.CreateFromQuaternion(result);
        }

        public static void CreateFromYawPitchRoll(float yaw, float pitch, float roll, out Matrix4x4 result)
        {
            Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll, out Quaternion result1);
            result = Matrix4x4.CreateFromQuaternion(result1);
        }

        public static Matrix4x4 CreateRotationX(float radians)
        {
            float num1 = (float)Math.Cos(radians);
            float num2 = (float)Math.Sin(radians);
            Matrix4x4 matrix44;
            matrix44.m00 = 1f;
            matrix44.m10 = 0.0f;
            matrix44.m20 = 0.0f;
            matrix44.m30 = 0.0f;
            matrix44.m01 = 0.0f;
            matrix44.m11 = num1;
            matrix44.m21 = num2;
            matrix44.m31 = 0.0f;
            matrix44.m02 = 0.0f;
            matrix44.m12 = -num2;
            matrix44.m22 = num1;
            matrix44.m32 = 0.0f;
            matrix44.m03 = 0.0f;
            matrix44.m13 = 0.0f;
            matrix44.m23 = 0.0f;
            matrix44.m33 = 1f;
            return matrix44;
        }

        public static void CreateRotationX(float radians, out Matrix4x4 result)
        {
            float num1 = (float)Math.Cos(radians);
            float num2 = (float)Math.Sin(radians);
            result.m00 = 1f;
            result.m10 = 0.0f;
            result.m20 = 0.0f;
            result.m30 = 0.0f;
            result.m01 = 0.0f;
            result.m11 = num1;
            result.m21 = num2;
            result.m31 = 0.0f;
            result.m02 = 0.0f;
            result.m12 = -num2;
            result.m22 = num1;
            result.m32 = 0.0f;
            result.m03 = 0.0f;
            result.m13 = 0.0f;
            result.m23 = 0.0f;
            result.m33 = 1f;
        }

        public static Matrix4x4 CreateRotationY(float radians)
        {
            float num1 = (float)Math.Cos(radians);
            float num2 = (float)Math.Sin(radians);
            Matrix4x4 matrix44;
            matrix44.m00 = num1;
            matrix44.m10 = 0.0f;
            matrix44.m20 = -num2;
            matrix44.m30 = 0.0f;
            matrix44.m01 = 0.0f;
            matrix44.m11 = 1f;
            matrix44.m21 = 0.0f;
            matrix44.m31 = 0.0f;
            matrix44.m02 = num2;
            matrix44.m12 = 0.0f;
            matrix44.m22 = num1;
            matrix44.m32 = 0.0f;
            matrix44.m03 = 0.0f;
            matrix44.m13 = 0.0f;
            matrix44.m23 = 0.0f;
            matrix44.m33 = 1f;
            return matrix44;
        }

        public static void CreateRotationY(float radians, out Matrix4x4 result)
        {
            float num1 = (float)Math.Cos(radians);
            float num2 = (float)Math.Sin(radians);
            result.m00 = num1;
            result.m10 = 0.0f;
            result.m20 = -num2;
            result.m30 = 0.0f;
            result.m01 = 0.0f;
            result.m11 = 1f;
            result.m21 = 0.0f;
            result.m31 = 0.0f;
            result.m02 = num2;
            result.m12 = 0.0f;
            result.m22 = num1;
            result.m32 = 0.0f;
            result.m03 = 0.0f;
            result.m13 = 0.0f;
            result.m23 = 0.0f;
            result.m33 = 1f;
        }

        public static Matrix4x4 CreateRotationZ(float radians)
        {
            float num1 = (float)Math.Cos(radians);
            float num2 = (float)Math.Sin(radians);
            Matrix4x4 matrix44;
            matrix44.m00 = num1;
            matrix44.m10 = num2;
            matrix44.m20 = 0.0f;
            matrix44.m30 = 0.0f;
            matrix44.m01 = -num2;
            matrix44.m11 = num1;
            matrix44.m21 = 0.0f;
            matrix44.m31 = 0.0f;
            matrix44.m02 = 0.0f;
            matrix44.m12 = 0.0f;
            matrix44.m22 = 1f;
            matrix44.m32 = 0.0f;
            matrix44.m03 = 0.0f;
            matrix44.m13 = 0.0f;
            matrix44.m23 = 0.0f;
            matrix44.m33 = 1f;
            return matrix44;
        }

        public static void CreateRotationZ(float radians, out Matrix4x4 result)
        {
            float num1 = (float)Math.Cos(radians);
            float num2 = (float)Math.Sin(radians);
            result.m00 = num1;
            result.m10 = num2;
            result.m20 = 0.0f;
            result.m30 = 0.0f;
            result.m01 = -num2;
            result.m11 = num1;
            result.m21 = 0.0f;
            result.m31 = 0.0f;
            result.m02 = 0.0f;
            result.m12 = 0.0f;
            result.m22 = 1f;
            result.m32 = 0.0f;
            result.m03 = 0.0f;
            result.m13 = 0.0f;
            result.m23 = 0.0f;
            result.m33 = 1f;
        }

        public static Matrix4x4 CreateFromAxisAngle(Vector3 axis, float angle)
        {
            float x = axis.x;
            float y = axis.y;
            float z = axis.z;
            float num1 = (float)Math.Sin(angle);
            float num2 = (float)Math.Cos(angle);
            float num3 = x * x;
            float num4 = y * y;
            float num5 = z * z;
            float num6 = x * y;
            float num7 = x * z;
            float num8 = y * z;
            Matrix4x4 matrix44;
            matrix44.m00 = num3 + num2 * (1f - num3);
            matrix44.m10 = (float)(num6 - num2 * (double)num6 + num1 * (double)z);
            matrix44.m20 = (float)(num7 - num2 * (double)num7 - num1 * (double)y);
            matrix44.m30 = 0.0f;
            matrix44.m01 = (float)(num6 - num2 * (double)num6 - num1 * (double)z);
            matrix44.m11 = num4 + num2 * (1f - num4);
            matrix44.m21 = (float)(num8 - num2 * (double)num8 + num1 * (double)x);
            matrix44.m31 = 0.0f;
            matrix44.m02 = (float)(num7 - num2 * (double)num7 + num1 * (double)y);
            matrix44.m12 = (float)(num8 - num2 * (double)num8 - num1 * (double)x);
            matrix44.m22 = num5 + num2 * (1f - num5);
            matrix44.m32 = 0.0f;
            matrix44.m03 = 0.0f;
            matrix44.m13 = 0.0f;
            matrix44.m23 = 0.0f;
            matrix44.m33 = 1f;
            return matrix44;
        }

        public static void CreateFromAxisAngle(ref Vector3 axis, float angle, out Matrix4x4 result)
        {
            float x = axis.x;
            float y = axis.y;
            float z = axis.z;
            float num1 = (float)Math.Sin(angle);
            float num2 = (float)Math.Cos(angle);
            float num3 = x * x;
            float num4 = y * y;
            float num5 = z * z;
            float num6 = x * y;
            float num7 = x * z;
            float num8 = y * z;
            result.m00 = num3 + num2 * (1f - num3);
            result.m10 = (float)(num6 - num2 * (double)num6 + num1 * (double)z);
            result.m20 = (float)(num7 - num2 * (double)num7 - num1 * (double)y);
            result.m30 = 0.0f;
            result.m01 = (float)(num6 - num2 * (double)num6 - num1 * (double)z);
            result.m11 = num4 + num2 * (1f - num4);
            result.m21 = (float)(num8 - num2 * (double)num8 + num1 * (double)x);
            result.m31 = 0.0f;
            result.m02 = (float)(num7 - num2 * (double)num7 + num1 * (double)y);
            result.m12 = (float)(num8 - num2 * (double)num8 - num1 * (double)x);
            result.m22 = num5 + num2 * (1f - num5);
            result.m32 = 0.0f;
            result.m03 = 0.0f;
            result.m13 = 0.0f;
            result.m23 = 0.0f;
            result.m33 = 1f;
        }

        public void Decompose(out Vector3 scale, out Quaternion rotation, out Vector3 translation)
        {
            Matrix4x4 identity = Matrix4x4.identity;
            float num1 = 1f / (float)Math.Sqrt(this[0, 0] * (double)this[0, 0] + this[1, 0] * (double)this[1, 0] +
                                                this[2, 0] * (double)this[2, 0]);
            identity[0, 0] = this[0, 0] * num1;
            identity[1, 0] = this[1, 0] * num1;
            identity[2, 0] = this[2, 0] * num1;
            float num2 = (float)(identity[0, 0] * (double)this[0, 1] + identity[1, 0] * (double)this[1, 1] +
                identity[2, 0] * (double)this[2, 1]);
            identity[0, 1] = this[0, 1] - num2 * identity[0, 0];
            identity[1, 1] = this[1, 1] - num2 * identity[1, 0];
            identity[2, 1] = this[2, 1] - num2 * identity[2, 0];
            float num3 = 1f / (float)Math.Sqrt(identity[0, 1] * (double)identity[0, 1] +
                                                identity[1, 1] * (double)identity[1, 1] +
                                                identity[2, 1] * (double)identity[2, 1]);
            identity[0, 1] *= num3;
            identity[1, 1] *= num3;
            identity[2, 1] *= num3;
            float num4 = (float)(identity[0, 0] * (double)this[0, 2] + identity[1, 0] * (double)this[1, 2] +
                identity[2, 0] * (double)this[2, 2]);
            identity[0, 2] = this[0, 2] - num4 * identity[0, 0];
            identity[1, 2] = this[1, 2] - num4 * identity[1, 0];
            identity[2, 2] = this[2, 2] - num4 * identity[2, 0];
            float num5 = (float)(identity[0, 1] * (double)this[0, 2] + identity[1, 1] * (double)this[1, 2] +
                identity[2, 1] * (double)this[2, 2]);
            identity[0, 2] -= num5 * identity[0, 1];
            identity[1, 2] -= num5 * identity[1, 1];
            identity[2, 2] -= num5 * identity[2, 1];
            float num6 = 1f / (float)Math.Sqrt(identity[0, 2] * (double)identity[0, 2] +
                                                identity[1, 2] * (double)identity[1, 2] +
                                                identity[2, 2] * (double)identity[2, 2]);
            identity[0, 2] *= num6;
            identity[1, 2] *= num6;
            identity[2, 2] *= num6;
            if (identity[0, 0] * (double)identity[1, 1] * identity[2, 2] +
                identity[0, 1] * (double)identity[1, 2] * identity[2, 0] +
                identity[0, 2] * (double)identity[1, 0] * identity[2, 1] -
                identity[0, 2] * (double)identity[1, 1] * identity[2, 0] -
                identity[0, 1] * (double)identity[1, 0] * identity[2, 2] -
                identity[0, 0] * (double)identity[1, 2] * identity[2, 1] < 0.0)
            {
                for (int index1 = 0; index1 < 3; ++index1)
                {
                    for (int index2 = 0; index2 < 3; ++index2)
                        identity[index1, index2] = -identity[index1, index2];
                }
            }

            scale =
                    new
                            Vector3((float)(identity[0, 0] * (double)this[0, 0] + identity[1, 0] * (double)this[1, 0] + identity[2, 0] * (double)this[2, 0]),
                                    (float)(identity[0, 1] * (double)this[0, 1] + identity[1, 1] * (double)this[1, 1] +
                                        identity[2, 1] * (double)this[2, 1]),
                                    (float)(identity[0, 2] * (double)this[0, 2] + identity[1, 2] * (double)this[1, 2] +
                                        identity[2, 2] * (double)this[2, 2]));
            rotation = Quaternion.CreateFromRotationMatrix(identity);
            translation = new Vector3(this[0, 3], this[1, 3], this[2, 3]);
        }

        public static Matrix4x4 Transpose(Matrix4x4 matrix)
        {
            Matrix4x4 matrix44;
            matrix44.m00 = matrix.m00;
            matrix44.m01 = matrix.m10;
            matrix44.m02 = matrix.m20;
            matrix44.m03 = matrix.m30;
            matrix44.m10 = matrix.m01;
            matrix44.m11 = matrix.m11;
            matrix44.m12 = matrix.m21;
            matrix44.m13 = matrix.m31;
            matrix44.m20 = matrix.m02;
            matrix44.m21 = matrix.m12;
            matrix44.m22 = matrix.m22;
            matrix44.m23 = matrix.m32;
            matrix44.m30 = matrix.m03;
            matrix44.m31 = matrix.m13;
            matrix44.m32 = matrix.m23;
            matrix44.m33 = matrix.m33;
            return matrix44;
        }

        public static void Transpose(ref Matrix4x4 matrix, out Matrix4x4 result)
        {
            result.m00 = matrix.m00;
            result.m01 = matrix.m10;
            result.m02 = matrix.m20;
            result.m03 = matrix.m30;
            result.m10 = matrix.m01;
            result.m11 = matrix.m11;
            result.m12 = matrix.m21;
            result.m13 = matrix.m31;
            result.m20 = matrix.m02;
            result.m21 = matrix.m12;
            result.m22 = matrix.m22;
            result.m23 = matrix.m32;
            result.m30 = matrix.m03;
            result.m31 = matrix.m13;
            result.m32 = matrix.m23;
            result.m33 = matrix.m33;
        }

        public float Determinant()
        {
            float m00 = this.m00;
            float m10 = this.m10;
            float m20 = this.m20;
            float m30 = this.m30;
            float m01 = this.m01;
            float m11 = this.m11;
            float m21 = this.m21;
            float m31 = this.m31;
            float m02 = this.m02;
            float m12 = this.m12;
            float m22 = this.m22;
            float m32 = this.m32;
            float m03 = this.m03;
            float m13 = this.m13;
            float m23 = this.m23;
            float m33 = this.m33;
            float num1 = (float)(m22 * (double)m33 - m32 * (double)m23);
            float num2 = (float)(m12 * (double)m33 - m32 * (double)m13);
            float num3 = (float)(m12 * (double)m23 - m22 * (double)m13);
            float num4 = (float)(m02 * (double)m33 - m32 * (double)m03);
            float num5 = (float)(m02 * (double)m23 - m22 * (double)m03);
            float num6 = (float)(m02 * (double)m13 - m12 * (double)m03);
            return (float)(m00 * (m11 * (double)num1 - m21 * (double)num2 + m31 * (double)num3) -
                m10 * (m01 * (double)num1 - m21 * (double)num4 + m31 * (double)num5) +
                m20 * (m01 * (double)num2 - m11 * (double)num4 + m31 * (double)num6) -
                m30 * (m01 * (double)num3 - m11 * (double)num5 + m21 * (double)num6));
        }

        public static Matrix4x4 Invert(Matrix4x4 matrix)
        {
            float m00 = matrix.m00;
            float m10 = matrix.m10;
            float m20 = matrix.m20;
            float m30 = matrix.m30;
            float m01 = matrix.m01;
            float m11 = matrix.m11;
            float m21 = matrix.m21;
            float m31 = matrix.m31;
            float m02 = matrix.m02;
            float m12 = matrix.m12;
            float m22 = matrix.m22;
            float m32 = matrix.m32;
            float m03 = matrix.m03;
            float m13 = matrix.m13;
            float m23 = matrix.m23;
            float m33 = matrix.m33;
            float num1 = (float)(m22 * (double)m33 - m32 * (double)m23);
            float num2 = (float)(m12 * (double)m33 - m32 * (double)m13);
            float num3 = (float)(m12 * (double)m23 - m22 * (double)m13);
            float num4 = (float)(m02 * (double)m33 - m32 * (double)m03);
            float num5 = (float)(m02 * (double)m23 - m22 * (double)m03);
            float num6 = (float)(m02 * (double)m13 - m12 * (double)m03);
            float num7 = (float)(m11 * (double)num1 - m21 * (double)num2 + m31 * (double)num3);
            float num8 = (float)-(m01 * (double)num1 - m21 * (double)num4 + m31 * (double)num5);
            float num9 = (float)(m01 * (double)num2 - m11 * (double)num4 + m31 * (double)num6);
            float num10 = (float)-(m01 * (double)num3 - m11 * (double)num5 + m21 * (double)num6);
            float num11 = (float)(1.0 / (m00 * (double)num7 + m10 * (double)num8 + m20 * (double)num9 +
                m30 * (double)num10));
            Matrix4x4 matrix44;
            matrix44.m00 = num7 * num11;
            matrix44.m01 = num8 * num11;
            matrix44.m02 = num9 * num11;
            matrix44.m03 = num10 * num11;
            matrix44.m10 = (float)-(m10 * (double)num1 - m20 * (double)num2 + m30 * (double)num3) * num11;
            matrix44.m11 = (float)(m00 * (double)num1 - m20 * (double)num4 + m30 * (double)num5) * num11;
            matrix44.m12 = (float)-(m00 * (double)num2 - m10 * (double)num4 + m30 * (double)num6) * num11;
            matrix44.m13 = (float)(m00 * (double)num3 - m10 * (double)num5 + m20 * (double)num6) * num11;
            float num12 = (float)(m21 * (double)m33 - m31 * (double)m23);
            float num13 = (float)(m11 * (double)m33 - m31 * (double)m13);
            float num14 = (float)(m11 * (double)m23 - m21 * (double)m13);
            float num15 = (float)(m01 * (double)m33 - m31 * (double)m03);
            float num16 = (float)(m01 * (double)m23 - m21 * (double)m03);
            float num17 = (float)(m01 * (double)m13 - m11 * (double)m03);
            matrix44.m20 = (float)(m10 * (double)num12 - m20 * (double)num13 + m30 * (double)num14) * num11;
            matrix44.m21 = (float)-(m00 * (double)num12 - m20 * (double)num15 + m30 * (double)num16) * num11;
            matrix44.m22 = (float)(m00 * (double)num13 - m10 * (double)num15 + m30 * (double)num17) * num11;
            matrix44.m23 = (float)-(m00 * (double)num14 - m10 * (double)num16 + m20 * (double)num17) * num11;
            float num18 = (float)(m21 * (double)m32 - m31 * (double)m22);
            float num19 = (float)(m11 * (double)m32 - m31 * (double)m12);
            float num20 = (float)(m11 * (double)m22 - m21 * (double)m12);
            float num21 = (float)(m01 * (double)m32 - m31 * (double)m02);
            float num22 = (float)(m01 * (double)m22 - m21 * (double)m02);
            float num23 = (float)(m01 * (double)m12 - m11 * (double)m02);
            matrix44.m30 = (float)-(m10 * (double)num18 - m20 * (double)num19 + m30 * (double)num20) * num11;
            matrix44.m31 = (float)(m00 * (double)num18 - m20 * (double)num21 + m30 * (double)num22) * num11;
            matrix44.m32 = (float)-(m00 * (double)num19 - m10 * (double)num21 + m30 * (double)num23) * num11;
            matrix44.m33 = (float)(m00 * (double)num20 - m10 * (double)num22 + m20 * (double)num23) * num11;
            return matrix44;
        }

        public static void Invert(ref Matrix4x4 matrix, out Matrix4x4 result)
        {
            float m00 = matrix.m00;
            float m10 = matrix.m10;
            float m20 = matrix.m20;
            float m30 = matrix.m30;
            float m01 = matrix.m01;
            float m11 = matrix.m11;
            float m21 = matrix.m21;
            float m31 = matrix.m31;
            float m02 = matrix.m02;
            float m12 = matrix.m12;
            float m22 = matrix.m22;
            float m32 = matrix.m32;
            float m03 = matrix.m03;
            float m13 = matrix.m13;
            float m23 = matrix.m23;
            float m33 = matrix.m33;
            float num1 = (float)(m22 * (double)m33 - m32 * (double)m23);
            float num2 = (float)(m12 * (double)m33 - m32 * (double)m13);
            float num3 = (float)(m12 * (double)m23 - m22 * (double)m13);
            float num4 = (float)(m02 * (double)m33 - m32 * (double)m03);
            float num5 = (float)(m02 * (double)m23 - m22 * (double)m03);
            float num6 = (float)(m02 * (double)m13 - m12 * (double)m03);
            float num7 = (float)(m11 * (double)num1 - m21 * (double)num2 + m31 * (double)num3);
            float num8 = (float)-(m01 * (double)num1 - m21 * (double)num4 + m31 * (double)num5);
            float num9 = (float)(m01 * (double)num2 - m11 * (double)num4 + m31 * (double)num6);
            float num10 = (float)-(m01 * (double)num3 - m11 * (double)num5 + m21 * (double)num6);
            float num11 = (float)(1.0 / (m00 * (double)num7 + m10 * (double)num8 + m20 * (double)num9 +
                m30 * (double)num10));
            result.m00 = num7 * num11;
            result.m01 = num8 * num11;
            result.m02 = num9 * num11;
            result.m03 = num10 * num11;
            result.m10 = (float)-(m10 * (double)num1 - m20 * (double)num2 + m30 * (double)num3) * num11;
            result.m11 = (float)(m00 * (double)num1 - m20 * (double)num4 + m30 * (double)num5) * num11;
            result.m12 = (float)-(m00 * (double)num2 - m10 * (double)num4 + m30 * (double)num6) * num11;
            result.m13 = (float)(m00 * (double)num3 - m10 * (double)num5 + m20 * (double)num6) * num11;
            float num12 = (float)(m21 * (double)m33 - m31 * (double)m23);
            float num13 = (float)(m11 * (double)m33 - m31 * (double)m13);
            float num14 = (float)(m11 * (double)m23 - m21 * (double)m13);
            float num15 = (float)(m01 * (double)m33 - m31 * (double)m03);
            float num16 = (float)(m01 * (double)m23 - m21 * (double)m03);
            float num17 = (float)(m01 * (double)m13 - m11 * (double)m03);
            result.m20 = (float)(m10 * (double)num12 - m20 * (double)num13 + m30 * (double)num14) * num11;
            result.m21 = (float)-(m00 * (double)num12 - m20 * (double)num15 + m30 * (double)num16) * num11;
            result.m22 = (float)(m00 * (double)num13 - m10 * (double)num15 + m30 * (double)num17) * num11;
            result.m23 = (float)-(m00 * (double)num14 - m10 * (double)num16 + m20 * (double)num17) * num11;
            float num18 = (float)(m21 * (double)m32 - m31 * (double)m22);
            float num19 = (float)(m11 * (double)m32 - m31 * (double)m12);
            float num20 = (float)(m11 * (double)m22 - m21 * (double)m12);
            float num21 = (float)(m01 * (double)m32 - m31 * (double)m02);
            float num22 = (float)(m01 * (double)m22 - m21 * (double)m02);
            float num23 = (float)(m01 * (double)m12 - m11 * (double)m02);
            result.m30 = (float)-(m10 * (double)num18 - m20 * (double)num19 + m30 * (double)num20) * num11;
            result.m31 = (float)(m00 * (double)num18 - m20 * (double)num21 + m30 * (double)num22) * num11;
            result.m32 = (float)-(m00 * (double)num19 - m10 * (double)num21 + m30 * (double)num23) * num11;
            result.m33 = (float)(m00 * (double)num20 - m10 * (double)num22 + m20 * (double)num23) * num11;
        }

        public static Matrix4x4 Add(Matrix4x4 matrix1, Matrix4x4 matrix2)
        {
            Matrix4x4 matrix44;
            matrix44.m00 = matrix1.m00 + matrix2.m00;
            matrix44.m01 = matrix1.m01 + matrix2.m01;
            matrix44.m02 = matrix1.m02 + matrix2.m02;
            matrix44.m03 = matrix1.m03 + matrix2.m03;
            matrix44.m10 = matrix1.m10 + matrix2.m10;
            matrix44.m11 = matrix1.m11 + matrix2.m11;
            matrix44.m12 = matrix1.m12 + matrix2.m12;
            matrix44.m13 = matrix1.m13 + matrix2.m13;
            matrix44.m20 = matrix1.m20 + matrix2.m20;
            matrix44.m21 = matrix1.m21 + matrix2.m21;
            matrix44.m22 = matrix1.m22 + matrix2.m22;
            matrix44.m23 = matrix1.m23 + matrix2.m23;
            matrix44.m30 = matrix1.m30 + matrix2.m30;
            matrix44.m31 = matrix1.m31 + matrix2.m31;
            matrix44.m32 = matrix1.m32 + matrix2.m32;
            matrix44.m33 = matrix1.m33 + matrix2.m33;
            return matrix44;
        }

        public static void Add(ref Matrix4x4 matrix1, ref Matrix4x4 matrix2, out Matrix4x4 result)
        {
            result.m00 = matrix1.m00 + matrix2.m00;
            result.m01 = matrix1.m01 + matrix2.m01;
            result.m02 = matrix1.m02 + matrix2.m02;
            result.m03 = matrix1.m03 + matrix2.m03;
            result.m10 = matrix1.m10 + matrix2.m10;
            result.m11 = matrix1.m11 + matrix2.m11;
            result.m12 = matrix1.m12 + matrix2.m12;
            result.m13 = matrix1.m13 + matrix2.m13;
            result.m20 = matrix1.m20 + matrix2.m20;
            result.m21 = matrix1.m21 + matrix2.m21;
            result.m22 = matrix1.m22 + matrix2.m22;
            result.m23 = matrix1.m23 + matrix2.m23;
            result.m30 = matrix1.m30 + matrix2.m30;
            result.m31 = matrix1.m31 + matrix2.m31;
            result.m32 = matrix1.m32 + matrix2.m32;
            result.m33 = matrix1.m33 + matrix2.m33;
        }

        public static Matrix4x4 Sub(Matrix4x4 matrix1, Matrix4x4 matrix2)
        {
            Matrix4x4 matrix44;
            matrix44.m00 = matrix1.m00 - matrix2.m00;
            matrix44.m01 = matrix1.m01 - matrix2.m01;
            matrix44.m02 = matrix1.m02 - matrix2.m02;
            matrix44.m03 = matrix1.m03 - matrix2.m03;
            matrix44.m10 = matrix1.m10 - matrix2.m10;
            matrix44.m11 = matrix1.m11 - matrix2.m11;
            matrix44.m12 = matrix1.m12 - matrix2.m12;
            matrix44.m13 = matrix1.m13 - matrix2.m13;
            matrix44.m20 = matrix1.m20 - matrix2.m20;
            matrix44.m21 = matrix1.m21 - matrix2.m21;
            matrix44.m22 = matrix1.m22 - matrix2.m22;
            matrix44.m23 = matrix1.m23 - matrix2.m23;
            matrix44.m30 = matrix1.m30 - matrix2.m30;
            matrix44.m31 = matrix1.m31 - matrix2.m31;
            matrix44.m32 = matrix1.m32 - matrix2.m32;
            matrix44.m33 = matrix1.m33 - matrix2.m33;
            return matrix44;
        }

        public static void Sub(ref Matrix4x4 matrix1, ref Matrix4x4 matrix2, out Matrix4x4 result)
        {
            result.m00 = matrix1.m00 - matrix2.m00;
            result.m01 = matrix1.m01 - matrix2.m01;
            result.m02 = matrix1.m02 - matrix2.m02;
            result.m03 = matrix1.m03 - matrix2.m03;
            result.m10 = matrix1.m10 - matrix2.m10;
            result.m11 = matrix1.m11 - matrix2.m11;
            result.m12 = matrix1.m12 - matrix2.m12;
            result.m13 = matrix1.m13 - matrix2.m13;
            result.m20 = matrix1.m20 - matrix2.m20;
            result.m21 = matrix1.m21 - matrix2.m21;
            result.m22 = matrix1.m22 - matrix2.m22;
            result.m23 = matrix1.m23 - matrix2.m23;
            result.m30 = matrix1.m30 - matrix2.m30;
            result.m31 = matrix1.m31 - matrix2.m31;
            result.m32 = matrix1.m32 - matrix2.m32;
            result.m33 = matrix1.m33 - matrix2.m33;
        }

        public static Matrix4x4 Multiply(Matrix4x4 matrix1, Matrix4x4 matrix2)
        {
            Matrix4x4 matrix44;
            matrix44.m00 = (float)(matrix1.m00 * (double)matrix2.m00 + matrix1.m01 * (double)matrix2.m10 +
                matrix1.m02 * (double)matrix2.m20 + matrix1.m03 * (double)matrix2.m30);
            matrix44.m01 = (float)(matrix1.m00 * (double)matrix2.m01 + matrix1.m01 * (double)matrix2.m11 +
                matrix1.m02 * (double)matrix2.m21 + matrix1.m03 * (double)matrix2.m31);
            matrix44.m02 = (float)(matrix1.m00 * (double)matrix2.m02 + matrix1.m01 * (double)matrix2.m12 +
                matrix1.m02 * (double)matrix2.m22 + matrix1.m03 * (double)matrix2.m32);
            matrix44.m03 = (float)(matrix1.m00 * (double)matrix2.m03 + matrix1.m01 * (double)matrix2.m13 +
                matrix1.m02 * (double)matrix2.m23 + matrix1.m03 * (double)matrix2.m33);
            matrix44.m10 = (float)(matrix1.m10 * (double)matrix2.m00 + matrix1.m11 * (double)matrix2.m10 +
                matrix1.m12 * (double)matrix2.m20 + matrix1.m13 * (double)matrix2.m30);
            matrix44.m11 = (float)(matrix1.m10 * (double)matrix2.m01 + matrix1.m11 * (double)matrix2.m11 +
                matrix1.m12 * (double)matrix2.m21 + matrix1.m13 * (double)matrix2.m31);
            matrix44.m12 = (float)(matrix1.m10 * (double)matrix2.m02 + matrix1.m11 * (double)matrix2.m12 +
                matrix1.m12 * (double)matrix2.m22 + matrix1.m13 * (double)matrix2.m32);
            matrix44.m13 = (float)(matrix1.m10 * (double)matrix2.m03 + matrix1.m11 * (double)matrix2.m13 +
                matrix1.m12 * (double)matrix2.m23 + matrix1.m13 * (double)matrix2.m33);
            matrix44.m20 = (float)(matrix1.m20 * (double)matrix2.m00 + matrix1.m21 * (double)matrix2.m10 +
                matrix1.m22 * (double)matrix2.m20 + matrix1.m23 * (double)matrix2.m30);
            matrix44.m21 = (float)(matrix1.m20 * (double)matrix2.m01 + matrix1.m21 * (double)matrix2.m11 +
                matrix1.m22 * (double)matrix2.m21 + matrix1.m23 * (double)matrix2.m31);
            matrix44.m22 = (float)(matrix1.m20 * (double)matrix2.m02 + matrix1.m21 * (double)matrix2.m12 +
                matrix1.m22 * (double)matrix2.m22 + matrix1.m23 * (double)matrix2.m32);
            matrix44.m23 = (float)(matrix1.m20 * (double)matrix2.m03 + matrix1.m21 * (double)matrix2.m13 +
                matrix1.m22 * (double)matrix2.m23 + matrix1.m23 * (double)matrix2.m33);
            matrix44.m30 = (float)(matrix1.m30 * (double)matrix2.m00 + matrix1.m31 * (double)matrix2.m10 +
                matrix1.m32 * (double)matrix2.m20 + matrix1.m33 * (double)matrix2.m30);
            matrix44.m31 = (float)(matrix1.m30 * (double)matrix2.m01 + matrix1.m31 * (double)matrix2.m11 +
                matrix1.m32 * (double)matrix2.m21 + matrix1.m33 * (double)matrix2.m31);
            matrix44.m32 = (float)(matrix1.m30 * (double)matrix2.m02 + matrix1.m31 * (double)matrix2.m12 +
                matrix1.m32 * (double)matrix2.m22 + matrix1.m33 * (double)matrix2.m32);
            matrix44.m33 = (float)(matrix1.m30 * (double)matrix2.m03 + matrix1.m31 * (double)matrix2.m13 +
                matrix1.m32 * (double)matrix2.m23 + matrix1.m33 * (double)matrix2.m33);
            return matrix44;
        }

        public static void Multiply(ref Matrix4x4 matrix1, ref Matrix4x4 matrix2, out Matrix4x4 result)
        {
            float num1 = (float)(matrix1.m00 * (double)matrix2.m00 + matrix1.m01 * (double)matrix2.m10 +
                matrix1.m02 * (double)matrix2.m20 + matrix1.m03 * (double)matrix2.m30);
            float num2 = (float)(matrix1.m00 * (double)matrix2.m01 + matrix1.m01 * (double)matrix2.m11 +
                matrix1.m02 * (double)matrix2.m21 + matrix1.m03 * (double)matrix2.m31);
            float num3 = (float)(matrix1.m00 * (double)matrix2.m02 + matrix1.m01 * (double)matrix2.m12 +
                matrix1.m02 * (double)matrix2.m22 + matrix1.m03 * (double)matrix2.m32);
            float num4 = (float)(matrix1.m00 * (double)matrix2.m03 + matrix1.m01 * (double)matrix2.m13 +
                matrix1.m02 * (double)matrix2.m23 + matrix1.m03 * (double)matrix2.m33);
            float num5 = (float)(matrix1.m10 * (double)matrix2.m00 + matrix1.m11 * (double)matrix2.m10 +
                matrix1.m12 * (double)matrix2.m20 + matrix1.m13 * (double)matrix2.m30);
            float num6 = (float)(matrix1.m10 * (double)matrix2.m01 + matrix1.m11 * (double)matrix2.m11 +
                matrix1.m12 * (double)matrix2.m21 + matrix1.m13 * (double)matrix2.m31);
            float num7 = (float)(matrix1.m10 * (double)matrix2.m02 + matrix1.m11 * (double)matrix2.m12 +
                matrix1.m12 * (double)matrix2.m22 + matrix1.m13 * (double)matrix2.m32);
            float num8 = (float)(matrix1.m10 * (double)matrix2.m03 + matrix1.m11 * (double)matrix2.m13 +
                matrix1.m12 * (double)matrix2.m23 + matrix1.m13 * (double)matrix2.m33);
            float num9 = (float)(matrix1.m20 * (double)matrix2.m00 + matrix1.m21 * (double)matrix2.m10 +
                matrix1.m22 * (double)matrix2.m20 + matrix1.m23 * (double)matrix2.m30);
            float num10 = (float)(matrix1.m20 * (double)matrix2.m01 + matrix1.m21 * (double)matrix2.m11 +
                matrix1.m22 * (double)matrix2.m21 + matrix1.m23 * (double)matrix2.m31);
            float num11 = (float)(matrix1.m20 * (double)matrix2.m02 + matrix1.m21 * (double)matrix2.m12 +
                matrix1.m22 * (double)matrix2.m22 + matrix1.m23 * (double)matrix2.m32);
            float num12 = (float)(matrix1.m20 * (double)matrix2.m03 + matrix1.m21 * (double)matrix2.m13 +
                matrix1.m22 * (double)matrix2.m23 + matrix1.m23 * (double)matrix2.m33);
            float num13 = (float)(matrix1.m30 * (double)matrix2.m00 + matrix1.m31 * (double)matrix2.m10 +
                matrix1.m32 * (double)matrix2.m20 + matrix1.m33 * (double)matrix2.m30);
            float num14 = (float)(matrix1.m30 * (double)matrix2.m01 + matrix1.m31 * (double)matrix2.m11 +
                matrix1.m32 * (double)matrix2.m21 + matrix1.m33 * (double)matrix2.m31);
            float num15 = (float)(matrix1.m30 * (double)matrix2.m02 + matrix1.m31 * (double)matrix2.m12 +
                matrix1.m32 * (double)matrix2.m22 + matrix1.m33 * (double)matrix2.m32);
            float num16 = (float)(matrix1.m30 * (double)matrix2.m03 + matrix1.m31 * (double)matrix2.m13 +
                matrix1.m32 * (double)matrix2.m23 + matrix1.m33 * (double)matrix2.m33);
            result.m00 = num1;
            result.m01 = num2;
            result.m02 = num3;
            result.m03 = num4;
            result.m10 = num5;
            result.m11 = num6;
            result.m12 = num7;
            result.m13 = num8;
            result.m20 = num9;
            result.m21 = num10;
            result.m22 = num11;
            result.m23 = num12;
            result.m30 = num13;
            result.m31 = num14;
            result.m32 = num15;
            result.m33 = num16;
        }

        public static Vector4 TransformVector4(Matrix4x4 matrix, Vector4 vector)
        {
            float num1 = (float)(vector.x * (double)matrix.m00 + vector.y * (double)matrix.m01 +
                vector.z * (double)matrix.m02 + vector.w * (double)matrix.m03);
            float num2 = (float)(vector.x * (double)matrix.m10 + vector.y * (double)matrix.m11 +
                vector.z * (double)matrix.m12 + vector.w * (double)matrix.m13);
            float num3 = (float)(vector.x * (double)matrix.m20 + vector.y * (double)matrix.m21 +
                vector.z * (double)matrix.m22 + vector.w * (double)matrix.m23);
            float num4 = (float)(vector.x * (double)matrix.m30 + vector.y * (double)matrix.m31 +
                vector.z * (double)matrix.m32 + vector.w * (double)matrix.m33);
            Vector4 vector4;
            vector4.x = num1;
            vector4.y = num2;
            vector4.z = num3;
            vector4.w = num4;
            return vector4;
        }

        public static void TransformVector4(ref Matrix4x4 matrix, ref Vector4 vector, out Vector4 result)
        {
            float num1 = (float)(vector.x * (double)matrix.m00 + vector.y * (double)matrix.m01 +
                vector.z * (double)matrix.m02 + vector.w * (double)matrix.m03);
            float num2 = (float)(vector.x * (double)matrix.m10 + vector.y * (double)matrix.m11 +
                vector.z * (double)matrix.m12 + vector.w * (double)matrix.m13);
            float num3 = (float)(vector.x * (double)matrix.m20 + vector.y * (double)matrix.m21 +
                vector.z * (double)matrix.m22 + vector.w * (double)matrix.m23);
            float num4 = (float)(vector.x * (double)matrix.m30 + vector.y * (double)matrix.m31 +
                vector.z * (double)matrix.m32 + vector.w * (double)matrix.m33);
            result.x = num1;
            result.y = num2;
            result.z = num3;
            result.w = num4;
        }

        public static Vector3 TransformPosition(Matrix4x4 matrix, Vector3 position)
        {
            float num1 = (float)(position.x * (double)matrix.m00 + position.y * (double)matrix.m01 +
                position.z * (double)matrix.m02) + matrix.m03;
            float num2 = (float)(position.x * (double)matrix.m10 + position.y * (double)matrix.m11 +
                position.z * (double)matrix.m12) + matrix.m13;
            float num3 = (float)(position.x * (double)matrix.m20 + position.y * (double)matrix.m21 +
                position.z * (double)matrix.m22) + matrix.m23;
            Vector3 vector3;
            vector3.x = num1;
            vector3.y = num2;
            vector3.z = num3;
            return vector3;
        }

        public static void TransformPosition(ref Matrix4x4 matrix, ref Vector3 position, out Vector3 result)
        {
            float num1 = (float)(position.x * (double)matrix.m00 + position.y * (double)matrix.m01 +
                position.z * (double)matrix.m02) + matrix.m03;
            float num2 = (float)(position.x * (double)matrix.m10 + position.y * (double)matrix.m11 +
                position.z * (double)matrix.m12) + matrix.m13;
            float num3 = (float)(position.x * (double)matrix.m20 + position.y * (double)matrix.m21 +
                position.z * (double)matrix.m22) + matrix.m23;
            result.x = num1;
            result.y = num2;
            result.z = num3;
        }

        public static Vector3 TransformDirection(Matrix4x4 matrix, Vector3 direction)
        {
            float num1 = (float)(direction.x * (double)matrix.m00 + direction.y * (double)matrix.m01 +
                direction.z * (double)matrix.m02);
            float num2 = (float)(direction.x * (double)matrix.m10 + direction.y * (double)matrix.m11 +
                direction.z * (double)matrix.m12);
            float num3 = (float)(direction.x * (double)matrix.m20 + direction.y * (double)matrix.m21 +
                direction.z * (double)matrix.m22);
            Vector3 vector3;
            vector3.x = num1;
            vector3.y = num2;
            vector3.z = num3;
            return vector3;
        }

        public static void TransformDirection(ref Matrix4x4 matrix, ref Vector3 direction, out Vector3 result)
        {
            float num1 = (float)(direction.x * (double)matrix.m00 + direction.y * (double)matrix.m01 +
                direction.z * (double)matrix.m02);
            float num2 = (float)(direction.x * (double)matrix.m10 + direction.y * (double)matrix.m11 +
                direction.z * (double)matrix.m12);
            float num3 = (float)(direction.x * (double)matrix.m20 + direction.y * (double)matrix.m21 +
                direction.z * (double)matrix.m22);
            result.x = num1;
            result.y = num2;
            result.z = num3;
        }

        public static Matrix4x4 operator -(Matrix4x4 matrix1)
        {
            Matrix4x4 matrix44;
            matrix44.m00 = -matrix1.m00;
            matrix44.m01 = -matrix1.m01;
            matrix44.m02 = -matrix1.m02;
            matrix44.m03 = -matrix1.m03;
            matrix44.m10 = -matrix1.m10;
            matrix44.m11 = -matrix1.m11;
            matrix44.m12 = -matrix1.m12;
            matrix44.m13 = -matrix1.m13;
            matrix44.m20 = -matrix1.m20;
            matrix44.m21 = -matrix1.m21;
            matrix44.m22 = -matrix1.m22;
            matrix44.m23 = -matrix1.m23;
            matrix44.m30 = -matrix1.m30;
            matrix44.m31 = -matrix1.m31;
            matrix44.m32 = -matrix1.m32;
            matrix44.m33 = -matrix1.m33;
            return matrix44;
        }

        public static Matrix4x4 operator +(Matrix4x4 matrix1, Matrix4x4 matrix2)
        {
            Matrix4x4 matrix44;
            matrix44.m00 = matrix1.m00 + matrix2.m00;
            matrix44.m01 = matrix1.m01 + matrix2.m01;
            matrix44.m02 = matrix1.m02 + matrix2.m02;
            matrix44.m03 = matrix1.m03 + matrix2.m03;
            matrix44.m10 = matrix1.m10 + matrix2.m10;
            matrix44.m11 = matrix1.m11 + matrix2.m11;
            matrix44.m12 = matrix1.m12 + matrix2.m12;
            matrix44.m13 = matrix1.m13 + matrix2.m13;
            matrix44.m20 = matrix1.m20 + matrix2.m20;
            matrix44.m21 = matrix1.m21 + matrix2.m21;
            matrix44.m22 = matrix1.m22 + matrix2.m22;
            matrix44.m23 = matrix1.m23 + matrix2.m23;
            matrix44.m30 = matrix1.m30 + matrix2.m30;
            matrix44.m31 = matrix1.m31 + matrix2.m31;
            matrix44.m32 = matrix1.m32 + matrix2.m32;
            matrix44.m33 = matrix1.m33 + matrix2.m33;
            return matrix44;
        }

        public static Matrix4x4 operator -(Matrix4x4 matrix1, Matrix4x4 matrix2)
        {
            Matrix4x4 matrix44;
            matrix44.m00 = matrix1.m00 - matrix2.m00;
            matrix44.m01 = matrix1.m01 - matrix2.m01;
            matrix44.m02 = matrix1.m02 - matrix2.m02;
            matrix44.m03 = matrix1.m03 - matrix2.m03;
            matrix44.m10 = matrix1.m10 - matrix2.m10;
            matrix44.m11 = matrix1.m11 - matrix2.m11;
            matrix44.m12 = matrix1.m12 - matrix2.m12;
            matrix44.m13 = matrix1.m13 - matrix2.m13;
            matrix44.m20 = matrix1.m20 - matrix2.m20;
            matrix44.m21 = matrix1.m21 - matrix2.m21;
            matrix44.m22 = matrix1.m22 - matrix2.m22;
            matrix44.m23 = matrix1.m23 - matrix2.m23;
            matrix44.m30 = matrix1.m30 - matrix2.m30;
            matrix44.m31 = matrix1.m31 - matrix2.m31;
            matrix44.m32 = matrix1.m32 - matrix2.m32;
            matrix44.m33 = matrix1.m33 - matrix2.m33;
            return matrix44;
        }
        #endregion
    }
}