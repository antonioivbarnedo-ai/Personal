using UnityEngine;
using System;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    public int currentState = 0;

    public static event Action<int> OnStateChanged;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetState(int newState)
    {
        currentState = newState;

        Debug.Log("State Changed: " + newState);

        OnStateChanged?.Invoke(newState);
    }

    public bool IsState(int state)
    {
        return currentState == state;
    }
}