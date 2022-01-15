using System;
using Sirenix.OdinInspector;
using Units.Services;
using UnityEngine;
using Zenject;

namespace Saving
{
    public class SavingLoadingGame : MonoBehaviour
    {
        private SaveData _saveData;
        private UnitSaveLoadHandler _unitSaveLoadHandler;
        private Serialization.Serialization _serialization;

        [Inject]
        public void Construct(SaveData saveData, UnitSaveLoadHandler unitSaveLoadHandler,
            Serialization.Serialization serialization)
        {
            _saveData = saveData;
            _unitSaveLoadHandler = unitSaveLoadHandler;
            _serialization = serialization;
        }

        public event Action Loading;

        [Button(ButtonSizes.Large)]
        [ButtonGroup]
        public void Save()
        {
            _saveData.Units = UnitSaveLoadHandler.GetUnits();
            _serialization.SaveToFile("save", _saveData);
        }

        [Button(ButtonSizes.Large)]
        [ButtonGroup]
        public void Load()
        {
            Loading?.Invoke();

            _saveData = (SaveData)_serialization.LoadFromFile("save");
            _unitSaveLoadHandler.SetUnits(_saveData.Units);
        }
    }
}
