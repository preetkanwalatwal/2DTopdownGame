using TMPro;
using UnityEngine;

public class LeverPuzzleController : MonoBehaviour
{
    public Lever[] levers;
    public Door door;
    public Lever.LeverState[] correctCombination;
    public bool puzzleSolved = false;

    // Update is called once per frame
    void Update()
    {
        if (puzzleSolved) return;
        bool allCorrect = true;

        for (int i = 0; i < levers.Length; i++)
        {
            if (levers[i].currentState != correctCombination[i])
            {
                allCorrect = false;
                break;
            }
        }

        if (allCorrect)
        {
            puzzleSolved = true;
            door.OpenDoor();    
        }    
    }
}
