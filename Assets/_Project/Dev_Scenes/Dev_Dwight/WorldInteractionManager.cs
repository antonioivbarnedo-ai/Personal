using UnityEngine;

public class WorldInteractionManager : MonoBehaviour
{
    // This 'Instance' allows other scripts to find this one easily
    public static WorldInteractionManager Instance { get; private set; }

    [Header("Quest States")]
    public bool isThirdEyeUnlocked = false; 
    public bool hasOfferingItem = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}