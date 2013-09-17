/**
 * The Legend of Epikouros
 *   ~ a game made in 48h by Erhune for Ludum Dare 27
 *   
 * Copyright (c) 2013 Erhune <erhune@gmail.com>
 * 
 * Feel free to contact me any time!
 */
public class Unlockable : GameComponent
{

    public int uid { get; private set; }
    public bool locked { get; private set; }

    void Awake()
    {
        uid = GameManager.nextUid++;
        locked = true;
    }

    public void Unlock()
    {
        locked = false;
        Destroy(gameObject);
    }

}
