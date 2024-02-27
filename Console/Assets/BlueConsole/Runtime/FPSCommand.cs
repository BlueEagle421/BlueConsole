using System;
using TMPro;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class FPSCommand : MonoBehaviour
{
    [SerializeField] private TMP_Text _fpsTMP;

    public static bool IsFPSToggled { get; private set; }
    public static FPSCommand Current;
    public Action<bool> OnFPSToggled;

    private static float[] _frameDeltaTimings = new float[50];
    private int _lastFrameIndex;

    private void Awake()
    {
        Current = this;
    }

    private void Start()
    {
        if (IsFPSToggled)
            FPS(true);
    }

    private void Update()
    {
        UpdateFPSLastFrame();
        FormatTextFPS();
    }

    private void UpdateFPSLastFrame()
    {
        _frameDeltaTimings[_lastFrameIndex] = UnityEngine.Time.unscaledDeltaTime;
        _lastFrameIndex = (_lastFrameIndex + 1) % _frameDeltaTimings.Length;
    }

    private void FormatTextFPS()
    {
        if (!IsFPSToggled)
            return;

        _fpsTMP.text = CurrentFPSFormatted();
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

    private void FPSToggled(bool on)
    {
        _fpsTMP.gameObject.SetActive(on);
        OnFPSToggled.Invoke(on);
    }

    [Command("fps", "toggles fps counter", InstanceTargetType.First)]
    public void FPS(bool on)
    {
        IsFPSToggled = on;
        FPSToggled(on);
    }
}
