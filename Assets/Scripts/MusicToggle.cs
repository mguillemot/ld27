/**
 * The Legend of Epikouros
 *   ~ a game made in 48h by Erhune for Ludum Dare 27
 *   
 * Copyright (c) 2013 Erhune <erhune@gmail.com>
 * 
 * Feel free to contact me any time!
 */
using UnityEngine;


[RequireComponent(typeof(UISprite))]
public class MusicToggle : GameComponent
{

    void OnClick()
    {
        gameManager.ambiantEnabled = !gameManager.ambiantEnabled;
        GetComponent<UISprite>().spriteName = "Speaker_" + ((gameManager.ambiantEnabled) ? "On" : "Off");
    }

}
