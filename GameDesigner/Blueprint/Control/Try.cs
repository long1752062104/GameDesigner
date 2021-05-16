using UnityEngine;

namespace GameDesigner.FlowControls
{
    public class Try
    {
        public static void Trys(BlueprintNode tryRuntime)
        {
            try
            {
                tryRuntime.Invoke();
            }
            catch (System.Exception e) { Debug.Log(e); }
        }
    }
}