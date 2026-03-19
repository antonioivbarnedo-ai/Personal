using UnityEngine;

public class NPC_Kapre : SpiritNPC
{
    public override void Interact()
    {
        base.Interact();

        UnlockThirdEye();
    }

    void UnlockThirdEye()
    {
        Debug.Log("Third Eye Unlocked");

        GameStateManager.Instance.SetState(4);
    }
}