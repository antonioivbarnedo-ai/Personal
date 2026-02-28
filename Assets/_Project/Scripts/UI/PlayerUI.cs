using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;

    [Header("UI Bars")]
    public Image sanityBar;
    public Image staminaBar;
    public Image spiritBar;

    void Update()
    {
        if (sanityBar != null)
            sanityBar.fillAmount = stats.currentSanity / stats.maxSanity;

        if (staminaBar != null)
            staminaBar.fillAmount = stats.currentStamina / stats.maxStamina;

        if (spiritBar != null)
            spiritBar.fillAmount = stats.currentSpirit / stats.maxSpirit;
    }
}