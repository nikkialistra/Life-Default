using System;
using System.Collections.Generic;
using ResourceManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game
{
    [RequireComponent(typeof(UIDocument))]
    public class ResourcesView : MonoBehaviour
    {
        private VisualElement _tree;

        private readonly Dictionary<ResourceType, Label> _resourceLabels = new();

        private void Awake()
        {
            _tree = GetComponent<UIDocument>().rootVisualElement;

            FillInLabels();
        }

        public void ChangeResourceTypeCount(ResourceType resourceType, int amount)
        {
            CheckResourceTypeExistence(resourceType);

            var label = _resourceLabels[resourceType];
            label.text = $"{amount}";
        }

        private void CheckResourceTypeExistence(ResourceType resourceType)
        {
            if (!_resourceLabels.ContainsKey(resourceType))
            {
                throw new ArgumentException($"Dictionary doesn't contain key {resourceType}");
            }
        }

        private void FillInLabels()
        {
            _resourceLabels.Add(ResourceType.Wood, _tree.Q<Label>("wood__count"));
            _resourceLabels.Add(ResourceType.Stone, _tree.Q<Label>("stone__count"));
            _resourceLabels.Add(ResourceType.Emerald, _tree.Q<Label>("emerald__count"));
            _resourceLabels.Add(ResourceType.Crystal, _tree.Q<Label>("crystal__count"));
        }
    }
}
