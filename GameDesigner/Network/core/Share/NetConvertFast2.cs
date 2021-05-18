namespace Net.Share
{
    using Net.Event;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// 快速序列化2接口
    /// </summary>
    public interface ISerialize<T>
    {
        /// <summary>
        /// 序列化写入
        /// </summary>
        /// <param name="value"></param>
        /// <param name="strem"></param>
        void Write(T value, Segment strem);
        /// <summary>
        /// 反序列化读取
        /// </summary>
        /// <param name="strem"></param>
        T Read(Segment strem);
    }

    /// <summary>
    /// 快速序列化2版本
    /// </summary>
    public class NetConvertFast2 : NetConvertBase
    {
        private static MyDictionary<ushort, Type> Types = new MyDictionary<ushort, Type>();
        private static MyDictionary<Type, TypeBind> Types1 = new MyDictionary<Type, TypeBind>();
        private static readonly MyDictionary<Type, Type> BindTypes = new MyDictionary<Type, Type>();

        static NetConvertFast2()
        {
            GetInterfaces();
            Init();
        }

        /// <summary>
        /// 初始化网络转换类型
        /// </summary>
        public static bool Init()
        {
            Types = new MyDictionary<ushort, Type>();
            Types1 = new MyDictionary<Type, TypeBind>();
            AddBaseType();
            return true;
        }

        /// <summary>
        /// 添加网络基本类型， int，float，bool，string......
        /// </summary>
        public static void AddBaseType()
        {
            AddBaseType<short>();
            AddBaseType<int>();
            AddBaseType<long>();
            AddBaseType<ushort>();
            AddBaseType<uint>();
            AddBaseType<ulong>();
            AddBaseType<float>();
            AddBaseType<double>();
            AddBaseType<bool>();
            AddBaseType<char>();
            AddBaseType<string>();
            AddBaseType<byte>();
            AddBaseType<sbyte>();
            AddBaseType<DateTime>();
            AddBaseType<decimal>();
            AddBaseType<DBNull>();
            //基础序列化数组
            AddBaseType<short[]>();
            AddBaseType<int[]>();
            AddBaseType<long[]>();
            AddBaseType<ushort[]>();
            AddBaseType<uint[]>();
            AddBaseType<ulong[]>();
            AddBaseType<float[]>();
            AddBaseType<double[]>();
            AddBaseType<bool[]>();
            AddBaseType<char[]>();
            AddBaseType<string[]>();
            AddBaseType<byte[]>();
            AddBaseType<sbyte[]>();
            AddBaseType<DateTime[]>();
            AddBaseType<decimal[]>();
            //基础序列化List
            AddBaseType<List<short>>();
            AddBaseType<List<int>>();
            AddBaseType<List<long>>();
            AddBaseType<List<ushort>>();
            AddBaseType<List<uint>>();
            AddBaseType<List<ulong>>();
            AddBaseType<List<float>>();
            AddBaseType<List<double>>();
            AddBaseType<List<bool>>();
            AddBaseType<List<char>>();
            AddBaseType<List<string>>();
            AddBaseType<List<byte>>();
            AddBaseType<List<sbyte>>();
            AddBaseType<List<DateTime>>();
            AddBaseType<List<decimal>>();
            //基础序列化List
            AddBaseType<List<short[]>>();
            AddBaseType<List<int[]>>();
            AddBaseType<List<long[]>>();
            AddBaseType<List<ushort[]>>();
            AddBaseType<List<uint[]>>();
            AddBaseType<List<ulong[]>>();
            AddBaseType<List<float[]>>();
            AddBaseType<List<double[]>>();
            AddBaseType<List<bool[]>>();
            AddBaseType<List<char[]>>();
            AddBaseType<List<string[]>>();
            AddBaseType<List<byte[]>>();
            AddBaseType<List<sbyte[]>>();
            AddBaseType<List<DateTime[]>>();
            AddBaseType<List<decimal[]>>();
            //其他可能用到的
            AddSerializeType<Vector2>();
            AddSerializeType<Vector3>();
            AddSerializeType<Vector4>();
            AddSerializeType<Quaternion>();
            AddSerializeType<Rect>();
            AddSerializeType<Color>();
            AddSerializeType<Color32>();
            AddSerializeType<UnityEngine.Vector2>();
            AddSerializeType<UnityEngine.Vector3>();
            AddSerializeType<UnityEngine.Vector4>();
            AddSerializeType<UnityEngine.Quaternion>();
            AddSerializeType<UnityEngine.Rect>();
            AddSerializeType<UnityEngine.Color>();
            AddSerializeType<UnityEngine.Color32>();
            //框架操作同步用到
            AddSerializeType<Operation>();
            AddSerializeType<Operation[]>();
            AddSerializeType<OperationList>();
        }

        /// <summary>
        /// 添加可序列化的参数类型, 网络参数类型 如果不进行添加将不会被序列化,反序列化
        /// </summary>
        /// <typeparam name="T">要添加的网络类型</typeparam>
        public static void AddSerializeType<T>()
        {
            AddSerializeType(typeof(T));
        }

        /// <summary>
        /// 添加可序列化的参数类型, 网络参数类型 如果不进行添加将不会被序列化,反序列化
        /// </summary>
        /// <param name="type">要添加的网络类型</param>
        public static void AddSerializeType(Type type)
        {
            if (Types1.ContainsKey(type))
                throw new Exception($"已经添加{type}键，不需要添加了!");
            if (!BindTypes.TryGetValue(type, out Type bindType))
                throw new Exception($"类型{type}尚未实现绑定类型,请使用工具生成绑定类型!");
            Types.Add((ushort)Types.Count, type);
            Types1.Add(type, new TypeBind() { type = bindType, hashCode = (ushort)Types1.Count } );
        }

        private static void AddBaseType<T>()
        {
            var type = typeof(T);
            if (Types1.ContainsKey(type))
                return;
            Types.Add((ushort)Types.Count, type);
            Types1.Add(type, new TypeBind() { type = typeof(BaseBind<T>), hashCode = (ushort)Types1.Count });
        }

        public static void GetInterfaces()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var types = assembly.GetTypes().Where((t)=> { return t.GetInterface("Net.Share.ISerialize`1") != null; });
                foreach (var type in types)
                {
                    var serType = type.GetInterface("Net.Share.ISerialize`1");
                    var itemType = serType.GetGenericArguments()[0];
                    BindTypes.Add(itemType, type);
                }
            }
        }

        /// <summary>
        /// 添加可序列化的参数类型, 网络参数类型 如果不进行添加将不会被序列化,反序列化
        /// </summary>
        /// <param name="types"></param>
        public static void AddSerializeType(params Type[] types)
        {
            foreach (Type type in types)
            {
                AddSerializeType(type);
            }
        }

        internal struct BaseBind<T> : ISerialize<T>
        {
            public void Write(T value, Segment strem)
            {
                strem.WriteValue(value);
            }
            public T Read(Segment strem)
            {
                return strem.ReadValue<T>();
            }
        }

        private class TypeBind 
        {
            public Type type;
            public ushort hashCode;
        }

        public static Segment SerializeObject<T>(T value)
        {
            var stream = BufferPool.Take();
            try
            {
                Type type = value.GetType();
                if(Types1.TryGetValue(type, out TypeBind typeBind))
                {
                    var bind = (ISerialize<T>)Activator.CreateInstance(typeBind.type);
                    bind.Write(value, stream);
                }
                else throw new Exception($"请注册或绑定:{type}类型后才能序列化!");
            }
            catch (Exception ex)
            {
                NDebug.LogError("序列化异常:" + ex);
            }
            finally
            {
                stream.Count = stream.Position;
                stream.Position = 0;
            }
            return stream;
        }

        public static T DeserializeObject<T>(Segment segment)
        {
            Type type = typeof(T);
            if (Types1.TryGetValue(type, out TypeBind typeBind)) 
            {
                var bind = (ISerialize<T>)Activator.CreateInstance(typeBind.type);
                T value = bind.Read(segment);
                BufferPool.Push(segment);
                return value;
            }
            throw new Exception($"请注册或绑定:{type}类型后才能序列化!");
        }
    }
}