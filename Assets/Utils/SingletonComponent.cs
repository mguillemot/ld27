/**
 * The Legend of Epikouros
 *   ~ a game made in 48h by Erhune for Ludum Dare 27
 *   
 * Copyright (c) 2013 Erhune <erhune@gmail.com>
 * 
 * Feel free to contact me any time!
 */
using UnityEngine;



public abstract class SingletonComponent<T> : GameComponent where T : Object
{

    private static T _instance;
    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                //Debug.Log("Instance of " + typeof(T) + " is " + _instance);
            }
            return _instance;
        }
    }

}
