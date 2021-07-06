using System;
using UnityEngine;
using UnityEngine.Internal;

namespace Net
{
    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
    [Serializable]
    public struct Vector3 : IEquatable<Vector3>
    {
        #region 源码
        // Token: 0x060048DB RID: 18651 RVA: 0x0007D58D File Offset: 0x0007B78D
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        // Token: 0x060048DC RID: 18652 RVA: 0x0007D5A5 File Offset: 0x0007B7A5
        public Vector3(float x, float y)
        {
            this.x = x;
            this.y = y;
            z = 0f;
        }

        // Token: 0x060048E4 RID: 18660 RVA: 0x0007D62C File Offset: 0x0007B82C
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
        }

        // Token: 0x060048E5 RID: 18661 RVA: 0x0007D69C File Offset: 0x0007B89C
        public static Vector3 LerpUnclamped(Vector3 a, Vector3 b, float t)
        {
            return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
        }

        // Token: 0x060048E6 RID: 18662 RVA: 0x0007D704 File Offset: 0x0007B904
        public static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta)
        {
            Vector3 a = target - current;
            float magnitude = a.magnitude;
            Vector3 result;
            if (magnitude <= maxDistanceDelta || magnitude < 1E-45f)
            {
                result = target;
            }
            else
            {
                result = current + a / magnitude * maxDistanceDelta;
            }
            return result;
        }

        // Token: 0x060048E7 RID: 18663 RVA: 0x0007D758 File Offset: 0x0007B958
        [ExcludeFromDocs]
        public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, float maxSpeed)
        {
            float deltaTime = Time.deltaTime;
            return Vector3.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        // Token: 0x060048E8 RID: 18664 RVA: 0x0007D780 File Offset: 0x0007B980
        [ExcludeFromDocs]
        public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime)
        {
            float deltaTime = Time.deltaTime;
            float positiveInfinity = float.PositiveInfinity;
            return Vector3.SmoothDamp(current, target, ref currentVelocity, smoothTime, positiveInfinity, deltaTime);
        }

        // Token: 0x060048E9 RID: 18665 RVA: 0x0007D7AC File Offset: 0x0007B9AC
        public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, [DefaultValue("Mathf.Infinity")] float maxSpeed, [DefaultValue("Time.deltaTime")] float deltaTime)
        {
            smoothTime = Mathf.Max(0.0001f, smoothTime);
            float num = 2f / smoothTime;
            float num2 = num * deltaTime;
            float d = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
            Vector3 vector = current - target;
            Vector3 vector2 = target;
            float maxLength = maxSpeed * smoothTime;
            vector = Vector3.ClampMagnitude(vector, maxLength);
            target = current - vector;
            Vector3 vector3 = (currentVelocity + num * vector) * deltaTime;
            currentVelocity = (currentVelocity - num * vector3) * d;
            Vector3 vector4 = target + (vector + vector3) * d;
            if (Vector3.Dot(vector2 - current, vector4 - vector2) > 0f)
            {
                vector4 = vector2;
                currentVelocity = (vector4 - vector2) / deltaTime;
            }
            return vector4;
        }

        // Token: 0x17001102 RID: 4354
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
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3 index!");
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
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3 index!");
                }
            }
        }

        // Token: 0x060048EC RID: 18668 RVA: 0x0007D58D File Offset: 0x0007B78D
        public void Set(float newX, float newY, float newZ)
        {
            x = newX;
            y = newY;
            z = newZ;
        }

        // Token: 0x060048ED RID: 18669 RVA: 0x0007D960 File Offset: 0x0007BB60
        public static Vector3 Scale(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        // Token: 0x060048EE RID: 18670 RVA: 0x0007D9A7 File Offset: 0x0007BBA7
        public void Scale(Vector3 scale)
        {
            x *= scale.x;
            y *= scale.y;
            z *= scale.z;
        }

        // Token: 0x060048EF RID: 18671 RVA: 0x0007D9E8 File Offset: 0x0007BBE8
        public static Vector3 Cross(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
        }

        // Token: 0x060048F0 RID: 18672 RVA: 0x0007DA60 File Offset: 0x0007BC60
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() << 2 ^ z.GetHashCode() >> 2;
        }

        // Token: 0x060048F1 RID: 18673 RVA: 0x0007DAB0 File Offset: 0x0007BCB0
        public override bool Equals(object other)
        {
            return other is Vector3 && Equals((Vector3)other);
        }

        // Token: 0x060048F2 RID: 18674 RVA: 0x0007DAE4 File Offset: 0x0007BCE4
        public bool Equals(Vector3 other)
        {
            return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);
        }

        // Token: 0x060048F3 RID: 18675 RVA: 0x0007DB3C File Offset: 0x0007BD3C
        public static Vector3 Reflect(Vector3 inDirection, Vector3 inNormal)
        {
            return -2f * Vector3.Dot(inNormal, inDirection) * inNormal + inDirection;
        }

        // Token: 0x060048F4 RID: 18676 RVA: 0x0007DB6C File Offset: 0x0007BD6C
        public static Vector3 Normalize(Vector3 value)
        {
            float num = Vector3.Magnitude(value);
            Vector3 result;
            if (num > 1E-05f)
            {
                result = value / num;
            }
            else
            {
                result = Vector3.zero;
            }
            return result;
        }

        // Token: 0x060048F5 RID: 18677 RVA: 0x0007DBA8 File Offset: 0x0007BDA8
        public void Normalize()
        {
            float num = Vector3.Magnitude(this);
            if (num > 1E-05f)
            {
                this /= num;
            }
            else
            {
                this = Vector3.zero;
            }
        }

        // Token: 0x17001103 RID: 4355
        // (get) Token: 0x060048F6 RID: 18678 RVA: 0x0007DBF0 File Offset: 0x0007BDF0
        [Newtonsoft_X.Json.JsonIgnore]
        [ProtoBuf.ProtoIgnore]
        public Vector3 normalized
        {
            get
            {
                return Vector3.Normalize(this);
            }
        }

        // Token: 0x060048F7 RID: 18679 RVA: 0x0007DC10 File Offset: 0x0007BE10
        public static float Dot(Vector3 lhs, Vector3 rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
        }

        // Token: 0x060048F8 RID: 18680 RVA: 0x0007DC54 File Offset: 0x0007BE54
        public static Vector3 Project(Vector3 vector, Vector3 onNormal)
        {
            float num = Vector3.Dot(onNormal, onNormal);
            Vector3 result;
            if (num < Mathf.Epsilon)
            {
                result = Vector3.zero;
            }
            else
            {
                result = onNormal * Vector3.Dot(vector, onNormal) / num;
            }
            return result;
        }

        // Token: 0x060048F9 RID: 18681 RVA: 0x0007DC9C File Offset: 0x0007BE9C
        public static Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal)
        {
            return vector - Vector3.Project(vector, planeNormal);
        }

        // Token: 0x060048FA RID: 18682 RVA: 0x0007DCC0 File Offset: 0x0007BEC0
        public static float Angle(Vector3 from, Vector3 to)
        {
            float num = Mathf.Sqrt(from.sqrMagnitude * to.sqrMagnitude);
            float result;
            if (num < 1E-15f)
            {
                result = 0f;
            }
            else
            {
                float f = Mathf.Clamp(Vector3.Dot(from, to) / num, -1f, 1f);
                result = Mathf.Acos(f) * 57.29578f;
            }
            return result;
        }

        // Token: 0x060048FB RID: 18683 RVA: 0x0007DD28 File Offset: 0x0007BF28
        public static float SignedAngle(Vector3 from, Vector3 to, Vector3 axis)
        {
            float num = Vector3.Angle(from, to);
            float num2 = Mathf.Sign(Vector3.Dot(axis, Vector3.Cross(from, to)));
            return num * num2;
        }

        // Token: 0x060048FC RID: 18684 RVA: 0x0007DD5C File Offset: 0x0007BF5C
        public static float Distance(Vector3 a, Vector3 b)
        {
            Vector3 vector = new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
            return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
        }

        // Token: 0x060048FD RID: 18685 RVA: 0x0007DDDC File Offset: 0x0007BFDC
        public static Vector3 ClampMagnitude(Vector3 vector, float maxLength)
        {
            Vector3 result;
            if (vector.sqrMagnitude > maxLength * maxLength)
            {
                result = vector.normalized * maxLength;
            }
            else
            {
                result = vector;
            }
            return result;
        }

        // Token: 0x060048FE RID: 18686 RVA: 0x0007DE14 File Offset: 0x0007C014
        public static float Magnitude(Vector3 vector)
        {
            return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
        }

        // Token: 0x17001104 RID: 4356
        // (get) Token: 0x060048FF RID: 18687 RVA: 0x0007DE60 File Offset: 0x0007C060
        public float magnitude
        {
            get
            {
                return Mathf.Sqrt(x * x + y * y + z * z);
            }
        }

        // Token: 0x06004900 RID: 18688 RVA: 0x0007DEA4 File Offset: 0x0007C0A4
        public static float SqrMagnitude(Vector3 vector)
        {
            return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
        }

        // Token: 0x17001105 RID: 4357
        // (get) Token: 0x06004901 RID: 18689 RVA: 0x0007DEE8 File Offset: 0x0007C0E8
        public float sqrMagnitude
        {
            get
            {
                return x * x + y * y + z * z;
            }
        }

        // Token: 0x06004902 RID: 18690 RVA: 0x0007DF28 File Offset: 0x0007C128
        public static Vector3 Min(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y), Mathf.Min(lhs.z, rhs.z));
        }

        // Token: 0x06004903 RID: 18691 RVA: 0x0007DF7C File Offset: 0x0007C17C
        public static Vector3 Max(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y), Mathf.Max(lhs.z, rhs.z));
        }

        // Token: 0x17001106 RID: 4358
        // (get) Token: 0x06004904 RID: 18692 RVA: 0x0007DFD0 File Offset: 0x0007C1D0
        public static Vector3 zero
        {
            get
            {
                return Vector3.zeroVector;
            }
        }

        // Token: 0x17001107 RID: 4359
        // (get) Token: 0x06004905 RID: 18693 RVA: 0x0007DFEC File Offset: 0x0007C1EC
        public static Vector3 one
        {
            get
            {
                return Vector3.oneVector;
            }
        }

        // Token: 0x17001108 RID: 4360
        // (get) Token: 0x06004906 RID: 18694 RVA: 0x0007E008 File Offset: 0x0007C208
        public static Vector3 forward
        {
            get
            {
                return Vector3.forwardVector;
            }
        }

        // Token: 0x17001109 RID: 4361
        // (get) Token: 0x06004907 RID: 18695 RVA: 0x0007E024 File Offset: 0x0007C224
        public static Vector3 back
        {
            get
            {
                return Vector3.backVector;
            }
        }

        // Token: 0x1700110A RID: 4362
        // (get) Token: 0x06004908 RID: 18696 RVA: 0x0007E040 File Offset: 0x0007C240
        public static Vector3 up
        {
            get
            {
                return Vector3.upVector;
            }
        }

        // Token: 0x1700110B RID: 4363
        // (get) Token: 0x06004909 RID: 18697 RVA: 0x0007E05C File Offset: 0x0007C25C
        public static Vector3 down
        {
            get
            {
                return Vector3.downVector;
            }
        }

        // Token: 0x1700110C RID: 4364
        // (get) Token: 0x0600490A RID: 18698 RVA: 0x0007E078 File Offset: 0x0007C278
        public static Vector3 left
        {
            get
            {
                return Vector3.leftVector;
            }
        }

        // Token: 0x1700110D RID: 4365
        // (get) Token: 0x0600490B RID: 18699 RVA: 0x0007E094 File Offset: 0x0007C294
        public static Vector3 right
        {
            get
            {
                return Vector3.rightVector;
            }
        }

        // Token: 0x1700110E RID: 4366
        // (get) Token: 0x0600490C RID: 18700 RVA: 0x0007E0B0 File Offset: 0x0007C2B0
        public static Vector3 positiveInfinity
        {
            get
            {
                return Vector3.positiveInfinityVector;
            }
        }

        // Token: 0x1700110F RID: 4367
        // (get) Token: 0x0600490D RID: 18701 RVA: 0x0007E0CC File Offset: 0x0007C2CC
        public static Vector3 negativeInfinity
        {
            get
            {
                return Vector3.negativeInfinityVector;
            }
        }

        // Token: 0x0600490E RID: 18702 RVA: 0x0007E0E8 File Offset: 0x0007C2E8
        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        // Token: 0x0600490F RID: 18703 RVA: 0x0007E130 File Offset: 0x0007C330
        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        // Token: 0x06004910 RID: 18704 RVA: 0x0007E178 File Offset: 0x0007C378
        public static Vector3 operator -(Vector3 a)
        {
            return new Vector3(-a.x, -a.y, -a.z);
        }

        // Token: 0x06004911 RID: 18705 RVA: 0x0007E1AC File Offset: 0x0007C3AC
        public static Vector3 operator *(Vector3 a, float d)
        {
            return new Vector3(a.x * d, a.y * d, a.z * d);
        }

        // Token: 0x06004912 RID: 18706 RVA: 0x0007E1E4 File Offset: 0x0007C3E4
        public static Vector3 operator *(float d, Vector3 a)
        {
            return new Vector3(a.x * d, a.y * d, a.z * d);
        }

        // Token: 0x06004913 RID: 18707 RVA: 0x0007E21C File Offset: 0x0007C41C
        public static Vector3 operator /(Vector3 a, float d)
        {
            return new Vector3(a.x / d, a.y / d, a.z / d);
        }

        // Token: 0x06004914 RID: 18708 RVA: 0x0007E254 File Offset: 0x0007C454
        public static bool operator ==(Vector3 lhs, Vector3 rhs)
        {
            return Vector3.SqrMagnitude(lhs - rhs) < 0.03f;//9.9999994E-11f;
        }

        // Token: 0x06004915 RID: 18709 RVA: 0x0007E27C File Offset: 0x0007C47C
        public static bool operator !=(Vector3 lhs, Vector3 rhs)
        {
            return !(lhs == rhs);
        }

        // Token: 0x06004916 RID: 18710 RVA: 0x0007E29C File Offset: 0x0007C49C
        public override string ToString()
        {
            return string.Format("({0:F1}, {1:F1}, {2:F1})", new object[]
            {
                x,
                y,
                z
            });
        }

        // Token: 0x06004917 RID: 18711 RVA: 0x0007E2EC File Offset: 0x0007C4EC
        public string ToString(string format)
        {
            return string.Format("({0}, {1}, {2})", new object[]
            {
                x.ToString(format),
                y.ToString(format),
                z.ToString(format)
            });
        }

        // Token: 0x17001110 RID: 4368
        // (get) Token: 0x06004918 RID: 18712 RVA: 0x0007E340 File Offset: 0x0007C540
        [Obsolete("Use Vector3.forward instead.")]
        public static Vector3 fwd
        {
            get
            {
                return new Vector3(0f, 0f, 1f);
            }
        }

        // Token: 0x06004919 RID: 18713 RVA: 0x0007E36C File Offset: 0x0007C56C
        [Obsolete("Use Vector3.Angle instead. AngleBetween uses radians instead of degrees and was deprecated for this reason")]
        public static float AngleBetween(Vector3 from, Vector3 to)
        {
            return Mathf.Acos(Mathf.Clamp(Vector3.Dot(from.normalized, to.normalized), -1f, 1f));
        }

        // Token: 0x0600491A RID: 18714 RVA: 0x0007E3A8 File Offset: 0x0007C5A8
        [Obsolete("Use Vector3.ProjectOnPlane instead.")]
        public static Vector3 Exclude(Vector3 excludeThis, Vector3 fromThat)
        {
            return Vector3.ProjectOnPlane(fromThat, excludeThis);
        }

        // Token: 0x04001916 RID: 6422
        public const float kEpsilon = 1E-05f;

        // Token: 0x04001917 RID: 6423
        public const float kEpsilonNormalSqrt = 1E-15f;

        // Token: 0x04001918 RID: 6424
        public float x;

        // Token: 0x04001919 RID: 6425
        public float y;

        // Token: 0x0400191A RID: 6426
        public float z;

        // Token: 0x0400191B RID: 6427
        private static readonly Vector3 zeroVector = new Vector3(0f, 0f, 0f);

        // Token: 0x0400191C RID: 6428
        private static readonly Vector3 oneVector = new Vector3(1f, 1f, 1f);

        // Token: 0x0400191D RID: 6429
        private static readonly Vector3 upVector = new Vector3(0f, 1f, 0f);

        // Token: 0x0400191E RID: 6430
        private static readonly Vector3 downVector = new Vector3(0f, -1f, 0f);

        // Token: 0x0400191F RID: 6431
        private static readonly Vector3 leftVector = new Vector3(-1f, 0f, 0f);

        // Token: 0x04001920 RID: 6432
        private static readonly Vector3 rightVector = new Vector3(1f, 0f, 0f);

        // Token: 0x04001921 RID: 6433
        private static readonly Vector3 forwardVector = new Vector3(0f, 0f, 1f);

        // Token: 0x04001922 RID: 6434
        private static readonly Vector3 backVector = new Vector3(0f, 0f, -1f);

        // Token: 0x04001923 RID: 6435
        private static readonly Vector3 positiveInfinityVector = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

        // Token: 0x04001924 RID: 6436
        private static readonly Vector3 negativeInfinityVector = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
        #endregion

        #region 网上代码
        private const float k1OverSqrt2 = 0.7071068f;
        private const float epsilon = 1E-05f;


        public Vector3(float value)
        {
            x = y = z = value;
        }

        public Vector3(Vector2 value, float z)
        {
            x = value.x;
            y = value.y;
            this.z = z;
        }

        public float Length()
        {
            return (float)Math.Sqrt(x * (double)x + y * (double)y + z * (double)z);
        }

        public float LengthSquared()
        {
            return (float)(x * (double)x + y * (double)y + z * (double)z);
        }

        public static void Distance(ref Vector3 value1, ref Vector3 value2, out float result)
        {
            float num1 = value1.x - value2.x;
            float num2 = value1.y - value2.y;
            float num3 = value1.z - value2.z;
            float num4 = (float)(num1 * (double)num1 + num2 * (double)num2 + num3 * (double)num3);
            result = (float)Math.Sqrt(num4);
        }

        public static float DistanceSquared(Vector3 value1, Vector3 value2)
        {
            float num1 = value1.x - value2.x;
            float num2 = value1.y - value2.y;
            float num3 = value1.z - value2.z;
            return (float)(num1 * (double)num1 + num2 * (double)num2 + num3 * (double)num3);
        }

        public static void DistanceSquared(ref Vector3 value1, ref Vector3 value2, out float result)
        {
            float num1 = value1.x - value2.x;
            float num2 = value1.y - value2.y;
            float num3 = value1.z - value2.z;
            result = (float)(num1 * (double)num1 + num2 * (double)num2 + num3 * (double)num3);
        }


        public static void Dot(ref Vector3 vector1, ref Vector3 vector2, out float result)
        {
            result = (float)(vector1.x * (double)vector2.x + vector1.y * (double)vector2.y +
                vector1.z * (double)vector2.z);
        }

        public static void Normalize(ref Vector3 value, out Vector3 result)
        {
            float num1 = (float)(value.x * (double)value.x + value.y * (double)value.y + value.z * (double)value.z);
            if (num1 < (double)Mathf.Epsilon)
            {
                result = value;
            }
            else
            {
                float num2 = 1f / (float)Math.Sqrt(num1);
                result.x = value.x * num2;
                result.y = value.y * num2;
                result.z = value.z * num2;
            }
        }


        public static void Cross(ref Vector3 vector1, ref Vector3 vector2, out Vector3 result)
        {
            float num1 = (float)(vector1.y * (double)vector2.z - vector1.z * (double)vector2.y);
            float num2 = (float)(vector1.z * (double)vector2.x - vector1.x * (double)vector2.z);
            float num3 = (float)(vector1.x * (double)vector2.y - vector1.y * (double)vector2.x);
            result.x = num1;
            result.y = num2;
            result.z = num3;
        }


        public static void Reflect(ref Vector3 vector, ref Vector3 normal, out Vector3 result)
        {
            float num =
                    (float)(vector.x * (double)normal.x + vector.y * (double)normal.y + vector.z * (double)normal.z);
            result.x = vector.x - 2f * num * normal.x;
            result.y = vector.y - 2f * num * normal.y;
            result.z = vector.z - 2f * num * normal.z;
        }

        public static void Min(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.x = value1.x < (double)value2.x ? value1.x : value2.x;
            result.y = value1.y < (double)value2.y ? value1.y : value2.y;
            result.z = value1.z < (double)value2.z ? value1.z : value2.z;
        }

        public static void Max(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.x = value1.x > (double)value2.x ? value1.x : value2.x;
            result.y = value1.y > (double)value2.y ? value1.y : value2.y;
            result.z = value1.z > (double)value2.z ? value1.z : value2.z;
        }

        public static Vector3 Clamp(Vector3 value1, Vector3 min, Vector3 max)
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
            Vector3 vector3;
            vector3.x = num2;
            vector3.y = num4;
            vector3.z = num6;
            return vector3;
        }

        public static void Clamp(ref Vector3 value1, ref Vector3 min, ref Vector3 max, out Vector3 result)
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
            result.x = num2;
            result.y = num4;
            result.z = num6;
        }

        public static void Lerp(ref Vector3 value1, ref Vector3 value2, float amount, out Vector3 result)
        {
            result.x = value1.x + (value2.x - value1.x) * amount;
            result.y = value1.y + (value2.y - value1.y) * amount;
            result.z = value1.z + (value2.z - value1.z) * amount;
        }

        public static Vector3 SmoothStep(Vector3 value1, Vector3 value2, float amount)
        {
            amount = amount > 1.0 ? 1f : (amount < 0.0 ? 0.0f : amount);
            amount = (float)(amount * (double)amount * (3.0 - 2.0 * amount));
            Vector3 vector3;
            vector3.x = value1.x + (value2.x - value1.x) * amount;
            vector3.y = value1.y + (value2.y - value1.y) * amount;
            vector3.z = value1.z + (value2.z - value1.z) * amount;
            return vector3;
        }

        public static void SmoothStep(ref Vector3 value1, ref Vector3 value2, float amount, out Vector3 result)
        {
            amount = amount > 1.0 ? 1f : (amount < 0.0 ? 0.0f : amount);
            amount = (float)(amount * (double)amount * (3.0 - 2.0 * amount));
            result.x = value1.x + (value2.x - value1.x) * amount;
            result.y = value1.y + (value2.y - value1.y) * amount;
            result.z = value1.z + (value2.z - value1.z) * amount;
        }

        public static Vector3 Hermite(Vector3 value1, Vector3 tangent1, Vector3 value2, Vector3 tangent2, float amount)
        {
            float num1 = amount * amount;
            float num2 = amount * num1;
            float num3 = (float)(2.0 * num2 - 3.0 * num1 + 1.0);
            float num4 = (float)(-2.0 * num2 + 3.0 * num1);
            float num5 = num2 - 2f * num1 + amount;
            float num6 = num2 - num1;
            Vector3 vector3;
            vector3.x = (float)(value1.x * (double)num3 + value2.x * (double)num4 + tangent1.x * (double)num5 +
                tangent2.x * (double)num6);
            vector3.y = (float)(value1.y * (double)num3 + value2.y * (double)num4 + tangent1.y * (double)num5 +
                tangent2.y * (double)num6);
            vector3.z = (float)(value1.z * (double)num3 + value2.z * (double)num4 + tangent1.z * (double)num5 +
                tangent2.z * (double)num6);
            return vector3;
        }

        public static void Hermite(
            ref Vector3 value1, ref Vector3 tangent1, ref Vector3 value2, ref Vector3 tangent2, float amount, out Vector3 result)
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
        }

        public static Vector3 Negate(Vector3 value)
        {
            Vector3 vector3;
            vector3.x = -value.x;
            vector3.y = -value.y;
            vector3.z = -value.z;
            return vector3;
        }

        public static void Negate(ref Vector3 value, out Vector3 result)
        {
            result.x = -value.x;
            result.y = -value.y;
            result.z = -value.z;
        }

        public static Vector3 Add(Vector3 value1, Vector3 value2)
        {
            Vector3 vector3;
            vector3.x = value1.x + value2.x;
            vector3.y = value1.y + value2.y;
            vector3.z = value1.z + value2.z;
            return vector3;
        }

        public static void Add(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.x = value1.x + value2.x;
            result.y = value1.y + value2.y;
            result.z = value1.z + value2.z;
        }

        public static Vector3 Sub(Vector3 value1, Vector3 value2)
        {
            Vector3 vector3;
            vector3.x = value1.x - value2.x;
            vector3.y = value1.y - value2.y;
            vector3.z = value1.z - value2.z;
            return vector3;
        }

        public static void Sub(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.x = value1.x - value2.x;
            result.y = value1.y - value2.y;
            result.z = value1.z - value2.z;
        }

        public static Vector3 Multiply(Vector3 value1, Vector3 value2)
        {
            Vector3 vector3;
            vector3.x = value1.x * value2.x;
            vector3.y = value1.y * value2.y;
            vector3.z = value1.z * value2.z;
            return vector3;
        }

        public static void Multiply(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.x = value1.x * value2.x;
            result.y = value1.y * value2.y;
            result.z = value1.z * value2.z;
        }

        public static Vector3 Multiply(Vector3 value1, float scaleFactor)
        {
            Vector3 vector3;
            vector3.x = value1.x * scaleFactor;
            vector3.y = value1.y * scaleFactor;
            vector3.z = value1.z * scaleFactor;
            return vector3;
        }

        public static void Multiply(ref Vector3 value1, float scaleFactor, out Vector3 result)
        {
            result.x = value1.x * scaleFactor;
            result.y = value1.y * scaleFactor;
            result.z = value1.z * scaleFactor;
        }

        public static Vector3 Divide(Vector3 value1, Vector3 value2)
        {
            Vector3 vector3;
            vector3.x = value1.x / value2.x;
            vector3.y = value1.y / value2.y;
            vector3.z = value1.z / value2.z;
            return vector3;
        }

        public static void Divide(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.x = value1.x / value2.x;
            result.y = value1.y / value2.y;
            result.z = value1.z / value2.z;
        }

        public static Vector3 Divide(Vector3 value1, float divider)
        {
            float num = 1f / divider;
            Vector3 vector3;
            vector3.x = value1.x * num;
            vector3.y = value1.y * num;
            vector3.z = value1.z * num;
            return vector3;
        }

        public static void Divide(ref Vector3 value1, float divider, out Vector3 result)
        {
            float num = 1f / divider;
            result.x = value1.x * num;
            result.y = value1.y * num;
            result.z = value1.z * num;
        }

        private static float magnitudeStatic(ref Vector3 inV)
        {
            return (float)Math.Sqrt(Vector3.Dot(inV, inV));
        }

        private static Vector3 orthoNormalVectorFast(ref Vector3 n)
        {
            Vector3 vector3;
            if (Math.Abs(n.z) > (double)Vector3.k1OverSqrt2)
            {
                float num = 1f / (float)Math.Sqrt(n.y * (double)n.y + n.z * (double)n.z);
                vector3.x = 0.0f;
                vector3.y = -n.z * num;
                vector3.z = n.y * num;
            }
            else
            {
                float num = 1f / (float)Math.Sqrt(n.x * (double)n.x + n.y * (double)n.y);
                vector3.x = -n.y * num;
                vector3.y = n.x * num;
                vector3.z = 0.0f;
            }

            return vector3;
        }

        public static void OrthoNormalize(ref Vector3 normal, ref Vector3 tangent)
        {
            float num1 = Vector3.magnitudeStatic(ref normal);
            if (num1 > (double)Mathf.Epsilon)
                normal /= num1;
            else
                normal = new Vector3(1f, 0.0f, 0.0f);
            float num2 = Vector3.Dot(normal, tangent);
            tangent -= num2 * normal;
            float num3 = Vector3.magnitudeStatic(ref tangent);
            if (num3 < (double)Mathf.Epsilon)
                tangent = Vector3.orthoNormalVectorFast(ref normal);
            else
                tangent /= num3;
        }

        public static void OrthoNormalize(ref Vector3 normal, ref Vector3 tangent, ref Vector3 binormal)
        {
            float num1 = Vector3.magnitudeStatic(ref normal);
            if (num1 > (double)Mathf.Epsilon)
                normal /= num1;
            else
                normal = new Vector3(1f, 0.0f, 0.0f);
            float num2 = Vector3.Dot(normal, tangent);
            tangent -= num2 * normal;
            float num3 = Vector3.magnitudeStatic(ref tangent);
            if (num3 > (double)Mathf.Epsilon)
                tangent /= num3;
            else
                tangent = Vector3.orthoNormalVectorFast(ref normal);
            float num4 = Vector3.Dot(tangent, binormal);
            float num5 = Vector3.Dot(normal, binormal);
            binormal -= num5 * normal + num4 * tangent;
            float num6 = Vector3.magnitudeStatic(ref binormal);
            if (num6 > (double)Mathf.Epsilon)
                binormal /= num6;
            else
                binormal = Vector3.Cross(normal, tangent);
        }

        public static void Project(ref Vector3 vector, ref Vector3 onNormal, out Vector3 result)
        {
            result = onNormal * Vector3.Dot(vector, onNormal) / Vector3.Dot(onNormal, onNormal);
        }


        public static void Angle(ref Vector3 from, ref Vector3 to, out float result)
        {
            from.Normalize();
            to.Normalize();
            Vector3.Dot(ref from, ref to, out float result1);
            result = Mathf.Cos(Mathf.Clamp(result1, -1f, 1f)) * 57.29578f;
        }

        public static Vector3 operator *(Vector3 value1, Vector3 value2)
        {
            Vector3 vector3;
            vector3.x = value1.x * value2.x;
            vector3.y = value1.y * value2.y;
            vector3.z = value1.z * value2.z;
            return vector3;
        }

        public static Vector3 operator /(Vector3 value1, Vector3 value2)
        {
            Vector3 vector3;
            vector3.x = value1.x / value2.x;
            vector3.y = value1.y / value2.y;
            vector3.z = value1.z / value2.z;
            return vector3;
        }
        #endregion

        public static UnityEngine.Vector3 operator +(UnityEngine.Vector3 a, Vector3 b)
        {
            return new UnityEngine.Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        // Token: 0x06005152 RID: 20818 RVA: 0x0008CE24 File Offset: 0x0008B024
        public static UnityEngine.Vector3 operator -(UnityEngine.Vector3 a, Vector3 b)
        {
            return new UnityEngine.Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        // Token: 0x06005153 RID: 20819 RVA: 0x0008CE5C File Offset: 0x0008B05C
        public static UnityEngine.Vector3 operator *(UnityEngine.Vector3 a, Vector3 b)
        {
            return new UnityEngine.Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        // Token: 0x06005154 RID: 20820 RVA: 0x0008CE94 File Offset: 0x0008B094
        public static UnityEngine.Vector3 operator /(UnityEngine.Vector3 a, Vector3 b)
        {
            return new UnityEngine.Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
        }


        public static Vector3 operator +(Vector3 a, UnityEngine.Vector3 b)
        {
            return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        // Token: 0x06005152 RID: 20818 RVA: 0x0008CE24 File Offset: 0x0008B024
        public static Vector3 operator -(Vector3 a, UnityEngine.Vector3 b)
        {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        // Token: 0x06005153 RID: 20819 RVA: 0x0008CE5C File Offset: 0x0008B05C
        public static Vector3 operator *(Vector3 a, UnityEngine.Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        // Token: 0x06005154 RID: 20820 RVA: 0x0008CE94 File Offset: 0x0008B094
        public static Vector3 operator /(Vector3 a, UnityEngine.Vector3 b)
        {
            return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
        }




        // Token: 0x06005159 RID: 20825 RVA: 0x0008CF7C File Offset: 0x0008B17C
        public static bool operator ==(UnityEngine.Vector3 lhs, Vector3 rhs)
        {
            return (lhs - rhs).sqrMagnitude < 9.9999994E-11f;
        }

        // Token: 0x0600515A RID: 20826 RVA: 0x0008CFA8 File Offset: 0x0008B1A8
        public static bool operator !=(UnityEngine.Vector3 lhs, Vector3 rhs)
        {
            return !(lhs == rhs);
        }

        public static implicit operator UnityEngine.Vector3(Vector3 v)
        {
            return new UnityEngine.Vector3(v.x, v.y, v.z);
        }

        public static implicit operator Vector3(UnityEngine.Vector3 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }
    }
}