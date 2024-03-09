using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TMP_InputFieldEx : TMP_InputField
{
    [SerializeField] private ScrollRect m_parentScrollRect;

    protected override void OnEnable()
    {
        m_parentScrollRect ??= GetComponentInParent<ScrollRect>();
        base.OnEnable();
    }

    public override void OnScroll(PointerEventData p_eventData)
    {
        if (m_parentScrollRect)
        {
            m_parentScrollRect.OnScroll(p_eventData);
        }
        else
        {
            base.OnScroll(p_eventData);
        }
    }
}
