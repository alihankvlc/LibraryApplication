using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using TMPro;
using Alihan.UI;

[System.Serializable]
public class TimeTracker
{
    #region Variables
    [SerializeField] private float m_TimeMultiplier = 6000;

    private int m_Days = 1;
    private int m_Hours;
    private int m_Minute;
    private bool m_IsNight;

    private TextMeshProUGUI m_TimeTextMesh;
    private float m_ElapsedSeconds;
    private List<string> m_DaysList;

    public event EventHandler<NotifyChangedEventArg> DayChangedEvent;
    public event EventHandler<NotifyChangedEventArg> HourChangedEvent;
    public event EventHandler<NotifyChangedEventArg> MinuteChangedEvent;
    #endregion

    #region Properties
    public bool IsNight => m_IsNight;
    public int Day
    {
        get => m_Days;
        private set => SetAndInvokeIfChanged(ref m_Days, value, OnDayChanged);
    }
    public int Hour
    {
        get => m_Hours;
        private set => SetAndInvokeIfChanged(ref m_Hours, value, OnHourChanged);
    }
    public int Minute
    {
        get => m_Minute;
        private set => SetAndInvokeIfChanged(ref m_Minute, value, OnMinuteChanged);
    }
    #endregion
    #region Functions
    public TimeTracker(TextMeshProUGUI timeTextMesh)
    {
        m_TimeTextMesh = timeTextMesh;
    }
    public void InitializeTimeTracker()
    {
        string[] daysArray = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
        m_DaysList = daysArray.ToList();
        m_Days = 1;
        m_TimeTextMesh.SetText(GetFormattedTime());
    }
    public string GetFormattedTime() =>
        $"DAY:{Day} {Hour:D2}:{Minute:D2}\n{m_DaysList[(m_Days + 6) % m_DaysList.Count]}";

    public void UpdateTime()
    {
        m_ElapsedSeconds += Time.deltaTime * m_TimeMultiplier;
        while (m_ElapsedSeconds > 59)
        {
            IncrementMinute();
            m_ElapsedSeconds -= 60;
            m_TimeTextMesh.SetText(GetFormattedTime());
        }
    }
    private void IncrementMinute()
    {
        Minute++;
        if (Minute > 59)
        {
            Minute = 0;
            IncrementHour();
        }
    }
    private void IncrementHour()
    {
        Hour++;

        if (Hour > 23)
        {
            Hour = 0;
            Day++;
        }
    }
    private void SetAndInvokeIfChanged(ref int field, int value, Action action)
    {
        if (field != value)
        {
            field = value;
            action?.Invoke();
        }
    }
    protected virtual void OnDayChanged() =>
        DayChangedEvent?.Invoke(this, new NotifyChangedEventArg(Day));
    protected virtual void OnHourChanged() =>
        HourChangedEvent?.Invoke(this, new NotifyChangedEventArg(Hour));
    protected virtual void OnMinuteChanged() =>
        MinuteChangedEvent?.Invoke(this, new NotifyChangedEventArg(Minute));
    #endregion
}
