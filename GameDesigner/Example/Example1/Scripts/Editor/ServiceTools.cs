#if UNITY_EDITOR
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class ServiceTools
{
    [MenuItem("GameDesigner/Example/OpenExample1_Service")]
    static void Init() 
    {
        var exe = Application.dataPath + "/GameDesigner/Example/Example1/ExampleServer~/bin/Debug/Example1.exe";
        Process p = new Process();
        p.StartInfo.FileName = exe;
        p.Start();
    }
    [MenuItem("GameDesigner/Example/OpenExample2_Service")]
    static void Init1()
    {
        var exe = Application.dataPath + "/GameDesigner/Example/Example2/Server~/bin/Debug/Server.exe";
        Process p = new Process();
        p.StartInfo.FileName = exe;
        p.Start();
    }
}
#endif