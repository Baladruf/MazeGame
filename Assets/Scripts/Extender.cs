using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extender
{
    public static T GetRandomElement<T>(this IList<T> _list)
    {
        Debug.Assert(_list.Count > 0);
        return _list[Random.Range(0, _list.Count)];
    }

    public static Direction Invert(this Direction _dir)
    {
        if( ((int)_dir) % 2 == 0)
        {
            return _dir + 1;
        }
        return _dir - 1;
    }
}
