using UnityEngine;
using UnityEngine.UI;

public class CutscenePlayer : MonoBehaviour
{
    public Image displayImage;
    public Sprite[] frames;

    int index = 0;

    void Start()
    {
        displayImage.sprite = frames[0];
    }

    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            index++;

            if (index >= frames.Length)
            {
                SceneTransitionManager.Instance.LoadScene("Greyboxing");
            }
            else
            {
                displayImage.sprite = frames[index];
            }
        }
    }
}
