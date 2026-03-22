using UnityEngine;
using System.Collections;
using TMPro;

// Handles Juan's internal thoughts — subtle text that fades in and out
// Reusable for any atmospheric moment in the game
public class InternalMonologue : MonoBehaviour
{
    public static InternalMonologue Instance { get; private set; }

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI thoughtText;
    [SerializeField] private CanvasGroup thoughtCanvasGroup;

    [Header("Settings")]
    public float fadeInDuration = 0.8f;
    public float holdDuration = 2.5f;
    public float fadeOutDuration = 1.2f;

    private Coroutine _activeCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        thoughtCanvasGroup.alpha = 0f;
        thoughtText.text = "";
    }

    // Show a single thought
    public void ShowThought(string thought)
    {
        if (_activeCoroutine != null)
            StopCoroutine(_activeCoroutine);

        _activeCoroutine = StartCoroutine(DisplayThought(thought));
    }

    // Show one of several thoughts at random — so it doesn't repeat
    public void ShowRandomThought(string[] thoughts)
    {
        if (thoughts == null || thoughts.Length == 0) return;
        ShowThought(thoughts[Random.Range(0, thoughts.Length)]);
    }

    public void ClearThought()
    {
        if (_activeCoroutine != null)
            StopCoroutine(_activeCoroutine);

        StartCoroutine(FadeOut());
    }

    IEnumerator DisplayThought(string thought)
    {
        // Fade out any existing thought first
        yield return StartCoroutine(FadeOut());

        thoughtText.text = thought;

        // Fade in
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            thoughtCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            yield return null;
        }

        thoughtCanvasGroup.alpha = 1f;

        // Hold
        yield return new WaitForSeconds(holdDuration);

        // Fade out
        yield return StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float startAlpha = thoughtCanvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            thoughtCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeOutDuration);
            yield return null;
        }

        thoughtCanvasGroup.alpha = 0f;
        thoughtText.text = "";
    }

    void OnEnable()
    {
        DayManager.OnDayEnding += HandleDayEnding;
        DayManager.OnDayStarted += HandleDayStarted;
        DayManager.OnDangerZone += HandleDangerZone;
    }

    void OnDisable()
    {
        DayManager.OnDayEnding -= HandleDayEnding;
        DayManager.OnDayStarted -= HandleDayStarted;
        DayManager.OnDangerZone -= HandleDangerZone;
    }


    void HandleDangerZone()
    {
        ShowThought("Gabing-gabi na... dapat matulog na ako.");
    }

    void HandleDayEnding(GameDay day)
    {
        ShowThought(day == GameDay.Day1_Arrival
            ? "Pagod na ako… kailangan ko nang matulog."
            : "Matutulog na muna ako.");
    }

    void HandleDayStarted(GameDay day)
    {
        switch (day)
        {
            case GameDay.Day2_Free:
                ShowThought("Umaga na. Anong gagawin ko ngayon?");
                break;
            case GameDay.Day3_KikoGone:
                ShowThought("…may kakaiba. Nasaan si Kiko?");
                break;
        }
    }
}
