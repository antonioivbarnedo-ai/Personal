using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;

    [Header("Typewriter")]
    public float typewriterSpeed = 0.03f;

    public bool IsDialogueActive { get; private set; }
    public System.Action onDialogueEnd;

    private Queue<DialogueLine> _lineQueue = new Queue<DialogueLine>();
    private Coroutine _typewriterCoroutine;
    private string _cachedCurrentText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartDialogue(DialogueLine[] lines)
    {
        IsDialogueActive = true;
        dialoguePanel.SetActive(true);

        _lineQueue.Clear();
        foreach (DialogueLine line in lines)
            _lineQueue.Enqueue(line);

        DisplayNextLine();
    }

    public void DisplayNextSentence() => DisplayNextLine();

    void DisplayNextLine()
    {
        if (_typewriterCoroutine != null)
        {
            StopCoroutine(_typewriterCoroutine);
            _typewriterCoroutine = null;
            dialogueText.text = _cachedCurrentText;
            return;
        }

        if (_lineQueue.Count == 0) { EndDialogue(); return; }

        DialogueLine line = _lineQueue.Dequeue();

        // TEMP debug logs — remove these once speaker name is working
        Debug.Log($"[Dialogue] Speaker: '{line.speakerName}'");
        Debug.Log($"[Dialogue] speakerNameText assigned: {speakerNameText != null}");
        Debug.Log($"[Dialogue] Text: '{line.text}'");

        if (speakerNameText != null)
        {
            bool hasSpeaker = !string.IsNullOrEmpty(line.speakerName);
            speakerNameText.gameObject.SetActive(hasSpeaker);
            speakerNameText.text = hasSpeaker ? line.speakerName.ToUpper() : "";
        }

        _typewriterCoroutine = StartCoroutine(TypewriterEffect(line.text));
    }

    IEnumerator TypewriterEffect(string text)
    {
        _cachedCurrentText = text;
        dialogueText.text = "";

        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        _typewriterCoroutine = null;
    }

    public void EndDialogue()
    {
        IsDialogueActive = false;
        dialoguePanel.SetActive(false);
        onDialogueEnd?.Invoke();
        onDialogueEnd = null;
    }
}