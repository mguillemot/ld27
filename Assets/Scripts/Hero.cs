/**
 * The Legend of Epikouros
 *   ~ a game made in 48h by Erhune for Ludum Dare 27
 *   
 * Copyright (c) 2013 Erhune <erhune@gmail.com>
 * 
 * Feel free to contact me any time!
 */
using System;
using System.Collections;
using UnityEngine;


public class Hero : GameComponent
{

    public float CollisionRadius;
    public float EnemyCollisionRadius;
    public float InvincibilityAfterHit;
    public float DeathAnimation;
    public float WalkSpeed;
    public CapsuleCollider Sword;
    public float SwordCooldown;
    public tk2dSpriteAnimator Sprite;
    public GameObject BombPrefab;
    public AudioClip AttackSound;
    public AudioClip PickupBombSound;
    public AudioClip PickupHeartSound;
    public AudioClip PickupKeySound;
    public AudioClip EnemyHitSound;
    public AudioClip EnemyDieSound;
    public AudioClip SelfHitSound;
    public AudioClip SelfDieSound;
    public AudioClip UnlockSound;
    public AudioClip ExplodeSound;

    public int Bombs;
    public int Keys;
    public int Life;
    public int MaxLife;
    public string Panel;
    public int PatrollerKills;
    public int SpiderKills;

    public string nickname { get; set; }

    private bool attacking;
    private string currentAnim = "Hero_Front";

    public int currentMovementX { get; private set; }
    public int currentMovementY { get; private set; }
    public Vector3 facing { get; private set; }
    public bool cannotAct { get; private set; }
    public double cannotAttackBefore { get; private set; }
    public double invincibleUntil { get; private set; }
    public Vector3 checkpoint { get; private set; }

    void Start()
    {
        checkpoint = transform.position;
        Life = MaxLife = GameManager.InitialLives;
    }

    public void StopMovement()
    {
        currentMovementX = currentMovementY = 0;
    }

    public bool CanAct()
    {
        return !cannotAct;
    }

    public bool CanAttack()
    {
        return CanAct() && (gameManager.currentTime >= cannotAttackBefore);
    }

    public bool CanPickup(PickupType type)
    {
        if (type == PickupType.SmallHeart && Life == MaxLife)
        {
            return false;
        }
        return true;
    }

    void Update()
    {
        // Animation
        if (gameManager.gameIsRunning)
        {
            var suffix = attacking && CanAct() ? "_Attack" : "";
            if (currentMovementX == 1 && currentMovementY == 0)
            {
                currentAnim = "Hero_Right";
                Sprite.Play(currentAnim + suffix);
            }
            else if (currentMovementX == -1 && currentMovementY == 0)
            {
                currentAnim = "Hero_Left";
                Sprite.Play(currentAnim + suffix);
            }
            else if (currentMovementX == 0 && currentMovementY == 1)
            {
                currentAnim = "Hero_Back";
                Sprite.Play(currentAnim + suffix);
            }
            else if (currentMovementX == 0 && currentMovementY == -1)
            {
                currentAnim = "Hero_Front";
                Sprite.Play(currentAnim + suffix);
            }
            else if (currentMovementX == 0 && currentMovementY == 0)
            {
                Sprite.Play(currentAnim + suffix);
                Sprite.StopAndResetFrame();
            }
        }
        else
        {
            Sprite.StopAndResetFrame();
        }
    }

    public void Advance(float deltaTime, ActionProcessor processor)
    {
        // Movement
        if (currentMovementX != 0 || currentMovementY != 0)
        {
            var moved = TryMovement(currentMovementX, currentMovementY, deltaTime, processor)
                        || TryMovement(currentMovementX, 0, deltaTime, processor)
                        || TryMovement(0, currentMovementY, deltaTime, processor);
            if (moved)
            {
                // Moved!
            }
        }

        // Check for pickups
        if (gameManager.state == PlayState.Playing) // not for replays
        {
            var pickups = Physics.OverlapSphere(transform.position, CollisionRadius, Layers.pickupsMask);
            foreach (var pickup in pickups)
            {
                var pickupScript = pickup.gameObject.FindInParents<Pickup>();
                if (pickupScript != null && CanPickup(pickupScript.Type))
                {
                    Log("Picked up {0} uid={1}", pickupScript.Type, pickupScript.uid);
                    processor(new Action
                                  {
                                      type = ActionType.Pickup,
                                      uid = pickupScript.uid
                                  });
                }
            }
        }

        // Check for panels
        Panel = null;
        var panels = Physics.SphereCastAll(transform.position.ChangeZ(-50), CollisionRadius, Vector3.forward, 100, Layers.panelsMask);
        foreach (var panel in panels)
        {
            var panelScript = panel.collider.gameObject.FindInParents<Panel>();
            if (panelScript != null)
            {
                //Log("Showing panel: {0}", panelScript.Message);
                Panel = panelScript.Message;
            }
        }

        // Check for damage
        if (gameManager.state == PlayState.Playing && gameManager.currentTime > invincibleUntil) // not for replays
        {
            var enemies = Physics.SphereCastAll(transform.position.ChangeZ(-50), EnemyCollisionRadius, Vector3.forward, 100, Layers.enemiesMask);
            //LogDebug("OVERLAP TEST " + enemies.Length);
            foreach (var enemy in enemies)
            {
                var enemyScript = enemy.collider.gameObject.FindInParents<Enemy>();
                if (enemyScript != null)
                {
                    Log("Collided with an enemy :(");
                    processor(new Action
                                  {
                                      type = ActionType.TakeDamage
                                  });
                    invincibleUntil = gameManager.currentTime + InvincibilityAfterHit;
                }
            }
        }
    }

    private bool TryMovement(float dx, float dy, float deltaTime, ActionProcessor processor)
    {
        if (dx == 0 && dy == 0)
        {
            // No movement
            return false;
        }

        var targetPos = transform.position + new Vector3(dx, dy) * WalkSpeed * deltaTime;
        var obstacles = Physics.OverlapSphere(targetPos, CollisionRadius, Layers.obstaclesMask);
        foreach (var obstacle in obstacles)
        {
            var unlockable = obstacle.gameObject.FindInParents<Unlockable>();
            if (unlockable != null && Keys > 0)
            {
                processor(new Action
                              {
                                  type = ActionType.Unlock,
                                  uid = unlockable.uid
                              });
            }
        }
        //LogDebug(obstacles.Length);
        if (obstacles.Length > 0)
        {
            return false;
        }

        transform.position = targetPos;
        facing = new Vector3(dx, dy).normalized;
        return true;
    }

    public void Apply(Action action, ActionProcessor processor)
    {
        if (action == null) // we might apply a null Movement
        {
            return;
        }

        switch (action.type)
        {
            case ActionType.Movement:
                currentMovementX = Mathf.RoundToInt(action.x);
                currentMovementY = Mathf.RoundToInt(action.y);
                break;

            case ActionType.Attack:
                var sword1 = transform.position + facing * (Sword.center.x - Sword.height / 2f + Sword.radius);
                var sword2 = transform.position + facing * (Sword.center.x + Sword.height / 2f - Sword.radius);
                Log("Attack! me={0} sword={1}=>{2}", transform.position, sword1, sword2);
                var hits = Physics.CapsuleCastAll(sword1.ChangeZ(-50), sword2.ChangeZ(-50), Sword.radius, Vector3.forward, 100, Layers.enemiesMask);
                Log(hits.Length + " hits");
                foreach (var hit in hits)
                {
                    var enemy = hit.collider.gameObject.FindInParents<Enemy>();
                    if (enemy != null)
                    {
                        Log("Hit {0}", enemy);
                        enemy.TakeDamage(1);
                        if (enemy.Life <= 0)
                        {
                            AudioSource.PlayClipAtPoint(EnemyDieSound, transform.position);
                            processor(new Action
                                          {
                                              type = ActionType.Kill,
                                              uid = enemy.uid
                                          });
                        }
                        else
                        {
                            AudioSource.PlayClipAtPoint(EnemyHitSound, transform.position);
                        }
                    }
                }
                StartCoroutine(AttackAnimation());
                cannotAttackBefore = action.gameTime + SwordCooldown;
                break;

            case ActionType.Pickup:
                var pickup = gameManager.FindPickup(action.uid);
                if (pickup != null)
                {
                    if (pickup.pickedUp)
                    {
                        LogWarning("The pickup {0} #{1} is already picked up", pickup.Type, pickup.uid);
                    }
                    else
                    {
                        switch (pickup.Type)
                        {
                            case PickupType.Bomb:
                                Bombs = Mathf.Clamp(Bombs + 1, 0, 99);
                                AudioSource.PlayClipAtPoint(PickupBombSound, transform.position);
                                break;

                            case PickupType.Key:
                                Keys = Mathf.Clamp(Keys + 1, 0, 99);
                                AudioSource.PlayClipAtPoint(PickupKeySound, transform.position);
                                break;

                            case PickupType.SmallHeart:
                                Life = Mathf.Clamp(Life + 1, 0, MaxLife);
                                AudioSource.PlayClipAtPoint(PickupHeartSound, transform.position);
                                break;

                            case PickupType.BigHeart:
                                MaxLife = Mathf.Clamp(MaxLife + 2, 0, 24);
                                Life = MaxLife;
                                AudioSource.PlayClipAtPoint(PickupHeartSound, transform.position);
                                break;

                            case PickupType.Checkpoint:
                                checkpoint = pickup.transform.position.ChangeZ(transform.position.z);
                                Log("New checkpoint! {0}", checkpoint);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        pickup.Die();
                    }
                }
                else
                {
                    LogError("Could not find pickup #{0}", action.uid);
                }
                break;

            case ActionType.Kill:
                var kill = gameManager.FindEnemy(action.uid);
                if (kill != null)
                {
                    if (kill.dead)
                    {
                        LogWarning("The enemy #{0} is already dead", action.uid);
                    }
                    else
                    {
                        switch (kill.Type)
                        {
                            case EnemyType.Patroller:
                                PatrollerKills++;
                                break;

                            case EnemyType.Spider:
                                SpiderKills++;
                                break;
                        }
                        kill.Die();
                    }
                }
                else
                {
                    LogError("Could not find enemy #{0}", action.uid);
                }
                break;

            case ActionType.Break:
                var bombable = gameManager.FindBombable(action.uid);
                if (bombable != null)
                {
                    bombable.Die();
                }
                else
                {
                    LogError("Could not find bombable #{0}", action.uid);
                }
                break;

            case ActionType.Unlock:
                var unlockable = gameManager.FindUnlockable(action.uid);
                if (unlockable != null)
                {
                    if (!unlockable.locked)
                    {
                        LogWarning("The unlockable #{0} is not locked", action.uid);
                    }
                    else if (Keys > 0)
                    {
                        Keys--;
                        unlockable.Unlock();
                        AudioSource.PlayClipAtPoint(UnlockSound, transform.position);
                    }
                    else
                    {
                        LogError("Not enough keys to unlock #{0}", action.uid);
                    }
                }
                else
                {
                    LogError("Could not find unlockable #{0}", action.uid);
                }
                break;

            case ActionType.TakeDamage:
                Life--;
                AudioSource.PlayClipAtPoint(SelfHitSound, transform.position);
                Log("Took damage. Life is now {0}", Life);
                if (Life == 0)
                {
                    processor(new Action
                                  {
                                      type = ActionType.Death
                                  });
                }
                break;

            case ActionType.Death:
                Log("Died :( Back to the last checkpoint at {0}", checkpoint);
                AudioSource.PlayClipAtPoint(SelfDieSound, transform.position);
                StartCoroutine(Die());
                break;

            case ActionType.DropBomb:
                Log("Dropping bomb at {0} on {1:0.000}", transform.position, gameManager.currentTime);
                Bombs--;
                InstantiatePrefabAt(BombPrefab, transform.position);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private IEnumerator Die()
    {
        StopMovement();
        cannotAct = true;

        var start = gameManager.currentTime;
        var initialRot = transform.localEulerAngles.z;
        var animPercent = 0f;
        while (animPercent < 1f)
        {
            animPercent = (float)(gameManager.currentTime - start) / DeathAnimation;
            Sprite.transform.SetRotation(initialRot + animPercent * 720);
            yield return null;
        }

        transform.position = checkpoint;
        Sprite.transform.SetRotation(initialRot);
        Life = MaxLife;
        cannotAct = false;
    }

    private IEnumerator AttackAnimation()
    {
        AudioSource.PlayClipAtPoint(AttackSound, transform.position);
        var start = gameManager.currentTime;
        attacking = true;
        while (gameManager.currentTime < start + .2f)
        {
            yield return null;
        }
        attacking = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, CollisionRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, EnemyCollisionRadius);
    }

}
