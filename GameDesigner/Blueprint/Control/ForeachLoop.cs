using System;

namespace GameDesigner.FlowControls
{
    public class ForeachLoop
    {
        public static void Foreach(Array array, BlueprintNode elementValue, BlueprintNode runtime)
        {
            ForLoop.For(array, elementValue, runtime);
        }
    }
}