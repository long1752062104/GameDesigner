namespace Net
{
    using Net.Share;
    using System.Reflection;

    /// <summary>
    /// 克隆工具类
    /// </summary>
    public sealed class Clone
    {
        /// <summary>
        /// 克隆对象, 脱离引用对象的地址
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static T Instance<T>(object target) where T : class
        {
            var buffer = NetConvertBinary.SerializeComplex(target);
            return (T)NetConvertBinary.DeserializeComplex(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 克隆对象, 脱离引用对象的地址
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static T Copy<T>(object target) where T : class
        {
            var buffer = NetConvertBinary.SerializeComplex(target);
            object obj = NetConvertBinary.DeserializeComplex(buffer, 0, buffer.Length);
            T obj1 = System.Activator.CreateInstance<T>();
            System.Type type1 = target.GetType();
            FieldInfo[] fields = type1.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(obj);
                if (value != null)
                    field.SetValue(obj1, value);
            }
            return obj1;
        }
    }
}
