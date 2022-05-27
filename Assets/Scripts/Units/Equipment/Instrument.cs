using System;
using ResourceManagement;
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

        public bool CanExtract(ResourceType resourceType)
        {
            return resourceType switch
            {
                ResourceType.Wood => _instrumentType == InstrumentType.Axe,
                ResourceType.Stone => _instrumentType == InstrumentType.Pickaxe,
                _ => throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null)
            };
        }
    }
}
