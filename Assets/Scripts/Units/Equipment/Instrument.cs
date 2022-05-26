using Units.Enums;
using UnityEngine;

namespace Units.Equipment
{
    
    [CreateAssetMenu(fileName = "Instrument", menuName = "Instrument")]
    public class Instrument : ScriptableObject
    {
        [SerializeField] private InstrumentType _instrumentType;
        [SerializeField] private GameObject _instrument;

        public InstrumentType InstrumentType => _instrumentType;
        public GameObject InstrumentGameObject => _instrument;
    }
}
