using UnityEngine;

public class Diagnostics : MonoBehaviour
{
    [SerializeField] private RectTransform _diagnosticsGUIParent;
    public static bool IsDiagnosing { get; private set; }

    private void Start()
    {
        if (IsDiagnosing)
            ToggleDiagnostics(true);
    }

    private void ToggleDiagnostics(bool toggle)
    {
        if (!IsDiagnosing && toggle)
            ToggledOn();

        IsDiagnosing = toggle;
    }

    private void ToggledOn()
    {
        Debug.Log("CPU type: " + SystemInfo.processorType);
        Debug.Log("GPU type: " + SystemInfo.graphicsDeviceName);
    }

    [Command("diagnose", "toggles fps, hwinfo and usage", InstanceTargetType.First)]
    public void Diagnose(bool on)
    {
        ToggleDiagnostics(on);
    }
}
