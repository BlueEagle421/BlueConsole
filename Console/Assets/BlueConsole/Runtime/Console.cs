using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditorInternal;
using UnityEngine;

public class Console : MonoBehaviour
{
    [Tooltip("Should the default assembly be searched for static commands?")]
    [SerializeField] private bool _includeAssemblyCSharp;
    [Tooltip("These assemblies will be searched for static commands")]
    [SerializeField] private List<AssemblyDefinitionAsset> _assembliesWithStaticCommands;
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
    private static readonly List<Command> _commands = new();
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

    private void LoadTypeParameters()
    {
        MethodInfo[] methodInfos = typeof(ConsoleTypeParameters).GetMethods();

        for (int i = 0; i < methodInfos.Length; i++)
        {
            if (Attribute.IsDefined(methodInfos[i], typeof(TypeParameterAttribute)))
            {
                TypeParameterAttribute attribute = methodInfos[i].GetCustomAttribute(typeof(TypeParameterAttribute)) as TypeParameterAttribute;
                TypeParameter typeParameter = new(methodInfos[i], attribute);
                _typeParameters.Add(typeParameter);

                _typeRegexKeysDictionary.Add(typeParameter.Type, typeParameter.RegexKey);
            }
        }
    }

    private List<Assembly> GetAllConsoleAssemblies()
    {
        List<string> userDefinedAssembliesNames = new();

        foreach (AssemblyDefinitionAsset assemblyAsset in _assembliesWithStaticCommands)
            userDefinedAssembliesNames.Add(GetAssemblyNameFromAsset(assemblyAsset));

        List<Assembly> allAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

        List<Assembly> result = allAssemblies.FindAll(x => userDefinedAssembliesNames.Contains(x.GetName().Name));

        if (_includeAssemblyCSharp)
            result.Add(allAssemblies.Find(x => x.GetName().Name == "Assembly-CSharp"));

        result.Add(GetType().Assembly);

        return result;
    }

    private void LoadStaticCommands()
    {
        foreach (Assembly assembly in GetAllConsoleAssemblies())
        {
            Type[] assemblyClasses = Array.FindAll(assembly.GetTypes(), x => x.IsClass);

            foreach (Type assemblyClass in assemblyClasses)
            {
                MethodInfo[] methodInfos = assemblyClass.GetMethods();

                for (int i = 0; i < methodInfos.Length; i++)
                {
                    if (!methodInfos[i].IsStatic)
                        continue;

                    AddCommandToList(methodInfos[i], null);
                }
            }
        }
    }

    private void LoadMonoBehaviourCommands()
    {
        _commands.RemoveAll(x => !x.IsStatic);


    }

    private void AddCommandToList(MethodInfo methodInfo, List<object> invokingObjects)
    {
        if (Attribute.IsDefined(methodInfo, typeof(CommandAttribute)))
        {
            CommandAttribute attribute = methodInfo.GetCustomAttribute(typeof(CommandAttribute)) as CommandAttribute;
            Command consoleCommand = new(methodInfo, attribute, _parametersColor);
            if (!consoleCommand.IsValid())
            {
                Debug.LogWarning(string.Format("Command ({0}) is invalid and will not be executable", consoleCommand.Format));
                return;
            }

            _commands.Add(consoleCommand);
            _commandsIDs.Add(consoleCommand.ID);
        }
    }

    private string GetAssemblyNameFromAsset(AssemblyDefinitionAsset assemblyAsset)
    {
        string fileContent = assemblyAsset.text;

        Match match = Regex.Match(fileContent, "\"name\":\\s*\"([^\"]+)\"");
        if (match.Success)
        {
            return match.Groups[1].Value;
        }
        else
        {
            Debug.LogWarning("Unable to extract assembly name from AssemblyDefinitionAsset: " + assemblyAsset.name);
            return null;
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
        LoadTypeParameters();
        LoadStaticCommands();
        ClearContent();
    }

    private void FirstToggleOnInScene()
    {
        _wasToggledInScene = true;
        LoadMonoBehaviourCommands();
        ResetCurrentHistoryRecall();
    }

    [Command("clear", "clears the console content")]
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
        if (indexToRecall >= historySize - 1)
            indexToRecall = historySize - 1;

        if (indexToRecall <= 0)
            indexToRecall = 0;

        string inputToRecall;
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

    public static void DisplayHelp()
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

        Command toExecute = _commands.Find(x => x.ID == wantedID);

        if (TryToParseParams(inputSplit, toExecute, out object[] parameters))
        {
            AppendContentLine(inputContent, "\n> ", ColorUtility.ToHtmlStringRGB(_executableColor));
            toExecute.MethodInfo.Invoke(toExecute.InvokingObjects, parameters);
        }
        else
            Debug.LogError(string.Format("Invalid command format. Command did not execute ({0})", inputContent) + NO_TRACE);
    }

    private bool TryToParseParams(List<string> inputParams, Command toFormat, out object[] result)
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

    private class Command
    {
        public string ID { get; private set; }
        public string Description { get; private set; }
        public string Format { get; private set; }
        public MethodInfo MethodInfo { get; private set; }
        public List<object> InvokingObjects { get; private set; } = new();
        public bool IsStatic { get; private set; }

        public Command(MethodInfo methodInfo, CommandAttribute attribute)
        {
            ID = string.IsNullOrEmpty(attribute.ID) ? methodInfo.Name : attribute.ID;
            Description = attribute.Description;
            Format = DefineFormat(ID, methodInfo, ColorUtility.ToHtmlStringRGB(Color.white));
            MethodInfo = methodInfo;
            IsStatic = methodInfo.IsStatic;
        }

        public void AddInvokingObject(object invoker)
        {
            InvokingObjects.Add(invoker);
        }

        public void AddInvokingObjects(List<object> invokers)
        {
            foreach (object toAdd in invokers)
                InvokingObjects.Add(toAdd);
        }

        public Command(MethodInfo methodInfo, CommandAttribute attribute, Color parametersColor) : this(methodInfo, attribute)
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

        public TypeParameter(MethodInfo methodInfo, TypeParameterAttribute attribute)
        {
            Type = methodInfo.ReturnType;
            RegexKey = attribute.RegexKey;
            AllowWhitespaces = attribute.AllowWhitespaces;
            MethodInfo = methodInfo;
            InvokingObject = null;
        }
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class CommandAttribute : Attribute
{
    public string ID { get; private set; }
    public string Description { get; private set; }

    public CommandAttribute()
    {

    }

    public CommandAttribute(string id) : this()
    {
        ID = id;
    }

    public CommandAttribute(string id, string description) : this(id)
    {
        Description = description;
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class TypeParameterAttribute : Attribute
{
    public string RegexKey { get; private set; }
    public bool AllowWhitespaces { get; private set; } = false;

    public TypeParameterAttribute(string description)
    {
        RegexKey = description;
    }

    public TypeParameterAttribute(string description, bool allowWhitespaces) : this(description)
    {
        RegexKey = description;
        AllowWhitespaces = allowWhitespaces;
    }
}
