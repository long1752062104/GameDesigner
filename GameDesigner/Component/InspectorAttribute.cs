#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
using UnityEngine;

/// <summary>
/// 只显示不能修改的属性
/// </summary>
public class DisplayOnly : PropertyAttribute { }

///<summary>
///定义多选属性
///</summary>
public class EnumFlags : PropertyAttribute { }

#endif