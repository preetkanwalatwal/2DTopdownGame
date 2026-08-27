using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.WindowsMR.Input;

public class Vamp1Base : MonoBehaviour
{ 
    public float vamp1MaxHealth;
    public float vamp1CurrentHealth;
    public float rotationSpeed = 5f;
    private Vector2 aimDirection;

    public bool lookingAtPlayer = false;

    Vamp1Movement vm;
    Animator vampAnim;
    public Transform myTarget;
    public Transform myAimBase;

    void Start()
    {
        vampAnim = GetComponentInChildren<Animator>();
        vm = GetComponent<Vamp1Movement>();
        vamp1CurrentHealth = vamp1MaxHealth;
        
    }
    void Update()
    {
        if (myTarget == null) return;

        UpdateVampAnimation();
        LookAtPlayer();

    }

    void LookAtPlayer()
    {
        if (!lookingAtPlayer) return;
        // Get the direction toward the target
        aimDirection = myTarget.position - myAimBase.position;

        // Compute the target rotation
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        // Smoothly rotate toward the target
        myAimBase.rotation = Quaternion.RotateTowards(
            myAimBase.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime * 25f // multiply by 100 for nicer tuning
        );
    }
    
    void UpdateVampAnimation()
    {
        vampAnim.SetFloat("MoveX", vm.moveDirection.x);
        vampAnim.SetFloat("MoveY", vm.moveDirection.y);
        vampAnim.SetFloat("Speed", vm.moveDirection.sqrMagnitude);

        if(vm.moveDirection != Vector2.zero)
        {
            vampAnim.SetFloat("LastMoveX", vm.moveDirection.x);
            vampAnim.SetFloat("LastMoveY", vm.moveDirection.y);
        }
    }
}
