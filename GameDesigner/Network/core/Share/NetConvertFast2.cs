namespace Net.Share
{
    using Net.Event;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 快速序列化2接口
    /// </summary>
    public interface ISerialize
    {
        /// <summary>
        /// 序列化写入
        /// </summary>
        /// <param name="strem"></param>
        void Write(Segment strem);
        /// <summary>
        /// 反序列化读取
        /// </summary>
        /// <param name="strem"></param>
        void Read(Segment strem);
    }

    /// <summary>
    /// 快速序列化2版本
    /// </summary>
    public class NetConvertFast2 : NetConvertBase
    {
        private static MyDictionary<ushort, Type> networkTypes = new MyDictionary<ushort, Type>();
        private static MyDictionary<Type, ushort> networkType1s = new MyDictionary<Type, ushort>();
        private static Type nonSerialized = typeof(NonSerializedAttribute);

        static NetConvertFast2()
        {
            Init();
        }

        /// <summary>
        /// 初始化网络转换类型
        /// </summary>
        public static bool Init()
        {
            networkTypes = new MyDictionary<ushort, Type>();
            networkType1s = new MyDictionary<Type, ushort>();
            AddNetworkBaseType();
            MakeNonSerializedAttribute<NonSerializedAttribute>();
            return true;
        }

        public static void MakeNonSerializedAttribute<T>() where T : Attribute
        {
            nonSerialized = typeof(T);
        }

        /// <summary>
        /// 添加网络基本类型， int，float，bool，string......
        /// </summary>
        public static void AddNetworkBaseType()
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
            AddNetworkType<Vector2>();
            AddNetworkType<Vector3>();
            AddNetworkType<Vector4>();
            AddNetworkType<Quaternion>();
            AddNetworkType<Rect>();
            AddNetworkType<Color>();
            AddNetworkType<Color32>();
            AddNetworkType<UnityEngine.Vector2>();
            AddNetworkType<UnityEngine.Vector3>();
            AddNetworkType<UnityEngine.Vector4>();
            AddNetworkType<UnityEngine.Quaternion>();
            AddNetworkType<UnityEngine.Rect>();
            AddNetworkType<UnityEngine.Color>();
            AddNetworkType<UnityEngine.Color32>();
            //框架操作同步用到
            AddNetworkType<Operation>();
            AddNetworkType<Operation[]>();
            AddNetworkType<OperationList>();
        }

        /// <summary>
        /// 添加可序列化的参数类型, 网络参数类型 如果不进行添加将不会被序列化,反序列化
        /// </summary>
        /// <typeparam name="T">要添加的网络类型</typeparam>
        public static void AddNetworkType<T>()
        {
            AddNetworkType(typeof(T));
        }

        /// <summary>
        /// 添加可序列化的参数类型, 网络参数类型 如果不进行添加将不会被序列化,反序列化
        /// </summary>
        /// <param name="type">要添加的网络类型</param>
        public static void AddNetworkType(Type type)
        {
            if (networkType1s.ContainsKey(type))
                throw new Exception($"已经添加{type}键，不需要添加了!");
            networkTypes.Add((ushort)networkTypes.Count, type);
            networkType1s.Add(type, (ushort)networkType1s.Count);
        }

        private static void AddBaseType<T>()
        {
            var type = typeof(T);
            if (networkType1s.ContainsKey(type))
                return;
            networkTypes.Add((ushort)networkTypes.Count, type);
            networkType1s.Add(type, (ushort)networkType1s.Count);
        }

        /// <summary>
        /// 添加可序列化的参数类型, 网络参数类型 如果不进行添加将不会被序列化,反序列化
        /// </summary>
        /// <param name="types"></param>
        public static void AddNetworkType(params Type[] types)
        {
            foreach (Type type in types)
            {
                AddNetworkType(type);
            }
        }

        public static Segment SerializeObject<T>(T obj) where T : ISerialize
        {
            var stream = BufferPool.Take();
            try
            {
                obj.Write(stream);
            }
            catch (Exception ex)
            {
                NDebug.LogError("序列化异常:" + ex);
            }
            finally
            {
                stream.Count = stream.Position;
            }
            return stream;
        }

        public static T DeserializeObject<T>(Segment segment) where T : ISerialize, new()
        {
            T obj = new T();
            obj.Read(segment);
            BufferPool.Push(segment);
            return obj;
        }
    }
}