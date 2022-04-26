using System;
using Colonists;
using Enemies;
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
        
        [ShowIf(nameof(_fraction), Fraction.Colonists)]
        [ValidateInput(nameof(ColonistUnitShouldHaveColonist), "Unit with fraction 'Colonists' should have colonist")]
        [SerializeField] private Colonist _colonist;

        [ShowIf(nameof(_fraction), Fraction.Enemies)]
        [ValidateInput(nameof(EnemyUnitShouldHaveEnemy), "Unit with fraction 'Enemies' should have enemy")]
        [SerializeField] private Enemy _enemy;
        
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
        public event Action Dying;

        public bool Alive => !_died;
        
        public Fraction Fraction => _fraction;

        public UnitEquipment UnitEquipment => _unitEquipment;
        
        private void Awake()
        {
            Vitality = GetComponent<UnitVitality>();
        }
        
        public Colonist Colonist => _colonist;
        public Enemy Enemy => _enemy;

        public UnitVitality Vitality { get; set; }

        private void OnEnable()
        {
            Vitality.HealthChange += OnHealthChange;
            Vitality.Wasted += OnWasted;
        }

        private void OnDisable()
        {
            Vitality.HealthChange -= OnHealthChange;
            Vitality.Wasted -= OnWasted;
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

        public void Die()
        {
            if (_died)
            {
                return;
            }
            
            Vitality.TakeDamage(10000f);
        }

        private void OnWasted()
        {
            _died = true;

            Dying?.Invoke();
        }

        private void OnHealthChange(float health, float blood)
        {
            _healthBars.SetHealth(health);
            _healthBars.SetRecoverySpeed(blood);
            HealthChange?.Invoke();
        }
        
        private bool ColonistUnitShouldHaveColonist()
        {
            if (_fraction == Fraction.Colonists && _colonist == null)
            {
                return false;
            }
            
            return true;
        }
        
        private bool EnemyUnitShouldHaveEnemy()
        {
            if (_fraction == Fraction.Enemies && _enemy == null)
            {
                return false;
            }
            
            return true;
        }
    }
}
