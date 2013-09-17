/**
 * The Legend of Epikouros
 *   ~ a game made in 48h by Erhune for Ludum Dare 27
 *   
 * Copyright (c) 2013 Erhune <erhune@gmail.com>
 * 
 * Feel free to contact me any time!
 */
using UnityEngine;



public class CameraFollow : GameComponent
{

    public float GrainStartAtY;
    public float GrainMaxAtY;

    private NoiseEffect noise;
    private float minGrainIntensity;
    private float maxGrainIntensity;

    void Awake()
    {
        noise = GetComponent<NoiseEffect>();
        minGrainIntensity = noise.grainIntensityMin;
        maxGrainIntensity = noise.grainIntensityMax;
        noise.enabled = false;
    }

    void LateUpdate()
    {
        transform.position = gameManager.hero.transform.position.ChangeZ(transform.position.z);

        if (transform.position.y > GrainStartAtY)
        {
            var t = Mathf.InverseLerp(GrainStartAtY, GrainMaxAtY, transform.position.y);
            noise.enabled = true;
            noise.grainIntensityMin = minGrainIntensity * t;
            noise.grainIntensityMax = maxGrainIntensity * t;
        }
        else
        {
            noise.enabled = false;
        }
    }

}
