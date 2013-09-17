/**
 * The Legend of Epikouros
 *   ~ a game made in 48h by Erhune for Ludum Dare 27
 *   
 * Copyright (c) 2013 Erhune <erhune@gmail.com>
 * 
 * Feel free to contact me any time!
 */
using UnityEngine;


public class Layers
{

    // Layers
    public static int player = LayerMask.NameToLayer("Player");
    public static int obstacles = LayerMask.NameToLayer("Obstacles");
    public static int enemies = LayerMask.NameToLayer("Enemies");
    public static int pickups = LayerMask.NameToLayer("Pickups");
    public static int panels = LayerMask.NameToLayer("Panels");

    // Masks
    public static int obstaclesMask = (1 << obstacles);
    public static int enemiesMask = (1 << enemies);
    public static int pickupsMask = (1 << pickups);
    public static int panelsMask = (1 << panels);
    public static int bombableMask = (1 << player | 1 << enemies | 1 << obstacles);

}
