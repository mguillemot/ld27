/**
 * The Legend of Epikouros
 *   ~ a game made in 48h by Erhune for Ludum Dare 27
 *   
 * Copyright (c) 2013 Erhune <erhune@gmail.com>
 * 
 * Feel free to contact me any time!
 */
using UnityEngine;



public static class VectorUtils
{

    public static Vector2 CreatePolar2(float distance, float angle)
    {
        angle *= Mathf.Deg2Rad;
        return new Vector2(distance * Mathf.Cos(angle), distance * Mathf.Sin(angle));
    }

    public static Vector3 CreatePolar3(float distance, float angle)
    {
        return CreatePolar2(distance, angle);
    }

    public static Vector2 CreateRandomUnit2()
    {
        return CreateRandomUnit();
    }

    public static Vector3 CreateRandomUnit()
    {
        return CreatePolar3(1f, MathUtils.RandomAngle());
    }

    public static Vector3 CreateRandomInside(float distanceMax)
    {
        return CreateRandomInside(0f, distanceMax);
    }

    public static Vector3 CreateRandomInside(float distanceMin, float distanceMax)
    {
        return CreatePolar3(Random.Range(distanceMin, distanceMax), MathUtils.RandomAngle());
    }

    public static Vector2 ToVector2(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    public static Vector3 ToVector3(this Vector2 v)
    {
        return new Vector3(v.x, v.y);
    }

    public static Vector3 ChangeX(this Vector3 v, float x)
    {
        return new Vector3(x, v.y, v.z);
    }

    public static Vector3 ChangeY(this Vector3 v, float y)
    {
        return new Vector3(v.x, y, v.z);
    }

    public static Vector3 ChangeZ(this Vector3 v, float z)
    {
        return new Vector3(v.x, v.y, z);
    }

    public static Vector3 AddX(this Vector3 v, float x)
    {
        return new Vector3(v.x + x, v.y, v.z);
    }

    public static Vector3 AddY(this Vector3 v, float y)
    {
        return new Vector3(v.x, v.y + y, v.z);
    }

    public static Vector3 AddZ(this Vector3 v, float z)
    {
        return new Vector3(v.x, v.y, v.z + z);
    }

    public static float Angle(this Vector2 v)
    {
        return MathUtils.Angle(v.x, v.y);
    }

    public static float Angle(this Vector3 v)
    {
        return MathUtils.Angle(v.x, v.y);
    }

    public static Vector2 RotateBy(this Vector2 v, float angle)
    {
        return CreatePolar2(v.magnitude, v.Angle() + angle);
    }

    public static Vector3 RotateBy(this Vector3 v, float angle)
    {
        return CreatePolar3(v.magnitude, v.Angle() + angle);
    }

    public static Vector2 Floor(this Vector2 v)
    {
        return new Vector2(Mathf.Floor(v.x), Mathf.Floor(v.y));
    }

    public static Vector3 Floor(this Vector3 v)
    {
        return new Vector3(Mathf.Floor(v.x), Mathf.Floor(v.y), Mathf.Floor(v.z));
    }

    public static Vector2 Round(this Vector2 v)
    {
        return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
    }

    public static Vector3 Round(this Vector3 v)
    {
        return new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
    }

}
