using Unity.VisualScripting;
using UnityEngine;

public class Lever : MonoBehaviour
{
    public enum LeverState
    {
        Left = -1,
        Middle = 0,
        Right = 1
    }
    public LeverState currentState = LeverState.Middle;

    // Sprites at each state
    public Sprite leftSprite;
    public Sprite middleSprite;
    public Sprite rightSprite;

    private SpriteRenderer sr;

    // Animations for each state
    // public Animator levelAnim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // levelAnim = GetComponent<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
        // UpdateVisual();
    }

    public void TurnLeft()
    {
        Debug.Log("Left");
        currentState = LeverState.Left;
        UpdateVisual();
    }

    public void TurnRight()
    {
        Debug.Log("Right");
        currentState = LeverState.Right;
        UpdateVisual();
    }

    public void ResetToMiddle()
    {
        Debug.Log("Middle");
        currentState = LeverState.Middle;
        UpdateVisual();
    }
    
    void UpdateVisual()
    {
        if (!sr)
        {
            sr = GetComponentInChildren<SpriteRenderer>();
        }

        switch (currentState)
        {
            case LeverState.Left:
                sr.sprite = leftSprite;
                break;
            case LeverState.Middle:
                sr.sprite = middleSprite;
                break;
            case LeverState.Right:
                sr.sprite = rightSprite;
                break;
        }
    }
}
