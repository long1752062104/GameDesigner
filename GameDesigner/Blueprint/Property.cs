using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace GameDesigner
{
    /// <summary>
    /// 属性信息处理器 -- 方法属性信息处理类，类似方法信息类
    /// </summary>

    [System.Serializable]
    public class Property : Field
    {
        private PropertyInfo _propertyInfo = null;
        public PropertyInfo propertyInfo
        {
            get
            {
                if (_propertyInfo == null)
                {
                    _propertyInfo = SystemType.GetType(targetTypeName).GetProperty(name);
                    parameterobjs = new object[1];
                }
                return _propertyInfo;
            }
        }

        public Property()
        {
            memberTypes = MemberTypes.Property;
        }

        public Property(string Name, string _type, string _targetTypeName)
        {
            name = Name;
            returnTypeName = _type;
            targetTypeName = _targetTypeName;
            memberTypes = MemberTypes.Property;
            valueModel = ValueModel.Get;
        }

        /// <summary>
        /// 获得所有方法信息,这个方法有可能是属性的定义Unity的Object类 ( target 对象 , not 查找方法时遇到的字符 , con 包含为真时，当方法出现与not字符相同时将才获取 )  当前为包含，包含就是遇到与not字符相同才获取
        /// </summary>

        static public List<Property> GetPropertys(object objType)
        {
            return GetPropertys(objType.GetType());
        }

        /// <summary>
        /// 获得所有方法信息 ( target 对象 , not 查找方法时遇到的字符 , con 包含为真时，当方法出现与not字符相同时才获取 )  当前为包含，包含就是遇到与not字符相同才获取
        /// </summary>

        static public List<Property> GetPropertys(System.Type type)
        {
            PropertyInfo[] methods = type.GetProperties();
            List<Property> M = new List<Property>();
            int index = 0;
            foreach (PropertyInfo ms in methods)
            {
                Property info = new Property(ms.Name, ms.PropertyType.FullName, type.FullName);
                info.Parameters.Add(new Parameter(ms.Name, ms.PropertyType.FullName, ms.PropertyType));
                info.parameterobjs = new object[1];
                info.memberTypes = MemberTypes.Property;
                info._propertyInfo = ms;
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

        static public object Invoke(object target, Property proinfo)
        {
            try
            {
                switch (proinfo.valueModel)
                {
                    case ValueModel.Get:
                        if (!proinfo.propertyInfo.CanRead)
                        {
                            Debug.LogWarning("只写属性!");
                            return proinfo.returnValue;
                        }
                        proinfo.returnValue = proinfo.propertyInfo.GetGetMethod().Invoke(target, null);
                        proinfo.Parameters[0].Value = proinfo.returnValue;
                        break;
                    case ValueModel.Set:
                        if (!proinfo.propertyInfo.CanWrite)
                        {
                            Debug.LogWarning("只读属性!");
                            return proinfo.returnValue;
                        }
                        proinfo.parameterobjs[0] = proinfo.Parameters[0].Value;
                        proinfo.propertyInfo.GetSetMethod().Invoke(target, proinfo.parameterobjs);
                        proinfo.returnValue = proinfo.parameterobjs[0];
                        break;
                    default:
                        Debug.LogWarning("属性没有这样的模式,请切换模式(Set,Get)!");
                        break;
                }
            }
            catch { }
            return proinfo.returnValue;
        }

        /// <summary>
        /// 调用变量进行赋值 ( target 实体对象  , ifield 字段信息管理 )
        /// </summary>

        static public object InvokeProperty(object target, Property proinfo)
        {
            switch (proinfo.valueModel)
            {
                case ValueModel.Get:
                    if (!proinfo.propertyInfo.CanRead)
                    {
                        Debug.LogWarning("只写属性!");
                        return proinfo.returnValue;
                    }
                    return proinfo.returnValue = proinfo.propertyInfo.GetGetMethod().Invoke(target, null);
                case ValueModel.Set:
                    if (!proinfo.propertyInfo.CanWrite)
                    {
                        Debug.LogWarning("只读属性!");
                        return proinfo.returnValue;
                    }
                    proinfo.parameterobjs[0] = proinfo.Parameters[0].Value;
                    proinfo.propertyInfo.GetSetMethod().Invoke(target, proinfo.parameterobjs);
                    break;
                default:
                    Debug.LogWarning("属性没有这样的模式,请切换模式(Set,Get)!");
                    break;
            }
            return proinfo.returnValue;
        }
    }
}