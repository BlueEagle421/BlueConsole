using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BlueConsole
{
    public class HeaderEntry
    {
        public Func<string> LabelFunc;
        public Func<Color> ColorFunc;

        private float _width;
        private int _priority;

        public float Width
        {
            get
            {
                return _width;
            }
            set
            {
                //if width is 0 it will be ignored and set to prefab's default
                _width = Mathf.Max(0, value);
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
                //the priority determines entry's position in the header
                _priority = value;
            }
        }

        public HeaderEntry(Func<string> labelFunc, Func<Color> colorFunc, [Optional] int priority, [Optional] int width)
        {
            LabelFunc = labelFunc;
            ColorFunc = colorFunc;
            Priority = priority;
            Width = width;
        }

        /// <summary>
        /// Adds the entry to current console header
        /// </summary>
        public void AddToHeader()
        {
            ConsoleHeader.Current.AddEntry(this);
        }

        /// <summary>
        /// Removes the entry to current console header
        /// </summary>
        public void RemoveFromHeader()
        {
            ConsoleHeader.Current.RemoveEntry(this);
        }

        /// <summary>
        /// Adds or removes the entry from current console header based on the "add" parameter
        /// </summary>
        public void Manage(bool add)
        {
            if (add)
                ConsoleHeader.Current.AddEntry(this);
            else
                ConsoleHeader.Current.RemoveEntry(this);
        }
    }
}