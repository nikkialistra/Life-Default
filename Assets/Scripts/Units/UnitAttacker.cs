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
    public class UnitAttacker : MonoBehaviour
    {
        [SerializeField] private float _timeAfterAttackToIdle = 2f;
        [Space]
        [Required]
        [SerializeField] private MessageShowing _messageShowing;

        private Unit _unit;
        
        private Action _onInteractionFinish;

        private UnitStats _unitStats;
        private UnitAnimator _unitAnimator;

        private float _attackAngle;
        
        private float _waitTime;

        private float _lastAttackTime;
        
        private Coroutine _attackingCoroutine;

        private void Awake()
        {
            _unitStats = GetComponent<UnitStats>();
            _unitAnimator = GetComponent<UnitAnimator>();
        }

        public bool IsAttacking { get; private set; }
        public bool Idle => !IsAttacking && Time.time - _lastAttackTime > _timeAfterAttackToIdle;

        private void Start()
        {
            _attackAngle = GlobalParameters.Instance.AttackAngle;
            _waitTime = GlobalParameters.Instance.TimeToStopInteraction;
            
            _unitAnimator.SetAttackSpeed(_unitStats.MeleeAttackSpeed);
        }

        public float AttackRange => _unitStats.MeleeAttackRange;

        public void Attack(Unit unit)
        {
            _unit = unit;
            _unitAnimator.Attack(true);
            
            _attackingCoroutine = StartCoroutine(WatchForDestroy());

            IsAttacking = true;
        }

        public void Hit(float passedTime)
        {
            if (_unit == null || !_unit.Alive)
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

            _unit.TakeDamage(damage);

            if (!_unit.Alive)
            {
                _unit = null;
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

            _unit = null;
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

        private IEnumerator WatchForDestroy()
        {
            while (_unit.Alive)
            {
                yield return new WaitForSeconds(_waitTime);

                if (_unit == null)
                {
                    break;
                }
            }

            _unit = null;

            FinishAttacking();
        }

        private IEnumerator FinishAttackingLater()
        {
            yield return new WaitForSeconds(_waitTime);

            FinishAttacking();
        }

        private void FinishAttacking()
        {
            if (_unit != null)
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
        }

        private void StopAttacking()
        {
            _unitAnimator.Attack(false);
            IsAttacking = false;

            _lastAttackTime = Time.time;
        }
    }
}