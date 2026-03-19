using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public string currentQuest;

    void Awake()
    {
        Instance = this;
    }

    public void SetQuest(string quest)
    {
        currentQuest = quest;
        Debug.Log("Quest Updated: " + quest);
    }
}