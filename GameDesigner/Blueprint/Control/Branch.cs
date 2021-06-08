using GameDesigner.MathOperations;

namespace GameDesigner.FlowControls
{
    [System.Serializable]
    public class Branch
    {
        /// <summary>
        /// 如果控制语句函数---v2017.12.17
        /// </summary>

        static public bool IF(object a, Contition contition, object b)
        {
            if (Condition(a, contition, b))
                return true;
            return false;
        }

        /// <summary>
        /// 如果控制语句函数---v2017.12.17
        /// </summary>

        static public void IF(bool contition, Node True)
        {
            if (contition)
            {
                True.Invoke();
            }
        }

        /// <summary>
        /// 如果控制语句函数---v2017.12.17
        /// </summary>

        static public void IF(bool contition, Node True, Node False)
        {
            if (contition)
            {
                True.Invoke();
            }
            else
            {
                False.Invoke();
            }
        }

        /// <summary>
        /// 如果控制语句函数---v2017.12.17
        /// </summary>

        static public void IF(object a, Contition contition, object b, Node True)
        {
            if (Condition(a, contition, b))
            {
                True.Invoke();
            }
        }

        /// <summary>
        /// 如果控制语句函数---v2017.12.17
        /// </summary>

        static public void IF(object a, Contition contition, object b, Node True, Node False)
        {
            if (Condition(a, contition, b))
            {
                True.Invoke();
            }
            else
            {
                False.Invoke();
            }
        }

        static public bool Or(bool a, bool b)
        {
            if (a | b)
                return true;
            return false;
        }

        static public bool Or(bool a, bool b, bool c)
        {
            if (a | b | c)
                return true;
            return false;
        }

        static public bool Or(bool a, bool b, bool c, bool d)
        {
            if (a | b | c | d)
                return true;
            return false;
        }

        static public bool Or(bool a, bool b, bool c, bool d, bool e)
        {
            if (a | b | c | d | e)
                return true;
            return false;
        }

        static public bool And(bool a, bool b)
        {
            if (a & b)
                return true;
            return false;
        }

        static public bool And(bool a, bool b, bool c)
        {
            if (a & b & c)
                return true;
            return false;
        }

        static public bool And(bool a, bool b, bool c, bool d)
        {
            if (a & b & c & d)
                return true;
            return false;
        }

        static public bool And(bool a, bool b, bool c, bool d, bool e)
        {
            if (a & b & c & d & e)
                return true;
            return false;
        }

        /// <summary>
        /// 判断参数值---v2017.7.15
        /// </summary>

        static private bool Condition(object a, Contition contition, object b)
        {
            if (a == null)
                return false;
            if (contition == Contition.Equals | contition == Contition.NotEquals)
                return ConditionOther(a, contition, b);
            if (a is int & b is int)
                return Int32.Contitions((int)a, contition, (int)b);
            if (a is float & b is float)
                return Float.Contitions((float)a, contition, (float)b);
            if (a is double & b is double)
                return Double.Contitions((double)a, contition, (double)b);
            if (a is double & b is byte)
                return Byte.Contitions((byte)a, contition, (byte)b);
            if (a is double & b is sbyte)
                return SByte.Contitions((sbyte)a, contition, (sbyte)b);
            if (a is System.Int16 & b is System.Int16)
                return Int16.Contitions((System.Int16)a, contition, (System.Int16)b);
            if (a is System.Int64 & b is System.Int64)
                return Int64.Contitions((System.Int64)a, contition, (System.Int64)b);
            if (a is System.UInt16 & b is System.UInt16)
                return UInt16.Contitions((System.UInt16)a, contition, (System.UInt16)b);
            if (a is System.UInt32 & b is System.UInt32)
                return UInt32.Contitions((System.UInt32)a, contition, (System.UInt32)b);
            if (a is System.UInt64 & b is System.UInt64)
                return UInt64.Contitions((System.UInt64)a, contition, (System.UInt64)b);
            return false;
        }

        static private bool ConditionOther(object a, Contition contition, object b)
        {
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