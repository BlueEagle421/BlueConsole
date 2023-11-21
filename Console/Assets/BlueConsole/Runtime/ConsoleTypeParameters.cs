using System.Globalization;
using UnityEngine;

public class ConsoleTypeParameters : MonoBehaviour
{
    [TypeParameter(@"[0|1]")]
    public bool BoolParameter(string inputParam)
    {
        return inputParam == "1";
    }

    [TypeParameter(@"\d+")]
    public byte ByteParameter(string inputParam)
    {
        return byte.Parse(inputParam);
    }

    [TypeParameter(@"[-+]?\d+")]
    public sbyte SByteParameter(string inputParam)
    {
        return sbyte.Parse(inputParam);
    }

    [TypeParameter(@"[-+]?\d+")]
    public int IntParameter(string inputParam)
    {
        return int.Parse(inputParam);
    }

    [TypeParameter(@"[-+]?\d+")]
    public uint UIntParameter(string inputParam)
    {
        return uint.Parse(inputParam);
    }

    [TypeParameter(@"[-+]?([0-9]*[.])?[0-9]+([eE][-+]?\d+)?")]
    public decimal DecimalParameter(string inputParam)
    {
        return decimal.Parse(inputParam, NumberStyles.Any, CultureInfo.InvariantCulture);
    }

    [TypeParameter(@"[-+]?([0-9]*[.])?[0-9]+([eE][-+]?\d+)?")]
    public double DoubleParameter(string inputParam)
    {
        return double.Parse(inputParam, NumberStyles.Any, CultureInfo.InvariantCulture);
    }

    [TypeParameter(@"[-+]?([0-9]*[.])?[0-9]+([eE][-+]?\d+)?")]
    public float FloatParameter(string inputParam)
    {
        return float.Parse(inputParam, NumberStyles.Any, CultureInfo.InvariantCulture);
    }

    [TypeParameter("\".*?\"", true)]
    public string StringParameter(string inputParam)
    {
        return inputParam.Replace("\"", string.Empty);
    }

    [TypeParameter("'.'")]
    public char CharParameter(string inputParam)
    {
        return inputParam.Replace("'", string.Empty).ToCharArray()[0];
    }
}
