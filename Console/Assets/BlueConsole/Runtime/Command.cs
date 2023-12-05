using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

public class Command
{
    public string ID { get; private set; }
    public string Description { get; private set; }
    public string Format { get; private set; }
    public MethodInfo MethodInfo { get; private set; }
    public List<object> InvokingObjects { get; private set; } = new();
    public InstanceTargetType InstanceTargetType { get; private set; }
    public bool IsStatic { get; private set; }

    public Command(MethodInfo methodInfo, CommandAttribute attribute)
    {
        ID = string.IsNullOrEmpty(attribute.ID) ? methodInfo.Name : attribute.ID;
        Description = attribute.Description;
        Format = DefineFormat(ID, methodInfo, ColorUtility.ToHtmlStringRGB(Color.white));
        MethodInfo = methodInfo;
        InstanceTargetType = attribute.InstanceTargetType;
        IsStatic = methodInfo.IsStatic;
    }

    public void Invoke(object[] parameters)
    {
        if (IsStatic)
            DoStaticInvoke(parameters);
        else
            DoInstanceInvoke(parameters);
    }

    private void DoStaticInvoke(object[] parameters)
    {
        MethodInfo.Invoke(null, parameters);
    }

    private void DoInstanceInvoke(object[] parameters)
    {
        switch (InstanceTargetType)
        {
            case InstanceTargetType.All:
                {
                    foreach (object invoker in InvokingObjects)
                    {
                        MethodInfo.Invoke(invoker, parameters);
                    }
                    break;
                }
            case InstanceTargetType.First:
                {
                    MethodInfo.Invoke(InvokingObjects[0], parameters);
                    break;
                }
        }
    }

    public void AddInvokingObject(object invoker)
    {
        InvokingObjects.Add(invoker);
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
            if (!Console.TypeRegexKeysDictionary.TryGetValue(type, out string typeKey))
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

    public string AssemblyLabel()
    {
        if (IsStatic)
            return (InvokingObjects[0] as Type).Assembly.GetName().Name;

        return InvokingObjects[0].GetType().Assembly.GetName().Name;
    }

    public string InvokingClassLabel()
    {
        if (IsStatic)
            return InvokingObjects[0].ToString();

        return InvokingObjects[0].GetType().Name;
    }

    public string ParametersTypesLabel()
    {
        if (MethodInfo.GetParameters().Length == 0)
            return "none";

        List<string> parameters = new();

        foreach (ParameterInfo param in MethodInfo.GetParameters())
            parameters.Add(string.Format("({0}) {1}", param.ParameterType.Name, param.Name));

        return parameters.Aggregate((a, b) => a + " " + b);

    }

    public string TargetTypeLabel()
    {
        if (IsStatic)
            return "static";

        return InstanceTargetType switch
        {
            InstanceTargetType.All => "all instances",
            InstanceTargetType.First => "first instance",
            _ => "invalid",
        };
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class CommandAttribute : Attribute
{
    public string ID { get; private set; }
    public string Description { get; private set; }
    public InstanceTargetType InstanceTargetType { get; private set; }

    public CommandAttribute([Optional] string id, [Optional] string description, [Optional] InstanceTargetType instanceTargetType)
    {
        ID = id;
        Description = description;
        InstanceTargetType = instanceTargetType;

    }
}

public enum InstanceTargetType
{
    First,
    All
}