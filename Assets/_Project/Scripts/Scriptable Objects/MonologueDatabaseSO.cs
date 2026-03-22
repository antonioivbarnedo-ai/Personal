using UnityEngine;
using System;

public enum MonologueTrigger
{
    LunchTime,
    AfterLunch,
    DangerZone,
    Day1Ending,
    Day2Ending,
    Day2Morning,
    Day3Morning,
    CannotSleepYet,
    LockedDoor,
    MoundOuter,
    MoundInner
}

[Serializable]
public class MonologueEntry
{
    public MonologueTrigger trigger;
    [TextArea(2, 4)]
    public string[] lines; // picked at random
}

[CreateAssetMenu(fileName = "NewMonologueDatabase",
                 menuName = "Tabi-Tabi/Monologue Database")]
public class MonologueDatabaseSO : ScriptableObject
{
    public MonologueEntry[] entries;

    public string GetRandom(MonologueTrigger trigger)
    {
        foreach (var entry in entries)
        {
            if (entry.trigger == trigger && entry.lines.Length > 0)
                return entry.lines[UnityEngine.Random.Range(0, entry.lines.Length)];
        }

        Debug.LogWarning($"[MonologueDatabase] No lines for trigger {trigger}");
        return "";
    }
}