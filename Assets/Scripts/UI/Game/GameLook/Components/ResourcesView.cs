using System;
using System.Collections.Generic;
using ResourceManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    [RequireComponent(typeof(StockView))]
    public class ResourcesView : MonoBehaviour
    {
        private const string VisualTreePath = "UI/Markup/GameLook/Components/Resources";
        
        private readonly Dictionary<ResourceType, Label> _resourceLabels = new();

        private StockView _parent;

        private void Awake()
        {
            _parent = GetComponent<StockView>();
            
            Tree = Resources.Load<VisualTreeAsset>(VisualTreePath).CloneTree();
        }
        
        public bool Shown { get; private set; }
        public VisualElement Tree { get; private set; }

        private void Start()
        {
            FillInLabels();
        }
        
        public void ShowSelf()
        {
            if (Shown)
            {
                return;
            }

            _parent.Content.Add(Tree);
            Shown = true;
        }

        public void HideSelf()
        {
            if (!Shown)
            {
                return;
            }
            
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
            {
                throw new ArgumentException($"Dictionary doesn't contain key {resourceType}");
            }
        }

        private void FillInLabels()
        {
            _resourceLabels.Add(ResourceType.Wood, Tree.Q<Label>("wood__count"));
            _resourceLabels.Add(ResourceType.Stone, Tree.Q<Label>("stone__count"));
        }
    }
}
