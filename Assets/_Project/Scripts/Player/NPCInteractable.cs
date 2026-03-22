using UnityEngine;

public class NPCInteractable : MonoBehaviour, IInteractable
{
    [Header("Dialogue Data")]
    [Tooltip("Drag the NPC's .asset file here — created via right-click > Tabi-Tabi > NPC Dialogue")]
    public NPCDialogueSO dialogueData;

    public void Interact()
    {
        if (dialogueData == null)
        {
            Debug.LogWarning($"[NPCInteractable] {gameObject.name} has no dialogue data assigned.");
            return;
        }

        DialogueEntry entry = dialogueData.GetEntryForState(QuestManager.currentQuestState);

        if (entry == null)
        {
            Debug.LogWarning($"[NPCInteractable] {gameObject.name}: no entry matches state {QuestManager.currentQuestState}");
            return;
        }

        // Convert DialogueLine[] into the format DialogueManager expects
        DialogueManager.Instance.StartDialogue(entry.lines);

        // If this conversation should push the quest forward, wire it up
        if (entry.advancesQuestOnEnd)
        {
            DialogueManager.Instance.onDialogueEnd = () =>
            {
                QuestManager.Instance.AdvanceQuest(entry.advanceTo);
            };
        }
    }
}