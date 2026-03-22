using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;



// Scene names as constants — no raw strings anywhere else in the project
public static class SceneName
{
    public const string MAIN_MENU = "MainMenu";
    public const string ARRIVAL_CUTSCENE = "ArrivalCutscene";  // 2D
    public const string GREYBOXING = "Greyboxing";        // 3D
    public const string KIKO_HOUSE = "KikoHouse";         // 3D (future)
}

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("Player Reference")]
    [SerializeField] private Transform playerTransform;
    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup fadeCanvas; // Black fullscreen panel
    public float fadeDuration = 0.5f;

    void Awake()
    {
        // Proper singleton — survives all scene loads
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ── Public API ───────────────────────────────────────────────

    public void TeleportPlayerImmediate(Transform destination)
    {
        var cc = playerTransform.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        playerTransform.position = destination.position;
        playerTransform.rotation = destination.rotation;

        if (cc != null) cc.enabled = true;
    }

    // Simple named load (replaces old LoadScene)
    public void GoTo(string sceneName)
    {
        StartCoroutine(Transition(sceneName));
    }

    // Convenience wrappers — every scene has a named method
    // so no one needs to remember string names
    public void GoToMainMenu() => GoTo(SceneName.MAIN_MENU);
    public void GoToArrivalCutscene() => GoTo(SceneName.ARRIVAL_CUTSCENE);
    public void GoToGameplay() => GoTo(SceneName.GREYBOXING);

    

    // ── Transition coroutine ─────────────────────────────────────
    IEnumerator Transition(string sceneName)
    {
        // 1. Fade OUT to black
        yield return StartCoroutine(Fade(0f, 1f));

        // 2. Load the scene
        yield return SceneManager.LoadSceneAsync(sceneName);

        // 3. Fade IN from black
        yield return StartCoroutine(Fade(1f, 0f));
    }

    IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        fadeCanvas.gameObject.SetActive(true);
        fadeCanvas.alpha = from;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime; // unscaled so it works when paused
            fadeCanvas.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }

        fadeCanvas.alpha = to;

        // Hide the canvas when fully transparent so it doesn't block raycasts
        if (to == 0f)
            fadeCanvas.gameObject.SetActive(false);
    }

    public IEnumerator FadeOut()
    {
        yield return StartCoroutine(Fade(0f, 1f));
    }

    public IEnumerator FadeIn()
    {
        yield return StartCoroutine(Fade(1f, 0f));
    }

    public void TeleportPlayer(Transform destination)
    {
        StartCoroutine(TeleportSequence(destination));
    }

    IEnumerator TeleportSequence(Transform destination)
    {
        yield return StartCoroutine(Fade(0f, 1f));

        // Disable CharacterController so it doesn't fight the position change
        var cc = playerTransform.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        playerTransform.position = destination.position;
        playerTransform.rotation = destination.rotation;

        if (cc != null) cc.enabled = true;

        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(Fade(1f, 0f));
    }
}