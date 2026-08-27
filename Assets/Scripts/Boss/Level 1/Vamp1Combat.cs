using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vamp1Combat : MonoBehaviour
{
    [Header("References")]
    Vamp1Base vb;
    Vamp1Movement vm;
    Animator anim;
    Rigidbody2D rb;

    public PlayerHealth playerHealth;
    public ReturnToMain returnToMain; 
    
    public GameObject exit;
    public GameObject door;
    public bool chasePlayer = false;

    public float basicAttackCooldown = 4f;
    public float basicAttackTimer = 0f;

    [Header("Health Threshold Settings")]
    public int threshold;
    public int specialPercentInterval = 10;
    int lastSpecialThreshold = 0;

    [Header("Ranged Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;

    [Header("Melee Settings")]
    public float meleeDamage = 20f;
    public float meleeRange = 1.5f;

    [Header("Dash Settings")]
    public float dashDamage = 100f;
    public float dashHitRadius = 0.6f;

    public float dashDistance = 5f;
    public float dashOvershoot = 1.5f;
    public float dashMinOvershoot = 0.5f;
    public float closeRangeThreshold = 2f;

    public float dashDuration = 0.2f;
    public float dashPauseDuration  = 0.3f;
    public float dashPauseDurationClose = 0.8f;

    public bool isDashing = false; 
    public bool isAttacking = false;

    [Header("Dash Boundaries")]
    public LayerMask wallLayer;
    public Vector2 mapBoundsMin;
    public Vector2 mapBoundsMax;
    public float wallCheckBuffer = 0.3f;

    [Header("Layer Masks")]
    public LayerMask playerLayer;
    public LayerMask pillarLayer;

    [SerializeField] private EnemyHealthUI enemyHealthUI;
    
    void Start()
    {
        vb = GetComponent<Vamp1Base>();
        vm = GetComponent<Vamp1Movement>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        enemyHealthUI.SetMaxHealth(vb.vamp1MaxHealth);
    }

    void Update()
    {
        if (!vm.agro) return;

        basicAttackTimer += Time.deltaTime; // Basic Attack Timer
        if (basicAttackTimer >= basicAttackCooldown)
        {
            BasicAttack(); // Projectile Attack
            basicAttackTimer = 0f;
        }

        // Special Attack Check
        CheckSpecialAttack();

        // If melee mode is active → move toward player
        if (chasePlayer) vm.HandleAgro();
    }

    void CheckSpecialAttack()
    {
        int maxHP = (int)vb.vamp1MaxHealth;
        float percentLost = 1 - (vb.vamp1CurrentHealth / maxHP);
        threshold = Mathf.FloorToInt(percentLost * 100f);

        if (threshold >= lastSpecialThreshold + specialPercentInterval)
        {
            isAttacking = true;
            lastSpecialThreshold = threshold;
            DoSpecialAttack(Random.Range(1, 3));  
        }
    }

    // Cycle through attacks
    void DoSpecialAttack(int attack)
    {
        if (attack == 1) DashAttack();
        if (attack == 2) MeleeAttack();
    }

    // Basic attack - called every frame.
    void BasicAttack()
    {
        // Mage projectile
        if (vb.myTarget == null) return;
        Vector2 dir = (vb.myTarget.position - transform.position).normalized;
        GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D brb = bullet.GetComponent<Rigidbody2D>();
        brb.linearVelocity = dir * projectileSpeed;
    }

    void MeleeAttack()
    {
        Debug.Log("Melee");
        chasePlayer = true;  // Start chasing the player

        // Check if in melee range
        float dist = Vector2.Distance(transform.position, vm.target.position);
        Debug.Log(dist);
        if(dist > meleeRange)
        {
            StartCoroutine(ChaseUntilMelee());
            return;
        }
        if (dist <= meleeRange)
        {
            // Damage player
            Debug.Log("Attempting to damage player");
            anim.SetTrigger("Attack");
            vm.StopMoving();
            playerHealth.AddDamage((int)meleeDamage);
            
        }
        chasePlayer = false;
        isAttacking = false;
    }

    IEnumerator ChaseUntilMelee()
    {
        Debug.Log("Coroutine");
        chasePlayer = true;

        while (Vector2.Distance(transform.position, vm.target.position) > meleeRange)
        {
            vm.MeleeRun();
            yield return null; // wait one frame
        }

        chasePlayer = false;
        MeleeAttack(); // or directly deal damage
    }

    IEnumerator Dash(Vector2 direction)
    {
        isDashing = true;
        chasePlayer = false;
        vm.StopMoving();

        float initialDist = vm.target != null
            ? Vector2.Distance(transform.position, vm.target.position)
            : float.MaxValue;
        bool isCloseStart = initialDist < closeRangeThreshold;

        float pauseDuration = isCloseStart ? dashPauseDurationClose : dashPauseDuration;
        yield return new WaitForSeconds(pauseDuration);

        Vector2 startPos = rb.position;
        float distToPlayer = 0f;
        if (vm.target != null)
        {
            Vector2 toPlayer = (Vector2)vm.target.position - startPos;
            distToPlayer = toPlayer.magnitude;
            direction = toPlayer.normalized;
        }

        float overshoot = isCloseStart ? dashMinOvershoot : dashOvershoot;
        float desiredDistance = Mathf.Min(distToPlayer + overshoot, dashDistance);

        const float minValidHitDistance = 0.1f;

        // Track what's already been hit this dash — declared early so the
        // pillar-cancel block below can register its direct hit too
        HashSet<Collider2D> alreadyHit = new HashSet<Collider2D>();

        // Wall check
        RaycastHit2D wallHit = Physics2D.CircleCast(startPos, dashHitRadius, direction, desiredDistance, wallLayer);
        if (wallHit.collider != null && wallHit.distance > minValidHitDistance)
        {
            desiredDistance = Mathf.Max(0f, wallHit.distance - wallCheckBuffer);
        }

        // Pillar check — cancels dash short if a pillar is between boss and player
        RaycastHit2D pillarHit = Physics2D.CircleCast(startPos, dashHitRadius, direction, distToPlayer, pillarLayer);
        bool cancelledByPillar = pillarHit.collider != null && pillarHit.distance > minValidHitDistance;

        if (cancelledByPillar)
        {
            // Take the MORE restrictive of the wall clip and the pillar clip,
            // don't let the pillar distance override a shorter wall clip
            desiredDistance = Mathf.Min(desiredDistance, Mathf.Max(0f, pillarHit.distance - wallCheckBuffer));

            Pillar pillar = pillarHit.collider.GetComponent<Pillar>();
            if (pillar != null)
            {
                pillar.TakeDamage(dashDamage);
            }
            alreadyHit.Add(pillarHit.collider); // prevent the in-loop sweep from double-hitting it
        }

        Vector2 endPos = startPos + direction * desiredDistance;
        endPos.x = Mathf.Clamp(endPos.x, mapBoundsMin.x, mapBoundsMax.x);
        endPos.y = Mathf.Clamp(endPos.y, mapBoundsMin.y, mapBoundsMax.y);

        float fullDistance = Mathf.Min(distToPlayer + overshoot, dashDistance);
        float distanceRatio = fullDistance > 0f ? Mathf.Clamp01(desiredDistance / fullDistance) : 1f;
        float effectiveDuration = Mathf.Max(0.05f, dashDuration * distanceRatio);

        float elapsed = 0f;
        while (elapsed < effectiveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / effectiveDuration);
            rb.MovePosition(Vector2.Lerp(startPos, endPos, t));

            // Check player and pillar hits separately
            Collider2D[] playerHits = Physics2D.OverlapCircleAll(rb.position, dashHitRadius, playerLayer);
            foreach (Collider2D hit in playerHits)
            {
                if (alreadyHit.Contains(hit)) continue;
                alreadyHit.Add(hit);
                if (!cancelledByPillar)
                {
                    playerHealth.AddDamage((int)dashDamage);
                }
            }

            Collider2D[] pillarHits = Physics2D.OverlapCircleAll(rb.position, dashHitRadius, pillarLayer);
            foreach (Collider2D hit in pillarHits)
            {
                if (alreadyHit.Contains(hit)) continue;
                alreadyHit.Add(hit);

                Pillar pillar = hit.GetComponent<Pillar>();
                if (pillar != null)
                {
                    Debug.Log("Pillar hit during dash");
                    pillar.TakeDamage(dashDamage); 
                }
            }

            yield return null;
        }
        rb.position = endPos;

        rb.linearVelocity = Vector2.zero;
        isDashing = false;
        isAttacking = false;
    }

    // DashAttack
    void DashAttack()
    {
        if (vm.target == null)
        {
            isAttacking = false;
            return;
        }

        Debug.Log("Dash");
        isAttacking = true;
        chasePlayer = false;
        Vector2 dir = (vm.target.position - transform.position).normalized;
        StartCoroutine(Dash(dir));
    }
    
    public void TakeDamage(float damage)
    {
        vb.vamp1CurrentHealth -= damage;
        vb.vamp1CurrentHealth = Mathf.Clamp(vb.vamp1CurrentHealth, 0 , vb.vamp1MaxHealth);
        enemyHealthUI.SetHealth(damage);
        if(vb.vamp1CurrentHealth <= 0)
        {
            // Death + animation 
            exit.SetActive(true);
            door.SetActive(false);
            returnToMain.isBossDefeated = true;
            LevelUnlocks.Instance.UnlockLevel(2);
            Destroy(gameObject);
        }
        if(vb.vamp1CurrentHealth > 0)
        {
            returnToMain.isBossDefeated = false;
        }
    }
}