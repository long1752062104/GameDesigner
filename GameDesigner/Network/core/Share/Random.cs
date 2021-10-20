namespace Net.Share
{
    using global::System;
    using Net.System;

    /// <summary>
    /// 随机帮助类 (多线程安全)
    /// </summary>
    public class RandomHelper : RandomSafe
    {
        private static RandomSafe random = new RandomSafe(Guid.NewGuid().GetHashCode());

        public RandomHelper() { }

        public RandomHelper(int seed) : base(seed)
        {
        }

        /// <summary>
        /// 初始化随机种子
        /// </summary>
        /// <param name="Seed"></param>
        public static void InitSeed(int Seed)
        {
            random = new RandomSafe(Seed);
        }

        /// <summary>
        /// 随机范围
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int Range(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                return random.Next(maxValue, minValue);
            }
            return random.Next(minValue, maxValue);
        }

        /// <summary>
        /// 随机范围
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static float Range(float minValue, float maxValue)
        {
            if (minValue > maxValue)
            {
                float maxValue1 = maxValue;
                float minValue1 = minValue;
                minValue = maxValue1;
                maxValue = minValue1;
            }
            int value1 = (int)(minValue * 10000);
            int value2 = (int)(maxValue * 10000);
            float value3 = random.Next(value1, value2) * 0.0001f;
            if (value3 < minValue | value3 > maxValue)
                return Range(minValue, maxValue);
            return value3;
        }

        /// <summary>
        /// 随机范围
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public int Range1(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                return Next(maxValue, minValue);
            }
            return Next(minValue, maxValue);
        }

        /// <summary>
        /// 随机范围
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public float Range1(float minValue, float maxValue)
        {
            if (minValue > maxValue)
            {
                float maxValue1 = maxValue;
                float minValue1 = minValue;
                minValue = maxValue1;
                maxValue = minValue1;
            }
            int value1 = (int)(minValue * 10000);
            int value2 = (int)(maxValue * 10000);
            float value3 = Next(value1, value2) * 0.0001f;
            if (value3 < minValue | value3 > maxValue)
                return Range1(minValue, maxValue);
            return value3;
        }
    }
}