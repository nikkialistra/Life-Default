using System;
using ResourceManagement;
using UnityEngine;
using Zenject;

namespace General.Questing
{
    [CreateAssetMenu(fileName = "Quest", menuName = "Quest/Collect Objective")]
    public class CollectObjective : ScriptableObject, IObjective
    {
        [SerializeField] private ResourceType _type;
        [SerializeField] private int _quantity;

        private int _collected;
        
        private ResourceCounts _resourceCounts;

        [Inject]
        public void Construct(ResourceCounts resourceCounts)
        {
            _resourceCounts = resourceCounts;
        }

        public event Action<string> Update;
        
        public void Activate()
        {
            _resourceCounts.ResourceUpdate += OnResourceUpdate;
        }

        public void Deactivate()
        {
            _resourceCounts.ResourceUpdate -= OnResourceUpdate;
        }

        private void OnResourceUpdate(ResourceType resourceType, int count)
        {
            if (_type != resourceType)
            {
                return;
            }

            _collected = count;
            
            Update?.Invoke(ToText());
        }

        public string ToText()
        {
            return $"Collect {_quantity} {_type}s  –  {_collected}/{_quantity}";
        }
    }
}
