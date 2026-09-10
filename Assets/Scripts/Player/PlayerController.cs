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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myAnim = GetComponentInChildren<Animator>();

        aimMode = (AimMode)PlayerPrefs.GetInt("AimMode", 0); // Default to Controller if not set
    }

    // Update is called once per frame
    void Update()
    {
        CheckMeleeTimer();
        shootTimer += Time.deltaTime;

        if (aimMode == AimMode.Mouse)
        {
              MouseAim();
        }

        // if (isAttacking && attackReady)
        // {
        //     Attack();
        // }

        if (Input.GetKeyDown(turnLeftKey) || Input.GetKeyDown(turnRightKey))
        {
            // Debug.Log("Input is being made");
            LeverInteraction();
        }
        // OnDrawGizmosSelected();
    }

    private void FixedUpdate()
    {
        PlayerMovement();

        //if (lookDirection.sqrMagnitude > 0.01f)
        //{
        //    Vector3 aim = Vector3.left * lookDirection.x + Vector3.down + Vector3.up * lookDirection.y;
        //    meleeAim.rotation = Quaternion.LookRotation(Vector3.forward, aim);
        //}
    }

    // Options menu where 0 = controller and 1 = mouse
    public void SetAimMode(int mode)
    {
        Debug.Log("SetAimMode is being called from Player");

        aimMode = (AimMode)mode;

        PlayerPrefs.SetInt("AimMode", mode);

        if(mode == 0)
        {
            Debug.Log("Aim mode set to Controller");
        }
        else
        {
            Debug.Log("Aim mode set to Mouse");
        }
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
        // if(Mouse.current == null) return;

        // // Convert mouse position to world position
        // Vector3 mouseScreen = Mouse.current.position.ReadValue();
        // mouseScreen.z = Mathf.Abs(Camera.main.transform.position.z);

        // Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        // mouseWorld.z = 0f;

        // // Direction from player to mouse
        // Vector2 direction = (mouseWorld - transform.position).normalized;

        // if (direction.sqrMagnitude < 0.001f) return;

        // lookDirection = direction;

        // lookTransform.up = lookDirection;
        // meleeAim.up = lookDirection;

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
        // Debug.Log("Running: " + isRunning);
    }

    public void OnFire1(InputValue attackValue)
    {
        if (!attackValue.isPressed) return;
        if (!attackReady) return;

        // Make sure the latest mouse position is used
        if (aimMode == AimMode.Mouse)
        {
            MouseAim();
        }
        else
        {
            // Controller: use current look direction
            if (lookDirection.sqrMagnitude > 0.001f)
            {
                facingDirection = lookDirection.normalized;
            }
        }

        // Force the Animator to face the aim direction
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

        Melee.SetActive(true);
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

    void Attack()
    {
        // if (!isAttacking && attackReady)
        // {
        //     attackReady = false;
        //     StartCoroutine(BasicAttack());
        // }
        // else
        // {
        //     attackReady = true;
        //     isAttacking = false;
        // }
        // attackReady = false;
        // myAnim.SetBool("IsAttacking", isAttacking);
        // isAttacking = false;
        // StartCoroutine(BasicAttack());
       
            Melee.SetActive(true);
            // isAttacking = true;
            attackReady = false;
            myAnim.SetBool("IsAttacking", isAttacking);
            // StartCoroutine(BasicAttack());
        
    }
    // void CheckRangedTimer()
    // {
    //     if(shootTimer > shootCooldown)
    //     {
    //         shootTimer = 
    //     }
    // }
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
            myAnim.SetBool("IsAttacking", false);
        }
    }
    IEnumerator BasicAttack()
    {
        lookDirection = transform.up;
        GameObject attack = Instantiate(projectile, lookTransform.position, lookTransform.rotation);
        // GameObject attack = Instantiate(projectile, lookTransform.position, Quaternion.identity);
        attack.GetComponent<SwordAttackProjectile>().SetDirection(lookDirection);
        Rigidbody2D projRb = attack.GetComponent<Rigidbody2D>();
        // projRb.AddForce(lookTransform.up * projectileSpeed, ForceMode2D.Impulse);
        projRb.linearVelocity = lookDirection.normalized * projectileSpeed;
        yield return new WaitForSeconds(attackRate);
        StartCoroutine(CoolDown());

        // Vector2 offset = lookDirection.normalized * 0.5f; // 0.5 units in front of player
        // Vector3 spawnPos = lookTransform.position + (Vector3)offset;
        // Vector2 attackDir = lookDirection.sqrMagnitude > 0.01f ? lookDirection : lookTransform.up;

        // // Spawn projectile facing that direction
        // Quaternion rot = Quaternion.LookRotation(Vector3.forward, attackDir);
        // // GameObject attack = Instantiate(projectile, lookTransform.position, rot);
        // GameObject attack = Instantiate(projectile, spawnPos, Quaternion.LookRotation(Vector3.forward, lookDirection));


        // // Apply velocity to the spawned instance
        // Rigidbody2D projRb = attack.GetComponent<Rigidbody2D>();
        // projRb.linearVelocity = attackDir.normalized * projectileSpeed;

        // // Optional: inform the projectile script of its direction
        // var swordProj = attack.GetComponent<SwordAttackProjectile>();
        // if (swordProj != null)
        //     swordProj.SetDirection(attackDir);

        // yield return new WaitForSeconds(attackRate);
        // StartCoroutine(CoolDown());
    }
    
    IEnumerator CoolDown()
    {
        myAnim.SetBool("IsAttacking", false);
        yield return new WaitForSeconds(attackRate);
        attackReady = true;
    }

    void UpdateAnimation()
    {
        // // Update Animator parameters
        // myAnim.SetFloat("MoveX", moveDirection.x);
        // myAnim.SetFloat("MoveY", moveDirection.y);
        // myAnim.SetFloat("Speed", moveDirection.sqrMagnitude);
        // myAnim.SetBool("IsRunning", isRunning);
        // if (!isAttacking)
        // {
        //     myAnim.SetBool("IsAttacking", isAttacking);
        // }
        // // Remember last movement direction for idle facing
        // if (moveDirection != Vector2.zero)
        // {
        //     myAnim.SetFloat("LastMoveX", facingDirection.x);
        //     myAnim.SetFloat("LastMoveY", facingDirection.y);
        // }
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
