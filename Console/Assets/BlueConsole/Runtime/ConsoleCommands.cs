using UnityEngine;
using UnityEngine.SceneManagement;

public class ConsoleCommands : MonoBehaviour
{
    private Console _attachedConsole;

    public void AttachConsole(Console console)
    {
        _attachedConsole = console;
    }

    [ConsoleCommand("help", "displays all commands")]
    public void Help()
    {
        _attachedConsole.DisplayHelp();
    }

    [ConsoleCommand("clear", "clears the console content")]
    public void Clear()
    {
        _attachedConsole.ClearContent();
    }

    [ConsoleCommand("quit", "closes the application")]
    public void Quit()
    {
        Application.Quit();
    }

    [ConsoleCommand("reset", "resets current scene")]
    public void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    [ConsoleCommand("version", "logs project name and version")]
    public void Version()
    {
        Debug.Log(string.Format("{0} version: {1}", Application.productName, Application.version));
    }

    [ConsoleCommand("log", "logs")]
    public void Log(string input)
    {
        Debug.Log(input);
    }

    [ConsoleCommand("log_error", "logs an error without stack trace")]
    public void Log_Error(string input)
    {
        Debug.LogError(input + Console.NO_TRACE);
    }

    [ConsoleCommand("log_warning", "logs warning")]
    public void Log_Warning(string input)
    {
        Debug.LogWarning(input);
    }

    [ConsoleCommand("history", "logs input history")]
    public void History()
    {
        for (int i = 0; i < Console.History.Count; i++)
            Debug.Log(Console.History[i]);
    }
}
