using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject ui;

    void Start()
    {
        if(SceneManager.GetActiveScene().name == "Main")
        {
            ui.SetActive(false);
        }
    }


}
