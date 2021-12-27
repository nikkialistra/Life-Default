using Game.UI.Game;
using Kernel.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Game.Units.Unit
{
    [RequireComponent(typeof(UnitSaveLoadHandler))]
    public class UnitFacade : MonoBehaviour
    {
        [Title("Properties")]
        [SerializeField] private UnitType _unitType;
        [SerializeField] private string _name;
        [MinValue(0)]
        [SerializeField] private int _maxHealth;
        
        [Title("Indicators")]
        [Required] 
        [SerializeField] private HealthIndicatorView _healthIndicatorView;
        [Required]
        [SerializeField] private GameObject _selectionIndicator;

        public UnitType UnitType => _unitType;
        public string Name => _name;
        public int Health { get; private set; }

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
            
            _healthIndicatorView.SetHealth(Health);
        }

        public void Select()
        {
            _selectionIndicator.SetActive(true);
            _healthIndicatorView.Show();
        }

        public void Deselect()
        {
            _selectionIndicator.SetActive(false);
            _healthIndicatorView.Hide();
        }

        public class Factory : PlaceholderFactory<UnitFacade>
        {
        }
    }
}