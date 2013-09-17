/**
 * The Legend of Epikouros
 *   ~ a game made in 48h by Erhune for Ludum Dare 27
 *   
 * Copyright (c) 2013 Erhune <erhune@gmail.com>
 * 
 * Feel free to contact me any time!
 */



public enum PlayState
{
    None,
    Replaying,
    WaitingForInput,
    RequestToStart,
    StartCountdown,
    Playing,
    PostingRun,
    Ended,
    PollingForMore,
}
