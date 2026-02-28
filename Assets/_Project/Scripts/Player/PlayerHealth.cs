using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;

    public bool isDead = false;

    public void TakeSanityDamage(float amount)
    {
        if (isDead) return;

        stats.currentSanity -= amount;
        stats.currentSanity = Mathf.Clamp(stats.currentSanity, 0, stats.maxSanity);

        if (stats.currentSanity <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        Debug.Log("Player Died");

        GameManager.Instance.GameOver();
    }
}