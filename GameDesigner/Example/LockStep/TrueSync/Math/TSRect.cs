using System;
using System.Runtime.CompilerServices;

namespace TrueSync
{
    [Serializable]
    public struct TSRect : IEquatable<TSRect>
    {
        #region 源码
        // Token: 0x06004B52 RID: 19282 RVA: 0x00081FBF File Offset: 0x000801BF
        public TSRect(FP x, FP y, FP width, FP height)
        {
            m_XMin = x;
            m_YMin = y;
            m_Width = width;
            m_Height = height;
        }

        // Token: 0x06004B53 RID: 19283 RVA: 0x00081FDF File Offset: 0x000801DF
        public TSRect(TSVector2 position, TSVector2 size)
        {
            m_XMin = position.x;
            m_YMin = position.y;
            m_Width = size.x;
            m_Height = size.y;
        }

        // Token: 0x06004B54 RID: 19284 RVA: 0x00082016 File Offset: 0x00080216
        public TSRect(TSRect source)
        {
            m_XMin = source.m_XMin;
            m_YMin = source.m_YMin;
            m_Width = source.m_Width;
            m_Height = source.m_Height;
        }

        // Token: 0x17001174 RID: 4468
        // (get) Token: 0x06004B55 RID: 19285 RVA: 0x00082050 File Offset: 0x00080250
        public static TSRect zero
        {
            [CompilerGenerated]
            get
            {
                return new TSRect(0f, 0f, 0f, 0f);
            }
        }

        // Token: 0x06004B56 RID: 19286 RVA: 0x00082080 File Offset: 0x00080280
        public static TSRect MinMaxRect(FP xmin, FP ymin, FP xmax, FP ymax)
        {
            return new TSRect(xmin, ymin, xmax - xmin, ymax - ymin);
        }

        // Token: 0x06004B57 RID: 19287 RVA: 0x00081FBF File Offset: 0x000801BF
        public void Set(FP x, FP y, FP width, FP height)
        {
            m_XMin = x;
            m_YMin = y;
            m_Width = width;
            m_Height = height;
        }

        // Token: 0x17001175 RID: 4469
        // (get) Token: 0x06004B58 RID: 19288 RVA: 0x000820A4 File Offset: 0x000802A4
        // (set) Token: 0x06004B59 RID: 19289 RVA: 0x000820BF File Offset: 0x000802BF
        public FP x
        {
            get
            {
                return m_XMin;
            }
            set
            {
                m_XMin = value;
            }
        }

        // Token: 0x17001176 RID: 4470
        // (get) Token: 0x06004B5A RID: 19290 RVA: 0x000820CC File Offset: 0x000802CC
        // (set) Token: 0x06004B5B RID: 19291 RVA: 0x000820E7 File Offset: 0x000802E7
        public FP y
        {
            get
            {
                return m_YMin;
            }
            set
            {
                m_YMin = value;
            }
        }

        // Token: 0x17001177 RID: 4471
        // (get) Token: 0x06004B5C RID: 19292 RVA: 0x000820F4 File Offset: 0x000802F4
        // (set) Token: 0x06004B5D RID: 19293 RVA: 0x0008211A File Offset: 0x0008031A
        public TSVector2 position
        {
            get
            {
                return new TSVector2(m_XMin, m_YMin);
            }
            set
            {
                m_XMin = value.x;
                m_YMin = value.y;
            }
        }

        // Token: 0x17001178 RID: 4472
        // (get) Token: 0x06004B5E RID: 19294 RVA: 0x00082138 File Offset: 0x00080338
        // (set) Token: 0x06004B5F RID: 19295 RVA: 0x00082178 File Offset: 0x00080378
        public TSVector2 center
        {
            get
            {
                return new TSVector2(x + m_Width / 2f, y + m_Height / 2f);
            }
            set
            {
                m_XMin = value.x - m_Width / 2f;
                m_YMin = value.y - m_Height / 2f;
            }
        }

        // Token: 0x17001179 RID: 4473
        // (get) Token: 0x06004B60 RID: 19296 RVA: 0x000821B0 File Offset: 0x000803B0
        // (set) Token: 0x06004B61 RID: 19297 RVA: 0x000821D6 File Offset: 0x000803D6
        public TSVector2 min
        {
            get
            {
                return new TSVector2(xMin, yMin);
            }
            set
            {
                xMin = value.x;
                yMin = value.y;
            }
        }

        // Token: 0x1700117A RID: 4474
        // (get) Token: 0x06004B62 RID: 19298 RVA: 0x000821F4 File Offset: 0x000803F4
        // (set) Token: 0x06004B63 RID: 19299 RVA: 0x0008221A File Offset: 0x0008041A
        public TSVector2 max
        {
            get
            {
                return new TSVector2(xMax, yMax);
            }
            set
            {
                xMax = value.x;
                yMax = value.y;
            }
        }

        // Token: 0x1700117B RID: 4475
        // (get) Token: 0x06004B64 RID: 19300 RVA: 0x00082238 File Offset: 0x00080438
        // (set) Token: 0x06004B65 RID: 19301 RVA: 0x00082253 File Offset: 0x00080453
        public FP width
        {
            get
            {
                return m_Width;
            }
            set
            {
                m_Width = value;
            }
        }

        // Token: 0x1700117C RID: 4476
        // (get) Token: 0x06004B66 RID: 19302 RVA: 0x00082260 File Offset: 0x00080460
        // (set) Token: 0x06004B67 RID: 19303 RVA: 0x0008227B File Offset: 0x0008047B
        public FP height
        {
            get
            {
                return m_Height;
            }
            set
            {
                m_Height = value;
            }
        }

        // Token: 0x1700117D RID: 4477
        // (get) Token: 0x06004B68 RID: 19304 RVA: 0x00082288 File Offset: 0x00080488
        // (set) Token: 0x06004B69 RID: 19305 RVA: 0x000822AE File Offset: 0x000804AE
        public TSVector2 size
        {
            get
            {
                return new TSVector2(m_Width, m_Height);
            }
            set
            {
                m_Width = value.x;
                m_Height = value.y;
            }
        }

        // Token: 0x1700117E RID: 4478
        // (get) Token: 0x06004B6A RID: 19306 RVA: 0x000822CC File Offset: 0x000804CC
        // (set) Token: 0x06004B6B RID: 19307 RVA: 0x000822E8 File Offset: 0x000804E8
        public FP xMin
        {
            get
            {
                return m_XMin;
            }
            set
            {
                FP xMax = this.xMax;
                m_XMin = value;
                m_Width = xMax - m_XMin;
            }
        }

        // Token: 0x1700117F RID: 4479
        // (get) Token: 0x06004B6C RID: 19308 RVA: 0x00082314 File Offset: 0x00080514
        // (set) Token: 0x06004B6D RID: 19309 RVA: 0x00082330 File Offset: 0x00080530
        public FP yMin
        {
            get
            {
                return m_YMin;
            }
            set
            {
                FP yMax = this.yMax;
                m_YMin = value;
                m_Height = yMax - m_YMin;
            }
        }

        // Token: 0x17001180 RID: 4480
        // (get) Token: 0x06004B6E RID: 19310 RVA: 0x0008235C File Offset: 0x0008055C
        // (set) Token: 0x06004B6F RID: 19311 RVA: 0x0008237E File Offset: 0x0008057E
        public FP xMax
        {
            get
            {
                return m_Width + m_XMin;
            }
            set
            {
                m_Width = value - m_XMin;
            }
        }

        // Token: 0x17001181 RID: 4481
        // (get) Token: 0x06004B70 RID: 19312 RVA: 0x00082390 File Offset: 0x00080590
        // (set) Token: 0x06004B71 RID: 19313 RVA: 0x000823B2 File Offset: 0x000805B2
        public FP yMax
        {
            get
            {
                return m_Height + m_YMin;
            }
            set
            {
                m_Height = value - m_YMin;
            }
        }

        // Token: 0x06004B72 RID: 19314 RVA: 0x000823C4 File Offset: 0x000805C4
        public bool Contains(TSVector2 point)
        {
            return point.x >= xMin && point.x < xMax && point.y >= yMin && point.y < yMax;
        }

        // Token: 0x06004B73 RID: 19315 RVA: 0x00082424 File Offset: 0x00080624
        public bool Contains(TSVector3 point)
        {
            return point.x >= xMin && point.x < xMax && point.y >= yMin && point.y < yMax;
        }

        // Token: 0x06004B74 RID: 19316 RVA: 0x00082484 File Offset: 0x00080684
        public bool Contains(TSVector3 point, bool allowInverse)
        {
            bool result;
            if (!allowInverse)
            {
                result = Contains(point);
            }
            else
            {
                bool flag = false;
                if ((width < 0f && point.x <= xMin && point.x > xMax) || (width >= 0f && point.x >= xMin && point.x < xMax))
                {
                    flag = true;
                }
                result = (flag && ((height < 0f && point.y <= yMin && point.y > yMax) || (height >= 0f && point.y >= yMin && point.y < yMax)));
            }
            return result;
        }

        // Token: 0x06004B75 RID: 19317 RVA: 0x00082590 File Offset: 0x00080790
        private static TSRect OrderMinMax(TSRect rect)
        {
            if (rect.xMin > rect.xMax)
            {
                FP xMin = rect.xMin;
                rect.xMin = rect.xMax;
                rect.xMax = xMin;
            }
            if (rect.yMin > rect.yMax)
            {
                FP yMin = rect.yMin;
                rect.yMin = rect.yMax;
                rect.yMax = yMin;
            }
            return rect;
        }

        // Token: 0x06004B76 RID: 19318 RVA: 0x0008260C File Offset: 0x0008080C
        public bool Overlaps(TSRect other)
        {
            return other.xMax > xMin && other.xMin < xMax && other.yMax > yMin && other.yMin < yMax;
        }

        // Token: 0x06004B77 RID: 19319 RVA: 0x0008266C File Offset: 0x0008086C
        public bool Overlaps(TSRect other, bool allowInverse)
        {
            TSRect rect = this;
            if (allowInverse)
            {
                rect = TSRect.OrderMinMax(rect);
                other = TSRect.OrderMinMax(other);
            }
            return rect.Overlaps(other);
        }

        // Token: 0x06004B78 RID: 19320 RVA: 0x000826A8 File Offset: 0x000808A8
        public static TSVector2 NormalizedToPoint(TSRect rectangle, TSVector2 normalizedRectCoordinates)
        {
            return new TSVector2(TSMathf.Lerp(rectangle.x, rectangle.xMax, normalizedRectCoordinates.x), TSMathf.Lerp(rectangle.y, rectangle.yMax, normalizedRectCoordinates.y));
        }

        // Token: 0x06004B79 RID: 19321 RVA: 0x000826F8 File Offset: 0x000808F8
        public static TSVector2 PointToNormalized(TSRect rectangle, TSVector2 point)
        {
            return new TSVector2(TSMathf.InverseLerp(rectangle.x, rectangle.xMax, point.x), TSMathf.InverseLerp(rectangle.y, rectangle.yMax, point.y));
        }

        // Token: 0x06004B7A RID: 19322 RVA: 0x00082748 File Offset: 0x00080948
        public static bool operator !=(TSRect lhs, TSRect rhs)
        {
            return !(lhs == rhs);
        }

        // Token: 0x06004B7B RID: 19323 RVA: 0x00082768 File Offset: 0x00080968
        public static bool operator ==(TSRect lhs, TSRect rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.width == rhs.width && lhs.height == rhs.height;
        }

        // Token: 0x06004B7C RID: 19324 RVA: 0x000827CC File Offset: 0x000809CC
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ width.GetHashCode() << 2 ^ y.GetHashCode() >> 2 ^ height.GetHashCode() >> 1;
        }

        // Token: 0x06004B7D RID: 19325 RVA: 0x0008283C File Offset: 0x00080A3C
        public override bool Equals(object other)
        {
            return other is TSRect && Equals((TSRect)other);
        }

        // Token: 0x06004B7E RID: 19326 RVA: 0x00082870 File Offset: 0x00080A70
        public bool Equals(TSRect other)
        {
            return x.Equals(other.x) && y.Equals(other.y) && width.Equals(other.width) && height.Equals(other.height);
        }

        // Token: 0x06004B7F RID: 19327 RVA: 0x000828F0 File Offset: 0x00080AF0
        public override string ToString()
        {
            return string.Format("(x:{0:F2}, y:{1:F2}, width:{2:F2}, height:{3:F2})", new object[]
            {
                x,
                y,
                width,
                height
            });
        }

        // Token: 0x06004B80 RID: 19328 RVA: 0x00082950 File Offset: 0x00080B50
        public string ToString(string format)
        {
            return string.Format("(x:{0}, y:{1}, width:{2}, height:{3})", new object[]
            {
                x.ToString(format),
                y.ToString(format),
                width.ToString(format),
                height.ToString(format)
            });
        }

        // Token: 0x17001182 RID: 4482
        // (get) Token: 0x06004B81 RID: 19329 RVA: 0x000829C0 File Offset: 0x00080BC0
        [Obsolete("use xMin")]
        public FP left
        {
            get
            {
                return m_XMin;
            }
        }

        // Token: 0x17001183 RID: 4483
        // (get) Token: 0x06004B82 RID: 19330 RVA: 0x000829DC File Offset: 0x00080BDC
        [Obsolete("use xMax")]
        public FP right
        {
            get
            {
                return m_XMin + m_Width;
            }
        }

        // Token: 0x17001184 RID: 4484
        // (get) Token: 0x06004B83 RID: 19331 RVA: 0x00082A00 File Offset: 0x00080C00
        [Obsolete("use yMin")]
        public FP top
        {
            get
            {
                return m_YMin;
            }
        }

        // Token: 0x17001185 RID: 4485
        // (get) Token: 0x06004B84 RID: 19332 RVA: 0x00082A1C File Offset: 0x00080C1C
        [Obsolete("use yMax")]
        public FP bottom
        {
            get
            {
                return m_YMin + m_Height;
            }
        }

        private FP m_XMin;

        private FP m_YMin;

        private FP m_Width;

        private FP m_Height;
        #endregion

        // Token: 0x06005151 RID: 20817 RVA: 0x0008CDEC File Offset: 0x0008AFEC
        public static UnityEngine.Rect operator +(UnityEngine.Rect a, TSRect b)
        {
            return new UnityEngine.Rect((a.x + b.x).AsFloat(), (a.y + b.y).AsFloat(), (a.width + b.width).AsFloat(), (a.height + b.height).AsFloat());
        }

        // Token: 0x06005152 RID: 20818 RVA: 0x0008CE24 File Offset: 0x0008B024
        public static UnityEngine.Rect operator -(UnityEngine.Rect a, TSRect b)
        {
            return new UnityEngine.Rect((a.x - b.x).AsFloat(), (a.y - b.y).AsFloat(), (a.width - b.width).AsFloat(), (a.height - b.height).AsFloat());
        }

        // Token: 0x06005153 RID: 20819 RVA: 0x0008CE5C File Offset: 0x0008B05C
        public static UnityEngine.Rect operator *(UnityEngine.Rect a, TSRect b)
        {
            return new UnityEngine.Rect((a.x * b.x).AsFloat(), (a.y * b.y).AsFloat(), (a.width * b.width).AsFloat(), (a.height * b.height).AsFloat());
        }

        // Token: 0x06005154 RID: 20820 RVA: 0x0008CE94 File Offset: 0x0008B094
        public static UnityEngine.Rect operator /(UnityEngine.Rect a, TSRect b)
        {
            return new UnityEngine.Rect((a.x / b.x).AsFloat(), (a.y / b.y).AsFloat(), (a.width / b.width).AsFloat(), (a.height / b.height).AsFloat());
        }


        public static TSRect operator +(TSRect a, UnityEngine.Rect b)
        {
            return new TSRect(a.x + b.x, a.y + b.y, a.width + b.width, a.height + b.height);
        }

        // Token: 0x06005152 RID: 20818 RVA: 0x0008CE24 File Offset: 0x0008B024
        public static TSRect operator -(TSRect a, UnityEngine.Rect b)
        {
            return new TSRect(a.x - b.x, a.y - b.y, a.width - b.width, a.height - b.height);
        }

        // Token: 0x06005153 RID: 20819 RVA: 0x0008CE5C File Offset: 0x0008B05C
        public static TSRect operator *(TSRect a, UnityEngine.Rect b)
        {
            return new TSRect(a.x * b.x, a.y * b.y, a.width * b.width, a.height * b.height);
        }

        // Token: 0x06005154 RID: 20820 RVA: 0x0008CE94 File Offset: 0x0008B094
        public static TSRect operator /(TSRect a, UnityEngine.Rect b)
        {
            return new TSRect(a.x / b.x, a.y / b.y, a.width / b.width, a.height / b.height);
        }



        // Token: 0x06005159 RID: 20825 RVA: 0x0008CF7C File Offset: 0x0008B17C
        public static bool operator ==(UnityEngine.Rect lhs, TSRect rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.width == rhs.width && lhs.height == rhs.height;
        }

        // Token: 0x0600515A RID: 20826 RVA: 0x0008CFA8 File Offset: 0x0008B1A8
        public static bool operator !=(UnityEngine.Rect lhs, TSRect rhs)
        {
            return !(lhs == rhs);
        }

        public static implicit operator UnityEngine.Rect(TSRect v)
        {
            return new UnityEngine.Rect(v.x.AsFloat(), v.y.AsFloat(), v.width.AsFloat(), v.height.AsFloat());
        }

        public static implicit operator TSRect(UnityEngine.Rect v)
        {
            return new TSRect(v.x, v.y, v.width, v.height);
        }
    }
}