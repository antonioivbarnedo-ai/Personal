using UnityEngine;
using System;

public enum QuestState
{
<<<<<<< HEAD
    ARRIVAL_LOLA        = 0,
    TOUR_COMPLETE       = 1,
    OFFERING_ASGNNED    = 2,
    PLAYGROUND_TALE     = 3,
    THIRD_EYE_UNLOCKED  = 4,
    KIKO_MISSING        = 5,
    NUNO_ENCOUNTER      = 6,
    MANGO_QUEST         = 7,
    VERTICAL_SLICE_END  = 8
=======
    ARRIVAL_LOLA = 0,
    TOUR_COMPLETE = 1,
    OFFERING_ASSIGNED = 2,   // fixed the typo from ASGNNED
    PLAYGROUND_TALE = 3,
    THIRD_EYE_UNLOCKED = 4,
    KIKO_MISSING = 5,
    NUNO_ENCOUNTER = 6,
    MANGO_QUEST = 7,
    VERTICAL_SLICE_END = 8
>>>>>>> integrated-systems-recover
}

public class QuestManager : MonoBehaviour
{
<<<<<<< HEAD
    // ── Singleton ──────────────────────────────────────────────
    public static QuestManager Instance { get; private set; }

    // ── The Global State (the "currentQuestState" from your spec) ──
    public static QuestState currentQuestState { get; private set; } = QuestState.ARRIVAL_LOLA;

    // ── C# Events (so UI and World update automatically) ────────
    public static event Action<QuestState, QuestState> OnQuestStateChanged;
    // fires as: OnQuestStateChanged(oldState, newState)

    // ────────────────────────────────────────────────────────────
=======
    public static QuestManager Instance { get; private set; }

    public static QuestState currentQuestState { get; private set; } = QuestState.ARRIVAL_LOLA;

    // Two events: one typed (for new code), one int (legacy bridge for old code)
    public static event Action<QuestState, QuestState> OnQuestStateChanged;
    public static event Action<int> OnStateChanged; // matches old GameStateManager signature

>>>>>>> integrated-systems-recover
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

<<<<<<< HEAD
    // ── Advance quest state ─────────────────────────────────────
=======
    // ── Main method (use this everywhere going forward) ─────────
>>>>>>> integrated-systems-recover
    public void AdvanceQuest(QuestState newState)
    {
        if (newState <= currentQuestState)
        {
            Debug.LogWarning($"[QuestManager] Already at {currentQuestState}, ignoring {newState}");
            return;
        }

        QuestState oldState = currentQuestState;
        currentQuestState = newState;

        Debug.Log($"[QuestManager] {oldState} → {currentQuestState}");

<<<<<<< HEAD
        // Fire the C# Event — UI and World will catch this
        OnQuestStateChanged?.Invoke(oldState, currentQuestState);
    }

    // ── Helper checks (use these in NPC/door/climb logic) ───────
    public bool IsState(QuestState state)       => currentQuestState == state;
    public bool IsAtLeast(QuestState state)     => currentQuestState >= state;
=======
        OnQuestStateChanged?.Invoke(oldState, currentQuestState);
        OnStateChanged?.Invoke((int)currentQuestState); // also fire legacy event
    }

    // ── Legacy bridge — drop-in replacement for GameStateManager.SetState() ──
    // Any old code calling GameStateManager.Instance.SetState(2) can now call
    // QuestManager.Instance.LegacySetState(2) and it routes correctly.
    public void LegacySetState(int stateIndex)
    {
        if (System.Enum.IsDefined(typeof(QuestState), stateIndex))
            AdvanceQuest((QuestState)stateIndex);
        else
            Debug.LogError($"[QuestManager] LegacySetState: {stateIndex} has no matching QuestState.");
    }

    // ── Helpers ──────────────────────────────────────────────────
    public bool IsState(QuestState state) => currentQuestState == state;
    public bool IsAtLeast(QuestState state) => currentQuestState >= state;
>>>>>>> integrated-systems-recover
    public bool IsBetween(QuestState a, QuestState b) => currentQuestState >= a && currentQuestState <= b;
}