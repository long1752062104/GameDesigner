#if UNITY_EDITOR
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

public class BlueprintSetting : ScriptableObject
{
    static private BlueprintSetting _instance = null;
    static public BlueprintSetting Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<BlueprintSetting>("BlueprintSetting");
                if (_instance == null)
                {
                    _instance = CreateInstance<BlueprintSetting>();
                    var path = "Assets/" + GetGameDesignerPath.Split(new string[] { @"Assets\" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    UnityEditor.AssetDatabase.CreateAsset(_instance, path + "/Editor/Resources/BlueprintSetting.asset");
                }
                PluginSettings.InitPlugin();
            }
            return _instance;
        }
    }

    public static string GetGameDesignerPath
    {
        get
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath);
            var dirs = directoryInfo.GetDirectories("GameDesigner", SearchOption.AllDirectories);
            if (dirs.Length > 0)
                return dirs[0].FullName;
            return string.Empty;
        }
    }

    /// <summary>
    /// 解释 : 判断type的基类是否是Typeof类型,是返回真,不是返回假
    /// </summary>

    static public bool IsSubclassOf(Type type, Type Typeof)
    {
        if (type == null | Typeof == null)
            return false;
        if (type.IsSubclassOf(Typeof) | type == Typeof)
            return true;
        return false;
    }

    /// <summary>
    /// 设置类的变量值,解决派生类的值控制父类的变量值 ( 被赋值变量对象 , 赋值变量对象 ) [尽可能的使用此方法,此方法产生GC]
    /// </summary>

    static void SetPropertyValue(GUIStyle style)
    {
        style.normal.background = Resources.Load<Texture2D>("RadioButton_Off");
        style.normal.textColor = Color.white;
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.MiddleCenter;
    }

    static void PropertyFor(object target, object setValue)
    {
        if (target == null)
            return;

        foreach (PropertyInfo property in target.GetType().GetProperties())
        {
            if (!property.CanWrite)
                continue;
            if (IsSubclassOf(property.PropertyType, typeof(UnityEngine.Object)) | property.PropertyType == typeof(string) | property.PropertyType == typeof(object) | property.PropertyType.IsValueType | property.PropertyType.IsEnum)
            {
                property.SetValue(target, property.GetValue(setValue, null), null);
            }
            else
            {
                PropertyFor(property.GetValue(target, null), property.GetValue(setValue, null));
            }
        }
    }

#if UNITY_EDITOR || DEBUG
    [Header("int型皮肤")]
    public string intStyleName = "button";
    [SerializeField]
    private GUIStyle _intStyle = null;
    private GUIStyle _intStyle1 = null;
    public GUIStyle intStyle
    {
        get
        {
            return GetBaseStyle(ref _intStyle, ref _intStyle1, intStyleName);
        }
    }

    private GUIStyle GetBaseStyle(ref GUIStyle style, ref GUIStyle style1, string styleName)
    {
        if (style1 == null)
            goto JUMP;
        if (style1.normal.background != null)
            return style1;
        if (style == null)
            goto JUMP;
        if (style.normal.background == null)
            goto JUMP;
        goto RET;
    JUMP: style = new GUIStyle(GUI.skin.GetStyle(styleName));
        SetPropertyValue(style);
    RET: return style1 = style;
    }

    private GUIStyle GetNodeStyle(ref GUIStyle style, ref GUIStyle style1, string styleName, Action<GUIStyle> action = null)
    {
        if (style1 == null)
            goto JUMP;
        if (style1.normal.background != null)
            return style1;
        if (style == null)
            goto JUMP;
        if (style.normal.background == null)
            goto JUMP;
        goto RET;
    JUMP: style = new GUIStyle(GUI.skin.GetStyle(styleName));
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.UpperCenter;
        action?.Invoke(style);
    RET: return style1 = style;
    }

    [Header("float型皮肤")]
    public string floatStyleName = "button";
    [SerializeField]
    private GUIStyle _floatStyle = null; private GUIStyle _floatStyle1 = null;
    public GUIStyle floatStyle
    {
        get
        {
            return GetBaseStyle(ref _floatStyle, ref _floatStyle1, floatStyleName);
        }
    }

    [Header("string型皮肤")]
    public string stringStyleName = "button";
    [SerializeField]
    private GUIStyle _stringStyle = null; private GUIStyle _stringStyle1 = null;
    public GUIStyle stringStyle
    {
        get
        {
            return GetBaseStyle(ref _stringStyle, ref _stringStyle1, stringStyleName);
        }
    }

    [Header("bool型皮肤")]
    public string boolStyleName = "button";
    [SerializeField]
    private GUIStyle _boolStyle = null; private GUIStyle _boolStyle1 = null;
    public GUIStyle boolStyle
    {
        get
        {
            return GetBaseStyle(ref _boolStyle, ref _boolStyle1, boolStyleName);
        }
    }

    [Header("vector2 型皮肤")]
    public string vector2StyleName = "button";
    [SerializeField]
    private GUIStyle _vector2Style = null; private GUIStyle _vector2Style1 = null;
    public GUIStyle vector2Style
    {
        get
        {
            return GetBaseStyle(ref _vector2Style, ref _vector2Style1, vector2StyleName);
        }
    }

    [Header("vector3 型皮肤")]
    public string vector3StyleName = "button";
    [SerializeField]
    private GUIStyle _vector3Style = null; private GUIStyle _vector3Style1 = null;
    public GUIStyle vector3Style
    {
        get
        {
            return GetBaseStyle(ref _vector3Style, ref _vector3Style1, vector3StyleName);
        }
    }

    [Header("vector4 型皮肤")]
    public string vector4StyleName = "button";
    [SerializeField]
    private GUIStyle _vector4Style = null; private GUIStyle _vector4Style1 = null;
    public GUIStyle vector4Style
    {
        get
        {
            return GetBaseStyle(ref _vector4Style, ref _vector4Style1, vector4StyleName);
        }
    }

    [Header("rect 型皮肤")]
    public string rectStyleName = "button";
    [SerializeField]
    private GUIStyle _rectStyle = null; private GUIStyle _rectStyle1 = null;
    public GUIStyle rectStyle
    {
        get
        {
            return GetBaseStyle(ref _rectStyle, ref _rectStyle1, rectStyleName);
        }
    }

    [Header("quaternion 型皮肤")]
    public string quaternionStyleName = "button";
    [SerializeField]
    private GUIStyle _quaternionStyle = null;
    public GUIStyle quaternionStyle
    {
        get
        {
            return GetBaseStyle(ref _quaternionStyle, ref _quaternionStyle, quaternionStyleName);
        }
    }

    [Header("color 型皮肤")]
    public string colorStyleName = "button";
    [SerializeField]
    private GUIStyle _colorStyle = null; private GUIStyle _colorStyle1 = null;
    public GUIStyle colorStyle
    {
        get
        {
            return GetBaseStyle(ref _colorStyle, ref _colorStyle1, colorStyleName);
        }
    }

    [Header("animCurve 型皮肤")]
    public string animCurveStyleName = "button";
    [SerializeField]
    private GUIStyle _animCurveStyle = null; private GUIStyle _animCurveStyle1 = null;
    public GUIStyle animCurveStyle
    {
        get
        {
            return GetBaseStyle(ref _animCurveStyle, ref _animCurveStyle1, animCurveStyleName);
        }
    }

    [Header("enum 型皮肤")]
    public string enumStyleName = "button";
    [SerializeField]
    private GUIStyle _enumStyle = null; private GUIStyle _enumStyle1 = null;
    public GUIStyle enumStyle
    {
        get
        {
            return GetBaseStyle(ref _enumStyle, ref _enumStyle1, enumStyleName);
        }
    }

    [Header("type 型皮肤")]
    public string typeStyleName = "button";
    [SerializeField]
    private GUIStyle _typeStyle = null; private GUIStyle _typeStyle1 = null;
    public GUIStyle typeStyle
    {
        get
        {
            return GetBaseStyle(ref _typeStyle, ref _typeStyle1, typeStyleName);
        }
    }

    [Header("class对象类型皮肤")]
    public string classStyleName = "button";
    [SerializeField]
    private GUIStyle _classStyle = null; private GUIStyle _classStyle1 = null;
    public GUIStyle classStyle
    {
        get
        {
            return GetBaseStyle(ref _classStyle, ref _classStyle1, classStyleName);
        }
    }

    [Header("null类型皮肤")]
    public string nullStyleName = "button";
    [SerializeField]
    private GUIStyle _nullStyle = null; private GUIStyle _nullStyle1 = null;
    public GUIStyle nullStyle
    {
        get
        {
            return GetBaseStyle(ref _nullStyle, ref _nullStyle1, nullStyleName);
        }
    }

    [Header("添加节点皮肤")]
    [SerializeField]
    private Texture _unityEngineImage = null;
    public Texture unityEngineImage
    {
        get
        {
            if (_unityEngineImage == null)
            {
                return _unityEngineImage = Resources.Load<Texture>("new_Add");
            }
            return _unityEngineImage;
        }
    }

    [Header("其他类型皮肤")]
    public string systemStyleName = "button";
    [SerializeField]
    private GUIStyle _systemStyle = null; private GUIStyle _systemStyle1 = null;
    public GUIStyle systemStyle
    {
        get
        {
            return GetBaseStyle(ref _systemStyle, ref _systemStyle1, systemStyleName);
        }
    }

    [Header("c#皮肤")]
    [SerializeField]
    private Texture _cshImage = null;
    public Texture cshImage
    {
        get
        {
            if (_cshImage == null)
            {
                return _cshImage = Resources.Load<Texture>("Icon_Csh");
            }
            return _cshImage;
        }
    }

    [Header("状态机皮肤")]
    [SerializeField]
    private Texture _stateMachineImage = null;
    public Texture stateMachineImage
    {
        get
        {
            if (_stateMachineImage == null)
            {
                return _stateMachineImage = Resources.Load<Texture>("stateStyle");
            }
            return _stateMachineImage;
        }
    }

    [Header("状态机名称皮肤")]
    public string stateMachineStyleName = "GUIEditor.BreadcrumbLeft";
    [SerializeField]
    private GUIStyle _stateMachineStyle = null; private GUIStyle _stateMachineStyle1 = null;
    public GUIStyle stateMachineStyle
    {
        get
        {
            return GetBaseStyle(ref _stateMachineStyle, ref _stateMachineStyle1, stateMachineStyleName);
        }
    }

    [Header("参数类型皮肤")]
    [SerializeField]
    private Texture _parameterTypeImage = null;
    public Texture parameterTypeImage
    {
        get
        {
            if (_parameterTypeImage == null)
            {
                return _parameterTypeImage = GUI.skin.GetStyle("MeTransPlayhead").normal.background;
            }
            return _parameterTypeImage;
        }
    }

    [Header("设置值皮肤")]
    public string setValueStyleName = "PreSliderThumb";
    [SerializeField]
    private GUIStyle _setValueStyle = null; private GUIStyle _setValueStyle1 = null;
    public GUIStyle setValueStyle
    {
        get
        {
            return GetNodeStyle(ref _setValueStyle, ref _setValueStyle1, setValueStyleName);
        }
    }

    [Header("获取值皮肤")]
    public string getValueStyleName = "PreSliderThumb";
    [SerializeField]
    private GUIStyle _getValueStyle = null; private GUIStyle _getValueStyle1 = null;
    public GUIStyle getValueStyle
    {
        get
        {
            return GetNodeStyle(ref _getValueStyle, ref _getValueStyle1, getValueStyleName);
        }
    }

    [Header("设置运行路径皮肤")]
    public string setRuntimeStyleName = "PreSliderThumb";
    [SerializeField]
    private GUIStyle _setRuntimeStyle = null; private GUIStyle _setRuntimeStyle1 = null;
    public GUIStyle setRuntimeStyle
    {
        get
        {
            return GetNodeStyle(ref _setRuntimeStyle, ref _setRuntimeStyle1, setRuntimeStyleName);
        }
    }

    [Header("获取运行路径皮肤")]
    public string getRuntimeStyleName = "PreSliderThumb";
    public bool initgetRuntimeStyle = true;
    [SerializeField]
    private GUIStyle _getRuntimeStyle = null; private GUIStyle _getRuntimeStyle1 = null;
    public GUIStyle getRuntimeStyle
    {
        get
        {
            return GetNodeStyle(ref _getRuntimeStyle, ref _getRuntimeStyle1, getRuntimeStyleName);
        }
    }

    [Header("设置参数名称皮肤")]
    public string setParamsStyleName = "PreSliderThumb";
    [SerializeField]
    private GUIStyle _setParamsStyle = null; private GUIStyle _setParamsStyle1 = null;
    public GUIStyle setParamsStyle
    {
        get
        {
            return GetNodeStyle(ref _setParamsStyle, ref _setParamsStyle1, setParamsStyleName);
        }
    }

    [Header("对象名称皮肤")]
    public string ObjectStyleStyleName = "label";
    [SerializeField]
    private GUIStyle _ObjectStyleStyle = null; private GUIStyle _ObjectStyleStyle1 = null;
    public GUIStyle ObjectStyle
    {
        get
        {
            return GetNodeStyle(ref _ObjectStyleStyle, ref _ObjectStyleStyle1, ObjectStyleStyleName);
        }
    }

    [Header("类型皮肤")]
    public string classStyles = "ProfilerSelectedLabel";

    [Header("node名称皮肤")]
    public string nodeNameStyle = "LODSceneText";
    [SerializeField]
    private GUIStyle _nodeStyle = null; private GUIStyle _nodeStyle1 = null;
    public GUIStyle nodeStyle
    {
        get
        {
            return GetNodeStyle(ref _nodeStyle, ref _nodeStyle1, nodeNameStyle);
        }
    }

    [Header("对象类型名称皮肤")]
    public string targetTypeNameStyle = "BoldLabel";
    [SerializeField]
    private GUIStyle _targetTypeStyle = null; private GUIStyle _targetTypeStyle1 = null;
    public GUIStyle targetTypeStyle
    {
        get
        {
            return GetNodeStyle(ref _targetTypeStyle, ref _targetTypeStyle1, targetTypeNameStyle);
        }
    }

    [Header("方法名称皮肤")]
    public string methodNameStyle = "ErrorLabel";
    [SerializeField]
    private GUIStyle _methodStyle = null;
    private GUIStyle _methodStyle1 = null;
    public GUIStyle methodStyle
    {
        get
        {
            return GetNodeStyle(ref _methodStyle, ref _methodStyle1, methodNameStyle);
        }
    }

    [Header("node字段,属性,方法皮肤")]
    public string typeFPMStyleNames = "ButtonMid";
    [SerializeField]
    private GUIStyle _typeFPMStyles = null; private GUIStyle _typeFPMStyles1 = null;
    public GUIStyle typeFPMStyle
    {
        get
        {
            return GetNodeStyle(ref _typeFPMStyles, ref _typeFPMStyles1, typeFPMStyleNames);
        }
    }

    [Header("横向间隔条皮肤")]
    public string horSpaceStyleNames = "ButtonMid";
    [SerializeField]
    private GUIStyle horSpaceStyle = null; private GUIStyle horSpaceStyle1 = null;
    public GUIStyle HorSpaceStyle
    {
        get
        {
            return GetNodeStyle(ref horSpaceStyle, ref horSpaceStyle1, horSpaceStyleNames);
        }
    }

    [Header("node选择方法皮肤")]
    public string selectMethodStyleName = "box";
    [SerializeField]
    private GUIStyle _selectMethodStyl = null; private GUIStyle _selectMethodStyl1 = null;
    public GUIStyle selectMethodStyl
    {
        get
        {
            return GetNodeStyle(ref _selectMethodStyl, ref _selectMethodStyl1, selectMethodStyleName, style =>
            {
                style.fontStyle = FontStyle.Bold;
                style.alignment = TextAnchor.UpperLeft;
            });
        }
    }

    [Header("node字段,属性,方法的文字颜色")]
    public Color nodeTextColor = Color.red;

    public ScriptableObject GraphEditor = null;
    public ScriptableObject BlueprintEditor = null;
#endif
    public PluginLanguage language = PluginLanguage.Chinese;
    public string[] LANGUAGE = new string[120];
}

/// <summary>
/// 插件语言
/// </summary>
public enum PluginLanguage
{
    /// <summary>
    /// 英文
    /// </summary>
    English,
    /// <summary>
    /// 中文
    /// </summary>
    Chinese
}
#endif