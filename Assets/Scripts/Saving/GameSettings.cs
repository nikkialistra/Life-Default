using System.IO;
using NPBehave;
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
        public string Resolution
        {
            get => _gameSettingsData.Resolution;
            set
            {
                _gameSettingsData.Resolution = value;
                Save();
            }
        }

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

        public float CameraSensitivity
        {
            get => _gameSettingsData.CameraSensitivity;
            set
            {
                _gameSettingsData.CameraSensitivity = value;
                Save();
            }
        }


        public bool ScreenEdgeMouseScroll
        {
            get => _gameSettingsData.ScreenEdgeMouseScroll;
            set
            {
                _gameSettingsData.ScreenEdgeMouseScroll = value;
                Save();
            }
        }

        private bool _loaded;

        private string _savePath;

        private GameSettingsData _gameSettingsData;

        private void Awake()
        {
            _savePath = Path.Combine(Application.persistentDataPath, "settings.json");
        }

        public static GameSettings Instance { get; private set; }

        private void Start()
        {
            _gameSettingsData = new GameSettingsData();
            Load();

            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance == this)
            {
                Destroy(gameObject);
            }

            Apply();
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

        private void Apply()
        {
            if (!_loaded)
            {
                Load();
            }

            foreach (var resolution in Screen.resolutions)
            {
                if (resolution.ToString() == Resolution)
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
