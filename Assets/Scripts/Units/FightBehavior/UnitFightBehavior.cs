using System;
using System.Collections;
using UnityEngine;

namespace Units.FightBehavior
{
    [RequireComponent(typeof(Unit))]
    [RequireComponent(typeof(UnitAttacker))]
    public class UnitFightBehavior : MonoBehaviour
    {
        [SerializeField] private float _advanceTime = 3f;
        [SerializeField] private float _refreshTime = 1f;

        private FightSpecs _selfSpecs;
        private FightSpecs _opponentSpecs;

        private bool _fighting;

        private Unit _self;
        private Unit _opponent;

        private UnitAttacker _unitAttacker;

        private Coroutine _choosingBehaviorCoroutine;

        private void Awake()
        {
            _self = GetComponent<Unit>();
            _unitAttacker = GetComponent<UnitAttacker>();
        }

        private void OnEnable()
        {
            _unitAttacker.AttackStart += StartFight;
            _unitAttacker.AttackEnd += StopFight;
        }

        private void OnDisable()
        {
            _unitAttacker.AttackStart -= StartFight;
            _unitAttacker.AttackEnd -= StopFight;
        }

        private void StartFight()
        {
            if (_fighting)
            {
                return;
            }
            
            _fighting = true;
            _opponent = _unitAttacker.AttackedUnit;

            _choosingBehaviorCoroutine = StartCoroutine(ChoosingBehavior());
        }

        private IEnumerator ChoosingBehavior()
        {
            while (true)
            {
                if (!TryRefreshSpecs())
                {
                    break;
                }

                if (WouldBeDefeated())
                {
                    _unitAttacker.Escape();
                }

                yield return new WaitForSeconds(_refreshTime);
            }
        }

        private void StopFight()
        {
            if (_choosingBehaviorCoroutine != null)
            {
                StopCoroutine(_choosingBehaviorCoroutine);
                _choosingBehaviorCoroutine = null;
            }
            
            _fighting = false;
            _opponent = null;
        }

        private bool TryRefreshSpecs()
        {
            _selfSpecs = _self.GetSpecs();

            if (!_opponent.Alive)
            {
                StopFight();
                return false;
            }
            
            _opponentSpecs = _opponent.GetSpecs();
            return true;
        }

        public bool WouldBeDefeated()
        {
            if (!_fighting)
            {
                throw new InvalidOperationException("Trying to check condition of not started fight");
            }

            var winTime = _selfSpecs.WouldWinInTime(_opponentSpecs, _advanceTime);
            var loseTime = _selfSpecs.WouldLoseInTime(_opponentSpecs, _advanceTime);

            if (float.IsNegativeInfinity(loseTime))
            {
                return false;
            }

            if (!float.IsNegativeInfinity(winTime) && winTime < loseTime)
            {
                return false;
            }

            return true;
        }
    }
}
