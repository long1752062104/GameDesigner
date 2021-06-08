#if UNITY_EDITOR
using GameDesigner;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PluginSettings : EditorWindow
{
    private static string chinesePath, englishPath;
    private static PluginLanguage language = PluginLanguage.Chinese;

    private void Awake()
    {
        InitPlugin();
    }

    public static void InitPlugin()
    {
        DirectoryInfo info = new DirectoryInfo(Application.dataPath);
        var files = info.GetFiles("ChineseLanguage.language", SearchOption.AllDirectories);
        if (files.Length > 0)
        {
            chinesePath = files[0].FullName;
        }
        files = info.GetFiles("EnglishLanguage.language", SearchOption.AllDirectories);
        if (files.Length > 0)
        {
            englishPath = files[0].FullName;
        }
        if (BlueprintSetting.Instance.language == PluginLanguage.Chinese & chinesePath != null)
        {
            BlueprintSetting.Instance.LANGUAGE = File.ReadAllLines(chinesePath);
        }
        else if (englishPath != null)
        {
            BlueprintSetting.Instance.LANGUAGE = File.ReadAllLines(englishPath);
        }
    }

    [MenuItem("GameDesigner/PluginSettings")]
    static void Init()
    {
        var setting = GetWindow<PluginSettings>();
        setting.maxSize = new Vector2(300, 100);
        setting.Show();
    }

    void OnGUI()
    {
        if (language != BlueprintSetting.Instance.language)
        {
            if (BlueprintSetting.Instance.language == PluginLanguage.Chinese & chinesePath != null)
                BlueprintSetting.Instance.LANGUAGE = File.ReadAllLines(chinesePath);
            else if (englishPath != null)
                BlueprintSetting.Instance.LANGUAGE = File.ReadAllLines(englishPath);
            language = BlueprintSetting.Instance.language;
        }
        BlueprintSetting.Instance.language = (PluginLanguage)EditorGUILayout.EnumPopup(BlueprintSetting.Instance.LANGUAGE[83], BlueprintSetting.Instance.language);
    }
}
#endif