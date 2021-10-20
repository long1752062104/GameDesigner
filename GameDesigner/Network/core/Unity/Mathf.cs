using global::System;

namespace Net
{
    public struct Mathf
    {
        #region 源码
        // Token: 0x0600496C RID: 18796 RVA: 0x0007F09C File Offset: 0x0007D29C
        public static float Sin(float f)
        {
            return (float)Math.Sin(f);
        }

        // Token: 0x0600496D RID: 18797 RVA: 0x0007F0BC File Offset: 0x0007D2BC
        public static float Cos(float f)
        {
            return (float)Math.Cos(f);
        }

        // Token: 0x0600496E RID: 18798 RVA: 0x0007F0DC File Offset: 0x0007D2DC
        public static float Tan(float f)
        {
            return (float)Math.Tan(f);
        }

        // Token: 0x0600496F RID: 18799 RVA: 0x0007F0FC File Offset: 0x0007D2FC
        public static float Asin(float f)
        {
            return (float)Math.Asin(f);
        }

        // Token: 0x06004970 RID: 18800 RVA: 0x0007F11C File Offset: 0x0007D31C
        public static float Acos(float f)
        {
            return (float)Math.Acos(f);
        }

        // Token: 0x06004971 RID: 18801 RVA: 0x0007F13C File Offset: 0x0007D33C
        public static float Atan(float f)
        {
            return (float)Math.Atan(f);
        }

        // Token: 0x06004972 RID: 18802 RVA: 0x0007F15C File Offset: 0x0007D35C
        public static float Atan2(float y, float x)
        {
            return (float)Math.Atan2(y, x);
        }

        // Token: 0x06004973 RID: 18803 RVA: 0x0007F17C File Offset: 0x0007D37C
        public static float Sqrt(float f)
        {
            return (float)Math.Sqrt(f);
        }

        // Token: 0x06004974 RID: 18804 RVA: 0x0007F19C File Offset: 0x0007D39C
        public static float Abs(float f)
        {
            return Math.Abs(f);
        }

        // Token: 0x06004975 RID: 18805 RVA: 0x0007F1B8 File Offset: 0x0007D3B8
        public static int Abs(int value)
        {
            return Math.Abs(value);
        }

        // Token: 0x06004976 RID: 18806 RVA: 0x0007F1D4 File Offset: 0x0007D3D4
        public static float Min(float a, float b)
        {
            return (a >= b) ? b : a;
        }

        // Token: 0x06004977 RID: 18807 RVA: 0x0007F1F8 File Offset: 0x0007D3F8
        public static float Min(params float[] values)
        {
            int num = values.Length;
            float result;
            if (num == 0)
            {
                result = 0f;
            }
            else
            {
                float num2 = values[0];
                for (int i = 1; i < num; i++)
                {
                    if (values[i] < num2)
                    {
                        num2 = values[i];
                    }
                }
                result = num2;
            }
            return result;
        }

        // Token: 0x06004978 RID: 18808 RVA: 0x0007F248 File Offset: 0x0007D448
        public static int Min(int a, int b)
        {
            return (a >= b) ? b : a;
        }

        // Token: 0x06004979 RID: 18809 RVA: 0x0007F26C File Offset: 0x0007D46C
        public static int Min(params int[] values)
        {
            int num = values.Length;
            int result;
            if (num == 0)
            {
                result = 0;
            }
            else
            {
                int num2 = values[0];
                for (int i = 1; i < num; i++)
                {
                    if (values[i] < num2)
                    {
                        num2 = values[i];
                    }
                }
                result = num2;
            }
            return result;
        }

        // Token: 0x0600497A RID: 18810 RVA: 0x0007F2B8 File Offset: 0x0007D4B8
        public static float Max(float a, float b)
        {
            return (a <= b) ? b : a;
        }

        // Token: 0x0600497B RID: 18811 RVA: 0x0007F2DC File Offset: 0x0007D4DC
        public static float Max(params float[] values)
        {
            int num = values.Length;
            float result;
            if (num == 0)
            {
                result = 0f;
            }
            else
            {
                float num2 = values[0];
                for (int i = 1; i < num; i++)
                {
                    if (values[i] > num2)
                    {
                        num2 = values[i];
                    }
                }
                result = num2;
            }
            return result;
        }

        // Token: 0x0600497C RID: 18812 RVA: 0x0007F32C File Offset: 0x0007D52C
        public static int Max(int a, int b)
        {
            return (a <= b) ? b : a;
        }

        // Token: 0x0600497D RID: 18813 RVA: 0x0007F350 File Offset: 0x0007D550
        public static int Max(params int[] values)
        {
            int num = values.Length;
            int result;
            if (num == 0)
            {
                result = 0;
            }
            else
            {
                int num2 = values[0];
                for (int i = 1; i < num; i++)
                {
                    if (values[i] > num2)
                    {
                        num2 = values[i];
                    }
                }
                result = num2;
            }
            return result;
        }

        // Token: 0x0600497E RID: 18814 RVA: 0x0007F39C File Offset: 0x0007D59C
        public static float Pow(float f, float p)
        {
            return (float)Math.Pow(f, p);
        }

        // Token: 0x0600497F RID: 18815 RVA: 0x0007F3BC File Offset: 0x0007D5BC
        public static float Exp(float power)
        {
            return (float)Math.Exp(power);
        }

        // Token: 0x06004980 RID: 18816 RVA: 0x0007F3DC File Offset: 0x0007D5DC
        public static float Log(float f, float p)
        {
            return (float)Math.Log(f, p);
        }

        // Token: 0x06004981 RID: 18817 RVA: 0x0007F3FC File Offset: 0x0007D5FC
        public static float Log(float f)
        {
            return (float)Math.Log(f);
        }

        // Token: 0x06004982 RID: 18818 RVA: 0x0007F41C File Offset: 0x0007D61C
        public static float Log10(float f)
        {
            return (float)Math.Log10(f);
        }

        // Token: 0x06004983 RID: 18819 RVA: 0x0007F43C File Offset: 0x0007D63C
        public static float Ceil(float f)
        {
            return (float)Math.Ceiling(f);
        }

        // Token: 0x06004984 RID: 18820 RVA: 0x0007F45C File Offset: 0x0007D65C
        public static float Floor(float f)
        {
            return (float)Math.Floor(f);
        }

        // Token: 0x06004985 RID: 18821 RVA: 0x0007F47C File Offset: 0x0007D67C
        public static float Round(float f)
        {
            return (float)Math.Round(f);
        }

        // Token: 0x06004986 RID: 18822 RVA: 0x0007F49C File Offset: 0x0007D69C
        public static int CeilToInt(float f)
        {
            return (int)Math.Ceiling(f);
        }

        // Token: 0x06004987 RID: 18823 RVA: 0x0007F4BC File Offset: 0x0007D6BC
        public static int FloorToInt(float f)
        {
            return (int)Math.Floor(f);
        }

        // Token: 0x06004988 RID: 18824 RVA: 0x0007F4DC File Offset: 0x0007D6DC
        public static int RoundToInt(float f)
        {
            return (int)Math.Round(f);
        }

        // Token: 0x06004989 RID: 18825 RVA: 0x0007F4FC File Offset: 0x0007D6FC
        public static float Sign(float f)
        {
            return (f < 0f) ? -1f : 1f;
        }

        // Token: 0x0600498A RID: 18826 RVA: 0x0007F52C File Offset: 0x0007D72C
        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }
            return value;
        }

        // Token: 0x0600498B RID: 18827 RVA: 0x0007F55C File Offset: 0x0007D75C
        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }
            return value;
        }

        // Token: 0x0600498C RID: 18828 RVA: 0x0007F58C File Offset: 0x0007D78C
        public static float Clamp01(float value)
        {
            float result;
            if (value < 0f)
            {
                result = 0f;
            }
            else if (value > 1f)
            {
                result = 1f;
            }
            else
            {
                result = value;
            }
            return result;
        }

        // Token: 0x0600498D RID: 18829 RVA: 0x0007F5D0 File Offset: 0x0007D7D0
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * Mathf.Clamp01(t);
        }

        // Token: 0x0600498E RID: 18830 RVA: 0x0007F5F4 File Offset: 0x0007D7F4
        public static float LerpUnclamped(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        // Token: 0x0600498F RID: 18831 RVA: 0x0007F610 File Offset: 0x0007D810
        public static float LerpAngle(float a, float b, float t)
        {
            float num = Mathf.Repeat(b - a, 360f);
            if (num > 180f)
            {
                num -= 360f;
            }
            return a + num * Mathf.Clamp01(t);
        }

        // Token: 0x06004990 RID: 18832 RVA: 0x0007F650 File Offset: 0x0007D850
        public static float MoveTowards(float current, float target, float maxDelta)
        {
            float result;
            if (Mathf.Abs(target - current) <= maxDelta)
            {
                result = target;
            }
            else
            {
                result = current + Mathf.Sign(target - current) * maxDelta;
            }
            return result;
        }

        // Token: 0x06004991 RID: 18833 RVA: 0x0007F688 File Offset: 0x0007D888
        public static float MoveTowardsAngle(float current, float target, float maxDelta)
        {
            float num = Mathf.DeltaAngle(current, target);
            float result;
            if (-maxDelta < num && num < maxDelta)
            {
                result = target;
            }
            else
            {
                target = current + num;
                result = Mathf.MoveTowards(current, target, maxDelta);
            }
            return result;
        }

        // Token: 0x06004992 RID: 18834 RVA: 0x0007F6C8 File Offset: 0x0007D8C8
        public static float SmoothStep(float from, float to, float t)
        {
            t = Mathf.Clamp01(t);
            t = -2f * t * t * t + 3f * t * t;
            return to * t + from * (1f - t);
        }

        // Token: 0x06004993 RID: 18835 RVA: 0x0007F70C File Offset: 0x0007D90C
        public static float Gamma(float value, float absmax, float gamma)
        {
            bool flag = false;
            if (value < 0f)
            {
                flag = true;
            }
            float num = Mathf.Abs(value);
            float result;
            if (num > absmax)
            {
                result = ((!flag) ? num : (-num));
            }
            else
            {
                float num2 = Mathf.Pow(num / absmax, gamma) * absmax;
                result = ((!flag) ? num2 : (-num2));
            }
            return result;
        }

        // Token: 0x06004995 RID: 18837 RVA: 0x0007F7B4 File Offset: 0x0007D9B4

        // Token: 0x06004997 RID: 18839 RVA: 0x0007F808 File Offset: 0x0007DA08
        public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            smoothTime = Mathf.Max(0.0001f, smoothTime);
            float num = 2f / smoothTime;
            float num2 = num * deltaTime;
            float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
            float num4 = current - target;
            float num5 = target;
            float num6 = maxSpeed * smoothTime;
            num4 = Mathf.Clamp(num4, -num6, num6);
            target = current - num4;
            float num7 = (currentVelocity + num * num4) * deltaTime;
            currentVelocity = (currentVelocity - num * num7) * num3;
            float num8 = target + (num4 + num7) * num3;
            if (num5 - current > 0f == num8 > num5)
            {
                num8 = num5;
                currentVelocity = (num8 - num5) / deltaTime;
            }
            return num8;
        }

        // Token: 0x0600499A RID: 18842 RVA: 0x0007F918 File Offset: 0x0007DB18
        public static float SmoothDampAngle(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            target = current + Mathf.DeltaAngle(current, target);
            return Mathf.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        // Token: 0x0600499B RID: 18843 RVA: 0x0007F948 File Offset: 0x0007DB48
        public static float Repeat(float t, float length)
        {
            return Mathf.Clamp(t - Mathf.Floor(t / length) * length, 0f, length);
        }

        // Token: 0x0600499C RID: 18844 RVA: 0x0007F974 File Offset: 0x0007DB74
        public static float PingPong(float t, float length)
        {
            t = Mathf.Repeat(t, length * 2f);
            return length - Mathf.Abs(t - length);
        }

        // Token: 0x0600499D RID: 18845 RVA: 0x0007F9A4 File Offset: 0x0007DBA4
        public static float InverseLerp(float a, float b, float value)
        {
            float result;
            if (a != b)
            {
                result = Mathf.Clamp01((value - a) / (b - a));
            }
            else
            {
                result = 0f;
            }
            return result;
        }

        // Token: 0x0600499E RID: 18846 RVA: 0x0007F9D8 File Offset: 0x0007DBD8
        public static float DeltaAngle(float current, float target)
        {
            float num = Mathf.Repeat(target - current, 360f);
            if (num > 180f)
            {
                num -= 360f;
            }
            return num;
        }

        // Token: 0x0600499F RID: 18847 RVA: 0x0007FA10 File Offset: 0x0007DC10
        internal static bool LineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 result)
        {
            float num = p2.x - p1.x;
            float num2 = p2.y - p1.y;
            float num3 = p4.x - p3.x;
            float num4 = p4.y - p3.y;
            float num5 = num * num4 - num2 * num3;
            bool result2;
            if (num5 == 0f)
            {
                result2 = false;
            }
            else
            {
                float num6 = p3.x - p1.x;
                float num7 = p3.y - p1.y;
                float num8 = (num6 * num4 - num7 * num3) / num5;
                result = new Vector2(p1.x + num8 * num, p1.y + num8 * num2);
                result2 = true;
            }
            return result2;
        }

        // Token: 0x060049A0 RID: 18848 RVA: 0x0007FAD8 File Offset: 0x0007DCD8
        internal static bool LineSegmentIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 result)
        {
            float num = p2.x - p1.x;
            float num2 = p2.y - p1.y;
            float num3 = p4.x - p3.x;
            float num4 = p4.y - p3.y;
            float num5 = num * num4 - num2 * num3;
            bool result2;
            if (num5 == 0f)
            {
                result2 = false;
            }
            else
            {
                float num6 = p3.x - p1.x;
                float num7 = p3.y - p1.y;
                float num8 = (num6 * num4 - num7 * num3) / num5;
                if (num8 < 0f || num8 > 1f)
                {
                    result2 = false;
                }
                else
                {
                    float num9 = (num6 * num2 - num7 * num) / num5;
                    if (num9 < 0f || num9 > 1f)
                    {
                        result2 = false;
                    }
                    else
                    {
                        result = new Vector2(p1.x + num8 * num, p1.y + num8 * num2);
                        result2 = true;
                    }
                }
            }
            return result2;
        }

        // Token: 0x060049A1 RID: 18849 RVA: 0x0007FBF0 File Offset: 0x0007DDF0
        internal static long RandomToLong(Random r)
        {
            byte[] array = new byte[8];
            r.NextBytes(array);
            return (long)(BitConverter.ToUInt64(array, 0) & 9223372036854775807UL);
        }

        // Token: 0x0400192B RID: 6443
        public const float PI = 3.1415927f;

        // Token: 0x0400192C RID: 6444
        public const float Infinity = float.PositiveInfinity;

        // Token: 0x0400192D RID: 6445
        public const float NegativeInfinity = float.NegativeInfinity;

        // Token: 0x0400192E RID: 6446
        public const float _Deg2Rad = 0.017453292f;

        // Token: 0x0400192F RID: 6447
        public const float _Rad2Deg = 57.29578f;
        #endregion

        #region 网上代码
        public const float Epsilon = 0.00001F;

        public static float Rad2Deg(float radians)
        {
            return (float)(radians * 180 / Math.PI);
        }

        public static float Deg2Rad(float degrees)
        {
            return (float)(degrees * Math.PI / 180);
        }

        public static Vector3 Rad2Deg(Vector3 radians)
        {
            return new Vector3(
                               (float)(radians.x * 180 / Math.PI),
                               (float)(radians.y * 180 / Math.PI),
                               (float)(radians.z * 180 / Math.PI));
        }
        public static Vector3 Deg2Rad(Vector3 degrees)
        {
            return new Vector3(
                               (float)(degrees.x * Math.PI / 180),
                               (float)(degrees.y * Math.PI / 180),
                               (float)(degrees.z * Math.PI / 180));
        }

        public const float CosAngle20 = 0.9396926208f;
        public const float CompareEpsilon = 0.000001f;

        public static bool CompareApproximate(float f0, float f1, float epsilon = CompareEpsilon)
        {
            return Math.Abs(f0 - f1) < epsilon;
        }
        public static bool CompareApproximate(double f0, double f1, float epsilon = CompareEpsilon)
        {
            return Math.Abs(f0 - f1) < epsilon;
        }
        #endregion
    }
}