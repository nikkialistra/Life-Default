using System;
using ResourceManagement;
using UnityEngine;

namespace General.Questing.Objectives
{
    [Serializable]
    public class CollectObjective : IObjective
    {
        [SerializeField] private ResourceType _type;
        [SerializeField] private int _quantity;

        private int _collected;
        
        private ResourceCounts _resourceCounts;

        public event Action<string> Update;
        
        public void Activate(QuestServices questServices)
        {
            _resourceCounts = questServices.ResourceCounts;
            
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
