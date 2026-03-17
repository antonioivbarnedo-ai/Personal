using UnityEngine;

public class NPC_Lola : NPCDialogue
{
    public Transform tourTarget;

    public override void Interact()
    {
        base.Interact();

        if (GameStateManager.Instance.IsState(0))
        {
            StartTour();
        }
    }

    void StartTour()
    {
        Debug.Log("Lola tour started");

        GameStateManager.Instance.SetState(1);
    }
}
