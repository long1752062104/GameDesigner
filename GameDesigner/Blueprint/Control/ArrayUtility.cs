namespace GameDesigner.FlowControls
{
    [System.Serializable]
    public class ArrayUtility
    {
        static public object GetValue(System.Array arrays, int index)
        {
            return arrays.GetValue(index);
        }

        static public void SetValue(System.Array arrays, object value, int index)
        {
            arrays.SetValue(value, index);
        }

        static public object CreateInstance(System.Type elementType, int length)
        {
            return (object[])System.Array.CreateInstance(elementType, length);
        }
    }
}