#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace MVC.View
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public class FieldCollection : MonoBehaviour
    {
        [Serializable]
        public class Field
        {
            public string name;
            public string typeName;
            public Object target;
            private Type type;
            public Type Type
            {
                get
                {
                    if (type == null)
                        type = Net.Serialize.NetConvertOld.GetType(typeName);
                    return type;
                }
                internal set { type = value; }
            }
            public T To<T>() where T : Object
            {
                return target as T;
            }
        }
        public string fieldName;
        public List<Field> fields = new List<Field>();
        private readonly Dictionary<string, Object> fieldsDic = new Dictionary<string, Object>();
#if UNITY_EDITOR
        public int nameIndex;
#endif
        private bool init;
        public Field this[int index]
        {
            get { return fields[index]; }
            set
            {
                fields[index].target = value.target;
                fields[index].typeName = value.typeName;
                fields[index].name = value.name;
            }
        }

        public Field this[string name]
        {
            get
            {
                foreach (Field f in fields)
                {
                    if (f.name == name)
                    {
                        return f;
                    }
                }
                return null;
            }
            set
            {
                foreach (Field f in fields)
                {
                    if (f.name == name)
                    {
                        f.name = value.name;
                        f.typeName = value.typeName;
                        f.Type = value.Type;
                        f.target = value.target;
                        return;
                    }
                }
            }
        }

        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            if (init)
                return;
            init = true;
            for (int i = 0; i < fields.Count; i++)
            {
                if (string.IsNullOrEmpty(fields[i].name))
                    fieldsDic.Add(i.ToString(), fields[i].target);
                else
                    fieldsDic.Add(fields[i].name, fields[i].target);
            }
        }

        public T GetField<T>(string name) where T : Object
        {
            return fieldsDic[name] as T;
        }

        public T GetField<T>(int index) where T : Object
        {
            return fields[index].target as T;
        }

        public T Get<T>(string name) where T : Object
        {
            return fieldsDic[name] as T;
        }

        public T Get<T>(int index) where T : Object
        {
            return fields[index].target as T;
        }
    }
}
#endif