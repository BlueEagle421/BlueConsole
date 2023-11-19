using System.Globalization;
using UnityEngine;

public class ConsoleTypeParameters : MonoBehaviour
{
    [TypeParameter(@"[0|1]", false)]
    public bool BoolParameter(string inputParam)
    {
        return inputParam == "1";
    }

    [TypeParameter(@"\d+", false)]
    public byte ByteParameter(string inputParam)
    {
        return byte.Parse(inputParam);
    }

    [TypeParameter(@"[-+]?\d+", false)]
    public sbyte SByteParameter(string inputParam)
    {
        return sbyte.Parse(inputParam);
    }

    [TypeParameter(@"[-+]?\d+", false)]
    public int IntParameter(string inputParam)
    {
        return int.Parse(inputParam);
    }

    [TypeParameter(@"[-+]?\d+", false)]
    public uint UIntParameter(string inputParam)
    {
        return uint.Parse(inputParam);
    }

    [TypeParameter(@"[-+]?([0-9]*[.])?[0-9]+([eE][-+]?\d+)?", false)]
    public decimal DecimalParameter(string inputParam)
    {
        return decimal.Parse(inputParam, NumberStyles.Any, CultureInfo.InvariantCulture);
    }

    [TypeParameter(@"[-+]?([0-9]*[.])?[0-9]+([eE][-+]?\d+)?", false)]
    public double DoubleParameter(string inputParam)
    {
        return double.Parse(inputParam, NumberStyles.Any, CultureInfo.InvariantCulture);
    }

    [TypeParameter(@"[-+]?([0-9]*[.])?[0-9]+([eE][-+]?\d+)?", false)]
    public float FloatParameter(string inputParam)
    {
        return float.Parse(inputParam, NumberStyles.Any, CultureInfo.InvariantCulture);
    }

    [TypeParameter("\".*?\"", true)]
    public string StringParameter(string inputParam)
    {
        return inputParam.Replace("\"", string.Empty);
    }

    [TypeParameter("'.'", false)]
    public char CharParameter(string inputParam)
    {
        return inputParam.Replace("'", string.Empty).ToCharArray()[0];
    }
}
