
using System;
using System.Reflection;
using System.Runtime.InteropServices;

public class TypeParameter
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

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class TypeParameterAttribute : Attribute
{
    public string RegexKey { get; private set; }
    public bool AllowWhitespaces { get; private set; } = false;

    public TypeParameterAttribute(string description, [Optional] bool allowWhitespaces)
    {
        RegexKey = description;
        AllowWhitespaces = allowWhitespaces;
    }
}