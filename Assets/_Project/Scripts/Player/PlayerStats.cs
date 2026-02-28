using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Sanity")]
    public float maxSanity = 100f;
    public float currentSanity;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;

    [Header("Spirit")]
    public float maxSpirit = 100f;
    public float currentSpirit;

    void Awake()
    {
        currentSanity = maxSanity;
        currentStamina = maxStamina;
        currentSpirit = maxSpirit;
    }
}
