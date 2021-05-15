#if UNITY_EDITOR
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class HelpEdit
{
    [MenuItem("GameDesigner/Help")]
    static void Init()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath);
        var files = dir.GetFiles("GameDesignerHelp.chm", SearchOption.AllDirectories);
        if (files.Length == 0)
            return;
        Process pro = new Process();
        ProcessStartInfo proinfo = new ProcessStartInfo(files[0].FullName);
        pro.StartInfo = proinfo;
        pro.Start();
    }
}
#endif