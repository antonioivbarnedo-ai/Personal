using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;

public class ThirdEyeAbility : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public Volume globalVolume;
    public string spiritLayerName = "SpiritWorld";

    [Header("State")]
    public bool isThirdEyeActive = false;
    private int defaultMask;
    private int spiritMask;

    void Start()
    {
        if (playerCamera == null) playerCamera = Camera.main;

        // Setup Masks
        // Default mask = Everything EXCEPT SpiritWorld
        defaultMask = playerCamera.cullingMask & ~(1 << LayerMask.NameToLayer(spiritLayerName));
        // Spirit mask = Everything INCLUDING SpiritWorld
        spiritMask = playerCamera.cullingMask | (1 << LayerMask.NameToLayer(spiritLayerName));

        CloseEye(); // Ensure we start closed
    }

    void Update()
    {
        // 1. Input Check (Toggle)
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            ToggleEye();
        }

        // 2. Logic Check: Do we have Spirit Energy?
        if (isThirdEyeActive)
        {
            // Drain spirit via the Stats Script
            // We use "spiritDrainRate" which now exists in PlayerStats!
            bool hasEnergy = PlayerStats.Instance.UseSpirit(PlayerStats.Instance.spiritDrainRate);

            // If UseSpirit returns false, it means we ran out!
            if (!hasEnergy)
            {
                CloseEye();
                Debug.Log("Out of Spirit Energy!");
            }
        }
    }

    void ToggleEye()
    {
        if (isThirdEyeActive)
            CloseEye();
        else
            OpenEye();
    }

    void OpenEye()
    {
        // Check if we have enough spirit to start (at least 5)
        if (PlayerStats.Instance.currentSpirit > 5f)
        {
            isThirdEyeActive = true;
            playerCamera.cullingMask = spiritMask;
            if (globalVolume != null) globalVolume.weight = 1;
        }
    }

    void CloseEye()
    {
        isThirdEyeActive = false;
        playerCamera.cullingMask = defaultMask;
        if (globalVolume != null) globalVolume.weight = 0;
    }
}