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


[RequireComponent(typeof(tk2dSprite))]
public class Pickup : GameComponent
{

    public PickupType Type;

    public int uid { get; private set; }
    public bool pickedUp { get; private set; }

    void Awake()
    {
        uid = GameManager.nextUid++;
    }

    void Start()
    {
        var sprite = GetComponent<tk2dSprite>();
        switch (Type)
        {
            case PickupType.Bomb:
                sprite.SetSprite("Bomb");
                break;

            case PickupType.Key:
                sprite.SetSprite("Key");
                break;

            case PickupType.SmallHeart:
                sprite.SetSprite("Heart_Small");
                break;

            case PickupType.BigHeart:
                sprite.SetSprite("Heart");
                break;

            case PickupType.Checkpoint:
                if (Application.isEditor)
                {
                    sprite.SetSprite("Checkpoint");
                }
                else
                {
                    renderer.enabled = false;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Die()
    {
        pickedUp = true;
        Destroy(gameObject);
    }

}
