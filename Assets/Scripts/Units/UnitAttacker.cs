using System;
using System.Collections;
using CompositionRoot.Settings;
using Sirenix.OdinInspector;
using Units.Ancillaries;
using Units.Calculations;
using Units.Enums;
using Units.Stats;
using UnityEngine;
using Zenject;

namespace Units
{
    [RequireComponent(typeof(Unit))]
    [RequireComponent(typeof(UnitStats))]
    [RequireComponent(typeof(UnitAnimator))]
    [RequireComponent(typeof(UnitSelection))]
    [RequireComponent(typeof(UnitFightCalculation))]
    public class UnitAttacker : MonoBehaviour
    {
        public event Action TrackingStart;

        public event Action AttackStart;
        public event Action AttackEnd;

        public event Action WantEscape;

        public Unit TrackedUnit { get; private set; }
        public Unit AttackedUnit { get; private set; }

        public bool IsAttacking { get; private set; }
        public bool Idle => !IsAttacking && Time.time - _lastAttackTime > _timeAfterAttackToIdle;

        public float AttackDistance => AttackRange * _attackRangeMultiplierToStartFight;

        [SerializeField] private float _timeAfterAttackToIdle = 2f;
        [Space]
        [Required]
        [SerializeField] private LineToTrackedUnit _lineToTrackedUnit;
        [Space]
        [Required]
        [SerializeField] private MessageShowing _messageShowing;

        private float AttackRange =>
            _weaponType.IsMelee() ? _unitStats.MeleeAttackRange.Value : _unitStats.RangedAttackRange.Value;

        private Unit _self;

        private Action _onInteractionFinish;

        private UnitStats _unitStats;
        private UnitAnimator _unitAnimator;

        private UnitSelection _unitSelection;

        private UnitFightCalculation _unitFightCalculation;

        private WeaponType _weaponType;

        private float _attackRangeMultiplierToStartFight;
        private float _attackAngle;
        private float _waitTime;

        private float _lastAttackTime;

        private bool _unitTargetExposed;

        private bool _hovered;
        private bool _selected;

        private bool _finalizingAttacking;

        private Coroutine _attackingCoroutine;

        [Inject]
        public void Construct(AttackSettings attackSettings, AnimationSettings animationSettings)
        {
            _attackRangeMultiplierToStartFight = attackSettings.AttackRangeMultiplierToStartFight;
            _attackAngle = attackSettings.AttackAngle;
            _waitTime = animationSettings.TimeToStopInteraction;
        }

        private void Awake()
        {
            _self = GetComponent<Unit>();

            _unitStats = GetComponent<UnitStats>();
            _unitAnimator = GetComponent<UnitAnimator>();
            _unitSelection = GetComponent<UnitSelection>();

            _unitFightCalculation = GetComponent<UnitFightCalculation>();
        }

        private void OnEnable()
        {
            _unitSelection.Hovered += OnHovered;
            _unitSelection.Selected += OnSelected;

            _unitSelection.Unhovered += OnUnhovered;
            _unitSelection.Deselected += OnDeselected;
        }

        private void OnDisable()
        {
            _unitSelection.Hovered -= OnHovered;
            _unitSelection.Selected -= OnSelected;

            _unitSelection.Unhovered -= OnUnhovered;
            _unitSelection.Deselected -= OnDeselected;
        }

        public void BindStats(Stat<UnitStat> meleeAttackSpeed, Stat<UnitStat> rangedAttackSpeed)
        {
            _unitAnimator.SetMeleeAttackSpeed(meleeAttackSpeed.Value);
            _unitAnimator.SetRangedAttackSpeed(meleeAttackSpeed.Value);

            meleeAttackSpeed.ValueChange += OnMeleeAttackSpeedChange;
            rangedAttackSpeed.ValueChange += OnRangedAttackSpeedChange;
        }

        public void UnbindStats(Stat<UnitStat> meleeAttackSpeed, Stat<UnitStat> rangedAttackSpeed)
        {
            meleeAttackSpeed.ValueChange -= OnMeleeAttackSpeedChange;
            rangedAttackSpeed.ValueChange -= OnRangedAttackSpeedChange;
        }

        public void ChangeWeaponType(WeaponType weaponType)
        {
            _weaponType = weaponType;
        }

        public void Attack(Unit unit)
        {
            AttackedUnit = unit;

            AttackedUnit.NotifyAboutAttackFrom(_self);

            _unitAnimator.Attack();

            _attackingCoroutine = StartCoroutine(CWatchForDestroy());

            IsAttacking = true;
            _finalizingAttacking = false;
            AttackStart?.Invoke();
        }

        public void Hit()
        {
            if (AttackedUnit == null || !AttackedUnit.Alive)
            {
                FinishAttacking();
                return;
            }

            if (_unitFightCalculation.Missed())
            {
                _messageShowing.Show("Miss");
                return;
            }

            MakeDamage();
        }

        private void MakeDamage()
        {
            var damage = _unitFightCalculation.CalculateDamage();

            AttackedUnit.TakeDamage(damage);

            if (!AttackedUnit.Alive)
            {
                ResetUnitTarget();
                FinishAttacking();
            }
        }

        public void FinalizeAttacking()
        {
            if (!IsAttacking || _finalizingAttacking) return;

            _finalizingAttacking = true;

            ResetAttacking();
            StartCoroutine(CFinishAttackingLater());
        }

        public void FinalizeAttackingInstantly()
        {
            ResetAttacking();
            StopAttacking();
        }

        private void OnMeleeAttackSpeedChange(float value)
        {
            _unitAnimator.SetMeleeAttackSpeed(value);
        }

        private void OnRangedAttackSpeedChange(float value)
        {
            _unitAnimator.SetRangedAttackSpeed(value);
        }

        private void OnHovered()
        {
            _hovered = true;
            TryExposeUnitTarget();
        }

        private void OnUnhovered()
        {
            _hovered = false;
            TryCoverUnitTarget();
        }

        private void OnSelected()
        {
            _selected = true;
            TryExposeUnitTarget();
        }

        private void OnDeselected()
        {
            _selected = false;
            TryCoverUnitTarget();
        }

        private void ResetAttacking()
        {
            if (_attackingCoroutine != null)
            {
                StopCoroutine(_attackingCoroutine);
                _attackingCoroutine = null;
            }

            ResetUnitTarget();
        }

        public bool OnAttackDistance(Vector3 position)
        {
            return Vector3.Distance(transform.position, position) < AttackDistance;
        }

        public bool OnAttackRange(Vector3 position)
        {
            return Vector3.Distance(transform.position, position) < AttackRange;
        }

        public bool OnAttackAngle(Vector3 position)
        {
            position.y = transform.position.y;
            var targetDirection = (position - transform.position).normalized;

            if (targetDirection == Vector3.zero)
                return true;

            var angleDifference = Quaternion.LookRotation(targetDirection).eulerAngles.y -
                                  transform.rotation.eulerAngles.y;

            return Mathf.Abs(angleDifference) < _attackAngle;
        }

        public void SetTrackedUnit(Unit trackedUnit)
        {
            TrackedUnit = trackedUnit;

            TrackingStart?.Invoke();
        }

        public void Escape()
        {
            WantEscape?.Invoke();
        }

        public void CoverUnitTarget()
        {
            _unitTargetExposed = false;

            if (TrackedUnit != null)
                TrackedUnit.HideTargetIndicator();

            _lineToTrackedUnit.HideLine();
        }

        private IEnumerator CWatchForDestroy()
        {
            while (AttackedUnit.Alive)
            {
                yield return new WaitForSeconds(_waitTime);

                if (AttackedUnit == null) break;
            }

            ResetUnitTarget();

            FinishAttacking();
        }

        private IEnumerator CFinishAttackingLater()
        {
            yield return new WaitForSeconds(_waitTime);

            FinishAttacking();
        }

        private void FinishAttacking()
        {
            if (AttackedUnit != null) return;

            if (_attackingCoroutine != null)
            {
                StopCoroutine(_attackingCoroutine);
                _attackingCoroutine = null;
            }

            _unitAnimator.FinishAttack();

            IsAttacking = false;
            _finalizingAttacking = false;

            AttackEnd?.Invoke();
        }

        private void StopAttacking()
        {
            _lastAttackTime = Time.time;

            _unitAnimator.StopAttack();

            IsAttacking = false;
            _finalizingAttacking = false;

            AttackEnd?.Invoke();
        }

        private void ResetUnitTarget()
        {
            CoverUnitTarget();

            if (TrackedUnit != null)
                TrackedUnit.HideTargetIndicator();

            if (AttackedUnit != null)
                AttackedUnit.NotifyAboutLeavingAttackFrom(_self);

            TrackedUnit = null;
            AttackedUnit = null;
        }

        private void TryExposeUnitTarget()
        {
            if (_unitTargetExposed) return;

            _unitTargetExposed = true;

            if (TrackedUnit != null)
            {
                TrackedUnit.ShowTargetIndicator();
                _lineToTrackedUnit.ShowLineTo(TrackedUnit);
            }
        }

        private void TryCoverUnitTarget()
        {
            if (!_unitTargetExposed || _hovered || _selected) return;

            CoverUnitTarget();
        }
    }
}
