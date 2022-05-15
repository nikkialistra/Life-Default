using System;
using System.IO;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;

namespace Saving
{
    public class GameSettings : MonoBehaviour
    {
        [Required]
        [SerializeField] private PanelSettings _panelSettings;

        public bool Fullscreen
        {
            get => _gameSettingsData.Fullscreen;
            set
            {
                Screen.fullScreen = value;
                _gameSettingsData.Fullscreen = value;
                Save();
            }
        }

        public ReactiveProperty<Resolution> Resolution { get; } = new();

        public int UiScale
        {
            get => _gameSettingsData.UiScale;
            set
            {
                _gameSettingsData.UiScale = value;
                ChangeUiScale();
                Save();
            }
        }

        public bool ShowHelpPanelAtStart
        {
            get => _gameSettingsData.ShowHelpPanelAtStart;
            set
            {
                _gameSettingsData.ShowHelpPanelAtStart = value;
                Save();
            }
        }

        public ReactiveProperty<float> CameraSensitivity { get; } = new();

        public ReactiveProperty<bool> ScreenEdgeMouseScroll { get; } = new();

        private bool _loaded;

        private string _savePath;

        private GameSettingsData _gameSettingsData;

        private void Awake()
        {
            GenerateSavePath();
        }

        private void Start()
        {
            _gameSettingsData = new GameSettingsData();
            Load();

            Apply();

            SubscribeToChanges();
        }

        private void GenerateSavePath()
        {
            _savePath = Path.Combine(Application.persistentDataPath, "settings.json");
        }

        private void SubscribeToChanges()
        {
            Resolution.Subscribe(OnResolutionChange);
            CameraSensitivity.Subscribe(OnCameraSensitivityChange);
            ScreenEdgeMouseScroll.Subscribe(OnScreenEdgeMouseScrollChange);
        }

        private void ChangeUiScale()
        {
            var uiScale = _gameSettingsData.UiScale / 100f;
            var referenceResolution =
                new Vector2Int((int)(1920 / uiScale), (int)(1080 / uiScale));

            _panelSettings.referenceResolution = referenceResolution;
        }

        private void OnResolutionChange(Resolution value)
        {
            if (value.width == 0)
            {
                return;
            }
            
            Screen.SetResolution(value.width, value.height, Fullscreen);
            _gameSettingsData.Resolution = value.ToString();
            Save();
        }

        private void OnCameraSensitivityChange(float value)
        {
            _gameSettingsData.CameraSensitivity = value;
            Save();
        }

        private void OnScreenEdgeMouseScrollChange(bool value)
        {
            _gameSettingsData.ScreenEdgeMouseScroll = value;
            Save();
        }

        private void Save()
        {
            var saveData = JsonUtility.ToJson(_gameSettingsData);

            try
            {
                File.WriteAllText(_savePath, saveData);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to write to {_savePath} with exception {e}");
            }
        }

        [Button(ButtonSizes.Medium)]
        private void DeleteSave()
        {
            GenerateSavePath();

            try
            {
                if (File.Exists(_savePath))
                {
                    File.Delete(_savePath);
                    Debug.Log("File deleted");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to delete file at {_savePath} with exception {e}");
            }
        }

        private void Apply()
        {
            if (!_loaded)
            {
                Load();
            }

            SetUpParameters();
        }

        private void Load()
        {
            if (!File.Exists(_savePath))
            {
                CreateDefaultSettings();

                _loaded = true;
                return;
            }

            try
            {
                var saveData = File.ReadAllText(_savePath);
                JsonUtility.FromJsonOverwrite(saveData, _gameSettingsData);

                _loaded = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to read from {_savePath} with exception {e}");
            }
        }

        private void SetUpParameters()
        {
            Fullscreen = _gameSettingsData.Fullscreen;
            FindResolution();
            UiScale = _gameSettingsData.UiScale;

            ShowHelpPanelAtStart = _gameSettingsData.ShowHelpPanelAtStart;
            CameraSensitivity.Value = _gameSettingsData.CameraSensitivity;
            ScreenEdgeMouseScroll.Value = _gameSettingsData.ScreenEdgeMouseScroll;
        }

        private void FindResolution()
        {
            foreach (var resolution in Screen.resolutions)
            {
                if (resolution.ToString() == _gameSettingsData.Resolution)
                {
                    Resolution.Value = resolution;
                }
            }
        }

        private void CreateDefaultSettings()
        {
            _gameSettingsData.Resolution = Screen.resolutions[^1].ToString();
            _gameSettingsData.Fullscreen = true;
            _gameSettingsData.UiScale = 100;

            _gameSettingsData.ShowHelpPanelAtStart = true;
            _gameSettingsData.CameraSensitivity = 1;
            _gameSettingsData.ScreenEdgeMouseScroll = true;
        }

        private class GameSettingsData
        {
            public bool Fullscreen;
            public string Resolution;
            public int UiScale;

            public bool ShowHelpPanelAtStart;
            public float CameraSensitivity;
            public bool ScreenEdgeMouseScroll;
        }
    }
}
