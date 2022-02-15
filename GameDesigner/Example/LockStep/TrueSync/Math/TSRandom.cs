namespace TrueSync
{
    using System;

    [Serializable]
    public class TSRandom
    {
        internal static TSRandom random = new TSRandom();
        private int inext;
        private int inextp;
        private int[] SeedArray = new int[56];
        private int currentValue;
        private int loopNum;

        internal static TSRandom New(int v)
        {
            return new TSRandom(v);
        }

        public TSRandom()
        {
            SetSeed(Environment.TickCount);
        }

        public TSRandom(int seed)
        {
            SetSeed(seed);
        }

        public static int MaxRandomInt { get { return 0x7fffffff; } }

        internal FP NextFP()
        {
            return ((FP)Next()) / MaxRandomInt;
        }

        public static FP Value => new TSRandom(1000).NextFP();

        /// <summary>
        /// 初始化随机种子
        /// </summary>
        /// <param name="Seed"></param>
        public static void InitSeed(int Seed)
        {
            random = new TSRandom(Seed);
        }

        public void SetSeed(int Seed)
        {
            int num = (Seed == int.MinValue) ? int.MaxValue : (int)TSMathf.Abs(Seed);
            int num2 = 161803398 - num;
            SeedArray[55] = num2;
            int num3 = 1;
            for (int i = 1; i < 55; i++)
            {
                int num4 = 21 * i % 55;
                SeedArray[num4] = num3;
                num3 = num2 - num3;
                if (num3 < 0)
                {
                    num3 += int.MaxValue;
                }
                num2 = SeedArray[num4];
            }
            for (int j = 1; j < 5; j++)
            {
                for (int k = 1; k < 56; k++)
                {
                    SeedArray[k] -= SeedArray[1 + (k + 30) % 55];
                    if (SeedArray[k] < 0)
                    {
                        SeedArray[k] += int.MaxValue;
                    }
                }
            }
            inext = 0;
            inextp = 21;
        }

        private decimal Sample()
        {
            return InternalSample() * 4.656612875245797E-10m;
        }

        private int InternalSample()
        {
            int num = inext;
            int num2 = inextp;
            if (++num >= 56)
            {
                num = 1;
            }
            if (++num2 >= 56)
            {
                num2 = 1;
            }
            int num3 = SeedArray[num] - SeedArray[num2];
            if (num3 == 2147483647)
            {
                num3--;
            }
            if (num3 < 0)
            {
                num3 += int.MaxValue;
            }
            SeedArray[num] = num3;
            inext = num;
            inextp = num2;
            return num3;
        }

        public int Next()
        {
            return InternalSample();
        }

        private decimal GetSampleForLargeRange()
        {
            int num = InternalSample();
            bool flag = InternalSample() % 2 == 0;
            if (flag)
            {
                num = -num;
            }
            decimal num2 = num;
            num2 += 2147483646.0m;
            return num2 / 4294967293.0m;
        }

        public int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                int min = minValue;
                int max = maxValue;
                maxValue = min;
                minValue = max;
            }
            long num = maxValue - minValue;
            if (num <= 2147483647L)
            {
                return (int)(Sample() * num) + minValue;
            }
            return (int)((long)(GetSampleForLargeRange() * num) + minValue);
        }

        public decimal Next(decimal minValue, decimal maxValue)
        {
            if (minValue > maxValue)
            {
                decimal min = minValue;
                decimal max = maxValue;
                maxValue = min;
                minValue = max;
            }
            decimal num = maxValue - minValue;
            if (num <= 2147483647L)
            {
                return (Sample() * num) + minValue;
            }
            return (long)(GetSampleForLargeRange() * num) + minValue;
        }

        public int Next(int maxValue)
        {
            if (maxValue < 0)
            {
                throw new ArgumentOutOfRangeException("maxValue");
            }
            return (int)(Sample() * maxValue);
        }

        public double NextDouble()
        {
            return decimal.ToDouble(Sample());
        }

        public void NextBytes(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)(InternalSample() % 256);
            }
        }

        /// <summary>
        /// 随机范围
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int Range(int minValue, int maxValue)
        {
            return random.Range1(minValue, maxValue);
        }

        /// <summary>
        /// 随机范围
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static FP Range(FP minValue, FP maxValue)
        {
            return (FP)random.Range1((decimal)minValue, (decimal)maxValue);
        }

        public int Range1(int minValue, int maxValue)
        {
            if (loopNum > 5)
            {
                if (minValue > maxValue)
                    currentValue = (minValue - maxValue) / 2;
                else
                    currentValue = (maxValue - minValue) / 2;
                loopNum = 0;
                return currentValue;
            }
            int value;
            loopNum++;
            if (minValue > maxValue)
                value = Next(maxValue, minValue);
            else
                value = Next(minValue, maxValue);
            if (value == currentValue)
                return Range1(minValue, maxValue);
            currentValue = value;
            loopNum = 0;
            return value;
        }

        /// <summary>
        /// 随机范围
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static decimal Range(decimal minValue, decimal maxValue)
        {
            return random.Range1(minValue, maxValue);
        }

        public decimal Range1(decimal minValue, decimal maxValue)
        {
            if (minValue > maxValue)
            {
                decimal maxValue1 = maxValue;
                decimal minValue1 = minValue;
                minValue = maxValue1;
                maxValue = minValue1;
            }
            long value1 = (long)(minValue * 10000);
            long value2 = (long)(maxValue * 10000);
            decimal value3 = Next(value1, value2) * 0.0001m;
            if (value3 < minValue | value3 > maxValue)
                return Range1(minValue, maxValue);
            return value3;
        }

        /// <summary>
        /// 随机范围
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static float Range(float minValue, float maxValue)
        {
            return random.Range1(minValue, maxValue);
        }

        public float Range1(float minValue, float maxValue)
        {
            return (float)Range1((decimal)minValue, (decimal)maxValue);
        }
    }
}