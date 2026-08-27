using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isOpen = false;

    public void OpenDoor()
    {
        if (isOpen) return;
        isOpen = true;
        Debug.Log("Door opened!");
        gameObject.SetActive(false);
        
    }
    
    public void CloseDoor()
    {
        if (!isOpen) return;
        isOpen = false;
        Debug.Log("Door closed!");
        gameObject.SetActive(true);
    }
}
