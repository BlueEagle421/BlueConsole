using UnityEngine;
using UnityEngine.SceneManagement;

public class ConsoleCommands : MonoBehaviour
{
    private Console _attachedConsole;

    public void AttachConsole(Console console)
    {
        _attachedConsole = console;
    }

    [Command("help", "displays all commands")]
    public void Help()
    {
        _attachedConsole.DisplayHelp();
    }

    [Command("clear", "clears the console content")]
    public void Clear()
    {
        _attachedConsole.ClearContent();
    }

    [Command("quit", "closes the application")]
    public void Quit()
    {
        Application.Quit();
    }

    [Command("reset", "resets current scene")]
    public static void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    [Command("version", "logs project name and version")]
    public void Version()
    {
        Debug.Log(string.Format("{0} version: {1}", Application.productName, Application.version));
    }

    [Command("log", "logs")]
    public static void Log(string message)
    {
        Debug.Log(message);
    }

    [Command("log_error", "logs an error without stack trace")]
    public static void Log_Error(string message, bool trace)
    {
        Debug.LogError(message + (trace ? string.Empty : Console.NO_TRACE));
    }

    [Command("log_warning", "logs warning")]
    public void Log_Warning(string message)
    {
        Debug.LogWarning(message);
    }

    [Command("history", "logs input history")]
    public void History()
    {
        for (int i = 0; i < Console.History.Count; i++)
            Debug.Log(i + ". " + Console.History[i]);
    }
}
