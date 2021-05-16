using System;
using UnityEngine;

namespace Net
{
    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
    [Serializable]
    public struct Color : IEquatable<Color>
    {
        #region code
        // Token: 0x06003EF1 RID: 16113 RVA: 0x00071313 File Offset: 0x0006F513
        public Color(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
            hex = "";
        }

        // Token: 0x06003EF2 RID: 16114 RVA: 0x00071333 File Offset: 0x0006F533
        public Color(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            a = 1f;
            hex = "";
        }

        // Token: 0x06003EF3 RID: 16115 RVA: 0x00071358 File Offset: 0x0006F558
        public override string ToString()
        {
            return string.Format("RGBA({0:F3}, {1:F3}, {2:F3}, {3:F3})", new object[]
            {
                r,
                g,
                b,
                a
            });
        }

        // Token: 0x06003EF4 RID: 16116 RVA: 0x000713B8 File Offset: 0x0006F5B8
        public string ToString(string format)
        {
            return string.Format("RGBA({0}, {1}, {2}, {3})", new object[]
            {
                r.ToString(format),
                g.ToString(format),
                b.ToString(format),
                a.ToString(format)
            });
        }

        // Token: 0x06003EF5 RID: 16117 RVA: 0x0007141C File Offset: 0x0006F61C
        public override int GetHashCode()
        {
            return GetHashCode();
        }

        // Token: 0x06003EF6 RID: 16118 RVA: 0x0007144C File Offset: 0x0006F64C
        public override bool Equals(object other)
        {
            return other is Color && Equals((Color)other);
        }

        // Token: 0x06003EF7 RID: 16119 RVA: 0x00071480 File Offset: 0x0006F680
        public bool Equals(Color other)
        {
            return r.Equals(other.r) && g.Equals(other.g) && b.Equals(other.b) && a.Equals(other.a);
        }

        // Token: 0x06003EF8 RID: 16120 RVA: 0x000714F0 File Offset: 0x0006F6F0
        public static Color operator +(Color a, Color b)
        {
            return new Color(a.r + b.r, a.g + b.g, a.b + b.b, a.a + b.a);
        }

        // Token: 0x06003EF9 RID: 16121 RVA: 0x00071548 File Offset: 0x0006F748
        public static Color operator -(Color a, Color b)
        {
            return new Color(a.r - b.r, a.g - b.g, a.b - b.b, a.a - b.a);
        }

        // Token: 0x06003EFA RID: 16122 RVA: 0x000715A0 File Offset: 0x0006F7A0
        public static Color operator *(Color a, Color b)
        {
            return new Color(a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a);
        }

        // Token: 0x06003EFB RID: 16123 RVA: 0x000715F8 File Offset: 0x0006F7F8
        public static Color operator *(Color a, float b)
        {
            return new Color(a.r * b, a.g * b, a.b * b, a.a * b);
        }

        // Token: 0x06003EFC RID: 16124 RVA: 0x00071638 File Offset: 0x0006F838
        public static Color operator *(float b, Color a)
        {
            return new Color(a.r * b, a.g * b, a.b * b, a.a * b);
        }

        // Token: 0x06003EFD RID: 16125 RVA: 0x00071678 File Offset: 0x0006F878
        public static Color operator /(Color a, float b)
        {
            return new Color(a.r / b, a.g / b, a.b / b, a.a / b);
        }

        // Token: 0x06003EFE RID: 16126 RVA: 0x000716B8 File Offset: 0x0006F8B8
        public static bool operator ==(Color lhs, Color rhs)
        {
            return lhs == rhs;
        }

        // Token: 0x06003EFF RID: 16127 RVA: 0x000716E0 File Offset: 0x0006F8E0
        public static bool operator !=(Color lhs, Color rhs)
        {
            return !(lhs == rhs);
        }

        // Token: 0x06003F00 RID: 16128 RVA: 0x00071700 File Offset: 0x0006F900
        public static Color Lerp(Color a, Color b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Color(a.r + (b.r - a.r) * t, a.g + (b.g - a.g) * t, a.b + (b.b - a.b) * t, a.a + (b.a - a.a) * t);
        }

        // Token: 0x06003F01 RID: 16129 RVA: 0x00071788 File Offset: 0x0006F988
        public static Color LerpUnclamped(Color a, Color b, float t)
        {
            return new Color(a.r + (b.r - a.r) * t, a.g + (b.g - a.g) * t, a.b + (b.b - a.b) * t, a.a + (b.a - a.a) * t);
        }

        // Token: 0x06003F02 RID: 16130 RVA: 0x00071808 File Offset: 0x0006FA08
        internal Color RGBMultiplied(float multiplier)
        {
            return new Color(r * multiplier, g * multiplier, b * multiplier, a);
        }

        // Token: 0x06003F03 RID: 16131 RVA: 0x00071840 File Offset: 0x0006FA40
        internal Color AlphaMultiplied(float multiplier)
        {
            return new Color(r, g, b, a * multiplier);
        }

        // Token: 0x06003F04 RID: 16132 RVA: 0x00071874 File Offset: 0x0006FA74
        internal Color RGBMultiplied(Color multiplier)
        {
            return new Color(r * multiplier.r, g * multiplier.g, b * multiplier.b, a);
        }

        // Token: 0x17000ED5 RID: 3797
        // (get) Token: 0x06003F05 RID: 16133 RVA: 0x000718C0 File Offset: 0x0006FAC0
        public static Color red
        {
            get
            {
                return new Color(1f, 0f, 0f, 1f);
            }
        }

        // Token: 0x17000ED6 RID: 3798
        // (get) Token: 0x06003F06 RID: 16134 RVA: 0x000718F0 File Offset: 0x0006FAF0
        public static Color green
        {
            get
            {
                return new Color(0f, 1f, 0f, 1f);
            }
        }

        // Token: 0x17000ED7 RID: 3799
        // (get) Token: 0x06003F07 RID: 16135 RVA: 0x00071920 File Offset: 0x0006FB20
        public static Color blue
        {
            get
            {
                return new Color(0f, 0f, 1f, 1f);
            }
        }

        // Token: 0x17000ED8 RID: 3800
        // (get) Token: 0x06003F08 RID: 16136 RVA: 0x00071950 File Offset: 0x0006FB50
        public static Color white
        {
            get
            {
                return new Color(1f, 1f, 1f, 1f);
            }
        }

        // Token: 0x17000ED9 RID: 3801
        // (get) Token: 0x06003F09 RID: 16137 RVA: 0x00071980 File Offset: 0x0006FB80
        public static Color black
        {
            get
            {
                return new Color(0f, 0f, 0f, 1f);
            }
        }

        // Token: 0x17000EDA RID: 3802
        // (get) Token: 0x06003F0A RID: 16138 RVA: 0x000719B0 File Offset: 0x0006FBB0
        public static Color yellow
        {
            get
            {
                return new Color(1f, 0.92156863f, 0.015686275f, 1f);
            }
        }

        // Token: 0x17000EDB RID: 3803
        // (get) Token: 0x06003F0B RID: 16139 RVA: 0x000719E0 File Offset: 0x0006FBE0
        public static Color cyan
        {
            get
            {
                return new Color(0f, 1f, 1f, 1f);
            }
        }

        // Token: 0x17000EDC RID: 3804
        // (get) Token: 0x06003F0C RID: 16140 RVA: 0x00071A10 File Offset: 0x0006FC10
        public static Color magenta
        {
            get
            {
                return new Color(1f, 0f, 1f, 1f);
            }
        }

        // Token: 0x17000EDD RID: 3805
        // (get) Token: 0x06003F0D RID: 16141 RVA: 0x00071A40 File Offset: 0x0006FC40
        public static Color gray
        {
            get
            {
                return new Color(0.5f, 0.5f, 0.5f, 1f);
            }
        }

        // Token: 0x17000EDE RID: 3806
        // (get) Token: 0x06003F0E RID: 16142 RVA: 0x00071A70 File Offset: 0x0006FC70
        public static Color grey
        {
            get
            {
                return new Color(0.5f, 0.5f, 0.5f, 1f);
            }
        }

        // Token: 0x17000EDF RID: 3807
        // (get) Token: 0x06003F0F RID: 16143 RVA: 0x00071AA0 File Offset: 0x0006FCA0
        public static Color clear
        {
            get
            {
                return new Color(0f, 0f, 0f, 0f);
            }
        }

        // Token: 0x17000EE0 RID: 3808
        // (get) Token: 0x06003F10 RID: 16144 RVA: 0x00071AD0 File Offset: 0x0006FCD0
        public float grayscale
        {
            get
            {
                return 0.299f * r + 0.587f * g + 0.114f * b;
            }
        }

        // Token: 0x17000EE3 RID: 3811
        // (get) Token: 0x06003F13 RID: 16147 RVA: 0x00071B94 File Offset: 0x0006FD94
        public float maxColorComponent
        {
            get
            {
                return Mathf.Max(Mathf.Max(r, g), b);
            }
        }

        // Token: 0x06003F14 RID: 16148 RVA: 0x00071BC8 File Offset: 0x0006FDC8
        public static implicit operator Vector4(Color c)
        {
            return new Vector4(c.r, c.g, c.b, c.a);
        }

        // Token: 0x06003F15 RID: 16149 RVA: 0x00071C00 File Offset: 0x0006FE00
        public static implicit operator Color(Vector4 v)
        {
            return new Color(v.x, v.y, v.z, v.w);
        }

        // Token: 0x17000EE4 RID: 3812
        public float this[int index]
        {
            get
            {
                float result;
                switch (index)
                {
                    case 0:
                        result = r;
                        break;
                    case 1:
                        result = g;
                        break;
                    case 2:
                        result = b;
                        break;
                    case 3:
                        result = a;
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
                        r = value;
                        break;
                    case 1:
                        g = value;
                        break;
                    case 2:
                        b = value;
                        break;
                    case 3:
                        a = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3 index!");
                }
            }
        }

        // Token: 0x06003F18 RID: 16152 RVA: 0x00071D04 File Offset: 0x0006FF04
        public static void RGBToHSV(Color rgbColor, out float H, out float S, out float V)
        {
            if (rgbColor.b > rgbColor.g && rgbColor.b > rgbColor.r)
            {
                Color.RGBToHSVHelper(4f, rgbColor.b, rgbColor.r, rgbColor.g, out H, out S, out V);
            }
            else if (rgbColor.g > rgbColor.r)
            {
                Color.RGBToHSVHelper(2f, rgbColor.g, rgbColor.b, rgbColor.r, out H, out S, out V);
            }
            else
            {
                Color.RGBToHSVHelper(0f, rgbColor.r, rgbColor.g, rgbColor.b, out H, out S, out V);
            }
        }

        // Token: 0x06003F19 RID: 16153 RVA: 0x00071DBC File Offset: 0x0006FFBC
        private static void RGBToHSVHelper(float offset, float dominantcolor, float colorone, float colortwo, out float H, out float S, out float V)
        {
            V = dominantcolor;
            if (V != 0f)
            {
                float num;
                if (colorone > colortwo)
                {
                    num = colortwo;
                }
                else
                {
                    num = colorone;
                }
                float num2 = V - num;
                if (num2 != 0f)
                {
                    S = num2 / V;
                    H = offset + (colorone - colortwo) / num2;
                }
                else
                {
                    S = 0f;
                    H = offset + (colorone - colortwo);
                }
                H /= 6f;
                if (H < 0f)
                {
                    H += 1f;
                }
            }
            else
            {
                S = 0f;
                H = 0f;
            }
        }

        // Token: 0x06003F1A RID: 16154 RVA: 0x00071E6C File Offset: 0x0007006C
        public static Color HSVToRGB(float H, float S, float V)
        {
            return Color.HSVToRGB(H, S, V, true);
        }

        // Token: 0x06003F1B RID: 16155 RVA: 0x00071E8C File Offset: 0x0007008C
        public static Color HSVToRGB(float H, float S, float V, bool hdr)
        {
            Color white = Color.white;
            if (S == 0f)
            {
                white.r = V;
                white.g = V;
                white.b = V;
            }
            else if (V == 0f)
            {
                white.r = 0f;
                white.g = 0f;
                white.b = 0f;
            }
            else
            {
                white.r = 0f;
                white.g = 0f;
                white.b = 0f;
                float num = H * 6f;
                int num2 = (int)Mathf.Floor(num);
                float num3 = num - num2;
                float num4 = V * (1f - S);
                float num5 = V * (1f - S * num3);
                float num6 = V * (1f - S * (1f - num3));
                switch (num2 + 1)
                {
                    case 0:
                        white.r = V;
                        white.g = num4;
                        white.b = num5;
                        break;
                    case 1:
                        white.r = V;
                        white.g = num6;
                        white.b = num4;
                        break;
                    case 2:
                        white.r = num5;
                        white.g = V;
                        white.b = num4;
                        break;
                    case 3:
                        white.r = num4;
                        white.g = V;
                        white.b = num6;
                        break;
                    case 4:
                        white.r = num4;
                        white.g = num5;
                        white.b = V;
                        break;
                    case 5:
                        white.r = num6;
                        white.g = num4;
                        white.b = V;
                        break;
                    case 6:
                        white.r = V;
                        white.g = num4;
                        white.b = num5;
                        break;
                    case 7:
                        white.r = V;
                        white.g = num6;
                        white.b = num4;
                        break;
                }
                if (!hdr)
                {
                    white.r = Mathf.Clamp(white.r, 0f, 1f);
                    white.g = Mathf.Clamp(white.g, 0f, 1f);
                    white.b = Mathf.Clamp(white.b, 0f, 1f);
                }
            }
            return white;
        }

        // Token: 0x04001336 RID: 4918
        public float r;

        // Token: 0x04001337 RID: 4919
        public float g;

        // Token: 0x04001338 RID: 4920
        public float b;

        // Token: 0x04001339 RID: 4921
        public float a;

        public string hex;
        #endregion

        public static UnityEngine.Color operator +(UnityEngine.Color a, Color b)
        {
            return new UnityEngine.Color(a.r + b.r, a.g + b.g, a.b + b.b, a.a + b.a);
        }

        // Token: 0x06005152 RID: 20818 RVA: 0x0008CE24 File Offset: 0x0008B024
        public static UnityEngine.Color operator -(UnityEngine.Color a, Color b)
        {
            return new UnityEngine.Color(a.r - b.r, a.g - b.g, a.b - b.b, a.a - b.a);
        }

        // Token: 0x06005153 RID: 20819 RVA: 0x0008CE5C File Offset: 0x0008B05C
        public static UnityEngine.Color operator *(UnityEngine.Color a, Color b)
        {
            return new UnityEngine.Color(a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a);
        }

        // Token: 0x06005154 RID: 20820 RVA: 0x0008CE94 File Offset: 0x0008B094
        public static UnityEngine.Color operator /(UnityEngine.Color a, Color b)
        {
            return new UnityEngine.Color(a.r / b.r, a.g / b.g, a.b / b.b, a.a / b.a);
        }


        public static Color operator +(Color a, UnityEngine.Color b)
        {
            return new Color(a.r + b.r, a.g + b.g, a.b + b.b, a.a + b.a);
        }

        // Token: 0x06005152 RID: 20818 RVA: 0x0008CE24 File Offset: 0x0008B024
        public static Color operator -(Color a, UnityEngine.Color b)
        {
            return new Color(a.r - b.r, a.g - b.g, a.b - b.b, a.a - b.a);
        }

        // Token: 0x06005153 RID: 20819 RVA: 0x0008CE5C File Offset: 0x0008B05C
        public static Color operator *(Color a, UnityEngine.Color b)
        {
            return new Color(a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a);
        }

        // Token: 0x06005154 RID: 20820 RVA: 0x0008CE94 File Offset: 0x0008B094
        public static Color operator /(Color a, UnityEngine.Color b)
        {
            return new Color(a.r / b.r, a.g / b.g, a.b / b.b, a.a / b.a);
        }



        // Token: 0x06005159 RID: 20825 RVA: 0x0008CF7C File Offset: 0x0008B17C
        public static bool operator ==(UnityEngine.Color lhs, Color rhs)
        {
            return lhs.r == rhs.r && lhs.g == rhs.g && lhs.b == rhs.b && lhs.a == rhs.a;
        }

        // Token: 0x0600515A RID: 20826 RVA: 0x0008CFA8 File Offset: 0x0008B1A8
        public static bool operator !=(UnityEngine.Color lhs, Color rhs)
        {
            return !(lhs == rhs);
        }

        // Token: 0x06005159 RID: 20825 RVA: 0x0008CF7C File Offset: 0x0008B17C
        public static bool operator ==(Color rhs, UnityEngine.Color lhs)
        {
            return lhs.r == rhs.r && lhs.g == rhs.g && lhs.b == rhs.b && lhs.a == rhs.a;
        }

        // Token: 0x0600515A RID: 20826 RVA: 0x0008CFA8 File Offset: 0x0008B1A8
        public static bool operator !=(Color rhs, UnityEngine.Color lhs)
        {
            return !(lhs == rhs);
        }

        public static implicit operator UnityEngine.Color(Color v)
        {
            try
            {//不能局限于unity编辑器使用, 当程序编译出来也可以使用才对
                if (string.IsNullOrEmpty(v.hex))
                    return new UnityEngine.Color(v.r, v.g, v.b, v.a);
                ColorUtility.TryParseHtmlString("#" + v.hex, out UnityEngine.Color color);//这里只限制在unity使用
                return color;
            }
            catch
            {
                return new UnityEngine.Color(v.r, v.g, v.b, v.a);
            }
        }

        public static implicit operator UnityEngine.Color32(Color v)
        {
            try
            {//不能局限于unity编辑器使用, 当程序编译出来也可以使用才对
                if (string.IsNullOrEmpty(v.hex))
                    return new UnityEngine.Color(v.r, v.g, v.b, v.a);
                ColorUtility.TryParseHtmlString("#" + v.hex, out UnityEngine.Color color);//这里只限制在unity使用
                return color;
            }
            catch
            {
                return new UnityEngine.Color(v.r, v.g, v.b, v.a);
            }
        }

        public static implicit operator Color(UnityEngine.Color v)
        {
            string hex = ColorUtility.ToHtmlStringRGBA(v);
            return new Color(v.r, v.g, v.b, v.a) { hex = hex };
        }

        public static implicit operator Color(UnityEngine.Color32 v)
        {
            string hex = ColorUtility.ToHtmlStringRGBA(v);
            return new Color32(v.r, v.g, v.b, v.a) { hex = hex };
        }
    }
}