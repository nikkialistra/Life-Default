using System;
using System.Collections.Generic;
using ResourceManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game
{
    public class ResourcesView : MonoBehaviour
    {
        private readonly Dictionary<ResourceType, Label> _resourceLabels = new();

        private void Awake()
        {
            Tree = Resources.Load<VisualTreeAsset>("UI/Markup/GameLook/Components/Resources").CloneTree();
        }

        public VisualElement Tree { get; private set; }

        private void Start()
        {
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
            _resourceLabels.Add(ResourceType.Wood, Tree.Q<Label>("wood__count"));
            _resourceLabels.Add(ResourceType.Stone, Tree.Q<Label>("stone__count"));
            _resourceLabels.Add(ResourceType.Emerald, Tree.Q<Label>("emerald__count"));
            _resourceLabels.Add(ResourceType.Crystal, Tree.Q<Label>("crystal__count"));
        }
    }
}
