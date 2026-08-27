using UnityEngine;

public class DestroyMe : MonoBehaviour
{
    public float lifeTime = 3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
