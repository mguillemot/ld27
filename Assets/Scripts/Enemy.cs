/**
 * The Legend of Epikouros
 *   ~ a game made in 48h by Erhune for Ludum Dare 27
 *   
 * Copyright (c) 2013 Erhune <erhune@gmail.com>
 * 
 * Feel free to contact me any time!
 */
using System;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : GameComponent
{

    public int Life = 1;
    public EnemyType Type;
    public float Speed;
    public float SpiderRadius;
    public GameObject DropPrefab;
    public List<Vector2> Patrol;
    public float PatrolDelay;
    public float PatrolPause;

    public int uid { get; private set; }
    public bool dead { get; private set; }

    private tk2dSprite sprite;
    private tk2dSpriteAnimator animator;
    private readonly List<Vector3> patrolPoints = new List<Vector3>();
    private float patrolLength;

    void Awake()
    {
        uid = GameManager.nextUid++;
        sprite = GetComponent<tk2dSprite>();
        animator = GetComponent<tk2dSpriteAnimator>();
    }

    void Update()
    {
        if (!gameManager.gameIsRunning)
        {
            animator.Pause();
        }
        else
        {
            animator.Resume();
        }

        if (Type == EnemyType.Patroller)
        {
            if (Life == 1)
            {
                sprite.color = ColorUtils.FromBytes(67, 60, 255);
            }
            else if (Life == 2)
            {
                sprite.color = ColorUtils.FromBytes(234, 43, 43);
            }
            else
            {
                sprite.color = ColorUtils.FromBytes(230, 255, 0);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        Life--;
    }

    public void Die()
    {
        dead = true;

        switch (Type)
        {
            case EnemyType.Patroller:
                if (DropPrefab != null)
                {
                    InstantiatePrefabAt(DropPrefab, transform.position);
                }
                break;

            case EnemyType.Spider:
                var byKill = new[]
                                 {
                                     PickupType.SmallHeart, // 1st kill
                                     PickupType.None,
                                     PickupType.Bomb,
                                     PickupType.SmallHeart,
                                     PickupType.Key,
                                     PickupType.Bomb,
                                     PickupType.SmallHeart,
                                     PickupType.Bomb,
                                     PickupType.None,
                                     PickupType.SmallHeart,
                                     PickupType.Bomb, // 10th kill
                                     PickupType.None,
                                     PickupType.BigHeart,
                                 };
                var drop = (gameManager.hero.SpiderKills < byKill.Length) ? byKill[gameManager.hero.SpiderKills] : PickupType.None;
                if (drop != PickupType.None && DropPrefab != null)
                {
                    var dropObject = InstantiatePrefabAt(DropPrefab, transform.position);
                    dropObject.GetComponent<Pickup>().Type = drop;
                }
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        Destroy(gameObject);
    }

    void Start()
    {
        if (Type == EnemyType.Spider)
        {
            Patrol.Clear();
            var r = new System.Random(uid);
            for (int i = 0; i < 25; i++)
            {
                var x = ((float) r.NextDouble() * 2f - 1f) * SpiderRadius;
                var y = ((float) r.NextDouble() * 2f - 1f) * SpiderRadius;
                Patrol.Add(new Vector2(x, y));
            }
        }

        foreach (var patrol in Patrol)
        {
            patrolPoints.Add(transform.position + patrol.ToVector3());
        }
        for (int i = 0, n = patrolPoints.Count; i < n; i++)
        {
            var p1 = patrolPoints[i];
            var p2 = patrolPoints[(i + 1) % n];
            patrolLength += (p2 - p1).magnitude + PatrolPause * Speed;
        }
    }
    
    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            for (int i = 0, n = patrolPoints.Count; i < n; i++)
            {
                var p1 = patrolPoints[i];
                var p2 = patrolPoints[(i + 1) % n];
                Gizmos.color = Color.red;
                Gizmos.DrawLine(p1, p2);
            }
        }
        else
        {
            for (int i = 0, n = Patrol.Count; i < n; i++)
            {
                var p1 = Patrol[i];
                var p2 = Patrol[(i + 1) % n];
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position + p1.ToVector3(), transform.position + p2.ToVector3());
            }
        }
    }

    public Vector3 GetPositionAt(double gameTime)
    {
        if (patrolLength == 0) return transform.position;

        if (gameTime <= PatrolDelay) return patrolPoints[0];

        var totalLen = Speed * (gameTime - PatrolDelay);
        var remainingDistance = (float)(totalLen % patrolLength);

        for (int i = 0, n = patrolPoints.Count; i < n; i++)
        {
            var p1 = patrolPoints[i];
            var p2 = patrolPoints[(i + 1) % n];
            var path = p2 - p1;
            //LogDebug("p1={0} p2={1} path={2} rem={3}", p1, p2, path.magnitude, remainingDistance);
            if (remainingDistance <= path.magnitude)
            {
                return p1 + path.normalized * remainingDistance;
            }
            remainingDistance -= path.magnitude + PatrolPause * Speed;
            if (remainingDistance < 0)
            {
                return p2;
            }
        }
        throw new Exception("invalid path time " + gameTime + " total=" + totalLen + " rem=" + remainingDistance);
    }

    public float GetRotationAt(double gameTime)
    {
        if (patrolLength == 0) return 0;

        if (gameTime <= PatrolDelay) return 0;

        var totalLen = Speed * (gameTime - PatrolDelay);
        var remainingDistance = (float)(totalLen % patrolLength);

        for (int i = 0, n = patrolPoints.Count; i < n; i++)
        {
            var p1 = patrolPoints[i];
            var p2 = patrolPoints[(i + 1) % n];
            var path = p2 - p1;
            //LogDebug("p1={0} p2={1} path={2} rem={3}", p1, p2, path.magnitude, remainingDistance);
            if (remainingDistance <= path.magnitude)
            {
                return path.Angle();
            }
            remainingDistance -= path.magnitude + PatrolPause * Speed;
            if (remainingDistance < 0)
            {
                return path.Angle();
            }
        }
        throw new Exception("invalid path time " + gameTime + " total=" + totalLen + " rem=" + remainingDistance);
    }

    public void Advance()
    {
        transform.position = GetPositionAt(gameManager.currentTime);
        transform.SetRotation(GetRotationAt(gameManager.currentTime));
    }

}
