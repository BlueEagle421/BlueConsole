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

        public void AddToHeader()
        {
            HeaderEntriesVisuals.Current.AddEntry(this);
        }

        public void RemoveFromHeader()
        {
            HeaderEntriesVisuals.Current.RemoveEntry(this);
        }

        public void Manage(bool add)
        {
            if (add)
                HeaderEntriesVisuals.Current.AddEntry(this);
            else
                HeaderEntriesVisuals.Current.RemoveEntry(this);
        }
    }
}