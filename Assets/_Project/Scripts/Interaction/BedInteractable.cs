using UnityEngine;

public class BedInteractable : MonoBehaviour, IInteractable
{
    [Header("Quest Gate")]
    public bool requiresMinimumState = true;
    public QuestState minimumStateToSleep;

    public void Interact()
    {
        // Check if the player has done enough today to sleep
        if (requiresMinimumState &&
            !QuestManager.Instance.IsAtLeast(minimumStateToSleep))
        {
            DayManager.Instance.ShowMonologue(MonologueTrigger.CannotSleepYet);

        }

        DayManager.Instance.TriggerSleep(forced: false);
    }
}

/*
**In Unity editor — setting up the bed:**
1.In your Hierarchy, go to your Lola interior studio set (at x=2000)
2. Click the bed mesh GameObject
3. Add Component → `BedInteractable`
4. Add Component → `BoxCollider` (if it doesn't have one already)
5. Make sure `Is Trigger` is **unchecked** — the raycast needs a solid collider
6. Set `Layer` to `Interactable`
7. In Inspector set:
```
   Requires Minimum State: 
   Minimum State To Sleep: THIRD_EYE_UNLOCKED
*/