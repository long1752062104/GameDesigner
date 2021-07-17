using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

public static class ArrayExtend
{
    #region 数组扩展
    public static void For<T>(this T[] self, Action<T> action)
    {
        for (int i = 0; i < self.Length; i++)
        {
            action(self[i]);
        }
    }

    public static void For<T>(this T[] self, Action<int> action)
    {
        for (int i = 0; i < self.Length; i++)
        {
            action(i);
        }
    }

    public static void For<T>(this T[] self, int index, int count, Action<T> action)
    {
        for (int i = index; i < count; i++)
        {
            action(self[i]);
        }
    }

    public static void For<T>(this T[] self, int index, Action<T> action)
    {
        for (int i = index; i < self.Length; i++)
        {
            action(self[i]);
        }
    }

    public static void For<T>(this T[] self, Action<int, T> action)
    {
        for (int i = 0; i < self.Length; i++)
        {
            action(i, self[i]);
        }
    }

    /// <summary>
    /// 随机一个值,在数组0-count范围内
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    /// <returns></returns>
    public static T Random<T>(this T[] self)
    {
        return self[Net.Share.RandomHelper.Range(0, self.Length)];
    }

    public static void ClearObjects<T>(this T[] self) where T : Object
    {
        for (int i = 0; i < self.Length; i++)
        {
            if (self[i] != null)
                Object.Destroy(self[i]);
        }
    }

    public static void ClearObjects<T>(this List<T> self) where T : Component
    {
        for (int i = 0; i < self.Count; i++)
        {
            if (self[i] != null)
                Object.Destroy(self[i].gameObject);
        }
        self.Clear();
    }
    #endregion

    public static void For<T>(this HashSet<T> self, Action<T> action)
    {
        foreach (T t in self)
        {
            action(t);
        }
    }

    public static T[] ToArray<T>(this HashSet<T> self)
    {
        T[] ts = new T[self.Count];
        int i = 0;
        foreach (T t in self)
        {
            ts[i] = t;
            i++;
        }
        return ts;
    }

    /// <summary>
    /// 反序列化数据(使用ProtoBuf反序列化)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    /// <returns></returns>
    public static T Deserialize<T>(this byte[] self, int index, int count)
    {
        using (MemoryStream stream = new MemoryStream(self, index, count))
        {
            return ProtoBuf.Serializer.Deserialize<T>(stream);
        }
    }
}