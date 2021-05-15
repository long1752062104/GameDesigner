using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameDesigner
{
    /// <summary>
    /// 类信息处理器
    /// </summary>
    [System.Serializable]
    public class TypeInfo
    {
        public static TypeInfo instance = new TypeInfo();
        public string nameSpace = "";
        public List<string> nameSpaces = new List<string>();
        public List<System.Type> types = new List<System.Type>();
        public string typeName = "UnityEngine.Input";
        public ObjectModel typeModel = ObjectModel.Object;
        public Object Target = new Object();
        public List<Constructor> Constructors = new List<Constructor>(1);
        public List<Field> Fields = new List<Field>(1);
        public List<Property> Propertys = new List<Property>(1);
        public List<Method> Methods = new List<Method>(1);

        [HideInInspector] public int typeNameIndex = 0;// 获取所有unity组件类名的唯一一个类型所有----PlayDesigner脚本的typeNames变量索要
        [HideInInspector] public string typeNameBool = "UnityEngine.Input";// 用来确认当是否更换类型
        [HideInInspector] public Object TargetBool = null;// 用来确认当是否更换物体类型

        public int invokeIndex = 0;

        //#if UNITY_EDITOR || DEBUG
        [HideInInspector]
        public bool findTools = false;
        public List<System.Type> findTypes = new List<System.Type>();
        public List<System.Type> findTypes1 = new List<System.Type>();
        [HideInInspector]
        public string findtype = "";
        [HideInInspector]
        public string findtypeBool = "";
        public int selectTypeIndex = 0;
        public bool constrsFolt = false;
        public bool fieldsFolt = false;
        public bool propesFolt = false;
        public bool methodsFolt = true;
        public List<Method> methods = new List<Method>();
        public List<Method> methods1 = new List<Method>();
        //#endif

        private System.Type _type = null;
        public System.Type type
        {
            get
            {
                if (_type == null)
                    _type = SystemType.GetType(typeName);
                else if (_type.FullName != typeName)
                    _type = SystemType.GetType(typeName);
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        public TypeInfo()
        {
            Constructors = new List<Constructor>();
            Fields = new List<Field>();
            Propertys = new List<Property>();
            Methods = new List<Method>();
            nameSpaces = new List<string>();
            findTypes = new List<System.Type>();
        }

        private object _target = null;
        public object target
        {
            get
            {
                if (_target == null)
                {
                    if (SystemType.IsSubclassOf(type, typeof(Object)))
                    {
                        typeModel = ObjectModel.Object;
                        return _target = Target;
                    }
                    else if (type.IsValueType)
                    {//值类默认为有一个构造器
                        typeModel = ObjectModel.ValueType;
                        return _target = CreateUtility.CreateInstance(type);
                    }
                    else if (type.IsAbstract)
                    {//抽象类没有构造函数
                        typeModel = ObjectModel.Class;
                    }
                    else
                    {
                        typeModel = ObjectModel.Class;
                        return _target = CreateUtility.CreateInstance(type);
                    }
                }
                return _target;
            }
            set { _target = value; }
        }

        /// <summary>
        /// 检查编辑器异动后进行更新类信息
        /// </summary>

        static public void UpdateCheckTypeInfo(TypeInfo type)
        {
            InitTypeData(type);
        }

        /// <summary>
        /// 初始化类的信息,此静态方法可以在Update运行
        /// </summary>

        static public TypeInfo InitTypeData(TypeInfo type)
        {
            if (type == null)
            {
                return null;
            }

            if (type.typeName != type.typeNameBool)
            {
                type.typeNameBool = type.typeName;
                type = InitTypeInfo(type);
            }

            if (type.typeModel == ObjectModel.Object)
            {
                type.target = type.Target;//当编写代码重新编译后丢失target对象为空
                if (type.Target != type.TargetBool)
                {
                    type = InitTypeInfo(type);
                    type.TargetBool = type.Target;
                }
            }
            return type;
        }

        /// <summary>
        /// 初始化类的信息
        /// </summary>

        static public TypeInfo InitTypeInfo(TypeInfo type)
        {
            try
            {
                System.Type t = SystemType.GetType(type.typeName);
                type.Constructors = Constructor.GetConstructors(t);
                type.Fields = Field.GetFields(t);
                type.Propertys = Property.GetPropertys(t);
                type.Methods = Method.GetMethods(t);
                if (SystemType.IsSubclassOf(t, typeof(Object)))
                {
                    type.target = type.Target;
                    type.typeModel = ObjectModel.Object;
                }
                else if (t.IsSubclassOf(typeof(System.ValueType))) //值类默认为有一个构造器
                {
                    type.target = CreateUtility.CreateInstance(t);
                    type.typeModel = ObjectModel.ValueType;
                }
                else if (t == typeof(string))
                {
                    type.target = "";
                    type.typeModel = ObjectModel.Class;
                }
                else if (t.IsAbstract)
                {
                    type.typeModel = ObjectModel.Class;
                }
                else
                {
                    type.typeModel = ObjectModel.Class;
                    type.target = CreateUtility.CreateInstance(t);
                }
            }
            catch
            {
                try { type.target = SystemType.GetType(type.typeName); } catch { }
            }
            return type;
        }


        /// <summary>
        /// 当对象为空时,创建new的对象
        /// </summary>

        static public object CreateInstance(TypeInfo type)
        {
            System.Type t = SystemType.GetType(type.typeName);
            if (SystemType.IsSubclassOf(t, typeof(Object)))
            {
                type.typeModel = ObjectModel.Object;
                return type.target = type.Target;
            }
            else if (SystemType.IsSubclassOf(t, typeof(System.ValueType))) //值类默认为有一个构造器
            {
                type.typeModel = ObjectModel.ValueType;
                return type.target = CreateUtility.CreateInstance(t);
            }
            else
            {
                type.typeModel = ObjectModel.Class;
                return type.target = CreateUtility.CreateInstance(t);
            }
        }
    }
}