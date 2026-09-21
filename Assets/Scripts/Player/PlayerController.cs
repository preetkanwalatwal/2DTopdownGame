using System.Collections;
using TMPro;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;
public class PlayerController : MonoBehaviour
{
    private Vector2 moveDirection;
    private Vector2 lookDirection;
    private Vector2 facingDirection = Vector2.down;

    public bool moving = false;
    public bool isRunning = false;
    public bool isAttacking = false;
    public bool attackReady = true;

    public float walkspeed;
    public float runSpeed;

    public float attackRate;
    public float attackDuration = 0.5f;
    public float attackTimer = 0f;

    // Melee hitbox timing per attack animation clip/state
    [System.Serializable]
    public class AttackTiming
    {
        public string stateName;       // must match the Animator state name for that clip
        [Range(0f, 1f)] public float hitStart = 0.4f; // normalized time hitbox turns on
        [Range(0f, 1f)] public float hitEnd = 0.6f;   // normalized time hitbox turns off
    }
    public AttackTiming[] attackTimings;
    private bool meleeCurrentlyActive = false;

    // Ranged settings
    public GameObject projectile;
    public float projectileSpeed;
    public float fireForce = 10f;
    float shootCooldown = 0.25f;
    float shootTimer = 0.5f;


    public GameObject Melee;
    // GameObject attackProjectile;
    private Rigidbody2D rb;
    public Transform lookTransform;
    public Transform meleeAim;
    public Animator myAnim;

    public float interactRange = 1f;
    public KeyCode turnLeftKey = KeyCode.Q;
    public KeyCode turnRightKey = KeyCode.E;
    public LayerMask leverLayer;

    public enum AimMode
    {
        Controller,
        Mouse
    }
    public AimMode aimMode = AimMode.Controller;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myAnim = GetComponentInChildren<Animator>();

        aimMode = (AimMode)PlayerPrefs.GetInt("AimMode", 0);
    }

    void Update()
    {
        CheckMeleeTimer();
        UpdateMeleeHitboxFromAnimator();
        shootTimer += Time.deltaTime;

        if (aimMode == AimMode.Mouse)
        {
              MouseAim();
        }

        if (Input.GetKeyDown(turnLeftKey) || Input.GetKeyDown(turnRightKey))
        {
            LeverInteraction();
        }
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    public void SetAimMode(int mode)
    {
        aimMode = (AimMode)mode;
        PlayerPrefs.SetInt("AimMode", mode);
    }

    public void OnMove(InputValue value)
    {
        moveDirection = value.Get<Vector2>().normalized;
        moving = moveDirection.sqrMagnitude > 0.01f;

        if(moving && aimMode == AimMode.Controller)
        {
            facingDirection = moveDirection;
        }
    }

    public void OnLook(InputValue lookValue)
    {
        if(aimMode != AimMode.Controller) return;

        lookDirection = lookValue.Get<Vector2>();

        if (lookDirection.sqrMagnitude > 0.01f)
        {
            lookDirection.Normalize();
            lookTransform.up = lookDirection;
            meleeAim.up = lookDirection;
        }
    }

    public void MouseAim()
    {
        if (Mouse.current == null) return;

        Vector3 mouseScreen = Mouse.current.position.ReadValue();
        mouseScreen.z = Mathf.Abs(Camera.main.transform.position.z);

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0f;

        Vector2 direction = (mouseWorld - transform.position).normalized;

        if (direction.sqrMagnitude < 0.001f) return;

        lookDirection = direction;
        facingDirection = direction;

        lookTransform.up = direction;
        meleeAim.up = direction;
    }

    public void OnRun(InputValue runValue)
    {
        isRunning = runValue.isPressed;
    }

    public void OnFire1(InputValue attackValue)
    {
        if (!attackValue.isPressed) return;
        if (!attackReady) return;

        if (aimMode == AimMode.Mouse)
        {
            MouseAim();
        }
        else
        {
            if (lookDirection.sqrMagnitude > 0.001f)
            {
                facingDirection = lookDirection.normalized;
            }
        }

        myAnim.SetFloat("LastMoveX", facingDirection.x);
        myAnim.SetFloat("LastMoveY", facingDirection.y);

        StartAttack();
    }

    public void OnFire2(InputValue rangeValue)
    {
        if(rangeValue.isPressed && attackReady)
        {
            StartRanged();
        }
    }

    void StartAttack()
    {
        isAttacking = true;
        attackReady = false;
        attackTimer = 0f;
        meleeCurrentlyActive = false; // reset so the normalized-time check re-evaluates fresh

        myAnim.SetBool("IsAttacking", true);
    }

    void StartRanged()
    {
        if(shootTimer > shootCooldown)
        {
            shootTimer = 0;
            GameObject intProjectile = Instantiate(projectile, meleeAim.position, meleeAim.rotation);
            intProjectile.GetComponent<Rigidbody2D>().AddForce(meleeAim.up * fireForce, ForceMode2D.Impulse);
            Destroy(intProjectile, 1.5f);
        }
    }

    void PlayerMovement()
    {
        float currentSpeed = isRunning ? runSpeed : walkspeed;
        rb.linearVelocity = moveDirection * currentSpeed;

        UpdateAnimation();
    }

    void CheckMeleeTimer()
    {
        if(!isAttacking) return;

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackDuration)
        {
            attackTimer = 0f;
            isAttacking = false;
            attackReady = true;

            Melee.SetActive(false);
            meleeCurrentlyActive = false;
            myAnim.SetBool("IsAttacking", false);
        }
    }

    void UpdateMeleeHitboxFromAnimator()
    {
        if (!isAttacking) return;

        AnimatorStateInfo state = myAnim.GetCurrentAnimatorStateInfo(0);

        foreach (var timing in attackTimings)
        {
            if (state.IsName(timing.stateName))
            {
                float t = state.normalizedTime % 1f;
                bool shouldBeActive = t >= timing.hitStart && t <= timing.hitEnd;

                if (shouldBeActive != meleeCurrentlyActive)
                {
                    Melee.SetActive(shouldBeActive);
                    meleeCurrentlyActive = shouldBeActive;
                }
                return;
            }
        }
    }

    IEnumerator CoolDown()
    {
        myAnim.SetBool("IsAttacking", false);
        yield return new WaitForSeconds(attackRate);
        attackReady = true;
    }

    void UpdateAnimation()
    {
        if (!isAttacking)
        {
            myAnim.SetFloat("MoveX", moveDirection.x);
            myAnim.SetFloat("MoveY", moveDirection.y);
            myAnim.SetFloat("Speed", moveDirection.sqrMagnitude);
            myAnim.SetBool("IsRunning", isRunning);

            if (moveDirection != Vector2.zero)
            {
                myAnim.SetFloat("LastMoveX", facingDirection.x);
                myAnim.SetFloat("LastMoveY", facingDirection.y);
            }
        }
    }

    void LeverInteraction()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactRange, leverLayer);
        if (!hit) return;

        Lever lever = hit.GetComponentInChildren<Lever>();
        if (!lever) return;

        if (Input.GetKeyDown(turnLeftKey)) lever.TurnLeft();
        else if (Input.GetKeyDown(turnRightKey)) lever.TurnRight();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}