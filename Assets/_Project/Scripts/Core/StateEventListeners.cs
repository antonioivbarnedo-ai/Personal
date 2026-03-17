using UnityEngine;

public class StateEventListener : MonoBehaviour
{
    public int triggerState;

    public GameObject[] activateObjects;
    public GameObject[] deactivateObjects;

    void OnEnable()
    {
        GameStateManager.OnStateChanged += HandleStateChange;
    }

    void OnDisable()
    {
        GameStateManager.OnStateChanged -= HandleStateChange;
    }

    void HandleStateChange(int state)
    {
        if (state != triggerState) return;

        foreach (var obj in activateObjects)
            obj.SetActive(true);

        foreach (var obj in deactivateObjects)
            obj.SetActive(false);
    }
}