using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    public bool isDead = false;

    // ── Subscribe/unsubscribe to night penalty ───────────────────
    void OnEnable()
    {
        TimeSystem.OnNightPenaltyTick += HandleNightPenalty;
    }

    void OnDisable()
    {
        TimeSystem.OnNightPenaltyTick -= HandleNightPenalty;
    }

    void HandleNightPenalty()
    {
        TakeSanityDamage(5f); // 5 sanity per second at night
    }

    // ── Public methods ───────────────────────────────────────────
    public void TakeSanityDamage(float amount)
    {
        if (isDead) return;

        stats.currentSanity -= amount;
        stats.currentSanity = Mathf.Clamp(stats.currentSanity, 0, stats.maxSanity);

        if (stats.currentSanity <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Player Died");
        GameManager.Instance.GameOver();
    }
}