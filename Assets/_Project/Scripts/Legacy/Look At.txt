using UnityEngine; // Line 1

public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        // Force this object (the UI) to face the Main Camera
        if (Camera.main != null)
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                             Camera.main.transform.rotation * Vector3.up);
        }
    }
}
