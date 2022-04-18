using System;
using System.Collections;
using DG.Tweening;
using ResourceManagement;
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
        [SerializeField] private Resource _resource;

        [Title("Rotation")]
        [Required]
        [SerializeField] private Transform _rotationPoint;
        [Required]
        [SerializeField] private Transform _rotationTransform;

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

        private Vector3 _initialRotation;

        private Coroutine _rotateCoroutine;

        public void Initialize()
        {
            _initialRotation = _rotationTransform.position;
        }

        public void OnHit(Vector3 agentPosition)
        {
            _axis = CalculateHitAxis(agentPosition);
            _currentAngle = Random.Range(_minRotationAngle, _maxRotationAngle);

            if (_rotateCoroutine != null)
            {
                DOTween.Kill(_tree.transform);
                _resource.StopCoroutine(_rotateCoroutine);
            }
            
            _rotateCoroutine = _resource.StartCoroutine(Rotate(_rotationTransform));
        }

        private IEnumerator Rotate(Transform rotationTransform)
        {
            rotationTransform.position = _initialRotation;
            
            rotationTransform.RotateAround(_rotationPoint.position, _axis, _currentAngle);

            yield return _tree.transform.DORotate(rotationTransform.rotation.eulerAngles, _rotationTime / 3)
                .WaitForCompletion();
            
            rotationTransform.RotateAround(_rotationPoint.position, _axis, -_currentAngle * _angleToOppositeDirectionMultiplier);
            
            yield return _tree.transform.DORotate(rotationTransform.rotation.eulerAngles, _rotationTime / 3)
                .WaitForCompletion();
            
            _rotationTransform.RotateAround(_rotationPoint.position, _axis, _currentAngle * (_angleToOppositeDirectionMultiplier - 1));
            
            yield return _tree.transform.DORotate(rotationTransform.rotation.eulerAngles, _rotationTime / 3)
                .WaitForCompletion();
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

        private void Fall(Action onFinish)
        {
            if (_rotateCoroutine != null)
            {
                DOTween.Kill(_tree.transform);
                _resource.StopCoroutine(_rotateCoroutine);
            }
            
            _rotationTransform.RotateAround(_rotationPoint.position, _axis, _fallAngle);
            
            _tree.transform.DOMove(_rotationTransform.position, _fallTime);
            _tree.transform.DORotate(_rotationTransform.rotation.eulerAngles, _fallTime).OnComplete(() => onFinish());
        }
    }
}
