using UnityEngine;

public class TeleportZone : MonoBehaviour, IInteractable
{
    [Header("Destination")]
    [Tooltip("The exact position the player arrives at")]
    public Transform destination;

    [Header("Quest Gate")]
    public bool requiresQuestState = false;
    public QuestState requiredState;

    [Header("Prompt Override")]
    [Tooltip("Leave empty to use default 'Press E' prompt")]
    public string customPromptText = "";

    public void Interact()
    {
        if (requiresQuestState &&
            !QuestManager.Instance.IsAtLeast(requiredState))
        {
            DayManager.Instance.ShowMonologue(MonologueTrigger.LockedDoor);
        }

        if (destination == null)
        {
            Debug.LogError($"[TeleportZone] {gameObject.name} has no destination assigned!");
            return;
        }

        SceneTransitionManager.Instance.TeleportPlayer(destination);
    }
}