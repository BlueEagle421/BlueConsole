using System.Globalization;
using System.Numerics;

namespace BlueConsole
{
    public static class ConsoleTypeParameters
    {
        [TypeParameter(@"(0|1|on|off|true|false)")]
        public static bool BoolParameter(string inputParam)
        {
            return inputParam switch
            {
                "1" => true,
                "0" => false,
                "on" => true,
                "off" => false,
                "true" => true,
                "false" => false,
                _ => throw new System.Exception()
            };
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

        [TypeParameter(@"\S*?", false)]
        public static Command CommandParameter(string inputParam)
        {
            Command result = ConsoleProcessor.Commands.Find(x => x.ID == inputParam) ?? throw new System.Exception();
            return result;
        }

        [TypeParameter("'.'")]
        public static char CharParameter(string inputParam)
        {
            return inputParam.Replace("'", string.Empty).ToCharArray()[0];
        }

        [TypeParameter(@"\([-+]?([0-9]*[.])?[0-9]+([eE][-+]?\d+)?, [-+]?([0-9]*[.])?[0-9]+([eE][-+]?\d+)?\)", true)]
        public static UnityEngine.Vector2 Vector2Parameter(string inputParam)
        {
            string[] inputSplit = inputParam.Replace(")", string.Empty).Replace("(", string.Empty).Split(", ");
            UnityEngine.Vector2 result = new();

            for (int i = 0; i < 2; i++)
                result[i] = FloatParameter(inputSplit[i]);

            return result;
        }

        [TypeParameter(@"\([-+]?([0-9]*[.])?[0-9]+([eE][-+]?\d+)?, [-+]?([0-9]*[.])?[0-9]+([eE][-+]?\d+)?, [-+]?([0-9]*[.])?[0-9]+([eE][-+]?\d+)?\)", true)]
        public static UnityEngine.Vector3 Vector3Parameter(string inputParam)
        {
            string[] inputSplit = inputParam.Replace(")", string.Empty).Replace("(", string.Empty).Split(", ");
            UnityEngine.Vector3 result = new();

            for (int i = 0; i < 3; i++)
                result[i] = FloatParameter(inputSplit[i]);

            return result;
        }

        [TypeParameter(@"\([-+]?([0-9]*[.])?[0-9]+([eE][-+]?\d+)?, [-+]?([0-9]*[.])?[0-9]+([eE][-+]?\d+)?, [-+]?([0-9]*[.])?[0-9]+([eE][-+]?\d+)?, [-+]?([0-9]*[.])?[0-9]+([eE][-+]?\d+)?\)", true)]
        public static UnityEngine.Vector4 Vector4Parameter(string inputParam)
        {
            string[] inputSplit = inputParam.Replace(")", string.Empty).Replace("(", string.Empty).Split(", ");
            UnityEngine.Vector4 result = new();

            for (int i = 0; i < 4; i++)
                result[i] = FloatParameter(inputSplit[i]);

            return result;
        }

        [TypeParameter(@"\([-+]?([0-9]*[.])?[0-9]+([eE][-+]?\d+)?, [-+]?([0-9]*[.])?[0-9]+([eE][-+]?\d+)?, [-+]?([0-9]*[.])?[0-9]+([eE][-+]?\d+)?, [-+]?([0-9]*[.])?[0-9]+([eE][-+]?\d+)?\)", true)]
        public static UnityEngine.Quaternion QuaternionParameter(string inputParam)
        {
            UnityEngine.Vector4 vector4 = Vector4Parameter(inputParam);
            UnityEngine.Quaternion result = new();

            for (int i = 0; i < 4; i++)
                result[i] = vector4[i];

            return result;
        }
    }
}