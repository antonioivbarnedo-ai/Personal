using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Panels")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject playerStatsHUD;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ToggleHUD(bool show)
    {
        if (playerStatsHUD != null)
        {
            playerStatsHUD.SetActive(show);
        }
        else
        {
            Debug.LogWarning("UIManager: playerStatsHUD is not assigned in the Inspector!");
        }
    }
    public void ShowGameOver()
    {
        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}