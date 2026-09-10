using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MeleeAttack : MonoBehaviour
{
    Scene current_Scene;
    string sceneName;
    
    public float damage;
    public enum WeaponType { Melee, Ranged }
    public WeaponType weaponType;

    void Start()
    {
        
        current_Scene = SceneManager.GetActiveScene();
        sceneName = current_Scene.name;
        // Debug.Log(sceneName);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(sceneName == "Main") return;
       
        if(sceneName == "Level1")
        {
            Vamp1Combat enemy = collision.GetComponent<Vamp1Combat>();
            if(enemy != null)
            {
                enemy.TakeDamage(damage);

                if(weaponType == WeaponType.Melee)
                {
                    Debug.Log("Melee attack hit");
                    enemy.TakeDamage(damage);
                    enemy.Knockback(transform.position);

                }

                if(weaponType == WeaponType.Ranged)
                {
                    Destroy(gameObject);
                }
            }  
        }
        if (sceneName == "Level2")
        {
            VB2_Health enemy = collision.GetComponent<VB2_Health>();
            // VB3_Controller enemyHit
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                
                if (weaponType == WeaponType.Ranged)
                {
                    Destroy(gameObject);
                }
            }
        }
        if(sceneName == "Level3")
        {
            VB3_HealthSystem enemy = collision.GetComponent<VB3_HealthSystem>();

            if(enemy != null)
            {   
                enemy.isHit = true;
                enemy.TakeDamage(damage);
                if(weaponType == WeaponType.Ranged)
                {
                    Destroy(gameObject);
                }
                
            }
        }
    }
}
