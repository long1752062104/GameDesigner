namespace Net.Serialize
{
    using Net.Event;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Reflection;
    using Net.System;
    using Net.Share;

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
            InitBindInterfaces();
            AddBaseType();
            return true;
        }

        /// <summary>
        /// 添加网络基本类型， int，float，bool，string......
        /// </summary>
        public static void AddBaseType()
        {
            AddBaseType3<short>();
            AddBaseType3<int>();
            AddBaseType3<long>();
            AddBaseType3<ushort>();
            AddBaseType3<uint>();
            AddBaseType3<ulong>();
            AddBaseType3<float>();
            AddBaseType3<double>();
            AddBaseType3<bool>();
            AddBaseType3<char>();
            AddBaseType3<string>();
            AddBaseType3<byte>();
            AddBaseType3<sbyte>();
            AddBaseType3<DateTime>();
            AddBaseType3<decimal>();
            AddBaseType3<DBNull>();
            //其他可能用到的
            AddSerializeType3<Vector2>();
            AddSerializeType3<Vector3>();
            AddSerializeType3<Vector4>();
            AddSerializeType3<Quaternion>();
            AddSerializeType3<Rect>();
            AddSerializeType3<Color>();
            AddSerializeType3<Color32>();
            AddSerializeType3<UnityEngine.Vector2>();
            AddSerializeType3<UnityEngine.Vector3>();
            AddSerializeType3<UnityEngine.Vector4>();
            AddSerializeType3<UnityEngine.Quaternion>();
            AddSerializeType3<UnityEngine.Rect>();
            AddSerializeType3<UnityEngine.Color>();
            AddSerializeType3<UnityEngine.Color32>();
            //框架操作同步用到
            AddSerializeType3<Operation>();
            AddSerializeType3<OperationList>();
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

        private static void AddBaseType3<T>()
        {
            AddBaseType<T>();
            AddBaseArrayType<T>();
            AddBaseListType<T>();
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

        private static void AddBaseArrayType<T>()
        {
            var type = typeof(T[]);
            if (Types2.ContainsKey(type))
                return;
            ushort hashType = (ushort)Types.Count;
            Types.Add(hashType, type);
            Types1.Add(type, hashType);
            Types2.Add(type, new TypeBind() { type = typeof(BaseArrayBind<T>), hashCode = hashType });
        }

        private static void AddBaseListType<T>()
        {
            var type = typeof(List<T>);
            if (Types2.ContainsKey(type))
                return;
            ushort hashType = (ushort)Types.Count;
            Types.Add(hashType, type);
            Types1.Add(type, hashType);
            Types2.Add(type, new TypeBind() { type = typeof(BaseListBind<T>), hashCode = hashType });
        }

        public static void InitBindInterfaces()
        { 
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    var serType = type.GetInterface(typeof(ISerialize<>).FullName);
                    if (serType == null)
                        continue;
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
            public void Write(T value, Segment stream)
            {
                stream.WriteValue(value);
            }
            public T Read(Segment stream)
            {
                return stream.ReadValue<T>();
            }

            public void WriteValue(object value, Segment stream)
            {
                stream.WriteValue(value);
            }

            object ISerialize.ReadValue(Segment stream)
            {
                return stream.ReadValue<T>();
            }
        }

        internal struct BaseArrayBind<T> : ISerialize<T[]>, ISerialize
        {
            public void Write(T[] value, Segment stream)
            {
                stream.WriteArray(value);
            }
            public T[] Read(Segment stream)
            {
                return stream.ReadArray<T>();
            }

            public void WriteValue(object value, Segment stream)
            {
                stream.WriteArray(value);
            }

            object ISerialize.ReadValue(Segment stream)
            {
                return stream.ReadArray<T>();
            }
        }

        internal struct BaseListBind<T> : ISerialize<List<T>>, ISerialize
        {
            public void Write(List<T> value, Segment stream)
            {
                stream.WriteList(value);
            }
            public List<T> Read(Segment stream)
            {
                return stream.ReadList<T>();
            }

            public void WriteValue(object value, Segment stream)
            {
                stream.WriteList(value);
            }

            object ISerialize.ReadValue(Segment stream)
            {
                return stream.ReadList<T>();
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
                NDebug.LogError("序列化:" + value + "出错 详细信息:" + ex);
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
                NDebug.LogError("序列化:" + value + "出错 详细信息:" + ex);
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
                NDebug.LogError("序列化:" + model.func + "方法出错 详细信息:" + ex);
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
                    var obj1 = DeserializeObject(type, segment);
                    list.Add(obj1);
                }
                obj.pars = list.ToArray();
            }
            catch (Exception ex)
            {
                NDebug.LogError($"解析[{obj.name}]出错 详细信息:" + ex);
                obj.error = true;
            }
            return obj;
        }
    }
}