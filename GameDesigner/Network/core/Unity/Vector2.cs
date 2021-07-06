using System;
using UnityEngine;

namespace Net
{
    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
    [Serializable]
    public struct Vector2 : IEquatable<Vector2>
    {
        #region "Vector2源码"
        // Token: 0x06005131 RID: 20785 RVA: 0x0008C610 File Offset: 0x0008A810
        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        // Token: 0x17001293 RID: 4755
        public float this[int index]
        {
            get
            {
                float result;
                if (index != 0)
                {
                    if (index != 1)
                    {
                        throw new IndexOutOfRangeException("Invalid Vector2 index!");
                    }
                    result = y;
                }
                else
                {
                    result = x;
                }
                return result;
            }
            set
            {
                if (index != 0)
                {
                    if (index != 1)
                    {
                        throw new IndexOutOfRangeException("Invalid Vector2 index!");
                    }
                    y = value;
                }
                else
                {
                    x = value;
                }
            }
        }

        // Token: 0x06005134 RID: 20788 RVA: 0x0008C610 File Offset: 0x0008A810
        public void Set(float newX, float newY)
        {
            x = newX;
            y = newY;
        }

        // Token: 0x06005135 RID: 20789 RVA: 0x0008C6A0 File Offset: 0x0008A8A0
        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Vector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
        }

        // Token: 0x06005136 RID: 20790 RVA: 0x0008C6F4 File Offset: 0x0008A8F4
        public static Vector2 LerpUnclamped(Vector2 a, Vector2 b, float t)
        {
            return new Vector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
        }

        // Token: 0x06005137 RID: 20791 RVA: 0x0008C740 File Offset: 0x0008A940
        public static Vector2 MoveTowards(Vector2 current, Vector2 target, float maxDistanceDelta)
        {
            Vector2 a = target - current;
            float magnitude = a.magnitude;
            Vector2 result;
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

        // Token: 0x06005138 RID: 20792 RVA: 0x0008C794 File Offset: 0x0008A994
        public static Vector2 Scale(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x * b.x, a.y * b.y);
        }

        // Token: 0x06005139 RID: 20793 RVA: 0x0008C7CC File Offset: 0x0008A9CC
        public void Scale(Vector2 scale)
        {
            x *= scale.x;
            y *= scale.y;
        }

        // Token: 0x0600513A RID: 20794 RVA: 0x0008C7F8 File Offset: 0x0008A9F8
        public void Normalize()
        {
            float magnitude = this.magnitude;
            if (magnitude > 1E-05f)
            {
                this /= magnitude;
            }
            else
            {
                this = Vector2.zero;
            }
        }

        // Token: 0x17001294 RID: 4756
        // (get) Token: 0x0600513B RID: 20795 RVA: 0x0008C83C File Offset: 0x0008AA3C
        [Newtonsoft_X.Json.JsonIgnore]
        [ProtoBuf.ProtoIgnore]
        public Vector2 normalized
        {
            get
            {
                Vector2 result = new Vector2(x, y);
                result.Normalize();
                return result;
            }
        }

        // Token: 0x0600513C RID: 20796 RVA: 0x0008C86C File Offset: 0x0008AA6C
        public override string ToString()
        {
            return string.Format("({0:F1}, {1:F1})", new object[]
            {
                x,
                y
            });
        }

        // Token: 0x0600513D RID: 20797 RVA: 0x0008C8B0 File Offset: 0x0008AAB0
        public string ToString(string format)
        {
            return string.Format("({0}, {1})", new object[]
            {
                x.ToString(format),
                y.ToString(format)
            });
        }

        // Token: 0x0600513E RID: 20798 RVA: 0x0008C8F4 File Offset: 0x0008AAF4
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() << 2;
        }

        // Token: 0x0600513F RID: 20799 RVA: 0x0008C930 File Offset: 0x0008AB30
        public override bool Equals(object other)
        {
            return other is Vector2 && Equals((Vector2)other);
        }

        // Token: 0x06005140 RID: 20800 RVA: 0x0008C964 File Offset: 0x0008AB64
        public bool Equals(Vector2 other)
        {
            return x.Equals(other.x) && y.Equals(other.y);
        }

        // Token: 0x06005141 RID: 20801 RVA: 0x0008C9A8 File Offset: 0x0008ABA8
        public static Vector2 Reflect(Vector2 inDirection, Vector2 inNormal)
        {
            return -2f * Vector2.Dot(inNormal, inDirection) * inNormal + inDirection;
        }

        // Token: 0x06005142 RID: 20802 RVA: 0x0008C9D8 File Offset: 0x0008ABD8
        public static Vector2 Perpendicular(Vector2 inDirection)
        {
            return new Vector2(-inDirection.y, inDirection.x);
        }

        // Token: 0x06005143 RID: 20803 RVA: 0x0008CA04 File Offset: 0x0008AC04
        public static float Dot(Vector2 lhs, Vector2 rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y;
        }

        // Token: 0x17001295 RID: 4757
        // (get) Token: 0x06005144 RID: 20804 RVA: 0x0008CA38 File Offset: 0x0008AC38
        public float magnitude
        {
            get
            {
                return Mathf.Sqrt(x * x + y * y);
            }
        }

        // Token: 0x17001296 RID: 4758
        // (get) Token: 0x06005145 RID: 20805 RVA: 0x0008CA70 File Offset: 0x0008AC70
        public float sqrMagnitude
        {
            get
            {
                return x * x + y * y;
            }
        }

        // Token: 0x06005146 RID: 20806 RVA: 0x0008CAA0 File Offset: 0x0008ACA0
        public static float Angle(Vector2 from, Vector2 to)
        {
            float num = Mathf.Sqrt(from.sqrMagnitude * to.sqrMagnitude);
            float result;
            if (num < 1E-15f)
            {
                result = 0f;
            }
            else
            {
                float f = Mathf.Clamp(Vector2.Dot(from, to) / num, -1f, 1f);
                result = Mathf.Acos(f) * 57.29578f;
            }
            return result;
        }

        // Token: 0x06005147 RID: 20807 RVA: 0x0008CB08 File Offset: 0x0008AD08
        public static float SignedAngle(Vector2 from, Vector2 to)
        {
            float num = Vector2.Angle(from, to);
            float num2 = Mathf.Sign(from.x * to.y - from.y * to.x);
            return num * num2;
        }

        // Token: 0x06005148 RID: 20808 RVA: 0x0008CB50 File Offset: 0x0008AD50
        public static float Distance(Vector2 a, Vector2 b)
        {
            return (a - b).magnitude;
        }

        // Token: 0x06005149 RID: 20809 RVA: 0x0008CB74 File Offset: 0x0008AD74
        public static Vector2 ClampMagnitude(Vector2 vector, float maxLength)
        {
            Vector2 result;
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

        // Token: 0x0600514A RID: 20810 RVA: 0x0008CBAC File Offset: 0x0008ADAC
        public static float SqrMagnitude(Vector2 a)
        {
            return a.x * a.x + a.y * a.y;
        }

        // Token: 0x0600514B RID: 20811 RVA: 0x0008CBE0 File Offset: 0x0008ADE0
        public float SqrMagnitude()
        {
            return x * x + y * y;
        }

        // Token: 0x0600514C RID: 20812 RVA: 0x0008CC10 File Offset: 0x0008AE10
        public static Vector2 Min(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y));
        }

        // Token: 0x0600514D RID: 20813 RVA: 0x0008CC50 File Offset: 0x0008AE50
        public static Vector2 Max(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y));
        }

        public static Vector2 SmoothDamp(Vector2 current, Vector2 target, ref Vector2 currentVelocity, float smoothTime, float maxSpeed)
        {
            float deltaTime = Time.deltaTime;
            return Vector2.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        public static Vector2 SmoothDamp(Vector2 current, Vector2 target, ref Vector2 currentVelocity, float smoothTime)
        {
            float deltaTime = Time.deltaTime;
            float positiveInfinity = float.PositiveInfinity;
            return Vector2.SmoothDamp(current, target, ref currentVelocity, smoothTime, positiveInfinity, deltaTime);
        }

        // Token: 0x06005150 RID: 20816 RVA: 0x0008CCE4 File Offset: 0x0008AEE4
        public static Vector2 SmoothDamp(Vector2 current, Vector2 target, ref Vector2 currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            smoothTime = Mathf.Max(0.0001f, smoothTime);
            float num = 2f / smoothTime;
            float num2 = num * deltaTime;
            float d = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
            Vector2 vector = current - target;
            Vector2 vector2 = target;
            float maxLength = maxSpeed * smoothTime;
            vector = Vector2.ClampMagnitude(vector, maxLength);
            target = current - vector;
            Vector2 vector3 = (currentVelocity + num * vector) * deltaTime;
            currentVelocity = (currentVelocity - num * vector3) * d;
            Vector2 vector4 = target + (vector + vector3) * d;
            if (Vector2.Dot(vector2 - current, vector4 - vector2) > 0f)
            {
                vector4 = vector2;
                currentVelocity = (vector4 - vector2) / deltaTime;
            }
            return vector4;
        }

        // Token: 0x06005151 RID: 20817 RVA: 0x0008CDEC File Offset: 0x0008AFEC
        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x + b.x, a.y + b.y);
        }

        // Token: 0x06005152 RID: 20818 RVA: 0x0008CE24 File Offset: 0x0008B024
        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x - b.x, a.y - b.y);
        }

        // Token: 0x06005153 RID: 20819 RVA: 0x0008CE5C File Offset: 0x0008B05C
        public static Vector2 operator *(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x * b.x, a.y * b.y);
        }

        // Token: 0x06005154 RID: 20820 RVA: 0x0008CE94 File Offset: 0x0008B094
        public static Vector2 operator /(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x / b.x, a.y / b.y);
        }

        // Token: 0x06005155 RID: 20821 RVA: 0x0008CECC File Offset: 0x0008B0CC
        public static Vector2 operator -(Vector2 a)
        {
            return new Vector2(-a.x, -a.y);
        }

        // Token: 0x06005156 RID: 20822 RVA: 0x0008CEF8 File Offset: 0x0008B0F8
        public static Vector2 operator *(Vector2 a, float d)
        {
            return new Vector2(a.x * d, a.y * d);
        }

        // Token: 0x06005157 RID: 20823 RVA: 0x0008CF24 File Offset: 0x0008B124
        public static Vector2 operator *(float d, Vector2 a)
        {
            return new Vector2(a.x * d, a.y * d);
        }

        // Token: 0x06005158 RID: 20824 RVA: 0x0008CF50 File Offset: 0x0008B150
        public static Vector2 operator /(Vector2 a, float d)
        {
            return new Vector2(a.x / d, a.y / d);
        }

        // Token: 0x06005159 RID: 20825 RVA: 0x0008CF7C File Offset: 0x0008B17C
        public static bool operator ==(Vector2 lhs, Vector2 rhs)
        {
            return (lhs - rhs).sqrMagnitude < 9.9999994E-11f;
        }

        // Token: 0x0600515A RID: 20826 RVA: 0x0008CFA8 File Offset: 0x0008B1A8
        public static bool operator !=(Vector2 lhs, Vector2 rhs)
        {
            return !(lhs == rhs);
        }

        // Token: 0x0600515B RID: 20827 RVA: 0x0008CFC8 File Offset: 0x0008B1C8
        public static implicit operator Vector2(Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        // Token: 0x0600515C RID: 20828 RVA: 0x0008CFF0 File Offset: 0x0008B1F0
        public static implicit operator Vector3(Vector2 v)
        {
            return new Vector3(v.x, v.y, 0f);
        }

        // Token: 0x17001297 RID: 4759
        // (get) Token: 0x0600515D RID: 20829 RVA: 0x0008D020 File Offset: 0x0008B220
        public static Vector2 zero
        {
            get
            {
                return Vector2.zeroVector;
            }
        }

        // Token: 0x17001298 RID: 4760
        // (get) Token: 0x0600515E RID: 20830 RVA: 0x0008D03C File Offset: 0x0008B23C
        public static Vector2 one
        {
            get
            {
                return Vector2.oneVector;
            }
        }

        // Token: 0x17001299 RID: 4761
        // (get) Token: 0x0600515F RID: 20831 RVA: 0x0008D058 File Offset: 0x0008B258
        public static Vector2 up
        {
            get
            {
                return Vector2.upVector;
            }
        }

        // Token: 0x1700129A RID: 4762
        // (get) Token: 0x06005160 RID: 20832 RVA: 0x0008D074 File Offset: 0x0008B274
        public static Vector2 down
        {
            get
            {
                return Vector2.downVector;
            }
        }

        // Token: 0x1700129B RID: 4763
        // (get) Token: 0x06005161 RID: 20833 RVA: 0x0008D090 File Offset: 0x0008B290
        public static Vector2 left
        {
            get
            {
                return Vector2.leftVector;
            }
        }

        // Token: 0x1700129C RID: 4764
        // (get) Token: 0x06005162 RID: 20834 RVA: 0x0008D0AC File Offset: 0x0008B2AC
        public static Vector2 right
        {
            get
            {
                return Vector2.rightVector;
            }
        }

        // Token: 0x1700129D RID: 4765
        // (get) Token: 0x06005163 RID: 20835 RVA: 0x0008D0C8 File Offset: 0x0008B2C8
        public static Vector2 positiveInfinity
        {
            get
            {
                return Vector2.positiveInfinityVector;
            }
        }

        // Token: 0x1700129E RID: 4766
        // (get) Token: 0x06005164 RID: 20836 RVA: 0x0008D0E4 File Offset: 0x0008B2E4
        public static Vector2 negativeInfinity
        {
            get
            {
                return Vector2.negativeInfinityVector;
            }
        }

        // Token: 0x04001A5D RID: 6749
        public float x;

        // Token: 0x04001A5E RID: 6750
        public float y;

        // Token: 0x04001A5F RID: 6751
        private static readonly Vector2 zeroVector = new Vector2(0f, 0f);

        // Token: 0x04001A60 RID: 6752
        private static readonly Vector2 oneVector = new Vector2(1f, 1f);

        // Token: 0x04001A61 RID: 6753
        private static readonly Vector2 upVector = new Vector2(0f, 1f);

        // Token: 0x04001A62 RID: 6754
        private static readonly Vector2 downVector = new Vector2(0f, -1f);

        // Token: 0x04001A63 RID: 6755
        private static readonly Vector2 leftVector = new Vector2(-1f, 0f);

        // Token: 0x04001A64 RID: 6756
        private static readonly Vector2 rightVector = new Vector2(1f, 0f);

        // Token: 0x04001A65 RID: 6757
        private static readonly Vector2 positiveInfinityVector = new Vector2(float.PositiveInfinity, float.PositiveInfinity);

        // Token: 0x04001A66 RID: 6758
        private static readonly Vector2 negativeInfinityVector = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

        // Token: 0x04001A67 RID: 6759
        public const float kEpsilon = 1E-05f;

        // Token: 0x04001A68 RID: 6760
        public const float kEpsilonNormalSqrt = 1E-15f;
        #endregion

        #region 网上代码

        public float Length()
        {
            return (float)Math.Sqrt(x * (double)x + y * (double)y);
        }

        public float LengthSquared()
        {
            return (float)(x * (double)x + y * (double)y);
        }

        public static void Distance(ref Vector2 value1, ref Vector2 value2, out float result)
        {
            float num1 = value1.x - value2.x;
            float num2 = value1.y - value2.y;
            float num3 = (float)(num1 * (double)num1 + num2 * (double)num2);
            result = (float)Math.Sqrt(num3);
        }

        public static float DistanceSquared(Vector2 value1, Vector2 value2)
        {
            float num1 = value1.x - value2.x;
            float num2 = value1.y - value2.y;
            return (float)(num1 * (double)num1 + num2 * (double)num2);
        }

        public static void DistanceSquared(ref Vector2 value1, ref Vector2 value2, out float result)
        {
            float num1 = value1.x - value2.x;
            float num2 = value1.y - value2.y;
            result = (float)(num1 * (double)num1 + num2 * (double)num2);
        }

        public static Vector2 Normalize(Vector2 value)
        {
            float num1 = (float)(value.x * (double)value.x + value.y * (double)value.y);
            if (num1 < 9.99999974737875E-06)
                return value;
            float num2 = 1f / (float)Math.Sqrt(num1);
            Vector2 vector2;
            vector2.x = value.x * num2;
            vector2.y = value.y * num2;
            return vector2;
        }

        public static void Normalize(ref Vector2 value, out Vector2 result)
        {
            float num1 = (float)(value.x * (double)value.x + value.y * (double)value.y);
            if (num1 < 9.99999974737875E-06)
            {
                result = value;
            }
            else
            {
                float num2 = 1f / (float)Math.Sqrt(num1);
                result.x = value.x * num2;
                result.y = value.y * num2;
            }
        }

        public static void Reflect(ref Vector2 vector, ref Vector2 normal, out Vector2 result)
        {
            float num = (float)(vector.x * (double)normal.x + vector.y * (double)normal.y);
            result.x = vector.x - 2f * num * normal.x;
            result.y = vector.y - 2f * num * normal.y;
        }

        public static void Min(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result.x = value1.x < (double)value2.x ? value1.x : value2.x;
            result.y = value1.y < (double)value2.y ? value1.y : value2.y;
        }

        public static void Max(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result.x = value1.x > (double)value2.x ? value1.x : value2.x;
            result.y = value1.y > (double)value2.y ? value1.y : value2.y;
        }

        public static Vector2 Clamp(Vector2 value1, Vector2 min, Vector2 max)
        {
            float x = value1.x;
            float num1 = x > (double)max.x ? max.x : x;
            float num2 = num1 < (double)min.x ? min.x : num1;
            float y = value1.y;
            float num3 = y > (double)max.y ? max.y : y;
            float num4 = num3 < (double)min.y ? min.y : num3;
            Vector2 vector2;
            vector2.x = num2;
            vector2.y = num4;
            return vector2;
        }

        public static void Clamp(ref Vector2 value1, ref Vector2 min, ref Vector2 max, out Vector2 result)
        {
            float x = value1.x;
            float num1 = x > (double)max.x ? max.x : x;
            float num2 = num1 < (double)min.x ? min.x : num1;
            float y = value1.y;
            float num3 = y > (double)max.y ? max.y : y;
            float num4 = num3 < (double)min.y ? min.y : num3;
            result.x = num2;
            result.y = num4;
        }

        public static void Lerp(ref Vector2 value1, ref Vector2 value2, float amount, out Vector2 result)
        {
            result.x = value1.x + (value2.x - value1.x) * amount;
            result.y = value1.y + (value2.y - value1.y) * amount;
        }

        public static Vector2 SmoothStep(Vector2 value1, Vector2 value2, float amount)
        {
            amount = amount > 1.0 ? 1f : (amount < 0.0 ? 0.0f : amount);
            amount = (float)(amount * (double)amount * (3.0 - 2.0 * amount));
            Vector2 vector2;
            vector2.x = value1.x + (value2.x - value1.x) * amount;
            vector2.y = value1.y + (value2.y - value1.y) * amount;
            return vector2;
        }

        public static void SmoothStep(ref Vector2 value1, ref Vector2 value2, float amount, out Vector2 result)
        {
            amount = amount > 1.0 ? 1f : (amount < 0.0 ? 0.0f : amount);
            amount = (float)(amount * (double)amount * (3.0 - 2.0 * amount));
            result.x = value1.x + (value2.x - value1.x) * amount;
            result.y = value1.y + (value2.y - value1.y) * amount;
        }

        public static Vector2 Negate(Vector2 value)
        {
            Vector2 vector2;
            vector2.x = -value.x;
            vector2.y = -value.y;
            return vector2;
        }

        public static void Negate(ref Vector2 value, out Vector2 result)
        {
            result.x = -value.x;
            result.y = -value.y;
        }

        public static void Dot(ref Vector2 value1, ref Vector2 value2, out float result)
        {
            result = (float)(value1.x * (double)value2.x + value1.y * (double)value2.y);
        }


        public static void Angle(ref Vector2 from, ref Vector2 to, out float result)
        {
            from.Normalize();
            to.Normalize();
            Vector2.Dot(ref from, ref to, out float result1);
            result = Mathf.Cos(Mathf.Clamp(result1, -1f, 1f)) * 57.29578f;
        }

        public static Vector2 Add(Vector2 value1, Vector2 value2)
        {
            Vector2 vector2;
            vector2.x = value1.x + value2.x;
            vector2.y = value1.y + value2.y;
            return vector2;
        }

        public static void Add(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result.x = value1.x + value2.x;
            result.y = value1.y + value2.y;
        }

        public static Vector2 Sub(Vector2 value1, Vector2 value2)
        {
            Vector2 vector2;
            vector2.x = value1.x - value2.x;
            vector2.y = value1.y - value2.y;
            return vector2;
        }

        public static void Sub(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result.x = value1.x - value2.x;
            result.y = value1.y - value2.y;
        }

        public static Vector2 Multiply(Vector2 value1, Vector2 value2)
        {
            Vector2 vector2;
            vector2.x = value1.x * value2.x;
            vector2.y = value1.y * value2.y;
            return vector2;
        }

        public static void Multiply(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result.x = value1.x * value2.x;
            result.y = value1.y * value2.y;
        }

        public static Vector2 Multiply(Vector2 value1, float scaleFactor)
        {
            Vector2 vector2;
            vector2.x = value1.x * scaleFactor;
            vector2.y = value1.y * scaleFactor;
            return vector2;
        }

        public static void Multiply(ref Vector2 value1, float scaleFactor, out Vector2 result)
        {
            result.x = value1.x * scaleFactor;
            result.y = value1.y * scaleFactor;
        }

        public static Vector2 Divide(Vector2 value1, Vector2 value2)
        {
            Vector2 vector2;
            vector2.x = value1.x / value2.x;
            vector2.y = value1.y / value2.y;
            return vector2;
        }

        public static void Divide(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result.x = value1.x / value2.x;
            result.y = value1.y / value2.y;
        }

        public static Vector2 Divide(Vector2 value1, float divider)
        {
            float num = 1f / divider;
            Vector2 vector2;
            vector2.x = value1.x * num;
            vector2.y = value1.y * num;
            return vector2;
        }

        public static void Divide(ref Vector2 value1, float divider, out Vector2 result)
        {
            float num = 1f / divider;
            result.x = value1.x * num;
            result.y = value1.y * num;
        }
        #endregion

        // Token: 0x06005151 RID: 20817 RVA: 0x0008CDEC File Offset: 0x0008AFEC
        public static UnityEngine.Vector2 operator +(UnityEngine.Vector2 a, Vector2 b)
        {
            return new UnityEngine.Vector2(a.x + b.x, a.y + b.y);
        }

        // Token: 0x06005152 RID: 20818 RVA: 0x0008CE24 File Offset: 0x0008B024
        public static UnityEngine.Vector2 operator -(UnityEngine.Vector2 a, Vector2 b)
        {
            return new UnityEngine.Vector2(a.x - b.x, a.y - b.y);
        }

        // Token: 0x06005153 RID: 20819 RVA: 0x0008CE5C File Offset: 0x0008B05C
        public static UnityEngine.Vector2 operator *(UnityEngine.Vector2 a, Vector2 b)
        {
            return new UnityEngine.Vector2(a.x * b.x, a.y * b.y);
        }

        // Token: 0x06005154 RID: 20820 RVA: 0x0008CE94 File Offset: 0x0008B094
        public static UnityEngine.Vector2 operator /(UnityEngine.Vector2 a, Vector2 b)
        {
            return new UnityEngine.Vector2(a.x / b.x, a.y / b.y);
        }


        public static Vector2 operator +(Vector2 a, UnityEngine.Vector2 b)
        {
            return new Vector2(a.x + b.x, a.y + b.y);
        }

        // Token: 0x06005152 RID: 20818 RVA: 0x0008CE24 File Offset: 0x0008B024
        public static Vector2 operator -(Vector2 a, UnityEngine.Vector2 b)
        {
            return new Vector2(a.x - b.x, a.y - b.y);
        }

        // Token: 0x06005153 RID: 20819 RVA: 0x0008CE5C File Offset: 0x0008B05C
        public static Vector2 operator *(Vector2 a, UnityEngine.Vector2 b)
        {
            return new Vector2(a.x * b.x, a.y * b.y);
        }

        // Token: 0x06005154 RID: 20820 RVA: 0x0008CE94 File Offset: 0x0008B094
        public static Vector2 operator /(Vector2 a, UnityEngine.Vector2 b)
        {
            return new Vector2(a.x / b.x, a.y / b.y);
        }



        // Token: 0x06005159 RID: 20825 RVA: 0x0008CF7C File Offset: 0x0008B17C
        public static bool operator ==(UnityEngine.Vector2 lhs, Vector2 rhs)
        {
            return (lhs - rhs).sqrMagnitude < 9.9999994E-11f;
        }

        // Token: 0x0600515A RID: 20826 RVA: 0x0008CFA8 File Offset: 0x0008B1A8
        public static bool operator !=(UnityEngine.Vector2 lhs, Vector2 rhs)
        {
            return !(lhs == rhs);
        }

        public static implicit operator UnityEngine.Vector2(Vector2 v)
        {
            return new UnityEngine.Vector2(v.x, v.y);
        }

        public static implicit operator Vector2(UnityEngine.Vector2 v)
        {
            return new Vector2(v.x, v.y);
        }
    }
}