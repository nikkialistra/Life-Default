using System;
using System.Collections;
using ResourceManagement;
using UnityEngine;

namespace Units
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

        private bool _weaponEquiped;
        
        private Coroutine _unequipAfterCoroutine;

        public void EquipWeapon()
        {
            _weaponEquiped = true;
            
            CancelUnequip();
            
            _handSlot.sharedMesh = _sword;
            _equipType = EquipType.Weapon;
        }
        
        public void EquipInstrumentFor(ResourceType resourceType)
        {
            CancelUnequip();
            
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
            if (!_weaponEquiped)
            {
                return;
            }
            
            if (_equipType == EquipType.Weapon)
            {
                _weaponEquiped = false;
                _unequipAfterCoroutine = StartCoroutine(UnequipAfter());
            }
        }

        public void UnequipInstrument()
        {
            if (_equipType == EquipType.Instrument)
            {
                StartCoroutine(UnequipAfter());
            }
        }

        private void CancelUnequip()
        {
            if (_unequipAfterCoroutine != null)
            {
                StopCoroutine(_unequipAfterCoroutine);
                _unequipAfterCoroutine = null;
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
