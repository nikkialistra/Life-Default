using System;
using System.Collections;
using ResourceManagement;
using Sirenix.OdinInspector;
using Units.Enums;
using Units.Equipment;
using UnityEngine;

namespace Units
{
    public class UnitEquipment : MonoBehaviour
    {
        [SerializeField] private MeshFilter _handSlot;
        [SerializeField] private float _timeToUnequipInstrument = 0.5f;
        [SerializeField] private float _timeToUnequipWeapon = 1.2f;

        [SerializeField] private WeaponSlot _meleeWeapon;
        [SerializeField] private WeaponSlot _rangedWeapon;

        [Title("Weapons")]
        [SerializeField] private GameObject _knife;
        
        [Title("Instruments")]
        [SerializeField] private GameObject _axe;
        [SerializeField] private GameObject _pickaxe;
        
        private Coroutine _unequipAfterCoroutine;

        public bool HoldingSomething { get; private set; }

        public void EquipWeapon()
        {
            UnequipInstantly();

            Instantiate(_knife, _handSlot.transform);

            HoldingSomething = true;
        }

        public void EquipInstrumentFor(ResourceType resourceType)
        {
            UnequipInstantly();
            
            var instrument = resourceType switch
            {
                ResourceType.Wood => _axe,
                ResourceType.Stone => _pickaxe,
                _ => throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null)
            };
            
            Instantiate(instrument, _handSlot.transform);

            HoldingSomething = true;
        }

        public void UnequipInstrument()
        {
            ResetUnequipment();
            _unequipAfterCoroutine = StartCoroutine(UnequipAfter(_timeToUnequipInstrument));
        }

        public void UnequipWeapon()
        {
            ResetUnequipment();
            _unequipAfterCoroutine = StartCoroutine(UnequipAfter(_timeToUnequipWeapon));
        }

        private void UnequipInstantly()
        {
            ResetUnequipment();
            Unequip();
        }
        
        public bool HasWeaponOf(WeaponType weaponType)
        {
            return weaponType == WeaponType.Melee ? _meleeWeapon.NotEmpty : _rangedWeapon.NotEmpty;
        }
        
        public void ChooseWeapon(WeaponType weaponType)
        {
            
        }

        private void ResetUnequipment()
        {
            if (_unequipAfterCoroutine != null)
            {
                StopCoroutine(_unequipAfterCoroutine);
                _unequipAfterCoroutine = null;
            }
        }

        private IEnumerator UnequipAfter(float timeToUnequip)
        {
            yield return new WaitForSeconds(timeToUnequip);

            Unequip();
            
            _unequipAfterCoroutine = null;
        }

        private void Unequip()
        {
            if (_handSlot.transform.childCount > 0)
            {
                Destroy(_handSlot.transform.GetChild(0).gameObject);
            }

            HoldingSomething = false;
        }
    }
}
