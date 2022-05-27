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
    [RequireComponent(typeof(UnitMeshAgent))]
    [RequireComponent(typeof(UnitAttacker))]
    public class Unit : MonoBehaviour
    {
        [SerializeField] private Faction _faction;
        
        [ShowIf(nameof(_faction), Faction.Colonists)]
        [ValidateInput(nameof(ColonistUnitShouldHaveColonist), "Unit with fraction 'Colonists' should have colonist")]
        [SerializeField] private Colonist _colonist;

        [ShowIf(nameof(_faction), Faction.Enemies)]
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

        private UnitVitality _unitVitality;
        private UnitMeshAgent _unitMeshAgent;
        private UnitAttacker _unitAttacker;

        private bool _died;
        
        private UnitFightCalculation _unitFightCalculation;

        private void Awake()
        {
            _unitStats = GetComponent<UnitStats>();
            _unitTraits = GetComponent<UnitTraits>();

            _unitVitality = GetComponent<UnitVitality>();
            _unitMeshAgent = GetComponent<UnitMeshAgent>();
            _unitAttacker = GetComponent<UnitAttacker>();

            _unitFightCalculation = GetComponent<UnitFightCalculation>();
        }
        
        public event Action<Unit> AttackFrom;
        public event Action<Unit> LeavingAttackFrom;
        
        public event Action VitalityChange;
        
        public event Action Dying;

        public bool Alive => !_died;
        
        public Faction Faction => _faction;

        public UnitEquipment UnitEquipment => _unitEquipment;
        
        public Colonist Colonist => _colonist;
        public Enemy Enemy => _enemy;

        public UnitVitality UnitVitality => _unitVitality;

        private void OnEnable()
        {
            _unitVitality.VitalityChange += OnVitalityChange;
            _unitVitality.Wasted += OnWasted;
        }

        private void OnDisable()
        {
            _unitVitality.VitalityChange -= OnVitalityChange;
            _unitVitality.Wasted -= OnWasted;
        }

        private void OnDestroy()
        {
            UnbindStatsFromComponents();
        }

        public void Initialize()
        {
            BindStatsToComponents();
            
            _unitVitality.SetInitialValues();
            
            _healthBars.SetHealth(_unitVitality.Health, _unitVitality.MaxHealth);
            _healthBars.SetRecoverySpeed(_unitVitality.RecoverySpeed, _unitVitality.MaxRecoverySpeed);
        }

        [Button(ButtonSizes.Medium)]
        public void AddTrait(Trait trait)
        {
            _unitTraits.AddTrait(trait);
        }

        [Button(ButtonSizes.Medium)]
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
            _unitVitality.TakeDamage(hitDamage);
        }
        
        public FightSpecs GetSpecs()
        {
            var health = _unitVitality.Health;
            var averageDamagePerSecond = PowerCalculation.CalculateAverageDps(_unitStats) * _unitStats.MeleeAccuracy.Value;

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
        
        public bool HasWeaponOf(WeaponSlotType weaponSlotType)
        {
            return _unitEquipment.HasWeaponOf(weaponSlotType);
        }
        
        public void ChooseWeapon(WeaponSlotType weaponSlotType)
        {
            _unitEquipment.ChooseWeapon(weaponSlotType);
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
            
            _unitVitality.TakeDamage(10000f);
        }
        
        private void BindStatsToComponents()
        {
            _unitVitality.BindStats(_unitStats.MaxHealth, _unitStats.MaxRecoverySpeed);
            _unitMeshAgent.BindStats(_unitStats.MovementSpeed);
            _unitAttacker.BindStats(_unitStats.MeleeAttackSpeed, _unitStats.RangedAttackSpeed);
        }

        private void UnbindStatsFromComponents()
        {
            _unitVitality.UnbindStats(_unitStats.MaxHealth, _unitStats.MaxRecoverySpeed);
            _unitMeshAgent.UnbindStats(_unitStats.MovementSpeed);
            _unitAttacker.UnbindStats(_unitStats.MeleeAttackSpeed, _unitStats.RangedAttackSpeed);
        }

        private void OnWasted()
        {
            _died = true;
            Dying?.Invoke();
        }

        private void OnVitalityChange()
        {
            _healthBars.SetHealth(_unitVitality.Health, _unitVitality.MaxHealth);
            _healthBars.SetRecoverySpeed(_unitVitality.RecoverySpeed, _unitVitality.MaxRecoverySpeed);
            VitalityChange?.Invoke();
        }

        private bool ColonistUnitShouldHaveColonist()
        {
            if (_faction == Faction.Colonists && _colonist == null)
            {
                return false;
            }
            
            return true;
        }

        private bool EnemyUnitShouldHaveEnemy()
        {
            if (_faction == Faction.Enemies && _enemy == null)
            {
                return false;
            }
            
            return true;
        }
    }
}
