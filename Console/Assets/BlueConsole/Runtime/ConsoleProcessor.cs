using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace BlueConsole
{
    [DefaultExecutionOrder(-1)]
    public class ConsoleProcessor : MonoBehaviour
    {
        [Tooltip("Should the default assembly be searched for commands?")]
        [SerializeField] private bool _includeAssemblyCSharp = true;
        [Tooltip("These assemblies will be searched for commands. Entered names should come directly from AssemblyDefinitionAsset Name field")]
        [SerializeField] private List<string> _assembliesNamesWithCommands;
        [Tooltip("The amount of hints the console should generate for the user")]
        [field: SerializeField] public int MaxHintsAmount { get; private set; } = 5;
        [Tooltip("A message that will always appear after toggling the console for the first time")]
        [SerializeField] private string _welcomeMessage = "Welcome to <color=#4895EF>BlueConsole</color>!";

        [Tooltip("A color that the message will appear in")]
        [field: SerializeField] public Color LogColor { get; private set; } = Color.white;

        [Tooltip("A color that the message will appear in")]
        [field: SerializeField] public Color ErrorColor { get; private set; } = Color.red;

        [Tooltip("A color that the message will appear in")]
        [field: SerializeField] public Color WarningColor { get; private set; } = Color.yellow;

        [Tooltip("A color that the message will appear in")]
        [field: SerializeField] public Color ExceptionColor { get; private set; } = Color.red;

        [Tooltip("A color that the message will appear in")]
        [field: SerializeField] public Color AssertColor { get; private set; } = Color.yellow;

        [Tooltip("A color that the message will appear in")]
        [field: SerializeField] public Color ExecutableColor { get; private set; } = Color.blue;

        [Tooltip("A color that the parameters will appear in")]
        [field: SerializeField] public Color ParametersColor { get; private set; } = Color.white;

        public static string Content { get; private set; }
        public static bool IsToggled { get; private set; }
        public static List<string> Hints { get; private set; } = new();
        public static List<string> History { get; private set; } = new();
        public const string NO_TRACE = " [no stack trace] ";
        public static readonly List<Command> Commands = new();
        private static readonly List<TypeParameter> _typeParameters = new();
        private static List<Assembly> _commandsAssemblies = new();
        public static readonly Dictionary<Type, string> TypeRegexKeysDictionary = new();
        private static bool _wasEnabledGlobally;
        private static bool _wasToggledGlobally;
        private bool _wasToggledInScene;
        private int _currentHistoryRecall = 0;

        public static ConsoleProcessor Current;

        public Action<bool> OnConsoleToggled;
        public Action OnContentChanged;
        public Action OnHintsChanged;
        public Action<string> OnHistoryRecall;
        public Action<string> OnHintAccept;

        private void Awake()
        {
            Current = this;
        }

        private void Start()
        {
            if (IsToggled)
                Toggle(true);
        }

        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;

            if (!_wasEnabledGlobally)
                FirstGlobalEnable();
        }

        private void FirstGlobalEnable()
        {
            _wasEnabledGlobally = true;

            ClearContent();
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
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

                    TypeRegexKeysDictionary.Add(typeParameter.Type, typeParameter.RegexKey);
                }
            }
        }

        private void LoadConsoleAssemblies()
        {
            List<string> userDefinedAssembliesNames = new();

            foreach (string assemblyName in _assembliesNamesWithCommands)
                userDefinedAssembliesNames.Add(assemblyName);

            List<Assembly> allAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

            _commandsAssemblies = allAssemblies.FindAll(x => userDefinedAssembliesNames.Contains(x.GetName().Name));

            if (_includeAssemblyCSharp)
                _commandsAssemblies.Add(allAssemblies.Find(x => x.GetName().Name == "Assembly-CSharp"));

            _commandsAssemblies.Add(GetType().Assembly);
        }

        private void LoadStaticCommands()
        {
            foreach (Assembly assembly in _commandsAssemblies)
            {
                Type[] assemblyClasses = Array.FindAll(assembly.GetTypes(), x => x.IsClass);

                foreach (Type assemblyClass in assemblyClasses)
                {
                    MethodInfo[] methodInfos = assemblyClass.GetMethods();

                    for (int i = 0; i < methodInfos.Length; i++)
                    {
                        if (!methodInfos[i].IsStatic)
                            continue;

                        AddCommandToList(methodInfos[i], assemblyClass);
                    }
                }
            }
        }

        private void LoadInstanceCommands()
        {
            Commands.RemoveAll(x => !x.IsStatic);

            MonoBehaviour[] components = FindObjectsOfType<MonoBehaviour>();

            foreach (MonoBehaviour component in components)
            {
                Type componentClass = component.GetType();
                if (_commandsAssemblies.Contains(componentClass.Assembly))
                {
                    MethodInfo[] methodInfos = componentClass.GetMethods();

                    for (int i = 0; i < methodInfos.Length; i++)
                    {
                        if (methodInfos[i].IsStatic)
                            continue;

                        AddCommandToList(methodInfos[i], component);
                    }
                }
            }
        }

        private void AddCommandToList(MethodInfo methodInfo, object invokingObject)
        {
            if (Attribute.IsDefined(methodInfo, typeof(CommandAttribute)))
            {
                CommandAttribute attribute = methodInfo.GetCustomAttribute(typeof(CommandAttribute)) as CommandAttribute;
                Command consoleCommand = new(methodInfo, attribute, ParametersColor);

                if (invokingObject != null)
                {
                    if (Commands.Exists(x => x.MethodInfo == methodInfo))
                    {
                        Commands[Commands.FindIndex(x => x.MethodInfo == methodInfo)].AddInvokingObject(invokingObject);
                        return;
                    }
                    else
                        consoleCommand.AddInvokingObject(invokingObject);
                }

                if (!consoleCommand.IsValid())
                {
                    Debug.LogWarning(string.Format("Command ({0}) is invalid and will not be executable", consoleCommand.Format));
                    return;
                }

                Commands.Add(consoleCommand);
            }
        }
        public void InvertToggle()
        {
            Toggle(!IsToggled);
        }

        public void Toggle(bool toggle)
        {
            if (toggle)
                ToggledOn();

            IsToggled = toggle;

            OnConsoleToggled?.Invoke(toggle);

            CheckFirstToggles();
        }

        private void ToggledOn()
        {

        }

        private void CheckFirstToggles()
        {
            if (!_wasToggledGlobally)
                FirstGlobalToggleOn();

            if (!_wasToggledInScene)
                FirstToggleOnInScene();
        }

        private void FirstGlobalToggleOn()
        {
            _wasToggledGlobally = true;
            LoadTypeParameters();
            LoadConsoleAssemblies();
            LoadStaticCommands();
        }

        private void FirstToggleOnInScene()
        {
            _wasToggledInScene = true;
            LoadInstanceCommands();
            ResetCurrentHistoryRecall();
        }

        private void ContentChanged()
        {
            OnContentChanged?.Invoke();
        }

        public void GenerateHints(string input)
        {
            Hints.Clear();

            string inputToCheck = input.Split(" ")[0];

            string perfectMatch = string.Empty;

            for (int i = 0; i < Commands.Count; i++)
            {
                string id = Commands[i].ID;

                if (Hints.Count >= MaxHintsAmount)
                    continue;

                if (Hints.Contains(id))
                    continue;

                if (string.IsNullOrEmpty(inputToCheck))
                    continue;

                try
                {
                    string format = Commands.Find(x => x.ID == id).Format;

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
                    AppendContentLine(logString, "\n> ", ColorUtility.ToHtmlStringRGB(LogColor));
                    break;
                case LogType.Error:
                    AppendTracedLogContent(logString, stackTrace, "\n> [error] ", ColorUtility.ToHtmlStringRGB(ErrorColor));
                    Toggle(true);
                    break;
                case LogType.Warning:
                    AppendContentLine(logString, "\n> [warning] ", ColorUtility.ToHtmlStringRGB(WarningColor));
                    break;
                case LogType.Exception:
                    AppendTracedLogContent(logString, stackTrace, "\n> [exception] ", ColorUtility.ToHtmlStringRGB(ExceptionColor));
                    break;
                case LogType.Assert:
                    AppendContentLine(logString, "\n> [assert] ", ColorUtility.ToHtmlStringRGB(AssertColor));
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

        public void ReadInput(string input)
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

        public void RecallHistory(int indexOffset)
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

        public void AcceptHint()
        {
            if (Hints.Count == 0)
                return;

            Command toHint = Commands.Find(x => x.Format == Hints[0]);
            OnHintAccept?.Invoke(toHint.ID + (toHint.MethodInfo.GetParameters().Length > 0 ? " " : string.Empty));
        }

        private void ExecuteCommandByInput(string inputContent)
        {
            if (string.IsNullOrEmpty(inputContent))
                return;

            List<string> inputSplit = inputContent.Split(" ").ToList();

            string wantedID = inputSplit[0];
            inputSplit.RemoveAt(0);

            Command toExecute = Commands.Find(x => x.ID == wantedID);

            if (TryToParseParams(inputSplit, toExecute, out object[] parameters))
            {
                AppendContentLine(inputContent, "\n> ", ColorUtility.ToHtmlStringRGB(ExecutableColor));
                toExecute.Invoke(parameters);
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
                    Debug.LogError(string.Format("Expected {0} parameter input in ({1})", typeParameter.Type.Name, inputParams.ElementAtOrDefault(i) == null ? string.Empty : inputParams[i]) + NO_TRACE);
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

            string id = inputContent.Split(" ")[0];

            if (!Commands.Exists(x => x.ID == id))
                return false;

            return true;
        }

        [Command("clear", "clears the console content")]
        public void ClearContent()
        {
            Content = string.Empty;
            ContentChanged();
            AppendContentLine(_welcomeMessage, "", "FFFFFF");
        }

        [Command("help", "displays all commands")]
        public void DisplayHelp()
        {
            Debug.Log(string.Format("Found {0} executable commands:", Commands.Count.ToString()));

            for (int i = 0; i < Commands.Count; i++)
                Debug.Log(Commands[i].Format + (string.IsNullOrEmpty(Commands[i].Description) ? string.Empty : " - " + Commands[i].Description));
        }

        [Command("params", "displays all parameter types")]
        public void DisplayParameters()
        {
            Debug.Log(string.Format("Found {0} parameter types:", TypeRegexKeysDictionary.Count.ToString()));

            for (int i = 0; i < TypeRegexKeysDictionary.Count; i++)
                Debug.Log(TypeRegexKeysDictionary.Keys.ElementAt(i).Name);
        }

        [Command("history", "logs input history")]
        public void DisplayHistory()
        {
            for (int i = 0; i < History.Count; i++)
                Debug.Log(i + ". " + History[i]);
        }

        [Command("man", "displays extended information about a command")]
        public void Man(Command command)
        {
            Debug.Log("Description: " + command.Description);
            Debug.Log("Parameters: " + command.ParametersTypesLabel(ColorUtility.ToHtmlStringRGB(ParametersColor)));
            Debug.Log("Target: " + command.TargetTypeLabel());
            Debug.Log("Source class name: " + command.InvokingClassLabel());
            Debug.Log("Source assembly name: " + command.AssemblyLabel());
        }
    }
}