using Net;
using System;
using System.Collections.Generic;
using System.IO;

namespace Example2
{
    [Serializable]
    public class RoamingPath1
    {
        public List<Vector3> waypointsList = new List<Vector3>();
    }

    [Serializable]
    public class MonsterPoint1
    {
        public int[] monsterIDs;
        public RoamingPath1 roamingPath;
    }

    [Serializable]
    public class SceneData
    {
        private static SceneData sceneData;
        public static SceneData I => sceneData;
        public List<MonsterPoint1> monsterPoints = new List<MonsterPoint1>();

        public static void ReadData()
        {
#if !UNITY_EDITOR
            var path = Directory.GetCurrentDirectory() + "/Data/SceneData.json";
#else
            var path = UnityEngine.Application.dataPath + "/GameDesigner/Example/ExampleServer~/bin/Debug/Data/SceneData.json";
#endif
            var jsonStr = File.ReadAllText(path);
            sceneData = Newtonsoft_X.Json.JsonConvert.DeserializeObject<SceneData>(jsonStr);
        }
    }
}