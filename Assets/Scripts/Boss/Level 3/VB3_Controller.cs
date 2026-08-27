using System.Collections;
using Unity.VisualScripting;

// using System.Numerics;

using UnityEngine;

public class VB3_Controller : MonoBehaviour
{
    public enum vb3_States
    {
        None,
        Initial, // Starting state
        Aggressive, 
        Retreat,
        Phase1, // Boss to center, spawn stuff to dodge
        Phase2, // Chase player, does melee and range while throwing traps
        Dead
    }
    public vb3_States currentState;

    VB3_Movement vB3_Movement;
    VB3_Animation vB3_Animation;
    VB3_Combat vB3_Combat;
    VB3_HealthSystem vB3_HealthSystem;
    public RotateCenter rotateCenter;
    public Transform playerTarg;
    public Transform aim;
    public float aimRotateSpeed;
    // Vector2 moveDirection;
    public Vector2 aimDirection;

    public bool initialEntered = false;
    public bool isFlying = false;
    public bool isAttacking = false;
    public bool isAgro = false;
    public bool attackReady = true;
    public bool phase1 = false;
    public bool changeToPhase1 = false;
    public bool phase1Started = false;
    public bool ph1firing = false;
    public bool phase2 = false;

    void Start()
    {
        vB3_HealthSystem = GetComponent<VB3_HealthSystem>();
        vB3_Movement = GetComponent<VB3_Movement>();
        vB3_Animation = GetComponentInChildren<VB3_Animation>();
        vB3_Combat = GetComponent<VB3_Combat>();
        
        // moveDirection = vB3_Movement.vb3_moveDirection;
        currentState = vb3_States.None;
    }

    void Update()
    {
        LookAim();
        // if(isFlying) return;
        // if(!isFlying && !vB3_HealthSystem.isHit) 
        if (!isFlying && isAgro)
        {
            vB3_Animation.UpdateAnims(vB3_Movement.vb3_moveDirection, isAttacking);
        }
        if (isFlying)
        {
            vB3_Animation.PlayMoveFly(vB3_Movement.vb3_moveDirection, isFlying);
        }
        if (vB3_HealthSystem.isHit)
        {
            vB3_Movement.vb3_isMoving = false;
            vB3_Animation.UpdateHurtAnim(vB3_Movement.vb3_moveDirection, vB3_HealthSystem.isHit);    
            // vB3_Animation.UpdateHurtAnim(vB3_Movement.vb3_moveDirection, false);    
        }
        
        if(vB3_HealthSystem.currentHealth <= 0 && vB3_HealthSystem.isDead)
        {
            vB3_Animation.PlayDeathAnim(vB3_Movement.vb3_moveDirection, vB3_HealthSystem.isDead);
        }
        
        HandleStateChange();
        
    }

    void LookAim()
    {
        // if (!lookingAtPlayer) return;
        // Get the direction toward the target
        aimDirection = playerTarg.position - aim.position;

        // Compute the target rotation
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg + 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        // Smoothly rotate toward the target
        aim.rotation = Quaternion.RotateTowards(
            aim.rotation,
            targetRotation,
            aimRotateSpeed * Time.deltaTime * 100f // multiply by 100 for nicer tuning
        );
    }

    void HandleStateChange()
    {
        // if(currentState == vb3_States.Dead ) return;
        phase1 = vB3_HealthSystem.InitiatePhase1(vB3_HealthSystem.currentHealth);
        phase2 = vB3_HealthSystem.InitiatePhase2(vB3_HealthSystem.currentHealth);
        if(phase1 && !changeToPhase1)
        {
            changeToPhase1 = true;
            ChangeState(vb3_States.Phase1);
        } 
        if(phase2) ChangeState(vb3_States.Phase2);
        switch (currentState)
        {
            case vb3_States.None:
                break;
            case vb3_States.Initial:
                InitialState();
                break;
            case vb3_States.Aggressive:
                AggressiveState();
                break;
            case vb3_States.Retreat:
                RetreatState();
                break;
            case vb3_States.Phase1:
                Phase1State();
                break;
            case vb3_States.Phase2:
                Phase2State();
                break;
            case vb3_States.Dead:
                DeadState();
                break;
            default:
                Debug.Log("Error state: " + currentState);
                break;
        }
    }

    void InitialState()
    {
        if (!initialEntered && !phase1)
        {
            initialEntered = true;
            StartCoroutine(InitialStateRoutine());
        }
        
        if (isAgro && vB3_Movement.hasLeftCenter)
        {
            initialEntered = false;
            ChangeState(vb3_States.Aggressive);   
        }
        
    }

    IEnumerator InitialStateRoutine()
    {
        if (!vB3_Movement.hasLeftCenter)
        {
            vB3_Animation.PlayFly();
        }  

        isFlying = true;
        vB3_Movement.LeaveCenter();
        
        yield return new WaitForSeconds(4);
        
        isAgro = true;
        isFlying = false;
    }

    void AggressiveState()
    {
        vB3_Movement.ChasePlayer(playerTarg.position);
        float distToPlayer = Vector2.Distance(transform.position, playerTarg.position);
        // Debug.Log(distToPlayer);
        if(distToPlayer >= 2.5f && !attackReady && !isAttacking)
        {
            attackReady = true;
        }
        if(distToPlayer <= 2.0f && attackReady && !isAttacking)
        {
            // vB3_Movement.StopMove();
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;
        attackReady = false;
        vB3_Movement.StopMove();
        // vB3_Combat.BasicAttack();
        vB3_Animation.PlayAttackAnim();
        vB3_Combat.ApplyDamge();
        yield return new WaitForSeconds(vB3_Combat.attackCooldown);
        
        isAttacking = false;
    }

    
    void RetreatState()
    {
        
    }
    void Phase1State()
    {
        
        isAgro = false;
        if (!phase1)
        {
            CancelInvoke(nameof(FireP1));
            ph1firing = false; // reset for next time
        }

        // if (phase1Started)
        // {
        //     phase1Started = false;

        //     isFlying = true;
        //     vB3_Animation.PlayFly();
        // }

        bool reachedCenter = vB3_Movement.IsAtCenter();

        if (!reachedCenter)
        {
            vB3_Animation.PlayMoveFly(vB3_Movement.vb3_moveDirection, true);
        }
        else if(reachedCenter && phase1)
        {
            isFlying = false;
            vB3_Animation.PlayAttackAnim();

            if (!ph1firing)
            {
                ph1firing = true;
                InvokeRepeating(nameof(FireP1), 0f, .5f);
            }
            
        }
    }

    void FireP1()
    {
        rotateCenter.FireProjectilesFromCenter();
    }

    IEnumerator P1Routine()
    {
        
        float count = 0f;

        while (phase1)
        {
            // Debug.Log("Playing P1");
            count += Time.deltaTime;
            if (count >= 3f)
            {
                count = 0f;
                rotateCenter.FireProjectilesFromCenter();
            }
            yield return null;  
        }
    }
    void Phase2State()
    {
        vB3_Movement.ChasePlayer(playerTarg.position);
        rotateCenter.transform.position = Vector2.MoveTowards(rotateCenter.transform.position, transform.position, 0.3f * Time.deltaTime);

        float distToPlayer = Vector2.Distance(transform.position, playerTarg.position);
        // Debug.Log(distToPlayer);
        if(distToPlayer >= 2.5f && !attackReady && !isAttacking)
        {
            attackReady = true;
        }
        if(distToPlayer <= 2.0f && attackReady && !isAttacking)
        {
            // vB3_Movement.StopMove();
            StartCoroutine(PerformAttack());
        }
    }
    void DeadState()
    {
        Debug.Log("Now is Dead");
        vB3_Movement.vb3_isMoving = false;
        vB3_HealthSystem.VB3Death();
    }

    public void StartFight()
    {
        ChangeState(vb3_States.Initial);
    }
    

    void ChangeState(vb3_States newState)
    {
        Debug.Log(newState);
        currentState = newState;
    }
}
