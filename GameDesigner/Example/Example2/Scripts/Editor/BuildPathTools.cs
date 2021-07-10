using Example2;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildPathTools
{
    [MenuItem("GameDesigner/Example/Example2/BuildSceneData")]
    static void Init()
    {
        var path = Application.dataPath + "/GameDesigner/Example/ExampleServer~/bin/Debug/Data/SceneData.json";
        var roamingPaths = Object.FindObjectsOfType<RoamingPath>();
        SceneData sceneData = new SceneData(); 
        foreach (var item in roamingPaths)
        {
            var monsterPoint = item.GetComponent<MonsterPoint>();
            sceneData.monsterPoints.Add(new MonsterPoint1() {
                roamingPath = new RoamingPath1() { waypointsList = item.waypointsList.ConvertAll(x => (Net.Vector3)x) },
                monsterIDs = monsterPoint.monsterIds,
            });
        }
        var jsonStr = Newtonsoft_X.Json.JsonConvert.SerializeObject(sceneData);
        File.WriteAllText(path, jsonStr);
        Debug.Log($"场景数据生成成功!--{path}");
    }
}
