using UnityEngine;

public class NPC_Nuno : SpiritNPC
{
    bool battleFinished = false;

    public override void Interact()
    {
        base.Interact();

        if (!battleFinished)
        {
            StartBattle();
        }
        else
        {
            GiveQuest();
        }
    }

    void StartBattle()
    {
        Debug.Log("Nuno battle started");

        GameStateManager.Instance.SetState(6);

        battleFinished = true;
    }

    void GiveQuest()
    {
        Debug.Log("Find the magical mango");

        GameStateManager.Instance.SetState(7);
    }
}