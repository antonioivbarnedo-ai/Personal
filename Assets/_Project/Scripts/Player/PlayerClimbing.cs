using UnityEngine;

public class PlayerClimbing : MonoBehaviour
{
    [SerializeField] private CharacterController controller;

    public LayerMask climbLayer;
    public float climbSpeed = 3f;
    public float detectDistance = 1f;

    private bool isClimbing;

    void Update()
    {
        DetectClimb();
    }

    void DetectClimb()
    {
        if (!QuestManager.Instance.IsAtLeast(QuestState.THIRD_EYE_UNLOCKED))
        {
            isClimbing = false;
            return;
        }
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, detectDistance, climbLayer))
        {
            if (Input.GetKey(KeyCode.W))
            {
                isClimbing = true;
                controller.Move(Vector3.up * climbSpeed * Time.deltaTime);
            }
        }
        else
        {
            isClimbing = false;
        }
    }

    public bool IsClimbing()
    {
        return isClimbing;
    }
}