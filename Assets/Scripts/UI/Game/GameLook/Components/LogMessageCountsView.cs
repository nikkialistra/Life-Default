using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    public class LogMessageCountsView : MonoBehaviour
    {
        private const string VisualTreePath = "UI/Markup/GameLook/Components/LogMessageCounts";

        private Label _infoCount;
        private Label _warningsCount;
        private Label _errorsCount;

        private int _info = 0;
        private int _warnings = 0;
        private int _errors = 0;
        
        private void Awake()
        {
            Tree = Resources.Load<VisualTreeAsset>(VisualTreePath).CloneTree();

            _infoCount = Tree.Q<Label>("info__count");
            _warningsCount = Tree.Q<Label>("warnings__count");
            _errorsCount = Tree.Q<Label>("errors__count");
        }

        public VisualElement Tree { get; private set; }
        
        private void OnEnable()
        {
            Application.logMessageReceivedThreaded += OnLogMessageReceive;
        }

        private void OnDisable()
        {
            Application.logMessageReceivedThreaded -= OnLogMessageReceive;
        }

        private void OnLogMessageReceive(string condition, string stacktrace, LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                    UpdateErrors();
                    break;
                case LogType.Assert:
                    UpdateErrors();
                    break;
                case LogType.Warning:
                    UpdateWarnings();
                    break;
                case LogType.Log:
                    UpdateInfo();
                    break;
                case LogType.Exception:
                    UpdateErrors();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void UpdateErrors()
        {
            _errors++;

            _errorsCount.text = $"{_errors}";
        }

        private void UpdateWarnings()
        {
            _warnings++;
            
            _warningsCount.text = $"{_warnings}";
        }

        private void UpdateInfo()
        {
            _info++;
            
            _infoCount.text = $"{_info}";
        }
    }
}
