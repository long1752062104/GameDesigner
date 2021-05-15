namespace GameDesigner.MathOperations
{
    [System.Serializable]
    public class Int16
    {
        /// <summary>
        /// 返回a+b
        /// </summary>
        static public object Adds(System.Int16 a, System.Int16 b)
        {
            return a + b;
        }

        /// <summary>
        /// 返回a-b
        /// </summary>
        static public object Subtracts(System.Int16 a, System.Int16 b)
        {
            return a - b;
        }

        /// <summary>
        /// 返回a*b
        /// </summary>
        static public object Multiplies(System.Int16 a, System.Int16 b)
        {
            return a * b;
        }

        /// <summary>
        /// 返回a/b
        /// </summary>
        static public object Divides(System.Int16 a, System.Int16 b)
        {
            return a / b;
        }

        /// <summary>
        /// 返回a%b
        /// </summary>
        static public object BaiFeiBi100(System.Int16 a, System.Int16 b)
        {
            return a % b;
        }

        /// <summary>
        /// 返回a^b
        /// </summary>
        static public object QiuYu(System.Int16 a, System.Int16 b)
        {
            return a ^ b;
        }

        /// <summary>
        /// 返回a&b
        /// </summary>
        static public object BinQie(System.Int16 a, System.Int16 b)
        {
            return a & b;
        }

        /// <summary>
        /// 返回a==b
        /// </summary>
        static public bool Equals(System.Int16 a, System.Int16 b)
        {
            return a == b;
        }

        /// <summary>
        /// 返回a!=b
        /// </summary>
        static public bool NotEquals(System.Int16 a, System.Int16 b)
        {
            return a != b;
        }

        /// <summary>
        /// 返回a>=b
        /// </summary>
        static public bool MaxEquals(System.Int16 a, System.Int16 b)
        {
            return a >= b;
        }

        /// <summary>
        /// 返回a<=b
        /// </summary>
        static public bool MinEquals(System.Int16 a, System.Int16 b)
        {
            return a <= b;
        }

        /// <summary>
        /// 返回a>b
        /// </summary>
        static public bool Max(System.Int16 a, System.Int16 b)
        {
            return a > b;
        }

        /// <summary>
        /// 返回a<b
        /// </summary>
        static public bool Min(System.Int16 a, System.Int16 b)
        {
            return a < b;
        }

        public System.Int16 value = 1;

        /// <summary>
        /// 返回a++
        /// </summary>
        public System.Int16 AddAdds()
        {
            return value++;
        }

        /// <summary>
        /// 返回a++
        /// </summary>
        public System.Int16 AddEquals(System.Int16 a)
        {
            return value += a;
        }

        /// <summary>
        /// 返回a--
        /// </summary>
        public System.Int16 SubtractSubtracts()
        {
            return value--;
        }

        /// <summary>
        /// 返回a--
        /// </summary>
        public System.Int16 SubtractEquals(System.Int16 a)
        {
            return value -= a;
        }

        /// <summary>
        /// 返回a*b
        /// </summary>
        public System.Int16 MultiplieEquals(System.Int16 a)
        {
            return value *= a;
        }

        /// <summary>
        /// 返回a/b
        /// </summary>
        public System.Int16 DivideEquals(System.Int16 a)
        {
            return value /= a;
        }

        /// <summary>
        /// 返回a%b
        /// </summary>
        public System.Int16 BaiFeiBi100Equals(System.Int16 a)
        {
            return value %= a;
        }

        /// <summary>
        /// 返回a^b
        /// </summary>
        public System.Int16 QiuYuEquals(System.Int16 a)
        {
            return value ^= a;
        }

        static public object MathOperator(System.Int16 a, Operator Operator, System.Int16 b)
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

        static public bool Contitions(System.Int16 a, Contition contition, System.Int16 b)
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