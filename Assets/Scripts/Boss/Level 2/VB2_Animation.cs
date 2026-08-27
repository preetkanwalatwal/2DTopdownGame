using UnityEngine;

public class VB2_Animation : MonoBehaviour
{
    public bool isAttacking = false;

    Animator v2Anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        v2Anim = GetComponentInChildren<Animator>();
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
}
