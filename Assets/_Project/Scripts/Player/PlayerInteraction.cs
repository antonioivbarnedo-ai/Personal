using UnityEngine;
using StarterAssets; // Important: This allows us to talk to your movement script

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float interactRange = 4f; // Distance of the "laser"
    [SerializeField] private LayerMask interactLayer;   // Set this to "Interactable" in Inspector
    [SerializeField] private Transform cameraTransform; // Drag MainCamera here
    
    private StarterAssetsInputs _input;

    private void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
    }

    void Update()
    {
        // Don't interact if we are already in a conversation
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive) return;

        // Check if the Interact button (E) was pressed
        if (_input.interact) 
        {
            // Reset the button immediately so it doesn't trigger every single frame
            _input.interact = false; 

            Debug.Log("Player tried to interact!");

            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactRange, interactLayer))
            {
                if (hit.collider.TryGetComponent(out IInteractable interactable))
                {
                    Debug.Log("Hit an interactable object: " + hit.collider.name);
                    interactable.Interact();
                }
            }
            else 
            {
                Debug.Log("Raycast hit nothing on the Interactable layer.");
            }
        }
    }
}