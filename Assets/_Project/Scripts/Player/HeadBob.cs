using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform cameraTransform;

    [Header("Immersive Head Bob Settings")]
    public float BobFrequency = 5f;
    public float BobAmplitude = 0.05f;
    public float BobSwayFactor = 0.5f;
    public float RunBobSpeedMultiplier = 1.5f;

    [Tooltip("If player speed goes above this, the bobbing speeds up (Sprint effect)")]
    public float SprintSpeedThreshold = 4.5f;

    private float _defaultYPos;
    private float _bobTimer;
    private float _bobIntensity = 0f;

    void Start()
    {
        // Store the starting height of the camera
        _defaultYPos = cameraTransform.localPosition.y;
    }

    void Update()
    {

        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
        {
            return;
        }
        // 1. Calculate how fast we are actually moving horizontally
        float currentSpeed = new Vector3(controller.velocity.x, 0f, controller.velocity.z).magnitude;

        // Check if moving AND grounded
        bool isMoving = controller.isGrounded && currentSpeed > 0.1f;

        // 2. THE FIX: SMOOTH INTENSITY
        // Fade the bob strength in and out so it doesn't snap
        float targetIntensity = isMoving ? 1f : 0f;
        _bobIntensity = Mathf.MoveTowards(_bobIntensity, targetIntensity, Time.deltaTime * 5f);

        // 3. Apply math only if we have intensity (Optimization)
        if (_bobIntensity > 0.01f)
        {
            // Automatically detect sprinting based on actual velocity
            float speedMultiplier = (currentSpeed > SprintSpeedThreshold) ? RunBobSpeedMultiplier : 1f;

            // Increment timer ONLY when we are actually moving
            if (isMoving)
            {
                _bobTimer += Time.deltaTime * BobFrequency * speedMultiplier;
            }

            // VERTICAL BOB (Sine Wave)
            float yOffset = Mathf.Sin(_bobTimer) * BobAmplitude * _bobIntensity;

            // HORIZONTAL SWAY (Cosine Wave)
            float xOffset = Mathf.Cos(_bobTimer / 2f) * BobSwayFactor * BobAmplitude * _bobIntensity;

            // Apply offsets (Preserve the original Z position)
            cameraTransform.localPosition = new Vector3(
                xOffset,
                _defaultYPos + yOffset,
                cameraTransform.localPosition.z
            );
        }
        else
        {
            // When stopped, strictly reset to center to prevent drift
            Vector3 currentPos = cameraTransform.localPosition;
            Vector3 targetPos = new Vector3(0f, _defaultYPos, cameraTransform.localPosition.z);

            cameraTransform.localPosition = Vector3.Lerp(currentPos, targetPos, Time.deltaTime * 5f);
        }
    }
}