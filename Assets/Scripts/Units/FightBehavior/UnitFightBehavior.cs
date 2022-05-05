using System;
using UnityEngine;

namespace Units.FightBehavior
{
    
    [RequireComponent(typeof(UnitAttacker))]
    public class UnitFightBehavior : MonoBehaviour
    {
        [SerializeField] private float _advanceTime = 3f;

        private UnitFightSpecs _selfSpecs;
        private UnitFightSpecs _opponentSpecs;
        
        private bool _fighting;
        
        private UnitAttacker _unitAttacker;

        private void Awake()
        {
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
            //RefreshSpecs(selfSpecs, opponentSpecs);
            _fighting = true;
        }

        private void StopFight()
        {
            _fighting = false;
        }

        public void RefreshSpecs(UnitFightSpecs selfSpecs, UnitFightSpecs opponentSpecs)
        {
            _selfSpecs = selfSpecs;
            _opponentSpecs = opponentSpecs;
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
