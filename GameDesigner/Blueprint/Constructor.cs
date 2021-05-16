using System.Collections.Generic;
using System.Reflection;

namespace GameDesigner
{
    /// <summary>
    /// 构造器
    /// </summary>
    [System.Serializable]
    public class Constructor : Method
    {
        public Constructor() { memberTypes = MemberTypes.Constructor; }

        public Constructor(string Name, string _targetTypeName, MemberTypes _memberTypes = MemberTypes.Constructor, string targetName = "m_XXX")
        {
            name = Name;
            nodeName = targetName;
            targetTypeName = _targetTypeName;
            memberTypes = _memberTypes;
        }

        private ConstructorInfo _constructorInfo = null;
        public ConstructorInfo constructorInfo
        {
            get
            {
                if (_constructorInfo == null)
                {
                    foreach (ConstructorInfo m in SystemType.GetType(targetTypeName).GetConstructors())
                    {
                        bool isInvoke = false;
                        parameterobjs = new object[m.GetParameters().Length];
                        if (parameterobjs.Length == Parameters.Count)
                        {
                            if (parameterobjs.Length == 0)
                                return _constructorInfo = m;

                            parameters = m.GetParameters();
                            for (int i = 0; i < parameters.Length; ++i)
                            {
                                if (parameters[i].ParameterType.FullName == Parameters[i].parameterTypeName)
                                {
                                    isInvoke = true;
                                }
                                else
                                {
                                    isInvoke = false;
                                    break;
                                }
                            }
                            if (isInvoke)
                            {
                                return _constructorInfo = m;
                            }
                        }
                    }
                }
                return _constructorInfo;
            }
        }

        /// <summary>
        /// 获得所有方法信息 ( typeName 要获得的类名 )
        /// </summary>

        static public List<Constructor> GetConstructors(string typeName)
        {
            return GetConstructors(SystemType.GetType(typeName));
        }

        /// <summary>
        /// 获得所有方法信息 ( typeName 要获得的类名 )
        /// </summary>

        static public List<Constructor> GetConstructors(System.Type type)
        {
            List<Constructor> M = new List<Constructor>();
            M.Add(new Constructor(type.Name + " (target)", type.FullName, MemberTypes.Custom, "" + type.Name));
            foreach (ConstructorInfo ms in type.GetConstructors())
            {
                Constructor info = new Constructor("New " + type.Name, type.FullName, MemberTypes.Constructor, "new " + type.Name + "()");
                foreach (ParameterInfo p in ms.GetParameters())
                {
                    info.Parameters.Add(new Parameter(p.Name, p.ParameterType.FullName, p.ParameterType));
                }
                info.memberTypes = MemberTypes.Constructor;
                info._constructorInfo = ms;
                M.Add(info);
            }
            return M;
        }

        /// <summary>
        /// 创建对象实例 ( typeName 类的完全限定名 , 构造存储器 ) )
        /// </summary>

        static public object CreateInstance(Constructor constructor)
        {
            return Invoke(constructor);
        }

        /// <summary>
        /// 创建对象实例 ( typeName 类的完全限定名 , 构造存储器 ) )
        /// </summary>

        static public object CreateInstance(string typeName, Constructor constructor)
        {
            return Invoke(constructor);
        }

        /// <summary>
        /// 调用唯一一个方法 , 使最优化，使尽可能有效 ( target 实体对象  , method 方法管理器与参数管理器 , varInfo[] 只有使用变量值时调用 )
        /// </summary>

        static public object Invoke(Constructor constructor)
        {
            if (constructor.constructorInfo == null)
            {
                return "null";
            }

            if (constructor.Parameters.Count == 0)
            {
                constructor.returnValue = constructor.constructorInfo.Invoke(null);
                return constructor.target = constructor.returnValue;
            }

            for (int i = 0; i < constructor.Parameters.Count; ++i)
            {
                constructor.parameterobjs[i] = constructor.Parameters[i].Value;
            }

            try
            {
                constructor.returnValue = constructor.constructorInfo.Invoke(constructor.parameterobjs);
                return constructor.target = constructor.returnValue;
            }
            catch { }

            return "null";
        }
    }

}