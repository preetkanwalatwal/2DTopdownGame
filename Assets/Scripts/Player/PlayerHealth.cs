using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // public SceneTransitionManager stm;

    public int maxHealth;
    public int currentHealth;
    
    [SerializeField]
    private HealthBarUI healthUI;

    public Animator playerAnim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        // playerAnim = GetComponentInChildren<Animator>();
        healthUI.SetMaxHealth(maxHealth);
        currentHealth = maxHealth;
    }

    public void AddDamage(int damage)
    {
        currentHealth -= damage;
        playerAnim.Play("HurtTree");
        // print("Taking damage " + damage);
        healthUI.SetHealth(damage);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (currentHealth <= 0)
        {
            playerAnim.Play("DeathTree");
            print("You're dead!");
            Application.Quit();
        }
        print("Player health: " + currentHealth);
    }
    
    public void SetMaxHealth(int myHealth)
    {
        print("Setting max health");
        maxHealth = myHealth;
        currentHealth = myHealth;
    }

    
}
