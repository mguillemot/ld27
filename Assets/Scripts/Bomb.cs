/**
 * The Legend of Epikouros
 *   ~ a game made in 48h by Erhune for Ludum Dare 27
 *   
 * Copyright (c) 2013 Erhune <erhune@gmail.com>
 * 
 * Feel free to contact me any time!
 */
using UnityEngine;


[RequireComponent(typeof(tk2dSpriteAnimator))]
public class Bomb : GameComponent
{

    public float ExplosionTime;
    public float ExplosionRadius;

    private tk2dSpriteAnimator animator;
    private double startTime;

    void Awake()
    {
        animator = GetComponent<tk2dSpriteAnimator>();

        startTime = gameManager.currentTime;
        Log("Bomb appears at {0} on {1:0.000}", transform.position, gameManager.currentTime);
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
    }

    public void Advance(ActionProcessor processor)
    {
        var anim = (float)(gameManager.currentTime - startTime) / ExplosionTime;
        if (anim < 1f)
        {
            transform.localScale = Vector3.one * (1f + .5f * anim);
        }
        else
        {
            AudioSource.PlayClipAtPoint(gameManager.hero.ExplodeSound, transform.position);
            Log("BOUM! startTime={0:0.000} current={1:0.000}", startTime, gameManager.currentTime);
            Destroy(gameObject);

            if (gameManager.state == PlayState.Playing) // not in replays
            {
                var hits = Physics.OverlapSphere(transform.position, ExplosionRadius, Layers.bombableMask);
                foreach (var hit in hits)
                {
                    var player = hit.gameObject.FindInParents<Hero>();
                    if (player != null)
                    {
                        processor(new Action
                                      {
                                          type = ActionType.TakeDamage
                                      });
                        processor(new Action
                                      {
                                          type = ActionType.TakeDamage
                                      });
                    }

                    var enemy = hit.gameObject.FindInParents<Enemy>();
                    if (enemy != null)
                    {
                        processor(new Action
                                      {
                                          type = ActionType.Kill,
                                          uid = enemy.uid
                                      });
                    }

                    var bombable = hit.gameObject.FindInParents<Bombable>();
                    if (bombable != null)
                    {
                        processor(new Action
                                      {
                                          type = ActionType.Break,
                                          uid = bombable.uid
                                      });
                    }
                }
            }
        }
    }
}
