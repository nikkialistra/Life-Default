using System;
using Aborigines;
using Colonists;
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
        public event Action<Unit> AttackFrom;
        public event Action<Unit> LeavingAttackFrom;

        public event Action VitalityChange;

        public event Action Dying;

        public bool Alive => !_died;

        public Faction Faction => _faction;

        public UnitEquipment UnitEquipment => _unitEquipment;

        public Colonist Colonist => _colonist;
        public Aborigine Aborigine => _aborigine;

        public UnitVitality UnitVitality { get; private set; }

        [SerializeField] private Faction _faction;

        [ShowIf(nameof(_faction), Faction.Colonists)]
        [ValidateInput(nameof(ColonistUnitShouldHaveColonist), "Unit with fraction 'Colonists' should have colonist")]
        [SerializeField] private Colonist _colonist;

        [ShowIf(nameof(_faction), Faction.Aborigines)]
        [ValidateInput(nameof(AborigineUnitShouldHaveAborigine), "Unit with fraction 'Aborigines' should have aborigine")]
        [SerializeField] private Aborigine _aborigine;

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

        private UnitMeshAgent _unitMeshAgent;
        private UnitAttacker _unitAttacker;

        private bool _died;

        private UnitFightCalculation _unitFightCalculation;

        private void Awake()
        {
            _unitStats = GetComponent<UnitStats>();
            _unitTraits = GetComponent<UnitTraits>();

            UnitVitality = GetComponent<UnitVitality>();
            _unitMeshAgent = GetComponent<UnitMeshAgent>();
            _unitAttacker = GetComponent<UnitAttacker>();

            _unitFightCalculation = GetComponent<UnitFightCalculation>();
        }

        private void OnEnable()
        {
            UnitVitality.VitalityChange += OnVitalityChange;
            UnitVitality.Wasted += OnWasted;
        }

        private void OnDisable()
        {
            UnitVitality.VitalityChange -= OnVitalityChange;
            UnitVitality.Wasted -= OnWasted;
        }

        private void OnDestroy()
        {
            UnbindStatsFromComponents();
        }

        public void Initialize()
        {
            BindStatsToComponents();

            UnitVitality.SetInitialValues();

            _healthBars.SetHealth(UnitVitality.Health, UnitVitality.MaxHealth);
            _healthBars.SetRecoverySpeed(UnitVitality.RecoverySpeed, UnitVitality.MaxRecoverySpeed);
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
            if (_died) return;

            if (_unitFightCalculation.Dodged())
            {
                _messageShowing.Show("Dodge");
                return;
            }

            var hitDamage = _unitFightCalculation.CalculateHitDamage(damage);

            _messageShowing.Show(Mathf.Round(hitDamage).ToString(), Color.red);
            UnitVitality.TakeDamage(hitDamage);
        }

        public FightSpecs GetSpecs()
        {
            var health = UnitVitality.Health;
            var averageDamagePerSecond =
                PowerCalculation.CalculateAverageDps(_unitStats) * _unitStats.MeleeAccuracy.Value;

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
            if (_died) return;

            UnitVitality.TakeDamage(10000f);
        }

        private void BindStatsToComponents()
        {
            UnitVitality.BindStats(_unitStats.MaxHealth, _unitStats.MaxRecoverySpeed);
            _unitMeshAgent.BindStats(_unitStats.MovementSpeed);
            _unitAttacker.BindStats(_unitStats.MeleeAttackSpeed, _unitStats.RangedAttackSpeed);
        }

        private void UnbindStatsFromComponents()
        {
            UnitVitality.UnbindStats(_unitStats.MaxHealth, _unitStats.MaxRecoverySpeed);
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
            _healthBars.SetHealth(UnitVitality.Health, UnitVitality.MaxHealth);
            _healthBars.SetRecoverySpeed(UnitVitality.RecoverySpeed, UnitVitality.MaxRecoverySpeed);
            VitalityChange?.Invoke();
        }

        private bool ColonistUnitShouldHaveColonist()
        {
            if (_faction == Faction.Colonists && _colonist == null)
                return false;

            return true;
        }

        private bool AborigineUnitShouldHaveAborigine()
        {
            if (_faction == Faction.Aborigines && _aborigine == null)
                return false;

            return true;
        }
    }
}
