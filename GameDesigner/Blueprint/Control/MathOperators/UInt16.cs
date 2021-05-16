namespace GameDesigner.MathOperations
{
    [System.Serializable]
    public class UInt16
    {
        /// <summary>
        /// 返回a+b
        /// </summary>
        static public object Adds(System.UInt16 a, System.UInt16 b)
        {
            return a + b;
        }

        /// <summary>
        /// 返回a-b
        /// </summary>
        static public object Subtracts(System.UInt16 a, System.UInt16 b)
        {
            return a - b;
        }

        /// <summary>
        /// 返回a*b
        /// </summary>
        static public object Multiplies(System.UInt16 a, System.UInt16 b)
        {
            return a * b;
        }

        /// <summary>
        /// 返回a/b
        /// </summary>
        static public object Divides(System.UInt16 a, System.UInt16 b)
        {
            return a / b;
        }

        /// <summary>
        /// 返回a%b
        /// </summary>
        static public object BaiFeiBi100(System.UInt16 a, System.UInt16 b)
        {
            return a % b;
        }

        /// <summary>
        /// 返回a^b
        /// </summary>
        static public object QiuYu(System.UInt16 a, System.UInt16 b)
        {
            return a ^ b;
        }

        /// <summary>
        /// 返回a&b
        /// </summary>
        static public object BinQie(System.UInt16 a, System.UInt16 b)
        {
            return a & b;
        }

        /// <summary>
        /// 返回a==b
        /// </summary>
        static public bool Equals(System.UInt16 a, System.UInt16 b)
        {
            return a == b;
        }

        /// <summary>
        /// 返回a!=b
        /// </summary>
        static public bool NotEquals(System.UInt16 a, System.UInt16 b)
        {
            return a != b;
        }

        /// <summary>
        /// 返回a>=b
        /// </summary>
        static public bool MaxEquals(System.UInt16 a, System.UInt16 b)
        {
            return a >= b;
        }

        /// <summary>
        /// 返回a<=b
        /// </summary>
        static public bool MinEquals(System.UInt16 a, System.UInt16 b)
        {
            return a <= b;
        }

        /// <summary>
        /// 返回a>b
        /// </summary>
        static public bool Max(System.UInt16 a, System.UInt16 b)
        {
            return a > b;
        }

        /// <summary>
        /// 返回a<b
        /// </summary>
        static public bool Min(System.UInt16 a, System.UInt16 b)
        {
            return a < b;
        }

        public System.UInt16 value = 1;

        /// <summary>
        /// 返回a++
        /// </summary>
        public System.UInt16 AddAdds()
        {
            return value++;
        }

        /// <summary>
        /// 返回a++
        /// </summary>
        public System.UInt16 AddEquals(System.UInt16 a)
        {
            return value += a;
        }

        /// <summary>
        /// 返回a--
        /// </summary>
        public System.UInt16 SubtractSubtracts()
        {
            return value--;
        }

        /// <summary>
        /// 返回a--
        /// </summary>
        public System.UInt16 SubtractEquals(System.UInt16 a)
        {
            return value -= a;
        }

        /// <summary>
        /// 返回a*b
        /// </summary>
        public System.UInt16 MultiplieEquals(System.UInt16 a)
        {
            return value *= a;
        }

        /// <summary>
        /// 返回a/b
        /// </summary>
        public System.UInt16 DivideEquals(System.UInt16 a)
        {
            return value /= a;
        }

        /// <summary>
        /// 返回a%b
        /// </summary>
        public System.UInt16 BaiFeiBi100Equals(System.UInt16 a)
        {
            return value %= a;
        }

        /// <summary>
        /// 返回a^b
        /// </summary>
        public System.UInt16 QiuYuEquals(System.UInt16 a)
        {
            return value ^= a;
        }

        static public object MathOperator(System.UInt16 a, Operator Operator, System.UInt16 b)
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

        static public bool Contitions(System.UInt16 a, Contition contition, System.UInt16 b)
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