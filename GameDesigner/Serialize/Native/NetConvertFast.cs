namespace Net.Serialize
{
    using Net.Event;
    using Newtonsoft_X.Json;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.IO;
    using global::System.Linq;
    using global::System.Reflection;
    using global::System.Text;
    using static Net.Serialize.NetConvert;
    using Net.Share;
    using Net.System;

    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
    public struct FastData
    {
        public byte type;
        public byte[] buffer;

        public FastData(byte type, byte[] buffer)
        {
            this.type = type;
            this.buffer = buffer;
        }

        public override string ToString()
        {
            return $"类型:{NetConvertFast.GetTypeHash(type)}|字节大小:{buffer.Length}";
        }
    }

    /// <summary>
    /// 快速解析类型, 使用此类需要使用AddNetworkType()先添加序列化类型, 类型是固定, 并且双端统一
    /// </summary>
    public class NetConvertFast : NetConvertBase
    {
        private static readonly Dictionary<byte, Type> Types = new Dictionary<byte, Type>();
        private static readonly Dictionary<Type, byte> Types1 = new Dictionary<Type, byte>();
        private static readonly Dictionary<string, List<string>> filter = new Dictionary<string, List<string>>();

        static NetConvertFast()
        {
            Init();
        }

        public static void Init()
        {
            Types.Clear();
            Types1.Clear();
            AddNetworkBaseType();
        }

        /// <summary>
        /// 添加网络基本类型， int，float，bool，string......
        /// </summary>
        public static void AddNetworkBaseType()
        {
            AddNetworkType<short>();
            AddNetworkType<int>();
            AddNetworkType<long>();
            AddNetworkType<ushort>();
            AddNetworkType<uint>();
            AddNetworkType<ulong>();
            AddNetworkType<float>();
            AddNetworkType<double>();
            AddNetworkType<bool>();
            AddNetworkType<char>();
            AddNetworkType<string>();
            AddNetworkType<byte>();
            AddNetworkType<sbyte>();
            //基础序列化数组
            AddNetworkType<short[]>();
            AddNetworkType<int[]>();
            AddNetworkType<long[]>();
            AddNetworkType<ushort[]>();
            AddNetworkType<uint[]>();
            AddNetworkType<ulong[]>();
            AddNetworkType<float[]>();
            AddNetworkType<double[]>();
            AddNetworkType<bool[]>();
            AddNetworkType<char[]>();
            AddNetworkType<string[]>();
            AddNetworkType<byte[]>();
            AddNetworkType<sbyte[]>();
            //基础序列化List
            AddNetworkType<List<short>>();
            AddNetworkType<List<int>>();
            AddNetworkType<List<long>>();
            AddNetworkType<List<ushort>>();
            AddNetworkType<List<uint>>();
            AddNetworkType<List<ulong>>();
            AddNetworkType<List<float>>();
            AddNetworkType<List<double>>();
            AddNetworkType<List<bool>>();
            AddNetworkType<List<char>>();
            AddNetworkType<List<string>>();
            AddNetworkType<List<byte>>();
            AddNetworkType<List<sbyte>>();
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
            //场景操作同步用到
            AddNetworkType<Operation>();
            AddNetworkType<Operation[]>();
            AddNetworkType<OperationList>();
        }

        /// <summary>
        /// 添加可序列化的参数类型, 网络参数类型 如果不进行添加将不会被序列化,反序列化
        /// </summary>
        public static void AddNetworkType<T>()
        {
            AddNetworkType(typeof(T));
        }

        /// <summary>
        /// 添加经过网络传送的类型
        /// </summary>
        /// <param name="type"></param>
        public static void AddNetworkType(Type type)
        {
            if (Types1.ContainsKey(type))
                throw new Exception($"已经添加{type}键，不需要添加了!");
            Types.Add((byte)Types.Count, type);
            Types1.Add(type, (byte)Types1.Count);
        }

        /// <summary>
        /// 添加网络传输的程序集, 程序集内的所有类型都会被添加, 注意: 客户端和服务器都必须统一使用一模一样的程序集, 否则有可能出现问题!
        /// </summary>
        /// <param name="assembly"></param>
        public static void AddAssembly(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes().Where(t => { return !t.IsAbstract; }))
            {
                AddNetworkType(type);
            }
        }

        /// <summary>
        /// 添加assembly程序集的所有nameSpace命名空间的类型
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="nameSpace"></param>
        public static void AddNameSpaceTypes(Assembly assembly, string nameSpace)
        {
            foreach (Type type in assembly.GetTypes().Where(t => { return !t.IsAbstract & t.Namespace == nameSpace; }))
            {
                AddNetworkType(type);
            }
        }

        public static byte[] Serialize(RPCModel model)
        {
            var bufferPool = BufferPool.Take();
            using (MemoryStream stream = new MemoryStream(bufferPool))
            {
                try
                {
                    stream.SetLength(0);
                    List<FastData> datas = new List<FastData>();
                    foreach (object obj in model.pars)
                    {
#if CLOSE_ILR
                        var type = obj.GetType();
#else
                        Type type = ObjectExtensions.GetType(obj);
#endif
                        FastData data = new FastData() { type = GetTypeHash(type) };
                        if (type.IsGenericType)
                        {
                            bool result = true;
                            foreach (Type ga in type.GetGenericArguments())
                            {
#if CLOSE_ILR
                                if (!ga.IsPrimitive & type != typeof(string) & ga.GetCustomAttribute<ProtoBuf.ProtoContractAttribute>() == null)
#else
                                if (!ga.IsPrimitive & type != typeof(string) & ga.GetCustomAttributes(typeof(ProtoBuf.ProtoContractAttribute), false).Length == 0)
#endif
                                {
                                    result = false;
                                    break;
                                }
                            }
                            if (result)
                            {
                                ProtoBuf.Serializer.Serialize(stream, obj);
                                data.buffer = stream.ToArray();
                                datas.Add(data);
                            }
                            else
                            {
                                byte[] bts = SerializeComplex(type, obj);
                                data.buffer = bts;
                                datas.Add(data);
                            }
                        }
                        else
                        {
#if CLOSE_ILR
                            var customAtt = type.GetCustomAttribute(typeof(ProtoBuf.ProtoContractAttribute), false);
                            if (customAtt == null & !type.IsPrimitive & type != typeof(string))
#else
                            object[] customAtt = type.GetCustomAttributes(typeof(ProtoBuf.ProtoContractAttribute), false);
                            if (customAtt.Length == 0 & !type.IsPrimitive & type != typeof(string))
#endif
                            {
                                byte[] bts = SerializeComplex(type, obj);
                                data.buffer = bts;
                                datas.Add(data);
                            }
                            else
                            {
                                ProtoBuf.Serializer.Serialize(stream, obj);
                                data.buffer = stream.ToArray();
                                datas.Add(data);
                            }
                        }
                        stream.SetLength(0);
                    }
                    byte head = 0;
                    bool hasFunc = !string.IsNullOrEmpty(model.func);
                    bool hasMask = model.methodMask != 0;
                    SetBit(ref head, 1, hasFunc);
                    SetBit(ref head, 2, hasMask);
                    stream.WriteByte(head);
                    if (hasFunc)
                    {
                        var funcbts = Encoding.UTF8.GetBytes(model.func);
                        stream.WriteByte((byte)funcbts.Length);
                        stream.Write(funcbts, 0, funcbts.Length);
                    }
                    if (hasMask) stream.Write(BitConverter.GetBytes(model.methodMask), 0, 2);
                    ProtoBuf.Serializer.Serialize(stream, datas);
                    return stream.ToArray();
                }
                catch (Exception ex)
                {
                    NDebug.LogError("序列化:" + model.func + "方法出错 详细信息:" + ex);
                    return new byte[0];
                }
                finally
                {
                    BufferPool.Push(bufferPool);
                }
            }
        }

        private static byte GetTypeHash(Type type)
        {
            if (Types1.TryGetValue(type, out byte typeHash))
                return typeHash;
            throw new IOException($"参数类型:[{type}]没有被注册! 请使用NetConvertFast.AddNetworkType<{type}>()添加序列化类型! 双端都要添加");
        }

        //序列化复杂类型的, 如unity的类型
        private static byte[] SerializeComplex(Type type, object obj)
        {
            DateTime time = DateTime.Now.AddSeconds(5);
        JUMP: try
            {
                JsonSerializerSettings jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                string typeName = type.ToString();
                if (filter.ContainsKey(typeName))
                    jSetting.ContractResolver = new LimitPropsContractResolver(filter[typeName].ToArray(), false);
                string jsonStr = JsonConvert.SerializeObject(obj, jSetting);
                return Encoding.Unicode.GetBytes(jsonStr);
            }
            catch (Exception e)
            {
                string ee = e.Message;
                //Error setting value to 'xxxxxxxxxxxxxxxxxx'xxxxxxxxxxxxx.
                if (ee.Contains("Error setting value to '"))
                {
                    string[] field = ee.Split(new string[] { "'" }, StringSplitOptions.None);
                    if (field.Length > 1)
                        ee = field[1].Split(new string[] { "." }, StringSplitOptions.None)[0];
                }
                //Error getting value from 'ScopeId' on 'System.Net.IPAddress'.
                if (ee.Contains("Error getting value from '"))
                {
                    string[] field = ee.Split(new string[] { "'" }, StringSplitOptions.None);
                    if (field.Length > 1)
                        ee = field[1].Split(new string[] { "." }, StringSplitOptions.None)[0];
                }
                //Self referencing loop detected for property 'normalized' with type 'UnityEngine.Vector3'. Path 'pos.normalized.normalized'.
                if (ee.Contains("Self referencing loop detected for property '"))
                {
                    string[] field = ee.Split(new string[] { "'" }, StringSplitOptions.None);
                    if (field.Length > 1)
                        ee = field[1].Split(new string[] { "." }, StringSplitOptions.None)[0];
                }
                //Self referencing loop detected with type 'Server.BydrClient'. Path 'scene.Players'.
                if (ee.Contains("Self referencing loop detected with type '"))
                {
                    string[] field = ee.Split(new string[] { "'" }, StringSplitOptions.None);
                    if (field.Length > 3)
                        ee = field[3].Split(new string[] { "." }, StringSplitOptions.None)[0];
                }
                //Could not create an instance of type System.Net.EndPoint. Type is an interface or abstract class and cannot be instantiated. Path 'RemotePoint.e6881ad2-e201-3376-9c81-f3496100c170.AddressFamily', line 1, position 488.
                if (ee.Contains("Could not create an instance of type"))
                {
                    string[] field = ee.Split(new string[] { "'" }, StringSplitOptions.None);
                    if (field.Length > 1)
                        ee = field[1].Split(new string[] { "." }, StringSplitOptions.None)[0];
                }
                string typeName = type.ToString();
                if (!filter.ContainsKey(typeName))
                    filter.Add(typeName, new List<string>());
                if (!filter[typeName].Contains(ee))
                    filter[typeName].Add(ee);
                if (DateTime.Now > time)
                    return new byte[0];
                NDebug.LogError(e.ToString());
                goto JUMP;
            }
        }

        public static Type GetTypeHash(byte hashCode)
        {
            if (Types.TryGetValue(hashCode, out Type type))
                return type;
            NDebug.LogError($"找不到哈希代码类型:{hashCode}, 类型太复杂时需要使用 NetConvertOld.AddSerializeType(type) 添加类型后再进行系列化!");
            return null;
        }

        public static FuncData Deserialize(byte[] buffer, int index, int count)
        {
            FuncData fdata = default;
            if (index < 0 | count <= 0 | count > buffer.Length)
            {
                fdata.error = true;
                return fdata;
            }
            byte head = buffer[index++];
            count--;
            bool hasFunc = GetBit(head, 1);
            bool hasMask = GetBit(head, 2);
            if (hasFunc)
            {
                int funcLen = buffer[index++];
                fdata.name = Encoding.UTF8.GetString(buffer, index, funcLen);
                index += funcLen;
                count -= funcLen + 1;
            }
            if (hasMask)
            {
                fdata.mask = BitConverter.ToUInt16(buffer, index);
                index += 2;
                count -= 2;
            }
            if (index + count > buffer.Length)
            {
                fdata.error = true;
                return fdata;
            }
            using (MemoryStream stream = new MemoryStream(buffer, index, count))
            {
                List<FastData> datas = ProtoBuf.Serializer.Deserialize<List<FastData>>(stream);
                List<object> pars = new List<object>();
                foreach (FastData data in datas)
                {
                    using (MemoryStream ms = new MemoryStream(data.buffer))
                    {
                        Type type = GetTypeHash(data.type);
                        if (type == null)
                        {
                            pars.Add(null);
                            continue;
                        }
                        if (type.IsGenericType)
                        {
                            bool result = true;
                            foreach (Type ga in type.GetGenericArguments())
                            {
#if CLOSE_ILR
                                if (!ga.IsPrimitive & type != typeof(string) & ga.GetCustomAttribute<ProtoBuf.ProtoContractAttribute>() == null)
#else
                                if (!ga.IsPrimitive & type != typeof(string) & ga.GetCustomAttributes(typeof(ProtoBuf.ProtoContractAttribute), false).Length == 0)
#endif
                                {
                                    result = false;
                                    break;
                                }
                            }
                            if (result)
                            {
                                object obj = ProtoBuf.Serializer.Deserialize(type, ms);
                                pars.Add(obj);
                            }
                            else
                            {
                                object obj = DeserializeComplex(type, data.buffer, 0, data.buffer.Length);
                                pars.Add(obj);
                            }
                        }
                        else
                        {
#if CLOSE_ILR
                            var customAtt = type.GetCustomAttribute(typeof(ProtoBuf.ProtoContractAttribute), false);
                            if (customAtt == null & !type.IsPrimitive & type != typeof(string))
#else
                            object[] customAtt = type.GetCustomAttributes(typeof(ProtoBuf.ProtoContractAttribute), false);
                            if (customAtt.Length == 0 & !type.IsPrimitive & type != typeof(string))
#endif
                            {
                                object obj = DeserializeComplex(type, data.buffer, 0, data.buffer.Length);
                                pars.Add(obj);
                            }
                            else
                            {
                                object obj = ProtoBuf.Serializer.Deserialize(type, ms);
                                pars.Add(obj);
                            }
                        }
                    }
                }
                fdata.pars = pars.ToArray();
                return fdata;
            }
        }

        /// <summary>
        /// 反序列化复杂类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        private static object DeserializeComplex(Type type, byte[] buffer, int index, int count)
        {
            DateTime time = DateTime.Now.AddSeconds(5);
        JUMP: try
            {
                JsonSerializerSettings jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                string typeName = type.ToString();
                if (filter.ContainsKey(typeName))
                    jSetting.ContractResolver = new LimitPropsContractResolver(filter[typeName].ToArray(), false);
                string jsonStr = Encoding.Unicode.GetString(buffer, index, count);
                return JsonConvert.DeserializeObject(jsonStr, type, jSetting);
            }
            catch (Exception e)
            {
                string ee = e.Message;
                //Error setting value to 'xxxxxxxxxxxxxxxxxx'xxxxxxxxxxxxx.
                if (ee.Contains("Error setting value to '"))
                {
                    string[] field = ee.Split(new string[] { "'" }, StringSplitOptions.None);
                    if (field.Length > 1)
                        ee = field[1].Split(new string[] { "." }, StringSplitOptions.None)[0];
                }
                //Error getting value from 'ScopeId' on 'System.Net.IPAddress'.
                if (ee.Contains("Error getting value from '"))
                {
                    string[] field = ee.Split(new string[] { "'" }, StringSplitOptions.None);
                    if (field.Length > 1)
                        ee = field[1].Split(new string[] { "." }, StringSplitOptions.None)[0];
                }
                //Self referencing loop detected for property 'normalized' with type 'UnityEngine.Vector3'. Path 'pos.normalized.normalized'.
                if (ee.Contains("Self referencing loop detected for property '"))
                {
                    string[] field = ee.Split(new string[] { "'" }, StringSplitOptions.None);
                    if (field.Length > 1)
                        ee = field[1].Split(new string[] { "." }, StringSplitOptions.None)[0];
                }
                //Self referencing loop detected with type 'Server.BydrClient'. Path 'scene.Players'.
                if (ee.Contains("Self referencing loop detected with type '"))
                {
                    string[] field = ee.Split(new string[] { "'" }, StringSplitOptions.None);
                    if (field.Length > 3)
                        ee = field[3].Split(new string[] { "." }, StringSplitOptions.None)[0];
                }
                //Could not create an instance of type System.Net.EndPoint. Type is an interface or abstract class and cannot be instantiated. Path 'RemotePoint.e6881ad2-e201-3376-9c81-f3496100c170.AddressFamily', line 1, position 488.
                if (ee.Contains("Could not create an instance of type"))
                {
                    string[] field = ee.Split(new string[] { "'" }, StringSplitOptions.None);
                    if (field.Length > 1)
                        ee = field[1].Split(new string[] { "." }, StringSplitOptions.None)[0];
                }
                string typeName = type.ToString();
                if (!filter.ContainsKey(typeName))
                    filter.Add(typeName, new List<string>());
                if (!filter[typeName].Contains(ee))
                    filter[typeName].Add(ee);
                if (DateTime.Now > time)
                    return null;
                NDebug.LogError(e.ToString());
                goto JUMP;
            }
        }
    }
}