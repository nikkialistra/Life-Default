using Game.Units.Services;
using Kernel.Saving.Serialization;
using UnityEngine;
using Zenject;

namespace Kernel.Saving
{
    public class SavingLoadingGame : MonoBehaviour
    {
        private SaveData _saveData;
        private UnitsSaveLoadHandler _unitsSaveLoadHandler;
        
        [Inject]
        public void Construct(SaveData saveData, UnitsSaveLoadHandler unitsSaveLoadHandler)
        {
            _saveData = saveData;
            _unitsSaveLoadHandler = unitsSaveLoadHandler;
        }

        public void Save()
        {
            _saveData.Units = _unitsSaveLoadHandler.GetUnits();

            SerializationManager.Save("save", _saveData);
        }

        public void Load()
        {
            _saveData = (SaveData) SerializationManager.Load("save");

            _unitsSaveLoadHandler.SetUnits(_saveData.Units);

        }
    }
}