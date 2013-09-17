/**
 * The Legend of Epikouros
 *   ~ a game made in 48h by Erhune for Ludum Dare 27
 *   
 * Copyright (c) 2013 Erhune <erhune@gmail.com>
 * 
 * Feel free to contact me any time!
 */
using UnityEngine;


public static class ColorUtils
{

    public static readonly Color TransparentWhite = new Color(1, 1, 1, 0);
    public static readonly Color TransparentBlack = new Color(0, 0, 0, 0);

    public static Color ChangeAlpha(this Color c, float a)
    {
        return new Color(c.r, c.g, c.b, a);
    }

    public static Color Transparent(this Color c)
    {
        return c.ChangeAlpha(0f);
    }

    public static Color Opaque(this Color c)
    {
        return c.ChangeAlpha(1f);
    }

    public static Color FromHex(uint hexColor)
    {
        uint r = (hexColor >> 16) & 0xff;
        uint g = (hexColor >> 8) & 0xff;
        uint b = hexColor & 0xff;
        return new Color(r / 255f, g / 255f, b / 255f);
    }

    public static Color FromBytes(uint r, uint g, uint b)
    {
        return new Color(r / 255f, g / 255f, b / 255f);
    }

    public static string ToHex(this Color c)
    {
        return string.Format("{0:x2}{1:x2}{2:x2}", Mathf.FloorToInt(c.r * 0xff), Mathf.FloorToInt(c.g * 0xff), Mathf.FloorToInt(c.b * 0xff));
    }

    public static string ToNGUI(this Color c)
    {
        return string.Format("[{0}]", ToHex(c));
    }

}
