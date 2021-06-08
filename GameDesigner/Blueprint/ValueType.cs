using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameDesigner
{
    /// <summary>
    /// 值存储管理类
    /// </summary>

    [Serializable]
    public class ValueType
    {
        public ValueType() { }

        public ValueType(Type type)
        {
            ValueTypeName = type.FullName;
            _valueType = type;
        }

        public string ValueTypeName = "UnityEngine.Object";
        private Type _valueType = null;
        public Type valueType
        {
            get
            {
                if (_valueType == null)
                {
                    _valueType = SystemType.GetType(ValueTypeName);
                    if (_valueType == null)//获取类型失败后,使用基类代替
                        _valueType = typeof(Type);
                }
                return _valueType;
            }
            set
            {
                _valueType = value;
            }
        }

        public Object _Object = null;
        public object _object = null;
        public string _valueString = "";
        public object Value
        {
            get
            {
                if (_object == null)
                {
                    if (SystemType.IsSubclassOf(valueType, typeof(Object)))
                    {
                        if (ValueTypeName.Contains("[]") | ValueTypeName == "System.Array")
                        {
                            return _object = new Object[0];
                        }
                        else
                        {
                            _object = (_Object ? _Object : new Object());
                        }
                    }
                    else if (valueType.IsEnum)
                    {
                        try { _object = (Enum)Enum.Parse(valueType, _valueString); }
                        catch { _object = CreateUtility.CreateInstance(valueType); }
                    }
                    else if (valueType.IsValueType)
                    {
                        try { _object = SystemType.StringToValue(valueType.FullName, _valueString); }
                        catch { _object = CreateUtility.CreateInstance(valueType); }
                    }
                    else if (valueType.FullName == "System.String")
                        _object = _valueString;
                    else if (valueType.IsAbstract | ValueTypeName == "System.Type")
                        _object = SystemType.GetType(ValueTypeName);
                    else
                    {
                        _object = CreateUtility.CreateInstance(valueType);
                        if(_object==null)
                            _object = SystemType.GetType(ValueTypeName);
                    }
                }
                if (SystemType.IsSubclassOf(valueType, typeof(Object)))
                {
                    if (ValueTypeName.Contains("[]") | ValueTypeName == "System.Array")
                        return _object;
                    return _object = _Object;
                }
                return _object;
            }
            set
            {
                if (SystemType.IsSubclassOf(valueType, typeof(Object)))
                {
                    if (ValueTypeName.Contains("[]") | ValueTypeName == "System.Array")
                    {
                        if (value == null)
                            _object = new Object[0];
                        else
                            _object = (Object[])value;
                    }
                    else
                    {
                        if (value == null)
                            _object = _Object = new Object();
                        else
                            _object = _Object = (Object)value;
                    }
                }
                else
                {
                    if (ValueTypeName.Contains("[]") | ValueTypeName == "System.Array")
                    {
                        if (value == null)
                            _object = new object[0];
                        else
                            _object = value;
                    }
                    else
                    {
                        if (value == null)
                            _object = new object();
                        else
                            _object = value;
                    }
                }
            }
        }

        public virtual void ValueToString()
        {
            _valueString = Value.ToString();
        }

        [Serializable]
        public class TypeParameter
        {
            public string typeName = "UnityEngine.Object";
            private Type _type = typeof(Type);
            /// 当参数类型为System.Type时使用
            public Type type
            {
                get
                {
                    if (_type == null)
                    {
                        _type = SystemType.GetType(typeName);
                        if (_type == null)
                            _type = typeof(Type);
                    }
                    if (_type.FullName != typeName)
                    {
                        _type = SystemType.GetType(typeName);
                        if (_type == null)
                            _type = typeof(Type);
                    }
                    return _type;
                }
            }
        }

        ///	编辑器扩展选择Types类型数组中的唯一一个数组索引
        [HideInInspector]
        public TypeParameter typeName = new TypeParameter();
    }

}