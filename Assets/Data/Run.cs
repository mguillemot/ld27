/**
 * The Legend of Epikouros
 *   ~ a game made in 48h by Erhune for Ludum Dare 27
 *   
 * Copyright © 2013 Erhune <erhune@gmail.com>
 * 
 * Feel free to contact me any time!
 */
using System.Collections.Generic;


public class Run
{

    public int cycle;
    public string nickname;
    public string title; // facultative
    public readonly List<Action> actions = new List<Action>();

    public double startTime
    {
        get { return (cycle - 1) * GameManager.RunDuration; }
    }

    public override string ToString()
    {
        return string.Format("[Run #{0} player={1} actions={2}]", cycle, nickname, actions.Count);
    }

}
