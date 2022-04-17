using System.Collections.Generic;
using ResourceManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    [RequireComponent(typeof(InfoPanelView))]
    public class EntityInfoView : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset _asset;

        private InfoPanelView _parent;
        private TemplateContainer _tree;
        
        private Label _name;

        private readonly List<Label> _rowNames = new(3);
        private readonly List<Label> _rowValues = new(3);
        
        private Resource _resource;

        private bool _shown;

        private void Awake()
        {
            _parent = GetComponent<InfoPanelView>();

            _tree = _asset.CloneTree();
            _tree.pickingMode = PickingMode.Ignore;

            _name = _tree.Q<Label>("name");
            
            _rowNames.Add(_tree.Q<Label>("row-one__name"));
            _rowNames.Add(_tree.Q<Label>("row-two__name"));
            _rowNames.Add(_tree.Q<Label>("row-three__name"));
            
            _rowValues.Add(_tree.Q<Label>("row-one__value"));
            _rowValues.Add(_tree.Q<Label>("row-two__value"));
            _rowValues.Add(_tree.Q<Label>("row-three__value"));
        }
        
        private void OnDestroy()
        {
            UnsubscribeFromResource();
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
            
            UnsubscribeFromResource();
            
            _parent.InfoPanel.Remove(_tree);
            _shown = false;
        }
        
        public void FillIn(Resource resource)
        {
            UnsubscribeFromResource();
            
            _resource = resource;
            
            _name.text = $"{resource.Name}";

            FillRow(0, $"{resource.ResourceType}:", $"~{resource.Quantity}");
            FillRow(1, $"Durability:", $"{resource.Durability}");

            SubscribeToResource();
        }

        private void SubscribeToResource()
        {
            _resource.QuantityChange += UpdateFirstRow;
            _resource.DurabilityChange += UpdateSecondRow;
        }
        
        private void UnsubscribeFromResource()
        {
            if (_resource != null)
            {
                _resource.QuantityChange -= UpdateFirstRow;
                _resource.DurabilityChange -= UpdateSecondRow;
            }
        }

        private void UpdateFirstRow()
        {
            _rowValues[0].text = $"~{_resource.Quantity}";
        }

        private void UpdateSecondRow()
        {
            _rowValues[1].text = $"{_resource.Durability}";
        }

        private void FillRow(int index, string name, string value)
        {
            _rowNames[index].text = name;
            _rowValues[index].text = value;
        }
    }
}
