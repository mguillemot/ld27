/**
 * The Legend of Epikouros
 *   ~ a game made in 48h by Erhune for Ludum Dare 27
 *   
 * Copyright (c) 2013 Erhune <erhune@gmail.com>
 * 
 * Feel free to contact me any time!
 */
using System;
using UnityEngine;
using Random = UnityEngine.Random;



public static class MathUtils 
{
	
    /// <summary>
    /// Clamps an angle to the ]-180.0;180.0] degree range.
    /// </summary>
    /// <param name="x">Angle in degree</param>
    /// <returns>Same angle, normalized</returns>
	public static float RectifyAngle(float x)
	{
		x += 180f;
		return (x - 360f * Mathf.Floor(x / 360f)) - 180f;
	}

    /// <summary>
    /// Returns a random angle (in degree).
    /// </summary>
    /// <returns>A random angle between -180 and 180 degree.</returns>
    public static float RandomAngle()
    {
        return Random.Range(-180f, 180f);
    }

    /// <summary>
    /// Returns a random short in a given range (both ends are included in the range).
    /// </summary>
    /// <returns>A random short in the [min;max] range.</returns>
    public static short RandomInRange(short min, short max)
    {
        var r = Random.Range(min, max + 1);
        return (short) r;
    }

    /// <summary>
    /// Returns a random int in a given range (both ends are included in the range).
    /// </summary>
    /// <returns>A random short in the [min;max] range.</returns>
    public static int RandomInRange(int min, int max)
    {
        return Random.Range(min, max + 1);
    }

    /// <summary>
    /// Get the polar coordinates angle (in degree) of a 2D vector.
    /// </summary>
    /// <param name="vec">Input vector</param>
    /// <returns>Angle in degree from the X axis to the vector</returns>
    public static float Angle(Vector2 vec)
    {
        return Angle(vec.x, vec.y);
    }

    /// <summary>
    /// Get the polar coordinates angle (in degree) of a 2D vector.
    /// </summary>
    /// <param name="x">The x coordinate of the vector</param>
    /// <param name="y">The y coordinate of the vector</param>
    /// <returns>Angle (in degree) from the X axis to the vector</returns>
    public static float Angle(float x, float y)
    {
        return Mathf.Atan2(y, x) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// InverseLerp for doubles
    /// </summary>
    public static double InverseLerp(double from, double to, double value)
    {
        return (value - from) / (to - from);
    }

    /// <summary>
    /// Tests the (approximate) equality of two doubles.
    /// </summary>
    public static bool Approximately(double a, double b)
    {
        return Math.Abs(b - a) < 0.000001;
    }

    /// <summary>
    /// Encode an angle into all bits of a 16-bit short.
    /// </summary>
    public static short EncodeAngle(float angle)
    {
        angle = RectifyAngle(angle) * short.MaxValue / 180f;
        return (short) angle;
    }

    /// <summary>
    /// Decode an angle from all bits of a 16-bit short.
    /// </summary>
    public static float DecodeAngle(short encoded)
    {
        var angle = (float) encoded;
        return RectifyAngle(angle * 180f / short.MaxValue);
    }

    /// <summary>
    /// Apply a gaussian interpolation from A to B at time T.
    /// </summary>
    public static float Gaussian(float a, float b, float t)
    {
        var xx = 1 - 2 * t;
        var g = 1f - xx * xx * xx;
        return a + g * (b - a);
    }

    /// <summary>
    /// Apply a quadratic ease-in-and-out tween from A to B at time T.
    /// </summary>
    public static float EaseInOutQuad(float a, float b, float t)
    {
        // Note: see NgInterpolate
        t /= .5f;
        b -= a;
        if (t < 1) return b / 2 * t * t + a;
        t--;
        return -b / 2 * (t * (t - 2) - 1) + a;
    }

    /// <summary>
    /// Apply a quadratic ease-out tween from A to B at time T over a duration.
    /// </summary>
    public static float EaseOutQuad(float a, float b, float t, float duration)
    {
        // clamp elapsedTime so that it cannot be greater than duration
        t = (t > duration) ? 1.0f : t / duration;
        return -b * t * (t - 2) + a;
    }

}
