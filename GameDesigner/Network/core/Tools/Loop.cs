using System;
using System.Collections.Generic;
public sealed class Loop
{
    public static void For(int count, Action<int> index)
    {
        for (int i = 0; i < count; i++)
        {
            index(i);
        }
    }

    public static void Foreach<T>(List<T> list, Action<T> obj)
    {
        for (int i = 0; i < list.Count; i++)
        {
            obj(list[i]);
        }
    }

    public static void Foreach<T>(T[] list, Action<T> obj)
    {
        for (int i = 0; i < list.Length; i++)
        {
            obj(list[i]);
        }
    }
}
