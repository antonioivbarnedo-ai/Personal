using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerInteract : MonoBehaviour
{
    [Header("Settings")]
    public float interactRange = 3f;
    public LayerMask interactLayer;
    public float typingSpeed = 0.05f;

    // --- NEW VARIABLE ADDED HERE ---
    public ThirdEyeAbility thirdEyeScript; // Reference to check if eye is open
    // -------------------------------

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    private Camera cam;
    private Interactable currentInteractable;

    // State Variables
    private bool isDialogueOpen = false;
    private bool isTyping = false;
    private string fullTextToType = "";
    private Coroutine typingCoroutine;

    void Start()
    {
        cam = Camera.main;
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (isDialogueOpen)
        {
            // NEW: Force close if eye gets closed mid-conversation
            if (isDialogueOpen && thirdEyeScript != null && !thirdEyeScript.isThirdEyeActive)
            {
                EndDialogue();
                return;
            }

            HandleDialogueInput();
            return;
        }

        HandleRaycast();
    }

    void HandleRaycast()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange, interactLayer))
        {
            // --- NEW LOGIC: CHECK FOR SPIRIT VISIBILITY ---
            // If the object is on the "SpiritWorld" layer...
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("SpiritWorld"))
            {
                // ...and the Third Eye script is attached but NOT active (Closed)
                if (thirdEyeScript != null && !thirdEyeScript.isThirdEyeActive)
                {
                    // Pretend we didn't see anything.
                    ClearInteractable();
                    return; // Stop here. Don't run the rest of the code.
                }
            }
            // ----------------------------------------------

            Interactable target = hit.collider.GetComponent<Interactable>();

            if (target != null)
            {
                if (currentInteractable != target)
                {
                    if (currentInteractable != null) currentInteractable.HidePrompt();
                    currentInteractable = target;
                    currentInteractable.ShowPrompt();
                }

                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    StartDialogue(target);
                }
            }
        }
        else
        {
            // We hit nothing (looked at sky/floor)
            ClearInteractable();
        }
    }

    // Helper function to clean up code
    void ClearInteractable()
    {
        if (currentInteractable != null)
        {
            currentInteractable.HidePrompt();
            currentInteractable = null;
        }
    }

    void StartDialogue(Interactable target)
    {
        isDialogueOpen = true;
        currentInteractable.HidePrompt();

        dialoguePanel.SetActive(true);
        fullTextToType = target.dialogueMessage;

        typingCoroutine = StartCoroutine(TypeLine());
    }

    void HandleDialogueInput()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame)
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = fullTextToType;
                isTyping = false;
            }
            else
            {
                EndDialogue();
            }
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in fullTextToType.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void EndDialogue()
    {
        isDialogueOpen = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }
}