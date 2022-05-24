using System;
using Colonists;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    public class ColonistIconView
    {
        private readonly ColonistIconsView _parent;
        private readonly TemplateContainer _tree;
        
        private readonly VisualElement _root;
        
        private readonly Label _name;
        
        private readonly VisualElement _outline;
        private readonly VisualElement _picture;
        
        private readonly ProgressBar _healthProgress;
        private readonly ProgressBar _recoverySpeedProgress;
        
        private Colonist _colonist;

        public ColonistIconView(ColonistIconsView parent, VisualTreeAsset asset)
        {
            _parent = parent;
            
            _tree = asset.CloneTree();
            
            _root = _tree.Q<VisualElement>("colonist-icon");

            _name = _tree.Q<Label>("name");

            _outline = _tree.Q<VisualElement>("outline");
            _picture = _tree.Q<VisualElement>("picture");

            _healthProgress = _tree.Q<ProgressBar>("health-progress");
            _recoverySpeedProgress = _tree.Q<ProgressBar>("recovery-speed-progress");
        }
        
        public event Action<Colonist> Click;

        public Vector2 Center => _root.LocalToWorld(_root.layout.center);

        public void Bind(Colonist colonist)
        {
            _colonist = colonist;

            _parent.ColonistIcons.Add(_tree);
            
            _root.RegisterCallback<MouseDownEvent>(OnMouseDownEvent);
            
            _colonist.VitalityChange += UpdateVitality;
            _colonist.NameChange += UpdateName;

            FillIn(colonist);
        }

        public void Unbind()
        {
            _parent.ColonistIcons.Remove(_tree);
            
            _root.UnregisterCallback<MouseDownEvent>(OnMouseDownEvent);
            
            if (_colonist == null)
            {
                return;
            }

            _colonist.VitalityChange -= UpdateVitality;
            _colonist.VitalityChange -= UpdateVitality;
            _colonist = null;
        }

        public void ShowOutline()
        {
            _outline.AddToClassList("show-outline");
        }

        public void HideOutline()
        {
            _outline.RemoveFromClassList("show-outline");
        }

        private void FillIn(Colonist colonist)
        {
            _healthProgress.highValue = _colonist.Unit.UnitVitality.MaxHealth;

            _recoverySpeedProgress.lowValue = -_colonist.Unit.UnitVitality.MaxRecoverySpeed;
            _recoverySpeedProgress.highValue = _colonist.Unit.UnitVitality.MaxRecoverySpeed;

            _name.text = colonist.Name;
            UpdateVitality();
        }

        private void OnMouseDownEvent(MouseDownEvent _)
        {
            Click?.Invoke(_colonist);
        }

        private void UpdateVitality()
        {
            _healthProgress.highValue = _colonist.Unit.UnitVitality.MaxHealth;
            
            _recoverySpeedProgress.highValue = _colonist.Unit.UnitVitality.MaxRecoverySpeed;
            _recoverySpeedProgress.lowValue = -_colonist.Unit.UnitVitality.MaxRecoverySpeed;
            
            _healthProgress.value = _colonist.Unit.UnitVitality.Health;
            _recoverySpeedProgress.value = _colonist.Unit.UnitVitality.RecoverySpeed;
        }

        private void UpdateName(string name)
        {
            _name.text = name;
        }
    }
}
