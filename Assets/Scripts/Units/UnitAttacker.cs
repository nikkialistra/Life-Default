using System;
using System.Collections;
using General;
using Sirenix.OdinInspector;
using Units.Ancillaries;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Units
{
    [RequireComponent(typeof(Unit))]
    [RequireComponent(typeof(UnitStats))]
    [RequireComponent(typeof(UnitAnimator))]
    [RequireComponent(typeof(UnitSelection))]
    public class UnitAttacker : MonoBehaviour
    {
        [SerializeField] private float _timeAfterAttackToIdle = 2f;
        [Space]
        [Required]
        [SerializeField] private LineToTrackedUnit _lineToTrackedUnit;
        [Space]
        [Required]
        [SerializeField] private MessageShowing _messageShowing;
        [Required]
        [SerializeField] private UnitEquipment _unitEquipment;

        private Unit _self;
        
        private Unit _trackedUnit;
        private Unit _attackedUnit;
        
        private Action _onInteractionFinish;

        private UnitStats _unitStats;
        private UnitAnimator _unitAnimator;
        
        private UnitSelection _unitSelection;

        private float _attackAngle;

        private float _waitTime;

        private float _lastAttackTime;
        
        private bool _unitTargetExposed;

        private bool _hovered;
        private bool _selected;

        private bool _finalizingAttacking;
        
        private Coroutine _attackingCoroutine;

        private void Awake()
        {
            _self = GetComponent<Unit>();
            
            _unitStats = GetComponent<UnitStats>();
            _unitAnimator = GetComponent<UnitAnimator>();
            _unitSelection = GetComponent<UnitSelection>();
        }

        public event Action TrackingStart;

        public event Action AttackStart;
        public event Action AttackEnd;

        public event Action WantEscape;

        public bool IsAttacking { get; private set; }
        public bool Idle => !IsAttacking && Time.time - _lastAttackTime > _timeAfterAttackToIdle;

        private void Start()
        {
            _attackAngle = GlobalParameters.Instance.AttackAngle;
            _waitTime = GlobalParameters.Instance.TimeToStopInteraction;
            
            _unitAnimator.SetAttackSpeed(_unitStats.MeleeAttackSpeed);
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

        public float AttackDistance => _unitStats.MeleeAttackDistance;

        public Unit TrackedUnit => _trackedUnit;
        public Unit AttackedUnit => _attackedUnit;
        
        public bool HoldingWeapon => IsAttacking || _trackedUnit != null;

        public void Attack(Unit unit)
        {
            _attackedUnit = unit;

            _attackedUnit.NotifyAboutAttackFrom(_self);
            
            _unitAnimator.Attack();
            
            _attackingCoroutine = StartCoroutine(WatchForDestroy());

            IsAttacking = true;
            _finalizingAttacking = false;
            AttackStart?.Invoke();
        }

        public void Hit(float passedTime)
        {
            if (_attackedUnit == null || !_attackedUnit.Alive)
            {
                FinishAttacking();
                return;
            }

            if (Miss())
            {
                _messageShowing.Show("Miss");
                return;
            }

            MakeDamage(passedTime);
        }

        private bool Miss()
        {
            return Random.Range(0f, 1f) > _unitStats.MeleeAccuracy;
        }

        private void MakeDamage(float passedTime)
        {
            var damage = _unitStats.MeleeDamagePerSecond * passedTime;

            _attackedUnit.TakeDamage(damage);

            if (!_attackedUnit.Alive)
            {
                ResetUnitTarget();
                FinishAttacking();
            }
        }

        public void FinalizeAttacking()
        {
            if (!IsAttacking || _finalizingAttacking)
            {
                return;
            }
            
            _finalizingAttacking = true;
            
            ResetAttacking();
            StartCoroutine(FinishAttackingLater());
        }

        public void FinalizeAttackingInstantly()
        {
            ResetAttacking();
            StopAttacking();
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
            return Vector3.Distance(transform.position, position) < _unitStats.MeleeAttackDistance;
        }

        public bool OnAttackRange(Vector3 position)
        {
            return Vector3.Distance(transform.position, position) < _unitStats.MeleeAttackRange;
        }

        public bool OnAttackAngle(Vector3 position)
        {
            position.y = transform.position.y;
            var targetDirection = (position - transform.position).normalized;

            if (targetDirection == Vector3.zero)
            {
                return true;
            }

            var angleDifference = Quaternion.LookRotation(targetDirection).eulerAngles.y -
                                  transform.rotation.eulerAngles.y;

            return Mathf.Abs(angleDifference) < _attackAngle;
        }

        public void SetTrackedUnit(Unit trackedUnit)
        {
            _trackedUnit = trackedUnit;
            
            TrackingStart?.Invoke();
        }

        public void Escape()
        {
            WantEscape?.Invoke();
        }

        public void CoverUnitTarget()
        {
            _unitTargetExposed = false;

            if (_trackedUnit != null)
            {
                _trackedUnit.HideTargetIndicator();
            }
            
            _lineToTrackedUnit.HideLine();
        }

        private IEnumerator WatchForDestroy()
        {
            while (_attackedUnit.Alive)
            {
                yield return new WaitForSeconds(_waitTime);

                if (_attackedUnit == null)
                {
                    break;
                }
            }

            ResetUnitTarget();

            FinishAttacking();
        }

        private IEnumerator FinishAttackingLater()
        {
            yield return new WaitForSeconds(_waitTime);

            FinishAttacking();
        }

        private void FinishAttacking()
        {
            if (_attackedUnit != null)
            {
                return;
            }

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
            
            if (_trackedUnit != null)
            {
                _trackedUnit.HideTargetIndicator();
            }

            if (_attackedUnit != null)
            {
                _attackedUnit.NotifyAboutLeavingAttackFrom(_self);
            }
            
            _trackedUnit = null;
            _attackedUnit = null;
        }

        private void TryExposeUnitTarget()
        {
            if (_unitTargetExposed)
            {
                return;
            }

            _unitTargetExposed = true;

            if (_trackedUnit != null)
            {
                _trackedUnit.ShowTargetIndicator();
                _lineToTrackedUnit.ShowLineTo(_trackedUnit);
            }
        }

        private void TryCoverUnitTarget()
        {
            if (!_unitTargetExposed || _hovered || _selected)
            {
                return;
            }

            CoverUnitTarget();
        }
    }
}