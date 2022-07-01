using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ResourceManagement.Animations
{
    [Serializable]
    public class TreeAnimations : IAnimations
    {
        [Required]
        [SerializeField] private Transform _tree;

        [Title("Rotation")]
        [SerializeField] private float _minTimeBetweenRotateAnimations = 0.2f;
        [Space]
        [Required]
        [SerializeField] private Transform _rotationPoint;
        [Required]
        [SerializeField] private Transform _rotationTransform;

        [Title("On Hit")]
        [Required]
        [SerializeField] private float _maxRotationAngle = 10f;
        [SerializeField] private float _minRotationAngle = 8f;

        [SerializeField] private float _angleToOppositeDirectionMultiplier = 1.3f;

        [Range(0.03f, 0.3f)]
        [SerializeField] private float _oneDirectionRotationTime = 0.2f;

        [Title("On Destroy")]
        [SerializeField] private float _fallAngle = 90f;
        [SerializeField] private float _fallTime = 1.5f;

        private Vector3 _axis;
        private float _currentAngle;

        private float _lastHitTime = 0f;

        public void OnHit(Vector3 agentPosition)
        {
            if (Time.time - _lastHitTime < _minTimeBetweenRotateAnimations) return;

            _lastHitTime = Time.time;

            DOTween.Kill(_tree.transform);

            _axis = CalculateHitAxis(agentPosition);
            _currentAngle = Random.Range(_minRotationAngle, _maxRotationAngle);

            Rotate();
        }

        public void OnDestroy(Vector3 agentPosition, Action onFinish)
        {
            DOTween.Kill(_tree.transform);

            _axis = CalculateHitAxis(agentPosition);

            Fall(onFinish);
        }

        private void Rotate()
        {
            ResetRotationTransform();

            var sequence = DOTween.Sequence();

            CalculateRotationToLeft(sequence);
            CalculateRotationToRight(sequence);
            CalculateRotationToInitialPosition(sequence);

            sequence.Play();
        }

        private void CalculateRotationToLeft(Sequence sequence)
        {
            _rotationTransform.RotateAround(_rotationPoint.position, _axis, _currentAngle);

            sequence.Insert(0, _tree.transform.DOMove(_rotationTransform.position, _oneDirectionRotationTime));
            sequence.Insert(0,
                _tree.transform.DORotate(_rotationTransform.rotation.eulerAngles, _oneDirectionRotationTime));
        }

        private void CalculateRotationToRight(Sequence sequence)
        {
            _rotationTransform.RotateAround(_rotationPoint.position, _axis,
                -_currentAngle * _angleToOppositeDirectionMultiplier);

            sequence.Insert(_oneDirectionRotationTime,
                _tree.transform.DOMove(_rotationTransform.position, _oneDirectionRotationTime));
            sequence.Insert(_oneDirectionRotationTime,
                _tree.transform.DORotate(_rotationTransform.rotation.eulerAngles, _oneDirectionRotationTime));
        }

        private void CalculateRotationToInitialPosition(Sequence sequence)
        {
            _rotationTransform.RotateAround(_rotationPoint.position, _axis,
                _currentAngle * (_angleToOppositeDirectionMultiplier - 1));

            sequence.Insert(2 * _oneDirectionRotationTime,
                _tree.transform.DOMove(_rotationTransform.position, _oneDirectionRotationTime));
            sequence.Insert(2 * _oneDirectionRotationTime,
                _tree.transform.DORotate(_rotationTransform.rotation.eulerAngles, _oneDirectionRotationTime));
        }

        private void ResetRotationTransform()
        {
            _rotationTransform.localPosition = Vector3.zero;
            _rotationTransform.localRotation = Quaternion.Euler(Vector3.zero);
        }

        private Vector3 CalculateHitAxis(Vector3 agentPosition)
        {
            return new Vector3(_tree.position.x - agentPosition.x, 0,
                _tree.position.z - agentPosition.z);
        }

        private void Fall(Action onFinish)
        {
            _rotationTransform.RotateAround(_rotationPoint.position, _axis, _fallAngle);

            _tree.transform.DOMove(_rotationTransform.position, _fallTime);
            _tree.transform.DORotate(_rotationTransform.rotation.eulerAngles, _fallTime).OnComplete(() => onFinish());
        }
    }
}
