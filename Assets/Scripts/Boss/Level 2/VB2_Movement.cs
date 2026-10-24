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

    public Vector2 quadrantMovementMin; // Bottom-left corner of qudrant
    public Vector2 quadrantMovementMax; // Top-right corner of quadrant
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

    public void MoveToQuadrant(Vector2 targetPosition)
    {
        VB_moveDirection = (targetPosition - VB_rb.position).normalized;
        VB_isMoving = true;
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

    // === Controlled movement ===
    // - Define x, y range for movement
    // - Split defined area into 4 quadrants
    // - Move to random point in quadrant depending on player position

    public void MoveToRandomPointInQuadrant(Vector2 playerPosition)
    {
        // x, y defined above.

        Vector2 midX = new Vector2((quadrantMovementMin.x + quadrantMovementMax.x) / 2, 0);
        Vector2 midY = new Vector2(0, (quadrantMovementMin.y + quadrantMovementMax.y) / 2);
        playerPosition = new Vector2(playerPosition.x, playerPosition.y);
        // Determine where player is relative to character.
        if(playerPosition.x < midX.x && playerPosition.y < midY.y) // Player in bottom left quadrant
        {
            // Move to top right quadrant
            Vector2 randomPoint = new Vector2(Random.Range(midX.x, quadrantMovementMax.x), Random.Range(midY.y, quadrantMovementMax.y));
            MoveToQuadrant(-randomPoint);
            Debug.Log("Moving to top right quadrant: " + randomPoint);
        }
        else if(playerPosition.x > midX.x && playerPosition.y < midY.y) // Player in bottom right quadrant
        {
            // Move to top left quadrant
            Vector2 randomPoint = new Vector2(Random.Range(quadrantMovementMin.x, midX.x), Random.Range(midY.y, quadrantMovementMax.y));
            MoveToQuadrant(-randomPoint);
            Debug.Log("Moving to top left quadrant: " + randomPoint);
        }
        else if(playerPosition.x < midX.x && playerPosition.y > midY.y) // Player in top left quadrant
        {
            // Move to bottom right quadrant
            Vector2 randomPoint = new Vector2(Random.Range(midX.x, quadrantMovementMax.x), Random.Range(quadrantMovementMin.y, midY.y));
            MoveToQuadrant(-randomPoint);
            Debug.Log("Moving to bottom right quadrant: " + randomPoint);
        }
        else if(playerPosition.x > midX.x && playerPosition.y > midY.y) // Player in top right quadrant
        {
            // Move to bottom left quadrant
            Vector2 randomPoint = new Vector2(Random.Range(quadrantMovementMin.x, midX.x), Random.Range(quadrantMovementMin.y, midY.y));
            MoveToQuadrant(-randomPoint);
            Debug.Log("Moving to bottom left quadrant: " + randomPoint);
        }
    }


    
}
