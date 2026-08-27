using System.Collections;
using NUnit.Framework;
using UnityEngine;

public class VB3_Combat : MonoBehaviour
{
    VB3_Movement movement;
    VB3_Animation anim;
    
    // float attackTime = 1.5f;
    // public bool isAttack = false;

    public Transform attackPoint;
    public float hitRadius = 1.5f;
    public LayerMask playerLayer;
    public float damage = 15f;
    
    private float lastAttackTime = -999f;
    public float attackDuration = 1.2f;
    public float attackCooldown = 1f;

    private void Start()
    
    {
        movement = GetComponent<VB3_Movement>();
        anim = GetComponent<VB3_Animation>();
    }
    
    public void BasicAttack()
    {
        Debug.Log("Is attacking");
        if(Time.time < lastAttackTime + attackCooldown)
        {
            return;
        }

        lastAttackTime = Time.time;
        // anim.UpdateAnims(movement.vb3_moveDirection, false);
        anim.PlayAttackAnim();
    }

    public void ApplyDamge()
    {
        // Debug.Log("Applying damage.");
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, hitRadius, playerLayer);
         Debug.Log(hits.Length);
        foreach (Collider2D hit in hits)
        {
            Debug.Log(hit.name);
            if(hit.TryGetComponent<PlayerHealth>(out var health))
            {
                Debug.Log("hit player");
                health.AddDamage((int)damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, hitRadius);
    }
}
