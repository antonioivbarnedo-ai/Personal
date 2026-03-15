using UnityEngine;

public class NPCDialogue : Interactable
{
    public DialogueData dialogue;

    public override void Interact()
    {
        DialogueUI.Instance.StartDialogue(dialogue);
    }
}