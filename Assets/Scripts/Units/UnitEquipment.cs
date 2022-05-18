using System;
using System.Collections;
using ResourceManagement;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Units
{
    public class UnitEquipment : MonoBehaviour
    {
        [SerializeField] private MeshFilter _handSlot;
        [SerializeField] private float _timeToUnequip;
        
        [Title("Weapons")]
        [SerializeField] private GameObject _knife;
        
        [Title("Instruments")]
        [SerializeField] private GameObject _axe;
        [SerializeField] private GameObject _pickaxe;

        public void EquipWeapon()
        {
            UnequipInstantly();

            Instantiate(_knife, _handSlot.transform);
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
        }

        public void Unequip()
        {
            StartCoroutine(UnequipAfter());
        }

        private IEnumerator UnequipAfter()
        {
            yield return new WaitForSeconds(_timeToUnequip);

            UnequipInstantly();
        }

        private void UnequipInstantly()
        {
            if (_handSlot.transform.childCount > 0)
            {
                Destroy(_handSlot.transform.GetChild(0).gameObject);
            }
        }
    }
}
