using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerClimbing : MonoBehaviour
{
    [Header("Climbing Settings")]
    public float climbSpeed = 3f;
    public float climbDetectDistance = 1f; // How close you must be to the wood
    public LayerMask climbLayer; // Set this to "SpiritWorld"

    [Header("Fall Damage Settings")]
    public float safeFallVelocity = -10f; // Speed before you take damage
    public float damageMultiplier = 2f; // How much it hurts

    private CharacterController controller;
    private PlayerStats stats; // Reference to your Health/Sanity script
    private bool isClimbing = false;
    private float lastVerticalVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        // stats = GetComponent<PlayerStats>(); // Uncomment if you have a stats script
    }

    void Update()
    {
        CheckClimbing();
        CheckFallDamage();

        // Track velocity for fall damage calculation next frame
        lastVerticalVelocity = controller.velocity.y;
    }

    void CheckClimbing()
    {
        // 1. Shoot a short ray forward from chest height
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // 2. Are we facing a "Climbable" object?
        if (Physics.Raycast(ray, out hit, climbDetectDistance, climbLayer))
        {
            if (hit.collider.CompareTag("Climbable"))
            {
                // We are touching the ladder!
                StartClimbing();
                return;
            }
        }

        // If raycast misses, stop climbing
        if (isClimbing) StopClimbing();
    }

    void StartClimbing()
    {
        isClimbing = true;

        // Get Input (W to go UP, S to go DOWN)
        float verticalInput = Input.GetAxis("Vertical");

        // Create climbing movement (No Gravity!)
        Vector3 climbMove = Vector3.up * verticalInput * climbSpeed * Time.deltaTime;

        // Apply movement
        controller.Move(climbMove);

        // Optional: Push player slightly toward the tree so they stick
        controller.Move(transform.forward * 2f * Time.deltaTime);
    }

    void StopClimbing()
    {
        isClimbing = false;
        // Gravity will naturally take over in your main movement script
    }

    void CheckFallDamage()
    {
        // If we just hit the ground AND we were falling fast...
        if (controller.isGrounded && lastVerticalVelocity < safeFallVelocity)
        {
            // Calculate Damage
            float damage = Mathf.Abs(lastVerticalVelocity - safeFallVelocity) * damageMultiplier;
            Debug.Log("Ouch! Took Fall Damage: " + damage);

            // Apply to your stats (Uncomment below)
            // if(stats != null) stats.TakeDamage(damage);
        }
    }
}