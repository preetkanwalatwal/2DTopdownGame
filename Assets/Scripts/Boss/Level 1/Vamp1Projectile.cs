using UnityEngine;
using UnityEngine.AI;

public class Vamp1Projectile : MonoBehaviour
{
    public float damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHealth player = collision.GetComponent<PlayerHealth>();
        if(player != null)
        {
            player.AddDamage((int)damage);
            Destroy(gameObject, 0.5f);
        }

        if(collision.CompareTag("Pillar")) Destroy(gameObject);
    }
}
