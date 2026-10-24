using Unity.VisualScripting;
using UnityEngine;

public class VB2_MeleeProjectile : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Movement")]
    public float speed = 2f;

    [Header("Splitting")]
    public GameObject projectilePrefab;
    public float splitDistance = 2f;
    public int maxSplits = 3;
    public float splitAngle = 20f;


    private int currentSplit = 0;
    private Vector2 lastPosition;

    public float lifeTime = 2f;
    public float projectileAge = 0f;
    public VB2_Animation vB2_Animation;

    public float animationProgress = 0f;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        vB2_Animation =  FindAnyObjectByType<VB2_Animation>();

        lastPosition = transform.position;

        // animationProgress = projectileAge / lifeTime;
        // vB2_Animation.UpdateMeleeProjectileAnim(lifeTime, splitDistance, 0f);
        // vB2_Animation.SetProjectileAnimationProgress(animationProgress);
        if(vB2_Animation != null)
        {
            Animator projectileAnimator = GetComponentInChildren<Animator>();
            vB2_Animation.SetProjectileAnimator(projectileAnimator);
            vB2_Animation.SetProjectileAnimationProgress(projectileAnimator, animationProgress);
        }
        else
        {
            Debug.LogWarning("VB2_Animation component not found on the projectile.", this);
        }
    }

    void Update()
    {
        // vB2_Animation.UpdateMeleeProjectileAnim(lifeTime, splitDistance);
        rb.linearVelocity = transform.up * speed;

        projectileAge += Time.deltaTime;
        animationProgress = Mathf.Clamp01(projectileAge / lifeTime);

        // Check how far the projectile has travelled
        float distanceTravelled = Vector2.Distance(
            transform.position,
            lastPosition
        );

        if (distanceTravelled >= splitDistance)
        {
            lastPosition = transform.position;

            if (currentSplit < maxSplits)
            {
                Split();
            }
        }

        Destroy(gameObject, lifeTime / 2f);
    }

    void Split()
    {
        // Create the two new directions
        Quaternion leftRotation =
            transform.rotation * Quaternion.Euler(0f, 0f, splitAngle);

        Quaternion rightRotation =
            transform.rotation * Quaternion.Euler(0f, 0f, -splitAngle);

        // Spawn the two projectiles
        SpawnProjectile(leftRotation);
        SpawnProjectile(rightRotation);

        // Destroy this projectile
        Destroy(gameObject);
    }

    void SpawnProjectile(Quaternion rotation)
    {
        GameObject newProjectile = Instantiate(
            projectilePrefab,
            transform.position,
            rotation
        );

        VB2_MeleeProjectile projectile = newProjectile.GetComponent<VB2_MeleeProjectile>();

        projectile.currentSplit = currentSplit + 1;

        // Pass the current animation position to the new projectile
        // projectile.animationProgress = animationProgress;
        projectile.projectileAge = projectileAge;
        projectile.lifeTime = lifeTime;
        projectile.animationProgress = animationProgress;
    }
        // GameObject newProjectile = Instantiate(
        //     projectilePrefab,
        //     transform.position,
        //     rotation
        // );

        // VB2_MeleeProjectile projectile =
        //     newProjectile.GetComponent<VB2_MeleeProjectile>();

        // projectile.currentSplit = currentSplit + 1;


        // // Get the animation script
        // VB2_Animation animation = newProjectile.GetComponent<VB2_Animation>();


        // // Calculate how far through the current projectile's
        // // lifetime/animation we are.
        // float animationProgress =
        //     projectileAge / lifeTime;


        // // Apply the animation speed and starting frame
        // animation.UpdateMeleeProjectileAnim(
        //     lifeTime,
        //     splitDistance,
        //     animationProgress
        // );
    
}
