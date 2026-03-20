using UnityEngine;

public class TimeSystem : MonoBehaviour
{
    [Header("Lighting")]
    public Light sun; // REAL lighting (safe, no underground rotation)

    [Header("Sky / Shader Control")]
    public Material skyboxMaterial; // optional (if you tweak sky params)

    [Header("Time")]
    [Range(0, 24)] public float timeOfDay = 15f;
    public float daySpeed = 0.1f;

    // Fake sun direction for shader graph (FULL 360)
    private Vector3 fakeSunDirection;

    void Update()
    {
        // Loop time 0–24
        timeOfDay = (timeOfDay + Time.deltaTime * daySpeed) % 24f;

        UpdateRealSun();     // safe lighting
        UpdateFakeSun();     // full rotation for sky

        HandleGameplayTime();
    }

    // 🌞 REAL SUN (SAFE - NEVER GOES UNDERGROUND)
    void UpdateRealSun()
    {
        float normalizedTime = timeOfDay / 24f;

        // Only rotate above horizon (0 → 180)
        float sunAngle = Mathf.Lerp(0f, 180f, normalizedTime);

        sun.transform.rotation = Quaternion.Euler(sunAngle - 90f, 170f, 0);

        // Smooth intensity (day curve)
        float intensity = Mathf.Sin(normalizedTime * Mathf.PI);

        sun.intensity = Mathf.Clamp01(intensity);

        // Enable only when visible
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

    // 🌌 FAKE SUN (FULL 360 FOR SKY / SHADER GRAPH)
    void UpdateFakeSun()
    {
        float rotation = (timeOfDay / 24f) * 360f;

        Quaternion rot = Quaternion.Euler(rotation - 90f, 170f, 0);
        fakeSunDirection = rot * Vector3.forward;

        // Send to Shader Graph
        Shader.SetGlobalVector("_FakeSunDirection", fakeSunDirection);
    }

    // 🧠 GAMEPLAY TIME LOGIC
    void HandleGameplayTime()
    {
        // 6PM–7PM uneasy
        if (timeOfDay >= 18f && timeOfDay < 19f)
        {
            Debug.Log("Uneasy feeling... The spirits are waking.");
        }

        // Night penalty
        if (timeOfDay >= 19f || timeOfDay < 5f)
        {
            NightPenalty();
        }
    }

    void NightPenalty()
    {
        // TODO: Reduce sanity, trigger cutscene, etc.
        // Keep this clean → call PlayerHealth or GameManager
    }
}