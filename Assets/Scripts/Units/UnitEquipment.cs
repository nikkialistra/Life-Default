using System;
using System.Collections;
using ResourceManagement;
using Units.Enums;
using Units.Equipment;
using UnityEngine;

namespace Units
{
    [RequireComponent(typeof(UnitWeapons))]
    [RequireComponent(typeof(UnitInventory))]
    public class UnitEquipment : MonoBehaviour
    {
        [SerializeField] private MeshFilter _handSlot;
        [SerializeField] private float _timeToUnequipTool = 0.5f;
        [SerializeField] private float _timeToUnequipWeapon = 1.2f;

        private UnitWeapons _unitWeapons;
        private UnitInventory _unitInventory;

        private Weapon _activeWeapon;
        
        private Coroutine _unequipAfterCoroutine;

        private void Awake()
        {
            _unitWeapons = GetComponent<UnitWeapons>();
            _unitInventory = GetComponent<UnitInventory>();
        }

        private void Start()
        {
            _activeWeapon = _unitWeapons.ChooseWeapon(WeaponSlotType.Melee);
        }

        public bool HoldingSomething { get; private set; }

        public void EquipWeapon()
        {
            UnequipInstantly();

            Instantiate(_activeWeapon.WeaponGameObject, _handSlot.transform);

            HoldingSomething = true;
        }

        public bool TryEquipToolFor(ResourceType resourceType)
        {
            if (!_unitInventory.HasToolFor(resourceType))
            {
                return false;
            }

            UnequipInstantly();

            var tool = _unitInventory.ChooseToolFor(resourceType);

            Instantiate(tool.ToolGameObject, _handSlot.transform);

            HoldingSomething = true;

            return true;
        }

        public void UnequipTool()
        {
            ResetUnequipment();
            _unequipAfterCoroutine = StartCoroutine(UnequipAfter(_timeToUnequipTool));
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
        
        public bool HasWeaponOf(WeaponSlotType weaponSlotType)
        {
            return _unitWeapons.HasWeaponOf(weaponSlotType);
        }
        
        public void ChooseWeapon(WeaponSlotType weaponSlotType)
        {
            _activeWeapon = _unitWeapons.ChooseWeapon(weaponSlotType);
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
