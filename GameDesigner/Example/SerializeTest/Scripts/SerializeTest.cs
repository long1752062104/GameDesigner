using Net.Event;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SerializeTestExample
{
    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
    public class test
    {
        public string name;
        public Net.Vector3 vector3;
        public List<int> list;
        public long[] array;
        public test1[] test1s;
        public List<test1> test1s1;
        public override string ToString()
        {
            string str = $"name:{name} vector3:{vector3} list:";
            foreach (var item in list)
            {
                str += item + ",";
            }
            str = str.TrimEnd(',');
            str += "array:";
            foreach (var item in array)
            {
                str += item + ",";
            }
            str = str.TrimEnd(',');
            str += "test1s:";
            foreach (var item in test1s)
            {
                str += item + ",";
            }
            str = str.TrimEnd(',');
            str += "test1s1:";
            foreach (var item in test1s1)
            {
                str += item + ",";
            }
            str = str.TrimEnd(',');
            return str;
        }
    }

    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
    public class test1 
    {
        public string str;
        public override string ToString()
        {
            return str;
        }
    }

    public static class BindingEntry
    {
        public static Type[] GetBindTypes()
        {
            List<Type> types = new List<Type>();
            types.Add(typeof(test1));
            types.Add(typeof(test));
            return types.ToArray();
        }
    }

    public class SerializeTest : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            NDebug.BindLogAll(Debug.Log);
            Fast2BuildToolMethod.DynamicBuild(BindingEntry.GetBindTypes());

            Stopwatch stopwatch = Stopwatch.StartNew();
            stopwatch.Start();
            for (int i = 0; i < 100000; i++)
            {
                var seg = Net.Share.NetConvertFast2.SerializeObject(new test()
                {
                    name = "dsad",
                    vector3 = new Net.Vector3(5, 6, 7),
                    array = new long[100],
                    list = new List<int>() { 1,2,3 },
                    test1s = new test1[] { new test1() { str = "dsad" } },
                    test1s1 = new List<test1>() { new test1() { str = "2222qwewqe" } }
                });
                var obj = Net.Share.NetConvertFast2.DeserializeObject<test>(seg);
            }
            Debug.Log("极速序列化:" + stopwatch.ElapsedMilliseconds);
            stopwatch.Restart();
            using (var stream = new System.IO.MemoryStream(1024))
            {
                for (int i = 0; i < 100000; i++)
                {
                    stream.SetLength(0);
                    ProtoBuf.Serializer.Serialize(stream, new test()
                    {
                        name = "dsad",
                        vector3 = new Net.Vector3(5, 6, 7),
                        array = new long[100],
                        list = new List<int>() { 1, 2, 3 },
                        test1s = new test1[] { new test1() { str = "dsad" } },
                        test1s1 = new List<test1>() { new test1() { str = "2222qwewqe" } }
                    });
                    stream.Position = 0;
                    var obj = ProtoBuf.Serializer.Deserialize<test>(stream);
                }
            }
            Debug.Log("protobuff序列化:" + stopwatch.ElapsedMilliseconds);
        }
    }
}