#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
using System;
using UnityEngine;
using Net.Component;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Net.UnityComponent
{
    [Serializable]
    public class ObjectRecord 
    {
        public int ID;
        public Object obj;
        public string path;//编辑器模式使用,任何物体都可以同步
    }

    /// <summary>
    /// 网络资源同步
    /// 此类主要用于同步字段为unity的组件或者基于UnityEngine.Objec的类型
    /// 在编辑器模式下是任何物体都可以同步的, 注意: 在编译项目后, 只能同步在Resources文件夹下的预制体或物体
    /// </summary>
    public class NetworkResources : SingleCase<NetworkResources>
    {
        public ObjectRecord[] objectRecords;
        public Dictionary<Object, ObjectRecord> dic = new Dictionary<Object, ObjectRecord>();
        public Dictionary<string, ObjectRecord> dic1 = new Dictionary<string, ObjectRecord>();

        void Awake()
        {
            var objects = Resources.LoadAll<Object>("");
            objectRecords = new ObjectRecord[objects.Length];
            for (int i = 0; i < objects.Length; i++)
            {
                objectRecords[i] = new ObjectRecord() { ID = i, obj = objects[i] };
                dic.Add(objects[i], objectRecords[i]);
            }
        }

        internal bool TryGetValue(Object obj, out ObjectRecord objectRecord)
        {
#if UNITY_EDITOR
            if (!dic.TryGetValue(obj, out objectRecord)) 
            {
                var path = UnityEditor.AssetDatabase.GetAssetPath(obj);
                objectRecord = new ObjectRecord() { ID = dic.Count, obj = obj, path = path };
                dic.Add(obj, objectRecord);
            }
            return true;
#else
            return dic.TryGetValue(obj, out objectRecord);
#endif
        }

        internal T GetObject<T>(int index, string path) where T : Object
        {
#if UNITY_EDITOR
            if(string.IsNullOrEmpty(path))
                return null;
            if (!dic1.TryGetValue(path, out ObjectRecord objectRecord))
            {
                var obj = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(T));
                objectRecord = new ObjectRecord() { ID = dic1.Count, obj = obj, path = path };
                dic1.Add(path, objectRecord);
            }
            return (T)objectRecord.obj;
#else
            return (T)objectRecords[index].obj;
#endif

        }
    }
}
#endif