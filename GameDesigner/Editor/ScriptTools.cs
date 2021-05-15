#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public class ScriptTools : MonoBehaviour
{

    /// <summary>
    /// path = Application.dataPath + @"GameDesigner\Skill\StateAction"
    /// </summary>

    static public void CreateScript(string path, string scriptName, string textContents)
    {
        if (Directory.Exists(path) == false)
            Directory.CreateDirectory(path);
        File.AppendAllText(path + "/" + scriptName + ".cs", textContents);
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// path = Application.dataPath + @"GameDesigner\Skill\StateAction"
    /// </summary>
    static public void CreateScript(string path, string scriptName, string[] scriptText)
    {
        if (Directory.Exists(path) == false)
            Directory.CreateDirectory(path);
        foreach (var str in scriptText)
        {
            File.AppendAllText(path + "/" + scriptName + ".cs", str + "\n");
        }
        AssetDatabase.Refresh();
    }
}
#endif