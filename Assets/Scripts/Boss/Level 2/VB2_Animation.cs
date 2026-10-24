using UnityEngine;

public class VB2_Animation : MonoBehaviour
{
    public bool isAttacking = false;

    Animator v2Anim;
    public Animator meleeProjectileAnim;

    private float projectileAnimationProgress = 0f;
    private bool hasProjectileAnimationProgress = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        v2Anim = GetComponentInChildren<Animator>();
        // meleeProjectileAnim = GetComponentInChildren<Animator>();
    }

    public void UpdateAnimation(Vector2 moveDir, bool isRunOn, bool isAttacking)
    {
        v2Anim.SetFloat("MoveX", moveDir.x);
        v2Anim.SetFloat("MoveY", moveDir.y);
        v2Anim.SetFloat("Speed", moveDir.sqrMagnitude);
        v2Anim.SetBool("IsRunning", isRunOn);
        
        v2Anim.SetBool("IsAttacking", isAttacking);
        
        if(moveDir != Vector2.zero)
        {
            v2Anim.SetFloat("LastMoveX", moveDir.x);
            v2Anim.SetFloat("LastMoveY", moveDir.y);
        }
    }

    public void UpdateCombatAnimation(Vector2 faceDir)
    {
        // Debug.Log("This method is being called and " + isAttacking);
        v2Anim.SetBool("IsAttacking", isAttacking);

        if (faceDir != Vector2.zero)
        {
            v2Anim.SetFloat("LastMoveX", faceDir.x);
            v2Anim.SetFloat("LastMoveY", faceDir.y);
        }
    }
    
    public void CornerIdle(int corner)
    {
        v2Anim.SetBool("IsRunning", false);
        if(corner == 0 || corner == 1)
        {
            v2Anim.SetFloat("LastMoveX", 0);
            v2Anim.SetFloat("LastMoveY", -1);
            v2Anim.SetBool("IsAttacking", true);
        }
        if(corner == 2 || corner == 3)
        {
            v2Anim.SetFloat("LastMoveX", 0);
            v2Anim.SetFloat("LastMoveY", 1);
            v2Anim.SetBool("IsAttacking", true);
        }
    }
    public void UpdateFlyAnimation(bool isRange)
    {
        v2Anim.SetBool("IsRunning", isRange);
    }

    public void SetProjectileAnimationProgress(Animator projectileAnimation, float progress)
    {
        if(projectileAnimation == null) return;

        projectileAnimationProgress = Mathf.Clamp01(progress);
        hasProjectileAnimationProgress = true;

        if(meleeProjectileAnim != null)
        {
            ApplyProjectileAnimation();
        }
    }

    void ApplyProjectileAnimation()
    {
        if(meleeProjectileAnim == null) return;

        meleeProjectileAnim.Play(
            "VB2_MeleeSouth",
            0,
            projectileAnimationProgress
        );

        meleeProjectileAnim.Update(0f);
    }

    public void UpdateMeleeProjectileAnim(
    float time,
    float distance,
    float animProgress)
    {
        if (meleeProjectileAnim == null || time <= 0f)
            return;

        AnimatorStateInfo stateInfo =
            meleeProjectileAnim.GetCurrentAnimatorStateInfo(0);

        float remainingProgress = 1f - Mathf.Clamp01(animProgress);

        float remainingAnimationTime =
            stateInfo.length * remainingProgress;

        if (remainingAnimationTime > 0f)
        {
            meleeProjectileAnim.speed =
                remainingAnimationTime / time;
        }

        meleeProjectileAnim.Play(
            "VB2_MeleeSouth",
            0,
            Mathf.Clamp01(animProgress)
        );

        meleeProjectileAnim.Update(0f);
    }

    public void SetProjectileAnimator(Animator projectileAnimator)
    {
        meleeProjectileAnim = projectileAnimator;
    }
    
    // public void UpdateMeleeProjectileAnim(float time, float distance, float animProgress)
    // {
    //     // Calculate animation speed over its life time.
    //     meleeProjectileAnim.speed = distance / time;

    //     // Continue from the parents animation position.
    //     AnimatorStateInfo stateInfo = meleeProjectileAnim.GetCurrentAnimatorStateInfo(0);

    //     float animationLength = stateInfo.length;

    //     // if(animationLength > 0f)
    //     // {
    //     //     meleeProjectileAnim.Play(stateInfo.fullPathHash, 0, animProgress / animationLength);
    //     //     meleeProjectileAnim.Update(0f); 
    //     // }
    //     meleeProjectileAnim.Play("VB2_MeleeSouth", 0, animProgress / animationLength);
    //     meleeProjectileAnim.Update(0f);
    // }
}
