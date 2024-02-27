using System;
using UnityEngine;

public class ConsoleHeaderEntry
{
    public Func<string> LabelFunc;
    public Func<Color> ColorFunc;

    public int Priority
    {
        get
        {
            return Priority;
        }
        set
        {
            Mathf.Max(0, value);
        }
    }

    public ConsoleHeaderEntry(Func<string> labelFunc, Func<Color> colorFunc, int priority)
    {
        LabelFunc = labelFunc;
        ColorFunc = colorFunc;
        Priority = priority;
    }
}