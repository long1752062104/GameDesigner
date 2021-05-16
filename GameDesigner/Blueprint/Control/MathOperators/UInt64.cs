namespace GameDesigner.MathOperations
{
    [System.Serializable]
    public class UInt64
    {
        /// <summary>
        /// 返回a+b
        /// </summary>
        static public System.UInt64 Adds(System.UInt64 a, System.UInt64 b)
        {
            return a + b;
        }

        /// <summary>
        /// 返回a-b
        /// </summary>
        static public System.UInt64 Subtracts(System.UInt64 a, System.UInt64 b)
        {
            return a - b;
        }

        /// <summary>
        /// 返回a*b
        /// </summary>
        static public System.UInt64 Multiplies(System.UInt64 a, System.UInt64 b)
        {
            return a * b;
        }

        /// <summary>
        /// 返回a/b
        /// </summary>
        static public System.UInt64 Divides(System.UInt64 a, System.UInt64 b)
        {
            return a / b;
        }

        /// <summary>
        /// 返回a%b
        /// </summary>
        static public System.UInt64 BaiFeiBi100(System.UInt64 a, System.UInt64 b)
        {
            return a % b;
        }

        /// <summary>
        /// 返回a^b
        /// </summary>
        static public System.UInt64 QiuYu(System.UInt64 a, System.UInt64 b)
        {
            return a ^ b;
        }

        /// <summary>
        /// 返回a&b
        /// </summary>
        static public System.UInt64 BinQie(System.UInt64 a, System.UInt64 b)
        {
            return a & b;
        }

        /// <summary>
        /// 返回a==b
        /// </summary>
        static public bool Equals(System.UInt64 a, System.UInt64 b)
        {
            return a == b;
        }

        /// <summary>
        /// 返回a!=b
        /// </summary>
        static public bool NotEquals(System.UInt64 a, System.UInt64 b)
        {
            return a != b;
        }

        /// <summary>
        /// 返回a>=b
        /// </summary>
        static public bool MaxEquals(System.UInt64 a, System.UInt64 b)
        {
            return a >= b;
        }

        /// <summary>
        /// 返回a<=b
        /// </summary>
        static public bool MinEquals(System.UInt64 a, System.UInt64 b)
        {
            return a <= b;
        }

        /// <summary>
        /// 返回a>b
        /// </summary>
        static public bool Max(System.UInt64 a, System.UInt64 b)
        {
            return a > b;
        }

        /// <summary>
        /// 返回a<b
        /// </summary>
        static public bool Min(System.UInt64 a, System.UInt64 b)
        {
            return a < b;
        }

        public System.UInt64 value = 1;

        /// <summary>
        /// 返回a++
        /// </summary>
        public System.UInt64 AddAdds()
        {
            return value++;
        }

        /// <summary>
        /// 返回a++
        /// </summary>
        public System.UInt64 AddEquals(System.UInt64 a)
        {
            return value += a;
        }

        /// <summary>
        /// 返回a--
        /// </summary>
        public System.UInt64 SubtractSubtracts()
        {
            return value--;
        }

        /// <summary>
        /// 返回a--
        /// </summary>
        public System.UInt64 SubtractEquals(System.UInt64 a)
        {
            return value -= a;
        }

        /// <summary>
        /// 返回a*b
        /// </summary>
        public System.UInt64 MultiplieEquals(System.UInt64 a)
        {
            return value *= a;
        }

        /// <summary>
        /// 返回a/b
        /// </summary>
        public System.UInt64 DivideEquals(System.UInt64 a)
        {
            return value /= a;
        }

        /// <summary>
        /// 返回a%b
        /// </summary>
        public System.UInt64 BaiFeiBi100Equals(System.UInt64 a)
        {
            return value %= a;
        }

        /// <summary>
        /// 返回a^b
        /// </summary>
        public System.UInt64 QiuYuEquals(System.UInt64 a)
        {
            return value ^= a;
        }

        static public System.UInt64 MathOperator(System.UInt64 a, Operator Operator, System.UInt64 b)
        {
            switch (Operator)
            {
                case Operator.Adds:
                    return a + b;
                case Operator.Subtracts:
                    return a - b;
                case Operator.Multiplies:
                    return a * b;
                case Operator.Divides:
                    return a / b;
                case Operator.BaiFeiBi100:
                    return a % b;
                case Operator.QiuYu:
                    return a ^ b;
                case Operator.BinQie:
                    return a & b;
            }
            return 0;
        }

        static public bool Contitions(System.UInt64 a, Contition contition, System.UInt64 b)
        {
            switch (contition)
            {
                case Contition.Equals:
                    return a == b;
                case Contition.NotEquals:
                    return a != b;
                case Contition.MaxEquals:
                    return a >= b;
                case Contition.MinEquals:
                    return a <= b;
                case Contition.Max:
                    return a > b;
                case Contition.Min:
                    return a < b;
            }
            return false;
        }
    }
}