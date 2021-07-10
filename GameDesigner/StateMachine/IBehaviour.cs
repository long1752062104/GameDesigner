namespace GameDesigner
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public enum TypeCode
    {
        Empty = 0,
        Object = 1,
        DBNull = 2,
        Boolean = 3,
        Char = 4,
        SByte = 5,
        Byte = 6,
        Int16 = 7,
        UInt16 = 8,
        Int32 = 9,
        UInt32 = 10,
        Int64 = 11,
        UInt64 = 12,
        Single = 13,
        Double = 14,
        Decimal = 0xF,
        DateTime = 0x10,
        String = 18,
        Vector2,
        Vector3,
        Vector4,
        Rect,
        Color,
        Color32,
        Quaternion,
        AnimationCurve,
        GenericType,
        Array,
    }

    [Serializable]
    public class Metadata
    {
        public string name;
        public TypeCode type;
        public string typeName;
        public string data;
        public object target;
        public FieldInfo field;
        public Object Value;
        public List<Object> values = new List<Object>();
        private object _value;
        public object value
        { 
            get
            {
                if (target != null & field != null)
                    _value = field.GetValue(target);
                if (_value == null)
                    _value = Read();
                return _value;
            }
            set 
            {
                _value = value;
                if (target != null & field != null)
                    field.SetValue(target, _value);
                Write(_value);
            }
        }
        private Type _type;
        public Type Type 
        {
            get {
                if (_type == null)
                    _type = SystemType.GetType(typeName);
                return _type;
            }
        }
        public Type _itemType;
        public Type itemType
        {
            get
            {
                if (_itemType == null)
                    _itemType = Type.GetInterface(typeof(IList<>).FullName).GenericTypeArguments[0];
                return _itemType;
            }
        }
        public int arraySize;
        public bool foldout;

        public Metadata() { }
        public Metadata(string name, string fullName, TypeCode type, object target, FieldInfo field)
        {
            this.name = name;
            typeName = fullName;
            this.type = type;
            this.field = field;
            this.target = target;
            Write(value);
        }

        public object Read()
        {
            switch (type) 
            {
                case TypeCode.Byte:
                    return Convert.ToByte(data);
                case TypeCode.SByte:
                    return Convert.ToSByte(data);
                case TypeCode.Boolean:
                    return Convert.ToBoolean(data);
                case TypeCode.Int16:
                    return Convert.ToInt16(data);
                case TypeCode.UInt16:
                    return Convert.ToUInt16(data);
                case TypeCode.Char:
                    return Convert.ToChar(data);
                case TypeCode.Int32:
                    return Convert.ToInt32(data);
                case TypeCode.UInt32:
                    return Convert.ToUInt32(data);
                case TypeCode.Single:
                    return Convert.ToSingle(data);
                case TypeCode.Int64:
                    return Convert.ToInt64(data);
                case TypeCode.UInt64:
                    return Convert.ToUInt64(data);
                case TypeCode.Double:
                    return Convert.ToDouble(data);
                case TypeCode.String:
                    return data;
                case TypeCode.Object:
                    if (Value == null)
                        return null;
                    return Value;
                case TypeCode.Vector2:
                    var datas = data.Split(',');
                    return new Vector2(float.Parse(datas[0]), float.Parse(datas[1]));
                case TypeCode.Vector3:
                    var datas1 = data.Split(',');
                    return new Vector3(float.Parse(datas1[0]), float.Parse(datas1[1]), float.Parse(datas1[2]));
                case TypeCode.Vector4:
                    var datas2 = data.Split(',');
                    return new Vector4(float.Parse(datas2[0]), float.Parse(datas2[1]), float.Parse(datas2[2]), float.Parse(datas2[2]));
                case TypeCode.Quaternion:
                    var datas3 = data.Split(',');
                    return new Quaternion(float.Parse(datas3[0]), float.Parse(datas3[1]), float.Parse(datas3[2]), float.Parse(datas3[2]));
                case TypeCode.Rect:
                    var datas4 = data.Split(',');
                    return new Rect(float.Parse(datas4[0]), float.Parse(datas4[1]), float.Parse(datas4[2]), float.Parse(datas4[2]));
                case TypeCode.Color:
                    var datas5 = data.Split(',');
                    return new Color(float.Parse(datas5[0]), float.Parse(datas5[1]), float.Parse(datas5[2]), float.Parse(datas5[2]));
                case TypeCode.Color32:
                    var datas6 = data.Split(',');
                    return new Color32(byte.Parse(datas6[0]), byte.Parse(datas6[1]), byte.Parse(datas6[2]), byte.Parse(datas6[2]));
                case TypeCode.GenericType:
                    if (itemType == typeof(Object) | itemType.IsSubclassOf(typeof(Object)))
                    {
                        IList list = (IList)Activator.CreateInstance(Type);
                        for (int i = 0; i < values.Count; i++)
                        {
                            if (values[i] == null) 
                                list.Add(null);
                            else
                                list.Add(values[i]);
                        }
                        return list;
                    }
                    else return Newtonsoft_X.Json.JsonConvert.DeserializeObject(data, Type);
                case TypeCode.Array:
                    if (itemType == typeof(Object) | itemType.IsSubclassOf(typeof(Object)))
                    {
                        IList list = Array.CreateInstance(itemType, values.Count);
                        for (int i = 0; i < values.Count; i++)
                        {
                            if (values[i] == null) continue;
                            list[i] = values[i];
                        }
                        return list;
                    }
                    else return Newtonsoft_X.Json.JsonConvert.DeserializeObject(data, Type);
            }
            return null;
        }

        public void Write(object value)
        {
            if (type == TypeCode.Object)
            {
                Value = (Object)value;
            }
            else if (value != null)
            {
                if (type == TypeCode.Vector2)
                {
                    Vector2 v2 = (Vector2)value;
                    data = $"{v2.x},{v2.y}";
                }
                else if (type == TypeCode.Vector3)
                {
                    Vector3 v = (Vector3)value;
                    data = $"{v.x},{v.y},{v.z}";
                }
                else if (type == TypeCode.Vector4)
                {
                    Vector4 v = (Vector4)value;
                    data = $"{v.x},{v.y},{v.z},{v.w}";
                }
                else if (type == TypeCode.Quaternion)
                {
                    Quaternion v = (Quaternion)value;
                    data = $"{v.x},{v.y},{v.z},{v.w}";
                }
                else if (type == TypeCode.Rect)
                {
                    Rect v = (Rect)value;
                    data = $"{v.x},{v.y},{v.width},{v.height}";
                }
                else if (type == TypeCode.Color)
                {
                    Color v = (Color)value;
                    data = $"{v.r},{v.g},{v.b},{v.a}";
                }
                else if (type == TypeCode.Color32)
                {
                    Color32 v = (Color32)value;
                    data = $"{v.r},{v.g},{v.b},{v.a}";
                }
                else if (type == TypeCode.GenericType | type == TypeCode.Array)
                {
                    if (itemType == typeof(Object) | itemType.IsSubclassOf(typeof(Object)))
                    {
                        values.Clear();
                        IList list = (IList)value;
                        for (int i = 0; i < list.Count; i++)
                            values.Add(list[i] as Object);
                    }
                    else data = Newtonsoft_X.Json.JsonConvert.SerializeObject(value);
                }
                else data = value.ToString();
            }
            else
            {
                data = null;
            }
        }
    }

    /// <summary>
    /// 状态行为基类 2019.3.3
    /// </summary>
    [Serializable]
    public class IBehaviour
    {
        public string name;
        public int ID;
        /// <summary>
        /// 展开编辑器检视面板
        /// </summary>
        [HideInInspector]
        public bool show = true;
        /// <summary>
        /// 脚本是否启用?
        /// </summary>
        public bool Active = true;
        public List<Metadata> metadatas = new List<Metadata>();
        public StateMachine stateMachine;
        public StateManager stateManager => stateMachine.stateManager;
        /// <summary>
        /// 当前状态
        /// </summary>
        public State state => stateMachine.states[ID];
        /// <summary>
        /// 状态管理器转换组建
        /// </summary>
        public Transform transform => stateManager.transform;
        public Type Type { get { return SystemType.GetType(name); } }
        public void InitMetadatas(StateMachine stateMachine) 
        {
            var type = GetType();
            InitMetadatas(stateMachine, type);
        }
        public void InitMetadatas(StateMachine stateMachine, Type type) 
        {
            this.stateMachine = stateMachine;
            name = type.FullName;
            var fields = type.GetFields();
            metadatas.Clear();
            foreach (var field in fields)
            {
                if (type != field.DeclaringType | field.IsStatic)
                    continue;
                InitField(field);
            }
        }

        private void InitField(FieldInfo field) 
        {
            var code = Type.GetTypeCode(field.FieldType);
            if (code == System.TypeCode.Object)
            {
                if (field.FieldType.IsSubclassOf(typeof(Object)) | field.FieldType == typeof(Object))
                    metadatas.Add(new Metadata(field.Name, field.FieldType.FullName, TypeCode.Object, this, field));
                else if (field.FieldType == typeof(Vector2))
                    metadatas.Add(new Metadata(field.Name, field.FieldType.FullName, TypeCode.Vector2, this, field));
                else if (field.FieldType == typeof(Vector3))
                    metadatas.Add(new Metadata(field.Name, field.FieldType.FullName, TypeCode.Vector3, this, field));
                else if (field.FieldType == typeof(Vector4))
                    metadatas.Add(new Metadata(field.Name, field.FieldType.FullName, TypeCode.Vector4, this, field));
                else if (field.FieldType == typeof(Quaternion))
                    metadatas.Add(new Metadata(field.Name, field.FieldType.FullName, TypeCode.Quaternion, this, field));
                else if (field.FieldType == typeof(Rect))
                    metadatas.Add(new Metadata(field.Name, field.FieldType.FullName, TypeCode.Rect, this, field));
                else if (field.FieldType == typeof(Color))
                    metadatas.Add(new Metadata(field.Name, field.FieldType.FullName, TypeCode.Color, this, field));
                else if (field.FieldType == typeof(Color32))
                    metadatas.Add(new Metadata(field.Name, field.FieldType.FullName, TypeCode.Color32, this, field));
                else if (field.FieldType == typeof(AnimationCurve))
                    metadatas.Add(new Metadata(field.Name, field.FieldType.FullName, TypeCode.AnimationCurve, this, field));
                else if (field.FieldType.IsGenericType)
                {
                    var gta = field.FieldType.GenericTypeArguments;
                    if (gta.Length > 1)
                        return;
                    metadatas.Add(new Metadata(field.Name, field.FieldType.FullName, TypeCode.GenericType, this, field));
                } 
                else if (field.FieldType.IsArray)
                    metadatas.Add(new Metadata(field.Name, field.FieldType.FullName, TypeCode.Array, this, field));
            } 
            else metadatas.Add(new Metadata(field.Name, field.FieldType.FullName, (TypeCode)code, this, field));
        }

        public void Reload(Type type, StateMachine stateMachine, List<Metadata> metadatas)
        {
            InitMetadatas(stateMachine, type);
            foreach (var item in this.metadatas)
            {
                foreach (var item1 in metadatas)
                {
                    if (item.name == item1.name & item.typeName == item1.typeName) 
                    {
                        item.data = item1.data;
                        item.Value = item1.Value;
                        item.arraySize = item1.arraySize;
                        item.foldout = item1.foldout;
                        item.field.SetValue(this, item1.value);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 当初始化调用
        /// </summary>
        public virtual void OnInit() { }

        /// <summary>
        /// 当组件被删除调用一次
        /// </summary>
        public virtual void OnDestroyComponent() { }

        /// <summary>
        /// 当绘制编辑器检视面板 (重要提示!你想自定义编辑器检视面板则返回真,否则显示默认编辑器检视面板)
        /// </summary>
        /// <param name="state">当前状态</param>
        /// <returns></returns>
        public virtual bool OnInspectorGUI(State state)
        {
            return false; //返回假: 绘制默认监视面板 | 返回真: 绘制扩展自定义监视面板
        }

        public virtual void OnSceneGUI(State state) { }

        /// <summary>
        /// 进入下一个状态, 如果状态正在播放就不做任何处理, 如果想让动作立即播放可以使用 OnEnterNextState 方法
        /// </summary>
        /// <param name="stateID"></param>
        public void EnterState(int stateID) => stateManager.StatusEntry(stateID);

        /// <summary>
        /// 当进入下一个状态, 你也可以立即进入当前播放的状态, 如果不想进入当前播放的状态, 使用StatusEntry方法
        /// </summary>
        /// <param name="stateID">下一个状态的ID</param>
        public void OnEnterNextState(int stateID) => stateManager.EnterNextState(stateID);
    }
}