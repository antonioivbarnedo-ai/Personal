using UnityEngine;

public class TimeSystem : MonoBehaviour
{
    public Light sun;
    public float timeOfDay = 15f;
    public float daySpeed = 0.1f;

    void Update()
    {
        timeOfDay += Time.deltaTime * daySpeed;

        if (timeOfDay >= 24)
            timeOfDay = 0;

        UpdateSun();

        if (timeOfDay >= 18 && timeOfDay < 19)
        {
            Debug.Log("Uneasy feeling...");
        }

        if (timeOfDay >= 19)
        {
            NightPenalty();
        }
    }

    void UpdateSun()
    {
        float sunRotation = (timeOfDay / 24f) * 360f;
        sun.transform.rotation = Quaternion.Euler(sunRotation - 90, 170, 0);
    }

    void NightPenalty()
    {
        Debug.Log("Player stayed out too late!");
    }
}