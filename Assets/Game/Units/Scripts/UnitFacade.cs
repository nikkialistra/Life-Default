using Kernel.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Game.Units
{
    [RequireComponent(typeof(UnitSaveLoadHandler))]
    public class UnitFacade : MonoBehaviour
    {
        [SerializeField] private UnitType _unitType;
        [Required]
        [SerializeField] private GameObject _selectionIndicator;

        public UnitType UnitType => _unitType;

        public UnitSaveLoadHandler UnitSaveLoadHandler { get; private set;  }

        public GameObject GameObject => gameObject;

        private void Awake()
        {
            UnitSaveLoadHandler = GetComponent<UnitSaveLoadHandler>();
        }

        public void OnSelect()
        {
            _selectionIndicator.SetActive(true);
        }

        public void OnDeselect()
        {
            _selectionIndicator.SetActive(false);
        }

        public class Factory : PlaceholderFactory<UnitFacade>
        {
        }
    }
}