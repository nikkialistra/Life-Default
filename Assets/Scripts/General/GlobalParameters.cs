using Sirenix.OdinInspector;
using UnityEngine;

namespace General
{
    public class GlobalParameters : MonoBehaviour
    {
        [Title("Hovering")]
        [SerializeField] private float _timeToHideHover = 0.05f;
        [SerializeField] private float _timeToHideSelection = 0.12f;

        [Title("Animations")]
        [SerializeField] private float _timeToStopInteraction = 0.2f;

        [Title("Visibility Fields")]
        [SerializeField] private Vector3 _targetPositionCorrection = Vector3.up * 1.5f;
        [SerializeField] private float _visibilityFieldRecalculationTime = 0.2f;
        [SerializeField] private LayerMask _obstacleMask;

        public static GlobalParameters Instance;

        public float TimeToHideHover => _timeToHideHover;
        public float TimeToHideSelection => _timeToHideSelection;

        public float TimeToStopInteraction => _timeToStopInteraction;

        public Vector3 TargetPositionCorrection => _targetPositionCorrection;
        public float VisibilityFieldRecalculationTime => _visibilityFieldRecalculationTime;
        public LayerMask ObstacleMask => _obstacleMask;

        private void Start()
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
    }
}
