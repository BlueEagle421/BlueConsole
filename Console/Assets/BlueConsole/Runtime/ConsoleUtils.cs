using System;

public static class ConsoleUtils
{
    public static void SetActionListener<T>(ref Action<T> action, Action<T> listener, bool subscribe)
    {
        if (subscribe)
            action += listener;
        else
            action -= listener;
    }

    public static void SetActionListener(ref Action action, Action listener, bool subscribe)
    {
        if (subscribe)
            action += listener;
        else
            action -= listener;
    }
}
