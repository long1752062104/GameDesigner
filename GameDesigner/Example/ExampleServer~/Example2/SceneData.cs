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
        public string name;
        public List<MonsterPoint1> monsterPoints = new List<MonsterPoint1>();

        public static SceneData ReadData(string path)
        {
            var jsonStr = File.ReadAllText(path);
            return Newtonsoft_X.Json.JsonConvert.DeserializeObject<SceneData>(jsonStr);
        }
    }
}