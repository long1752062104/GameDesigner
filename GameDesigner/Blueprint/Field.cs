using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace GameDesigner
{
    /// <summary>
    /// 字段信息处理器
    /// </summary>

    [System.Serializable]
    public class Field : Constructor
    {
        private FieldInfo _fieldInfo = null;
        public FieldInfo fieldInfo
        {
            get
            {
                if (_fieldInfo == null)
                {
                    _fieldInfo = SystemType.GetType(targetTypeName).GetField(name);
                    parameterobjs = new object[1];
                }
                return _fieldInfo;
            }
        }

        public Field()
        {
            memberTypes = MemberTypes.Field;
        }

        public Field(string Name, string _type, string _targetTypeName)
        {
            name = Name;
            returnTypeName = _type;
            targetTypeName = _targetTypeName;
            memberTypes = MemberTypes.Field;
            valueModel = ValueModel.Get;
        }

        /// <summary>
        /// 获得所有字段信息 ( target 对象 )
        /// </summary>

        static public List<Field> GetFields(object target)
        {
            return GetFields(target.GetType());
        }

        /// <summary>
        /// 获得所有字段信息 ( type类名 )
        /// </summary>

        static public List<Field> GetFields(System.Type type)
        {
            FieldInfo[] fields = type.GetFields();
            List<Field> M = new List<Field>();
            int index = 0;
            foreach (FieldInfo ms in fields)
            {
                Field info = new Field(ms.Name, ms.FieldType.FullName, type.FullName);
                info.Parameters.Add(new Parameter(ms.Name, ms.FieldType.FullName, ms.FieldType));
                info.parameterobjs = new object[1];
                info.memberTypes = MemberTypes.Field;
                info._fieldInfo = ms;
                info.nodeName = "m_" + type.Name;
                info.index = index;
                M.Add(info);
                index++;
            }
            return M;
        }

        /// <summary>
        /// 调用变量进行赋值 ( target 实体对象  , ifield 字段信息管理 )
        /// </summary>

        static public object Invoke(object target, Field field)
        {
            switch (field.valueModel)
            {
                case ValueModel.Get:
                    field.returnValue = field.fieldInfo.GetValue(target);
                    field.Parameters[0].Value = field.returnValue;
                    break;
                case ValueModel.Set:
                    field.fieldInfo.SetValue(target, field.Parameters[0].Value);
                    break;
                default:
                    Debug.LogWarning("属性没有这样的模式,请切换模式(Set,Get)!");
                    break;
            }
            return field.returnValue;
        }

        /// <summary>
        /// 调用变量进行赋值 ( target 实体对象  , ifield 字段信息管理 )
        /// </summary>

        static public object InvokeField(object target, Field field)
        {
            switch (field.valueModel)
            {
                case ValueModel.Get:
                    return field.returnValue = field.fieldInfo.GetValue(target);
                case ValueModel.Set:
                    field.fieldInfo.SetValue(target, field.Parameters[0].Value);
                    break;
                default:
                    Debug.LogWarning("字段没有这样的模式,请切换模式(Set,Get)!");
                    break;
            }
            return field.returnValue;
        }
    }
}