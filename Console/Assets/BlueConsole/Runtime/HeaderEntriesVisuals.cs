using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeaderEntriesVisuals : MonoBehaviour
{
    [SerializeField] private TMP_Text _headerEntryPrefab;
    [SerializeField] private RectTransform _consoleHeaderTextRect;

    private List<ConsoleHeaderEntry> _entries = new();
    private List<TMP_Text> _entriesTMPs = new();

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
        //_consoleHeaderTextRect.gameObject.SetActive(!FPSCommand.IsFPSToggled);
    }

    public void AddHeaderEntry(ConsoleHeaderEntry entry)
    {
        _entries.Add(entry);
    }

    public void RemoveHeaderEntry(ConsoleHeaderEntry entry)
    {
        _entries.Remove(entry);
    }
}
