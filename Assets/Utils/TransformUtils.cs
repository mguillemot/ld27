/**
 * The Legend of Epikouros
 *   ~ a game made in 48h by Erhune for Ludum Dare 27
 *   
 * Copyright (c) 2013 Erhune <erhune@gmail.com>
 * 
 * Feel free to contact me any time!
 */
using UnityEngine;


public static class TransformUtils 
{

	public static void SetPosition(this Transform t, float x, float y)
	{
	    t.localPosition = new Vector3(x, y, t.localPosition.z);
	}

	public static void SetRotation(this Transform t, float angle)
	{
	    t.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	public static void DestroyAllChildren(this Transform t)
	{
	    for (int i = t.childCount - 1; i >= 0; i--)
	    {
	        Object.Destroy(t.GetChild(i).gameObject);
	    }
	}

	public static void DestroyImmediateAllChildren(this Transform t)
	{
	    for (int i = t.childCount - 1; i >= 0; i--)
	    {
	        Object.DestroyImmediate(t.GetChild(i).gameObject);
	    }
	}

}
