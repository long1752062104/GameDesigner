namespace Net.Serialize
{
    using Net.Event;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.IO;
    using global::System.Linq;
    using global::System.Reflection;
    using global::System.Text;
#if SERVICE
    using global::System.Runtime.CompilerServices;
    using Microsoft.CSharp.RuntimeBinder;
    using Binder = Microsoft.CSharp.RuntimeBinder.Binder;
#endif
    using global::System.Collections;
    using global::System.Runtime.InteropServices;
    using Net.Share;
    using Net.System;

    /// <summary>
    /// 提供序列化二进制类
    /// </summary>
    public class NetConvertBinary : NetConvertBase
    {
        private static MyDictionary<ushort, Type> networkTypes = new MyDictionary<ushort, Type>();
        private static MyDictionary<Type, ushort> networkType1s = new MyDictionary<Type, ushort>();
        private static Type nonSerialized = typeof(NonSerializedAttribute);
        private static MyDictionary<Type, Member[]> map;

        static NetConvertBinary()
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
            AddBaseType<Type>();
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
            //基础结构类型初始化
            map = new MyDictionary<Type, Member[]>
            {
                { typeof(byte), new Member[] { new Member() { Type = typeof(byte), IsPrimitive = true, TypeCode = TypeCode.Byte } } },
                { typeof(sbyte), new Member[] { new Member() { Type = typeof(sbyte), IsPrimitive = true, TypeCode = TypeCode.SByte } } },
                { typeof(bool), new Member[] { new Member() { Type = typeof(bool), IsPrimitive = true, TypeCode = TypeCode.Boolean } } },
                { typeof(short), new Member[] { new Member() { Type = typeof(short), IsPrimitive = true, TypeCode = TypeCode.Int16 } } },
                { typeof(ushort), new Member[] { new Member() { Type = typeof(ushort), IsPrimitive = true, TypeCode = TypeCode.UInt16 } } },
                { typeof(char), new Member[] { new Member() { Type = typeof(char), IsPrimitive = true, TypeCode = TypeCode.Char } } },
                { typeof(int), new Member[] { new Member() { Type = typeof(int), IsPrimitive = true, TypeCode = TypeCode.Int32 } } },
                { typeof(uint), new Member[] { new Member() { Type = typeof(uint), IsPrimitive = true, TypeCode = TypeCode.UInt32 } } },
                { typeof(long), new Member[] { new Member() { Type = typeof(long), IsPrimitive = true, TypeCode = TypeCode.Int64 } } },
                { typeof(ulong), new Member[] { new Member() { Type = typeof(ulong), IsPrimitive = true, TypeCode = TypeCode.UInt64 } } },
                { typeof(float), new Member[] { new Member() { Type = typeof(float), IsPrimitive = true, TypeCode = TypeCode.Single } } },
                { typeof(double), new Member[] { new Member() { Type = typeof(double), IsPrimitive = true, TypeCode = TypeCode.Double } } },
                { typeof(DateTime), new Member[] { new Member() { Type = typeof(DateTime), IsPrimitive = true, TypeCode = TypeCode.DateTime } } },
                { typeof(decimal), new Member[] { new Member() { Type = typeof(decimal), IsPrimitive = true, TypeCode = TypeCode.Decimal } } },
                { typeof(string), new Member[] { new Member() { Type = typeof(string), IsPrimitive = true, TypeCode = TypeCode.String } } },
            };
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
            GetMembers(type);
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

        /// <summary>
        /// 添加网络程序集，此方法将会添加获取传入的类的程序集并全部添加
        /// </summary>
        /// <param name="value">传入的类</param>
        [Obsolete("不再建议使用此方法，请使用AddNetworkType方法来代替", true)]
        public static void AddNetworkTypeAssembly(Type value)
        {
            foreach (Type type in value.Assembly.GetTypes().Where((t) =>
            {
                return !t.IsAbstract & !t.IsInterface & !t.IsGenericType & !t.IsGenericTypeDefinition & t.IsPublic;
            }))
            {
                if (networkType1s.ContainsKey(type))
                    continue;
                networkTypes.Add((ushort)networkTypes.Count, type);
                networkType1s.Add(type, (ushort)networkType1s.Count);
            }
        }

        /// <summary>
        /// 添加网络传输程序集， 注意：添加客户端的程序集必须和服务器的程序集必须保持一致， 否则将会出现问题
        /// </summary>
        /// <param name="assemblies">程序集</param>
        [Obsolete("不再建议使用此方法，请使用AddNetworkType方法来代替", true)]
        public static void AddNetworkAssembly(Assembly[] assemblies)
        {
            foreach (Assembly assemblie in assemblies)
            {
                foreach (Type type in assemblie.GetTypes().Where((t) =>
                {
                    return !t.IsAbstract & !t.IsInterface & !t.IsGenericType & !t.IsGenericTypeDefinition & t.IsPublic;
                }))
                {
                    if (networkType1s.ContainsKey(type))
                        continue;
                    networkTypes.Add((ushort)networkTypes.Count, type);
                    networkType1s.Add(type, (ushort)networkType1s.Count);
                }
            }
        }

        /// <summary>
        /// 索引取类型
        /// </summary>
        /// <param name="typeIndex"></param>
        /// <returns></returns>
        private static Type IndexToType(ushort typeIndex)
        {
            if (networkTypes.TryGetValue(typeIndex, out Type type))
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
            if (networkType1s.TryGetValue(type, out ushort typeHash))
                return typeHash;
            throw new KeyNotFoundException($"没有注册[{type}]类为序列化对象, 请使用NetConvertBinary.AddNetworkType<{type}>()进行注册类型!");
        }

        private const byte BYTE = 1, SHORT = 2, INT24 = 3, INT32 = 4, INT40 = 5, INT48 = 6, INT56 = 7, LONG = 8;
        private const long Int24 = 16777216, Int40 = 1099511627776, Int48 = 281474976710656, Int56 = 72057594037927936;

        /// <summary>
        /// 序列化基本类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private unsafe static bool WriteValue<T>(Segment stream, T value)
        {
            byte num;
            switch (value)
            {
                case byte b1:
                    if (b1 == 0) return false;
                    stream.WriteByte(b1);
                    break;
                case sbyte sb:
                    if (sb == 0) return false;
                    stream.WriteByte((byte)sb);
                    break;
                case bool b:
                    if (b == false) return false;
                    stream.WriteByte((byte)(b ? 1 : 0));
                    break;
                case short s:
                    if (s == 0) return false;
                    if (s > 0) num = s < byte.MaxValue ? BYTE : SHORT;
                    else num = SHORT;
                    stream.WriteByte(num);
                    fixed (byte* ptr = stream.Buffer)
                    {
                        *(short*)(ptr + stream.Position) = s;
                        stream.Position += num;
                    }
                    break;
                case ushort us:
                    if (us == 0) return false;
                    num = us < byte.MaxValue ? BYTE : SHORT;
                    stream.WriteByte(num);
                    fixed (byte* ptr = stream.Buffer)
                    {
                        *(ushort*)(ptr + stream.Position) = us;
                        stream.Position += num;
                    }
                    break;
                case char c:
                    if (c == 0) return false;
                    if (c > 0) num = c < byte.MaxValue ? BYTE : SHORT;
                    else num = SHORT;
                    stream.WriteByte(num);
                    fixed (byte* ptr = stream.Buffer)
                    {
                        *(char*)(ptr + stream.Position) = c;
                        stream.Position += num;
                    }
                    break;
                case int i:
                    if (i == 0) return false;
                    if(i > 0) num = i < byte.MaxValue ? BYTE : i < ushort.MaxValue ? SHORT : i < Int24 ? INT24 : INT32;
                    else num = INT32;
                    stream.WriteByte(num);
                    fixed (byte* ptr = stream.Buffer)
                    {
                        *(int*)(ptr + stream.Position) = i;
                        stream.Position += num;
                    }
                    break;
                case uint ui:
                    if (ui == 0) return false;
                    num = ui < byte.MaxValue ? BYTE : ui < ushort.MaxValue ? SHORT : ui < Int24 ? INT24 : INT32;
                    stream.WriteByte(num);
                    fixed (byte* ptr = stream.Buffer)
                    {
                        *(uint*)(ptr + stream.Position) = ui;
                        stream.Position += num;
                    }
                    break;
                case float f:
                    if (f == 0) return false;
                    num = 4;
                    stream.WriteByte(num);
                    stream.Write(&f, 4);
                    break;
                case long l:
                    if (l == 0) return false;
                    if (l > 0) num = l < byte.MaxValue ? BYTE : l < ushort.MaxValue ? SHORT : l < Int24 ? INT24 : 
                            l < int.MaxValue ? INT32 : l < Int40 ? INT40 : l < Int48 ? INT48 : l < Int56 ? INT56 : LONG;
                    else num = LONG;
                    stream.WriteByte(num);
                    fixed (byte* ptr = stream.Buffer)
                    {
                        *(long*)(ptr + stream.Position) = l;
                        stream.Position += num;
                    }
                    break;
                case ulong ul:
                    if (ul == 0) return false;
                    num = ul < byte.MaxValue ? BYTE : ul < ushort.MaxValue ? SHORT : ul < Int24 ? INT24 :
                        ul < int.MaxValue ? INT32 : ul < INT40 ? INT40 : ul < Int48 ? INT48 : ul < Int56 ? INT56 : LONG;
                    stream.WriteByte(num);
                    fixed (byte* ptr = stream.Buffer)
                    {
                        *(ulong*)(ptr + stream.Position) = ul;
                        stream.Position += num;
                    }
                    break;
                case double d:
                    if (d == 0) return false;
                    num = 8;
                    stream.WriteByte(num);
                    stream.Write(&d, 8);
                    break;
                case DateTime time:
                    long tocks = time.Ticks;
                    if (tocks > 0) num = tocks < byte.MaxValue ? BYTE : tocks < ushort.MaxValue ? SHORT : tocks < int.MaxValue ? INT32 : LONG;
                    else num = LONG;
                    stream.WriteByte(num);
                    fixed (byte* ptr = stream.Buffer)
                    {
                        *(long*)(ptr + stream.Position) = tocks;
                        stream.Position += num;
                    }
                    break;
                case decimal dec:
                    if (dec == 0) return false;
                    fixed (byte* ptr = stream.Buffer)
                    {
                        *(decimal*)(ptr + stream.Position) = dec;
                        stream.Position += 16;
                    }
                    break;
                case string str:
                    int num1 = str.Length * 3;
                    num = num1 < byte.MaxValue ? BYTE : num1 < ushort.MaxValue ? SHORT : num1 < Int24 ? INT24 : INT32;
                    stream.WriteByte(num);
                    num1 = stream.Position;
                    stream.Position += num;
                    int count = Encoding.UTF8.GetBytes(str, 0, str.Length, stream.Buffer, stream.Position);
                    stream.Position = num1;
                    stream.Write(BitConverter.GetBytes(count), 0, num);
                    stream.Position += count;
                    break;
                default:
                    throw new IOException("试图写入的类型不是基本类型!");
            }
            return true;
        }

        /// <summary>
        /// 反序列化基本类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private unsafe static object ReadValue(TypeCode type, byte[] buffer, ref int index)
        {
            object obj = null;
            int num = buffer[index++];
            switch (type)
            {
                case TypeCode.Byte:
                    obj = (byte)num;
                    break;
                case TypeCode.SByte:
                    obj = (sbyte)num;
                    break;
                case TypeCode.Boolean:
                    obj = num == 1;
                    break;
                case TypeCode.Int16:
                    switch (num)
                    {
                        case BYTE:
                            obj = (short)buffer[index];
                            break;
                        case SHORT:
                            obj = BitConverter.ToInt16(buffer, index);
                            break;
                    }
                    index += num;
                    break;
                case TypeCode.UInt16:
                    switch (num)
                    {
                        case BYTE:
                            obj = (ushort)buffer[index];
                            break;
                        case SHORT:
                            obj = BitConverter.ToUInt16(buffer, index);
                            break;
                    }
                    index += num;
                    break;
                case TypeCode.Char:
                    switch (num)
                    {
                        case BYTE:
                            obj = (char)buffer[index];
                            break;
                        case SHORT:
                            obj = BitConverter.ToChar(buffer, index);
                            break;
                    }
                    index += num;
                    break;
                case TypeCode.Int32:
                    switch (num)
                    {
                        case BYTE:
                            obj = (int)buffer[index];
                            break;
                        case SHORT:
                            obj = (int)BitConverter.ToUInt16(buffer, index);
                            break;
                        case INT24:
                            if (BitConverter.IsLittleEndian)
                                obj = buffer[index] | (buffer[index + 1] << 8) | (buffer[index + 2] << 16);
                            else
                                obj = (buffer[index] << 24) | (buffer[index + 1] << 16) | (buffer[index + 2] << 8);
                            break;
                        case INT32:
                            obj = BitConverter.ToInt32(buffer, index);
                            break;
                    }
                    index += num;
                    break;
                case TypeCode.UInt32:
                    switch (num)
                    {
                        case BYTE:
                            obj = (uint)buffer[index];
                            break;
                        case SHORT:
                            obj = (uint)BitConverter.ToUInt16(buffer, index);
                            break;
                        case INT24:
                            if (BitConverter.IsLittleEndian)
                                obj = (uint)(buffer[index] | (buffer[index + 1] << 8) | (buffer[index + 2] << 16));
                            else
                                obj = (uint)((buffer[index] << 24) | (buffer[index + 1] << 16) | (buffer[index + 2] << 8));
                            break;
                        case INT32:
                            obj = BitConverter.ToUInt32(buffer, index);
                            break;
                    }
                    index += num;
                    break;
                case TypeCode.Single:
                    obj = BitConverter.ToSingle(buffer, index);
                    index += 4;
                    break;
                case TypeCode.Int64:
                    switch (num)
                    {
                        case BYTE:
                            obj = (long)buffer[index];
                            break;
                        case SHORT:
                            obj = (long)BitConverter.ToUInt16(buffer, index);
                            break;
                        case INT24:
                            if (BitConverter.IsLittleEndian)
                                obj = (long)(buffer[index] | (buffer[index + 1] << 8) | (buffer[index + 2] << 16));
                            else
                                obj = (long)((buffer[index] << 24) | (buffer[index + 1] << 16) | (buffer[index + 2] << 8));
                            break;
                        case INT32:
                            obj = (long)BitConverter.ToInt32(buffer, index);
                            break;
                        case INT40:
                            if (BitConverter.IsLittleEndian)
                            {
                                int num1 = buffer[index + 0] | (buffer[index + 1] << 8) | (buffer[index + 2] << 16) | (buffer[index + 3] << 24);
                                int num2 = buffer[index + 4];
                                obj = (uint)num1 | ((long)num2 << 32);
                            }
                            else 
                            {
                                int num3 = (buffer[index + 0] << 24) | (buffer[index + 1] << 16) | (buffer[index + 2] << 8) | buffer[index + 3];
                                int num4 = buffer[index + 4] << 24;
                                obj = (uint)num4 | ((long)num3 << 32);
                            }
                            break;
                        case INT48:
                            if (BitConverter.IsLittleEndian)
                            {
                                int num1 = buffer[index + 0] | (buffer[index + 1] << 8) | (buffer[index + 2] << 16) | (buffer[index + 3] << 24);
                                int num2 = buffer[index + 4] | (buffer[index + 5] << 8);
                                obj = (uint)num1 | ((long)num2 << 32);
                            }
                            else
                            {
                                int num3 = (buffer[index + 0] << 24) | (buffer[index + 1] << 16) | (buffer[index + 2] << 8) | buffer[index + 3];
                                int num4 = (buffer[index + 4] << 24) | (buffer[index + 5] << 16);
                                obj = (uint)num4 | ((long)num3 << 32);
                            }
                            break;
                        case INT56:
                            if (BitConverter.IsLittleEndian)
                            {
                                int num1 = buffer[index + 0] | (buffer[index + 1] << 8) | (buffer[index + 2] << 16) | (buffer[index + 3] << 24);
                                int num2 = buffer[index + 4] | (buffer[index + 5] << 8) | (buffer[index + 6] << 16);
                                obj = (uint)num1 | ((long)num2 << 32);
                            }
                            else
                            {
                                int num3 = (buffer[index + 0] << 24) | (buffer[index + 1] << 16) | (buffer[index + 2] << 8) | buffer[index + 3];
                                int num4 = (buffer[index + 4] << 24) | (buffer[index + 5] << 16) | (buffer[index + 6] << 8);
                                obj = (uint)num4 | ((long)num3 << 32);
                            }
                            break;
                        case LONG:
                            obj = BitConverter.ToInt64(buffer, index);
                            break;
                    }
                    index += num;
                    break;
                case TypeCode.UInt64:
                    switch (num)
                    {
                        case BYTE:
                            obj = (ulong)buffer[index];
                            break;
                        case SHORT:
                            obj = (ulong)BitConverter.ToUInt16(buffer, index);
                            break;
                        case INT24:
                            if (BitConverter.IsLittleEndian)
                                obj = (ulong)(buffer[index] | (buffer[index + 1] << 8) | (buffer[index + 2] << 16));
                            else
                                obj = (ulong)((buffer[index] << 24) | (buffer[index + 1] << 16) | (buffer[index + 2] << 8));
                            break;
                        case INT32:
                            obj = (ulong)BitConverter.ToInt32(buffer, index);
                            break;
                        case INT40:
                            if (BitConverter.IsLittleEndian)
                            {
                                int num1 = buffer[index + 0] | (buffer[index + 1] << 8) | (buffer[index + 2] << 16) | (buffer[index + 3] << 24);
                                int num2 = buffer[index + 4];
                                obj = (ulong)((uint)num1 | ((long)num2 << 32));
                            }
                            else
                            {
                                int num3 = (buffer[index + 0] << 24) | (buffer[index + 1] << 16) | (buffer[index + 2] << 8) | buffer[index + 3];
                                int num4 = buffer[index + 4] << 24;
                                obj = (ulong)((uint)num4 | ((long)num3 << 32));
                            }
                            break;
                        case INT48:
                            if (BitConverter.IsLittleEndian)
                            {
                                int num1 = buffer[index + 0] | (buffer[index + 1] << 8) | (buffer[index + 2] << 16) | (buffer[index + 3] << 24);
                                int num2 = buffer[index + 4] | (buffer[index + 5] << 8);
                                obj = (ulong)((uint)num1 | ((long)num2 << 32));
                            }
                            else
                            {
                                int num3 = (buffer[index + 0] << 24) | (buffer[index + 1] << 16) | (buffer[index + 2] << 8) | buffer[index + 3];
                                int num4 = (buffer[index + 4] << 24) | (buffer[index + 5] << 16);
                                obj = (ulong)((uint)num4 | ((long)num3 << 32));
                            }
                            break;
                        case INT56:
                            if (BitConverter.IsLittleEndian)
                            {
                                int num1 = buffer[index + 0] | (buffer[index + 1] << 8) | (buffer[index + 2] << 16) | (buffer[index + 3] << 24);
                                int num2 = buffer[index + 4] | (buffer[index + 5] << 8) | (buffer[index + 6] << 16);
                                obj = (ulong)((uint)num1 | ((long)num2 << 32));
                            }
                            else
                            {
                                int num3 = (buffer[index + 0] << 24) | (buffer[index + 1] << 16) | (buffer[index + 2] << 8) | buffer[index + 3];
                                int num4 = (buffer[index + 4] << 24) | (buffer[index + 5] << 16) | (buffer[index + 6] << 8);
                                obj = (ulong)((uint)num4 | ((long)num3 << 32));
                            }
                            break;
                        case LONG:
                            obj = BitConverter.ToUInt64(buffer, index);
                            break;
                    }
                    index += num;
                    break;
                case TypeCode.Double:
                    obj = BitConverter.ToDouble(buffer, index);
                    index += 8;
                    break;
                case TypeCode.DateTime:
                    switch (num)
                    {
                        case BYTE:
                            obj = new DateTime(buffer[index]);
                            break;
                        case SHORT:
                            obj = new DateTime(BitConverter.ToUInt16(buffer, index));
                            break;
                        case INT32:
                            obj = new DateTime(BitConverter.ToInt32(buffer, index));
                            break;
                        case LONG:
                            obj = new DateTime(BitConverter.ToInt64(buffer, index));
                            break;
                    }
                    index += num;
                    break;
                case TypeCode.Decimal:
                    fixed (byte* ptr = &buffer[index - 1])
                    {
                        obj = Marshal.PtrToStructure<decimal>((IntPtr)ptr);
                        index += 15;
                    }
                    break;
                case TypeCode.String:
                    index--;
                    num = (int)ReadValue(TypeCode.Int32, buffer, ref index);
                    obj = Encoding.UTF8.GetString(buffer, index, num);
                    index += num;
                    break;
            }
            return obj;
        }

        /// <summary>
        /// 序列化数组实体
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="array"></param>
        private unsafe static void WriteArray(Segment stream, IList array, Type itemType, bool recordType, bool ignore)
        {
            int len = array.Count;
            if (len == 0)
                return;
            WriteValue(stream, len);
            if (itemType.IsPrimitive)
            {
                int num;
                if (itemType == typeof(byte) | itemType == typeof(sbyte) | itemType == typeof(bool))
                    num = 1;
                else if (itemType == typeof(short) | itemType == typeof(ushort))
                    num = 2;
                else if (itemType == typeof(int) | itemType == typeof(uint) | itemType == typeof(float))
                    num = 4;
                else if (itemType == typeof(long) | itemType == typeof(ulong) | itemType == typeof(double))
                    num = 8;
                else throw new Exception($"无法识别的基础类型:{itemType}!");
                object item;
                if (!(array is Array))
                    item = array.GetType().GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(array);
                else item = array;
                var handle = GCHandle.Alloc(item, GCHandleType.Pinned);
                var ptr = handle.AddrOfPinnedObject();
                int size = len * num;
                var buf = BufferPool.Take(size);
                Marshal.Copy(ptr, buf, 0, size);
                handle.Free();//防止内存复制当中, gc回收! 内存已被释放问题
                stream.Write(buf, 0, size);
                BufferPool.Push(buf);
            }
            else
            {
                var bitLen = ((len - 1) / 8) + 1;
                byte[] bits = new byte[bitLen];
                int strPos = stream.Position;
                stream.Write(bits, 0, bitLen);
                for (int i = 0; i < len; i++)
                {
                    var arr1 = array[i];
                    int bitInx1 = i % 8;
                    int bitPos = i / 8;
                    if (arr1 == null)
                        continue;
                    SetBit(ref bits[bitPos], bitInx1 + 1, true);
                    if (recordType)
                    {
                        Type type = arr1.GetType();
                        stream.Write(BitConverter.GetBytes(TypeToIndex(type)), 0, 2);
                        WriteObject(stream, type, arr1, recordType, ignore);
                    }    
                    else WriteObject(stream, itemType, arr1, recordType, ignore);
                }
                int strLen = stream.Position;
                stream.Position = strPos;
                stream.Write(bits, 0, bitLen);
                stream.Position = strLen;
            }
        }

        /// <summary>
        /// 反序列化数组
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="array"></param>
        private unsafe static void ReadArray(byte[] buffer, ref int index, ref IList array, Type itemType, bool recordType, bool ignore)
        {
            if (itemType.IsPrimitive)
            {
                int len = array.Count;
                int num;
                if (itemType == typeof(byte) | itemType == typeof(sbyte) | itemType == typeof(bool))
                    num = 1;
                else if (itemType == typeof(short) | itemType == typeof(ushort))
                    num = 2;
                else if (itemType == typeof(int) | itemType == typeof(uint) | itemType == typeof(float))
                    num = 4;
                else if (itemType == typeof(long) | itemType == typeof(ulong) | itemType == typeof(double))
                    num = 8;
                else throw new Exception($"无法识别的基础类型:{itemType}!");
                object item;
                if (!(array is Array))
                    item = array.GetType().GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(array);
                else item = array;
                if (buffer.Length - index < len * num)
                {
                    index = buffer.Length - 1;
                    return;
                }
                var handle = GCHandle.Alloc(item, GCHandleType.Pinned);
                var ptr = handle.AddrOfPinnedObject();
                Marshal.Copy(buffer, index, ptr, len * num);
                handle.Free();
                index += len * num;
            }
            else 
            {
                var bitLen = ((array.Count - 1) / 8) + 1;
                byte[] bits = new byte[bitLen];
                Array.Copy(buffer, index, bits, 0, bitLen);
                index += bitLen;
                for (int i = 0; i < array.Count; i++)
                {
                    int bitInx1 = i % 8;
                    int bitPos = i / 8;
                    if (!GetBit(bits[bitPos], (byte)(++bitInx1)))
                        continue;
                    if (recordType)
                    {
                        Type type = IndexToType(BitConverter.ToUInt16(buffer, index));
                        index += 2;
                        array[i] = ReadObject(buffer, ref index, type, recordType, ignore);
                    }
                    else array[i] = ReadObject(buffer, ref index, itemType, recordType, ignore);
                }
            }
        }

        public static byte[] Serialize(string func, params object[] pars)
        {
            var stream = BufferPool.Take();
            byte[] buffer1 = new byte[0];
            try
            {
                WriteValue(stream, func);
                foreach (var obj in pars)
                {
                    if (obj == null)
                        continue;
                    Type type = obj.GetType();
                    byte[] typeBytes = BitConverter.GetBytes(TypeToIndex(type));
                    stream.Write(typeBytes, 0, 2);
                    WriteObject(stream, type, obj, false, false);
                }
                buffer1 = stream.ToArray();
            }
            catch (Exception ex)
            {
                string str = "函数:" + func + " 参数:";
                foreach (var obj in pars)
                    if (obj == null)
                        str += $"[null]";
                    else
                        str += $"[{obj}]";
                NDebug.LogError("序列化:" + str + "方法出错 详细信息:" + ex);
            }
            finally
            {
                BufferPool.Push(stream);
            }
            return buffer1;
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
                if (hasFunc) WriteValue(stream, model.func);
                if (hasMask) WriteValue(stream, model.methodMask);
                foreach (var obj in model.pars)
                {
                    Type type = obj.GetType();
                    byte[] typeBytes = BitConverter.GetBytes(TypeToIndex(type));
                    stream.Write(typeBytes, 0, 2);
                    WriteObject(stream, type, obj, false, false);
                }
                buffer1 = stream.ToArray();
            }
            catch (Exception ex)
            {
                string str = "函数:" + model.func + " 参数:";
                foreach (var obj in model.pars)
                    if (obj == null)
                        str += $"[null]";
                    else
                        str += $"[{obj}]";
                NDebug.LogError("序列化:" + str + "方法出错 详细信息:" + ex);
            }
            finally
            {
                BufferPool.Push(stream);
            }
            return buffer1;
        }

        /// <summary>
        /// 序列化对象, 记录类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] SerializeComplex(object obj, bool recordType = false, bool ignore = false)
        {
            var stream = BufferPool.Take();
            byte[] buffer1 = new byte[0];
            try
            {
                if (obj == null)
                    return stream.ToArray();
                Type type = obj.GetType();
                byte[] typeBytes = BitConverter.GetBytes(TypeToIndex(type));
                stream.Write(typeBytes, 0, 2);
                WriteObject(stream, type, obj, recordType, ignore);
                buffer1 = stream.ToArray();
            }
            catch (Exception ex)
            {
                NDebug.LogError("序列化:" + obj + "出错 详细信息:" + ex);
            }
            finally
            {
                BufferPool.Push(stream);
            }
            return buffer1;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="recordType"></param>
        /// <returns></returns>
        public static Segment Serialize(object obj, bool recordType = false, bool ignore = false)
        {
            var stream = BufferPool.Take();
            try
            {
                if (obj == null)
                    return default;
                Type type = obj.GetType();
                byte[] typeBytes = BitConverter.GetBytes(TypeToIndex(type));
                stream.Write(typeBytes, 0, 2);
                WriteObject(stream, type, obj, recordType, ignore);
            }
            catch (Exception ex)
            {
                NDebug.LogError("序列化:" + obj + "出错 详细信息:" + ex);
            }
            finally
            {
                stream.Count = stream.Position;
            }
            return stream;
        }

        /// <summary>
        /// 序列化对象, 不记录反序列化类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Segment SerializeObject<T>(T obj, bool recordType = false, bool ignore = false)
        {
            var stream = BufferPool.Take();
            try
            {
                if (obj == null)
                    return stream;
                Type type = obj.GetType();
                WriteObject(stream, type, obj, recordType, ignore);
            }
            catch (Exception ex)
            {
                NDebug.LogError("序列化:" + obj + "出错 详细信息:" + ex);
            }
            finally 
            {
                stream.Count = stream.Position;
                stream.Position = 0;
            }
            return stream;
        }

        /// <summary>
        /// 序列化对象, 不记录反序列化类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static void SerializeObject<T>(Segment stream, T obj, bool recordType = false, bool ignore = false)
        {
            try
            {
                if (obj == null)
                    return;
                Type type = obj.GetType();
                WriteObject(stream, type, obj, recordType, ignore);
            }
            catch (Exception ex)
            {
                NDebug.LogError("序列化:" + obj + "出错 详细信息:" + ex);
            }
            finally
            {
                stream.Count = stream.Position;
            }
        }

        private class Member
        {
            internal string name;
            internal bool IsPrimitive;
            internal bool IsEnum;
            internal bool IsArray;
            internal bool IsGenericType;
            internal Type Type;
            internal TypeCode TypeCode;
            internal Type ItemType;
            internal Type KeyType;
            internal Type ValueType;
#if !SERVICE
            internal Func<object, object> getValue;
            internal Action<object, object> setValue;
#endif
            internal virtual object GetValue(object obj)
            {
                return obj;
            }

            internal virtual void SetValue(ref object obj, object v)
            {
                obj = v;
            }

            public override string ToString()
            {
                return $"{name} {Type}";
            }
        }
#if SERVICE
        private class FPMember<T> : Member
        {
            internal CallSite<Func<CallSite, object, object>> getValueCall;
            internal CallSite<Func<CallSite, object, T, object>> setValueCall;
            internal override object GetValue(object obj)
            {
                return getValueCall.Target(getValueCall, obj);
            }
            internal override void SetValue(ref object obj, object v)
            {
                setValueCall.Target(setValueCall, obj, (T)v);
            }
        }
        private class FPArrayMember<T> : Member
        {
            internal CallSite<Func<CallSite, object, object>> getValueCall;
            internal CallSite<Func<CallSite, object, T[], object>> setValueCall;
            internal override object GetValue(object obj)
            {
                return getValueCall.Target(getValueCall, obj);
            }
            internal override void SetValue(ref object obj, object v)
            {
                setValueCall.Target(setValueCall, obj, (T[])v);
            }
        }
#else
        private class FPMember : Member
        {
            internal override object GetValue(object obj)
            {
                return getValue(obj);
            }
            internal override void SetValue(ref object obj, object v)
            {
                setValue(obj, v);
            }
        }
#endif
        private static Member[] GetMembers(Type type)
        {
            if (!map.TryGetValue(type, out Member[] members2))
            {
                var members1 = new List<Member>();
                if (type.IsArray | type.IsGenericType) 
                {
                    Member member1 = GetFPMember(null, type, type.FullName, false);
                    members1.Add(member1);
                }
                else
                {
                    var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                    var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    var members = new List<MemberInfo>(fields);
                    members.AddRange(properties);
                    foreach (var member in members)
                    {
                        if (member.GetCustomAttribute(nonSerialized) != null)
                            continue;
                        Member member1;
                        if (member.MemberType == MemberTypes.Field)
                        {
                            var field = member as FieldInfo;
                            var fType = field.FieldType;
                            if (fType == typeof(Type) | fType == typeof(object))
                                continue;
                            var constructors = fType.GetConstructors();
                            bool hasConstructor = false;
                            foreach (var constructor in constructors)
                            {
                                if (constructor.GetParameters().Length == 0)
                                {
                                    hasConstructor = true;
                                    break;
                                }
                            }
                            var code = Type.GetTypeCode(fType);
                            if (!hasConstructor & code == TypeCode.Object & !fType.IsValueType)//string问题
                                continue;
                            member1 = GetFPMember(type, fType, field.Name, true);
#if !SERVICE
                            if (fType.IsSubclassOf(typeof(UnityEngine.Object)) | fType == typeof(UnityEngine.Object))
                                continue;
                            member1.getValue = field.GetValue;
                            member1.setValue = field.SetValue;
#endif
                            members1.Add(member1);
                        }
                        else if (member.MemberType == MemberTypes.Property)
                        {
                            var property = member as PropertyInfo;
                            if (!property.CanRead | !property.CanWrite)
                                continue;
                            if (property.GetIndexParameters().Length > 0)
                                continue;
                            var pType = property.PropertyType;
                            if (pType == typeof(Type) | pType == typeof(object))
                                continue;
                            var constructors = pType.GetConstructors();
                            bool hasConstructor = false;
                            foreach (var constructor in constructors)
                            {
                                if (constructor.GetParameters().Length == 0)
                                {
                                    hasConstructor = true;
                                    break;
                                }
                            }
                            var code = Type.GetTypeCode(pType);
                            if (!hasConstructor & code == TypeCode.Object & !pType.IsValueType)//string问题
                                continue;
                            member1 = GetFPMember(type, pType, property.Name, true);
#if !SERVICE
                            if (pType.IsSubclassOf(typeof(UnityEngine.Object)) | pType == typeof(UnityEngine.Object))
                                continue;
                            member1.getValue = property.GetValue;
                            member1.setValue = property.SetValue;
#endif
                            members1.Add(member1);
                        }
                    }
                }
                map.Add(type, members2 = members1.ToArray());
            }
            return members2;
        }

        private static Member GetFPMember(Type type, Type fpType, string Name, bool isClassField) 
        {
#if SERVICE
            object getValueCall = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(0, Name, type, new CSharpArgumentInfo[]
            {
                CSharpArgumentInfo.Create(0, null)
            }));
            var csType = typeof(Func<,,,>).MakeGenericType(typeof(CallSite), typeof(object), fpType, typeof(object));
            var setValueCall = CallSite.Create(csType, Binder.SetMember(0, Name, type, new CSharpArgumentInfo[]
            {
                CSharpArgumentInfo.Create(0, null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
            }));
            var pm = typeof(FPMember<>).MakeGenericType(new Type[] { fpType });
            var member1 = (Member)Activator.CreateInstance(pm);
#else
            Member member1;
            if (!isClassField)
                member1 = new Member();
            else
                member1 = new FPMember();
#endif
            if (fpType.IsArray)
            {
                Type itemType = fpType.GetInterface(typeof(IList<>).FullName).GenericTypeArguments[0];
#if SERVICE
                if (isClassField)
                {
                    var pm1 = typeof(FPArrayMember<>).MakeGenericType(new Type[] { itemType });
                    member1 = (Member)Activator.CreateInstance(pm1);
                    csType = typeof(Func<,,,>).MakeGenericType(typeof(CallSite), typeof(object), fpType, typeof(object));
                    setValueCall = CallSite.Create(csType, Binder.SetMember(0, Name, type, new CSharpArgumentInfo[]
                    {
                        CSharpArgumentInfo.Create(0, null),
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
                    }));
                }
                else 
                {
                    member1 = new Member();
                    getValueCall = null;
                    setValueCall = null;
                }
#endif
                member1.ItemType = itemType;
            }
            else if (fpType.IsGenericType)
            {
#if SERVICE
                if (!isClassField) 
                {
                    member1 = new Member();
                    getValueCall = null;
                    setValueCall = null;
                }
#endif
                if (fpType.GenericTypeArguments.Length == 1)
                {
                    Type itemType = fpType.GenericTypeArguments[0];
                    member1.ItemType = itemType;
                }
                else if (fpType.GenericTypeArguments.Length == 2)
                {
                    Type keyType = fpType.GenericTypeArguments[0];
                    Type valueType = fpType.GenericTypeArguments[1];
                    member1.KeyType = keyType;
                    member1.ValueType = valueType;
                }
            }
#if SERVICE
            if (setValueCall != null & getValueCall != null)
            {
                var callS = member1.GetType().GetField("setValueCall", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                callS.SetValue(member1, setValueCall);
                var callS1 = member1.GetType().GetField("getValueCall", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                callS1.SetValue(member1, getValueCall);
            }
#endif
            member1.name = Name;
            member1.IsPrimitive = fpType.IsPrimitive | (fpType == typeof(string)) | (fpType == typeof(decimal)) | (fpType == typeof(DateTime));
            member1.IsEnum = fpType.IsEnum;
            member1.IsArray = fpType.IsArray;
            member1.IsGenericType = fpType.IsGenericType;
            member1.Type = fpType;
            member1.TypeCode = Type.GetTypeCode(fpType);
            return member1;
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="type"></param>
        /// <param name="target"></param>
        /// <param name="recordType"></param>
        /// <param name="ignore">忽略不使用<see cref="AddBaseType"/>方法也会被序列化</param>
        private static void WriteObject<T>(Segment stream, Type type, T target, bool recordType, bool ignore)
        {
            var members = GetMembers(type);
            var bitLen = ((members.Length - 1) / 8) + 1;
            byte[] bits = new byte[bitLen];
            var strPos = stream.Position;
            stream.Position += bitLen;
            for (byte i = 0; i < members.Length; i++)
            {
                var member = members[i];
                object value = member.GetValue(target);
                if (value == null)
                    continue;
                int bitInx1 = i % 8;
                int bitPos = i / 8;
                if (member.IsPrimitive)
                {
                    if (WriteValue(stream, value))
                        SetBit(ref bits[bitPos], bitInx1 + 1, true);
                }
                else if (member.IsEnum)
                {
                    var enumValue = (byte)value.GetHashCode();
                    if (enumValue == 0)
                        continue;
                    stream.WriteByte(enumValue);
                    SetBit(ref bits[bitPos], bitInx1 + 1, true);
                }
                else if (member.IsArray | member.IsGenericType)
                {
                    if (member.ItemType != null)
                    {
                        var array = (IList)value;
                        if (array.Count == 0)
                            continue;
                        SetBit(ref bits[bitPos], bitInx1 + 1, true);
                        WriteArray(stream, array, member.ItemType, recordType, ignore);
                    }
                    else
                    {
                        IDictionary array = (IDictionary)value;
                        if (array.Count == 0)
                            continue;
                        SetBit(ref bits[bitPos], bitInx1 + 1, true);
                        Array keys = Array.CreateInstance(member.KeyType, array.Count);
                        Array values = Array.CreateInstance(member.ValueType, array.Count);
                        array.Keys.CopyTo(keys, 0);
                        array.Values.CopyTo(values, 0);
                        WriteArray(stream, keys, member.KeyType, recordType, ignore);
                        WriteArray(stream, values, member.ValueType, recordType, ignore);
                    }
                }
                else if (networkType1s.ContainsKey(member.Type) | ignore)
                {
                    SetBit(ref bits[bitPos], bitInx1 + 1, true);
                    WriteObject(stream, member.Type, value, recordType, ignore);
                }
                else throw new Exception($"你没有标记此类[{member.Type}]为可序列化! 请使用NetConvertBinary.AddNetworkType<T>()方法进行添加此类为可序列化类型!");
            }
            var strLen = stream.Position;
            stream.Position = strPos;
            stream.Write(bits, 0, bitLen);
            stream.Position = strLen;
        }

        public static FuncData Deserialize(byte[] buffer, int index, int count, bool ignore = false)
        {
            FuncData obj = default;
            try
            {
                count += index;
                var func = (string)ReadValue(TypeCode.String, buffer, ref index);
                obj.name = func;
                List<object> list = new List<object>();
                while (index < count)
                {
                    Type type = IndexToType(BitConverter.ToUInt16(buffer, index));
                    if (type == null)
                        break;
                    index += 2;
                    var obj1 = ReadObject(buffer, ref index, type, false, ignore);
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

        public static FuncData DeserializeModel(byte[] buffer, int index, int count, bool ignore = false)
        {
            FuncData obj = default;
            try
            {
                count += index;
                byte head = buffer[index++];
                bool hasFunc = GetBit(head, 1);
                bool hasMask = GetBit(head, 2);
                if(hasFunc) obj.name = (string)ReadValue(TypeCode.String, buffer, ref index);
                if(hasMask) obj.mask = (ushort)ReadValue(TypeCode.UInt16, buffer, ref index);
                List<object> list = new List<object>();
                while (index < count)
                {
                    Type type = IndexToType(BitConverter.ToUInt16(buffer, index));
                    if (type == null)
                        break;
                    index += 2;
                    var obj1 = ReadObject(buffer, ref index, type, false, ignore);
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

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="segment"></param>
        /// <param name="recordType"></param>
        /// <param name="ignore">忽略不使用<see cref="AddBaseType"/>方法也会被序列化</param>
        /// <returns></returns>
        public static T DeserializeObject<T>(Segment segment, bool recordType = false, bool ignore = false)
        {
            T obj = default;
            int index = segment.Index + segment.Position;
            int count = segment.Index + segment.Count;
            while (index < count)
            {
                Type type = typeof(T);
                obj = (T)ReadObject(segment.Buffer, ref index, type, recordType, ignore);
            }
            BufferPool.Push(segment);
            return obj;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <param name="recordType">序列化的类型字段是 object[]字段时, 可以帮你记录object的绝对类型</param>
        /// <param name="ignore">忽略不使用<see cref="AddBaseType"/>方法也会被序列化</param>
        /// <returns></returns>
        public static T DeserializeObject<T>(byte[] buffer, int index, int count, bool recordType = false, bool ignore = false)
        {
            T obj = default;
            while (index < count)
            {
                Type type = typeof(T);
                obj = (T)ReadObject(buffer, ref index, type, recordType, ignore);
            }
            return obj;
        }

        /// <summary>
        /// 反序列化复杂类型
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public static object DeserializeComplex(byte[] buffer, int index, int count, bool recordType = false, bool ignore = false)
        {
            object obj = null;
            while (index < count)
            {
                Type type = IndexToType(BitConverter.ToUInt16(buffer, index));
                if (type == null)
                    break;
                index += 2;
                obj = ReadObject(buffer, ref index, type, recordType, ignore);
            }
            return obj;
        }

        public static object Deserialize(Segment segment, bool ignore = false)
        {
            object obj = null;
            int index = segment.Index + segment.Position;
            int count = segment.Index + segment.Count;
            while (index < count)
            {
                Type type = IndexToType(BitConverter.ToUInt16(segment.Buffer, index));
                if (type == null)
                    break;
                index += 2;
                obj = ReadObject(segment.Buffer, ref index, type, false, ignore);
            }
            BufferPool.Push(segment);
            return obj;
        }

        /// <summary>
        /// 反序列化实体对象
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static object ReadObject(byte[] buffer, ref int index, Type type, bool recordType, bool ignore)
        {
            object obj;
            if (type == typeof(string)) obj = string.Empty;
            else if (type.IsArray) obj = default;
            else obj = Activator.CreateInstance(type);
            var members = GetMembers(type);
            var bitLen = ((members.Length - 1) / 8) + 1;
            byte[] bits = new byte[bitLen];
            Array.Copy(buffer, index, bits, 0, bitLen);
            index += bitLen;
            for (int i = 0; i < members.Length; i++)
            {
                int bitInx1 = i % 8;
                int bitPos = i / 8;
                if (!GetBit(bits[bitPos], (byte)(++bitInx1)))
                    continue;
                var member = members[i];
                if (member.IsPrimitive)//如果是基础类型
                {
                    member.SetValue(ref obj, ReadValue(member.TypeCode, buffer, ref index));
                }
                else if (member.IsEnum)//如果是枚举类型
                {
                    member.SetValue(ref obj, Enum.ToObject(member.Type, buffer[index++]));
                }
                else if (member.IsArray)//如果是数组类型
                {
                    int arrCount = (int)ReadValue(TypeCode.Int32, buffer, ref index);
                    IList array = (IList)Activator.CreateInstance(member.Type, arrCount);
                    ReadArray(buffer, ref index, ref array, member.ItemType, recordType, ignore);
                    member.SetValue(ref obj, array);
                }
                else if (member.IsGenericType)
                {
                    if (member.ItemType != null)
                    {
                        int arrCount = (int)ReadValue(TypeCode.Int32, buffer, ref index);
                        var array1 = Array.CreateInstance(member.ItemType, arrCount);
                        IList array = (IList)Activator.CreateInstance(member.Type, array1);
                        ReadArray(buffer, ref index, ref array, member.ItemType, recordType, ignore);
                        member.SetValue(ref obj, array);
                    }
                    else
                    {
                        int arrCount = (int)ReadValue(TypeCode.Int32, buffer, ref index);
                        IList array = Array.CreateInstance(member.KeyType, arrCount);
                        ReadArray(buffer, ref index, ref array, member.KeyType, recordType, ignore);
                        arrCount = (int)ReadValue(TypeCode.Int32, buffer, ref index);
                        IList array1 = Array.CreateInstance(member.ValueType, arrCount);
                        ReadArray(buffer, ref index, ref array1, member.ValueType, recordType, ignore);
                        IDictionary dictionary = (IDictionary)Activator.CreateInstance(member.Type);
                        for (int a = 0; a < arrCount; a++)
                            dictionary.Add(array[a], array1[a]);
                        member.SetValue(ref obj, dictionary);
                    }
                }
                else if (networkType1s.ContainsKey(member.Type) | ignore)//如果是序列化类型
                {
                    member.SetValue(ref obj, ReadObject(buffer, ref index, member.Type, recordType, ignore));
                }
                else throw new Exception($"你没有标记此类[{member.Type}]为可序列化! 请使用NetConvertBinary.AddNetworkType<T>()方法进行添加此类为可序列化类型!");
            }
            return obj;
        }
    }
}