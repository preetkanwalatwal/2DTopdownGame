using TMPro;
using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    public TMP_Dropdown aimDropdown;

    void Start()
    {
        aimDropdown.value = PlayerPrefs.GetInt("AimMode", 0); // Default to Controller if not set   
    }

    public void SetAimMode(int mode)
    {
        Debug.Log("SetAimMode is being called from OptionsManager");
        PlayerPrefs.SetInt("AimMode", mode);
        PlayerPrefs.Save();
    }
}
