using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

public class Console : MonoBehaviour
{
    [SerializeField] private ConsoleCommands _consoleCommands;
    [SerializeField] private ConsoleTypeParameters _consoleTypeParameters;
    [Tooltip("The amount of hints the console should generate for the user")]
    [SerializeField] private int _maxHintsAmount = 5;
    [Tooltip("A message that will always appear after toggling the console for the first time")]
    [SerializeField] private string _welcomeMessage = "Welcome to <color=#4895EF>BlueConsole</color>!";

    [Tooltip("A color that the message will appear in")]
    [SerializeField] private Color _logColor = Color.white, _errorColor = Color.red, _warningColor = Color.yellow, _exceptionColor = Color.red, _assertColor = Color.yellow, _executableColor = Color.cyan, _parametersColor;

    public int MaxHintsAmount { get { return _maxHintsAmount; } }
    public static string Content { get; private set; }
    public static bool IsToggled { get; private set; }
    public static List<string> Hints { get; private set; } = new();
    public static List<string> History { get; private set; } = new();
    public const string NO_TRACE = " [no stack trace] ";
    private static readonly List<string> _commandsIDs = new();
    private static readonly List<ConsoleCommand> _commands = new();
    private static readonly List<TypeParameter> _typeParameters = new();
    private static readonly Dictionary<Type, string> _typeRegexKeysDictionary = new();
    private static bool _wasToggledGlobally;
    private bool _wasToggledInScene;
    private int _currentHistoryRecall = 0;

    public Action<bool> OnConsoleToggled;
    public Action OnContentChanged;
    public Action OnHintsChanged;
    public Action<string> OnHistoryRecall;
    public Action<string> OnHintAccept;

    private void Start()
    {
        if (IsToggled)
            ToggleConsole(true);
    }

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    public void InputFieldChangedInput(string input)
    {
        SetHintsByInput(input);
    }

    public void EnterInput(string input)
    {
        ReadInput(input);
    }

    public void ToggleConsoleInput()
    {
        ToggleConsole(!IsToggled);
    }

    public void RecallHistoryUpInput()
    {
        RecallHistory(-1);
    }

    public void RecallHistoryDownInput()
    {
        RecallHistory(1);
    }

    public void AcceptHintInput()
    {
        AcceptHint();
    }

    private void SetupTypeParameters()
    {
        MethodInfo[] methodInfos = _consoleTypeParameters.GetType().GetMethods();

        for (int i = 0; i < methodInfos.Length; i++)
        {
            if (Attribute.IsDefined(methodInfos[i], typeof(TypeParameterAttribute)))
            {
                TypeParameterAttribute attribute = methodInfos[i].GetCustomAttribute(typeof(TypeParameterAttribute)) as TypeParameterAttribute;
                TypeParameter typeParameter = new(methodInfos[i], _consoleTypeParameters, attribute);
                _typeParameters.Add(typeParameter);

                _typeRegexKeysDictionary.Add(typeParameter.Type, typeParameter.RegexKey);
            }
        }
    }

    private void SetupCommands()
    {
        _consoleCommands.AttachConsole(this);

        _commands.Clear();

        MethodInfo[] methodInfos = _consoleCommands.GetType().GetMethods();

        for (int i = 0; i < methodInfos.Length; i++)
        {
            if (Attribute.IsDefined(methodInfos[i], typeof(ConsoleCommandAttribute)))
            {
                ConsoleCommandAttribute attribute = methodInfos[i].GetCustomAttribute(typeof(ConsoleCommandAttribute)) as ConsoleCommandAttribute;
                ConsoleCommand consoleCommand = new(methodInfos[i], _consoleCommands, attribute, _parametersColor);
                if (!consoleCommand.IsValid())
                {
                    Debug.LogWarning(string.Format("Command ({0}) is invalid and will not be executable", consoleCommand.Format));
                    continue;
                }

                _commands.Add(consoleCommand);
                _commandsIDs.Add(consoleCommand.ID);
            }
        }
    }

    private void ToggleConsole(bool toggle)
    {
        if (!IsToggled && toggle)
            ToggleOn();

        if (!_wasToggledGlobally)
            FirstGlobalToggleOn();

        if (!_wasToggledInScene)
            FirstToggleOnInScene();

        IsToggled = toggle;

        OnConsoleToggled?.Invoke(toggle);
    }

    private void ToggleOn()
    {

    }

    private void FirstGlobalToggleOn()
    {
        _wasToggledGlobally = true;
        SetupTypeParameters();
        ClearContent();
    }

    private void FirstToggleOnInScene()
    {
        _wasToggledInScene = true;
        SetupCommands();
        ResetCurrentHistoryRecall();
    }

    public void ClearContent()
    {
        Content = string.Empty;
        ContentChanged();
        AppendContentLine(_welcomeMessage, "", "FFFFFF");
    }

    private void ContentChanged()
    {
        OnContentChanged?.Invoke();
    }

    private void SetHintsByInput(string input)
    {
        Hints.Clear();

        string inputToCheck = input.Split(" ")[0];

        string perfectMatch = string.Empty;

        for (int i = 0; i < _commandsIDs.Count; i++)
        {
            string id = _commandsIDs[i];

            if (Hints.Count >= _maxHintsAmount)
                continue;

            if (Hints.Contains(id))
                continue;

            if (string.IsNullOrEmpty(inputToCheck))
                continue;

            try
            {
                string format = _commands.Find(x => x.ID == id).Format;

                if (Regex.IsMatch(id, inputToCheck))
                    Hints.Add(format);

                if (inputToCheck == id)
                    perfectMatch = format;
            }
            catch { }
        }

        if (!string.IsNullOrEmpty(perfectMatch) && Hints.Count > 0 && input.Contains(" "))
            Hints.RemoveRange(1, Hints.Count - 1);

        OnHintsChanged?.Invoke();
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        switch (type)
        {
            case LogType.Log:
                AppendContentLine(logString, "\n> ", ColorUtility.ToHtmlStringRGB(_logColor));
                break;
            case LogType.Error:
                AppendTracedLogContent(logString, stackTrace, "\n> [error] ", ColorUtility.ToHtmlStringRGB(_errorColor));
                ToggleConsole(true);
                break;
            case LogType.Warning:
                AppendContentLine(logString, "\n> [warning] ", ColorUtility.ToHtmlStringRGB(_warningColor));
                break;
            case LogType.Exception:
                AppendTracedLogContent(logString, stackTrace, "\n> [exception] ", ColorUtility.ToHtmlStringRGB(_exceptionColor));
                break;
            case LogType.Assert:
                AppendContentLine(logString, "\n> [assert] ", ColorUtility.ToHtmlStringRGB(_assertColor));
                break;
        }
    }

    private void AppendTracedLogContent(string logString, string stackTrace, string prefix, string colorHex)
    {
        if (logString.Contains(NO_TRACE))
        {
            logString = logString.Replace(NO_TRACE, "");
            stackTrace = string.Empty;
        }
        else
            stackTrace = "\n" + stackTrace;

        AppendContentLine(logString + stackTrace, prefix, colorHex);
    }

    private void AppendContentLine(string content, string prefix, string colorHex)
    {
        if (string.IsNullOrEmpty(content))
            return;

        string[] lines = content.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        Content += string.Format("<color=#{0}>", colorHex);
        for (int i = 0; i < lines.Length; i++)
        {
            string finalPrefix = (i == 0) ? prefix : "\n";
            Content += finalPrefix + lines[i];
        }
        Content += "</color>";
        ContentChanged();
    }

    private void ReadInput(string input)
    {
        if (!IsToggled)
            return;

        AppendHistory(input);

        if (FoundMatchingCommand(input))
            ExecuteCommandByInput(input);
        else
            Debug.Log(input);

    }

    private void AppendHistory(string input)
    {
        if (!string.IsNullOrEmpty(input))
            History.Add(input);

        ResetCurrentHistoryRecall();
    }

    private void ResetCurrentHistoryRecall()
    {
        _currentHistoryRecall = History.Count;
    }

    private void RecallHistory(int indexOffset)
    {
        int historySize = History.Count;

        if (historySize == 0)
            return;

        int indexToRecall = _currentHistoryRecall + indexOffset;
        string inputToRecall = string.Empty;

        if (indexToRecall >= historySize - 1)
            indexToRecall = historySize - 1;

        if (indexToRecall <= 0)
            indexToRecall = 0;

        if (indexOffset > 0 && _currentHistoryRecall >= historySize - 1)
        {
            inputToRecall = string.Empty;
            ResetCurrentHistoryRecall();
        }
        else
        {
            inputToRecall = History[indexToRecall];
            _currentHistoryRecall = indexToRecall;
        }


        OnHistoryRecall?.Invoke(inputToRecall);
    }

    private void AcceptHint()
    {
        if (Hints.Count == 0)
            return;

        OnHintAccept?.Invoke(_commands.Find(x => x.Format == Hints[0]).ID + " ");
    }

    public void DisplayHelp()
    {
        Debug.Log(string.Format("Found {0} executable commands:", _commands.Count.ToString()));

        for (int i = 0; i < _commands.Count; i++)
            Debug.Log(_commands[i].Format + " - " + _commands[i].Description);
    }

    private void ExecuteCommandByInput(string inputContent)
    {
        if (string.IsNullOrEmpty(inputContent))
            return;

        List<string> inputSplit = inputContent.Split(" ").ToList();

        string wantedID = inputSplit[0];
        inputSplit.RemoveAt(0);

        ConsoleCommand toExecute = _commands.Find(x => x.ID == wantedID);

        if (TryToParseParams(inputSplit, toExecute, out object[] parameters))
        {
            AppendContentLine(inputContent, "\n> ", ColorUtility.ToHtmlStringRGB(_executableColor));
            toExecute.MethodInfo.Invoke(toExecute.InvokingObject, parameters);
        }
        else
            Debug.LogError(string.Format("Invalid command format. Command did not execute ({0})", inputContent) + NO_TRACE);
    }

    private bool TryToParseParams(List<string> inputParams, ConsoleCommand toFormat, out object[] result)
    {
        ParameterInfo[] parameterInfos = toFormat.MethodInfo.GetParameters();
        result = new object[parameterInfos.Length];
        bool success = true;

        if (parameterInfos.Length == 0)
            return true;

        List<string> groupedInputParams = GroupParametersByRegexKeys(inputParams, toFormat.MethodInfo);

        for (int i = 0; i < parameterInfos.Length; i++)
        {
            ParameterInfo parameterInfo = toFormat.MethodInfo.GetParameters()[i];
            TypeParameter typeParameter = _typeParameters.Find(x => x.Type == parameterInfo.ParameterType);
            try
            {
                object formattedParam = typeParameter.MethodInfo.Invoke(typeParameter.InvokingObject, new object[] { groupedInputParams[i] });
                result[i] = formattedParam;
            }
            catch
            {
                Debug.LogError(string.Format("Invalid {0} parameter input in ({1})", typeParameter.Type.Name, inputParams.ElementAtOrDefault(i) == null ? string.Empty : inputParams[i]) + NO_TRACE);
                success = false;
            }
        }

        return success;
    }

    private List<string> GroupParametersByRegexKeys(List<string> inputParams, MethodInfo methodInfo)
    {
        if (inputParams.Count == 0)
            return null;

        List<string> result = new();
        string check = inputParams.Aggregate((a, b) => a + " " + b);
        ParameterInfo[] parameterInfos = methodInfo.GetParameters();


        for (int i = 0; i < parameterInfos.Length; i++)
        {
            ParameterInfo parameterInfo = methodInfo.GetParameters()[i];
            TypeParameter typeParameter = _typeParameters.Find(x => x.Type == parameterInfo.ParameterType);

            string formatted = string.Empty;

            Match match = Regex.Match(check, typeParameter.RegexKey);
            formatted = typeParameter.AllowWhitespaces ? match.Value : check.Split(" ")[i];

            if (!match.Success)
                return result;

            result.Add(formatted);
            check = ReplaceFirst(check, formatted, string.Empty);
        }

        return result;
    }

    private string ReplaceFirst(string text, string search, string replace)
    {
        int pos = text.IndexOf(search);
        if (pos < 0)
            return text;

        return text[..pos] + replace + text.Substring(pos + search.Length);
    }

    private bool FoundMatchingCommand(string inputContent)
    {
        if (string.IsNullOrEmpty(inputContent))
            return false;

        string ID = inputContent.Split(" ")[0];

        if (!_commandsIDs.Contains(ID))
            return false;

        return true;
    }

    private class ConsoleCommand
    {
        public string ID { get; private set; }
        public string Description { get; private set; }
        public string Format { get; private set; }
        public MethodInfo MethodInfo { get; private set; }
        public object InvokingObject { get; private set; }

        public ConsoleCommand(MethodInfo methodInfo, object invokingObject, ConsoleCommandAttribute attribute)
        {
            ID = attribute.ID;
            Description = attribute.Description;
            Format = DefineFormat(ID, methodInfo, ColorUtility.ToHtmlStringRGB(Color.white));
            MethodInfo = methodInfo;
            InvokingObject = invokingObject;
        }

        public ConsoleCommand(MethodInfo methodInfo, object invokingObject, ConsoleCommandAttribute attribute, Color parametersColor) : this(methodInfo, invokingObject, attribute)
        {
            Format = DefineFormat(ID, methodInfo, ColorUtility.ToHtmlStringRGB(parametersColor));
        }
        public bool IsValid()
        {
            if (!HasAllTypeParameters(MethodInfo))
                return false;

            return true;
        }

        private bool HasAllTypeParameters(MethodInfo methodInfo)
        {
            for (int i = 0; i < MethodInfo.GetParameters().Length; i++)
            {
                Type type = MethodInfo.GetParameters()[i].ParameterType;
                if (!_typeRegexKeysDictionary.TryGetValue(type, out string typeKey))
                    return false;
            }

            return true;
        }

        private string DefineFormat(string ID, MethodInfo methodInfo, string colorHex)
        {
            string result = ID;

            for (int i = 0; i < methodInfo.GetParameters().Length; i++)
            {
                result += string.Format(" <color=#{0}>{1}</color>", colorHex, methodInfo.GetParameters()[i].Name);
            }

            return result;
        }
    }

    private class TypeParameter
    {
        public Type Type { get; private set; }
        public string RegexKey { get; private set; }
        public bool AllowWhitespaces { get; private set; }
        public MethodInfo MethodInfo { get; private set; }
        public object InvokingObject { get; private set; }

        public TypeParameter(MethodInfo methodInfo, object invokingObject, TypeParameterAttribute attribute)
        {
            Type = methodInfo.ReturnType;
            RegexKey = attribute.RegexKey;
            AllowWhitespaces = attribute.AllowWhitespaces;
            MethodInfo = methodInfo;
            InvokingObject = invokingObject;
        }
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class ConsoleCommandAttribute : Attribute
{
    public string ID { get; private set; }
    public string Description { get; private set; }

    public ConsoleCommandAttribute(string id, string description)
    {
        ID = id;
        Description = description;
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class TypeParameterAttribute : Attribute
{
    public string RegexKey { get; private set; }
    public bool AllowWhitespaces { get; private set; }

    public TypeParameterAttribute(string description, bool allowWhitespaces)
    {
        RegexKey = description;
        AllowWhitespaces = allowWhitespaces;
    }
}
