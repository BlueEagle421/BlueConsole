using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BlueConsole
{
    public class ConsoleVisuals : MonoBehaviour
    {
        [Tooltip("Height of the console")]
        [SerializeField] private float _height;
        [Tooltip("Scale multiplier applied to the console")]
        [SerializeField] private float _scale;
        [Tooltip("The rect transform containing the entire console")]
        [SerializeField] private RectTransform _consoleGUIParent;
        [Tooltip("The rect transform containing the entire console")]
        [SerializeField] private RectTransform _GUIParent;
        [Tooltip("The read only input field displaying console content")]
        [SerializeField] private TMP_InputField _consoleContentField;
        [Tooltip("The scroll rect that allows console content field to be moved vertically")]
        [SerializeField] private ScrollRect _consoleContentScrollRect;
        [Tooltip("The rect transform containing console content field")]
        [SerializeField] private RectTransform _consoleContentRect;
        [Tooltip("The read only input field displaying console hints")]
        [SerializeField] private TMP_InputField _hintInputField;
        private readonly List<TMP_InputField> _hintsInputFields = new();
        [Tooltip("The list containing all rect transform meant to be scaled with the console")]
        [SerializeField] private List<ScalableRect> _reckTransformsToScale;


        private void Awake()
        {
            SetEvents();
            CheckEventSystem();
        }

        private void Start()
        {
            CloneHintField();
            SetGUIHeight(_height);
            SetAllGUIScale(_scale);
        }

        private void CheckEventSystem()
        {
            if (EventSystem.current == null)
            {
                ConsoleProcessor.Current.Toggle(true);
                Debug.LogError("Missing EventSystem. Console will not work properly.");
                enabled = false;
            }
        }

        private void SetEvents()
        {
            ConsoleProcessor.Current.OnConsoleToggled += OnConsoleToggled;
            ConsoleProcessor.Current.OnContentChanged += OnConsoleContentChanged;
            ConsoleProcessor.Current.OnHintsChanged += OnHintsChanged;
        }

        private void CloneHintField()
        {
            for (int i = 0; i < ConsoleProcessor.Current.MaxHintsAmount; i++)
            {
                TMP_InputField toAdd = Instantiate(_hintInputField);
                toAdd.transform.SetParent(_hintInputField.transform.parent, false);
                _hintsInputFields.Add(toAdd);
                _reckTransformsToScale.Add(new ScalableRect(toAdd.GetComponent<RectTransform>(), ScaleType.Height));
                _reckTransformsToScale.Add(new ScalableRect(toAdd.textComponent.GetComponent<RectTransform>(), ScaleType.FontSize));
            }
            Destroy(_hintInputField.gameObject);
        }

        private void OnConsoleToggled(bool toggled)
        {
            _consoleGUIParent.gameObject.SetActive(toggled);

            if (toggled)
            {
                _consoleContentField.text = ConsoleProcessor.Content;

                ResizeContentRect();
                ScrollDown();
            }
        }

        private void OnConsoleContentChanged()
        {
            _consoleContentField.text = ConsoleProcessor.Content;

            ResizeContentRect();
            ScrollDown();
        }

        private void OnHintsChanged()
        {
            int hintsCount = ConsoleProcessor.Hints.Count;
            for (int i = 0; i < _hintsInputFields.Count; i++)
            {
                if (i < hintsCount)
                {
                    _hintsInputFields[i].text = ConsoleProcessor.Hints[i];
                    _hintsInputFields[i].textComponent.ForceMeshUpdate();
                }

                _hintsInputFields[i].gameObject.SetActive(i < hintsCount);
            }
        }

        private void SetGUIHeight(float height)
        {
            _GUIParent.sizeDelta = new Vector2(_GUIParent.sizeDelta.x, height);
        }

        private void SetAllGUIScale(float scale)
        {
            for (int i = 0; i < _reckTransformsToScale.Count; i++)
                SetGUIScale(_reckTransformsToScale[i], scale);
        }

        private void SetGUIScale(ScalableRect scalableRect, float scale)
        {
            RectTransform rectToScale = scalableRect.RectTransform;

            if (!rectToScale)
            {
                Debug.LogWarning(scalableRect.DisplayName + " is null in " + nameof(ConsoleVisuals));
                return;
            }

            switch (scalableRect.ScaleType)
            {
                case ScaleType.Height:
                    {
                        rectToScale.sizeDelta = new(rectToScale.sizeDelta.x, rectToScale.sizeDelta.y * scale);
                        break;
                    }
                case ScaleType.Width:
                    {
                        rectToScale.sizeDelta = new(rectToScale.sizeDelta.x * scale, rectToScale.sizeDelta.y);
                        break;
                    }
                case ScaleType.Both:
                    {
                        rectToScale.sizeDelta *= scale;
                        break;
                    }
                case ScaleType.FontSize:
                    {
                        TMP_Text componentTMP = rectToScale.GetComponent<TMP_Text>();
                        componentTMP.fontSize *= scale;
                        break;
                    }
            }
        }

        public void AddScalableRect(RectTransform rectTransform, ScaleType scaleType)
        {
            ScalableRect scalableRect = new(rectTransform, scaleType);
            _reckTransformsToScale.Add(scalableRect);
            SetGUIScale(scalableRect, _scale);
        }

        public void RemoveScalableRect(RectTransform rectTransform)
        {
            ScalableRect scalableRect = _reckTransformsToScale.Find(x => x.RectTransform == rectTransform);

            if (scalableRect == null)
                return;

            _reckTransformsToScale.Remove(scalableRect);
        }

        private void ResizeContentRect()
        {
            TMP_Text textComponent = _consoleContentField.textComponent;

            textComponent.ForceMeshUpdate();

            if (textComponent.textInfo == null)
                return;

            float height = textComponent.margin.y * 2;

            for (int i = 0; i < textComponent.textInfo.lineCount; i++)
                height += textComponent.textInfo.lineInfo[i].lineHeight;

            _consoleContentRect.sizeDelta = new Vector2(0, height);
        }

        private void ScrollDown()
        {
            if (_consoleContentScrollRect)
                _consoleContentScrollRect.verticalNormalizedPosition = 0f;
        }

        [Serializable]
        public class ScalableRect
        {
            public string DisplayName;
            [SerializeField] private RectTransform rectTransform;
            [SerializeField] private ScaleType scaleType;

            public ScalableRect(RectTransform rectTransform, ScaleType scaleType)
            {
                this.rectTransform = rectTransform;
                this.scaleType = scaleType;
            }

            public RectTransform RectTransform { get { return rectTransform; } }
            public ScaleType ScaleType { get { return scaleType; } }
        }

        public enum InputType
        {
            InputManager,
            InputSystem
        }

        public enum ScaleType
        {
            Height,
            Width,
            Both,
            FontSize,
        }
    }
}
