using System;
using Colonists;
using Sirenix.OdinInspector;
using Units.Ancillaries;
using Units.Ancillaries.Fields;
using Units.Enums;
using UnityEngine;

namespace Units
{
    [RequireComponent(typeof(UnitVitality))]
    public class Unit : MonoBehaviour
    {
        [SerializeField] private Fraction _fraction;
        [Required]
        [SerializeField] private HealthBars _healthBars;
        [Required]
        [SerializeField] private UnitEquipment _unitEquipment;
        [Space]
        [Required]
        [SerializeField] private FieldOfView _unitFieldOfView;
        [SerializeField] private FieldOfHearing _unitFieldOfHearing;

        private bool _died;

        public event Action HealthChange;
        public event Action Die;

        public bool Alive => !_died;
        
        public Fraction Fraction => _fraction;

        public UnitEquipment UnitEquipment => _unitEquipment;
        
        private void Awake()
        {
            Vitality = GetComponent<UnitVitality>();
        }

        public UnitVitality Vitality { get; set; }
        
        private void OnEnable()
        {
            Vitality.HealthChange += OnHealthChange;
            Vitality.Die += OnDie;
        }

        private void OnDisable()
        {
            Vitality.HealthChange -= OnHealthChange;
            Vitality.Die -= OnDie;
        }

        public void Initialize()
        {
            Vitality.Initialize();
            
            _healthBars.SetHealth(Vitality.Health);
            _healthBars.SetRecoverySpeed(Vitality.RecoverySpeed);
        }

        [Button(ButtonSizes.Medium)]
        public void TakeDamage(float value)
        {
            if (_died)
            {
                return;
            }

            Vitality.TakeDamage(value);
        }

        public void Select()
        {
            _healthBars.Selected = true;
        }

        public void Deselect()
        {
            _healthBars.Selected = false;
        }

        public void ToggleUnitVisibilityFields()
        {
            _unitFieldOfView.ToggleDebugShow();
            _unitFieldOfHearing.ToggleDebugShow();
        }

        private void OnDie()
        {
            _died = true;

            Die?.Invoke();
        }

        private void OnHealthChange(float health, float blood)
        {
            _healthBars.SetHealth(health);
            _healthBars.SetRecoverySpeed(blood);
            HealthChange?.Invoke();
        }
    }
}
