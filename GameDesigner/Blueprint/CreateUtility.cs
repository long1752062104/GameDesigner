using System;

namespace GameDesigner
{
    [Serializable]
    public class CreateUtility
    {
        /// <summary>
        /// 创建类对象实例
        /// </summary>
        static public object CreateInstance(string typeName)
        {
            Type type = SystemType.GetType(typeName);
            return CreateInstance(type);
        }

        /// <summary>
        /// 创建类对象实例
        /// </summary>
        static public object CreateInstance(Type type)
        {
            try
            {
                object obj = Activator.CreateInstance(type);
                if (obj != null)
                    return obj;
                return type;
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 创建类对象实例
        /// </summary>
        static public object CreateInstance(Type typeName, object[] args)
        {
            try
            {
                object obj = Activator.CreateInstance(typeName, args);
                if (obj != null)
                    return obj;
                return typeName;
            }
            catch { }
            return null;
        }
    }
}