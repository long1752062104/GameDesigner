namespace GameDesigner.FlowControls
{
    [System.Serializable]
    public class ForLoop
    {
        /// <summary>
        /// 循环语句,array数组,元素索引:把元素对象发送给elementValue对象,runtime用来判断当前元素的逻辑
        /// </summary>
        static public void For(System.Array arrays, Node elementValue, Node runtime)
        {
            for (int i = 0; i < arrays.Length; i++)
            {
                if (elementValue.method.memberTypes == System.Reflection.MemberTypes.Custom)
                {
                    elementValue.method.target = arrays.GetValue(i);
                    runtime.Invoke();
                    arrays.SetValue(elementValue.method.target, i);
                }
            }
        }

        /// <summary>
        /// 循环语句,array数组,元素索引:把索引发送给elementIndex对象,这个对象必须为int类型,runtime用来判断当前元素的逻辑
        /// </summary>
        static public void For1(System.Array arrays, Node elementIndex, Node runtime)
        {
            for (int i = 0; i < arrays.Length; i++)
            {
                if (elementIndex.method.memberTypes == System.Reflection.MemberTypes.Custom)
                {
                    elementIndex.method.target = i;
                    runtime.Invoke();
                }
            }
        }

        static public void For(System.Array arrays, Node elementValue, Node elementIndex, Node runtime)
        {
            for (int i = 0; i < arrays.Length; i++)
            {
                if (elementValue.method.memberTypes == System.Reflection.MemberTypes.Custom)
                {
                    elementIndex.method.target = i;
                    elementValue.method.target = arrays.GetValue(i);
                    runtime.Invoke();
                }
            }
        }

        /// <summary>
        /// 循环语句,array数组,元素对象:把元素对象发送给elementValue对象,
        /// 元素索引:把索引发送给elementIndex对象
        /// 判断a和b的条件,当条件成立设置元素值为elementValue
        /// </summary>
        static public void For(System.Array arrays, Node elementValue, Node a, Contition condition, Node b, Node trueRun)
        {
            for (int i = 0; i < arrays.Length; i++)
            {
                elementValue.method.target = arrays.GetValue(i);
                a.Invoke();
                b.Invoke();
                if (Branch.IF(a.returnValue, condition, b.returnValue))
                {
                    trueRun.Invoke();
                    arrays.SetValue(elementValue.method.target, i);
                }
            }
        }

        /// <summary>
        /// 循环语句,array数组,元素对象:把元素对象发送给elementValue对象,
        /// 元素索引:把索引发送给elementIndex对象,这个对象必须为int类型,
        /// 判断a和b的条件,当条件成立进入True参数
        /// </summary>
        static public void For(System.Array arrays, Node elementValue, Node elementIndex, object a, Contition contition, object b, Node True)
        {
            for (int i = 0; i < arrays.Length; i++)
            {
                if (elementValue.method.memberTypes == System.Reflection.MemberTypes.Custom)
                {
                    elementIndex.method.target = i;
                    elementValue.method.target = arrays.GetValue(i);
                    Branch.IF(a, contition, b, True);
                }
            }
        }

        /// <summary>
        /// 循环语句,array数组,元素对象:把元素对象发送给elementValue对象,
        /// 元素索引:把索引发送给elementIndex对象,这个对象必须为int类型,
        /// 判断a和b的条件,当条件成立进入True参数,假则加入False
        /// </summary>
        static public void For(System.Array arrays, Node elementValue, Node elementIndex, object a, Contition contition, object b, Node True, Node False)
        {
            for (int i = 0; i < arrays.Length; i++)
            {
                if (elementValue.method.memberTypes == System.Reflection.MemberTypes.Custom)
                {
                    elementIndex.method.target = i;
                    elementValue.method.target = arrays.GetValue(i);
                    Branch.IF(a, contition, b, True, False);
                }
            }
        }

        static public void For(System.Array arrays, int startPoint, Node elementValue, Node elementIndex, Node runtime)
        {
            for (int i = startPoint; i < arrays.Length; i++)
            {
                if (elementValue.method.memberTypes == System.Reflection.MemberTypes.Custom)
                {
                    elementValue.method.target = arrays.GetValue(i);
                    runtime.Invoke();
                }
            }
        }

        static public void For1(System.Array arrays, int length, Node elementValue, Node elementIndex, Node runtime)
        {
            for (int i = 0; i < length; i++)
            {
                if (elementValue.method.memberTypes == System.Reflection.MemberTypes.Custom)
                {
                    elementValue.method.target = arrays.GetValue(i);
                    runtime.Invoke();
                }
            }
        }

        static public void For(System.Array arrays, int startPoint, int lengthPoint, Node elementValue, Node elementIndex, Node runtime)
        {
            for (int i = startPoint; i < lengthPoint; i++)
            {
                if (elementValue.method.memberTypes == System.Reflection.MemberTypes.Custom)
                {
                    elementValue.method.target = arrays.GetValue(i);
                    runtime.Invoke();
                }
            }
        }
    }
}