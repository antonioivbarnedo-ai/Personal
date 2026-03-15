using UnityEngine;
using UnityEngine.Rendering;

public class ThirdEyeAbility : MonoBehaviour

{
    [SerializeField] private PlayerStats stats;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Volume postProcessVolume;

    [Header("Settings")]

    public string spiritLayer = "SpiritWorld";
    public float spiritDrainRate = 15f;
    public bool pauseDrain = false;

    private bool isActive = false;
    private int normalMask;
    private int spiritMask;


    void Start()

    {

        normalMask = playerCamera.cullingMask & ~(1 << LayerMask.NameToLayer(spiritLayer));
        spiritMask = playerCamera.cullingMask | (1 << LayerMask.NameToLayer(spiritLayer));

        CloseEye();
    }

    public void PauseDrain()
    {
        pauseDrain = true;
    }

    public void ResumeDrain()
    {
        pauseDrain = false;
    }

    void Update()

    {

        if (UnityEngine.InputSystem.Keyboard.current.qKey.wasPressedThisFrame)

        {
            ToggleEye();
        }



        if (isActive && !pauseDrain)

        {
            stats.currentSpirit -= spiritDrainRate * Time.deltaTime;

            if (stats.currentSpirit <= 0f)

            {

                stats.currentSpirit = 0f;

                CloseEye();
            }
        }
    }



    void ToggleEye()

    {

        if (isActive && !pauseDrain)

            CloseEye();

        else if (stats.currentSpirit > 5f)

            OpenEye();

    }



    void OpenEye()

    {

        isActive = true;

        playerCamera.cullingMask = spiritMask;



        if (postProcessVolume != null)

            postProcessVolume.weight = 1;

    }



    void CloseEye()

    {

        isActive = false;

        playerCamera.cullingMask = normalMask;



        if (postProcessVolume != null)

            postProcessVolume.weight = 0;
    }
}