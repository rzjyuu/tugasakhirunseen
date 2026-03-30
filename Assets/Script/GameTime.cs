using UnityEngine;
using TMPro;

public class GameTime : MonoBehaviour
{
    [Header("Start Time Per Night")]
    public int startHour = 0;
    public int startMinute = 0;

    [Header("Current Time")]
    public int hour;
    public int minute;

    [Header("Settings")]
    public float realSecondsPerMinute = 0.1f;

    [Header("UI")]
    public TextMeshProUGUI timeText;

    float timer;
    bool sixAMTriggered;

    void Start()
    {
        ApplyStartTime();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= realSecondsPerMinute)
        {
            timer = 0f;
            AddMinute();
        }

        UpdateUI();
    }

    void ApplyStartTime()
    {
        hour = startHour;
        minute = startMinute;
        sixAMTriggered = false;
    }

    void AddMinute()
    {
        minute++;

        if (minute >= 60)
        {
            minute = 0;
            hour++;
        }

        if (hour >= 24)
            hour = 0;
    }

    void UpdateUI()
    {
        if (timeText)
            timeText.text = $"{hour:00}:{minute:00}";
    }

    public bool IsSixAM()
    {
        if (!sixAMTriggered && hour >= 6)
        {
            sixAMTriggered = true;
            return true;
        }

        return false;
    }

    public void ResetTime()
    {
        ApplyStartTime();
    }
}