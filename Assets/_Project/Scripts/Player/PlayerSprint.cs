using UnityEngine;
using StarterAssets;

public class PlayerSprint : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    [SerializeField] private StarterAssetsInputs input;

    [Header("Speed Settings")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 8f;
    public float staminaDrainRate = 20f;

    private float currentSpeed;

    void Start()
    {
        currentSpeed = walkSpeed;
    }

    void Update()
    {
        HandleSprint();
    }

    void HandleSprint()
    {
        if (input.sprint && stats.currentStamina > 0f)
        {
            currentSpeed = sprintSpeed;
            stats.currentStamina -= staminaDrainRate * Time.deltaTime;
        }
        else
        {
            currentSpeed = walkSpeed;
        }

        stats.currentStamina = Mathf.Clamp(stats.currentStamina, 0, stats.maxStamina);
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
}