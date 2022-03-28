using System;
using Colonists.Colonist;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    public class ColonistIconView
    {
        private const string VisualTreePath = "UI/Markup/GameLook/Components/ColonistIcon";
        
        private readonly ColonistIconsView _parent;
        private readonly TemplateContainer _tree;
        
        private readonly VisualElement _root;
        
        private readonly Label _name;
        
        private readonly VisualElement _outline;
        private readonly VisualElement _picture;
        
        private readonly ProgressBar _vitalityProgress;
        private readonly ProgressBar _bloodProgress;
        
        private ColonistFacade _colonist;

        public ColonistIconView(ColonistIconsView parent)
        {
            _parent = parent;
            
            _tree = Resources.Load<VisualTreeAsset>(VisualTreePath).CloneTree();
            
            _root = _tree.Q<VisualElement>("colonist-icon");

            _name = _tree.Q<Label>("name");

            _outline = _tree.Q<VisualElement>("outline");
            _picture = _tree.Q<VisualElement>("picture");

            _vitalityProgress = _tree.Q<ProgressBar>("vitality-progress");
            _bloodProgress = _tree.Q<ProgressBar>("blood-progress");
        }
        
        public event Action<ColonistFacade> Click;

        public void Bind(ColonistFacade colonist)
        {
            _colonist = colonist;
            
            _parent.ColonistIcons.Add(_tree);
            
            _root.RegisterCallback<MouseDownEvent>(OnMouseDownEvent);
            
            _colonist.HealthChange += UpdateHealth;
            _colonist.Die += Unbind;

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

            _colonist.HealthChange -= UpdateHealth;
            _colonist.Die -= Unbind;
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

        private void FillIn(ColonistFacade colonist)
        {
            _name.text = colonist.Name;
            UpdateHealth();
        }

        private void OnMouseDownEvent(MouseDownEvent _)
        {
            Click?.Invoke(_colonist);
        }

        private void UpdateHealth()
        {
            _vitalityProgress.value = _colonist.Health.Vitality;
            _bloodProgress.value = _colonist.Health.Blood;
        }
    }
}
