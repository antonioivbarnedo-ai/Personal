using UnityEngine;
using TMPro;
using System.Collections.Generic;
using StarterAssets;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    private Queue<string> sentences = new Queue<string>();
    private StarterAssetsInputs _input;
    public bool IsDialogueActive { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        _input = FindObjectOfType<StarterAssetsInputs>();
        if(dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    void Update()
    {
        // Now only checks for the "Enter" key action (NextDialogue)
        if (IsDialogueActive && _input != null && _input.nextDialogue)
        {
            _input.nextDialogue = false; // Reset the button press
            Debug.Log("Enter pressed: Next line.");
            DisplayNextSentence();
        }
    }

    public void StartDialogue(string[] lines)
    {
        IsDialogueActive = true;
        dialoguePanel.SetActive(true);

        if (UIManager.Instance != null) UIManager.Instance.ToggleHUD(false);

        sentences.Clear();
        foreach (string line in lines) sentences.Enqueue(line);
        
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        dialogueText.text = sentences.Dequeue();
    }

    private void EndDialogue()
    {
        IsDialogueActive = false;
        dialoguePanel.SetActive(false);
        if (UIManager.Instance != null) UIManager.Instance.ToggleHUD(true);
    }
}