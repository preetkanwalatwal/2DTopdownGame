
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyProfile : MonoBehaviour
{
    public Image targetImage = null;        
    public Sprite level1Sprite;
    public Sprite level2Sprite;
    public Sprite level3Sprite;

    void Start()
    {
        string scene = SceneManager.GetActiveScene().name;
        
        switch (scene)
        {
            case "Level1":
                targetImage.sprite = level1Sprite;
                break;

            case "Level2":
                targetImage.sprite = level2Sprite;
                break;

            case "Level3":
                targetImage.sprite = level3Sprite;
                break;
            case "Main":
                break;
        }
    }
}
