using TMPro;
using UnityEngine;

public class HeaderEntryDisplayer : MonoBehaviour
{
    [SerializeField] private TMP_Text _entryTMP;
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
}