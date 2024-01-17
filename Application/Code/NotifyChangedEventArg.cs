
using System;
using UnityEngine;
public class NotifyChangedEventArg : EventArgs
{
    public int Time;
    public NotifyChangedEventArg(int Time) => this.Time = Time;
}

