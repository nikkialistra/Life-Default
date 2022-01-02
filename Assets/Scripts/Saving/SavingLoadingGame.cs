using System;
using Sirenix.OdinInspector;
using Units.Services;
using UnityEngine;
using Zenject;

namespace Saving
{
    public class SavingLoadingGame : MonoBehaviour
    {
        public event Action Loading;

        private SaveData _saveData;
        private UnitsSaveLoadHandler _unitsSaveLoadHandler;
        private Serialization.Serialization _serialization;

        [Inject]
        public void Construct(SaveData saveData, UnitsSaveLoadHandler unitsSaveLoadHandler,
            Serialization.Serialization serialization)
        {
            _saveData = saveData;
            _unitsSaveLoadHandler = unitsSaveLoadHandler;
            _serialization = serialization;
        }

        [Button(ButtonSizes.Large)]
        [ButtonGroup]
        public void Save()
        {
            _saveData.Units = UnitsSaveLoadHandler.GetUnits();
            _serialization.SaveToFile("save", _saveData);
        }

        [Button(ButtonSizes.Large)]
        [ButtonGroup]
        public void Load()
        {
            Loading?.Invoke();

            _saveData = (SaveData)_serialization.LoadFromFile("save");
            _unitsSaveLoadHandler.SetUnits(_saveData.Units);
        }
    }
}