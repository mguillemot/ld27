/**
 * The Legend of Epikouros
 *   ~ a game made in 48h by Erhune for Ludum Dare 27
 *   
 * Copyright (c) 2013 Erhune <erhune@gmail.com>
 * 
 * Feel free to contact me any time!
 */



public class Bombable : GameComponent
{

    public int uid { get; private set; }

    void Awake()
    {
        uid = GameManager.nextUid++;
    }

    public void Die()
    {
        Destroy(gameObject);
    }

}
