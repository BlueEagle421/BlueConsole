using UnityEngine;

[DefaultExecutionOrder(-1)]
public class FPSCommand : MonoBehaviour
{
    public static bool IsFPSToggled { get; private set; }
    private static float[] _frameDeltaTimings = new float[50];
    private int _lastFrameIndex;
    private HeaderEntry _fpsHeaderEntry = new(() => CurrentFPSFormatted(), () => Color.green, 1);

    private void Start()
    {
        if (IsFPSToggled)
            FPS(true);
    }

    private void Update()
    {
        UpdateFPSLastFrame();
    }

    private void UpdateFPSLastFrame()
    {
        _frameDeltaTimings[_lastFrameIndex] = UnityEngine.Time.unscaledDeltaTime;
        _lastFrameIndex = (_lastFrameIndex + 1) % _frameDeltaTimings.Length;
    }

    public static float CurrentFPS()
    {
        float total = 0f;
        foreach (float deltaTime in _frameDeltaTimings)
        {
            total += deltaTime;
        }
        return _frameDeltaTimings.Length / total;
    }

    public static string CurrentFPSFormatted()
    {
        return Mathf.RoundToInt(CurrentFPS()).ToString();
    }

    [Command("fps", "toggles fps counter", InstanceTargetType.First)]
    public void FPS(bool on)
    {
        IsFPSToggled = on;

        if (on)
            HeaderEntriesVisuals.Current.AddHeaderEntry(_fpsHeaderEntry);
        else
            HeaderEntriesVisuals.Current.RemoveHeaderEntry(_fpsHeaderEntry);
    }
}