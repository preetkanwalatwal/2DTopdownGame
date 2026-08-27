using UnityEngine;
using UnityEngine.Jobs;

public class Vamp1Movement : MonoBehaviour
{
    public enum BossState { Idle, Agro }
    public BossState currentState = BossState.Idle;

    [Header("General Settings")]
    public float moveSpeed = 2f;
    private Rigidbody2D rb;

    [Header("Idle Movement Settings")]
    public Vector2 moveAreaMin;  // bottom-left of roam area
    public Vector2 moveAreaMax;  // top-right of roam area
    public float idleMoveTime = 2f;  // how long to move
    public float idleWaitTime = 1.5f;  // wait before moving again

    private Vector2 randomTarget;
    public bool isMovingIdle = false;
    public bool agro = false;

    [Header("Agro Settings")]
    public Transform target;
    public float chaseSpeed = 3f;

    public Vector2 moveDirection;

    [Header("Obstacle Avoidance")]
    public LayerMask obstacleLayer;
    public float avoidRayDistance = 1.5f;
    public float steerCommitDuration = 0.5f; // how long to stick with a chosen side
    public float tieBreakThreshold = 0.1f;   // how close dotA/dotB need to be to count as a tie

    private CircleCollider2D bossCollider;
    private float bossColliderRadius = 1f; // fallback if no CircleCollider2D found

    private int lastSteerSign = 0;
    private float steerCommitTimer = 0f;
    private int persistentBiasSign = 1; // fixed tie-break preference, set once in Start()

    

    Vamp1Combat vc;

    void Start()
    {
        vc = GetComponent<Vamp1Combat>();
        rb = GetComponent<Rigidbody2D>();

        // Pull the real collider radius so avoidance always matches the actual physics body
        bossCollider = GetComponent<CircleCollider2D>();
        if (bossCollider != null)
        {
            bossColliderRadius = bossCollider.radius;
        }

        // Decide the tie-break bias once per boss instance (so multiple bosses don't all clump the same way)
        persistentBiasSign = (GetInstanceID() % 2 == 0) ? 1 : -1;

        StartIdleMovement();
    }

    void Update()
    {
        switch (currentState)
        {
            case BossState.Idle:
                HandleIdle();
                break;

            case BossState.Agro:
                HandleAgro();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (vc.isDashing)
        {
            // Let Vamp1Combat's Dash() coroutine drive velocity directly —
            // don't let this script stomp on it or zero it out.
            return;
        }

        if (vc.isAttacking)
        {
            StopMoving();
        }
        rb.linearVelocity = moveDirection;
    }

    // -------------------------------
    // IDLE LOGIC
    // -------------------------------
    void HandleIdle()
    {
        if (!isMovingIdle) return;

        Vector2 direction = (randomTarget - (Vector2)transform.position).normalized;
        moveDirection = direction * moveSpeed;

        if (Vector2.Distance(transform.position, randomTarget) < 0.2f)
        {
            moveDirection = Vector2.zero;
            isMovingIdle = false;
            Invoke(nameof(StartIdleMovement), idleWaitTime);
        }
    }

    void StartIdleMovement()
    {
        randomTarget = new Vector2(
            Random.Range(moveAreaMin.x, moveAreaMax.x),
            Random.Range(moveAreaMin.y, moveAreaMax.y)
        );
        isMovingIdle = true;
    }

    // -------------------------------
    // AGRO LOGIC
    // -------------------------------
    public void HandleAgro()
    {
        if (target == null)
        {
            moveDirection = Vector2.zero;
            return;
        }

        Vector2 desiredDir = (target.position - transform.position).normalized;
        Vector2 steeredDir = GetSteeredDirection(desiredDir);
        moveDirection = steeredDir * chaseSpeed;
    }

    public void MeleeRun()
    {
        Vector2 desiredDir = (target.position - transform.position).normalized;
        Vector2 steeredDir = GetSteeredDirection(desiredDir);
        moveDirection = steeredDir * chaseSpeed * 5f;
        rb.MovePosition(rb.position + moveDirection * Time.deltaTime);
    }

    public void StopMoving()
    {
        rb.linearVelocity = Vector2.zero;
    }

    public void StartFight(Transform playerTransform)
    {
        target = playerTransform;
        currentState = BossState.Agro;
        agro = true;

        CancelInvoke(); // stop idle wandering
    }

    // -------------------------------
    // OBSTACLE AVOIDANCE
    // -------------------------------
    Vector2 GetCastOrigin()
    {
        // Use the collider's actual bounds center so casts originate from
        // where the physical body really is (feet), not the sprite pivot.
        Collider2D col = GetComponent<Collider2D>();
        return col != null ? (Vector2)col.bounds.center : (Vector2)transform.position;
    }

    Vector2 GetSteeredDirection(Vector2 desiredDir)
    {
        Vector2 origin = GetCastOrigin();
        RaycastHit2D hit = Physics2D.CircleCast(origin, bossColliderRadius, desiredDir, avoidRayDistance, obstacleLayer);
        Debug.DrawRay(origin, desiredDir * avoidRayDistance, hit.collider == null ? Color.green : Color.red);

        if (hit.collider == null)
        {
            lastSteerSign = 0;
            steerCommitTimer = 0f;
            return desiredDir;
        }

        Vector2 normal = hit.normal;
        Vector2 tangentA = new Vector2(-normal.y, normal.x);
        Vector2 tangentB = new Vector2(normal.y, -normal.x);

        // If still committed to a side from a previous decision, keep using it
        if (steerCommitTimer > 0f)
        {
            steerCommitTimer -= Time.deltaTime;
            Debug.DrawRay(origin, tangentA * avoidRayDistance, Color.blue);
            Debug.DrawRay(origin, tangentB * avoidRayDistance, Color.magenta);
            return lastSteerSign > 0 ? tangentA : tangentB;
        }

        // Commitment expired (or first contact) — make a fresh choice
        float dotA = Vector2.Dot(tangentA, desiredDir);
        float dotB = Vector2.Dot(tangentB, desiredDir);

        if (Mathf.Abs(dotA - dotB) < tieBreakThreshold)
        {
            // Genuine tie (e.g. dead-center alignment) — don't trust float noise, use fixed bias
            lastSteerSign = persistentBiasSign;
        }
        else
        {
            lastSteerSign = dotA >= dotB ? 1 : -1;
        }

        steerCommitTimer = steerCommitDuration;

        Debug.DrawRay(origin, tangentA * avoidRayDistance, Color.blue);
        Debug.DrawRay(origin, tangentB * avoidRayDistance, Color.magenta);

        return lastSteerSign > 0 ? tangentA : tangentB;
    }
}