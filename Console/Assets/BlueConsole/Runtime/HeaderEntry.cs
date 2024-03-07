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
                //higher priority means it's more to the left
                _priority = value;
            }
        }

        //example constructor
        //HeaderEntry exampleEntry = new HeaderEntry(() => "An example header text", () => Color.white, 0, 100)
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
        public virtual void AddToHeader()
        {
            ConsoleHeader.Current.AddEntry(this);
        }

        /// <summary>
        /// Removes the entry from current console header
        /// </summary>
        public virtual void RemoveFromHeader()
        {
            ConsoleHeader.Current.RemoveEntry(this);
        }

        /// <summary>
        /// Adds or removes the entry from current console header based on the "add" parameter
        /// </summary>
        public virtual void Manage(bool add)
        {
            if (add)
                AddToHeader();
            else
                RemoveFromHeader();
        }
    }
}