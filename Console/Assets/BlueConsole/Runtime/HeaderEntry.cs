using System;

public class ConsoleHeaderEntry
{
    public Func<string> LabelFunc;
    public Func<UnityEngine.Color> ColorFunc;

    public ConsoleHeaderEntry(Func<string> labelFunc, Func<UnityEngine.Color> colorFunc)
    {
        LabelFunc = labelFunc;
        ColorFunc = colorFunc;
    }
}