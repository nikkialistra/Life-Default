using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UI.Game.GameLook.Components;
using UnityEngine;
using Zenject;

namespace ResourceManagement
{
    public class ResourceCounts : MonoBehaviour
    {
        private ResourcesView _resourcesView;

        private readonly Dictionary<ResourceType, int> _resourceCounts = new();

        [Inject]
        public void Construct(ResourcesView resourcesView)
        {
            _resourcesView = resourcesView;
        }

        private void Start()
        {
            FillInCounts();
        }

        [Button(ButtonSizes.Large)]
        public void ChangeResourceTypeCount(ResourceType resourceType, int amount)
        {
            _resourceCounts[resourceType] += amount;

            if (_resourceCounts[resourceType] < 0)
            {
                throw new InvalidOperationException($"{resourceType} cannot be less than zero");
            }

            _resourcesView.ChangeResourceTypeCount(resourceType, _resourceCounts[resourceType]);
        }

        private void FillInCounts()
        {
            _resourceCounts.Add(ResourceType.Wood, 0);
            _resourceCounts.Add(ResourceType.Stone, 0);
        }
    }
}
