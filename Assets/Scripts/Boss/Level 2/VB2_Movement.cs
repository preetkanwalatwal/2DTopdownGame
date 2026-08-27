using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VB2_Movement : MonoBehaviour
{
    public Vector2 VB_moveDirection;
    public bool VB_isMoving = false;
    public float VB_moveSpeed = 5f;
    public float VB_flySpeed = 10f;

    public Rigidbody2D VB_rb;

    public Transform[] corners;
    public Vector2 chosenCorner;
    public bool hasReachedCorner = false;
    public bool hasChosenTarget = false;
    public bool moveToCenter = false;
    public int travelTo;
    void Awake()
    {
        VB_rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (VB_isMoving)
        {
            VB_rb.linearVelocity = VB_moveDirection * VB_moveSpeed;
        }
        else
        {
            VB_rb.linearVelocity = Vector2.zero;
        }
    }

    public void MoveTo(Vector2 playerTarget)
    {
        // Move to player
        VB_moveDirection = (playerTarget - VB_rb.position).normalized;
        VB_isMoving = true;
        // Debug.Log("Is moving to player");
        // Distance to stop at

        
    }

    public void MoveAway(Vector2 newPosition)
    {
        Debug.Log("Moving away");
        Vector2 moveAwayTo = (VB_rb.position - newPosition).normalized;
        VB_isMoving = true;
    }

    public void HoverBy(Transform playerTarget)
    {
        
    }

    public void FlyToCorner(bool isFly)
    {
        // Debug.Log(isFly);
        if (isFly)
        {
            if (!hasChosenTarget)
            {
                travelTo = Random.Range(0, 3);
                chosenCorner = corners[travelTo].transform.position;
                hasChosenTarget = true;
                Debug.Log("Chosen corner: " + corners[travelTo].name);
            }

            transform.position = Vector2.MoveTowards(transform.position, chosenCorner, VB_flySpeed * Time.deltaTime);
            // Debug.Log(chosenCorner);    
            if(transform.position == corners[travelTo].transform.position) hasReachedCorner = true;    
        }


    }

    public int ChooseNewCorner(int currentCornerIndex, bool rangeBool)
{
    // Build a list of allowed corners
    List<int> possibleCorners = new List<int>();

    for (int i = 0; i < corners.Length; i++)
    {
        if (i != currentCornerIndex)   // exclude current corner
            possibleCorners.Add(i);
    }

    // If rangeBool affects selection, modify this logic
    if (rangeBool)
    {
        // Example: filter again based on some condition
        // This is just a placeholder until you define what rangeBool means
    }

    // Pick a random corner from the valid list
    int newCornerIndex = possibleCorners[Random.Range(0, possibleCorners.Count)];

    Debug.Log("Choosing NEW corner: " + corners[newCornerIndex].name);

    return newCornerIndex;
}
    
    public void FlyToCenter(bool moveToCenter)
    {
        if(moveToCenter) // If out of range attack threshold 
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(0, 0), VB_flySpeed * Time.deltaTime);
        }
    }

    public void StopMove()
    {
        VB_moveDirection = Vector2.zero;
        VB_isMoving = false;
    }

    
}
