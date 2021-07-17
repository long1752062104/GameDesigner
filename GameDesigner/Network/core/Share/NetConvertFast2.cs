namespace Net.Share
{
    using Net.Event;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// 快速序列化2接口--动态匹配
    /// </summary>
    public interface ISerialize
    {
        /// <summary>
        /// 序列化写入
        /// </summary>
        /// <param name="value"></param>
        /// <param name="stream"></param>
        void WriteValue(object value, Segment stream);
        /// <summary>
        /// 反序列化读取
        /// </summary>
        /// <param name="stream"></param>
        object ReadValue(Segment stream);
    }

    /// <summary>
    /// 快速序列化2接口---类型匹配
    /// </summary>
    public interface ISerialize<T>
    {
        /// <summary>
        /// 序列化写入
        /// </summary>
        /// <param name="value"></param>
        /// <param name="stream"></param>
        void Write(T value, Segment stream);
        /// <summary>
        /// 反序列化读取
        /// </summary>
        /// <param name="stream"></param>
        T Read(Segment stream);
    }

    /// <summary>
    /// 极速序列化2版本
    /// </summary>
    public class NetConvertFast2 : NetConvertBase
    {
        private static MyDictionary<ushort, Type> Types = new MyDictionary<ushort, Type>();
        private static MyDictionary<Type, ushort> Types1 = new MyDictionary<Type, ushort>();
        private static MyDictionary<Type, TypeBind> Types2 = new MyDictionary<Type, TypeBind>();
        private static readonly MyDictionary<Type, Type> BindTypes = new MyDictionary<Type, Type>();

        static NetConvertFast2()
        {
            Init();
        }

        /// <summary>
        /// 初始化网络转换类型
        /// </summary>
        public static bool Init()
        {
            Types.Clear();
            Types1.Clear();
            Types2.Clear();
            BindTypes.Clear();
            GetInterfaces();
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
            //其他数组
            AddSerializeType<Vector2[]>();
            AddSerializeType<Vector3[]>();
            AddSerializeType<Vector4[]>();
            AddSerializeType<Quaternion[]>();
            AddSerializeType<Rect[]>();
            AddSerializeType<Color[]>();
            AddSerializeType<Color32[]>();
            AddSerializeType<UnityEngine.Vector2[]>();
            AddSerializeType<UnityEngine.Vector3[]>();
            AddSerializeType<UnityEngine.Vector4[]>();
            AddSerializeType<UnityEngine.Quaternion[]>();
            AddSerializeType<UnityEngine.Rect[]>();
            AddSerializeType<UnityEngine.Color[]>();
            AddSerializeType<UnityEngine.Color32[]>();
            //其他泛型
            AddSerializeType<List<Vector2>>();
            AddSerializeType<List<Vector3>>();
            AddSerializeType<List<Vector4>>();
            AddSerializeType<List<Quaternion>>();
            AddSerializeType<List<Rect>>();
            AddSerializeType<List<Color>>();
            AddSerializeType<List<Color32>>();
            AddSerializeType<List<UnityEngine.Vector2>>();
            AddSerializeType<List<UnityEngine.Vector3>>();
            AddSerializeType<List<UnityEngine.Vector4>>();
            AddSerializeType<List<UnityEngine.Quaternion>>();
            AddSerializeType<List<UnityEngine.Rect>>();
            AddSerializeType<List<UnityEngine.Color>>();
            AddSerializeType<List<UnityEngine.Color32>>();
            //框架操作同步用到
            AddSerializeType<Operation>();
            AddSerializeType<Operation[]>();
            AddSerializeType<List<Operation>>();
            AddSerializeType<OperationList>();
            AddSerializeType<OperationList[]>();
            AddSerializeType<List<OperationList>>();
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
        /// 添加可序列化的3个参数类型(T类,T类数组,T类List泛型), 网络参数类型 如果不进行添加将不会被序列化,反序列化
        /// </summary>
        /// <typeparam name="T">要添加的网络类型</typeparam>
        public static void AddSerializeType3<T>()
        {
            AddSerializeType(typeof(T));
            AddSerializeType(typeof(T[]));
            AddSerializeType(typeof(List<T>));
        }

        /// <summary>
        /// 添加可序列化的3个参数类型(T类,T类数组,T类List泛型), 网络参数类型 如果不进行添加将不会被序列化,反序列化
        /// </summary>
        /// <typeparam name="T">要添加的网络类型</typeparam>
        public static void AddSerializeType3(Type type)
        {
            AddSerializeType(type);
            AddSerializeType(Array.CreateInstance(type, 0).GetType());
            AddSerializeType(typeof(List<>).MakeGenericType(type));
        }

        /// <summary>
        /// 添加可序列化的参数类型, 网络参数类型 如果不进行添加将不会被序列化,反序列化
        /// </summary>
        /// <param name="type">要添加的网络类型</param>
        public static void AddSerializeType(Type type)
        {
            if (Types2.ContainsKey(type))
                throw new Exception($"已经添加{type}键，不需要添加了!");
            if (!BindTypes.TryGetValue(type, out Type bindType))
                throw new Exception($"类型{type}尚未实现绑定类型,请使用工具生成绑定类型!");
            ushort hashType = (ushort)Types.Count;
            Types.Add(hashType, type);
            Types1.Add(type, hashType);
            Types2.Add(type, new TypeBind() { type = bindType, hashCode = hashType } );
        }

        private static void AddBaseType<T>()
        {
            var type = typeof(T);
            if (Types2.ContainsKey(type))
                return;
            ushort hashType = (ushort)Types.Count;
            Types.Add(hashType, type);
            Types1.Add(type, hashType);
            Types2.Add(type, new TypeBind() { type = typeof(BaseBind<T>), hashCode = hashType });
        }

        public static void GetInterfaces()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var types = assembly.GetTypes().Where((t)=> { 
                    return t.GetInterface(typeof(ISerialize<>).FullName) != null; 
                });
                foreach (var type in types)
                {
                    var serType = type.GetInterface(typeof(ISerialize<>).FullName);
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

        internal struct BaseBind<T> : ISerialize<T>, ISerialize
        {
            public void Write(T value, Segment strem)
            {
                strem.WriteValue(value);
            }
            public T Read(Segment strem)
            {
                return strem.ReadValue<T>();
            }

            public void WriteValue(object value, Segment strem)
            {
                strem.WriteValue(value);
            }

            object ISerialize.ReadValue(Segment strem)
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
                if(Types2.TryGetValue(type, out TypeBind typeBind))
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

        public static Segment SerializeObject(object value)
        {
            var stream = BufferPool.Take();
            try
            {
                Type type = value.GetType();
                if (Types2.TryGetValue(type, out TypeBind typeBind))
                {
                    var bind = (ISerialize)Activator.CreateInstance(typeBind.type);
                    bind.WriteValue(value, stream);
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
            if (Types2.TryGetValue(type, out TypeBind typeBind)) 
            {
                var bind = (ISerialize<T>)Activator.CreateInstance(typeBind.type);
                T value = bind.Read(segment);
                BufferPool.Push(segment);
                return value;
            }
            throw new Exception($"请注册或绑定:{type}类型后才能反序列化!");
        }

        public static object DeserializeObject(Type type, Segment segment)
        {
            if (Types2.TryGetValue(type, out TypeBind typeBind))
            {
                var bind = (ISerialize)Activator.CreateInstance(typeBind.type);
                object value = bind.ReadValue(segment);
                BufferPool.Push(segment);
                return value;
            }
            throw new Exception($"请注册或绑定:{type}类型后才能反序列化!");
        }

        /// <summary>
        /// 索引取类型
        /// </summary>
        /// <param name="typeIndex"></param>
        /// <returns></returns>
        private static Type IndexToType(ushort typeIndex)
        {
            if (Types.TryGetValue(typeIndex, out Type type))
                return type;
            return null;
        }

        /// <summary>
        /// 类型取索引
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static ushort TypeToIndex(Type type)
        {
            if (Types1.TryGetValue(type, out ushort typeHash))
                return typeHash;
            throw new KeyNotFoundException($"没有注册[{type}]类为序列化对象, 请使用序列化生成工具生成{type}绑定类! (如果是基类,请类型作者修复!)");
        }

        public static byte[] SerializeModel(RPCModel model)
        {
            var stream = BufferPool.Take();
            byte[] buffer1 = new byte[0];
            try
            {
                byte head = 0;
                bool hasFunc = !string.IsNullOrEmpty(model.func);
                bool hasMask = model.methodMask != 0;
                SetBit(ref head, 1, hasFunc);
                SetBit(ref head, 2, hasMask);
                stream.WriteByte(head);
                if (hasFunc) stream.WriteValue(model.func);
                if (hasMask) stream.WriteValue(model.methodMask);
                foreach (var obj in model.pars)
                {
                    if (obj == null)
                        continue;
                    Type type = obj.GetType();
                    stream.WriteValue(TypeToIndex(type));
                    if (Types2.TryGetValue(type, out TypeBind typeBind))
                    {
                        var bind = (ISerialize)Activator.CreateInstance(typeBind.type);
                        bind.WriteValue(obj, stream);
                    }
                    else throw new Exception($"请注册或绑定:{type}类型后才能序列化!");
                }
                buffer1 = stream.ToArray(true);
            }
            catch (Exception ex)
            {
                NDebug.LogError("序列化异常:" + ex);
            }
            return buffer1;
        }

        public static FuncData DeserializeModel(Segment segment)
        {
            FuncData obj = default;
            try
            {
                byte head = segment.ReadByte();
                bool hasFunc = GetBit(head, 1);
                bool hasMask = GetBit(head, 2);
                if (hasFunc) obj.name = segment.ReadValue<string>();
                if (hasMask) obj.mask = segment.ReadValue<ushort>();
                List<object> list = new List<object>();
                int count = segment.Index + segment.Count;
                while (segment.Position < count)
                {
                    ushort typeIndex = segment.ReadValue<ushort>();
                    Type type = IndexToType(typeIndex);
                    if (type == null)
                        break;
                    var obj1 = DeserializeObject(type, segment);
                    list.Add(obj1);
                }
                obj.pars = list.ToArray();
            }
            catch (Exception ex)
            {
                NDebug.LogError("解析出错:" + ex);
                obj.error = true;
            }
            return obj;
        }
    }
}