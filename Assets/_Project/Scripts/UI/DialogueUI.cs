using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    private string[] lines;
    private int index;

    void Awake()
    {
        Instance = this;
    }

    public void StartDialogue(DialogueData data)
    {
        lines = data.dialogueLines;
        index = 0;

        dialoguePanel.SetActive(true);
        ShowLine();
    }

    void Update()
    {
        if (dialoguePanel.activeSelf &&
            UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            NextLine();
        }
    }

    void ShowLine()
    {
        dialogueText.text = lines[index];
    }

    void NextLine()
    {
        index++;

        if (index >= lines.Length)
        {
            EndDialogue();
        }
        else
        {
            ShowLine();
        }
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
    }
}
