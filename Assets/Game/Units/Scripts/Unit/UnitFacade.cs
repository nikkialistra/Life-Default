using Kernel.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Game.Units.Unit
{
    [RequireComponent(typeof(UnitSaveLoadHandler))]
    public class UnitFacade : MonoBehaviour
    {
        [SerializeField] private UnitType _unitType;
        [SerializeField] private string _name;
        [MinValue(0)]
        [SerializeField] private float _maxHealth;
        [Required]
        [SerializeField] private GameObject _selectionIndicator;

        public UnitType UnitType => _unitType;
        public string Name => _name;
        public float Health { get; private set; }

        public UnitSaveLoadHandler UnitSaveLoadHandler { get; private set;  }

        private void Awake()
        {
            UnitSaveLoadHandler = GetComponent<UnitSaveLoadHandler>();
        }

        private void Start()
        {
            if (_name == "")
            {
                _name = NameGenerator.GetRandomName();
            }
            Health = _maxHealth;
        }

        public void Select()
        {
            _selectionIndicator.SetActive(true);
        }

        public void Deselect()
        {
            _selectionIndicator.SetActive(false);
        }

        public class Factory : PlaceholderFactory<UnitFacade>
        {
        }
    }
}