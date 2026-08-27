using UnityEngine;

public class VB2_Projectile : MonoBehaviour
{
    public float damage;
    public float speed;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        rb.linearVelocity = transform.right * speed;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHealth player = collision.GetComponent<PlayerHealth>();
        if(player != null)
        {
            player.AddDamage((int)damage);
            Destroy(gameObject, 3f);
        }
    }
}
