using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Units.FightBehavior
{
    [RequireComponent(typeof(Unit))]
    [RequireComponent(typeof(UnitAttacker))]
    public class UnitFightBehavior : MonoBehaviour
    {
        [SerializeField] private float _advanceTime = 3f;
        [SerializeField] private float _refreshTime = 1f;
        
        private bool _fighting;

        private Unit _self;
        private Unit _opponent;

        private FightSpecs _selfSpecs;
        private FightSpecs _opponentSpecs;

        private Dictionary<Unit, FightSpecs> _surroundingOpponentsSpecs = new();

        private UnitAttacker _unitAttacker;

        private Coroutine _choosingBehaviorCoroutine;

        private void Awake()
        {
            _self = GetComponent<Unit>();
            _unitAttacker = GetComponent<UnitAttacker>();
        }

        private void OnEnable()
        {
            _self.AttackFrom += AddOpponent;
            _self.LeavingAttackFrom += RemoveOpponent;
            
            _unitAttacker.AttackStart += StartFight;
            _unitAttacker.AttackEnd += StopFight;
        }

        private void OnDisable()
        {
            _self.AttackFrom -= AddOpponent;
            _self.LeavingAttackFrom -= RemoveOpponent;
            
            _unitAttacker.AttackStart -= StartFight;
            _unitAttacker.AttackEnd -= StopFight;
        }

        private void AddOpponent(Unit opponent)
        {
            if (_fighting && _opponent == opponent)
            {
                return;
            }
            
            _surroundingOpponentsSpecs.Add(opponent, opponent.GetSpecs());
        }

        private void RemoveOpponent(Unit opponent)
        {
            _surroundingOpponentsSpecs.Remove(opponent);
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

            if (_opponent == null || !_opponent.Alive)
            {
                StopFight();
                return false;
            }
            
            _opponentSpecs = _opponent.GetSpecs();
            return true;
        }

        private bool WouldBeDefeated()
        {
            if (!_fighting)
            {
                throw new InvalidOperationException("Trying to check condition of not started fight");
            }

            var surroundingOpponentsSpecs = _surroundingOpponentsSpecs.Values.ToList();

            var winTime = _selfSpecs.WouldWinInTime(_opponentSpecs, _advanceTime);
            var loseTime = _selfSpecs.WouldLoseInTime(_opponentSpecs, surroundingOpponentsSpecs, _advanceTime);

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
