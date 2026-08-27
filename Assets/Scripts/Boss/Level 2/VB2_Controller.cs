// using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class VB2_Controller : MonoBehaviour
{
    VB2_Combat combat;
    VB2_Movement movement;
    VB2_Animation animations;
    VB2_Health health;
    VB2_Sound aduio;

    public Transform[] corners;
    public Transform currentCorner;
    public int hitsToInterupt = 3;
    public int currentHits;
    public float cornerIntervals;
    Coroutine cornerRoutine;
    
    public Transform player;
    public Transform aim;
    public Vector2 aimDirection;

    public bool lookingAtPlayer = false;
    public float aimRotateSpeed;

    public GameObject prefab;
    public Transform firePoint;
    public float spreadAngle = 10f;   

    public enum BossState
    {
        Idle, Chase, Melee, Range,
        // Retreat,
        Dead
    }
    public BossState currentState;

    public float minDistance = 2.5f;
    public float maxDistance;
    private float distanceToPlayer;
    public float attackTime = 1.5f;

    public bool start = false;
    public bool meleeAttack = false;
    public bool rangeAttack = false;
    public bool isMove = false;
    public bool isRun = false;
    public bool isAttack = false;
    public bool rotateCorner = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        combat = GetComponent<VB2_Combat>();
        movement = GetComponent<VB2_Movement>();
        health = GetComponent<VB2_Health>();
        animations = GetComponentInChildren<VB2_Animation>();

        currentState = BossState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector2.Distance(transform.position, player.position);
        // Debug.Log(distanceToPlayer);
        animations.UpdateAnimation(movement.VB_moveDirection, isRun, isAttack);
        LookAim();

        HandleStateChange();
        // switch (currentState)
        // {
        //     case BossState.Dead:
        //         if (health.currentHealth <= 0)
        //         {
        //             health.VB2Deadth();
        //         }
        //         break;
        //     case BossState.Attack:
        //         if (distanceToPlayer <= 2.0f)
        //         {
        //             movement.StopMove();
        //             combat.MeleeAttack();
        //         }
        //         break;
        //     default:
        //         break;
        // }
        // if (currentState == BossState.Attack)
        // {
        //     // Movee closer
        //     movement.MoveTo(player.position);
        //     // Attack
        //     // Debug.Log(distanceToPlayer);
        //     if (distanceToPlayer <= 2.0f)
        //     {
        //         Debug.Log("Stop");
        //         movement.StopMove();
        //         // MeleeAttackAnim
        //         // MeleeAttackSound
        //         combat.MeleeAttack();

        //     }
        //     // Next Move.
        //     movement.MoveTo(player.position);
        // }

        // if (hitcounts % 10 == 0)
        // {
        //     currentState = BossState.Retreat;

        //     // Distance away from player

        //     // Mage attack

        //     // Reset to Attack state
        // }

        // if(health.maxHealth <= 0)
        // {
        //     currentState = BossState.Dead;
        // }



    }

    void HandleStateChange()
    {
        if (currentState == BossState.Dead) return;

        rangeAttack = health.rangePhase(health.currentHealth);
        if (rangeAttack)
        {
            ChangeState(BossState.Range);
        }
        switch (currentState)
        {
            case BossState.Idle:
                IdleBehaviour();
                break;
            case BossState.Chase:
                ChaseBehaviour();
                break;
            case BossState.Melee:
                MeleeBehaviour();
                break;
            case BossState.Range:
                RangeBehaviour();
                break;
            // case BossState.Retreat:
            //     // Retreat behaviour
            //     RetreatBehaviour();
            //     break;
            case BossState.Dead:
                // Death behaviour
                break;
            default:
                break;
        }
    }

    void LookAim()
    {
        // if (!lookingAtPlayer) return;
        // Get the direction toward the target
        aimDirection = player.position - aim.position;

        // Compute the target rotation
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        // Smoothly rotate toward the target
        aim.rotation = Quaternion.RotateTowards(
            aim.rotation,
            targetRotation,
            aimRotateSpeed * Time.deltaTime * 25f // multiply by 100 for nicer tuning
        );
    }

    void IdleBehaviour()
    {
        // Initiate fight.
        if (start)
        {
            if (distanceToPlayer >= minDistance)
            {
                ChangeState(BossState.Chase);
            }
            start = false;
        }
        if (distanceToPlayer <= minDistance && !isMove)
        {
            movement.StopMove();
        }
        if(distanceToPlayer > minDistance && isMove)
        {
            ChangeState(BossState.Chase);
        }
        
        

        // Health for Melee/Range phase
    }
    
    void ChaseBehaviour()
    {
         // Move to player 
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer >= minDistance)
        {
            isMove = true;
            movement.MoveTo(player.position);
        }
        else if (distanceToPlayer <= minDistance)
        {
            ChangeState(BossState.Melee);
        }
    
    }

    void MeleeBehaviour()
    {
        movement.StopMove();

        if (!meleeAttack)
        {
            StartCoroutine(AttackRoutine());
        }

        // hitCounts to throw random ranged attack - if()
        // ChangeState(BossState.Retreat);

        if (isMove && !isAttack)
        {
           ChangeState(BossState.Chase); 
        }
        else
        {
            ChangeState(BossState.Idle);
        }
        // ChangeState(BossState.Chase);
    }

    IEnumerator AttackRoutine()
    {
        isAttack = true;
        meleeAttack = true;
        // movement.StopMove();
        // movement.VB_isMoving = false;
        isMove = false;
        float timer = 0f;
        animations.isAttacking = true;

        yield return null; // Wait a frame
        while (timer < attackTime)
        {
            timer += Time.deltaTime;
            animations.UpdateCombatAnimation(movement.VB_moveDirection);
            yield return null;
        }

        animations.isAttacking = false;
        meleeAttack = false;
        isAttack = false;
        isMove = true;
    }
    
    void RangeBehaviour()
    {
        movement.StopMove();
        if(rangeAttack && !rotateCorner) 
        {
            StartCoroutine(FlyToCornerCoroutine());
        }
        if (movement.hasReachedCorner)
        {
            
            // movement.hasChosenTarget = false;
            animations.CornerIdle(movement.travelTo);
            StartCornerRoutine();
            if(currentHits == hitsToInterupt)
            {
                movement.hasReachedCorner = false;
                movement.hasChosenTarget = false;
                rotateCorner = true;
            }
            rotateCorner = false;
        }

    

        if(!rangeAttack)
        {
            // rangeAttack = false;
            movement.FlyToCenter(true);

            ChangeState(BossState.Chase);
        }

        
    }

    void FireProjectiles()
    {
        Vector2 aimDirection = (player.position - firePoint.position).normalized;
        
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        
        Quaternion centerRotation = Quaternion.Euler(0, 0, angle);
        Quaternion leftRotation = Quaternion.Euler(0, 0, angle + spreadAngle);
        Quaternion rightRotation = Quaternion.Euler(0, 0, angle - spreadAngle);

        Instantiate(prefab, firePoint.position, leftRotation);
        Instantiate(prefab, firePoint.position, centerRotation);
        Instantiate(prefab, firePoint.position, rightRotation);

        // Rigidbody2D prefabRb = prefab.GetComponent<Rigidbody2D>();
        // prefabRb.linearVelocity = aimDirection * projectileSpeed;
    }

    public void StartCornerRoutine()
    {
        if(cornerRoutine == null)
        {
            cornerRoutine = StartCoroutine(FireProjectilesCoroutine());
        }
    }

    IEnumerator FireProjectilesCoroutine()
    {
        while(currentHits < hitsToInterupt)
        {
            FireProjectiles();
            yield return new WaitForSeconds(cornerIntervals);
            
        }
        cornerRoutine = null;
        movement.ChooseNewCorner(movement.travelTo, rangeAttack);
        currentHits = 0;
        
        // movement.FlyToCenter();
    }
    IEnumerator FlyToCornerCoroutine()
    {
        // Fly to corner
        animations.UpdateFlyAnimation(rangeAttack);

        // Move to random corner
        movement.FlyToCorner(rangeAttack);

        yield return null;
    }

    // IEnumerator RangeBehaviourCoroutine()
    // {        
        
    //     // Start RangeAttack coroutine

    //     // 3 Player hit counts to visit new corner.

        
    // }
   
    // Retreat state = random ranged attack
    // void RetreatBehaviour()
    // {
    //     meleeAttack = false;
    //     rangeAttack = false;
    //     isMove = true;
    //     movement.MoveTo(GetRandomPointBehind(transform, 5, 10));
    //     ChangeState(BossState.Idle);

    //     // Decide for next attack
    // }


    void DeathBehaviour()
    {
        movement.VB_moveDirection = Vector2.zero;
        movement.VB_moveSpeed = 0;
        // Anim
        // Possibly sound
        
        Destroy(gameObject);
    }

    void ChangeState(BossState newState)
    {
        // Debug.Log("Was " + currentState + ", now is " + newState);
        currentState = newState;
    }
    
    // Start battle
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log("Player detected");
        if (collision.CompareTag("Player"))
        {
            start = true;
            // ChangeState(BossState.Range);
            // ChangeState(BossState.Retreat);
        }
    }
    
    // Vector2 GetRandomPointBehind(Transform obj, float minDist, float maxDist)
    // {
    //     // Get the object's facing direction (2D)
    //     Vector2 facingDir = obj.up.normalized;

    //     // Opposite direction
    //     Vector2 oppositeDir = -facingDir;

    //     // Pick a random distance behind the object
    //     float distance = Random.Range(minDist, maxDist);

    //     // Compute the final point
    //     Vector2 randomPoint = (Vector2)obj.position + oppositeDir * distance;

    //     Debug.DrawLine(obj.position, randomPoint, Color.red, 5f);
    //     Debug.Log(randomPoint);
    //     return randomPoint;
    // }
}
