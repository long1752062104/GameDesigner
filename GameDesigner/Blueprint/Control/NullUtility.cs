using UnityEngine;

namespace GameDesigner.FlowControls
{
    public class NullUtility
    {
        public static bool TargetIsNull(object target)
        {
            return target == null;
        }

        public static bool TargetIsNull(Object target)
        {
            return target == null;
        }

        public static bool ObjectIsNull(Object target)
        {
            //System.Convert.IsDBNull
            return target == null;
        }

        public static bool IsInTarget(object target)
        {
            return target != null;
        }

        public static bool IsInTarget(Object target)
        {
            return target != null;
        }

        public static bool IsInObject(Object target)
        {
            return target != null;
        }
    }
}