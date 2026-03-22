using UnityEngine;
using UnityEngine.Rendering;

public class MoundUnease : MonoBehaviour
{
    [Header("Detection")]
    public float outerRadius = 8f;   // triggers internal monologue
    public float innerRadius = 3f;   // triggers sanity drain

    [Header("Sanity Drain")]
    public float sanityDrainRate = 3f;      // per second at inner range
    private float _drainCooldown = 0f;

    [Header("Post Process")]
    [SerializeField] private Volume uneasePPVolume; // separate low-weight volume
    public float maxVignetteWeight = 0.4f;

    [Header("Player Reference")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Monologue Lines — outer range")]
    // These are the only hardcoded strings allowed — atmospheric flavor only
    // If you want these in a SO later, easy to move
    private readonly string[] _outerThoughts = new string[]
    {
        "…may kakaibang pakiramdam.",
        "Parang may nagmamasid sa akin.",
        "Ang puno ng lupa dito… hindi parang normal.",
        "Mas mabuti sigurong lumayo dito.",
        "…bakit parang nag-iinit ang aking balat dito?"
    };

    [Header("Monologue Lines — inner range (more urgent)")]
    private readonly string[] _innerThoughts = new string[]
    {
        "Hindi maganda ito. Kailangan ko lumayo.",
        "…nararamdaman ko ang galit ng lugar na ito.",
        "Tabi-tabi po… tabi-tabi po…",
        "May natutulog dito. Huwag ko itong gambalain."
    };

    // State tracking
    private bool _wasInOuterRange = false;
    private bool _wasInInnerRange = false;
    private float _thoughtCooldown = 0f;
    private float _thoughtInterval = 8f; // seconds between monologue triggers

    // Only active before Nuno encounter — after meeting him, remove the unease
    // (you've acknowledged him, the tension is different now)
    private bool _isActive = true;

    void OnEnable()
    {
        QuestManager.OnQuestStateChanged += HandleQuestChange;
    }

    void OnDisable()
    {
        QuestManager.OnQuestStateChanged -= HandleQuestChange;
    }

    void HandleQuestChange(QuestState old, QuestState next)
    {
        // After the Nuno encounter is resolved, disable the passive unease
        // The encounter itself replaces the ambient dread
        if (next >= QuestState.NUNO_ENCOUNTER)
        {
            _isActive = false;
            if (uneasePPVolume != null)
                uneasePPVolume.weight = 0f;
            InternalMonologue.Instance?.ClearThought();
        }
    }

    void Update()
    {
        if (!_isActive) return;
        if (playerTransform == null) return;

        float dist = Vector3.Distance(transform.position, playerTransform.position);

        bool inOuter = dist <= outerRadius;
        bool inInner = dist <= innerRadius;

        HandleVignette(dist);
        HandleMonologue(inOuter, inInner);
        HandleSanityDrain(inInner);

        _wasInOuterRange = inOuter;
        _wasInInnerRange = inInner;
    }

    void HandleVignette(float dist)
    {
        if (uneasePPVolume == null) return;

        // Vignette gets stronger the closer you are
        // Zero outside outer range, max at inner range
        float t = 1f - Mathf.Clamp01((dist - innerRadius) / (outerRadius - innerRadius));
        uneasePPVolume.weight = Mathf.Lerp(0f, maxVignetteWeight, t);
    }

    void HandleMonologue(bool inOuter, bool inInner)
    {
        _thoughtCooldown -= Time.deltaTime;

        // Just entered outer range — immediate first thought
        if (inOuter && !_wasInOuterRange)
        {
            InternalMonologue.Instance?.ShowRandomThought(_outerThoughts);
            _thoughtCooldown = _thoughtInterval;
            return;
        }

        // Repeated thoughts on cooldown while inside range
        if (inOuter && _thoughtCooldown <= 0f)
        {
            // Inner range gets the more urgent lines
            string[] pool = inInner ? _innerThoughts : _outerThoughts;
            InternalMonologue.Instance?.ShowRandomThought(pool);
            _thoughtCooldown = _thoughtInterval;
        }

        // Left range — clear the thought
        if (!inOuter && _wasInOuterRange)
        {
            InternalMonologue.Instance?.ClearThought();
        }
    }

    void HandleSanityDrain(bool inInner)
    {
        if (!inInner) return;

        _drainCooldown -= Time.deltaTime;
        if (_drainCooldown > 0f) return;

        _drainCooldown = 1f; // once per second
        playerHealth?.TakeSanityDamage(sanityDrainRate);
    }

    // Draw the detection radii in the Scene view so you can see them
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, outerRadius);

        Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
        Gizmos.DrawSphere(transform.position, innerRadius);
    }
}
/*```

---

## How to set up the `InternalMonologue` UI

In Unity, on your HUD canvas add a new `TextMeshPro - Text` element:
-Anchor it to the **bottom center** of the screen, about 20% up from the bottom
- Font size around 18, italicized, soft white or very light warm tone
- Add a `CanvasGroup` component to it
- Keep it subtle — this is Juan's thoughts, not a UI notification
```
Canvas (Screen Space Overlay)
  ├── PlayerStatsHUD
  ├── DialoguePanel
  ├── InteractPrompt
  └── InternalMonologueText    ← new, bottom center, CanvasGroup alpha starts at 0
        TextMeshPro: italic, size 18, color #F5EED8
        CanvasGroup: alpha 0*/