using UnityEngine;

// Attach this to any GameObject that needs to REACT to quest changes
// e.g. a door, a UI panel, an NPC
public class QuestEventListener : MonoBehaviour
{
    void OnEnable()
    {
        QuestManager.OnQuestStateChanged  += HandleQuestChanged;
        InventorySystem.OnInventoryChanged += HandleInventoryChanged;
    }

    void OnDisable()
    {
        QuestManager.OnQuestStateChanged  -= HandleQuestChanged;
        InventorySystem.OnInventoryChanged -= HandleInventoryChanged;
    }

    void HandleQuestChanged(QuestState oldState, QuestState newState)
    {
        Debug.Log($"[Listener] Quest changed: {oldState} → {newState}");

        switch (newState)
        {
            case QuestState.THIRD_EYE_UNLOCKED:
                // e.g. enable third eye UI overlay
                break;
            case QuestState.KIKO_MISSING:
                // e.g. Kiko's house becomes a destination
                break;
            case QuestState.VERTICAL_SLICE_END:
                // e.g. trigger shortcut dialogue
                break;
        }
    }

    void HandleInventoryChanged(string itemName, bool wasAdded)
    {
        if (itemName == InventorySystem.OFFERING_PLATE && wasAdded)
            Debug.Log("[Listener] Player picked up the Offering Plate!");

        if (itemName == InventorySystem.MAGICAL_MANGO && wasAdded)
            Debug.Log("[Listener] Player has the Magical Mango!");
    }
}