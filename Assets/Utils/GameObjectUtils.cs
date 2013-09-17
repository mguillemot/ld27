/**
 * The Legend of Epikouros
 *   ~ a game made in 48h by Erhune for Ludum Dare 27
 *   
 * Copyright (c) 2013 Erhune <erhune@gmail.com>
 * 
 * Feel free to contact me any time!
 */
using System.Collections.Generic;
using UnityEngine;


public static class GameObjectUtils 
{

    public static T FindInParents<T>(this GameObject gameObject) where T : class 
    {
        if (gameObject == null) return null;

        object comp = gameObject.GetComponent(typeof(T));
        if (comp == null)
        {
            var t = gameObject.transform.parent;
            while (t != null && comp == null)
            {
                comp = t.gameObject.GetComponent(typeof(T));
                t = t.parent;
            }
        }
        return (T)comp;
    }

    public static string Hierarchy(this GameObject gameObject)
    {
        if (!gameObject) return "";
        var result = new List<string>();
        var t = gameObject.transform;
        while (t != null)
        {
            result.Add(t.name);
            t = t.parent;
        }
        result.Reverse();
        return string.Join(" > ", result.ToArray());
    }

}
