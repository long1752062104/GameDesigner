using UnityEngine;

namespace GameDesigner.MathOperations
{
    [System.Serializable]
    public class Float
    {
        /// <summary>
        /// 返回a+b
        /// </summary>
        static public float Adds(float a, float b)
        {
            return a + b;
        }

        /// <summary>
        /// 返回a-b
        /// </summary>
        static public float Subtracts(float a, float b)
        {
            return a - b;
        }

        /// <summary>
        /// 返回a*b
        /// </summary>
        static public float Multiplies(float a, float b)
        {
            return a * b;
        }

        /// <summary>
        /// 返回a/b
        /// </summary>
        static public float Divides(float a, float b)
        {
            return a / b;
        }

        /// <summary>
        /// 返回a%b
        /// </summary>
        static public float BaiFeiBi100(float a, float b)
        {
            return a % b;
        }

        /// <summary>
        /// 返回a==b
        /// </summary>
        static public bool Equals(float a, float b)
        {
            return a == b;
        }

        /// <summary>
        /// 返回a!=b
        /// </summary>
        static public bool NotEquals(float a, float b)
        {
            return a != b;
        }

        /// <summary>
        /// 返回a>=b
        /// </summary>
        static public bool MaxEquals(float a, float b)
        {
            return a >= b;
        }

        /// <summary>
        /// 返回a<=b
        /// </summary>
        static public bool MinEquals(float a, float b)
        {
            return a <= b;
        }

        /// <summary>
        /// 返回a>b
        /// </summary>
        static public bool Max(float a, float b)
        {
            return a > b;
        }

        /// <summary>
        /// 返回a<b
        /// </summary>
        static public bool Min(float a, float b)
        {
            return a < b;
        }

        public float value = 0;

        /// <summary>
        /// 返回a++
        /// </summary>
        public float AddAdds()
        {
            return value++;
        }

        /// <summary>
        /// 返回a++
        /// </summary>
        public float AddEquals(float a)
        {
            return value += a;
        }

        /// <summary>
        /// 返回a++
        /// </summary>
        public bool AddEquals(float timeMax, bool zero)
        {
            if (value > timeMax)
            {
                if (zero)
                    value = 0;
                return true;
            }
            value += Time.deltaTime;
            return false;
        }

        /// <summary>
        /// 返回a++
        /// </summary>
        public bool AddEquals(float a, float timeMax, bool zero)
        {
            if (value > timeMax)
            {
                if (zero)
                    value = 0;
                return true;
            }
            value += a;
            return false;
        }

        /// <summary>
        /// 返回a--
        /// </summary>
        public float SubtractSubtracts()
        {
            return value--;
        }

        /// <summary>
        /// 返回a--
        /// </summary>
        public float SubtractEquals(float a)
        {
            return value -= a;
        }

        /// <summary>
        /// 返回a*b
        /// </summary>
        public float MultiplieEquals(float a)
        {
            return value *= a;
        }

        /// <summary>
        /// 返回a/b
        /// </summary>
        public float DivideEquals(float a)
        {
            return value /= a;
        }

        /// <summary>
        /// 返回a%b
        /// </summary>
        public float BaiFeiBi100Equals(float a)
        {
            return value %= a;
        }

        static public float MathOperator(float a, Operator Operator, float b)
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

        static public bool Contitions(float a, Contition contition, float b)
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