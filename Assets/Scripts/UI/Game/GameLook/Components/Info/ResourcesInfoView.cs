using System.Collections.Generic;
using System.Linq;
using ResourceManagement;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components.Info
{
    [RequireComponent(typeof(InfoPanelView))]
    public class ResourcesInfoView : MonoBehaviour
    {
        [Required]
        [SerializeField] private VisualTreeAsset _asset;

        private InfoPanelView _parent;
        private TemplateContainer _tree;
        
        private Label _name;

        private readonly List<VisualElement> _rows = new(2);
        private readonly List<Label> _rowNames = new(2);
        private readonly List<Label> _rowValues = new(2);

        private List<Resource> _resources = new();

        private bool _shown;

        private void Awake()
        {
            _parent = GetComponent<InfoPanelView>();

            _tree = _asset.CloneTree();
            _tree.pickingMode = PickingMode.Ignore;

            _name = _tree.Q<Label>("name");
            
            BindRows();
        }

        private void BindRows()
        {
            _rows.Add(_tree.Q<VisualElement>("row-one"));
            _rows.Add(_tree.Q<VisualElement>("row-two"));

            _rowNames.Add(_tree.Q<Label>("row-one__name"));
            _rowNames.Add(_tree.Q<Label>("row-two__name"));

            _rowValues.Add(_tree.Q<Label>("row-one__value"));
            _rowValues.Add(_tree.Q<Label>("row-two__value"));
        }

        private void OnDestroy()
        {
            UnsubscribeFromResources();
        }
        
        public void ShowSelf()
        {
            if (_shown)
            {
                return;
            }

            _parent.InfoPanel.Add(_tree);
            _shown = true;
        }

        public void HideSelf()
        {
            if (!_shown)
            {
                return;
            }
            
            UnsubscribeFromResources();
            
            _parent.InfoPanel.Remove(_tree);
            _shown = false;
        }
        
        public void FillIn(List<Resource> resources)
        {
            if (resources.Count == 0)
            {
                return;
            }

            UnsubscribeFromResources();

            _resources = resources;
            
            _name.text = $"{resources[0].ResourceType.GetStringForResources()}";

            ShowRows();

            FillRow(0, $"{_resources[0].ResourceType}:", $"~{_resources.Sum(resource => resource.Quantity)}");
            FillRow(1, $"Durability:", $"{_resources.Sum(resource => resource.Durability)}");

            SubscribeToResources();
        }

        private void SubscribeToResources()
        {
            foreach (var resource in _resources)
            {
                resource.QuantityChange += UpdateQuantity;
                resource.DurabilityChange += UpdateDurability;
                
                resource.ResourceDestroying += UnsubscribeFromResource;
            }
        }
        
        private void UnsubscribeFromResources()
        {
            foreach (var resource in _resources)
            {
                UnsubscribeFromResource(resource);
            }
        }

        private void UnsubscribeFromResource(Resource resource)
        {
            resource.QuantityChange -= UpdateQuantity;
            resource.DurabilityChange -= UpdateDurability;
        }

        private void ShowRows()
        {
            _rows[0].style.display = DisplayStyle.Flex;
            _rows[1].style.display = DisplayStyle.Flex;
        }

        private void UpdateQuantity(int _)
        {
            _rowValues[0].text = $"~{_resources.Sum(resource => resource.Quantity)}";
        }

        private void UpdateDurability(int _)
        {
            _rowValues[1].text = $"{_resources.Sum(resource => resource.Durability)}";
        }

        private void FillRow(int index, string name, string value)
        {
            _rowNames[index].text = name;
            _rowValues[index].text = value;
        }
    }
}
