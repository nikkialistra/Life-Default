using System;
using System.Collections.Generic;
using Common;
using Sirenix.OdinInspector;
using Units.Unit;
using Units.Unit.UnitTypes;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game
{
    [RequireComponent(typeof(InfoPanelView))]
    [RequireComponent(typeof(ChangeColorFractions))]
    public class UnitsInfoView : MonoBehaviour
    {
        [Required]
        [SerializeField] private Texture2D _multipleUnitsPreview;

        [MinValue(0)]
        [SerializeField] private int _maximumUnitIconsShowing;
        
        [ValidateInput("@_iconPreviews.Count == 5", "Preview dictionary should have 5 elements for all 5 unit types.")]
        [SerializeField] private IconPreviewsDictionary _iconPreviews;
        
        [Serializable] public class IconPreviewsDictionary : SerializableDictionary<UnitType, Texture2D> {}
        
        public event Action<UnitFacade> SelectUnit; 
        
        public VisualElement IconContainer { get; private set; }

        private int _count;
        
        private ChangeColorFractions _changeColorFractions;

        private InfoPanelView _parent;
        private TemplateContainer _tree;

        private VisualElement _image;
        private Label _unitCount;

        private readonly List<UnitIconView> _unitIconViews = new();

        private void Awake()
        {
            _parent = GetComponent<InfoPanelView>();
            _changeColorFractions = GetComponent<ChangeColorFractions>();
            
            _tree = Resources.Load<VisualTreeAsset>("UI/Markup/Components/UnitsInfo").CloneTree();

            _image = _tree.Q<VisualElement>("info-image");
            _unitCount = _tree.Q<Label>("units-description__nomination__count");
            IconContainer = _tree.Q<VisualElement>("units-description__icon-container");

            InitializeUnitIconViews();
        }
        
        private void OnDestroy()
        {
            foreach (var unitIconView in _unitIconViews)
            {
                unitIconView.Click -= OnUnitIconClick;
                unitIconView.Remove -= OnUnitIconRemove;
                
                unitIconView.Dispose();
            }
        }
        
        private void InitializeUnitIconViews()
        {
            for (var i = 0; i < _maximumUnitIconsShowing; i++)
            {
                var unitIconView = new UnitIconView(this, _iconPreviews, _changeColorFractions);
                unitIconView.Click += OnUnitIconClick;
                unitIconView.Remove += OnUnitIconRemove;
                
                _unitIconViews.Add(unitIconView);
            }
        }

        private void OnUnitIconRemove()
        {
            _count--;
            UpdateCountText();
        }

        private void OnUnitIconClick(UnitFacade unit)
        {
            SelectUnit?.Invoke(unit);
        }

        public void ShowSelf()
        {
            _parent.Info.Add(_tree);
        }

        public void HideSelf()
        {
            if (_parent.Info.Contains(_tree))
            {
                _parent.Info.Remove(_tree);
            }
        }
        
        private void HidePanel()
        {
            _parent.HideSelf();
        }

        public void FillIn(List<UnitFacade> units)
        {
            FillInPreview();
            FillInProperties(units);
        }

        private void FillInPreview()
        {
            _image.style.backgroundImage = new StyleBackground(_multipleUnitsPreview);
        }

        private void FillInProperties(List<UnitFacade> units)
        {
            _count = units.Count;
            UpdateCountText();
            
            units.Sort((x,y) =>
                x.UnitType.CompareTo(y.UnitType));
            
            ShowUnitIcons(units);
        }

        private void UpdateCountText()
        {
            if (_count == 0)
            {
                HidePanel();
            }
            
            _unitCount.text = $"Units ({_count})";
        }

        private void ShowUnitIcons(List<UnitFacade> units)
        {
            foreach (var unitIconView in _unitIconViews)
            {
                unitIconView.Unbind();
            }

            var iconsToShow = units.Count <= _maximumUnitIconsShowing ? units.Count : _maximumUnitIconsShowing;

            for (var i = 0; i < iconsToShow; i++)
            {
                _unitIconViews[i].Bind(units[i]);
            }
        }
    }
}