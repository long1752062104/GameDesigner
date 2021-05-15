namespace GameDesigner.MathOperations
{
    [System.Serializable]
    public class SByte
    {
        /// <summary>
        /// 返回a+b
        /// </summary>
        static public object Adds(sbyte a, sbyte b)
        {
            return a + b;
        }

        /// <summary>
        /// 返回a-b
        /// </summary>
        static public object Subtracts(sbyte a, sbyte b)
        {
            return a - b;
        }

        /// <summary>
        /// 返回a*b
        /// </summary>
        static public object Multiplies(sbyte a, sbyte b)
        {
            return a * b;
        }

        /// <summary>
        /// 返回a/b
        /// </summary>
        static public object Divides(sbyte a, sbyte b)
        {
            return a / b;
        }

        /// <summary>
        /// 返回a%b
        /// </summary>
        static public object BaiFeiBi100(sbyte a, sbyte b)
        {
            return a % b;
        }

        /// <summary>
        /// 返回a^b
        /// </summary>
        static public object QiuYu(sbyte a, sbyte b)
        {
            return a ^ b;
        }

        /// <summary>
        /// 返回a&b
        /// </summary>
        static public object BinQie(sbyte a, sbyte b)
        {
            return a & b;
        }

        /// <summary>
        /// 返回a==b
        /// </summary>
        static public bool Equals(sbyte a, sbyte b)
        {
            return a == b;
        }

        /// <summary>
        /// 返回a!=b
        /// </summary>
        static public bool NotEquals(sbyte a, sbyte b)
        {
            return a != b;
        }

        /// <summary>
        /// 返回a>=b
        /// </summary>
        static public bool MaxEquals(sbyte a, sbyte b)
        {
            return a >= b;
        }

        /// <summary>
        /// 返回a<=b
        /// </summary>
        static public bool MinEquals(sbyte a, sbyte b)
        {
            return a <= b;
        }

        /// <summary>
        /// 返回a>b
        /// </summary>
        static public bool Max(sbyte a, sbyte b)
        {
            return a > b;
        }

        /// <summary>
        /// 返回a<b
        /// </summary>
        static public bool Min(sbyte a, sbyte b)
        {
            return a < b;
        }

        public sbyte value = 1;

        /// <summary>
        /// 返回a++
        /// </summary>
        public sbyte AddAdds()
        {
            return value++;
        }

        /// <summary>
        /// 返回a++
        /// </summary>
        public sbyte AddEquals(sbyte a)
        {
            return value += a;
        }

        /// <summary>
        /// 返回a--
        /// </summary>
        public sbyte SubtractSubtracts()
        {
            return value--;
        }

        /// <summary>
        /// 返回a--
        /// </summary>
        public sbyte SubtractEquals(sbyte a)
        {
            return value -= a;
        }

        /// <summary>
        /// 返回a*b
        /// </summary>
        public sbyte MultiplieEquals(sbyte a)
        {
            return value *= a;
        }

        /// <summary>
        /// 返回a/b
        /// </summary>
        public sbyte DivideEquals(sbyte a)
        {
            return value /= a;
        }

        /// <summary>
        /// 返回a%b
        /// </summary>
        public sbyte BaiFeiBi100Equals(sbyte a)
        {
            return value %= a;
        }

        /// <summary>
        /// 返回a^b
        /// </summary>
        public sbyte QiuYuEquals(sbyte a)
        {
            return value ^= a;
        }

        static public object MathOperator(sbyte a, Operator Operator, sbyte b)
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

        static public bool Contitions(sbyte a, Contition contition, sbyte b)
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