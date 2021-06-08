namespace GameDesigner
{
    using UnityEngine;

    /// <summary>
    /// 参数管理类
    /// </summary>
    [System.Serializable]
    public class Parameter
    {
        public string name;
        public int preID;
        public int ID;
        public string parameterTypeName
        {
            get
            {
                return value.ValueTypeName;
            }
            set
            {
                this.value.ValueTypeName = value;
            }
        }

        private System.Type _parameterType = null;
        public System.Type parameterType
        {
            get
            {
                if (_parameterType == null)
                {
                    _parameterType = SystemType.GetType(parameterTypeName);
                    if (_parameterType == null)
                        _parameterType = typeof(System.Type);
                }
                return _parameterType;
            }
        }

        public ValueType value = new ValueType();//基础参数值，比配一个值
        public object Value
        {
            get
            {
                if (setValue)
                {
                    if (parameterTypeName == "GameDesigner.BlueprintNode")
                    {
                        return setValue;
                    }
                    else if (setValue.method.returnTypeName == "System.Void")
                    {
                        Debug.Log("无返回值方法! 系统自动取消对象值... ");
                        setValue = null;
                    }
                    else
                    {
                        setValue.Invoke();
                        return value._object = setValue.method.returnValue;
                    }
                }
                return value.Value;
            }
            set
            {
                this.value.Value = value;
            }
        }

        public object EditorValue
        {
            get
            {
                if (setValue)
                {
                    if (parameterTypeName == "GameDesigner.BlueprintNode")
                    {
                        return setValue;
                    }
                    else if (setValue.method.returnTypeName == "System.Void")
                    {
                        Debug.Log("无返回值方法! 系统自动取消对象值... ");
                        setValue = null;
                    }
                    else
                    {
                        if (setValue.method.returnValue == null)
                        {
                            setValue.Invoke();
                        }
                        return value.Value = setValue.method.returnValue;
                    }
                }
                return value.Value;
            }
            set
            {
                this.value.Value = value;
            }
        }

        public object image = null;

        public Blueprint blueprint;

        public int setValueIndex = -1;
        public Node setValue 
        {
            get {
                if (setValueIndex == -1)
                    return null;
                return blueprint.nodes[setValueIndex];
            }
            set { setValueIndex = value.ID; }
        }
        [HideInInspector]
        public bool makeTransition = false;
        [HideInInspector]
        public int parameterID = 0;//参数ID,便于编辑器显示数组类型的方法中的第N个参数进行显示所用

        private Parameter()
        {

        }

        public Parameter(string Name, string ParameterTypeName, System.Type parameterType, int ParameterID = 0)
        {
            name = Name;
            parameterTypeName = ParameterTypeName;
            value.ValueTypeName = ParameterTypeName;
            _parameterType = parameterType;
            parameterID = ParameterID;
        }

        public Parameter(string Name, System.Type parameterType, int ParameterID = 0)
        {
            name = Name;
            parameterTypeName = parameterType.FullName;
            value.ValueTypeName = parameterType.FullName;
            _parameterType = parameterType;
            parameterID = ParameterID;
        }
    }
}