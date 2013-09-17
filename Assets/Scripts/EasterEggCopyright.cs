/**
 * The Legend of Epikouros
 *   ~ a game made in 48h by Erhune for Ludum Dare 27
 *   
 * Copyright (c) 2013 Erhune <erhune@gmail.com>
 * 
 * Feel free to contact me any time!
 */
using UnityEngine;


[RequireComponent(typeof(UILabel))]
public class EasterEggCopyright : GameComponent
{

    void Start()
    {
        if (Random.value < .1f)
        {
            GetComponent<UILabel>().text = "a game created in 48h by Erhune and lots of coffee";
        }
    }

}
