using System;
using System.IO;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace Saving
{
    public class GameSettings : MonoBehaviour
    {
        public bool Fullscreen
        {
            get => _gameSettingsData.Fullscreen;
            set
            {
                _gameSettingsData.Fullscreen = value;
                Save();
            }
        }

        public ReactiveProperty<string> Resolution { get; } = new();

        public int UiScale
        {
            get => _gameSettingsData.UiScale;
            set
            {
                _gameSettingsData.UiScale = value;
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

        private void OnResolutionChange(string value)
        {
            _gameSettingsData.Resolution = value;
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

            foreach (var resolution in Screen.resolutions)
            {
                if (resolution.ToString() == Resolution.Value)
                {
                    Screen.SetResolution(resolution.width, resolution.height, Fullscreen);
                }
            }
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
            Resolution.Value = _gameSettingsData.Resolution;
            UiScale = _gameSettingsData.UiScale;

            ShowHelpPanelAtStart = _gameSettingsData.ShowHelpPanelAtStart;
            CameraSensitivity.Value = _gameSettingsData.CameraSensitivity;
            ScreenEdgeMouseScroll.Value = _gameSettingsData.ScreenEdgeMouseScroll;
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
