using System;
using System.Collections;
using General;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Units
{
    [RequireComponent(typeof(UnitStats))]
    [RequireComponent(typeof(UnitAnimator))]
    [RequireComponent(typeof(UnitSelection))]
    public class UnitAttacker : MonoBehaviour
    {
        [SerializeField] private float _timeAfterAttackToIdle = 2f;
        [Space]
        [Required]
        [SerializeField] private MessageShowing _messageShowing;

        private Unit _trackedUnit;
        private Unit _attackedUnit;
        
        private Action _onInteractionFinish;

        private UnitStats _unitStats;
        private UnitAnimator _unitAnimator;
        
        private UnitSelection _unitSelection;

        private float _attackAngle;

        private float _waitTime;

        private float _lastAttackTime;

        private Coroutine _attackingCoroutine;

        private void Awake()
        {
            _unitStats = GetComponent<UnitStats>();
            _unitAnimator = GetComponent<UnitAnimator>();
            _unitSelection = GetComponent<UnitSelection>();
        }

        public event Action AttackStart;
        public event Action AttackEnd;

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
            _unitSelection.Hovered += ExposeUnitTarget;
            _unitSelection.Unhovered += CoverUnitTarget;
        }

        private void OnDisable()
        {
            _unitSelection.Hovered -= ExposeUnitTarget;
            _unitSelection.Unhovered -= CoverUnitTarget;
        }

        public float AttackRange => _unitStats.MeleeAttackRange;

        public void Attack(Unit unit)
        {
            _attackedUnit = unit;
            _unitAnimator.Attack(true);
            
            _attackingCoroutine = StartCoroutine(WatchForDestroy());

            IsAttacking = true;
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
            ResetAttacking();
            StartCoroutine(FinishAttackingLater());
        }

        public void FinalizeAttackingInstantly()
        {
            ResetAttacking();
            StopAttacking();
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
            
            _unitAnimator.Attack(false);
            IsAttacking = false;
            AttackEnd?.Invoke();
        }

        private void StopAttacking()
        {
            _lastAttackTime = Time.time;
            
            _unitAnimator.Attack(false);
            IsAttacking = false;
            AttackEnd?.Invoke();
        }

        private void ResetUnitTarget()
        {
            if (_trackedUnit != null)
            {
                _trackedUnit.HideTargetIndicator();
            }

            _trackedUnit = null;
            _attackedUnit = null;
        }

        private void ExposeUnitTarget()
        {
            if (_trackedUnit != null)
            {
                _trackedUnit.ShowTargetIndicator();
            }
        }

        private void CoverUnitTarget()
        {
            if (_trackedUnit != null)
            {
                _trackedUnit.HideTargetIndicator();
            }
        }
    }
}