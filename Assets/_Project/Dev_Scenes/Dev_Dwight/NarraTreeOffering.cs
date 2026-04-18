using UnityEngine;

public class NarraTreeOffering : MonoBehaviour, IInteractable
{
    public GameObject offeringVisual; 

    public void Interact()
    {
        // SAFETY CHECK: If the QuestManager object isn't in your Hierarchy, stop here!
        if (QuestManager.Instance == null)
        {
            Debug.LogError("FATAL: QuestManager is missing from the Dev_Dwight scene!");
            return;
        }

        if (QuestManager.currentQuestState == QuestState.OFFERING_ASSIGNED)
        {
            // Advance the quest
            QuestManager.Instance.AdvanceQuest(QuestState.THIRD_EYE_UNLOCKED);
            
            if (offeringVisual != null) offeringVisual.SetActive(true);
            
            if (TryGetComponent<Collider>(out var col)) col.enabled = false;
        }
    }
}