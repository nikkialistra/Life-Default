using System;
using System.Collections;
using ResourceManagement;
using UnityEngine;

namespace Colonists
{
    public class UnitEquipment : MonoBehaviour
    {
        [SerializeField] private MeshFilter _handSlot;
        [Space]
        [SerializeField] private Mesh _sword;
        [Space]
        [SerializeField] private Mesh _axe;
        [SerializeField] private Mesh _pickaxe;
        [Space]
        [SerializeField] private float _timeToUnequip;

        private EquipType _equipType = EquipType.None;

        public void EquipWeapon()
        {
            _handSlot.sharedMesh = _sword;
            _equipType = EquipType.Weapon;
        }
        
        public void EquipInstrumentFor(ResourceType resourceType)
        {
            var instrument = resourceType switch
            {
                ResourceType.Wood => _axe,
                ResourceType.Stone => _pickaxe,
                _ => throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null)
            };
            
            _handSlot.sharedMesh = instrument;
            _equipType = EquipType.Instrument;
        }

        public void UnequipWeapon()
        {
            if (_equipType == EquipType.Weapon)
            {
                StartCoroutine(UnequipAfter());
            }
        }

        public void UnequipInstrument()
        {
            if (_equipType == EquipType.Instrument)
            {
                StartCoroutine(UnequipAfter());
            }
        }

        private IEnumerator UnequipAfter()
        {
            yield return new WaitForSeconds(_timeToUnequip);
                
            _handSlot.sharedMesh = null;
        }

        private enum EquipType
        {
            None,
            Instrument,
            Weapon
        }
    }
}
