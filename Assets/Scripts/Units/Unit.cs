using System;
using Colonists;
using Enemies;
using Sirenix.OdinInspector;
using Units.Ancillaries;
using Units.Ancillaries.Fields;
using Units.Calculations;
using Units.Enums;
using Units.FightBehavior;
using Units.Stats;
using Units.Traits;
using UnityEngine;

namespace Units
{
    [RequireComponent(typeof(UnitStats))]
    [RequireComponent(typeof(UnitTraits))]
    [RequireComponent(typeof(UnitVitality))]
    [RequireComponent(typeof(UnitFightCalculation))]
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
        [Required]
        [SerializeField] private FieldOfHearing _unitFieldOfHearing;
        
        [Title("Hints")]
        [Required]
        [SerializeField] private LandIndicator _targetIndicator;
        [Required]
        [SerializeField] private MessageShowing _messageShowing;

        private UnitStats _unitStats;
        private UnitTraits _unitTraits;
        
        private bool _died;
        
        private UnitFightCalculation _unitFightCalculation;

        public event Action<Unit> AttackFrom;
        public event Action<Unit> LeavingAttackFrom;
        
        public event Action HealthChange;
        
        public event Action Dying;

        public bool Alive => !_died;
        
        public Fraction Fraction => _fraction;

        public UnitEquipment UnitEquipment => _unitEquipment;
        
        private void Awake()
        {
            _unitStats = GetComponent<UnitStats>();
            _unitTraits = GetComponent<UnitTraits>();
            
            Vitality = GetComponent<UnitVitality>();
            _unitFightCalculation = GetComponent<UnitFightCalculation>();
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

        public void AddTrait(Trait trait)
        {
            _unitTraits.AddTrait(trait);
        }

        public void RemoveTrait(Trait trait)
        {
            _unitTraits.RemoveTrait(trait);
        }

        [Button(ButtonSizes.Medium)]
        public void TakeDamage(float damage)
        {
            if (_died)
            {
                return;
            }

            if (_unitFightCalculation.Dodged())
            {
                _messageShowing.Show("Dodge");
                return;
            }

            var hitDamage = _unitFightCalculation.CalculateHitDamage(damage);

            _messageShowing.Show(Mathf.Round(hitDamage).ToString(), Color.red);
            Vitality.TakeDamage(damage);
        }
        
        public FightSpecs GetSpecs()
        {
            var health = Vitality.Health;
            var averageDamagePerSecond = PowerCalculation.CalculateAverageDps(_unitStats) * _unitStats.MeleeAccuracy;

            return new FightSpecs(health, averageDamagePerSecond);
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

        public void HideUnitVisibilityFields()
        {
            _unitFieldOfView.HideDebugShow();
            _unitFieldOfHearing.HideDebugShow();
        }

        public void ShowTargetIndicator()
        {
            _targetIndicator.Activate();
        }
        
        public void HideTargetIndicator()
        {
            _targetIndicator.Deactivate();
        }

        public void NotifyAboutAttackFrom(Unit unit)
        {
            AttackFrom?.Invoke(unit);
        }

        public void NotifyAboutLeavingAttackFrom(Unit unit)
        {
            LeavingAttackFrom?.Invoke(unit);
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
