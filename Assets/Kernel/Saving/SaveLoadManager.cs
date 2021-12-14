using Game.Units.Services;
using Kernel.Saving.Serialization;
using UnityEngine;
using Zenject;

namespace Kernel.Saving
{
    public class SaveLoadManager : MonoBehaviour
    {
        private UnitsHandler _unitsHandler;
        
        
        [Inject]
        public void Construct(UnitsHandler unitsHandler)
        {
            _unitsHandler = unitsHandler;
        }

        public void SaveGame()
        {
            var saveData = SaveData.Current;
       
            saveData.Units = _unitsHandler.GetUnits();

            SerializationManager.Save("save", saveData);
        }

        public void LoadGame()
        {
            SaveData.Current = (SaveData) SerializationManager.Load("save");

            _unitsHandler.SetUnits(SaveData.Current.Units);

        }
    }
}