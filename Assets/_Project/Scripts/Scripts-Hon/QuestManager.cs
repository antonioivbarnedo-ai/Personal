using UnityEngine;
using System;

public enum QuestState
{
    ARRIVAL_LOLA        = 0,
    TOUR_COMPLETE       = 1,
    OFFERING_ASGNNED    = 2,
    PLAYGROUND_TALE     = 3,
    THIRD_EYE_UNLOCKED  = 4,
    KIKO_MISSING        = 5,
    NUNO_ENCOUNTER      = 6,
    MANGO_QUEST         = 7,
    VERTICAL_SLICE_END  = 8
}

public class QuestManager : MonoBehaviour
{
    // ── Singleton ──────────────────────────────────────────────
    public static QuestManager Instance { get; private set; }

    // ── The Global State (the "currentQuestState" from your spec) ──
    public static QuestState currentQuestState { get; private set; } = QuestState.ARRIVAL_LOLA;

    // ── C# Events (so UI and World update automatically) ────────
    public static event Action<QuestState, QuestState> OnQuestStateChanged;
    // fires as: OnQuestStateChanged(oldState, newState)

    // ────────────────────────────────────────────────────────────
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ── Advance quest state ─────────────────────────────────────
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

        // Fire the C# Event — UI and World will catch this
        OnQuestStateChanged?.Invoke(oldState, currentQuestState);
    }

    // ── Helper checks (use these in NPC/door/climb logic) ───────
    public bool IsState(QuestState state)       => currentQuestState == state;
    public bool IsAtLeast(QuestState state)     => currentQuestState >= state;
    public bool IsBetween(QuestState a, QuestState b) => currentQuestState >= a && currentQuestState <= b;
}