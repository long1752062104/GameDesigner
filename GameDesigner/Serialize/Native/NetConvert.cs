namespace Net.Serialize
{
    using Net.Event;
    using Newtonsoft_X.Json;
    using Newtonsoft_X.Json.Serialization;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.IO;
    using global::System.Linq;
    using global::System.Reflection;
    using global::System.Text;
    using Net.Share;
    using Net.System;

    /// <summary>
    /// 序列化数据标示
    /// </summary>
    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
    public struct Data
    {
        public string type;
        public byte[] buffer;

        public Data(string type, byte[] buffer)
        {
            this.type = type;
            this.buffer = buffer;
        }

        public override string ToString()
        {
            return $"类型:{type}|字节大小:{buffer.Length}";
        }
    }

    /// <summary>
    /// 网络转换核心 2019.7.16
    /// </summary>
    public class NetConvert : NetConvertBase
    {
        private static readonly Dictionary<string, List<string>> filter = new Dictionary<string, List<string>>();

        /// <summary>
        /// 
        /// </summary>
        public class LimitPropsContractResolver : DefaultContractResolver
        {
            readonly string[] props = null;
            readonly bool retain;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="props">传入的属性数组</param>
            /// <param name="retain">true:表示props是需要保留的字段  false：表示props是要排除的字段</param>
            public LimitPropsContractResolver(string[] props, bool retain = false)
            {
                //指定要序列化属性的清单
                this.props = props;
                this.retain = retain;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="type"></param>
            /// <param name="memberSerialization"></param>
            /// <returns></returns>
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                IList<JsonProperty> list =
                base.CreateProperties(type, memberSerialization);
                //只保留清单有列出的属性
                return list.Where(p =>
                {
                    if (retain)
                        return props.Contains(p.PropertyName);
                    else
                        return !props.Contains(p.PropertyName);
                }).ToList();
            }
        }

        /// <summary>
        /// 新版网络序列化
        /// </summary>
        /// <param name="model">函数名</param>
        /// <returns></returns>
        public static byte[] Serialize(RPCModel model, byte[] flag = null)
        {
            var bufferPool = BufferPool.Take();
            using (MemoryStream stream = new MemoryStream(bufferPool))
            {
                try
                {
                    stream.SetLength(0);
                    List<Data> datas = new List<Data>();
                    foreach (object obj in model.pars)
                    {
#if CLOSE_ILR
                        var type = obj.GetType();
#else
                        Type type = ObjectExtensions.GetType(obj);
#endif
                        Data data = new Data() { type = type.ToString() };
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
                    if (flag != null) stream.Write(flag, 0, flag.Length);
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

        /// <summary>
        /// 新版反序列化
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static FuncData Deserialize(byte[] buffer)
        {
            return Deserialize(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 新版反序列化
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
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
                List<Data> datas = ProtoBuf.Serializer.Deserialize<List<Data>>(stream);
                List<object> pars = new List<object>();
                foreach (Data data in datas)
                {
                    using (MemoryStream ms = new MemoryStream(data.buffer))
                    {
                        Type type = NetConvertOld.GetTypeAll(data.type);
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