using UnityEngine;

public class Diagnostics : MonoBehaviour
{
    [SerializeField] private RectTransform _diagnosticsGUIParent;
    public static bool IsDiagnosing { get; private set; }

    [Command("diagnose", "toggles fps, hwinfo and usage")]
    public static void Diagnose(bool on)
    {

    }
}
