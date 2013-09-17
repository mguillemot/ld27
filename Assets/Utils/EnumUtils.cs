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


public class Alternative : Attribute
{
    public Alternative(string alternative)
    {
        this.alternative = alternative;
    }

    public readonly string alternative;
}

public static class EnumUtils
{

    public static T TryParse<T>(string repr, T defaultValue, bool logWarning = true) where T : struct, IConvertible
    {
        try
        {
            return (T) Enum.Parse(typeof (T), repr);
        }
        catch (Exception)
        {
            // Try alternative representations
            var typeInfo = typeof(T);
            foreach (var value in GetValues<T>())
            {
                var enumInfo = typeInfo.GetMember(value.ToString())[0];
                var alternatives = (Alternative[]) enumInfo.GetCustomAttributes(typeof(Alternative), false);
                for (int i = 0, n = alternatives.Length; i < n; i++)
                {
                    var alternative = alternatives[i];
                    if (alternative.alternative == repr)
                    {
                        return value;
                    }
                }
            }

            if (logWarning)
            {
                Debug.LogWarning(string.Format("Cannot parse \"{0}\" as a {1}", repr, typeof (T)));
            }
            return defaultValue;
        }
    }

    public static T[] GetValues<T>() where T : struct
    {
        return (T[]) Enum.GetValues(typeof (T));
    }

}
