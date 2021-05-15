namespace GameDesigner.MathOperations
{
    [System.Serializable]
    public class Byte
    {
        /// <summary>
        /// 返回a+b
        /// </summary>
        static public object Adds(byte a, byte b)
        {
            return a + b;
        }

        /// <summary>
        /// 返回a-b
        /// </summary>
        static public object Subtracts(byte a, byte b)
        {
            return a - b;
        }

        /// <summary>
        /// 返回a*b
        /// </summary>
        static public object Multiplies(byte a, byte b)
        {
            return a * b;
        }

        /// <summary>
        /// 返回a/b
        /// </summary>
        static public object Divides(byte a, byte b)
        {
            return a / b;
        }

        /// <summary>
        /// 返回a%b
        /// </summary>
        static public object BaiFeiBi100(byte a, byte b)
        {
            return a % b;
        }

        /// <summary>
        /// 返回a^b
        /// </summary>
        static public object QiuYu(byte a, byte b)
        {
            return a ^ b;
        }

        /// <summary>
        /// 返回a&b
        /// </summary>
        static public object BinQie(byte a, byte b)
        {
            return a & b;
        }

        /// <summary>
        /// 返回a==b
        /// </summary>
        static public bool Equals(byte a, byte b)
        {
            return a == b;
        }

        /// <summary>
        /// 返回a!=b
        /// </summary>
        static public bool NotEquals(byte a, byte b)
        {
            return a != b;
        }

        /// <summary>
        /// 返回a>=b
        /// </summary>
        static public bool MaxEquals(byte a, byte b)
        {
            return a >= b;
        }

        /// <summary>
        /// 返回a<=b
        /// </summary>
        static public bool MinEquals(byte a, byte b)
        {
            return a <= b;
        }

        /// <summary>
        /// 返回a>b
        /// </summary>
        static public bool Max(byte a, byte b)
        {
            return a > b;
        }

        /// <summary>
        /// 返回a<b
        /// </summary>
        static public bool Min(byte a, byte b)
        {
            return a < b;
        }

        public byte value = 1;

        /// <summary>
        /// 返回a++
        /// </summary>
        public byte AddAdds()
        {
            return value++;
        }

        /// <summary>
        /// 返回a++
        /// </summary>
        public byte AddEquals(byte a)
        {
            return value += a;
        }

        /// <summary>
        /// 返回a--
        /// </summary>
        public byte SubtractSubtracts()
        {
            return value--;
        }

        /// <summary>
        /// 返回a--
        /// </summary>
        public byte SubtractEquals(byte a)
        {
            return value -= a;
        }

        /// <summary>
        /// 返回a*b
        /// </summary>
        public byte MultiplieEquals(byte a)
        {
            return value *= a;
        }

        /// <summary>
        /// 返回a/b
        /// </summary>
        public byte DivideEquals(byte a)
        {
            return value /= a;
        }

        /// <summary>
        /// 返回a%b
        /// </summary>
        public byte BaiFeiBi100Equals(byte a)
        {
            return value %= a;
        }

        /// <summary>
        /// 返回a^b
        /// </summary>
        public byte QiuYuEquals(byte a)
        {
            return value ^= a;
        }

        static public object MathOperator(byte a, Operator Operator, byte b)
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

        static public bool Contitions(byte a, Contition contition, byte b)
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