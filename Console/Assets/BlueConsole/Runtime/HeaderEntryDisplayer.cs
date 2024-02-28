using TMPro;
using UnityEngine;

public class HeaderEntryDisplayer : MonoBehaviour
{
    [SerializeField] private TMP_Text _entryTMP;
    [SerializeField] private RectTransform _rectTransform;
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

    public void SetWidth(int width)
    {
        if (width == 0)
            return;

        _rectTransform.sizeDelta = new(width, _rectTransform.sizeDelta.y);
    }

    public void SetHeight(int height)
    {
        if (height == 0)
            return;

        _rectTransform.sizeDelta = new(_rectTransform.sizeDelta.x, height);
    }
}