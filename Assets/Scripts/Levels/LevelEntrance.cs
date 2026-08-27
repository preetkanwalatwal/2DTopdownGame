using UnityEngine;

public class LevelEntrance : MonoBehaviour
{
    public string levelName;
    public int levelID;

    private bool playerInside;
    private LevelUnlocks levelUnlocks;

    private void Start()
    {
        levelUnlocks = FindAnyObjectByType<LevelUnlocks>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInside = false;
    }

    private void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            TryEnterLevel();
        }
    }

    private void TryEnterLevel()
    {
        if (!levelUnlocks.IsUnlocked(levelID))
        {
            Debug.Log("Level Locked");
            return;
        }

        //Debug.Log("Loading " + levelName);
        SceneTransitionManager.Instance.LoadScene(levelName);
    }
}