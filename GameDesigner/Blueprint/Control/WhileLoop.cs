using UnityEngine;

namespace GameDesigner.FlowControls
{
    public class WhileLoop
    {
        public static void While(bool condition, Node runtime)
        {
            while (!condition)
            {
                runtime.Invoke();
            }
        }

        public static void LimitWhile(bool condition, Node runtime)
        {
            int i = 0;//为了确保是死循环模式的措施
            while (!condition)
            {
                runtime.Invoke();
                i++;
                if (i > 10000)
                {//如果循环一万次还没有退出则自动退出
                    Debug.Log("死循环!");
                    break;
                }
            }
        }
    }
}