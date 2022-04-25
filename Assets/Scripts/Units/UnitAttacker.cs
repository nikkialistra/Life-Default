using System;
using System.Collections;
using General;
using UnityEngine;

namespace Units
{
    [RequireComponent(typeof(UnitStats))]
    [RequireComponent(typeof(UnitAnimator))]
    public class UnitAttacker : MonoBehaviour
    {
        private Unit _unit;
        
        private Action _onInteractionFinish;

        private UnitStats _unitStats;
        private UnitAnimator _unitAnimator;
        
        private float _waitTime;

        private Coroutine _attackingCoroutine;

        private void Awake()
        {
            _unitStats = GetComponent<UnitStats>();
            _unitAnimator = GetComponent<UnitAnimator>();
        }

        public bool IsAttacking { get; private set; }

        private void Start()
        {
            _waitTime = GlobalParameters.Instance.TimeToStopInteraction;
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
                StopAttacking();
                return;
            }

            var damage = _unitStats.MeleeDamagePerSecond * passedTime;

            _unit.TakeDamage(damage);

            if (!_unit.Alive)
            {
                _unit = null;
                StopAttacking();
            }
        }

        public void FinishAttacking()
        {
            if (_attackingCoroutine != null)
            {
                StopCoroutine(_attackingCoroutine);
                _attackingCoroutine = null;
            }

            _unit = null;

            StartCoroutine(StopAttackingLater());
        }

        public bool OnAttackRange(Vector3 position)
        {
            return Vector3.Distance(transform.position, position) < _unitStats.MeleeAttackRange;
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

            StopAttacking();
        }

        private IEnumerator StopAttackingLater()
        {
            yield return new WaitForSeconds(_waitTime);

            StopAttacking();
        }

        private void StopAttacking()
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
    }
}