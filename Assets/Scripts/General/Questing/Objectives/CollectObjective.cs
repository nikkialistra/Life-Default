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
        public event Action<string> Complete;
        
        public void Activate(QuestServices questServices)
        {
            _collected = 0;

            _resourceCounts = questServices.ResourceCounts;
            
            _resourceCounts.ResourceUpdate += OnResourceUpdate;
        }

        public void Deactivate()
        {
            _resourceCounts.ResourceUpdate -= OnResourceUpdate;
        }

        public string ToText()
        {
            return $"Collect {_quantity} {_type.GetStringForMultiple()}  –  {_collected}/{_quantity}";
        }

        private void OnResourceUpdate(ResourceType resourceType, int count)
        {
            if (_type != resourceType)
            {
                return;
            }

            _collected = count;

            CheckForCompletion();
            
            Update?.Invoke(ToText());
        }

        private void CheckForCompletion()
        {
            if (_collected >= _quantity)
            {
                Deactivate();
                Complete?.Invoke($"<s>{ToText()}</s>");
            }
        }
    }
}
