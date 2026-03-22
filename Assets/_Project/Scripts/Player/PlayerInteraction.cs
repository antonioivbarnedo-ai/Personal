using UnityEngine;
using StarterAssets;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float interactRange = 4f;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private Transform cameraTransform;

    [Header("Interaction Prompt UI")]
    [SerializeField] private GameObject interactPromptUI;

    private StarterAssetsInputs _input;
    private FirstPersonController _fpsController;
    private IInteractable _currentTarget;

    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
        _fpsController = GetComponent<FirstPersonController>();

        if (interactPromptUI != null)
            interactPromptUI.SetActive(false);
    }

    void Update()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
        {
            HidePrompt();
            LockMovement(true);

            if (_input.interact)
            {
                _input.interact = false;
                DialogueManager.Instance.DisplayNextSentence();
            }
            return;
        }

        // Dialogue just ended — make sure movement is restored
        LockMovement(false);

        ScanForTarget();

        if (_input.interact && _currentTarget != null)
        {
            _input.interact = false;
            _currentTarget.Interact();
        }
    }

    void LockMovement(bool locked)
    {
        if (_fpsController != null)
            _fpsController.enabled = !locked;
    }

    void ScanForTarget()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactLayer)
            && hit.collider.TryGetComponent(out IInteractable interactable))
        {
            _currentTarget = interactable;
            ShowPrompt();
        }
        else
        {
            _currentTarget = null;
            HidePrompt();
        }
    }

    void ShowPrompt()
    {
        if (interactPromptUI != null && !interactPromptUI.activeSelf)
            interactPromptUI.SetActive(true);
    }

    void HidePrompt()
    {
        if (interactPromptUI != null && interactPromptUI.activeSelf)
            interactPromptUI.SetActive(false);
    }
}