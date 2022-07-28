using System;
using Units;
using UnityEngine;

namespace Aborigines
{
    [RequireComponent(typeof(UnitAnimator))]
    public class AborigineAnimator : MonoBehaviour
    {
        private UnitAnimator _unitAnimator;

        private void Awake()
        {
            _unitAnimator = GetComponent<UnitAnimator>();
        }

        public void Idle()
        {
            _unitAnimator.Idle();
        }

        public void Move()
        {
            _unitAnimator.Move();
        }

        public void Die(Action died)
        {
            _unitAnimator.Die(died);
        }
    }
}
