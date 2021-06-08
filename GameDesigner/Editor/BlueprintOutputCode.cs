#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameDesigner
{
    public class BlueprintOutputCode : EditorWindow
    {
        Vector2 scrollPosition;
        static public Blueprint designer = null;

        static BlueprintOutputCode window;
        static public BlueprintOutputCode instance
        {
            get
            {
                if (window == null)
                    window = GetWindow<BlueprintOutputCode>("蓝图代码生成编辑器", true);
                return window;
            }
            set { window = value; }
        }

        [MenuItem("GameDesigner/Blueprint/BlueprintCodeGenerationEditor")]
        static void init()
        {
            window = instance;
        }

        void OnGUI()
        {
            if (designer == null) return;
            string text = "";
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            text = "";
            foreach (var s in namespaces)
            {
                text += s + "\n";
            }
            text += "\npublic class " + CreateScriptName + " : " + designer.InheritedClassName + "\n{\n";

            foreach (var m in designer.nodes)
            {
                if (m.method.memberTypes == MemberTypes.All)//方法入口(函数名)枚举
                    continue;
                switch (m.method.memberTypes)
                {
                    case MemberTypes.Field:
                        if (m.method.fieldInfo.IsStatic)//静态方法不需要被声明
                            continue;
                        break;
                    case MemberTypes.Property:
                        if (m.method.propertyInfo.CanRead)//如果属性可读
                            if (m.method.propertyInfo.GetGetMethod().IsStatic)//静态方法不需要被声明
                                continue;
                            else if (m.method.propertyInfo.CanWrite)//如果属性可写
                                if (m.method.propertyInfo.GetSetMethod().IsStatic)//静态方法不需要被声明
                                    continue;
                        break;
                    case MemberTypes.Method:
                        if (m.method.methodInfo.IsStatic)//静态方法不需要被声明
                            continue;
                        break;
                    case MemberTypes.Constructor://构造函数不需要声明
                        continue;
                }
                if (m.method.memberTypes != MemberTypes.Custom & m.setValue)
                    continue;
                if (!namespaces.Contains("using " + m.method.targetType.Namespace + ";") & m.method.targetType.Namespace != null & m.method.targetType.Namespace != "System")
                {
                    namespaces.Add("using " + m.method.targetType.Namespace + ";");
                }
                if (m.method.typeModel == ObjectModel.Object)
                {
                    text += "\tpublic " + m.method.targetType.Name + " " + m.method.nodeName + " = null;\n";
                }
                else
                {
                    var name = m.method.targetTypeName;
                    switch (name)
                    {
                        case "System.Single":
                            text += "\tpublic " + m.method.targetTypeName + " " + m.method.nodeName + " = " + m.method.targetValue.Value + "F;\n";
                            break;
                        case "System.Int32":
                            text += "\tpublic " + m.method.targetTypeName + " " + m.method.nodeName + " = " + m.method.targetValue.Value + ";\n";
                            break;
                        case "System.Boolean":
                            text += "\tpublic " + m.method.targetTypeName + " " + m.method.nodeName + " = " + ConvertUtility.BooleanToString(m.method.targetValue.Value) + ";\n";
                            break;
                        case "System.String":
                            text += "\tpublic " + m.method.targetTypeName + " " + m.method.nodeName + " = " + '"' + m.method.targetValue.Value + '"' + ";\n";
                            break;
                        case "UnityEngine.Vector2":
                            text += "\tpublic " + m.method.targetType.Name + " " + m.method.nodeName + " = new UnityEngine.Vector2" + ConvertUtility.Vector2_3_4ToString(m.method.targetValue.Value) + ";\n";
                            break;
                        case "UnityEngine.Vector3":
                            text += "\tpublic " + m.method.targetType.Name + " " + m.method.nodeName + " = new UnityEngine.Vector3" + ConvertUtility.Vector2_3_4ToString(m.method.targetValue.Value) + ";\n";
                            break;
                        case "UnityEngine.Vector4":
                            text += "\tpublic " + m.method.targetType.Name + " " + m.method.nodeName + " = new UnityEngine.Vector4" + ConvertUtility.Vector2_3_4ToString(m.method.targetValue.Value) + ";\n";
                            break;
                        case "UnityEngine.Rect":
                            text += "\tpublic " + m.method.targetType.Name + " " + m.method.nodeName + " = new UnityEngine.Rect" + ConvertUtility.RectToString(m.method.targetValue.Value) + ";\n";
                            break;
                        case "UnityEngine.Color":
                            text += "\tpublic " + m.method.targetType.Name + " " + m.method.nodeName + " = new UnityEngine.Color" + ConvertUtility.ColorToString(m.method.targetValue.Value) + ";\n";
                            break;
                        case "UnityEngine.Quaternion":
                            text += "\tpublic " + m.method.targetType.Name + " " + m.method.nodeName + " = new UnityEngine.Quaternion" + ConvertUtility.QuaternionToString(m.method.targetValue.Value) + ";\n";
                            break;
                        default:
                            if (m.method.targetType.IsEnum)
                            {
                                text += "\tpublic " + m.method.targetType.Name + " " + m.method.nodeName + " = " + m.method.targetType.Name + "." + m.method.targetValue.Value + ";\n";
                            }
                            else if (m.method.targetType.GetConstructors().Length == 0)
                            {//没有构造函数,声明的变量不能赋值
                                text += "\tpublic " + m.method.targetType.Name + " " + m.method.nodeName + " = null;\n";
                            }
                            else if (m.method.targetType.Namespace == "System")
                                text += "\tpublic " + m.method.targetTypeName + " " + m.method.nodeName + " = new " + m.method.targetTypeName + "();\n";
                            else
                                text += "\tpublic " + m.method.targetType.Name + " " + m.method.nodeName + " = new " + m.method.targetType.Name + "();\n";
                            break;
                    }
                }
            }
            foreach (var m in designer.nodes)
            {
                if (m.method.memberTypes == MemberTypes.All)
                {
                    try
                    {
                        text += "\tpublic " + (m.method.methodInfo.IsVirtual ? "override " : "") + (m.method.methodInfo.ReturnType.FullName == "System.Void" ? "void" : m.method.methodInfo.ReturnType.FullName) + " " + m.method.methodInfo.Name + " (";
                    }
                    catch
                    {
                        text += "\tpublic void " + m.method.name + " (";
                    }
                    int i = 0;
                    foreach (var par in m.method.methodInfo.GetParameters())
                    {
                        text += par.ToString() + (i != m.method.methodInfo.GetParameters().Length - 1 ? " , " : " ");
                        i++;
                    }
                    text += ") // " + m.method.nodeName + "\n\t{\n";
                    if (m.runtime)//这里表示的是m是函数开头名称,没有起什么作用
                        text += Invoke(m.runtime);
                    text += "\t}\n";
                }
            }
            text += "}\n";
            EditorGUILayout.TextArea(text);
            EditorGUILayout.EndScrollView();

            scriptPath = EditorGUILayout.TextField("脚本路径", scriptPath);
            CreateScriptName = EditorGUILayout.TextField("脚本名称", CreateScriptName);
            designer.InheritedClassName = EditorGUILayout.TextField("继承类型", designer.InheritedClassName);
            if (GUILayout.Button(new GUIContent("BuildScript")))
            {
                foreach (var body in designer.nodes)
                {
                    foreach (var b in designer.nodes)
                    {
                        if (body.runtime == null & !body.IsFunction & !body.IsStatic & body != b & body.method.nodeName == b.method.nodeName)
                        {
                            body.method.nodeName = body.method.nodeName + body.ID;
                            Debug.Log(body.method.nodeName + "节点重名,自动更改节点名称加上索引号...");
                            return;
                        }
                        if (body.method.nodeName == "")
                        {
                            Debug.Log(body.method.targetTypeName + "." + body.method.name + "节点名为空,变量中不允许一个变量的名为: public GameObject  = null; , 无法生成脚本!请先设置节点名(变量名)在试...");
                            return;
                        }
                    }
                }
                if (System.IO.File.Exists(Application.dataPath + scriptPath + CreateScriptName + ".cs"))
                {
                    if (EditorUtility.DisplayDialog("提示!", "文件已经存在!是否覆盖?", "是", "否"))
                    {
                        System.IO.File.WriteAllText(Application.dataPath + scriptPath + CreateScriptName + ".cs", "");
                    }
                    else
                    {
                        return;
                    }
                }
                ScriptTools.CreateScript(Application.dataPath + scriptPath, CreateScriptName, text);
                Debug.Log("蓝图代码生成完毕!");
            }
            Repaint();
        }

        string scriptPath = "/";
        string CreateScriptName = "newStateBehaviour";
        List<string> namespaces = new List<string>() { "using UnityEngine;", "using System.Collections.Generic;" };

        private string IFPars(Parameter par)
        {
            string str = "";
            if (par.setValue)
            {//如果给参数赋值
                MemberTypes metype = par.setValue.method.memberTypes;//方法和属性,字段,构造都是由返回值返回的类
                if (metype == MemberTypes.Method | metype == MemberTypes.Property)
                {
                    if (par.parameterTypeName != "System.Object" & par.parameterTypeName != par.setValue.method.returnTypeName)
                    {//如果类型不同,就要强制转换类型才能编译通过
                        str += "(" + par.parameterType.Name + ")" + NInvoke(par.setValue); // 设置参数值
                        if (!namespaces.Contains("using " + par.parameterType.Namespace + ";") & par.parameterType.Namespace != null & par.parameterType.Namespace != "System")
                        {
                            namespaces.Add("using " + par.parameterType.Namespace + ";");
                        }
                        return str;//如果给参数赋值,下面自带值就不使用
                    }
                    else
                    {
                        str += NInvoke(par.setValue); // 设置参数值
                        return str;//如果给参数赋值,下面自带值就不使用
                    }
                }
                else if (metype == MemberTypes.Field)
                {
                    if (par.parameterTypeName != "System.Object" & par.parameterTypeName != par.setValue.method.returnTypeName)
                    {//如果变量类型与节点返回类型不同,就要强制转换类型才能编译通过
                        str += "(" + par.parameterType.Name + ")" + NInvoke(par.setValue, MemberTypes.Field); // 设置参数值
                        if (!namespaces.Contains("using " + par.parameterType.Namespace + ";") & par.parameterType.Namespace != null & par.parameterType.Namespace != "System")
                        {
                            namespaces.Add("using " + par.parameterType.Namespace + ";");
                        }
                        return str;//如果给参数赋值,下面自带值就不使用
                    }
                    else
                    {
                        str += NInvoke(par.setValue, MemberTypes.Field); // 设置参数值
                        return str;//如果给参数赋值,下面自带值就不使用
                    }
                }
                else if (metype == MemberTypes.Custom | metype == MemberTypes.Constructor)
                {//如果节点是对象,比较对象类型
                    if (par.parameterTypeName != "System.Object" & par.parameterTypeName != par.setValue.method.targetTypeName)
                    {// (这里使用的对象类型) 如果类型不同,就要强制转换类型才能编译通过
                        str += "(" + par.parameterType.Name + ")" + NInvoke(par.setValue); // 设置参数值
                        if (!namespaces.Contains("using " + par.parameterType.Namespace + ";") & par.parameterType.Namespace != null & par.parameterType.Namespace != "System")
                        {
                            namespaces.Add("using " + par.parameterType.Namespace + ";");
                        }
                        return str;//如果给参数赋值,下面自带值就不使用
                    }
                    else
                    {
                        str += NInvoke(par.setValue); // 设置参数值
                        return str;//如果给参数赋值,下面自带值就不使用
                    }
                }
            }
            //当使用自带参数值,这也是最后一步的赋值给方法的参数值(简称结尾参数值)
            SetLoaclPar(par);
            return str;
        }

        private string ForPars(Parameter par)
        {
            string str = "";
            if (par.setValue)
            {//如果给参数赋值
                MemberTypes metype = par.setValue.method.memberTypes;//方法和属性,字段,构造都是由返回值返回的类
                if (metype == MemberTypes.Method | metype == MemberTypes.Property)
                {
                    str += NInvoke(par.setValue); // 设置参数值
                    return str;//如果给参数赋值,下面自带值就不使用
                }
                else if (metype == MemberTypes.Field)
                {
                    str += NInvoke(par.setValue, MemberTypes.Field); // 设置参数值
                    return str;//如果给参数赋值,下面自带值就不使用
                }
                else if (metype == MemberTypes.Custom | metype == MemberTypes.Constructor)
                {//如果节点是对象,比较对象类型
                    str += NInvoke(par.setValue); // 设置参数值
                    return str;//如果给参数赋值,下面自带值就不使用
                }
            }
            //当使用自带参数值,这也是最后一步的赋值给方法的参数值(简称结尾参数值)
            SetLoaclPar(par);
            return str;
        }

        private string SetLoaclPar(Parameter par)
        {
            string str = "";
            //当使用自带参数值,这也是最后一步的赋值给方法的参数值(简称结尾参数值)
            if (SystemType.IsSubclassOf(par.parameterType, typeof(UnityEngine.Object)))
            {
                str += "null";
            }
            else
            {
                switch (par.parameterTypeName)
                {
                    case "System.Single":
                        str += par.Value + "F";
                        break;
                    case "System.Int32":
                        str += par.Value;
                        break;
                    case "System.Boolean":
                        str += ConvertUtility.BooleanToString(par.Value);
                        break;
                    case "System.String":
                        str += "" + '"' + par.Value + '"';
                        break;
                    case "UnityEngine.Vector2":
                        str += "new UnityEngine.Vector2" + ConvertUtility.Vector2_3_4ToString(par.Value);
                        break;
                    case "UnityEngine.Vector3":
                        str += "new UnityEngine.Vector3" + ConvertUtility.Vector2_3_4ToString(par.Value);
                        break;
                    case "UnityEngine.Vector4":
                        str += "new UnityEngine.Vector4" + ConvertUtility.Vector2_3_4ToString(par.Value);
                        break;
                    case "UnityEngine.Rect":
                        str += "new UnityEngine.Rect" + ConvertUtility.Vector234_Rect_Quaternion_ColorToString(par.Value);
                        break;
                    case "UnityEngine.Color":
                        str += "new UnityEngine.Color" + ConvertUtility.Vector234_Rect_Quaternion_ColorToString(par.Value);
                        break;
                    case "UnityEngine.Quaternion":
                        str += "new UnityEngine.Quaternion" + ConvertUtility.Vector234_Rect_Quaternion_ColorToString(par.Value);
                        break;
                    case "System.Object":
                        str += "new System.Object()";
                        break;
                    default:
                        if (par.parameterType.IsEnum)
                        {
                            str += par.parameterTypeName + "." + par.Value;
                        }
                        else if (par.parameterType.GetConstructors().Length == 0)
                        {//如果参数类型没有构造函数,赋值一个空的值,不能创建对象
                            str += "null";
                        }
                        else
                        {
                            if (par.parameterType.Namespace == "System")
                                str += "new " + par.parameterTypeName + "()";
                            else
                                str += "new " + par.parameterType.Name + "()";
                            if (!namespaces.Contains("using " + par.parameterType.Namespace + ";") & par.parameterType.Namespace != null & par.parameterType.Namespace != "System")
                            {
                                namespaces.Add("using " + par.parameterType.Namespace + ";");
                            }
                        }
                        break;
                }
            }
            return str;
        }

        private bool Contains(Node body)
        {
            foreach (var p in body.method.Parameters)
            {
                if (p.parameterTypeName == "GameDesigner.BlueprintNode")
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 主方法
        /// </summary>
        public string Invoke(Node body, string tab = "\t\t")
        {
            string text = "";
            switch (body.method.memberTypes)
            {//第一个被赋值的都是主对象 target = gameobject.transform = other.transform;
                case MemberTypes.Method://如果第一个节点属于方法
                    if (body.method.name == "IF" & Contains(body))
                    {
                        text += tab + "if(";
                        for (int i = 0; i < body.method.Parameters.Count; ++i)
                        {
                            if (body.method.Parameters[i].parameterTypeName == "GameDesigner.BlueprintNode")
                            {
                                if (body.method.Parameters[i].name == "True")
                                {
                                    text += "){\n";
                                    if (body.method.Parameters[i].setValue)
                                    {
                                        text += Invoke(body.method.Parameters[i].setValue, "\t\t\t");
                                    }
                                }
                                else if (body.method.Parameters[i].name == "False")
                                {
                                    text += "\n\t\t}else{\n";
                                    if (body.method.Parameters[i].setValue)
                                    {
                                        text += Invoke(body.method.Parameters[i].setValue, "\t\t\t");
                                    }
                                }
                            }
                            else if (body.method.Parameters[i].parameterTypeName == "Contition")
                            {
                                if (body.method.Parameters[i].value.Value.ToString() == "Equals")
                                {
                                    text += " == ";
                                }
                                else if (body.method.Parameters[i].value.Value.ToString() == "NotEquals")
                                {
                                    text += " != ";
                                }
                                else if (body.method.Parameters[i].value.Value.ToString() == "MaxEquals")
                                {
                                    text += " >= ";
                                }
                                else if (body.method.Parameters[i].value.Value.ToString() == "MinEquals")
                                {
                                    text += " <= ";
                                }
                                else if (body.method.Parameters[i].value.Value.ToString() == "Max")
                                {
                                    text += " > ";
                                }
                                else if (body.method.Parameters[i].value.Value.ToString() == "Min")
                                {
                                    text += " < ";
                                }
                            }
                            else
                            {
                                text += IFPars(body.method.Parameters[i]);
                                if (body.method.Parameters.Count - 1 == i)
                                {
                                    text += "){";
                                }
                            }
                        }
                        text += "\n\t\t}\n";
                    }
                    else if (body.method.name == "For")
                    {
                        text += tab + "foreach( var target in ";
                        for (int i = 0; i < body.method.Parameters.Count; ++i)
                        {
                            if (body.method.Parameters[i].name == "runtime")
                            {
                                text += " ){\n";
                                if (body.method.Parameters[i].setValue)
                                {
                                    text += Invoke(body.method.Parameters[i].setValue, "\t\t\t");
                                }
                            }
                            else if (body.method.Parameters[i].name == "arrays")
                            {
                                if (body.method.Parameters[i].setValue)
                                {
                                    if (body.method.Parameters[i].setValue.method.returnTypeName.Contains("[]") | body.method.Parameters[i].setValue.method.returnTypeName == "System.Array" | body.method.Parameters[i].setValue.method.returnTypeName.Contains("List`1"))
                                    {
                                        text += ForPars(body.method.Parameters[i]);
                                    }
                                    else
                                    {
                                        Debug.Log("arrays参数错误 => 你指定的类型不是数组类型或集合类型！");
                                    }
                                }
                            }
                        }
                        text += "\n\t\t}\n";
                    }
                    else
                    {
                        if (body.setValue)
                        {//如果给对象赋值
                            if (body.method.methodInfo.IsStatic)//如果方法是静态方法使用类名点方法名调用,不需要声明对象
                                Debug.Log("静态方法不能设置对象值,直接使用类名点方法调用!");
                            else
                            {//当给对象赋值,为了考虑赋值的长度问题,用一个()框起来,对象赋值完成后再点方法名
                                if (body.method.name.Contains("set_"))
                                {//如果方法名包含set_,说明这是属性被方法获得到的,当编译后无法使用,所以不能加()
                                    if (body.setValue.setValue)
                                        text += tab + "(" + NInvoke(body.setValue) + ")." + body.method.name.Replace("set_", "");//编译不通过,所以得修改这个set_,只要后面字段
                                    else
                                        text += tab + NInvoke(body.setValue) + "." + body.method.name.Replace("set_", "");//编译不通过,所以得修改这个set_,只要后面字段
                                    text += " = " + SetPars(body);
                                }
                                else
                                {
                                    if (body.setValue.setValue)
                                        text += tab + "(" + NInvoke(body.setValue) + ")." + body.method.name + "(";//设置参数值
                                    else
                                        text += tab + NInvoke(body.setValue) + "." + body.method.name + "(";//设置参数值
                                    text += SetPars(body);
                                    text += ")";
                                }
                            }
                        }
                        else
                        {//当没有给对象赋值,使用自动对象调用方法
                            if (body.method.methodInfo.IsStatic)
                            {//如果方法是静态方法使用类名点方法名调用,不需要声明对象
                                if (body.method.name.Contains("get_"))
                                {//如果方法名包含get_,说明这是属性被方法获得到的,当编译后无法使用,所以不能加()
                                    text += tab + body.method.targetType.Name + "." + body.method.name.Replace("get_", "");//编译不通过,所以得修改这个get_,只要后面字段
                                    if (!namespaces.Contains("using " + body.method.targetType.Namespace + ";") & body.method.targetType.Namespace != null & body.method.targetType.Namespace != "System")
                                    {
                                        namespaces.Add("using " + body.method.targetType.Namespace + ";");
                                    }
                                }
                                else
                                {
                                    text += tab + body.method.targetType.Name + "." + body.method.name + "(";//设置参数值
                                    if (!namespaces.Contains("using " + body.method.targetType.Namespace + ";") & body.method.targetType.Namespace != null & body.method.targetType.Namespace != "System")
                                    {
                                        namespaces.Add("using " + body.method.targetType.Namespace + ";");
                                    }
                                    text += SetPars(body);
                                    text += ")";
                                }
                            }
                            else
                            {
                                if (body.method.name.Contains("get_"))
                                {//如果方法名包含get_,说明这是属性被方法获得到的,当编译后无法使用,所以不能加()
                                    text += tab + body.method.nodeName + "." + body.method.name.Replace("get_", "");//编译不通过,所以得修改这个get_,只要后面字段
                                }
                                else
                                {
                                    text += tab + body.method.nodeName + "." + body.method.name + "(";//设置参数值
                                    text += SetPars(body);
                                    text += ")";
                                }
                            }
                        }
                        text += ";\n";
                    }
                    break;
                case MemberTypes.Property:
                    if (body.setValue)
                    {//如果给对象赋值
                        if (body.method.propertyInfo.CanWrite)
                        {//如果属性可写才能赋值
                            if (body.method.propertyInfo.GetSetMethod().IsStatic)//如果变量是静态不能给对象赋值
                                Debug.Log("静态属性不能设置对象值,直接使用类名点属性名调用!");
                            else
                            {//当给对象赋值,为了考虑赋值的长度问题,用一个()框起来,对象赋值完成后再点方法名
                                if (body.setValue.setValue)
                                    text += tab + "(" + NInvoke(body.setValue) + ")." + body.method.name + " = ";//设置参数值
                                else
                                    text += tab + NInvoke(body.setValue) + "." + body.method.name + " = ";//设置参数值
                            }
                        }
                        else
                            Debug.Log("只写属性,不能获得属性值!");
                    }
                    else
                    {//当没有给对象赋值,使用自动对象调用方法
                        if (body.method.propertyInfo.CanWrite)
                        {//如果属性可写才能设置属性值
                            if (body.method.propertyInfo.GetSetMethod().IsStatic)
                            {//如果方法是静态方法使用类名点方法名调用,不需要声明对象
                                text += tab + body.method.targetTypeName + "." + body.method.name + " = ";//设置参数值
                            }
                            else
                            {
                                text += tab + body.method.nodeName + "." + body.method.name + " = ";//设置参数值
                            }
                        }
                        else
                            Debug.Log("只读属性,不能设置属性值!");
                    }
                    text += SetPars(body);
                    text += ";\n";
                    break;
                case MemberTypes.Field:
                    if (body.setValue)
                    {//如果给对象赋值
                        if (body.method.fieldInfo.IsStatic)//如果变量是静态不能给对象赋值
                            Debug.Log("静态变量不能设置对象值,直接使用类名点方法调用!");
                        else
                        {//当给对象赋值,为了考虑赋值的长度问题,用一个()框起来,对象赋值完成后再点方法名
                            if (body.setValue.setValue)
                                text += tab + "(" + NInvoke(body.setValue) + ")." + body.method.name + " = ";//设置参数值
                            else
                                text += tab + NInvoke(body.setValue) + "." + body.method.name + " = ";//设置参数值
                        }
                    }
                    else
                    {//当没有给对象赋值,使用自动对象调用方法
                        if (body.method.fieldInfo.IsStatic)
                        {//如果方法是静态方法使用类名点方法名调用,不需要声明对象
                            text += tab + body.method.targetType.Name + "." + body.method.name + " = ";//设置参数值
                            if (!namespaces.Contains("using " + body.method.targetType.Namespace + ";") & body.method.targetType.Namespace != null & body.method.targetType.Namespace != "System")
                            {
                                namespaces.Add("using " + body.method.targetType.Namespace + ";");
                            }
                        }
                        else
                        {
                            text += tab + body.method.nodeName + "." + body.method.name + " = ";//设置参数值
                        }
                    }
                    text += SetPars(body);
                    text += ";\n";
                    break;
                case MemberTypes.Constructor:
                    text += tab + "new " + body.method.targetType.Name + "(";
                    text += SetPars(body);
                    text += ");\n";
                    Debug.Log("构造函数不允许在第一个节点调用,没有用处!");
                    break;
                case MemberTypes.Custom:
                    if (body.setValue)
                    {//如果给对象赋值
                        text += tab + body.method.nodeName + " = " + NInvoke(body.setValue, MemberTypes.Custom);//从对象出发
                    }
                    else
                    {
                        Debug.Log("对象没有赋值！");
                    }
                    text += ";\n";
                    break;
            }

            if (body.runtime != null)
            { //接入节点
                text += Invoke(body.runtime, tab);
            }
            return text;
        }

        /// <summary>
        /// 次方法
        /// onEnter 当第一次连接进入是从什么方法或字段或属性开始呢
        /// nEnter 如果从n多次进入
        /// </summary>
        public string NInvoke(Node body, MemberTypes onEnter = MemberTypes.Event)
        {
            string str = "";
            switch (body.method.memberTypes)
            {
                case MemberTypes.Method://如果被获得的节点是方法
                    if (body.setValue)
                    {//如果给对象赋值
                        if (body.method.methodInfo.IsStatic)//如果方法是静态方法使用类名点方法名调用,不需要声明对象
                            Debug.Log("静态方法不能设置对象值,直接使用类名点方法调用!");
                        else
                        {//当给对象赋值,为了考虑赋值的长度问题,用一个()框起来,对象赋值完成后再点方法名
                            if (body.method.name.Contains("set_"))
                            {//如果方法名包含set_,说明这是属性被方法获得到的,当编译后无法使用,所以不能加()
                                str += "(" + NInvoke(body.setValue) + ")." + body.method.name.Replace("set_", "");//编译不通过,所以得修改这个set_,只要后面字段
                                str += " = " + SetPars(body);
                                return str;
                            }
                            str += "(" + NInvoke(body.setValue) + ")." + body.method.name + "(";//设置参数值
                        }
                    }
                    else
                    {//当没有给对象赋值,使用自动对象调用方法
                        if (body.method.methodInfo.IsStatic)
                        {//如果方法是静态方法使用类名点方法名调用,不需要声明对象
                            if (body.method.name.Contains("get_"))
                            {//如果方法名包含get_,说明这是属性被方法获得到的,当编译后无法使用,所以不能加()
                                str += body.method.targetType.Name + "." + body.method.name.Replace("get_", "");//编译不通过,所以得修改这个get_,只要后面字段
                                if (!namespaces.Contains("using " + body.method.targetType.Namespace + ";") & body.method.targetType.Namespace != null & body.method.targetType.Namespace != "System")
                                {
                                    namespaces.Add("using " + body.method.targetType.Namespace + ";");
                                }
                                return str;
                            }
                            str += body.method.targetType.Name + "." + body.method.name + "(";//设置参数值
                            if (!namespaces.Contains("using " + body.method.targetType.Namespace + ";") & body.method.targetType.Namespace != null & body.method.targetType.Namespace != "System")
                            {
                                namespaces.Add("using " + body.method.targetType.Namespace + ";");
                            }
                        }
                        else
                        {
                            if (body.method.name.Contains("get_"))
                            {//如果方法名包含get_,说明这是属性被方法获得到的,当编译后无法使用,所以不能加()
                                str += body.method.nodeName + "." + body.method.name.Replace("get_", "");//编译不通过,所以得修改这个get_,只要后面字段
                                return str;
                            }
                            str += body.method.nodeName + "." + body.method.name + "(";//设置参数值
                        }
                    }
                    str += SetPars(body);
                    str += ")";
                    break;
                case MemberTypes.Property://当属性在次方法时都属于获得值(获得属性值) , 所以不允许在获得属性值的同时设置属性值 , 要想设置属性值请在第一个节点(主方法)才能设置
                    if (body.setValue)
                    {//如果给对象赋值
                        if (body.method.propertyInfo.CanRead)
                        {//如果属性可读才能获得属性值
                            if (body.method.propertyInfo.GetGetMethod().IsStatic)//如果变量是静态不能给对象赋值
                                Debug.Log("静态属性不能设置对象值,直接使用类名点属性名调用!");
                            else
                            {//当给对象赋值,为了考虑赋值的长度问题,用一个()框起来,对象赋值完成后再点方法名
                                if (body.setValue.setValue)
                                    str += "(" + body.method.nodeName + " = " + NInvoke(body.setValue) + ")." + body.method.name;//设置参数值
                                else
                                    str += body.method.nodeName + " = " + NInvoke(body.setValue) + "." + body.method.name;//设置参数值
                            }
                        }
                        else
                            Debug.Log("只写属性,不能获得属性值!");
                    }
                    else
                    {//当没有给对象赋值,使用自动对象调用方法
                        if (body.method.propertyInfo.CanRead)
                        {//如果属性可读才能获得属性值
                            if (body.method.propertyInfo.GetGetMethod().IsStatic)
                            {//如果方法是静态方法使用类名点方法名调用,不需要声明对象
                                str += body.method.targetType.Name + "." + body.method.name;//设置参数值
                                if (!namespaces.Contains("using " + body.method.targetType.Namespace + ";") & body.method.targetType.Namespace != null & body.method.targetType.Namespace != "System")
                                {
                                    namespaces.Add("using " + body.method.targetType.Namespace + ";");
                                }
                            }
                            else
                            {
                                str += body.method.nodeName + "." + body.method.name;//设置参数值
                            }
                        }
                        else
                            Debug.Log("只写属性,不能获得属性值!");
                    }
                    if (body.method.Parameters[0].setValue)
                        Debug.Log(body.method.name + "只读属性,不能设置属性值!设置也不会叠加代码字段!");
                    break;
                case MemberTypes.Field:
                    if (body.setValue)
                    {//如果给对象赋值
                        if (body.method.fieldInfo.IsStatic)//如果变量是静态不能给对象赋值
                            Debug.Log("静态变量不能设置对象值,直接使用类名点方法调用!");
                        else if (body.method.valueModel == Method.ValueModel.Get)
                        {//如果字段属于获得类型，那么就不需要给字段赋值
                            str += "(" + NInvoke(body.setValue) + ")." + body.method.name;//设置参数值
                        }
                        else
                        {//当给对象赋值,为了考虑赋值的长度问题,用一个()框起来,对象赋值完成后再点方法名
                            str += "((" + NInvoke(body.setValue) + ")." + body.method.name + " = " + SetPars(body) + ")";//设置参数值
                        }
                    }
                    else
                    {//当没有给对象赋值,使用自动对象调用方法
                        if (body.method.fieldInfo.IsStatic)
                        {//如果方法是静态方法使用类名点方法名调用,不需要声明对象
                            if (body.method.valueModel == Method.ValueModel.Get)
                            {//如果字段属于获得类型，那么就不需要给字段赋值
                                str += body.method.targetType.Name + "." + body.method.name;//设置参数值
                            }
                            else
                                str += body.method.targetType.Name + "." + body.method.name + " = ";//设置参数值
                            if (!namespaces.Contains("using " + body.method.targetType.Namespace + ";") & body.method.targetType.Namespace != null & body.method.targetType.Namespace != "System")
                            {
                                namespaces.Add("using " + body.method.targetType.Namespace + ";");
                            }
                        }
                        else if (body.method.valueModel == Method.ValueModel.Get)
                        {//如果字段属于获得类型，那么就不需要给字段赋值
                            str += body.method.nodeName + "." + body.method.name;//设置参数值
                        }
                        else
                        {
                            str += "(" + body.method.nodeName + "." + body.method.name + " = " + SetPars(body) + ")";//设置参数值
                        }
                    }
                    break;
                case MemberTypes.Constructor:
                    str += "new " + body.method.targetType.Name + "(";
                    str += SetPars(body);
                    str += ")";
                    if (body.setValue)
                    {
                        Debug.Log("new " + body.method.targetType.Name + "构造函数不能被设置对象,只能设置其参数");
                    }
                    break;
                case MemberTypes.Custom:
                    if (body.setValue)
                    {//如果给对象赋值
                        str += body.method.nodeName + " = " + NInvoke(body.setValue, MemberTypes.Custom);//从对象出发
                    }
                    else
                    {
                        str += body.method.nodeName + "";
                    }
                    break;
            }
            if (body.runtime != null)
            { //接入节点
                str += Invoke(body.runtime, "\t");
            }
            return str;
        }

        string SetPars(Node body)
        {
            string str = "";
            for (int i = 0; i < body.method.Parameters.Count; ++i)
            {
                var par = body.method.Parameters[i];
                bool length = (i != body.method.Parameters.Count - 1);
                if (par.setValue)
                {//如果给参数赋值
                    MemberTypes metype = par.setValue.method.memberTypes;//方法和属性,字段,构造都是由返回值返回的类
                    if (metype == MemberTypes.Method | metype == MemberTypes.Property | metype == MemberTypes.Field)
                    {
                        if (par.parameterTypeName != "System.Object" & par.parameterTypeName != par.setValue.method.returnTypeName)
                        {//如果类型不同,就要强制转换类型才能编译通过
                            str += "(" + par.parameterType.Name + ")" + NInvoke(par.setValue) + (length ? "," : ""); // 设置参数值
                            if (!namespaces.Contains("using " + par.parameterType.Namespace + ";") & par.parameterType.Namespace != null & par.parameterType.Namespace != "System")
                            {
                                namespaces.Add("using " + par.parameterType.Namespace + ";");
                            }
                            continue;//如果给参数赋值,下面自带值就不使用
                        }
                        else
                        {
                            str += NInvoke(par.setValue) + (length ? "," : ""); // 设置参数值
                            continue;//如果给参数赋值,下面自带值就不使用
                        }
                    }
                    else if (metype == MemberTypes.Custom | metype == MemberTypes.Constructor)
                    {//如果节点是对象,比较对象类型
                        if (par.parameterTypeName != "System.Object" & par.parameterTypeName != par.setValue.method.targetTypeName)
                        {// (这里使用的对象类型) 如果类型不同,就要强制转换类型才能编译通过
                            str += "(" + par.parameterType.Name + ")" + NInvoke(par.setValue) + (length ? "," : ""); // 设置参数值
                            continue;//如果给参数赋值,下面自带值就不使用
                        }
                        else
                        {
                            str += NInvoke(par.setValue) + (length ? "," : ""); // 设置参数值
                            continue;//如果给参数赋值,下面自带值就不使用
                        }
                    }
                }
                //当使用自带参数值,这也是最后一步的赋值给方法的参数值(简称结尾参数值)
                if (SystemType.IsSubclassOf(par.parameterType, typeof(UnityEngine.Object)))
                {
                    str += "null" + (length ? "," : "");
                }
                else
                {
                    switch (par.parameterTypeName)
                    {
                        case "System.Single":
                            str += par.Value + (length ? "F," : "F");
                            break;
                        case "System.Int32":
                            str += par.Value + (length ? "," : "");
                            break;
                        case "System.Boolean":
                            str += ConvertUtility.BooleanToString(par.Value) + (length ? "," : "");
                            break;
                        case "System.String":
                            str += "" + '"' + par.Value + '"' + (length ? "," : "");
                            break;
                        case "UnityEngine.Vector2":
                            str += "new UnityEngine.Vector2" + ConvertUtility.Vector2_3_4ToString(par.Value) + (length ? "," : "");
                            break;
                        case "UnityEngine.Vector3":
                            str += "new UnityEngine.Vector3" + ConvertUtility.Vector2_3_4ToString(par.Value) + (length ? "," : "");
                            break;
                        case "UnityEngine.Vector4":
                            str += "new UnityEngine.Vector4" + ConvertUtility.Vector2_3_4ToString(par.Value) + (length ? "," : "");
                            break;
                        case "UnityEngine.Rect":
                            str += "new UnityEngine.Rect" + ConvertUtility.Vector234_Rect_Quaternion_ColorToString(par.Value) + (length ? "," : "");
                            break;
                        case "UnityEngine.Color":
                            str += "new UnityEngine.Color" + ConvertUtility.Vector234_Rect_Quaternion_ColorToString(par.Value) + (length ? "," : "");
                            break;
                        case "UnityEngine.Quaternion":
                            str += "new UnityEngine.Quaternion" + ConvertUtility.Vector234_Rect_Quaternion_ColorToString(par.Value) + (length ? "," : "");
                            break;
                        case "System.Object":
                            str += "new System.Object()" + (length ? "," : "");
                            break;
                        default:
                            if (par.parameterType.IsEnum)
                            {
                                str += par.parameterType.Name + "." + par.Value + (length ? "," : "");
                                if (!namespaces.Contains("using " + par.parameterType.Namespace + ";") & par.parameterType.Namespace != null & par.parameterType.Namespace != "System")
                                {
                                    namespaces.Add("using " + par.parameterType.Namespace + ";");
                                }
                            }
                            else if (par.parameterType.GetConstructors().Length == 0)
                            {//如果参数类型没有构造函数,赋值一个空的值,不能创建对象
                                str += "null" + (length ? "," : "");
                            }
                            else
                            {
                                str += "new " + par.parameterType.Name + "()" + (length ? "," : "");
                                if (!namespaces.Contains("using " + par.parameterType.Namespace + ";") & par.parameterType.Namespace != null & par.parameterType.Namespace != "System")
                                {
                                    namespaces.Add("using " + par.parameterType.Namespace + ";");
                                }
                            }
                            break;
                    }
                }
            }
            return str;
        }
    }
}
#endif