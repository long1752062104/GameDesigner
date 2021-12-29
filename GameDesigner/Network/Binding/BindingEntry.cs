using Net.Share;
using System;
using System.Collections.Generic;

namespace Net.Binding
{
    public static class BindingEntry
    {
        public static Type[] GetBindTypes() 
        {
            List<Type> types = new List<Type>();
            types.Add(typeof(Vector2));
            types.Add(typeof(Vector3));
            types.Add(typeof(Vector4));
            types.Add(typeof(Quaternion));
            //types.Add(typeof(Rect));//此类字段是私有的, 并且实例化属性会出问题, 我已经处理好了
            types.Add(typeof(Color));
            types.Add(typeof(Color32));
            types.Add(typeof(UnityEngine.Vector2));
            types.Add(typeof(UnityEngine.Vector3));
            types.Add(typeof(UnityEngine.Vector4));
            types.Add(typeof(UnityEngine.Quaternion));
            //types.Add(typeof(UnityEngine.Rect));//此类字段是私有的, 并且实例化属性会出问题, 我已经处理好了
            types.Add(typeof(UnityEngine.Color));
            types.Add(typeof(UnityEngine.Color32));
            types.Add(typeof(Operation));
            types.Add(typeof(OperationList));
            return types.ToArray();
        }
    }
}
