using UnityEngine;
using System; // needed for Action

public class TimeSystem : MonoBehaviour
{
    [Header("Lighting")]
    public Light sun;

    [Header("Sky / Shader Control")]
    public Material skyboxMaterial;

    [Header("Time")]
    public float timeOfDay = 0f;
    public float daySpeed = 0.1f;

    // ── Night penalty event ──────────────────────────────────────
    public static event Action OnNightPenaltyTick;
    private float _nightPenaltyCooldown = 0f;

    private Vector3 fakeSunDirection;

    void Update()
    {
        timeOfDay = (timeOfDay + Time.deltaTime * daySpeed) % 24f;
        UpdateRealSun();
        UpdateFakeSun();
        HandleGameplayTime();
    }

    void UpdateRealSun()
    {
        float normalizedTime = timeOfDay / 24f;
        float sunAngle = Mathf.Lerp(0f, 180f, normalizedTime);
        sun.transform.rotation = Quaternion.Euler(sunAngle - 90f, 170f, 0);
        float intensity = Mathf.Sin(normalizedTime * Mathf.PI);
        sun.intensity = Mathf.Clamp01(intensity);

        if (sun.intensity > 0.01f)
        {
            sun.enabled = true;
            sun.shadows = LightShadows.Soft;
        }
        else
        {
            sun.enabled = false;
        }
    }

    void UpdateFakeSun()
    {
        float rotation = (timeOfDay / 24f) * 360f;

        Quaternion rot = Quaternion.Euler(rotation + 90f, 170f, 0);
        fakeSunDirection = rot * Vector3.forward;

        Shader.SetGlobalVector("_FakeSunDirection", fakeSunDirection);
    }

    void HandleGameplayTime()
    {
        // Clamp at forced sleep time — DayManager takes over from here
        if (timeOfDay >= 20f)
        {
            timeOfDay = 20f;
            return;
        }

        if (timeOfDay >= 18f && timeOfDay < 19f)
            Debug.Log("Uneasy feeling... The spirits are waking.");

        if (timeOfDay >= 19f)
            NightPenalty();
    }

    void NightPenalty()
    {
        _nightPenaltyCooldown -= Time.deltaTime;
        if (_nightPenaltyCooldown > 0f) return;

        _nightPenaltyCooldown = 1f; // fires once per second, not every frame
        OnNightPenaltyTick?.Invoke();
    }

}