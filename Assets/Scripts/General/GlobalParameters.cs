using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace General
{
    public class GlobalParameters : MonoBehaviour
    {
        [Title("Units")]
        [SerializeField] private float _healthFractionToDecreaseRecoverySpeed = 0.8f;
        [SerializeField] private float _recoveryHealthDelayAfterHit = 5f;
        
        [Title("Hovering")]
        [SerializeField] private float _timeToHideHover = 0.05f;
        [SerializeField] private float _timeToHideSelection = 0.12f;

        [Title("Animations")]
        [SerializeField] private float _timeToStopInteraction = 0.2f;
        [SerializeField] private float _waitTimeToIdle;

        [Title("Visibility Fields")]
        [SerializeField] private Vector3 _targetPositionCorrection = Vector3.up * 1.5f;
        [SerializeField] private float _visibilityFieldRecalculationTime = 0.2f;
        [SerializeField] private LayerMask _obstacleMask;
        
        [Title("Terrain Raycasting")]
        [SerializeField] private Vector3 _raycastToTerrainCorrection = Vector3.up * 10f;

        [Title("Attacking")]
        [SerializeField] private float _attackRangeMultiplierToStartFight = 0.75f;
        [SerializeField] private float _attackAngle = 60f;
        [SerializeField] private float _seekPredictionMultiplier = 2f;

        public static GlobalParameters Instance { get; private set; }

        public float HealthFractionToDecreaseRecoverySpeed => _healthFractionToDecreaseRecoverySpeed;
        public float RecoveryHitDelayAfterHit => _recoveryHealthDelayAfterHit;

        public float TimeToHideHover => _timeToHideHover;
        public float TimeToHideSelection => _timeToHideSelection;

        public float TimeToStopInteraction => _timeToStopInteraction;
        public float WaitTimeToIdle => _waitTimeToIdle;

        public Vector3 TargetPositionCorrection => _targetPositionCorrection;
        public float VisibilityFieldRecalculationTime => _visibilityFieldRecalculationTime;
        public LayerMask ObstacleMask => _obstacleMask;

        public Vector3 RaycastToTerrainCorrection => _raycastToTerrainCorrection;


        public float AttackRangeMultiplierToStartFight => _attackRangeMultiplierToStartFight;
        public float AttackAngle => _attackAngle;
        public float SeekPredictionMultiplier => _seekPredictionMultiplier;
        

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance == this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            AnimancerState.AutomaticallyClearEvents = false;
        }
    }
}
