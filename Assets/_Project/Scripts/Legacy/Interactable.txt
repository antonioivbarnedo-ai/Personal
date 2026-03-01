using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Settings")]
    public string prompt = "Talk";
    [TextArea] public string dialogueMessage = "I am a ghost.";

    [Header("UI Positioning")]
    public GameObject floatingPrompt; // The Prefab
    // THIS is the magic variable. Change X/Y/Z to move the text!
    public Vector3 promptOffset = new Vector3(0, 1.5f, 0);

    private GameObject currentPrompt;

    void Start()
    {
        if (floatingPrompt != null)
        {
            // Spawn it at (Object Position + Offset)
            currentPrompt = Instantiate(floatingPrompt, transform.position + promptOffset, Quaternion.identity);

            // Note: We do NOT parent it anymore. 
            // Parenting causes the text to stretch if the ghost is scaled weirdly.
            currentPrompt.SetActive(false);
        }
    }

    void Update()
    {
        if (currentPrompt != null)
        {
            // 1. Keep the prompt stuck to the ghost (with offset)
            currentPrompt.transform.position = transform.position + promptOffset;

            // 2. Make it face the camera
            if (currentPrompt.activeSelf)
            {
                currentPrompt.transform.LookAt(Camera.main.transform);
                currentPrompt.transform.Rotate(0, 180, 0); // Fix mirrored text
            }
        }
    }

    public void ShowPrompt()
    {
        if (currentPrompt != null) currentPrompt.SetActive(true);
    }

    public void HidePrompt()
    {
        if (currentPrompt != null) currentPrompt.SetActive(false);
    }

    // This draws a little gizmo in the editor so you can see where the text will be!
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + promptOffset, 0.2f);
    }
}