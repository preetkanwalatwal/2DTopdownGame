using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    public GameObject menuParent;
    public GameObject optionsPanel;
    public GameObject resumeButton;
    public GameObject playButton;

    private static bool gameStarted = false;

    void Awake()
    {
        if(gameStarted) menuParent.SetActive(false);
    }
    void Start()
    {
        if(SceneManager.GetActiveScene().name != "Main")
        {
            Debug.Log("PlayGame() in Start()");
            PlayGame();
        } else 
        {
            Time.timeScale = 1f;
        }
    }

    void Update()
    {
        if(!gameStarted) return;

        if (Input.GetKeyDown(KeyCode.P))
        {
            if(Time.timeScale == 1)
            {
                PauseGame();
                resumeButton.SetActive(true);
                playButton.SetActive(false);
            }
            else
            {
                ResumeGame();
                resumeButton.SetActive(false);
                playButton.SetActive(true);
            }
        }
    }

    public void PlayGame()
    {
        Debug.Log("PlayerGame()");
        menuParent.SetActive(false);
        gameStarted = true;
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        menuParent.SetActive(true);
        Debug.Log("Paused");
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        menuParent.SetActive(false);
        Debug.Log("Unpaused");
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game.");
        Application.Quit();
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
        menuParent.SetActive(false);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        menuParent.SetActive(true);
    }
}
