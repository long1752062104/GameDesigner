namespace GameDesigner.MathOperations
{
    [System.Serializable]
    public class Double
    {
        /// <summary>
        /// 返回a+b
        /// </summary>
        static public double Adds(double a, double b)
        {
            return a + b;
        }

        /// <summary>
        /// 返回a-b
        /// </summary>
        static public double Subtracts(double a, double b)
        {
            return a - b;
        }

        /// <summary>
        /// 返回a*b
        /// </summary>
        static public double Multiplies(double a, double b)
        {
            return a * b;
        }

        /// <summary>
        /// 返回a/b
        /// </summary>
        static public double Divides(double a, double b)
        {
            return a / b;
        }

        /// <summary>
        /// 返回a%b
        /// </summary>
        static public double BaiFeiBi100(double a, double b)
        {
            return a % b;
        }

        /// <summary>
        /// 返回a==b
        /// </summary>
        static public bool Equals(double a, double b)
        {
            return a == b;
        }

        /// <summary>
        /// 返回a!=b
        /// </summary>
        static public bool NotEquals(double a, double b)
        {
            return a != b;
        }

        /// <summary>
        /// 返回a>=b
        /// </summary>
        static public bool MaxEquals(double a, double b)
        {
            return a >= b;
        }

        /// <summary>
        /// 返回a<=b
        /// </summary>
        static public bool MinEquals(double a, double b)
        {
            return a <= b;
        }

        /// <summary>
        /// 返回a>b
        /// </summary>
        static public bool Max(double a, double b)
        {
            return a > b;
        }

        /// <summary>
        /// 返回a<b
        /// </summary>
        static public bool Min(double a, double b)
        {
            return a < b;
        }

        public double value = 1;

        /// <summary>
        /// 返回a++
        /// </summary>
        public double AddAdds()
        {
            return value++;
        }

        /// <summary>
        /// 返回a++
        /// </summary>
        public double AddEquals(double a)
        {
            return value += a;
        }

        /// <summary>
        /// 返回a--
        /// </summary>
        public double SubtractSubtracts()
        {
            return value--;
        }

        /// <summary>
        /// 返回a--
        /// </summary>
        public double SubtractEquals(double a)
        {
            return value -= a;
        }

        /// <summary>
        /// 返回a*b
        /// </summary>
        public double MultiplieEquals(double a)
        {
            return value *= a;
        }

        /// <summary>
        /// 返回a/b
        /// </summary>
        public double DivideEquals(double a)
        {
            return value /= a;
        }

        /// <summary>
        /// 返回a%b
        /// </summary>
        public double BaiFeiBi100Equals(double a)
        {
            return value %= a;
        }

        static public double MathOperator(double a, Operator Operator, double b)
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
            }
            return 0;
        }

        static public bool Contitions(double a, Contition contition, double b)
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