using System;
using System.Collections.Generic;
using ResourceManagement;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components.Stock
{
    [RequireComponent(typeof(StockView))]
    public class ResourcesView : MonoBehaviour
    {
        public bool Shown { get; private set; }

        [Required]
        [SerializeField] private VisualTreeAsset _asset;

        private readonly Dictionary<ResourceType, Label> _resourceLabels = new();

        private StockView _parent;

        private void Awake()
        {
            _parent = GetComponent<StockView>();

            Tree = _asset.CloneTree();

            BindElements();
        }

        private VisualElement Tree { get; set; }

        public void ShowSelf()
        {
            if (Shown) return;

            _parent.Content.Add(Tree);
            Shown = true;
        }

        public void HideSelf()
        {
            if (!Shown) return;

            _parent.Content.Remove(Tree);
            Shown = false;
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
                throw new ArgumentException($"Dictionary doesn't contain key {resourceType}");
        }

        private void BindElements()
        {
            _resourceLabels.Add(ResourceType.Wood, Tree.Q<Label>("wood__count"));
            _resourceLabels.Add(ResourceType.Stone, Tree.Q<Label>("stone__count"));
        }
    }
}
