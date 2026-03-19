using UnityEngine;

public class SpiritNPC : NPCDialogue
{
    private ThirdEyeAbility thirdEye;

    void Start()
    {
        thirdEye = FindFirstObjectByType<ThirdEyeAbility>();
    }

    public override void Interact()
    {
        if (thirdEye != null)
            thirdEye.PauseDrain();

        base.Interact();
    }

    void Update()
    {
        if (!DialogueUI.Instance.dialoguePanel.activeSelf)
        {
            if (thirdEye != null)
                thirdEye.ResumeDrain();
        }
    }
}