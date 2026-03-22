using UnityEngine;
using System;

// A single line of dialogue — who says it and what they say
[Serializable]
public class DialogueLine
{
    public string speakerName;    // "Lola", "Kapre", "Nuno", "Juan"
    [TextArea(2, 5)]
    public string text;
}

// One block of dialogue tied to a quest state
// "Show these lines when the quest is at least this state"
[Serializable]
public class DialogueEntry
{
    [Tooltip("This block activates when quest state reaches this value")]
    public QuestState requiredState;

    [Tooltip("Set true if reaching this dialogue should also advance the quest")]
    public bool advancesQuestOnEnd;
    public QuestState advanceTo;

    public DialogueLine[] lines;
}

// The ScriptableObject asset — one per NPC (or per conversation)
[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "Tabi-Tabi/NPC Dialogue")]
public class NPCDialogueSO : ScriptableObject
{
    [Tooltip("Human-readable label — just for you in the editor")]
    public string npcLabel;

    public DialogueEntry[] entries;

    // Called by NPCInteractable at runtime
    // Returns the best matching entry for the current quest state
    public DialogueEntry GetEntryForState(QuestState currentState)
    {
        DialogueEntry best = null;

        foreach (DialogueEntry entry in entries)
        {
            if (currentState >= entry.requiredState)
            {
                if (best == null || entry.requiredState >= best.requiredState)
                    best = entry;
            }
        }

        return best;
    }
}