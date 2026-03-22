using UnityEngine;
using System;

public enum QuestState
{
    ARRIVAL_LOLA = 0,
    TOUR_COMPLETE = 1,
    OFFERING_ASSIGNED = 2,   // fixed the typo from ASGNNED
    PLAYGROUND_TALE = 3,
    THIRD_EYE_UNLOCKED = 4,
    KIKO_MISSING = 5,
    NUNO_ENCOUNTER = 6,
    MANGO_QUEST = 7,
    VERTICAL_SLICE_END = 8
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    public static QuestState currentQuestState { get; private set; } = QuestState.ARRIVAL_LOLA;

    // Two events: one typed (for new code), one int (legacy bridge for old code)
    public static event Action<QuestState, QuestState> OnQuestStateChanged;
    public static event Action<int> OnStateChanged; // matches old GameStateManager signature

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ── Main method (use this everywhere going forward) ─────────
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
    public bool IsBetween(QuestState a, QuestState b) => currentQuestState >= a && currentQuestState <= b;
}