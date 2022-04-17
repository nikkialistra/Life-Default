using System;
using System.Collections;
using ResourceManagement;
using UnityEngine;

namespace Colonists
{
    public class ColonistHandEquipment : MonoBehaviour
    {
        [SerializeField] private MeshFilter _handSlot;
        [Space]
        [SerializeField] private Mesh _axe;
        [SerializeField] private Mesh _pickaxe;
        [Space]
        [SerializeField] private float _timeToUnequip;

        public void Unequip()
        {
            StartCoroutine(UnequipAfter());
        }

        private IEnumerator UnequipAfter()
        {
            yield return new WaitForSeconds(_timeToUnequip);
                
            _handSlot.sharedMesh = null;
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
        }
    }
}
