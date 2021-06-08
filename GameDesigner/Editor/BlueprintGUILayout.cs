#if UNITY_EDITOR
using GameDesigner;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class BlueprintGUILayout : Editor
{
    public static BlueprintSetting Instance
    {
        get
        {
            return BlueprintSetting.Instance;
        }
    }

    /// <summary>
    /// 开始水平向前移动几个格子
    /// </summary>

    public static void BeginSpaceHorizontal(float pixels, string styleName)
    {
        EditorGUILayout.BeginHorizontal(styleName);
        GUILayout.Space(pixels);
        EditorGUILayout.BeginVertical();
    }

    /// <summary>
    /// 开始水平向前移动几个格子
    /// </summary>

    public static void BeginSpaceHorizontal(float pixels = 10f)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(pixels);
        EditorGUILayout.BeginVertical();
    }

    /// <summary>
    /// 结束水平向前移动
    /// </summary>

    public static void EndSpaceHorizontal()
    {
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// 开始水平向前移动几个格子
    /// </summary>

    public static void BeginStyleVertical(string name = "系统基础动作组件", string styleName = "ProgressBarBack")
    {
        GUILayout.Button(name, GUI.skin.GetStyle("dragtabdropwindow"));
        if (styleName == "")
            EditorGUILayout.BeginVertical();
        else
            EditorGUILayout.BeginVertical(styleName);
    }

    /// <summary>
    /// 结束水平向前移动
    /// </summary>

    public static void EndStyleVertical()
    {
        EditorGUILayout.EndVertical();
    }

    public static void TextColor(Rect rect, string label, Color color, float offset = 25)
    {
        Color c = GUI.skin.label.normal.textColor;
        GUI.skin.label.normal.textColor = color;
        GUI.Label(new Rect(rect.x + offset, rect.y, rect.width - offset, rect.height), label);
        GUI.skin.label.normal.textColor = c;
    }

    public static bool Button(Rect rect, GUIStyle Style, float offset = 25)
    {
        return DrawTextureButton(rect, Style, "", offset);
    }

    public static bool Button(Rect rect, GUIStyle Style, string tooltip, float offset = 25)
    {
        return DrawTextureButton(rect, Style, tooltip, offset);
    }

    public static bool DrawTextureButton(Rect rect, GUIStyle Style, string tooltip, float offset = 25)
    {
        GUI.Label(new Rect(rect.x, rect.y, rect.width, rect.height), "", Style);
        if (GUI.Button(new Rect(rect.x + offset, rect.y, rect.width - offset, rect.height), new GUIContent("", tooltip), "scrollview"))
        {
            return true;
        }
        return false;
    }

    public static bool DrawTextureButton(Rect rect, Texture Style, float offset = 25)
    {
        GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, rect.height), Style);
        if (GUI.Button(new Rect(rect.x + offset, rect.y, rect.width - offset, rect.height), "", "scrollview"))
        {
            return true;
        }
        return false;
    }

    public static void LabelTexture(Rect rect, string label, Texture Style, string styleName = "LODSliderRangeSelected", float offset = 25)
    {
        GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, rect.height), Style);
        GUI.Label(new Rect(rect.x + offset, rect.y, rect.width - offset, rect.height), label, styleName);
    }

    /// <summary>
    /// 在监视目标显示类的值并且可视化修改 （ 将value值穿进去赋值并且返回修改后的值 ）
    /// typeName(类型完全限定符) , 如果typeName为默认则获取value对象的类型来判断编辑器字段显示,否则使用typeName名称来获取类型在编辑器字段显示
    /// </summary>

    public static object PropertyField(Rect rect, string label, object value, System.Type type = null, GameDesigner.ValueType.TypeParameter typeName = null, Texture image = null, bool isTargetType = false)
    {
        return PropertyField(rect, label, value, type, typeName, image, "", isTargetType);
    }

    /// <summary>
    /// 在监视目标显示类的值并且可视化修改 （ 将value值穿进去赋值并且返回修改后的值 ）
    /// </summary>

    public static object PropertyField(Rect rect, string label, object value, System.Type type, GameDesigner.ValueType.TypeParameter typeName, Texture image = null, string tooltip = "", bool isTargetType = false)
    {
        try
        {
            return PropertyFields(rect, label, value, type, typeName, image, tooltip, isTargetType);
        }
        catch { return null; }
    }

    /// <summary>
    /// 在监视目标显示类的值并且可视化修改 （ 将value值穿进去赋值并且返回修改后的值 ）
    /// </summary>

    public static object PropertyFields(Rect rect, string label, object value, System.Type type, GameDesigner.ValueType.TypeParameter typeName, Texture image = null, string tooltip = "", bool isTargetType = false)
    {
        float width = GUI.skin.label.CalcSize(new GUIContent(label)).x;
        if (value == null)
        {
            GUI.Label(new Rect(rect.x, rect.y, 20, 18), new GUIContent("N", tooltip), Instance.nullStyle);
            GUI.Label(new Rect(rect.x + 20, rect.y, width, 18), new GUIContent(label, tooltip));
            EditorGUI.LabelField(new Rect(rect.x + 20 + width, rect.y, rect.width - 20 - width, 16), label, "null--value!!!");
            return null;
        }

        switch (type.FullName)
        {
            case "System.Int32":
                GUI.Label(new Rect(rect.x, rect.y, 20, 18), new GUIContent("I", tooltip), Instance.intStyle);
                GUI.Label(new Rect(rect.x + 20, rect.y, width, 18), new GUIContent(label, tooltip));
                return EditorGUI.IntField(new Rect(rect.x + 20 + width, rect.y, rect.width - 20 - width, 16), (int)value);
            case "System.Single":
                GUI.Label(new Rect(rect.x, rect.y, 20, 18), new GUIContent("F", tooltip), Instance.floatStyle);
                GUI.Label(new Rect(rect.x + 20, rect.y, width, 18), new GUIContent(label, tooltip));
                return EditorGUI.FloatField(new Rect(rect.x + 20 + width, rect.y, rect.width - 20 - width, 16), (float)value);
            case "System.String":
                GUI.Label(new Rect(rect.x, rect.y, 20, 18), new GUIContent("S", tooltip), Instance.stringStyle);
                GUI.Label(new Rect(rect.x + 20, rect.y, width, 18), new GUIContent(label, tooltip));
                return EditorGUI.TextField(new Rect(rect.x + 20 + width, rect.y, rect.width - 20 - width, 16), (string)value);
            case "System.Boolean":
                GUI.Label(new Rect(rect.x, rect.y, 20, 18), new GUIContent("B", tooltip), Instance.boolStyle);
                GUI.Label(new Rect(rect.x + 20, rect.y, width, 18), new GUIContent(label, tooltip));
                return EditorGUI.Toggle(new Rect(rect.x + 20 + width, rect.y, 20, 16), (bool)value);
            case "UnityEngine.Vector2":
                GUI.Label(new Rect(rect.x, rect.y, 20, 18), new GUIContent("2", tooltip), Instance.vector2Style);
                return EditorGUI.Vector2Field(new Rect(rect.x + 20, rect.y, rect.width - 20, 16), "", (Vector2)value);
            case "UnityEngine.Vector3":
                GUI.Label(new Rect(rect.x, rect.y, 20, 18), new GUIContent("3", tooltip), Instance.vector3Style);
                return EditorGUI.Vector3Field(new Rect(rect.x + 20, rect.y, rect.width - 20, 16), "", (Vector3)value);
            case "UnityEngine.Vector4":
                GUI.Label(new Rect(rect.x, rect.y, 20, 18), new GUIContent("4", tooltip), Instance.vector4Style);
                return EditorGUI.Vector4Field(new Rect(rect.x + 20, rect.y, rect.width - 20, 16), "", (Vector4)value);
            case "UnityEngine.Rect":
                Rect r = (Rect)value;
                GUI.Label(new Rect(rect.x, rect.y, 20, 18), new GUIContent("R", tooltip), Instance.rectStyle);
                Vector4 rv = EditorGUI.Vector4Field(new Rect(rect.x + 20, rect.y, rect.width - 20, 16), "", new Vector4(r.x, r.y, r.width, r.height));
                return new Rect(rv.x, rv.y, rv.z, rv.w);
            case "UnityEngine.Color":
                GUI.Label(new Rect(rect.x, rect.y, 20, 18), new GUIContent("C", tooltip), Instance.colorStyle);
                GUI.Label(new Rect(rect.x + 20, rect.y, width, 18), new GUIContent(label, tooltip));
                return EditorGUI.ColorField(new Rect(rect.x + 20 + width, rect.y, rect.width - 20 - width, 16), (Color)value);
            case "UnityEngine.AnimationCurve":
                GUI.Label(new Rect(rect.x, rect.y, 20, 18), new GUIContent("A", tooltip), Instance.animCurveStyle);
                GUI.Label(new Rect(rect.x + 20, rect.y, width, 18), new GUIContent(label, tooltip));
                return EditorGUI.CurveField(new Rect(rect.x + 20 + width, rect.y, rect.width - 20 - width, 16), (AnimationCurve)value);
            case "UnityEngine.Quaternion":
                Quaternion q = (Quaternion)value;
                GUI.Label(new Rect(rect.x, rect.y, 20, 18), new GUIContent("Q", tooltip), Instance.quaternionStyle);
                Vector4 v = EditorGUI.Vector4Field(new Rect(rect.x + 20, rect.y, rect.width - 20, 16), "", new Vector4(q.x, q.y, q.z, q.w));
                return new Quaternion(v.x, v.y, v.z, v.w);

            // -- -- 系统数据值类型-----------------

            case "System.Char":
                GUI.Label(new Rect(rect.x, rect.y, 20, 18), new GUIContent("C", tooltip), Instance.stringStyle);
                GUI.Label(new Rect(rect.x + 20, rect.y, width, 18), new GUIContent(label, tooltip));
                return EditorGUI.TextField(new Rect(rect.x + 20 + width, rect.y, rect.width - 20 - width, 16), value.ToString()).ToArray()[0];
            case "System.Int16":
                GUI.Label(new Rect(rect.x, rect.y, 20, 18), new GUIContent("I", tooltip), Instance.intStyle);
                GUI.Label(new Rect(rect.x + 20, rect.y, width, 18), new GUIContent(label, tooltip));
                return (Int16)EditorGUI.IntField(new Rect(rect.x + 20 + width, rect.y, rect.width - 20 - width, 16), (int)System.Convert.ToInt16(value));
            case "System.Int64":
                GUI.Label(new Rect(rect.x, rect.y, 20, 18), new GUIContent("I", tooltip), Instance.intStyle);
                GUI.Label(new Rect(rect.x + 20, rect.y, width, 18), new GUIContent(label, tooltip));
                return (Int64)EditorGUI.IntField(new Rect(rect.x + 20 + width, rect.y, rect.width - 20 - width, 16), (int)System.Convert.ToInt64(value));
            case "System.UInt16":
                GUI.Label(new Rect(rect.x, rect.y, 20, 18), new GUIContent("U", tooltip), Instance.intStyle);
                GUI.Label(new Rect(rect.x + 20, rect.y, width, 18), new GUIContent(label, tooltip));
                return (UInt16)EditorGUI.IntField(new Rect(rect.x + 20 + width, rect.y, rect.width - 20 - width, 16), (int)System.Convert.ToUInt16(value));
            case "System.UInt32":
                GUI.Label(new Rect(rect.x, rect.y, 20, 18), new GUIContent("U", tooltip), Instance.intStyle);
                GUI.Label(new Rect(rect.x + 20, rect.y, width, 18), new GUIContent(label, tooltip));
                return (UInt32)EditorGUI.IntField(new Rect(rect.x + 20 + width, rect.y, rect.width - 20 - width, 16), (int)System.Convert.ToUInt32(value));
            case "System.UInt64":
                GUI.Label(new Rect(rect.x, rect.y, 20, 18), new GUIContent("U", tooltip), Instance.intStyle);
                GUI.Label(new Rect(rect.x + 20, rect.y, width, 18), new GUIContent(label, tooltip));
                return (UInt64)EditorGUI.IntField(new Rect(rect.x + 20 + width, rect.y, rect.width - 20 - width, 16), (int)System.Convert.ToUInt64(value));
            case "System.Double":
                GUI.Label(new Rect(rect.x, rect.y, 20, 18), new GUIContent("D", tooltip), Instance.floatStyle);
                GUI.Label(new Rect(rect.x + 20, rect.y, width, 18), new GUIContent(label, tooltip));
                return EditorGUI.DoubleField(new Rect(rect.x + 20 + width, rect.y, rect.width - 20 - width, 16), (double)value);
            case "System.Byte":
                GUI.Label(new Rect(rect.x, rect.y, 20, 18), new GUIContent("B", tooltip), Instance.intStyle);
                GUI.Label(new Rect(rect.x + 20, rect.y, width, 18), new GUIContent(label, tooltip));
                return (Byte)EditorGUI.IntField(new Rect(rect.x + 20 + width, rect.y, rect.width - 20 - width, 16), (int)System.Convert.ToByte(value));
            case "System.SByte":
                GUI.Label(new Rect(rect.x, rect.y, 20, 18), new GUIContent("S", tooltip), Instance.intStyle);
                GUI.Label(new Rect(rect.x + 20, rect.y, width, 18), new GUIContent(label, tooltip));
                return (SByte)EditorGUI.IntField(new Rect(rect.x + 20 + width, rect.y, rect.width - 20 - width, 16), (int)System.Convert.ToSByte(value));
        }

        if (SystemType.IsSubclassOf(type, typeof(Object)))
        {
            GUI.Label(new Rect(rect.x, rect.y, 20, 18), new GUIContent(image, tooltip), Instance.ObjectStyle);
            GUI.Label(new Rect(rect.x + 20, rect.y, width, 18), new GUIContent(label, tooltip));
            return EditorGUI.ObjectField(new Rect(rect.x + 20 + width, rect.y, rect.width - 20 - width, 16), (Object)value, type, true);
        }
        if (SystemType.IsSubclassOf(type, typeof(Enum)))
        {
            GUI.Label(new Rect(rect.x, rect.y, 20, 18), new GUIContent("E", tooltip), Instance.enumStyle);
            GUI.Label(new Rect(rect.x + 20, rect.y, width, 18), new GUIContent(label, tooltip));
            return EditorGUI.EnumPopup(new Rect(rect.x + 20 + width, rect.y, rect.width - 20 - width, 16), (Enum)value);
        }
        if (type.FullName == "System.Type")
        {
            GUI.Label(new Rect(rect.x, rect.y, 20, 18), new GUIContent("T", tooltip), Instance.typeStyle);
            GUI.Label(new Rect(rect.x + 20, rect.y, width, 18), new GUIContent(label, tooltip));
            typeName.typeName = EditorGUI.TextField(new Rect(rect.x + 20 + width, rect.y, rect.width - 20 - width, 16), typeName.typeName);
            return typeName.type;
        }
        if (isTargetType)
        {
            GUI.Label(new Rect(rect.x, rect.y, 20, 18), new GUIContent("", tooltip), Instance.systemStyle);
            GUI.Label(new Rect(rect.x + 20 + width, rect.y, rect.width - 20 - width, 16), type.Name, Instance.classStyle);
        }
        else
        {
            GUI.Label(new Rect(rect.x, rect.y, 20, 18), new GUIContent("", tooltip), Instance.systemStyle);
            GUI.Label(new Rect(rect.x + 20, rect.y, width, 18), new GUIContent(label, tooltip));
            GUI.Label(new Rect(rect.x + 20 + width, rect.y, rect.width - 20 - width, 16), value.ToString(), Instance.classStyle);
        }
        return value;
    }

    /// <summary>
    /// 绘制只读字符串属性字段
    /// </summary>
    public static void PropertyField(Rect rect, string label, string value, string tooltip)
    {
        float width = GUI.skin.label.CalcSize(new GUIContent(label)).x;
        GUI.Label(new Rect(rect.x, rect.y, 20, 18), new GUIContent("S", tooltip), Instance.stringStyle);
        GUI.Label(new Rect(rect.x + 20, rect.y, width, 18), new GUIContent(label, tooltip));
        EditorGUI.LabelField(new Rect(rect.x + 20 + width, rect.y, rect.width - 20 - width, 16), value);
    }

    /// <summary>
    /// 在监视目标显示类的值并且可视化修改 （ 将value值穿进去赋值并且返回修改后的值 ）
    /// selectTypeIndex(选择类型索引) , 
    /// 重要提示: 如果value的类型是System.Type类型(Type类型包含UnityEngine命名空间的所有类,你只能选择一个类型,那么就要使用到索引)
    /// 如果你想要索引不变,你得使用你自己的一个类型索引变量来存储获得的类型索引,如果不使用,当其他类型为Type类型时,你所选择的类型会跟着变化(如果你只有一个Type调用此方法的前提下)
    /// selectTypeIndex = -1 是参数默认值 , -1 为使用系统静态变量
    /// </summary>

    public static object PropertyField(string label, object value, int selectTypeIndex = -1, params GUILayoutOption[] options)
    {
        if (value == null) { EditorGUILayout.TextField("null--value!!!", options); return value; }
        if (label == "" | label == null)
            return DrawPropertyField(value, value.GetType(), selectTypeIndex, options);
        return DrawPropertyField(label, value, value.GetType(), selectTypeIndex, options);
    }

    /// <summary>
    /// 在监视目标显示类的值并且可视化修改 （ 将value值穿进去赋值并且返回修改后的值 ）
    /// selectTypeIndex(选择类型索引) , 
    /// 重要提示: 如果value的类型是System.Type类型(Type类型包含UnityEngine命名空间的所有类,你只能选择一个类型,那么就要使用到索引)
    /// 如果你想要索引不变,你得使用你自己的一个类型索引变量来存储获得的类型索引,如果不使用,当其他类型为Type类型时,你所选择的类型会跟着变化(如果你只有一个Type调用此方法的前提下)
    /// selectTypeIndex = -1 是参数默认值 , -1 为使用系统静态变量
    /// </summary>

    public static object PropertyField(string label, object value, System.Type type, int selectTypeIndex = -1, params GUILayoutOption[] options)
    {
        if (label == "" | label == null)
            return DrawPropertyField(value, type, selectTypeIndex, options);
        return DrawPropertyField(label, value, type, selectTypeIndex, options);
    }

    /// <summary>
    /// 绘制没有标签的字段
    /// </summary>

    static object DrawPropertyField(object value, Type type, int selectTypeIndex = -1, params GUILayoutOption[] options)
    {
        if (type == null)
        {
            EditorGUILayout.TextField("null--type!!!", options);
            return value;
        }
        switch (type.FullName)
        {
            case "System.Int32":
                return EditorGUILayout.IntField((int)value, options);
            case "System.Single":
                return EditorGUILayout.FloatField((float)value, options);
            case "System.String":
                return EditorGUILayout.TextField((string)value, options);
            case "System.Boolean":
                return EditorGUILayout.Toggle((bool)value, options);
            case "UnityEngine.Vector2":
                return EditorGUILayout.Vector2Field("", (Vector2)value, options);
            case "UnityEngine.Vector3":
                return EditorGUILayout.Vector3Field("", (Vector3)value, options);
            case "UnityEngine.Vector4":
                return EditorGUILayout.Vector4Field("", (Vector4)value, options);
            case "UnityEngine.Rect":
                return EditorGUILayout.RectField((Rect)value, options);
            case "UnityEngine.Color":
                return EditorGUILayout.ColorField((Color)value, options);
            case "UnityEngine.AnimationCurve":
                return EditorGUILayout.CurveField((AnimationCurve)value, options);
            case "UnityEngine.Quaternion":
                Quaternion q = (Quaternion)value;
                Vector4 v = EditorGUILayout.Vector4Field("", new Vector4(q.x, q.y, q.z, q.w), options);
                return new Quaternion(v.x, v.y, v.z, v.w);
        }
        if (SystemType.IsSubclassOf(type, typeof(Object)))
            return EditorGUILayout.ObjectField((Object)value, value.GetType(), true, options);
        if (type.IsEnum)
            return EditorGUILayout.EnumPopup((Enum)value, options);
        EditorGUILayout.TextField(value.ToString(), options);
        return value;
    }

    /// <summary>
    /// 绘制有标签的字段
    /// </summary>

    static object DrawPropertyField(string label, object value, Type type, int selectTypeIndex = -1, params GUILayoutOption[] options)
    {
        if (type == null)
        {
            EditorGUILayout.TextField("null--type!!!", options);
            return value;
        }
        switch (type.FullName)
        {
            case "System.Int32":
                return EditorGUILayout.IntField(label, (int)value, options);
            case "System.Single":
                return EditorGUILayout.FloatField(label, (float)value, options);
            case "System.String":
                return EditorGUILayout.TextField(label, (string)value, options);
            case "System.Boolean":
                return EditorGUILayout.Toggle(label, (bool)value, options);
            case "UnityEngine.Vector2":
                return EditorGUILayout.Vector2Field(label, (Vector2)value, options);
            case "UnityEngine.Vector3":
                return EditorGUILayout.Vector3Field(label, (Vector3)value, options);
            case "UnityEngine.Vector4":
                return EditorGUILayout.Vector4Field(label, (Vector4)value, options);
            case "UnityEngine.Rect":
                return EditorGUILayout.RectField(label, (Rect)value, options);
            case "UnityEngine.Color":
                return EditorGUILayout.ColorField(label, (Color)value, options);
            case "UnityEngine.AnimationCurve":
                return EditorGUILayout.CurveField(label, (AnimationCurve)value, options);
            case "UnityEngine.Quaternion":
                Quaternion q = (Quaternion)value;
                Vector4 v = EditorGUILayout.Vector4Field(label, new Vector4(q.x, q.y, q.z, q.w), options);
                return new Quaternion(v.x, v.y, v.z, v.w);
        }
        if (SystemType.IsSubclassOf(type, typeof(Object)))
            return EditorGUILayout.ObjectField(label, (Object)value, type, true, options);
        if (type.IsEnum)
            return EditorGUILayout.EnumPopup(label, (Enum)value, options);
        EditorGUILayout.TextField(label, value.ToString(), options);
        return value;
    }
}
#endif