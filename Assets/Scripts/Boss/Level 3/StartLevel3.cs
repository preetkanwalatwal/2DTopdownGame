using UnityEngine;

public class StartLevel3 : MonoBehaviour
{
    public VB3_Controller vb3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // vb3 = GetComponent<VB3_Controller>();    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            vb3.StartFight();
            gameObject.SetActive(false);
        }
    }
}
