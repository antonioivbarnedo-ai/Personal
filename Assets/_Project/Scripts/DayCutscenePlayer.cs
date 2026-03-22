using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DayCutscenePlayer : MonoBehaviour
{
    public static DayCutscenePlayer Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject cutscenePanel;
    [SerializeField] private Image displayImage;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        cutscenePanel.SetActive(false);
    }

    // Called by DayManager
    public void PlayCutscene(DayCutsceneEntry entry, System.Action onComplete)
    {
        StartCoroutine(CutsceneSequence(entry, onComplete));
    }

    IEnumerator CutsceneSequence(DayCutsceneEntry entry, System.Action onComplete)
    {
        // 1. Fade out
        yield return StartCoroutine(SceneTransitionManager.Instance.FadeOut());

        // 2. Show cutscene panel
        cutscenePanel.SetActive(true);

        // 3. Play frames
        foreach (Sprite frame in entry.frames)
        {
            displayImage.sprite = frame;

            // Fade in the panel
            yield return StartCoroutine(FadePanel(0f, 1f, 0.3f));

            // Hold
            yield return new WaitForSeconds(entry.secondsPerFrame);

            // Fade out the panel
            yield return StartCoroutine(FadePanel(1f, 0f, 0.3f));
        }

        // 4. Hide panel
        cutscenePanel.SetActive(false);

        // 5. Teleport player if destination is set
        var destination = DayManager.Instance.GetCutsceneDestination(entry.type);
        if (destination != null)
            SceneTransitionManager.Instance.TeleportPlayerImmediate(destination);

        // 6. Fade back in
        yield return StartCoroutine(SceneTransitionManager.Instance.FadeIn());

        // 7. Fire callback so DayManager continues
        onComplete?.Invoke();
    }

    IEnumerator FadePanel(float from, float to, float duration)
    {
        var canvasGroup = cutscenePanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null) yield break;

        float elapsed = 0f;
        canvasGroup.alpha = from;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = to;
    }
}