using GameDesigner.MathOperations;

namespace GameDesigner.FlowControls
{
    [System.Serializable]
    public class Operators
    {
        static public object MathOperator(object a, Operator Operator, object b)
        {
            if (a is int)
            {
                return Int32.MathOperator((int)a, Operator, (int)b);
            }
            else if (a is float)
            {
                return Float.MathOperator((float)a, Operator, (float)b);
            }
            else if (a is System.Int16)
            {
                return Int16.MathOperator((System.Int16)a, Operator, (System.Int16)b);
            }
            else if (a is System.Int64)
            {
                return Int64.MathOperator((System.Int64)a, Operator, (System.Int64)b);
            }
            else if (a is System.UInt16)
            {
                return UInt16.MathOperator((System.UInt16)a, Operator, (System.UInt16)b);
            }
            else if (a is System.UInt32)
            {
                return UInt32.MathOperator((System.UInt32)a, Operator, (System.UInt32)b);
            }
            else if (a is System.UInt64)
            {
                return UInt64.MathOperator((System.UInt64)a, Operator, (System.UInt64)b);
            }
            else if (a is double)
            {
                return Double.MathOperator((double)a, Operator, (double)b);
            }
            else if (a is byte)
            {
                return Byte.MathOperator((byte)a, Operator, (byte)b);
            }
            else if (a is sbyte)
            {
                return SByte.MathOperator((sbyte)a, Operator, (sbyte)b);
            }
            return a.ToString() + b.ToString();
        }

        static public bool Contitions(object a, Contition contition, object b)
        {
            if (a is int)
            {
                return Int32.Contitions((int)a, contition, (int)b);
            }
            else if (a is float)
            {
                return Float.Contitions((float)a, contition, (float)b);
            }
            else if (a is System.Int16)
            {
                return Int16.Contitions((System.Int16)a, contition, (System.Int16)b);
            }
            else if (a is System.Int64)
            {
                return Int64.Contitions((System.Int64)a, contition, (System.Int64)b);
            }
            else if (a is System.UInt16)
            {
                return UInt16.Contitions((System.UInt16)a, contition, (System.UInt16)b);
            }
            else if (a is System.UInt32)
            {
                return UInt32.Contitions((System.UInt32)a, contition, (System.UInt32)b);
            }
            else if (a is System.UInt64)
            {
                return UInt64.Contitions((System.UInt64)a, contition, (System.UInt64)b);
            }
            else if (a is double)
            {
                return Double.Contitions((double)a, contition, (double)b);
            }
            else if (a is byte)
            {
                return Byte.Contitions((byte)a, contition, (byte)b);
            }
            else if (a is sbyte)
            {
                return SByte.Contitions((sbyte)a, contition, (sbyte)b);
            }
            switch (contition)
            {
                case Contition.Equals:
                    return a.Equals(b);
                case Contition.NotEquals:
                    return !a.Equals(b);
            }
            return false;
        }
    }
}