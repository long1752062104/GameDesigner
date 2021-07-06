using System;

namespace Net
{
    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
    [Serializable]
    public struct Vector4 : IEquatable<Vector4>
    {
        #region 源码
        // Token: 0x060051B5 RID: 20917 RVA: 0x0008E19B File Offset: 0x0008C39B
        public Vector4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        // Token: 0x060051B6 RID: 20918 RVA: 0x0008E1BB File Offset: 0x0008C3BB
        public Vector4(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            w = 0f;
        }

        // Token: 0x060051B7 RID: 20919 RVA: 0x0008E1DE File Offset: 0x0008C3DE
        public Vector4(float x, float y)
        {
            this.x = x;
            this.y = y;
            z = 0f;
            w = 0f;
        }

        // Token: 0x170012B6 RID: 4790
        public float this[int index]
        {
            get
            {
                float result;
                switch (index)
                {
                    case 0:
                        result = x;
                        break;
                    case 1:
                        result = y;
                        break;
                    case 2:
                        result = z;
                        break;
                    case 3:
                        result = w;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector4 index!");
                }
                return result;
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector4 index!");
                }
            }
        }

        // Token: 0x060051BA RID: 20922 RVA: 0x0008E19B File Offset: 0x0008C39B
        public void Set(float newX, float newY, float newZ, float newW)
        {
            x = newX;
            y = newY;
            z = newZ;
            w = newW;
        }

        // Token: 0x060051BB RID: 20923 RVA: 0x0008E2D4 File Offset: 0x0008C4D4
        public static Vector4 Lerp(Vector4 a, Vector4 b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Vector4(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t, a.w + (b.w - a.w) * t);
        }

        // Token: 0x060051BC RID: 20924 RVA: 0x0008E35C File Offset: 0x0008C55C
        public static Vector4 LerpUnclamped(Vector4 a, Vector4 b, float t)
        {
            return new Vector4(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t, a.w + (b.w - a.w) * t);
        }

        // Token: 0x060051BD RID: 20925 RVA: 0x0008E3DC File Offset: 0x0008C5DC
        public static Vector4 MoveTowards(Vector4 current, Vector4 target, float maxDistanceDelta)
        {
            Vector4 a = target - current;
            float magnitude = a.magnitude;
            Vector4 result;
            if (magnitude <= maxDistanceDelta || magnitude == 0f)
            {
                result = target;
            }
            else
            {
                result = current + a / magnitude * maxDistanceDelta;
            }
            return result;
        }

        // Token: 0x060051BE RID: 20926 RVA: 0x0008E430 File Offset: 0x0008C630
        public static Vector4 Scale(Vector4 a, Vector4 b)
        {
            return new Vector4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
        }

        // Token: 0x060051BF RID: 20927 RVA: 0x0008E488 File Offset: 0x0008C688
        public void Scale(Vector4 scale)
        {
            x *= scale.x;
            y *= scale.y;
            z *= scale.z;
            w *= scale.w;
        }

        // Token: 0x060051C0 RID: 20928 RVA: 0x0008E4E8 File Offset: 0x0008C6E8
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() << 2 ^ z.GetHashCode() >> 2 ^ w.GetHashCode() >> 1;
        }

        // Token: 0x060051C1 RID: 20929 RVA: 0x0008E54C File Offset: 0x0008C74C
        public override bool Equals(object other)
        {
            return other is Vector4 && Equals((Vector4)other);
        }

        // Token: 0x060051C2 RID: 20930 RVA: 0x0008E580 File Offset: 0x0008C780
        public bool Equals(Vector4 other)
        {
            return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && w.Equals(other.w);
        }

        // Token: 0x060051C3 RID: 20931 RVA: 0x0008E5F0 File Offset: 0x0008C7F0
        public static Vector4 Normalize(Vector4 a)
        {
            float num = Vector4.Magnitude(a);
            Vector4 result;
            if (num > 1E-05f)
            {
                result = a / num;
            }
            else
            {
                result = Vector4.zero;
            }
            return result;
        }

        // Token: 0x060051C4 RID: 20932 RVA: 0x0008E62C File Offset: 0x0008C82C
        public void Normalize()
        {
            float num = Vector4.Magnitude(this);
            if (num > 1E-05f)
            {
                this /= num;
            }
            else
            {
                this = Vector4.zero;
            }
        }

        // Token: 0x170012B7 RID: 4791
        // (get) Token: 0x060051C5 RID: 20933 RVA: 0x0008E674 File Offset: 0x0008C874
        [Newtonsoft_X.Json.JsonIgnore]
        [ProtoBuf.ProtoIgnore]
        public Vector4 normalized
        {
            get
            {
                return Vector4.Normalize(this);
            }
        }

        // Token: 0x060051C6 RID: 20934 RVA: 0x0008E694 File Offset: 0x0008C894
        public static float Dot(Vector4 a, Vector4 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        // Token: 0x060051C7 RID: 20935 RVA: 0x0008E6E8 File Offset: 0x0008C8E8
        public static Vector4 Project(Vector4 a, Vector4 b)
        {
            return b * Vector4.Dot(a, b) / Vector4.Dot(b, b);
        }

        // Token: 0x060051C8 RID: 20936 RVA: 0x0008E718 File Offset: 0x0008C918
        public static float Distance(Vector4 a, Vector4 b)
        {
            return Vector4.Magnitude(a - b);
        }

        // Token: 0x060051C9 RID: 20937 RVA: 0x0008E73C File Offset: 0x0008C93C
        public static float Magnitude(Vector4 a)
        {
            return Mathf.Sqrt(Vector4.Dot(a, a));
        }

        // Token: 0x170012B8 RID: 4792
        // (get) Token: 0x060051CA RID: 20938 RVA: 0x0008E760 File Offset: 0x0008C960
        public float magnitude
        {
            get
            {
                return Mathf.Sqrt(Vector4.Dot(this, this));
            }
        }

        // Token: 0x170012B9 RID: 4793
        // (get) Token: 0x060051CB RID: 20939 RVA: 0x0008E78C File Offset: 0x0008C98C
        public float sqrMagnitude
        {
            get
            {
                return Vector4.Dot(this, this);
            }
        }

        // Token: 0x060051CC RID: 20940 RVA: 0x0008E7B4 File Offset: 0x0008C9B4
        public static Vector4 Min(Vector4 lhs, Vector4 rhs)
        {
            return new Vector4(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y), Mathf.Min(lhs.z, rhs.z), Mathf.Min(lhs.w, rhs.w));
        }

        // Token: 0x060051CD RID: 20941 RVA: 0x0008E81C File Offset: 0x0008CA1C
        public static Vector4 Max(Vector4 lhs, Vector4 rhs)
        {
            return new Vector4(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y), Mathf.Max(lhs.z, rhs.z), Mathf.Max(lhs.w, rhs.w));
        }

        // Token: 0x170012BA RID: 4794
        // (get) Token: 0x060051CE RID: 20942 RVA: 0x0008E884 File Offset: 0x0008CA84
        public static Vector4 zero
        {
            get
            {
                return Vector4.zeroVector;
            }
        }

        // Token: 0x170012BB RID: 4795
        // (get) Token: 0x060051CF RID: 20943 RVA: 0x0008E8A0 File Offset: 0x0008CAA0
        public static Vector4 one
        {
            get
            {
                return Vector4.oneVector;
            }
        }

        // Token: 0x170012BC RID: 4796
        // (get) Token: 0x060051D0 RID: 20944 RVA: 0x0008E8BC File Offset: 0x0008CABC
        public static Vector4 positiveInfinity
        {
            get
            {
                return Vector4.positiveInfinityVector;
            }
        }

        // Token: 0x170012BD RID: 4797
        // (get) Token: 0x060051D1 RID: 20945 RVA: 0x0008E8D8 File Offset: 0x0008CAD8
        public static Vector4 negativeInfinity
        {
            get
            {
                return Vector4.negativeInfinityVector;
            }
        }

        // Token: 0x060051D2 RID: 20946 RVA: 0x0008E8F4 File Offset: 0x0008CAF4
        public static Vector4 operator +(Vector4 a, Vector4 b)
        {
            return new Vector4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        }

        // Token: 0x060051D3 RID: 20947 RVA: 0x0008E94C File Offset: 0x0008CB4C
        public static Vector4 operator -(Vector4 a, Vector4 b)
        {
            return new Vector4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
        }

        // Token: 0x060051D4 RID: 20948 RVA: 0x0008E9A4 File Offset: 0x0008CBA4
        public static Vector4 operator -(Vector4 a)
        {
            return new Vector4(-a.x, -a.y, -a.z, -a.w);
        }

        // Token: 0x060051D5 RID: 20949 RVA: 0x0008E9E0 File Offset: 0x0008CBE0
        public static Vector4 operator *(Vector4 a, float d)
        {
            return new Vector4(a.x * d, a.y * d, a.z * d, a.w * d);
        }

        // Token: 0x060051D6 RID: 20950 RVA: 0x0008EA20 File Offset: 0x0008CC20
        public static Vector4 operator *(float d, Vector4 a)
        {
            return new Vector4(a.x * d, a.y * d, a.z * d, a.w * d);
        }

        // Token: 0x060051D7 RID: 20951 RVA: 0x0008EA60 File Offset: 0x0008CC60
        public static Vector4 operator /(Vector4 a, float d)
        {
            return new Vector4(a.x / d, a.y / d, a.z / d, a.w / d);
        }

        // Token: 0x060051D8 RID: 20952 RVA: 0x0008EAA0 File Offset: 0x0008CCA0
        public static bool operator ==(Vector4 lhs, Vector4 rhs)
        {
            return Vector4.SqrMagnitude(lhs - rhs) < 9.9999994E-11f;
        }

        // Token: 0x060051D9 RID: 20953 RVA: 0x0008EAC8 File Offset: 0x0008CCC8
        public static bool operator !=(Vector4 lhs, Vector4 rhs)
        {
            return !(lhs == rhs);
        }

        // Token: 0x060051DA RID: 20954 RVA: 0x0008EAE8 File Offset: 0x0008CCE8
        public static implicit operator Vector4(Vector3 v)
        {
            return new Vector4(v.x, v.y, v.z, 0f);
        }

        // Token: 0x060051DB RID: 20955 RVA: 0x0008EB1C File Offset: 0x0008CD1C
        public static implicit operator Vector3(Vector4 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        // Token: 0x060051DC RID: 20956 RVA: 0x0008EB4C File Offset: 0x0008CD4C
        public static implicit operator Vector4(Vector2 v)
        {
            return new Vector4(v.x, v.y, 0f, 0f);
        }

        // Token: 0x060051DD RID: 20957 RVA: 0x0008EB80 File Offset: 0x0008CD80
        public static implicit operator Vector2(Vector4 v)
        {
            return new Vector2(v.x, v.y);
        }

        // Token: 0x060051DE RID: 20958 RVA: 0x0008EBA8 File Offset: 0x0008CDA8
        public override string ToString()
        {
            return string.Format("({0:F1}, {1:F1}, {2:F1}, {3:F1})", new object[]
            {
                x,
                y,
                z,
                w
            });
        }

        // Token: 0x060051DF RID: 20959 RVA: 0x0008EC08 File Offset: 0x0008CE08
        public string ToString(string format)
        {
            return string.Format("({0}, {1}, {2}, {3})", new object[]
            {
                x.ToString(format),
                y.ToString(format),
                z.ToString(format),
                w.ToString(format)
            });
        }

        // Token: 0x060051E0 RID: 20960 RVA: 0x0008EC6C File Offset: 0x0008CE6C
        public static float SqrMagnitude(Vector4 a)
        {
            return Vector4.Dot(a, a);
        }

        // Token: 0x060051E1 RID: 20961 RVA: 0x0008EC88 File Offset: 0x0008CE88
        public float SqrMagnitude()
        {
            return Vector4.Dot(this, this);
        }

        // Token: 0x04001A7A RID: 6778
        public const float kEpsilon = 1E-05f;

        // Token: 0x04001A7B RID: 6779
        public float x;

        // Token: 0x04001A7C RID: 6780
        public float y;

        // Token: 0x04001A7D RID: 6781
        public float z;

        // Token: 0x04001A7E RID: 6782
        public float w;

        // Token: 0x04001A7F RID: 6783
        private static readonly Vector4 zeroVector = new Vector4(0f, 0f, 0f, 0f);

        // Token: 0x04001A80 RID: 6784
        private static readonly Vector4 oneVector = new Vector4(1f, 1f, 1f, 1f);

        // Token: 0x04001A81 RID: 6785
        private static readonly Vector4 positiveInfinityVector = new Vector4(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

        // Token: 0x04001A82 RID: 6786
        private static readonly Vector4 negativeInfinityVector = new Vector4(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
        #endregion

        #region 网上代码

        public float Length()
        {
            return (float)Math.Sqrt(x * (double)x + y * (double)y + z * (double)z +
                                     w * (double)w);
        }

        public float LengthSquared()
        {
            return (float)(x * (double)x + y * (double)y + z * (double)z +
                w * (double)w);
        }

        public static void Distance(ref Vector4 value1, ref Vector4 value2, out float result)
        {
            float num1 = value1.x - value2.x;
            float num2 = value1.y - value2.y;
            float num3 = value1.z - value2.z;
            float num4 = value1.w - value2.w;
            float num5 = (float)(num1 * (double)num1 + num2 * (double)num2 + num3 * (double)num3 +
                num4 * (double)num4);
            result = (float)Math.Sqrt(num5);
        }

        public static float DistanceSquared(Vector4 value1, Vector4 value2)
        {
            float num1 = value1.x - value2.x;
            float num2 = value1.y - value2.y;
            float num3 = value1.z - value2.z;
            float num4 = value1.w - value2.w;
            return (float)(num1 * (double)num1 + num2 * (double)num2 + num3 * (double)num3 +
                num4 * (double)num4);
        }

        public static void DistanceSquared(ref Vector4 value1, ref Vector4 value2, out float result)
        {
            float num1 = value1.x - value2.x;
            float num2 = value1.y - value2.y;
            float num3 = value1.z - value2.z;
            float num4 = value1.w - value2.w;
            result = (float)(num1 * (double)num1 + num2 * (double)num2 + num3 * (double)num3 +
                num4 * (double)num4);
        }

        public static void Dot(ref Vector4 vector1, ref Vector4 vector2, out float result)
        {
            result = (float)(vector1.x * (double)vector2.x + vector1.y * (double)vector2.y +
                vector1.z * (double)vector2.z + vector1.w * (double)vector2.w);
        }

        public static void Normalize(ref Vector4 vector, out Vector4 result)
        {
            float num1 = (float)(vector.x * (double)vector.x + vector.y * (double)vector.y +
                vector.z * (double)vector.z + vector.w * (double)vector.w);
            if (num1 < (double)Mathf.Epsilon)
            {
                result = vector;
            }
            else
            {
                float num2 = 1f / (float)Math.Sqrt(num1);
                result.x = vector.x * num2;
                result.y = vector.y * num2;
                result.z = vector.z * num2;
                result.w = vector.w * num2;
            }
        }

        public static void Min(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
        {
            result.x = value1.x < (double)value2.x ? value1.x : value2.x;
            result.y = value1.y < (double)value2.y ? value1.y : value2.y;
            result.z = value1.z < (double)value2.z ? value1.z : value2.z;
            result.w = value1.w < (double)value2.w ? value1.w : value2.w;
        }


        public static void Max(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
        {
            result.x = value1.x > (double)value2.x ? value1.x : value2.x;
            result.y = value1.y > (double)value2.y ? value1.y : value2.y;
            result.z = value1.z > (double)value2.z ? value1.z : value2.z;
            result.w = value1.w > (double)value2.w ? value1.w : value2.w;
        }

        public static Vector4 Clamp(Vector4 value1, Vector4 min, Vector4 max)
        {
            float x = value1.x;
            float num1 = x > (double)max.x ? max.x : x;
            float num2 = num1 < (double)min.x ? min.x : num1;
            float y = value1.y;
            float num3 = y > (double)max.y ? max.y : y;
            float num4 = num3 < (double)min.y ? min.y : num3;
            float z = value1.z;
            float num5 = z > (double)max.z ? max.z : z;
            float num6 = num5 < (double)min.z ? min.z : num5;
            float w = value1.w;
            float num7 = w > (double)max.w ? max.w : w;
            float num8 = num7 < (double)min.w ? min.w : num7;
            Vector4 vector4;
            vector4.x = num2;
            vector4.y = num4;
            vector4.z = num6;
            vector4.w = num8;
            return vector4;
        }

        public static void Clamp(ref Vector4 value1, ref Vector4 min, ref Vector4 max, out Vector4 result)
        {
            float x = value1.x;
            float num1 = x > (double)max.x ? max.x : x;
            float num2 = num1 < (double)min.x ? min.x : num1;
            float y = value1.y;
            float num3 = y > (double)max.y ? max.y : y;
            float num4 = num3 < (double)min.y ? min.y : num3;
            float z = value1.z;
            float num5 = z > (double)max.z ? max.z : z;
            float num6 = num5 < (double)min.z ? min.z : num5;
            float w = value1.w;
            float num7 = w > (double)max.w ? max.w : w;
            float num8 = num7 < (double)min.w ? min.w : num7;
            result.x = num2;
            result.y = num4;
            result.z = num6;
            result.w = num8;
        }


        public static void Lerp(ref Vector4 value1, ref Vector4 value2, float amount, out Vector4 result)
        {
            result.x = value1.x + (value2.x - value1.x) * amount;
            result.y = value1.y + (value2.y - value1.y) * amount;
            result.z = value1.z + (value2.z - value1.z) * amount;
            result.w = value1.w + (value2.w - value1.w) * amount;
        }

        public static Vector4 SmoothStep(Vector4 value1, Vector4 value2, float amount)
        {
            amount = amount > 1.0 ? 1f : (amount < 0.0 ? 0.0f : amount);
            amount = (float)(amount * (double)amount * (3.0 - 2.0 * amount));
            Vector4 vector4;
            vector4.x = value1.x + (value2.x - value1.x) * amount;
            vector4.y = value1.y + (value2.y - value1.y) * amount;
            vector4.z = value1.z + (value2.z - value1.z) * amount;
            vector4.w = value1.w + (value2.w - value1.w) * amount;
            return vector4;
        }

        public static void SmoothStep(ref Vector4 value1, ref Vector4 value2, float amount, out Vector4 result)
        {
            amount = amount > 1.0 ? 1f : (amount < 0.0 ? 0.0f : amount);
            amount = (float)(amount * (double)amount * (3.0 - 2.0 * amount));
            result.x = value1.x + (value2.x - value1.x) * amount;
            result.y = value1.y + (value2.y - value1.y) * amount;
            result.z = value1.z + (value2.z - value1.z) * amount;
            result.w = value1.w + (value2.w - value1.w) * amount;
        }

        public static Vector4 Hermite(Vector4 value1, Vector4 tangent1, Vector4 value2, Vector4 tangent2, float amount)
        {
            float num1 = amount * amount;
            float num2 = amount * num1;
            float num3 = (float)(2.0 * num2 - 3.0 * num1 + 1.0);
            float num4 = (float)(-2.0 * num2 + 3.0 * num1);
            float num5 = num2 - 2f * num1 + amount;
            float num6 = num2 - num1;
            Vector4 vector4;
            vector4.x = (float)(value1.x * (double)num3 + value2.x * (double)num4 + tangent1.x * (double)num5 +
                tangent2.x * (double)num6);
            vector4.y = (float)(value1.y * (double)num3 + value2.y * (double)num4 + tangent1.y * (double)num5 +
                tangent2.y * (double)num6);
            vector4.z = (float)(value1.z * (double)num3 + value2.z * (double)num4 + tangent1.z * (double)num5 +
                tangent2.z * (double)num6);
            vector4.w = (float)(value1.w * (double)num3 + value2.w * (double)num4 + tangent1.w * (double)num5 +
                tangent2.w * (double)num6);
            return vector4;
        }

        public static void Hermite(
            ref Vector4 value1, ref Vector4 tangent1, ref Vector4 value2, ref Vector4 tangent2, float amount, out Vector4 result)
        {
            float num1 = amount * amount;
            float num2 = amount * num1;
            float num3 = (float)(2.0 * num2 - 3.0 * num1 + 1.0);
            float num4 = (float)(-2.0 * num2 + 3.0 * num1);
            float num5 = num2 - 2f * num1 + amount;
            float num6 = num2 - num1;
            result.x = (float)(value1.x * (double)num3 + value2.x * (double)num4 + tangent1.x * (double)num5 +
                tangent2.x * (double)num6);
            result.y = (float)(value1.y * (double)num3 + value2.y * (double)num4 + tangent1.y * (double)num5 +
                tangent2.y * (double)num6);
            result.z = (float)(value1.z * (double)num3 + value2.z * (double)num4 + tangent1.z * (double)num5 +
                tangent2.z * (double)num6);
            result.w = (float)(value1.w * (double)num3 + value2.w * (double)num4 + tangent1.w * (double)num5 +
                tangent2.w * (double)num6);
        }

        public static void Project(ref Vector4 vector, ref Vector4 onNormal, out Vector4 result)
        {
            result = onNormal * Vector4.Dot(vector, onNormal) / Vector4.Dot(onNormal, onNormal);
        }

        public static Vector4 Negate(Vector4 value)
        {
            Vector4 vector4;
            vector4.x = -value.x;
            vector4.y = -value.y;
            vector4.z = -value.z;
            vector4.w = -value.w;
            return vector4;
        }

        public static void Negate(ref Vector4 value, out Vector4 result)
        {
            result.x = -value.x;
            result.y = -value.y;
            result.z = -value.z;
            result.w = -value.w;
        }

        public static Vector4 Add(Vector4 value1, Vector4 value2)
        {
            Vector4 vector4;
            vector4.x = value1.x + value2.x;
            vector4.y = value1.y + value2.y;
            vector4.z = value1.z + value2.z;
            vector4.w = value1.w + value2.w;
            return vector4;
        }

        public static void Add(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
        {
            result.x = value1.x + value2.x;
            result.y = value1.y + value2.y;
            result.z = value1.z + value2.z;
            result.w = value1.w + value2.w;
        }

        public static Vector4 Sub(Vector4 value1, Vector4 value2)
        {
            Vector4 vector4;
            vector4.x = value1.x - value2.x;
            vector4.y = value1.y - value2.y;
            vector4.z = value1.z - value2.z;
            vector4.w = value1.w - value2.w;
            return vector4;
        }

        public static void Sub(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
        {
            result.x = value1.x - value2.x;
            result.y = value1.y - value2.y;
            result.z = value1.z - value2.z;
            result.w = value1.w - value2.w;
        }

        public static Vector4 Multiply(Vector4 value1, Vector4 value2)
        {
            Vector4 vector4;
            vector4.x = value1.x * value2.x;
            vector4.y = value1.y * value2.y;
            vector4.z = value1.z * value2.z;
            vector4.w = value1.w * value2.w;
            return vector4;
        }

        public static void Multiply(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
        {
            result.x = value1.x * value2.x;
            result.y = value1.y * value2.y;
            result.z = value1.z * value2.z;
            result.w = value1.w * value2.w;
        }

        public static Vector4 Multiply(Vector4 value1, float scaleFactor)
        {
            Vector4 vector4;
            vector4.x = value1.x * scaleFactor;
            vector4.y = value1.y * scaleFactor;
            vector4.z = value1.z * scaleFactor;
            vector4.w = value1.w * scaleFactor;
            return vector4;
        }

        public static void Multiply(ref Vector4 value1, float scaleFactor, out Vector4 result)
        {
            result.x = value1.x * scaleFactor;
            result.y = value1.y * scaleFactor;
            result.z = value1.z * scaleFactor;
            result.w = value1.w * scaleFactor;
        }

        public static Vector4 Divide(Vector4 value1, Vector4 value2)
        {
            Vector4 vector4;
            vector4.x = value1.x / value2.x;
            vector4.y = value1.y / value2.y;
            vector4.z = value1.z / value2.z;
            vector4.w = value1.w / value2.w;
            return vector4;
        }

        public static void Divide(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
        {
            result.x = value1.x / value2.x;
            result.y = value1.y / value2.y;
            result.z = value1.z / value2.z;
            result.w = value1.w / value2.w;
        }

        public static Vector4 Divide(Vector4 value1, float divider)
        {
            float num = 1f / divider;
            Vector4 vector4;
            vector4.x = value1.x * num;
            vector4.y = value1.y * num;
            vector4.z = value1.z * num;
            vector4.w = value1.w * num;
            return vector4;
        }

        public static void Divide(ref Vector4 value1, float divider, out Vector4 result)
        {
            float num = 1f / divider;
            result.x = value1.x * num;
            result.y = value1.y * num;
            result.z = value1.z * num;
            result.w = value1.w * num;
        }

        public static Vector4 operator *(Vector4 value1, Vector4 value2)
        {
            Vector4 vector4;
            vector4.x = value1.x * value2.x;
            vector4.y = value1.y * value2.y;
            vector4.z = value1.z * value2.z;
            vector4.w = value1.w * value2.w;
            return vector4;
        }

        public static Vector4 operator /(Vector4 value1, Vector4 value2)
        {
            Vector4 vector4;
            vector4.x = value1.x / value2.x;
            vector4.y = value1.y / value2.y;
            vector4.z = value1.z / value2.z;
            vector4.w = value1.w / value2.w;
            return vector4;
        }
        #endregion

        // Token: 0x06005151 RID: 20817 RVA: 0x0008CDEC File Offset: 0x0008AFEC
        public static UnityEngine.Vector4 operator +(UnityEngine.Vector4 a, Vector4 b)
        {
            return new UnityEngine.Vector4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        }

        // Token: 0x06005152 RID: 20818 RVA: 0x0008CE24 File Offset: 0x0008B024
        public static UnityEngine.Vector4 operator -(UnityEngine.Vector4 a, Vector4 b)
        {
            return new UnityEngine.Vector4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
        }

        // Token: 0x06005153 RID: 20819 RVA: 0x0008CE5C File Offset: 0x0008B05C
        public static UnityEngine.Vector4 operator *(UnityEngine.Vector4 a, Vector4 b)
        {
            return new UnityEngine.Vector4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
        }

        // Token: 0x06005154 RID: 20820 RVA: 0x0008CE94 File Offset: 0x0008B094
        public static UnityEngine.Vector4 operator /(UnityEngine.Vector4 a, Vector4 b)
        {
            return new UnityEngine.Vector4(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);
        }


        public static Vector4 operator +(Vector4 a, UnityEngine.Vector4 b)
        {
            return new Vector4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        }

        // Token: 0x06005152 RID: 20818 RVA: 0x0008CE24 File Offset: 0x0008B024
        public static Vector4 operator -(Vector4 a, UnityEngine.Vector4 b)
        {
            return new Vector4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
        }

        // Token: 0x06005153 RID: 20819 RVA: 0x0008CE5C File Offset: 0x0008B05C
        public static Vector4 operator *(Vector4 a, UnityEngine.Vector4 b)
        {
            return new Vector4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
        }

        // Token: 0x06005154 RID: 20820 RVA: 0x0008CE94 File Offset: 0x0008B094
        public static Vector4 operator /(Vector4 a, UnityEngine.Vector4 b)
        {
            return new Vector4(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);
        }



        // Token: 0x06005159 RID: 20825 RVA: 0x0008CF7C File Offset: 0x0008B17C
        public static bool operator ==(UnityEngine.Vector4 lhs, Vector4 rhs)
        {
            return Vector4.SqrMagnitude(lhs - rhs) < 9.9999994E-11f;
        }

        // Token: 0x0600515A RID: 20826 RVA: 0x0008CFA8 File Offset: 0x0008B1A8
        public static bool operator !=(UnityEngine.Vector4 lhs, Vector4 rhs)
        {
            return !(lhs == rhs);
        }

        public static implicit operator UnityEngine.Vector4(Vector4 v)
        {
            return new UnityEngine.Vector4(v.x, v.y, v.z, v.w);
        }

        public static implicit operator Vector4(UnityEngine.Vector4 v)
        {
            return new Vector4(v.x, v.y, v.z, v.w);
        }
    }
}