using UnityEngine;

public class RotateCenter : MonoBehaviour
{
    public float rotationSpeed = 10f; // degrees per second
    public Transform spawner1;
    public Transform spawner2;
    public Transform spawner3;
    public Transform spawner4;
    
    public GameObject projectile;
    public bool startAttack = false;

    public Transform spawnPoint1;
    public Transform spawnPoint2;
    public Transform spawnPoint3;
    public Transform spawnPoint4;
    void Update()
    {
        // Rotate around Z-axis
        if(!gameObject.activeInHierarchy) return;
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        // spawner1.transform.Rotate(0,0, -rotationSpeed * Time.deltaTime);
        // spawner2.transform.Rotate(0,0, -rotationSpeed * Time.deltaTime);
        // spawner3.transform.Rotate(0,0, -rotationSpeed * Time.deltaTime);
        // spawner4.transform.Rotate(0,0, -rotationSpeed * Time.deltaTime);

        
    }

    public void FireProjectilesFromCenter()
    {
        Debug.Log("Firing Phase1");
        Instantiate(projectile, spawnPoint1.position, Quaternion.Euler(0,0, 80));
        Instantiate(projectile, spawnPoint1.position, Quaternion.Euler(0,0, 90));
        Instantiate(projectile, spawnPoint1.position, Quaternion.Euler(0,0, 100));

        Instantiate(projectile, spawnPoint2.position, Quaternion.Euler(0,0, -80));
        Instantiate(projectile, spawnPoint2.position, Quaternion.Euler(0,0, -90));
        Instantiate(projectile, spawnPoint2.position, Quaternion.Euler(0,0, -100));

        Instantiate(projectile, spawnPoint3.position, Quaternion.Euler(0,0, -10));
        Instantiate(projectile, spawnPoint3.position, Quaternion.Euler(0,0, 0));
        Instantiate(projectile, spawnPoint3.position, Quaternion.Euler(0,0, 10));

        Instantiate(projectile, spawnPoint4.position, Quaternion.Euler(0,0, -170));
        Instantiate(projectile, spawnPoint4.position, Quaternion.Euler(0,0, -180));
        Instantiate(projectile, spawnPoint4.position, Quaternion.Euler(0, 0, -190));
    }
}
