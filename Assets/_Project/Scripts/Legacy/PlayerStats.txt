using UnityEngine;
using UnityEngine.UI; // Required for UI

public class PlayerStats : MonoBehaviour
{
    // Singleton: This lets other scripts (like movement) find this script easily
    public static PlayerStats Instance;

    [Header("--- SANITY (Health) ---")]
    public float maxSanity = 100f;
    public float currentSanity;
    public bool isDead = false; // To prevent dying twice

    [Header("--- STAMINA (Sprinting) ---")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaDrainRate = 20f; // Cost to run per second
    public float staminaRegenRate = 10f; // Recovery per second

    [Header("--- SPIRIT (Third Eye) ---")]
    public float maxSpirit = 100f;
    public float currentSpirit;
    public float spiritDrainRate = 15f; // Cost to use eye per second
    public float spiritRegenRate = 2f;  // Slow recovery

    [Header("--- UI References ---")]
    public Image sanityBar;   // Drag your Green/Red bar here
    public Image staminaBar;  // Drag your Yellow bar here
    public Image spiritBar;   // Drag your Purple bar here
    public GameObject gameOverScreen; // Optional: Drag a panel here later

    void Awake()
    {
        // Set up the Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates
        }
    }

    void Start()
    {
        // Reset stats to full at start
        currentSanity = maxSanity;
        currentStamina = maxStamina;
        currentSpirit = maxSpirit;
    }

    void Update()
    {
        if (isDead) return; // Stop logic if we are dead

        // 1. REGENERATE STAMINA
        // We always regen stamina unless it's full. 
        // If the player runs, the "Drain" happens faster than this "Regen".
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
        }

        // 2. REGENERATE SPIRIT
        if (currentSpirit < maxSpirit)
        {
            currentSpirit += spiritRegenRate * Time.deltaTime;
        }

        // 3. CLAMP VALUES (Keep them between 0 and Max)
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        currentSpirit = Mathf.Clamp(currentSpirit, 0, maxSpirit);
        currentSanity = Mathf.Clamp(currentSanity, 0, maxSanity);

        // 4. CHECK FOR DEATH
        if (currentSanity <= 0)
        {
            Die();
        }

        // 5. UPDATE UI
        UpdateUI();
    }

    // --- PUBLIC FUNCTIONS (Other scripts call these) ---

    // Called by FirstPersonController when holding Shift
    public bool UseStamina(float amount)
    {
        if (currentStamina > 5f) // buffer so you can't run with 0.1 stamina
        {
            currentStamina -= amount * Time.deltaTime;
            return true; // "Yes, you can run"
        }
        return false; // "No, you are tired"
    }

    // Called by ThirdEyeAbility when Q is active
    public bool UseSpirit(float amount)
    {
        if (currentSpirit > 0)
        {
            currentSpirit -= amount * Time.deltaTime;
            return true; // "Yes, keep eye open"
        }
        return false; // "No, force eye closed"
    }

    // Call this when a monster hits the player
    public void TakeSanityDamage(float amount)
    {
        currentSanity -= amount;
        Debug.Log("Sanity Hit! Remaining: " + currentSanity);
    }

    // --- INTERNAL FUNCTIONS ---

    void UpdateUI()
    {
        if (sanityBar != null) sanityBar.fillAmount = currentSanity / maxSanity;
        if (staminaBar != null) staminaBar.fillAmount = currentStamina / maxStamina;
        if (spiritBar != null) spiritBar.fillAmount = currentSpirit / maxSpirit;
    }

    void Die()
    {
        isDead = true;
        Debug.Log("GAME OVER");

        // Freeze the game
        Time.timeScale = 0f;

        // Unlock mouse so player can click "Retry" (if you have buttons)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Show Game Over UI if you assigned one
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }
    }
}