using UnityEngine;

public class ReturnToMain : MonoBehaviour
{
    [SerializeField] private int returnSpawnID;
    public bool isBossDefeated = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        isBossDefeated = true;
        SceneTransitionManager.Instance.LoadScene("Main");
        SceneTransitionManager.Instance.SetReturnSpawn(returnSpawnID);
        
    }
}