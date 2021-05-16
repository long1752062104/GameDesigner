namespace GameDesigner.MathOperations
{
    [System.Serializable]
    public class Int64
    {
        /// <summary>
        /// 返回a+b
        /// </summary>
        static public System.Int64 Adds(System.Int64 a, System.Int64 b)
        {
            return a + b;
        }

        /// <summary>
        /// 返回a-b
        /// </summary>
        static public System.Int64 Subtracts(System.Int64 a, System.Int64 b)
        {
            return a - b;
        }

        /// <summary>
        /// 返回a*b
        /// </summary>
        static public System.Int64 Multiplies(System.Int64 a, System.Int64 b)
        {
            return a * b;
        }

        /// <summary>
        /// 返回a/b
        /// </summary>
        static public System.Int64 Divides(System.Int64 a, System.Int64 b)
        {
            return a / b;
        }

        /// <summary>
        /// 返回a%b
        /// </summary>
        static public System.Int64 BaiFeiBi100(System.Int64 a, System.Int64 b)
        {
            return a % b;
        }

        /// <summary>
        /// 返回a^b
        /// </summary>
        static public System.Int64 QiuYu(System.Int64 a, System.Int64 b)
        {
            return a ^ b;
        }

        /// <summary>
        /// 返回a&b
        /// </summary>
        static public System.Int64 BinQie(System.Int64 a, System.Int64 b)
        {
            return a & b;
        }

        /// <summary>
        /// 返回a==b
        /// </summary>
        static public bool Equals(System.Int64 a, System.Int64 b)
        {
            return a == b;
        }

        /// <summary>
        /// 返回a!=b
        /// </summary>
        static public bool NotEquals(System.Int64 a, System.Int64 b)
        {
            return a != b;
        }

        /// <summary>
        /// 返回a>=b
        /// </summary>
        static public bool MaxEquals(System.Int64 a, System.Int64 b)
        {
            return a >= b;
        }

        /// <summary>
        /// 返回a<=b
        /// </summary>
        static public bool MinEquals(System.Int64 a, System.Int64 b)
        {
            return a <= b;
        }

        /// <summary>
        /// 返回a>b
        /// </summary>
        static public bool Max(System.Int64 a, System.Int64 b)
        {
            return a > b;
        }

        /// <summary>
        /// 返回a<b
        /// </summary>
        static public bool Min(System.Int64 a, System.Int64 b)
        {
            return a < b;
        }

        public System.Int64 value = 1;

        /// <summary>
        /// 返回a++
        /// </summary>
        public System.Int64 AddAdds()
        {
            return value++;
        }

        /// <summary>
        /// 返回a++
        /// </summary>
        public System.Int64 AddEquals(System.Int64 a)
        {
            return value += a;
        }

        /// <summary>
        /// 返回a--
        /// </summary>
        public System.Int64 SubtractSubtracts()
        {
            return value--;
        }

        /// <summary>
        /// 返回a--
        /// </summary>
        public System.Int64 SubtractEquals(System.Int64 a)
        {
            return value -= a;
        }

        /// <summary>
        /// 返回a*b
        /// </summary>
        public System.Int64 MultiplieEquals(System.Int64 a)
        {
            return value *= a;
        }

        /// <summary>
        /// 返回a/b
        /// </summary>
        public System.Int64 DivideEquals(System.Int64 a)
        {
            return value /= a;
        }

        /// <summary>
        /// 返回a%b
        /// </summary>
        public System.Int64 BaiFeiBi100Equals(System.Int64 a)
        {
            return value %= a;
        }

        /// <summary>
        /// 返回a^b
        /// </summary>
        public System.Int64 QiuYuEquals(System.Int64 a)
        {
            return value ^= a;
        }

        static public System.Int64 MathOperator(System.Int64 a, Operator Operator, System.Int64 b)
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

        static public bool Contitions(System.Int64 a, Contition contition, System.Int64 b)
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