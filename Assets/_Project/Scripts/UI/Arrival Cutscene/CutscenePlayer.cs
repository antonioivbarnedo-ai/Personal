using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CutscenePlayer : MonoBehaviour
{
    public Image displayImage;
    public Sprite[] frames;

    private int index = 0;

    void Start()
    {
        if (frames.Length == 0)
        {
            Debug.LogError("No frames assigned!");
            return;
        }

        if (displayImage == null)
        {
            Debug.LogError("Display Image missing!");
            return;
        }

        displayImage.sprite = frames[0];
    }

    void Update()
    {
        // NEXT FRAME
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            NextFrame();
        }

        // 🔥 SKIP CUTSCENE
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            SkipCutscene();
        }
    }

    void NextFrame()
    {
        index++;

        if (index >= frames.Length)
        {
            SkipCutscene();
        }
        else
        {
            displayImage.sprite = frames[index];
        }
    }

    void SkipCutscene()
    {
        Debug.Log("Cutscene Skipped");
        SceneTransitionManager.Instance.LoadScene("Greyboxing");
    }
}