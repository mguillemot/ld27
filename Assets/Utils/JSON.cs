/**
 * The Legend of Epikouros
 *   ~ a game made in 48h by Erhune for Ludum Dare 27
 *   
 * Copyright (c) 2013 Erhune <erhune@gmail.com>
 * 
 * Feel free to contact me any time!
 */
using System.Text;
using JsonFx.Json;


public static class JSON
{

    public static T Deserialize<T>(string repr) where T : class
    {
        if (string.IsNullOrEmpty(repr)) return null;
        
        return JsonReader.Deserialize<T>(repr);
    }

    public static string Serialize<T>(T value, bool pretty) where T : class
    {
        if (value == null) return null;

        var result = new StringBuilder();
        var writer = new JsonWriter(result, new JsonWriterSettings {PrettyPrint = pretty});
        writer.Write(value);
        return result.ToString();
    }

}
