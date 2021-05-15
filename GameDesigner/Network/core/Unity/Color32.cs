using System;
using UnityEngine;

namespace Net
{
    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
    [Serializable]
    public struct Color32
    {
        // Token: 0x06003F1C RID: 16156 RVA: 0x000720FE File Offset: 0x000702FE
        public Color32(byte r, byte g, byte b, byte a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
            hex = "";
        }

        // Token: 0x06003F1D RID: 16157 RVA: 0x00072128 File Offset: 0x00070328
        public static implicit operator Color32(Color c)
        {
            return new Color32((byte)(Mathf.Clamp01(c.r) * 255f), (byte)(Mathf.Clamp01(c.g) * 255f), (byte)(Mathf.Clamp01(c.b) * 255f), (byte)(Mathf.Clamp01(c.a) * 255f));
        }

        // Token: 0x06003F1E RID: 16158 RVA: 0x00072190 File Offset: 0x00070390
        public static implicit operator Color(Color32 c)
        {
            return new Color(c.r / 255f, c.g / 255f, c.b / 255f, c.a / 255f);
        }

        public static implicit operator Color32(UnityEngine.Color32 v)
        {
            string hex = ColorUtility.ToHtmlStringRGB(v);
            return new Color32(v.r, v.g, v.b, v.a) { hex = hex };
        }

        public static implicit operator Color32(UnityEngine.Color v)
        {
            string hex = ColorUtility.ToHtmlStringRGB(v);
            return new Color(v.r, v.g, v.b, v.a) { hex = hex };
        }

        // Token: 0x06003F1D RID: 16157 RVA: 0x00072128 File Offset: 0x00070328
        public static implicit operator UnityEngine.Color32(Color32 c)
        {
            try
            {//不能局限于unity编辑器使用, 当程序编译出来也可以使用才对
                if (string.IsNullOrEmpty(c.hex))
                    return new UnityEngine.Color32(c.r, c.g, c.b, c.a);
                ColorUtility.TryParseHtmlString("#" + c.hex, out UnityEngine.Color color);//这里只限制在unity使用
                return color;
            }
            catch
            {
                return new UnityEngine.Color32(c.r, c.g, c.b, c.a);
            }
        }

        // Token: 0x06003F1E RID: 16158 RVA: 0x00072190 File Offset: 0x00070390
        public static implicit operator UnityEngine.Color(Color32 c)
        {
            try
            {//不能局限于unity编辑器使用, 当程序编译出来也可以使用才对
                if (string.IsNullOrEmpty(c.hex))
                    return new UnityEngine.Color32(c.r, c.g, c.b, c.a);
                ColorUtility.TryParseHtmlString("#" + c.hex, out UnityEngine.Color color);//这里只限制在unity使用
                return color;
            }
            catch
            {
                return new UnityEngine.Color32(c.r, c.g, c.b, c.a);
            }
        }

        // Token: 0x06003F1F RID: 16159 RVA: 0x000721E4 File Offset: 0x000703E4
        public static Color32 Lerp(Color32 a, Color32 b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Color32((byte)(a.r + (b.r - a.r) * t), (byte)(a.g + (b.g - a.g) * t), (byte)(a.b + (b.b - a.b) * t), (byte)(a.a + (b.a - a.a) * t));
        }

        // Token: 0x06003F20 RID: 16160 RVA: 0x00072278 File Offset: 0x00070478
        public static Color32 LerpUnclamped(Color32 a, Color32 b, float t)
        {
            return new Color32((byte)(a.r + (b.r - a.r) * t), (byte)(a.g + (b.g - a.g) * t), (byte)(a.b + (b.b - a.b) * t), (byte)(a.a + (b.a - a.a) * t));
        }

        // Token: 0x06003F21 RID: 16161 RVA: 0x00072304 File Offset: 0x00070504
        public override string ToString()
        {
            return string.Format("RGBA({0}, {1}, {2}, {3})", new object[]
            {
                r,
                g,
                b,
                a
            });
        }

        // Token: 0x06003F22 RID: 16162 RVA: 0x00072364 File Offset: 0x00070564
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

        public byte r;

        public byte g;

        public byte b;

        public byte a;

        public string hex;
    }
}
