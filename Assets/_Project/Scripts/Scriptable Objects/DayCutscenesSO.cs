using UnityEngine;
using System;

public enum CutsceneType
{
    Lunch,
    ForcedSleep,
    VoluntarySleep
}

[Serializable]
public class DayCutsceneEntry
{
    public CutsceneType type;
    public Sprite[] frames;              // images that play in sequence
    public float secondsPerFrame = 1.5f; // how long each image shows
   
}

[CreateAssetMenu(fileName = "NewDayCutscene", menuName = "Tabi-Tabi/Day Cutscene")]
public class DayCutsceneSO : ScriptableObject
{
    public DayCutsceneEntry[] entries;

    public DayCutsceneEntry GetEntry(CutsceneType type)
    {
        foreach (var entry in entries)
            if (entry.type == type) return entry;

        Debug.LogWarning($"[DayCutsceneSO] No entry found for type {type}");
        return null;
    }
}