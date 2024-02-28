using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HeaderEntriesVisuals : MonoBehaviour
{
    [SerializeField] private HeaderEntryDisplayer _entryDisplayerPrefab;
    [SerializeField] private RectTransform _consoleHeaderTextRect;
    [SerializeField] private HorizontalLayoutGroup _entriesLayoutGroup;
    private List<HeaderEntryDisplayer> _entriesDisplayers = new();

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
        foreach (HeaderEntryDisplayer entryDisplayer in _entriesDisplayers)
        {
            HeaderEntry internalEntry = entryDisplayer.InternalHeaderEntry;

            entryDisplayer.SetTMPText(internalEntry.LabelFunc());
            entryDisplayer.SetTMPColor(internalEntry.ColorFunc());
        }
    }

    private void OnConsoleToggled(bool toggled)
    {
        SetConsoleHeaderTextRect();
    }

    private void SetConsoleHeaderTextRect()
    {
        _consoleHeaderTextRect.gameObject.SetActive(DisplayConsoleHeaderText());
    }

    private bool DisplayConsoleHeaderText()
    {
        return _entriesDisplayers.Count == 0;
    }

    public void AddEntry(HeaderEntry entry)
    {
        if (_entriesDisplayers.Any(x => x.InternalHeaderEntry == entry))
            return;

        HeaderEntryDisplayer newDisplayer = Instantiate(_entryDisplayerPrefab, _entriesLayoutGroup.transform);
        newDisplayer.SetInternalHeaderEntry(entry);
        newDisplayer.SetWidth(entry.Width);
        _entriesDisplayers.Add(newDisplayer);
    }

    public void RemoveEntry(HeaderEntry entry)
    {
        HeaderEntryDisplayer toRemove = _entriesDisplayers.Find(x => x.InternalHeaderEntry == entry);

        if (!toRemove)
            return;

        Destroy(toRemove.gameObject);
        _entriesDisplayers.Remove(toRemove);
    }

    public void ManageEntry(HeaderEntry entry, bool add)
    {
        if (add)
            AddEntry(entry);
        else
            RemoveEntry(entry);
    }
}
