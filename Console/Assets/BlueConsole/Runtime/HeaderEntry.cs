using System;
using UnityEngine;

public class HeaderEntry
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
            //if priority is 0 it will be ignored and added as the last one
            Mathf.Max(0, value);
        }
    }

    public HeaderEntry(Func<string> labelFunc, Func<Color> colorFunc, int priority)
    {
        LabelFunc = labelFunc;
        ColorFunc = colorFunc;
        Priority = priority;
    }
}