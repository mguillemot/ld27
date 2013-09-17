/**
 * The Legend of Epikouros
 *   ~ a game made in 48h by Erhune for Ludum Dare 27
 *   
 * Copyright © 2013 Erhune <erhune@gmail.com>
 * 
 * Feel free to contact me any time!
 */
using System.Collections.Generic;
using UnityEngine;



public class DebugUtils
{
    public static void DebugCollection<T>(IEnumerable<T> collection)
    {
        foreach (var element in collection)
        {
            Debug.Log(element);
        }
    }
}
