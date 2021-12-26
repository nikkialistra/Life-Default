using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Units.Unit
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitAnimator : MonoBehaviour
    {
        [Required]
        [SerializeField] private Animator _animator;
        
        private NavMeshAgent _navMeshAgent;

        private readonly int _velocity = Animator.StringToHash("velocity");

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Update() => SetAnimatorVelocity();

        private void SetAnimatorVelocity()
        {
            var velocity = _navMeshAgent.velocity.magnitude;
            _animator.SetFloat(_velocity, velocity);
        }
    }
}