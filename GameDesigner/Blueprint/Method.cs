using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace GameDesigner
{
    /// <summary>
    /// 方法信息处理器
    /// </summary>

    [System.Serializable]
    public class Method
    {
        public string name = "";
        public string nodeName = "";
        public string xmlTexts = "";

        public ValueType targetValue = new ValueType();
        public object target
        {
            get
            {
                return targetValue.Value;
            }
            set
            {
                targetValue.Value = value;
            }
        }

        public string targetTypeName
        {
            get
            {
                return targetValue.ValueTypeName;
            }
            set
            {
                targetValue.ValueTypeName = value;
            }
        }

        public System.Type targetType
        {
            get
            {
                return targetValue.valueType;
            }
            set
            {
                targetValue.valueType = value;
            }
        }

        public string returnTypeName = "";//存储唯一方法返回类型名
        private System.Type _returnType = null;
        public System.Type returnType
        {
            get
            {
                if (_returnType == null)
                {
                    _returnType = SystemType.GetType(returnTypeName);
                    if (_returnType == null)
                        _returnType = typeof(System.Type);
                }
                return _returnType;
            }
        }

        private object _returnValue = null;//存储唯一方法返回类型的值
        public object returnValue
        {
            get
            {
                if (memberTypes == MemberTypes.Custom)
                    return _returnValue = target;
                return _returnValue;
            }
            set { _returnValue = value; }
        }

        public List<Parameter> Parameters = new List<Parameter>();//方法的参数数组
        public MemberTypes memberTypes = MemberTypes.Method;

        [HideInInspector] public ObjectModel typeModel = ObjectModel.Object;
        [HideInInspector] public int index = 0;
        [HideInInspector] public bool info = false;
        ///	编辑器扩展选择Types类型数组中的唯一一个数组索引
        [HideInInspector] public ValueType.TypeParameter typeName = new ValueType.TypeParameter();

        private MethodInfo _methodInfo;
        public MethodInfo methodInfo
        {
            get
            {
                if (_methodInfo == null)
                    _methodInfo = GetMethod(SystemType.GetType(targetTypeName), this);
                return _methodInfo;
            }
        }

        /// 优化方法参数存储等待使用
        private ParameterInfo[] _parameters = new ParameterInfo[0];
        public ParameterInfo[] parameters
        {
            get
            {
                if (_parameters == null)
                    _parameters = new ParameterInfo[0];
                return _parameters;
            }
            set { _parameters = value; }
        }

        /// 优化方法参数赋值存储等待使用
        private object[] _parameterobjs = new object[0];
        public object[] parameterobjs
        {
            get
            {
                if (_parameterobjs == null)
                    _parameterobjs = new object[0];
                return _parameterobjs;
            }
            set { _parameterobjs = value; }
        }

        public List<Parameter> genericArguments = new List<Parameter>();
        private System.Type[] _typeArguments = new System.Type[0];
        public System.Type[] typeArguments
        {
            get
            {
                if (_typeArguments.Length != genericArguments.Count)
                    _typeArguments = new System.Type[genericArguments.Count];
                for (int i = 0; i < _typeArguments.Length; ++i)
                {
                    _typeArguments[i] = (System.Type)genericArguments[i].value.Value;
                }
                return _typeArguments;
            }
        }

        public object image = null;

        /// 设置值或获得值模式--Invoke和Set是一样的,只是名称看起来哪个好些,Return和Get也是一样的
        public enum ValueModel
        {
            Get, Set
        }

        /// 设置值或获得值模式--Invoke和Set是一样的,只是名称看起来哪个好些,Return和Get也是一样的
        public ValueModel valueModel = ValueModel.Set;

        [SerializeField]
        [HideInInspector]
        private bool isVirtual = false;
        public bool IsVirtual
        {
            get { return isVirtual; }
            set { isVirtual = value; }
        }

        public Method()
        {
            memberTypes = MemberTypes.Method;
        }

        public Method(string Name, string returnType, string _targetTypeName)
        {
            name = Name;
            returnTypeName = returnType;
            targetTypeName = _targetTypeName;
            memberTypes = MemberTypes.Method;
        }

        /// <summary>
        /// 获得所有方法信息 ( type 类型 )
        /// </summary>

        static public List<Method> GetMethods(System.Type type)
        {
            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            List<Method> M = new List<Method>();
            int index = 0;
            foreach (MethodInfo ms in methods)
            {
                Method info = new Method(ms.Name, ms.ReturnType.FullName, type.FullName);
                if (ms.IsGenericMethod)
                {   //T无参方法
                    foreach (System.Type t in ms.GetGenericArguments())
                    {
                        info.genericArguments.Add(new Parameter(t.Name, "System.Type", typeof(System.Type), 0));
                    }
                }
                if (ms.ReturnType.IsGenericParameter) //T无参返回值
                    info.returnTypeName = "System.Object";
                int paramsindex = 0;
                foreach (ParameterInfo p in ms.GetParameters())
                {
                    info.Parameters.Add(new Parameter(p.Name, p.ParameterType.FullName, p.ParameterType, paramsindex));
                    if (p.ParameterType.IsGenericParameter) //T无参 参数 注意: T 被更改为Object对象, 请小心使用妥当
                        info.Parameters[paramsindex].parameterTypeName = "System.Object";
                    paramsindex++;
                }
                info.memberTypes = MemberTypes.Method;
                info.targetType = type;
                info.nodeName = "m_" + type.Name;
                info._methodInfo = ms;
                info.index = index;
                M.Add(info);
                index++;
            }
            return M;
        }

        static public MethodInfo GetMethod(object target, Method method)
        {
            return GetMethod(target.GetType(), method);
        }

        static public MethodInfo GetMethod(System.Type type, Method method)
        {
            bool isInvoke = false;
            int i = 0;
            foreach (MethodInfo m in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                if (m.Name == method.name & m.GetParameters().Length == method.Parameters.Count)
                {
                    if (method.Parameters.Count == 0)
                        return m;

                    foreach (ParameterInfo p in m.GetParameters())
                    {
                        if (p.ParameterType.FullName == method.Parameters[i].parameterTypeName | p.ParameterType.IsGenericParameter)
                        {
                            isInvoke = true;
                        }
                        else
                        {
                            isInvoke = false;
                            break;
                        }
                        ++i;
                    }
                    if (isInvoke)
                    {
                        method.parameters = m.GetParameters();
                        method.parameterobjs = new object[method.parameters.Length];
                        return m;
                    }
                }
            }
            return null;
        }

        static public Method GetMethod(object target, string methodName)
        {
            MethodInfo m = target.GetType().GetMethod(methodName);
            if (m == null)
            {
                return null;
            }
            Method method = new Method(m.Name, m.ReturnType.FullName, target.GetType().FullName);
            foreach (ParameterInfo p in m.GetParameters())
            {
                method.Parameters.Add(new Parameter(p.Name, p.ParameterType));
            }
            return method;
        }

        /// <summary>
        /// 方法自调 ( method 方法信息存储器--必须是完整的方法(包含对象,方法,参数) )
        /// </summary>

        static public object Invoke(Method method)
        {
            return Invoke(method.target, method, method.Parameters);
        }

        /// <summary>
        /// 调用唯一一个方法 , 使最优化，使尽可能有效 ( target 对象 , method 方法信息存储器--必须是完整的方法(方法,参数) )
        /// </summary>

        static public object Invoke(object target, Method method)
        {
            return Invoke(target, method, method.Parameters);
        }

        /// <summary>
        /// 调用唯一一个方法 , 使最优化，使尽可能有效 ( target 实体对象  , method 方法信息存储器 , parameters 参数信息存储器 )
        /// </summary>

        static public object Invoke(object target, Method method, List<Parameter> parameters)
        {
            if (target == null | method.methodInfo == null)
                return null;

            if (method.Parameters.Count == 0)
            {
                if (method.methodInfo.IsGenericMethod)
                    return method.returnValue = method.methodInfo.MakeGenericMethod(method.typeArguments).Invoke(target, null);
                return method.returnValue = method.methodInfo.Invoke(target, null);
            }

            for (int i = 0; i < method.Parameters.Count; ++i)
            {
                method.parameterobjs[i] = parameters[i].Value;
            }

            try
            {
                if (method.methodInfo.IsGenericMethod)
                    return method.returnValue = method.methodInfo.MakeGenericMethod(method.typeArguments).Invoke(target, method.parameterobjs);
                return method.returnValue = method.methodInfo.Invoke(target, method.parameterobjs);
            }
            catch { }

            return method.targetType;
        }
    }
}