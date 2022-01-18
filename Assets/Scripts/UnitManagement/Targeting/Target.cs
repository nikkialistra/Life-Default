using System;
using ResourceManagement;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UnitManagement.Targeting
{
    public class Target : MonoBehaviour
    {
        [SerializeField] private bool _isEntity;
        
        [ShowIf("_isEntity")]
        [SerializeField] private Entity _entity;
        
        [ShowIf("_isEntity")]
        [SerializeField] private Transform _destinationPoint;
        [ShowIf("_isEntity")]
        [SerializeField] private GameObject _targetIndicator;

        public bool IsEntity => _isEntity;

        public Entity Entity => _entity;

        public Vector3 GetDestinationPoint()
        {
            CheckIsEntity();

            return _destinationPoint.position;
        }

        public void ShowIndicator()
        {
            CheckIsEntity();

            _targetIndicator.SetActive(true);
        }

        public void HideIndicator()
        {
            CheckIsEntity();

            _targetIndicator.SetActive(false);
        }

        private void CheckIsEntity()
        {
            if (!_isEntity)
            {
                throw new InvalidOperationException("Method should not be called when destination point is not set");
            }
        }
    }
}
