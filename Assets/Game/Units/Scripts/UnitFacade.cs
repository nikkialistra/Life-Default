using System;
using Kernel.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Game.Units
{
    [RequireComponent(typeof(UnitHandler))]
    public class UnitFacade : MonoBehaviour, ISelectable
    {
        [Required]
        [SerializeField] private GameObject _selectionIndicator;

        public UnitHandler UnitHandler { get; private set;  }

        public GameObject GameObject => gameObject;

        private void Awake()
        {
            UnitHandler = GetComponent<UnitHandler>();
        }

        public void OnSelect()
        {
            _selectionIndicator.SetActive(true);
        }

        public void OnDeselect() => _selectionIndicator.SetActive(false);
        
        public class Factory : PlaceholderFactory<UnitFacade>
        {
        }
    }
}