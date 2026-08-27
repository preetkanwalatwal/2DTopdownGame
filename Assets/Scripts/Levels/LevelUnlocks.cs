using UnityEngine;

public class LevelUnlocks : MonoBehaviour
{
    public static LevelUnlocks Instance;

    private bool[] unlocked = new bool[4];

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        UnlockLevel(1);
        
    }

    public bool IsUnlocked(int id)
    {
        return unlocked[id];
    }

    public void UnlockLevel(int id)
    {
        unlocked[id] = true;
        Debug.Log($"Unlocked level {id}");
    }

    public int SpawnPoint()
    {
        if (IsUnlocked(3))
        {
            return 3;
        }
        else if (IsUnlocked(2))
        {
            return 2;
        }
        else if (IsUnlocked(1))
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

}
