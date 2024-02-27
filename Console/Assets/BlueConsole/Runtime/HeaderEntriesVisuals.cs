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

    private void OnEnable()
    {
        ConsoleProcessor.Current.OnConsoleToggled += OnConsoleToggled;
    }

    private void OnDisable()
    {
        ConsoleProcessor.Current.OnConsoleToggled -= OnConsoleToggled;
    }

    private void OnConsoleToggled(bool toggled)
    {
        SetConsoleHeaderTextRect();
    }

    private void SetConsoleHeaderTextRect()
    {
        _consoleHeaderTextRect.gameObject.SetActive(!DisplayConsoleHeaderText());
    }

    private bool DisplayConsoleHeaderText()
    {
        return _entriesDisplayers.Count == 0;
    }

    public void AddHeaderEntry(HeaderEntry entry)
    {
        if (_entriesDisplayers.Any(x => x.InternalHeaderEntry == entry))
            return;

        HeaderEntryDisplayer newDisplayer = Instantiate(_entryDisplayerPrefab, _entriesLayoutGroup.transform);
        _entriesDisplayers.Add(newDisplayer);
    }

    public void RemoveHeaderEntry(HeaderEntry entry)
    {
        HeaderEntryDisplayer toRemove = _entriesDisplayers.Find(x => x.InternalHeaderEntry == entry);

        if (!toRemove)
            return;

        Destroy(toRemove);
        _entriesDisplayers.Remove(toRemove);
    }
}
