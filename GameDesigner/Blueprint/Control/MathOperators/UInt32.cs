namespace GameDesigner.MathOperations
{
    [System.Serializable]
    public class UInt32
    {
        /// <summary>
        /// 返回a+b
        /// </summary>
        static public System.UInt32 Adds(System.UInt32 a, System.UInt32 b)
        {
            return a + b;
        }

        /// <summary>
        /// 返回a-b
        /// </summary>
        static public System.UInt32 Subtracts(System.UInt32 a, System.UInt32 b)
        {
            return a - b;
        }

        /// <summary>
        /// 返回a*b
        /// </summary>
        static public System.UInt32 Multiplies(System.UInt32 a, System.UInt32 b)
        {
            return a * b;
        }

        /// <summary>
        /// 返回a/b
        /// </summary>
        static public System.UInt32 Divides(System.UInt32 a, System.UInt32 b)
        {
            return a / b;
        }

        /// <summary>
        /// 返回a%b
        /// </summary>
        static public System.UInt32 BaiFeiBi100(System.UInt32 a, System.UInt32 b)
        {
            return a % b;
        }

        /// <summary>
        /// 返回a^b
        /// </summary>
        static public System.UInt32 QiuYu(System.UInt32 a, System.UInt32 b)
        {
            return a ^ b;
        }

        /// <summary>
        /// 返回a&b
        /// </summary>
        static public System.UInt32 BinQie(System.UInt32 a, System.UInt32 b)
        {
            return a & b;
        }

        /// <summary>
        /// 返回a==b
        /// </summary>
        static public bool Equals(System.UInt32 a, System.UInt32 b)
        {
            return a == b;
        }

        /// <summary>
        /// 返回a!=b
        /// </summary>
        static public bool NotEquals(System.UInt32 a, System.UInt32 b)
        {
            return a != b;
        }

        /// <summary>
        /// 返回a>=b
        /// </summary>
        static public bool MaxEquals(System.UInt32 a, System.UInt32 b)
        {
            return a >= b;
        }

        /// <summary>
        /// 返回a<=b
        /// </summary>
        static public bool MinEquals(System.UInt32 a, System.UInt32 b)
        {
            return a <= b;
        }

        /// <summary>
        /// 返回a>b
        /// </summary>
        static public bool Max(System.UInt32 a, System.UInt32 b)
        {
            return a > b;
        }

        /// <summary>
        /// 返回a<b
        /// </summary>
        static public bool Min(System.UInt32 a, System.UInt32 b)
        {
            return a < b;
        }

        public System.UInt32 value = 1;

        /// <summary>
        /// 返回a++
        /// </summary>
        public System.UInt32 AddAdds()
        {
            return value++;
        }

        /// <summary>
        /// 返回a++
        /// </summary>
        public System.UInt32 AddEquals(System.UInt32 a)
        {
            return value += a;
        }

        /// <summary>
        /// 返回a--
        /// </summary>
        public System.UInt32 SubtractSubtracts()
        {
            return value--;
        }

        /// <summary>
        /// 返回a--
        /// </summary>
        public System.UInt32 SubtractEquals(System.UInt32 a)
        {
            return value -= a;
        }

        /// <summary>
        /// 返回a*b
        /// </summary>
        public System.UInt32 MultiplieEquals(System.UInt32 a)
        {
            return value *= a;
        }

        /// <summary>
        /// 返回a/b
        /// </summary>
        public System.UInt32 DivideEquals(System.UInt32 a)
        {
            return value /= a;
        }

        /// <summary>
        /// 返回a%b
        /// </summary>
        public System.UInt32 BaiFeiBi100Equals(System.UInt32 a)
        {
            return value %= a;
        }

        /// <summary>
        /// 返回a^b
        /// </summary>
        public System.UInt32 QiuYuEquals(System.UInt32 a)
        {
            return value ^= a;
        }

        static public System.UInt32 MathOperator(System.UInt32 a, Operator Operator, System.UInt32 b)
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

        static public bool Contitions(System.UInt32 a, Contition contition, System.UInt32 b)
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