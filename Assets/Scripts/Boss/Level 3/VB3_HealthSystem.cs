using System;
using System.Collections;
using NUnit.Framework;
using Unity.Profiling;
using UnityEngine;

public class VB3_HealthSystem : MonoBehaviour
{
    public float maxHealth = 3000f;
    public float currentHealth;
    public bool isHit;
    public bool isDead;
    VB3_Animation anim;
    VB3_Movement move;
    public ReturnToMain returnToMain;
    public GameObject exit;
    [SerializeField]
    private EnemyHealthUI enemyHealthUI;
    
    void Start()
    {
        move = GetComponent<VB3_Movement>();
        anim = GetComponentInChildren<VB3_Animation>();
        currentHealth = maxHealth;
        enemyHealthUI.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        if (!isHit)
        {
            anim.UpdateHurtAnim(move.vb3_moveDirection, false);
        }
    }

    public void TakeDamage(float damage)
    {
        StartCoroutine(HurtRoutine());
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0 , maxHealth);
        enemyHealthUI.SetHealth(damage);
        
        if(currentHealth <= 0){
            // bool for ifDead player animation before Destroying
            isDead = true;
            move.vb3_isMoving = false;
            move.vb3_moveDirection = Vector2.zero;
            VB3Death();
        }
        if(currentHealth > 0)
        {
            returnToMain.isBossDefeated = false;
        }
    }

    IEnumerator HurtRoutine()
    {
        anim.UpdateHurtAnim(move.vb3_moveDirection, isHit);
        isHit=false;
        yield return new WaitForSeconds(2);
    }

    
    public void VB3Death()
    {
        // Debug.Log("Vampire Boss 3 has been defeat!");
        // Destroy(gameObject);
        exit.SetActive(true);
        returnToMain.isBossDefeated = true;
        anim.PlayDeathAnim(move.vb3_moveDirection, isDead);
        // if(anim.deathFinished) Destroy(gameObject);
    }

    public bool InitiatePhase1(float currentHealth)
    {
        if(currentHealth >= 1000 && currentHealth < 2000)
        {
            return true;
        }
        return false;
    }

    public bool InitiatePhase2(float currentHealth)
    {
        if(currentHealth > 0 && currentHealth < 1000)
        {
            return true;
        }
        return false;
    }
    // IEnumerator DeathRoutine()
    // {
    //     float timer = 0;
    //     while(timer < deathTimer)
    //     {
    //         timer += Time.deltaTime;

    //     }
    // }

}
