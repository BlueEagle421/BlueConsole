using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BlueConsole
{
    public class HeaderEntry
    {
        public Func<string> LabelFunc;
        public Func<Color> ColorFunc;

        private int _width;
        private int _priority;

        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                //if width is 0 it will be ignored and set to prefab's default
                _priority = Mathf.Max(0, value);
            }
        }

        public int Priority
        {
            get
            {
                return _priority;
            }
            set
            {
                //if priority is 0 it will be ignored and added as the last one
                _priority = Mathf.Max(0, value);
            }
        }

        public HeaderEntry(Func<string> labelFunc, Func<Color> colorFunc, [Optional] int priority, [Optional] int width)
        {
            LabelFunc = labelFunc;
            ColorFunc = colorFunc;
            Priority = priority;
            Width = width;
        }
    }
}