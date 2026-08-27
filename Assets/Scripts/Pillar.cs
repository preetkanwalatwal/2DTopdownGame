using UnityEngine;
using UnityEngine.UI;

public class Pillar : MonoBehaviour
{
    public float maxPillarHealth;
    public float currentPillarHealth;
    public Slider slider;

    public Vamp1Projectile projectile;

    private void Start()
    {
        currentPillarHealth = maxPillarHealth;

        slider.minValue = 0f;
        slider.maxValue = maxPillarHealth;
        slider.value = currentPillarHealth;
    }

    void Update()
    {
        if(currentPillarHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemProjectile")) // || other.CompareTag("PlayerProjectile") Add player prefend too if I want later.
        {
            Debug.Log("Pillar is being damaged");
            
            currentPillarHealth -= projectile.damage;
            slider.value = currentPillarHealth;
            if(currentPillarHealth <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        currentPillarHealth -= damage;
        slider.value = currentPillarHealth;
        if(currentPillarHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
