/**
 * The Legend of Epikouros
 *   ~ a game made in 48h by Erhune for Ludum Dare 27
 *   
 * Copyright (c) 2013 Erhune <erhune@gmail.com>
 * 
 * Feel free to contact me any time!
 */



public delegate void ActionProcessor(Action action);

public class Action
{

    // Context
    public double gameTime;
    public float atX;
    public float atY;

    // Action
    public ActionType type;
    public int uid;
    public int x;
    public int y;

    public override string ToString()
    {
        return string.Format("[Action @{0:0.000}:{1:0.000}T{2:0.000} {3} pos={4}:{5} uid={6}]", gameTime, atX, atY, type, x, y, uid);
    }

}
