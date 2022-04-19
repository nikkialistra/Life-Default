using System;
using UnityEngine;

namespace ResourceManagement
{
    [RequireComponent(typeof(Rigidbody))]
    public class ResourceChunk : MonoBehaviour
    {
        private Rigidbody _ridigbody;
        
        public ResourceType ResourceType { get; private set; }
        public int Quantity { get; private set; }

        private void Awake()
        {
            _ridigbody = GetComponent<Rigidbody>();
        }

        public void Initialize(ResourceType resourceType, int quantity)
        {
            ResourceType = resourceType;
            Quantity = quantity;
        }

        public void BurstOutTo(Vector3 randomForce)
        {
            _ridigbody.velocity = randomForce;
        }
    }
}
