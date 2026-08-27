using TMPro;
using UnityEngine;

public class StartBattle : MonoBehaviour
{
    public Vamp1Base vb;
    public Vamp1Movement movement;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        movement.StartFight(other.transform);
        gameObject.SetActive(false);
    }
}
