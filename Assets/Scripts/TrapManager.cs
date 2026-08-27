using UnityEngine;

public class TrapManager : MonoBehaviour
{
    public GameObject spike;
    public float damage;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerHealth>().AddDamage((int)damage);
        }
    }

}
