using UnityEngine;

public class PlayerStatRegen : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;

    [Header("Regen Settings")]
    public float staminaRegenRate = 10f;
    public float spiritRegenRate = 2f;

    void Update()
    {
        RegenerateStamina();
        RegenerateSpirit();
    }

    void RegenerateStamina()
    {
        if (stats.currentStamina < stats.maxStamina)
        {
            stats.currentStamina += staminaRegenRate * Time.deltaTime;
            stats.currentStamina = Mathf.Clamp(stats.currentStamina, 0, stats.maxStamina);
        }
    }

    void RegenerateSpirit()
    {
        if (stats.currentSpirit < stats.maxSpirit)
        {
            stats.currentSpirit += spiritRegenRate * Time.deltaTime;
            stats.currentSpirit = Mathf.Clamp(stats.currentSpirit, 0, stats.maxSpirit);
        }
    }
}