using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ResourceManagement.Animations
{
    [Serializable]
    public class RockAnimations : IAnimations
    {
        [Required]
        [SerializeField] private Transform _rock;

        [Title("Rotation")]
        [SerializeField] private float _minTimeBetweenRotateAnimations = 0.2f;
        [Space]
        [Required]
        [SerializeField] private Transform _rotationPoint;
        [Required]
        [SerializeField] private Transform _rotationTransform;

        [Title("On Hit")]
        [Required]
        [SerializeField] private float _maxRotationAngle = 3f;
        [SerializeField] private float _minRotationAngle = 1f;

        [Range(0.03f, 0.3f)]
        [SerializeField] private float _oneDirectionRotationTime = 0.2f;
        
        [Title("On Destroy")]
        [SerializeField] private float _collapseTime = 0.5f;

        private Vector3 _axis;
        private float _currentAngle;
        
        private float _lastHitTime = 0f;

        public void OnHit(Vector3 agentPosition)
        {
            if (Time.time - _lastHitTime < _minTimeBetweenRotateAnimations)
            {
                return;
            }
            
            _lastHitTime = Time.time;
            
            DOTween.Kill(_rock.transform);
            
            _axis = CalculateHitAxis(agentPosition);
            _currentAngle = Random.Range(_minRotationAngle, _maxRotationAngle);

            Rotate();
        }

        public void OnDestroy(Vector3 agentPosition, Action onFinish)
        {
            DOTween.Kill(_rock.transform);
            
            _axis = CalculateHitAxis(agentPosition);

            Fall(onFinish);
        }

        private void Rotate()
        {
            ResetRotationTransform();

            var sequence = DOTween.Sequence();

            CalculateRotationToAgent(sequence);
            CalculateRotationToInitialPosition(sequence);

            sequence.Play();
        }

        private void CalculateRotationToAgent(Sequence sequence)
        {
            _rotationTransform.RotateAround(_rotationPoint.position, _axis, _currentAngle);
            
            sequence.Insert(0, _rock.transform.DOMove(_rotationTransform.position, _oneDirectionRotationTime));
            sequence.Insert(0, _rock.transform.DORotate(_rotationTransform.rotation.eulerAngles, _oneDirectionRotationTime));
        }

        private void CalculateRotationToInitialPosition(Sequence sequence)
        {
            ResetRotationTransform();
            
            sequence.Insert(_oneDirectionRotationTime, _rock.transform.DOMove(_rotationTransform.position, _oneDirectionRotationTime));
            sequence.Insert(_oneDirectionRotationTime, _rock.transform.DORotate(_rotationTransform.rotation.eulerAngles, _oneDirectionRotationTime));
        }

        private void ResetRotationTransform()
        {
            _rotationTransform.localPosition = Vector3.zero;
            _rotationTransform.localRotation = Quaternion.Euler(Vector3.zero);
        }

        private Vector3 CalculateHitAxis(Vector3 agentPosition)
        {
            var direction = new Vector2(_rock.position.x - agentPosition.x, _rock.position.z - agentPosition.z);
            var perpendicular = Vector2.Perpendicular(direction);
            
            return new Vector3(perpendicular.x, 0,
                perpendicular.y);
        }

        private void Fall(Action onFinish)
        {
            _rock.transform.DOScale(Vector3.zero, _collapseTime).OnComplete(() => onFinish());
        }
    }
}
