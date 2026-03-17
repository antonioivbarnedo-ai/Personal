using UnityEngine;

public class NPCInteractable : MonoBehaviour, IInteractable
{
    [TextArea(3, 10)]
    public string[] dialogueLines; // Your story sentences

    public void Interact()
    {
        Debug.Log("NPC says: Starting Dialogue now!");
        DialogueManager.Instance.StartDialogue(dialogueLines);
    }
}