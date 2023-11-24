using System.Globalization;
using UnityEngine;

public static class ConsoleTypeParameters
{
    [TypeParameter(@"[0|1]")]
    public static bool BoolParameter(string inputParam)
    {
        switch (inputParam)
        {
            case "true": return true;
            case "on": return true;
            case "1": return true;
            case "yes": return true;
            case "false": return false;
            case "off": return false;
            case "0": return false;
            case "no": return false;
            default: return false;
        }
    }

    [TypeParameter(@"\d+")]
    public static byte ByteParameter(string inputParam)
    {
        return byte.Parse(inputParam);
    }

    [TypeParameter(@"[-+]?\d+")]
    public static sbyte SByteParameter(string inputParam)
    {
        return sbyte.Parse(inputParam);
    }

    [TypeParameter(@"[-+]?\d+")]
    public static int IntParameter(string inputParam)
    {
        return int.Parse(inputParam);
    }

    [TypeParameter(@"[-+]?\d+")]
    public static uint UIntParameter(string inputParam)
    {
        return uint.Parse(inputParam);
    }

    [TypeParameter(@"[-+]?([0-9]*[.])?[0-9]+([eE][-+]?\d+)?")]
    public static decimal DecimalParameter(string inputParam)
    {
        return decimal.Parse(inputParam, NumberStyles.Any, CultureInfo.InvariantCulture);
    }

    [TypeParameter(@"[-+]?([0-9]*[.])?[0-9]+([eE][-+]?\d+)?")]
    public static double DoubleParameter(string inputParam)
    {
        return double.Parse(inputParam, NumberStyles.Any, CultureInfo.InvariantCulture);
    }

    [TypeParameter(@"[-+]?([0-9]*[.])?[0-9]+([eE][-+]?\d+)?")]
    public static float FloatParameter(string inputParam)
    {
        return float.Parse(inputParam, NumberStyles.Any, CultureInfo.InvariantCulture);
    }

    [TypeParameter("\".*?\"", true)]
    public static string StringParameter(string inputParam)
    {
        return inputParam.Replace("\"", string.Empty);
    }

    [TypeParameter("'.'")]
    public static char CharParameter(string inputParam)
    {
        return inputParam.Replace("'", string.Empty).ToCharArray()[0];
    }
}
