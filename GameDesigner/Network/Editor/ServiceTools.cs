#if UNITY_EDITOR
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class ServiceTools : EditorWindow
{
    [MenuItem("GameDesigner/Network/OpenService")]
    static void Init() 
    {
        var ser = GetWindow<ServiceTools>();
        ser.maxSize = new Vector2(250,20);
        ser.Show();
    }
    public enum Protocol 
    {
        tcp,udp
    }
    Protocol protocol = Protocol.udp;
    private void OnGUI()
    {
        protocol = (Protocol)EditorGUILayout.EnumPopup("选择协议", protocol) ;
        if (GUILayout.Button("启动服务器", GUILayout.Height(30)))
        {
            var exe = Application.dataPath + "/GameDesigner/Example/Example1~/ConsoleApp1.exe";
            Process p = new Process();
            p.StartInfo.FileName = exe;
            p.StartInfo.Arguments = protocol.ToString();
            p.Start();
        }
    }
}
#endif