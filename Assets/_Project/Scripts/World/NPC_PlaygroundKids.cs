using UnityEngine;

public class NPC_PlaygroundKids : NPCDialogue
{
    public string kidName;

    static int kidsTalked = 0;

    public override void Interact()
    {
        base.Interact();

        kidsTalked++;

        if (kidsTalked >= 2)
        {
            GameStateManager.Instance.SetState(3);
        }
    }
}