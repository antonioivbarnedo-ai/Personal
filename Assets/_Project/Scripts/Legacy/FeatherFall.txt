using UnityEngine;

public class FeatherFall : MonoBehaviour
{
    [Header("Settings")]
    public float slowedGravity = -5f; // Normal gravity is usually -9.81. This is "Moon Gravity".
    private CharacterController controller;
    private ThirdEyeAbility thirdEye;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        thirdEye = GetComponent<ThirdEyeAbility>();
    }

    void Update()
    {
        // Logic: Eye CLOSED + In Air = Reduced Gravity
        if (thirdEye != null && !thirdEye.isThirdEyeActive && !controller.isGrounded)
        {
            // If we are falling fast, clamp the speed
            if (controller.velocity.y < slowedGravity)
            {
                // Apply an UPWARD force to counteract real gravity
                // This makes you fall at a constant, slower speed (like a parachute)
                Vector3 drift = Vector3.up * 2f * Time.deltaTime;
                controller.Move(drift);
            }
        }
    }
}