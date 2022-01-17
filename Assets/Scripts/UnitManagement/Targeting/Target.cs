using System;
using ResourceManagement;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UnitManagement.Targeting
{
    public class Target : MonoBehaviour
    {
        [Title("Destination")]
        [SerializeField] private bool _hasDestinationPoint;
        
        [ShowIf("_hasDestinationPoint")]
        [SerializeField] private Transform _destinationPoint;
        [ShowIf("_hasDestinationPoint")]
        [SerializeField] private GameObject _targetIndicator;
        
        [Title("Resource")]
        [SerializeField] private bool _hasResource;

        [ShowIf("_hasResource")]
        [SerializeField] private Resource _resource;

        public bool HasDestinationPoint => _hasDestinationPoint;
        public bool HasResource => _hasResource;

        public Resource Resource => _resource;

        public Vector3 GetDestinationPoint()
        {
            CheckHavingDestinationPoint();

            return _destinationPoint.position;
        }

        public void ShowIndicator()
        {
            CheckHavingDestinationPoint();

            _targetIndicator.SetActive(true);
        }

        public void HideIndicator()
        {
            CheckHavingDestinationPoint();

            _targetIndicator.SetActive(false);
        }

        private void CheckHavingDestinationPoint()
        {
            if (!_hasDestinationPoint)
            {
                throw new InvalidOperationException("Method should not be called when destination point is not set");
            }
        }
    }
}
