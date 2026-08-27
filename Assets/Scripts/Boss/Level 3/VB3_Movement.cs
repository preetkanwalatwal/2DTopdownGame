using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class VB3_Movement : MonoBehaviour
{
    public Vector2 vb3_moveDirection;
    public bool vb3_isMoving = false;
    public float vb3_moveSpeed = 5f;

    Rigidbody2D vb3_Rb;

    public Transform target; // Player

    // The 4 possible exits
    public Transform northExit;
    public Transform southExit;
    public Transform eastExit;
    public Transform westExit;
    public Transform center;
    
    public Transform chosenExit = null;
    public bool hasLeftCenter = false;
    public bool hasLeftInitial = false;
    VB3_HealthSystem vB3_HealthSystem;
    VB3_Combat vB3_Combat;
    VB3_Controller vB3_Controller;

    void Awake()
    {
        vB3_Controller = GetComponent<VB3_Controller>();
        vB3_Combat = GetComponent<VB3_Combat>();
        vB3_HealthSystem = GetComponent<VB3_HealthSystem>();
        vb3_Rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (vb3_isMoving && !vB3_HealthSystem.isDead && !vB3_Controller.isAttacking)
            vb3_Rb.linearVelocity = vb3_moveDirection * vb3_moveSpeed;
        else
            vb3_Rb.linearVelocity = Vector2.zero;

        // Stop movement when exit reached
        if (vb3_isMoving && chosenExit != null)
        {
            float dist = Vector2.Distance(transform.position, chosenExit.position);
            if (dist <= 0.15f)
            {
                // StopMove();
                hasLeftCenter = true;  // <==== prevents more transitions
            }
        }
    }

    public void LeaveCenter()
    {
        // Prevent calling again after one successful transition
        if (hasLeftCenter) return;

        // Choose exit only once
        if (chosenExit == null)
        {
            Vector2 facingDirection = (target.position - transform.position).normalized;

            // Identify dominant axis
            if (Mathf.Abs(facingDirection.x) > Mathf.Abs(facingDirection.y))
            {
                // EAST or WEST
                chosenExit = facingDirection.x > 0 ? eastExit : westExit;
            }
            else
            {
                // NORTH or SOUTH
                chosenExit = facingDirection.y > 0 ? northExit : southExit;
            }
        }

        // Start moving toward chosen exit
        vb3_moveDirection = (chosenExit.position - transform.position).normalized;

        if(Vector2.Distance(transform.position, chosenExit.position) > 0.1f)
        {
            hasLeftInitial = true;
        }
        else
        {
            hasLeftInitial = false;
        }
        vb3_isMoving = true;
    }

    public bool IsAtCenter()
    {
        vb3_moveDirection = (center.position - transform.position).normalized;
        if(Vector2.Distance(center.position, transform.position) < 0.1)
        {
            StopMove();
            vb3_moveDirection = Vector2.zero;
            return true;
        } 
        return false;
    }

    public void ChasePlayer(Vector2 player)
    {
        if (hasLeftCenter)
        {
            vb3_moveDirection = (player - vb3_Rb.position).normalized;
            vb3_isMoving = true;    
        }
        
    }

    public void StopMove()
    {
        vb3_isMoving = false;
        // vb3_Rb.linearVelocity = Vector2.zero;
    }
}
