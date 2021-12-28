using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Units.Unit
{
    [RequireComponent(typeof(UnitFacade))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitAnimator : MonoBehaviour
    {
        [Required]
        [SerializeField] private Animator _animator;
        
        public event Action DeathFinish;

        private UnitFacade _unitFacade;
        private NavMeshAgent _navMeshAgent;

        private readonly int _velocity = Animator.StringToHash("velocity");
        private readonly int _death = Animator.StringToHash("death");

        private void Awake()
        {
            _unitFacade = GetComponent<UnitFacade>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            SetAnimatorVelocity();
        }

        private void OnEnable()
        {
            _unitFacade.Die += Die;
        }

        private void OnDisable()
        {
            _unitFacade.Die -= Die;
        }

        private void Die()
        {
            _animator.SetTrigger(_death);
            StartCoroutine(WaitDeathFinish());
        }

        private IEnumerator WaitDeathFinish()
        {
            yield return null;
            
            while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.5f)
            {
                yield return null;
            }
            
            DeathFinish?.Invoke();
        }

        private void SetAnimatorVelocity()
        {
            var velocity = _navMeshAgent.velocity.magnitude;
            _animator.SetFloat(_velocity, velocity);
        }
    }
}