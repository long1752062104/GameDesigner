using System;

namespace GameDesigner.FlowControls
{
    public class ForeachLoop
    {
        public static void Foreach(Array array, Node elementValue, Node runtime)
        {
            ForLoop.For(array, elementValue, runtime);
        }
    }
}