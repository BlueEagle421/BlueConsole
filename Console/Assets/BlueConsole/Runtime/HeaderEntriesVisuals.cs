using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlueConsole
{
    public class HeaderEntriesVisuals : MonoBehaviour
    {
        [SerializeField] private ConsoleVisuals _consoleVisuals;
        [SerializeField] private TMP_Text _entryDisplayerPrefab;
        [SerializeField] private RectTransform _consoleHeaderTextRect;
        [SerializeField] private HorizontalLayoutGroup _entriesLayoutGroup;
        private List<HeaderEntryHandler> _entryDisplayers = new();

        public static HeaderEntriesVisuals Current;

        private void Awake()
        {
            Current = this;
        }

        private void OnEnable()
        {
            ConsoleProcessor.Current.OnConsoleToggled += OnConsoleToggled;
        }

        private void OnDisable()
        {
            ConsoleProcessor.Current.OnConsoleToggled -= OnConsoleToggled;
        }

        private void Update()
        {
            foreach (HeaderEntryHandler entryDisplayer in _entryDisplayers)
                entryDisplayer.UpdateEntry();
        }

        private void OnConsoleToggled(bool toggled)
        {
            UpdateConsoleHeaderText();
        }

        private void UpdateConsoleHeaderText()
        {
            _consoleHeaderTextRect.gameObject.SetActive(DisplayConsoleHeaderText());
        }

        private bool DisplayConsoleHeaderText()
        {
            return _entryDisplayers.Count == 0;
        }

        private void SortEntryDisplayers()
        {
            _entryDisplayers = _entryDisplayers.OrderByDescending(x => x.InternalHeaderEntry.Priority).ToList();
            for (int i = 0; i < _entryDisplayers.Count; i++)
                _entryDisplayers[i].RectTransform.SetSiblingIndex(i);
        }

        public void AddEntry(HeaderEntry entry)
        {
            if (_entryDisplayers.Any(x => x.InternalHeaderEntry == entry))
                return;


            TMP_Text newTMP = Instantiate(_entryDisplayerPrefab, _entriesLayoutGroup.transform);
            HeaderEntryHandler newDisplayer = new(newTMP, newTMP.rectTransform, entry);
            newDisplayer.SetWidth(entry.Width * _consoleVisuals.Scale);
            _entryDisplayers.Add(newDisplayer);

            SortEntryDisplayers();

            _consoleVisuals.AddScalableRect(newDisplayer.RectTransform, ConsoleVisuals.ScaleType.FontSize);
            UpdateConsoleHeaderText();
        }

        public void RemoveEntry(HeaderEntry entry)
        {
            HeaderEntryHandler toRemove = _entryDisplayers.Find(x => x.InternalHeaderEntry == entry);

            if (toRemove == null)
                return;

            Destroy(toRemove.RectTransform.gameObject);
            _entryDisplayers.Remove(toRemove);

            _consoleVisuals.RemoveScalableRect(toRemove.RectTransform);
            UpdateConsoleHeaderText();
        }

        public void ManageEntry(HeaderEntry entry, bool add)
        {
            if (add)
                AddEntry(entry);
            else
                RemoveEntry(entry);
        }

        private class HeaderEntryHandler
        {
            [SerializeField] private TMP_Text _entryTMP;
            [field: SerializeField] public RectTransform RectTransform { get; private set; }
            public HeaderEntry InternalHeaderEntry { get; private set; }

            public HeaderEntryHandler(TMP_Text entryTMP, RectTransform rectTransform, HeaderEntry internalEntry)
            {
                _entryTMP = entryTMP;
                RectTransform = rectTransform;
                InternalHeaderEntry = internalEntry;

                UpdateEntry();
            }

            public void SetTMPText(string text)
            {
                _entryTMP.text = text;
            }

            public void SetTMPColor(Color color)
            {
                _entryTMP.color = color;
            }

            public void UpdateEntry()
            {
                SetTMPText(InternalHeaderEntry.LabelFunc());
                SetTMPColor(InternalHeaderEntry.ColorFunc());
            }

            public void SetWidth(float width)
            {
                if (width == 0)
                    return;

                RectTransform.sizeDelta = new(width, RectTransform.sizeDelta.y);
            }
        }
    }
}