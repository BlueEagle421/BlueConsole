using UnityEngine;
using UnityEngine.SceneManagement;

public static class Commands
{
    [Command("quit", "closes the application")]
    public static void Quit()
    {
        Application.Quit();
    }

    [Command("reset", "resets current scene")]
    public static void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    [Command("version", "logs project name and version")]
    public static void Version()
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
    public static void Log_Warning(string message)
    {
        Debug.LogWarning(message);
    }
}
