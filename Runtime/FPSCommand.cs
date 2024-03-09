using UnityEngine;

namespace BlueConsole
{
    public class FPSCommand : MonoBehaviour
    {
        [SerializeField] private Color _fpsTextColor;
        public static bool IsFPSToggled { get; private set; }
        private static readonly float[] _frameDeltaTimings = new float[50];
        private int _lastFrameIndex;
        private HeaderEntry _fpsHeaderEntry;

        private void Awake()
        {
            _fpsHeaderEntry = new(() => CurrentFPSFormatted(), () => _fpsTextColor, 10, 50);
        }

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
            _fpsHeaderEntry.Manage(on);
            //HeaderEntriesVisuals.Current.ManageEntry(_fpsHeaderEntry, on);
        }
    }
}