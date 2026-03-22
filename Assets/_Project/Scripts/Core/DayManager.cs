using UnityEngine;
using System;
using System.Collections;

public enum GameDay
{
    Day1_Arrival = 1,
    Day2_Free = 2,
    Day3_KikoGone = 3
}

public class DayManager : MonoBehaviour
{
    public static DayManager Instance { get; private set; }

    [Header("Current Day")]
    public GameDay currentDay = GameDay.Day1_Arrival;

    [Header("References")]
    [SerializeField] private TimeSystem timeSystem;

    [Header("Data")]
    [SerializeField] private DayCutsceneSO cutsceneDatabase;
    [SerializeField] private MonologueDatabaseSO monologueDatabase;

    [Header("Teleport Destinations")]
    [SerializeField] private Transform lunchDestination;
    [SerializeField] private Transform forcedSleepDestination;
    [SerializeField] private Transform voluntarySleepDestination;

    // ── Events ───────────────────────────────────────────────────
    public static event Action<GameDay> OnDayStarted;
    public static event Action<GameDay> OnDayEnding;
    public static event Action OnSleepStart;
    public static event Action OnLunchTime;
    public static event Action OnDangerZone;
    public static event Action OnForcedSleepTime;

    // ── Internal flags ───────────────────────────────────────────
    private bool _sleepTriggered = false;
    private bool _lunchFired = false;
    private bool _dangerFired = false;
    private bool _forcedSleepFired = false;

    private const float MORNING_START = 7f;
    private const float LUNCH_TIME = 10f;
    private const float AFTERNOON_START = 15f;
    private const float DANGER_START = 19f;
    private const float FORCE_SLEEP = 20f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        timeSystem.timeOfDay = MORNING_START;
        OnDayStarted?.Invoke(currentDay);
    }

    void Update()
    {
        if (_sleepTriggered) return;

        float t = timeSystem.timeOfDay;

        if (!_lunchFired && t >= LUNCH_TIME)
        {
            _lunchFired = true;
            TriggerLunchCutscene();
        }

        if (!_dangerFired && t >= DANGER_START)
        {
            _dangerFired = true;
            OnDangerZone?.Invoke();
            ShowMonologue(MonologueTrigger.DangerZone);
        }

        if (!_forcedSleepFired && t >= FORCE_SLEEP)
        {
            _forcedSleepFired = true;
            OnForcedSleepTime?.Invoke();
            TriggerSleep(forced: true);
        }
    }

    // ── Lunch ────────────────────────────────────────────────────
    void TriggerLunchCutscene()
    {
        ShowMonologue(MonologueTrigger.LunchTime);
        OnLunchTime?.Invoke();
        StartCoroutine(LunchCutsceneDelayed());
    }

    IEnumerator LunchCutsceneDelayed()
    {
        yield return new WaitForSeconds(2.5f);

        timeSystem.enabled = false;

        var entry = cutsceneDatabase != null
            ? cutsceneDatabase.GetEntry(CutsceneType.Lunch)
            : null;

        if (entry != null)
            DayCutscenePlayer.Instance.PlayCutscene(entry, OnLunchCutsceneComplete);
        else
            OnLunchCutsceneComplete();
    }

    void OnLunchCutsceneComplete()
    {
        timeSystem.timeOfDay = AFTERNOON_START;
        timeSystem.enabled = true;
        StartCoroutine(AfterLunchMonologueDelayed());
    }

    IEnumerator AfterLunchMonologueDelayed()
    {
        yield return new WaitForSeconds(1f);
        ShowMonologue(MonologueTrigger.AfterLunch);
    }

    // ── Sleep ────────────────────────────────────────────────────
    public void TriggerSleep(bool forced)
    {
        if (_sleepTriggered) return;
        _sleepTriggered = true;

        OnDayEnding?.Invoke(currentDay);
        ShowMonologue(forced ? MonologueTrigger.Day1Ending : MonologueTrigger.Day2Ending);

        var type = forced ? CutsceneType.ForcedSleep : CutsceneType.VoluntarySleep;
        var entry = cutsceneDatabase != null
            ? cutsceneDatabase.GetEntry(type)
            : null;

        if (entry != null)
            StartCoroutine(SleepWithCutscene(entry));
        else
            StartCoroutine(SleepSequence());
    }

    IEnumerator SleepWithCutscene(DayCutsceneEntry entry)
    {
        yield return new WaitForSeconds(2.5f);

        OnSleepStart?.Invoke();
        timeSystem.enabled = false;

        bool cutsceneDone = false;
        DayCutscenePlayer.Instance.PlayCutscene(entry, () => cutsceneDone = true);

        yield return new WaitUntil(() => cutsceneDone);

        AdvanceDay();
        ResetDay();
    }

    IEnumerator SleepSequence()
    {
        yield return new WaitForSeconds(2.5f);
        OnSleepStart?.Invoke();

        yield return StartCoroutine(SceneTransitionManager.Instance.FadeOut());

        AdvanceDay();
        ResetDay();

        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(SceneTransitionManager.Instance.FadeIn());

        OnDayStarted?.Invoke(currentDay);
    }

    // ── Day control ──────────────────────────────────────────────
    void ResetDay()
    {
        timeSystem.timeOfDay = MORNING_START;
        timeSystem.enabled = true;
        _sleepTriggered = false;
        _lunchFired = false;
        _dangerFired = false;
        _forcedSleepFired = false;

        OnDayStarted?.Invoke(currentDay);
    }

    void AdvanceDay()
    {
        switch (currentDay)
        {
            case GameDay.Day1_Arrival:
                currentDay = GameDay.Day2_Free;
                break;
            case GameDay.Day2_Free:
                currentDay = GameDay.Day3_KikoGone;
                break;
            case GameDay.Day3_KikoGone:
                Debug.Log("[DayManager] End of vertical slice.");
                break;
        }
    }

    // ── Helpers ──────────────────────────────────────────────────
    public void ShowMonologue(MonologueTrigger trigger)
    {
        if (monologueDatabase == null) return;
        string line = monologueDatabase.GetRandom(trigger);
        if (!string.IsNullOrEmpty(line))
            InternalMonologue.Instance?.ShowThought(line);
    }

    public Transform GetCutsceneDestination(CutsceneType type)
    {
        return type switch
        {
            CutsceneType.Lunch => lunchDestination,
            CutsceneType.ForcedSleep => forcedSleepDestination,
            CutsceneType.VoluntarySleep => voluntarySleepDestination,
            _ => null
        };
    }
}