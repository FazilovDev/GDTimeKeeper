using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Extensions
{
    public static Transform[] Children(this Transform transform)
    {
        var res = new Transform[transform.childCount];
        for (int i = 0; i < res.Length; i++)
        {
            res[i] = transform.GetChild(i);
        }
        return res;
    }

    public static void Map<T>(this IEnumerable<T> collection, System.Action<T> action)
    {
        if (collection == null)
        {
            return;
        }

        foreach (var item in collection)
        {
            action(item);
        }
    }
}