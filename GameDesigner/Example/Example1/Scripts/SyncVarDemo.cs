#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Example1
{
    using Net.Share;
    using Net.UnityComponent;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public struct SyncVarStructTest //同步结构体内没有类字段的, 这样就完美了
    {
        public int number;
        public Vector3 position;
        public Quaternion quaternion;
    }

    [System.Serializable]
    public struct SyncVarStructTest1 //如果结构体内有类字段, 则需要重写Equals判断, 如果不重写Equals,你改变classField里面的数值,将不会检测到同步
    {
        public int number;
        public Vector3 position;
        public Quaternion quaternion;
        public SyncVarClassTest classField;

        public override bool Equals(object obj)
        {
            var other = (SyncVarStructTest1)obj;
            if (number != other.number | !position.Equals(other.position) | !quaternion.Equals(other.quaternion))//position == other.position判断会出问题,想知道为什么,需要用反编译工具查看源码
                return false;
            if (classField != null & other.classField != null)
                return classField.Equals(other.classField);
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [System.Serializable]
    public class SyncVarClassTest //同步类变量时, 必须重写Equals判断, 因为默认只判断类引用地址是不是同一地址而已
    {
        public int number;
        public Vector3 position;
        public Quaternion quaternion;

        public override bool Equals(object obj)
        {
            var other = obj as SyncVarClassTest;
            if (other == null)
                return false;
            if (number == other.number & position.Equals(other.position) & quaternion.Equals(other.quaternion))//position == other.position判断会出问题,想知道为什么,需要用反编译工具查看源码
                return true;
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class SyncVarDemo : NetworkBehaviour
    {
        [Header("同步结构类型字段")]
        [SyncVar]
        public SyncVarStructTest structTest;
        [Header("同步结构类型带有类类型的字段")]
        [SyncVar]
        public SyncVarStructTest1 structTest1;
        [Header("同步类类型字段")]
        [SyncVar]
        public SyncVarClassTest classTest;
        [Header("同步结构类型数组")]
        [SyncVar]
        public SyncVarStructTest[] array;
        [Header("同步类类型数组列表")]
        [SyncVar]
        public List<string> list1;
        [Header("同步所有基于UnityEngine.Object类型的物体")]
        [SyncVar]
        public GameObject obj;//只在编辑器有效!
        [Header("p2p同步int")]
        [SyncVar(id = 1)]
        public int testint;
        [Header("p2p同步string")]
        [SyncVar(id = 2)]
        public string teststring;
    }
}
#endif