using TMPro;
using UnityEngine;

namespace BlueConsole
{
    public class HeaderEntryDisplayer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _entryTMP;
        [field: SerializeField] public RectTransform RectTransform { get; private set; }
        public HeaderEntry InternalHeaderEntry { get; private set; }

        public void SetInternalHeaderEntry(HeaderEntry entry)
        {
            InternalHeaderEntry = entry;
        }

        public void SetTMPText(string text)
        {
            _entryTMP.text = text;
        }

        public void SetTMPColor(Color color)
        {
            _entryTMP.color = color;
        }

        public void SetWidth(float width)
        {
            if (width == 0)
                return;

            RectTransform.sizeDelta = new(width, RectTransform.sizeDelta.y);
        }
    }
}