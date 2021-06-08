using System.Reflection;
using UnityEngine;

namespace GameDesigner
{
    /// <summary>
    /// 蓝图节点组件
    /// </summary>
    [System.Serializable]
    public class Node
    {
        public string name;
        public int preID;
        public int ID;
        [HideInInspector]
        public bool makeTransition = false;
        [HideInInspector]
        public bool makeGetValueTransition = false;
        [HideInInspector]
        public bool makeRuntimeTransition = false;
        [HideInInspector]
        public bool makeOutRuntimeTransition = false;
        public Rect rect = new Rect(10, 10, 150, 30);

        public Property method = new Property();

        public int setValueIndex = -1;
        public int runtimeIndex = -1;

        public Node setValue {
            get {
                if (setValueIndex == -1)
                    return null;
                return blueprint.nodes[setValueIndex];
            }
            set { setValueIndex = value.ID; }
        }// 入口方法,从这个方法类进入后自动遍历所有血脉

		public Node runtime {
            get {
                if (runtimeIndex == -1)
                    return null;
                return blueprint.nodes[runtimeIndex];
            }
            set { runtimeIndex = value.ID; }
        }

        public Blueprint blueprint;

        public System.Type returnType
        {
            get
            {
                return method.returnType;
            }
        }

        public object returnValue
        {
            get
            {
                return method.returnValue;
            }
            set { method.returnValue = value; }
        }

        public Node()
        {
        }

        static public Node CreateBlueprintNodeInstance(Blueprint designer, string nodeName, Vector2 position)
        {
            Node state = new Node();
            state.name = nodeName;
            state.rect.position = position;
            state.ID = designer.nodes.Count;
            state.blueprint = designer;
            state.rect.size = new Vector2(180, 100);
            designer.nodes.Add(state);
            return state;
        }

        /// <summary>
        /// 创建蓝图方法入口节点(简称节点入口)
        /// Designer 蓝图管理器,用来管理节点之间的连接
        /// Type 要获取的类型
        /// Fun name "获得类的方法名"，必须声明函数为公有才能获取成功
        /// Inherited name "继承类型名" ， 可以忽略，代码生成所用
        /// Node name 自定义节点介绍名称
        /// </summary>
        static public Node CreateFunctionBody(Blueprint designer, System.Type type, string funName, string inheritedName = "MonoBehaviour", string nodeName = "函数入口")
        {
            Node body = CreateBlueprintNodeInstance(designer, funName, new Vector2(5150, 5150));
            MethodInfo method = type.GetMethod(funName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (method == null)
            {
                Debug.Log("此类 " + type.FullName + "没有" + funName + "方法！请编写" + funName + "方法后在创建蓝图入口节点！");
                return null;
            }
            Method method1 = new Method(method.Name, method.ReturnType.FullName, type.FullName);
            int paramsindex = 0;
            foreach (ParameterInfo p in method.GetParameters())
            {
                method1.Parameters.Add(new Parameter(p.Name, p.ParameterType.FullName, p.ParameterType, paramsindex));
                paramsindex++;
            }
            method1.targetType = type;
            SystemType.SetFieldValue(body.method, method1);
            body.method.Parameters = method1.Parameters;
            body.method.typeModel = ObjectModel.Class;
            designer.InheritedClassName = inheritedName;
            body.method.name = funName;
            body.method.nodeName = nodeName;
            body.method.targetTypeName = type.FullName;
            body.method.memberTypes = MemberTypes.All;
            return body;
        }

        public bool IsStatic
        {
            get
            {
                switch (method.memberTypes)
                {
                    case MemberTypes.Method:
                        return method.methodInfo.IsStatic;
                    case MemberTypes.Property:
                        if (method.propertyInfo.CanRead)
                            return method.propertyInfo.GetGetMethod().IsStatic;
                        if (method.propertyInfo.CanWrite)
                            return method.propertyInfo.GetSetMethod().IsStatic;
                        break;
                    case MemberTypes.Field:
                        return method.fieldInfo.IsStatic;
                }
                return false;
            }
        }

        public bool IsFunction
        {
            get
            {
                return method.memberTypes == MemberTypes.All;
            }
        }

        /// <summary>
        /// 调用入口
        /// </summary>
        public void Invoke()
        {
            if (setValue)
            {
                setValue.Invoke(); //没有参数的对象,所有需要进入执行下一个方法或属性--遍历
                switch (setValue.method.memberTypes)
                {
                    case MemberTypes.Method:
                        method.target = Method.Invoke(setValue.method.target, setValue.method);
                        break;
                    case MemberTypes.Property:
                        method.target = Property.Invoke(setValue.method.target, setValue.method);
                        break;
                    case MemberTypes.Field:
                        method.target = Field.Invoke(setValue.method.target, setValue.method);
                        break;
                    case MemberTypes.Constructor:
                        method.target = Constructor.Invoke(setValue.method);
                        break;
                    case MemberTypes.Custom:
                        method.target = setValue.method.target;
                        break;
                }
            }

            Runtime();

            if (runtime != null)
            { //接入节点
                runtime.Invoke();
            }
        }

        void Runtime()
        {
            switch (method.memberTypes)
            {
                case MemberTypes.Method:
                    Method.Invoke(method.target, method, method.Parameters);
                    break;
                case MemberTypes.Property:
                    Property.Invoke(method.target, method);
                    break;
                case MemberTypes.Field:
                    Field.Invoke(method.target, method);
                    break;
                case MemberTypes.Constructor:
                    Constructor.Invoke(method);
                    break;
            }
        }

        public static implicit operator bool(Node exists)
        {
            return exists != null;
        }
    }
}