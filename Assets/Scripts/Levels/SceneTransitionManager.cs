using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    // [SerializeField] private FadeCanvas fadeCanvas;

    private int returnSpawnID = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

       // fadeCanvas = FadeCanvas.Instance;
        // if (fadeCanvas == null)
        //     fadeCanvas = FindFirstObjectByType<FadeCanvas>();

       // Debug.Log($"FadeCanvas Instance: {fadeCanvas.GetInstanceID()}");
    }

    public void SetReturnSpawn(int spawnID)
    {
        returnSpawnID = spawnID;
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        //fadeCanvas.FadeIn();

        yield return new WaitForSeconds(0.5f);

        yield return SceneManager.LoadSceneAsync(sceneName);

        yield return null;

        if (sceneName == "Main")
        {
            SpawnPoint[] spawnPoints =
                FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);

            GameObject player = GameObject.FindGameObjectWithTag("Player");

            foreach (SpawnPoint point in spawnPoints)
            {
                if (point.spawnID == returnSpawnID)
                {
                    player.transform.position = point.transform.position;
                    break;
                }
            }
        }

        //fadeCanvas.FadeOut();
    }
}