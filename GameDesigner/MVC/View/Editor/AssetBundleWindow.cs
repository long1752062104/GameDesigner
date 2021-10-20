#if UNITY_EDITOR
using Net.System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetBundleWindow : EditorWindow
{
    private BuildTarget target = BuildTarget.StandaloneWindows;
    private string path = "AssetBundles";
    MyDictionary<string, bool> bundleNames;

    [MenuItem("GameDesigner/Hotfix/AssetBundleBuild")]
    static void ShowWindow()
    {
        GetWindow<AssetBundleWindow>().Show();
    }

    private void OnEnable()
    {
        bundleNames = new MyDictionary<string, bool>();
        var bundleNames1 = AssetDatabase.GetAllAssetBundleNames();
        foreach (var name in bundleNames1)
        {
            bundleNames.Add(name, false);
        }
    }

    private void OnGUI()
    {
        target = (BuildTarget)EditorGUILayout.EnumPopup("平台:", target);
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("保存路径:", path);
        if (GUILayout.Button("选择路径", GUILayout.Width(100)))
        {
            path = EditorUtility.OpenFolderPanel("保存路径", "", "");
        }
        GUILayout.EndHorizontal();
        for (int i = 0; i < bundleNames.Count; i++)
        {
            bundleNames.entries[i].value = GUILayout.Toggle(bundleNames.entries[i].value, bundleNames.entries[i].key);
        }
        if (GUILayout.Button("构建AB包", GUILayout.Height(30)))
        {
            if (path.Length != 0 & target != BuildTarget.NoTarget)
            {
                List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
                foreach (var name in bundleNames)
                {
                    if (!name.Value)
                        continue;
                    AssetBundleBuild build = new AssetBundleBuild();
                    build.assetBundleName = name.Key;
                    build.assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(name.Key);
                    builds.Add(build);
                }
                BuildPipeline.BuildAssetBundles(path, builds.ToArray(), BuildAssetBundleOptions.None, target);
            }
        }
    }
}
#endif