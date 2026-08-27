using UnityEngine;

public class VB2_Health : MonoBehaviour
{
    public VB2_Controller vB2_Controller;
    public float maxHealth = 1000f;
    public float currentHealth;

    [SerializeField]
    private EnemyHealthUI enemyHealthUI;

    public ReturnToMain returnToMain;
    public GameObject exit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        enemyHealthUI.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(float damage)
    {
        // Debug.Log("Enemy is taking damage: " + damage + ", Health at: " + currentHealth);
        currentHealth -= damage;
        enemyHealthUI.SetHealth(damage);
        if (vB2_Controller.rangeAttack)
        {
            vB2_Controller.currentHits++;
        }
        if(currentHealth <= 0)
        {
            VB2Death();
            exit.SetActive(true);
            returnToMain.isBossDefeated = true;
            LevelUnlocks.Instance.UnlockLevel(3);
        }
        if(currentHealth > 0)
        {
            returnToMain.isBossDefeated = false;
        }
    }

    public void VB2Death()
    {
        Debug.Log("Vampire Boss 2 is dead!");
        Destroy(gameObject);
    }

    public bool rangePhase(float currentHealth)
    {
        if (currentHealth >= 250 && currentHealth <= 400)
        {
            return true;
        }
        if (currentHealth >= 650 && currentHealth <= 800)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
