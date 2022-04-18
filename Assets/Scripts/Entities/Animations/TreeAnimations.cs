using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Entities.Animations
{
    [Serializable]
    public class TreeAnimations : IAnimations
    {
        [Required]
        [SerializeField] private Transform _tree;
        [Required]
        
        [Title("Rotation")]
        [SerializeField] private Transform _rotationPoint;
        [Required]
        [SerializeField] private Transform _endTransform;
        
        [Title("On Hit")]
        [Required]
        [SerializeField] private float _maxRotationAngle = 10f;
        [SerializeField] private float _minRotationAngle = 8f;
        
        [SerializeField] private float _angleToOppositeDirectionMultiplier = 1.3f;
        
        [Range(0.1f, 0.9f)]
        [SerializeField] private float _rotationTime = 0.6f;
        
        [Title("On Destroy")]
        [SerializeField] private float _fallAngle = 90f;
        [SerializeField] private float _fallTime = 1.5f;

        private Vector3 _axis;
        
        private float _currentAngle;
        
        private bool _hitAnimationPlaying;

        public void OnHit(Vector3 agentPosition)
        {
            if (_hitAnimationPlaying)
            {
                return;
            }
            
            _hitAnimationPlaying = true;
            
            _axis = CalculateHitAxis(agentPosition);
            _currentAngle = Random.Range(_minRotationAngle, _maxRotationAngle);

            RotateToLeft();
        }

        public void OnDestroy(Vector3 agentPosition, Action onFinish)
        {
            _axis = CalculateHitAxis(agentPosition);
            Fall(onFinish);
        }

        private Vector3 CalculateHitAxis(Vector3 lumberjackPosition)
        {
            return new Vector3(_tree.position.x - lumberjackPosition.x, 0,
                _tree.position.z - lumberjackPosition.z);
        }

        private void RotateToLeft()
        {
            _endTransform.RotateAround(_rotationPoint.position, _axis, _currentAngle);

            _tree.transform.DOMove(_endTransform.position, _rotationTime / 3);
            _tree.transform.DORotate(_endTransform.rotation.eulerAngles, _rotationTime / 3).OnComplete(RotateToRight);
        }

        private void RotateToRight()
        {
            _endTransform.RotateAround(_rotationPoint.position, _axis, -_currentAngle * _angleToOppositeDirectionMultiplier);

            _tree.transform.DOMove(_endTransform.position, _rotationTime / 3);
            _tree.transform.DORotate(_endTransform.rotation.eulerAngles, _rotationTime / 3).OnComplete(RotateToStartingPosition);
        }

        private void RotateToStartingPosition()
        {
            _endTransform.RotateAround(_rotationPoint.position, _axis, _currentAngle * (_angleToOppositeDirectionMultiplier - 1));

            _tree.transform.DOMove(_endTransform.position, _rotationTime / 3);
            _tree.transform.DORotate(_endTransform.rotation.eulerAngles, _rotationTime / 3);

            _hitAnimationPlaying = false;
        }

        private void Fall(Action onFinish)
        {
            DOTween.Kill(_tree.transform);
            
            _endTransform.RotateAround(_rotationPoint.position, _axis, _fallAngle);
            
            _tree.transform.DOMove(_endTransform.position, _fallTime);
            _tree.transform.DORotate(_endTransform.rotation.eulerAngles, _fallTime).OnComplete(() => onFinish());
        }
    }
}
