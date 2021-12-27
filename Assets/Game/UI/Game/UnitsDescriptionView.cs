using System;
using System.Collections.Generic;
using Game.Units.Unit;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI.Game
{
    [RequireComponent(typeof(InfoPanelView))]
    public class UnitsDescriptionView : MonoBehaviour
    {
        public event Action<UnitFacade> UnitIconClick;
        
        [Title("Previews")]
        [Required]
        [SerializeField] private Texture2D _multipleUnitsPreview;

        [Title("Icons")]
        [Required]
        [SerializeField] private Texture2D _travelerIcon;
        [Required]
        [SerializeField] private Texture2D _lumberjackIcon;
        [Required]
        [SerializeField] private Texture2D _masonIcon;
        [Required]
        [SerializeField] private Texture2D _meleeIcon;

        [Required]
        [SerializeField] private Texture2D _archerIcon;

        [Title("Other")] 
        [SerializeField] private int _maximumUnitIconsShowing;

        private InfoPanelView _parent;
        private TemplateContainer _tree;
        
        private VisualElement _image;
        
        private VisualElement _descriptionBottom;
        private Label _count;
        
        private List<TemplateContainer> _unitIconComponents;
        private List<VisualElement> _unitIconRoots;
        private List<VisualElement> _unitIconImages;
        private List<ProgressBar> _unitIconProgressBars;
        
        private void Awake()
        {
            _parent = GetComponent<InfoPanelView>();
            
            _tree = Resources.Load<VisualTreeAsset>("UI/Markup/Components/UnitsInfo").CloneTree();

            _image = _tree.Q<VisualElement>("info-image");
            
            _descriptionBottom = _tree.Q<VisualElement>("units-description__bottom");
            
            _count = _tree.Q<Label>("units-nomination-count");
            
            InitializeUnitIconComponentPool();
        }
        
        private void OnDestroy()
        {
            for (int i = 0; i < _maximumUnitIconsShowing; i++)
            {
                var iconRoot = _unitIconRoots[i];
                iconRoot.UnregisterCallback<MouseDownEvent, UnitFacade>(IconOnMouseDownEvent);
            }
        }
        
        private void InitializeUnitIconComponentPool()
        {
            _unitIconComponents = new List<TemplateContainer>(_maximumUnitIconsShowing);
            _unitIconRoots = new List<VisualElement>(_maximumUnitIconsShowing);
            _unitIconImages = new List<VisualElement>(_maximumUnitIconsShowing);
            _unitIconProgressBars = new List<ProgressBar>(_maximumUnitIconsShowing);

            var unitIconComponent = Resources.Load<VisualTreeAsset>("UI/Markup/Components/UnitIcon");
            for (var i = 0; i < _maximumUnitIconsShowing; i++)
            {
                var tree = unitIconComponent.CloneTree();
                _unitIconComponents.Add(tree);
                _unitIconRoots.Add(tree.Q<VisualElement>("icon"));
                _unitIconImages.Add(tree.Q<VisualElement>("image"));
                _unitIconProgressBars.Add(tree.Q<ProgressBar>("health__progress-bar"));
            }
        }

        public void ShowSelf()
        {
            _parent.Root.Add(_tree);
        }

        public void HideSelf()
        {
            if (_parent.Root.Contains(_tree))
            {
                _parent.Root.Remove(_tree);
            }
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
            _count.text = $"Units ({units.Count})";
            
            units.Sort((x,y) =>
                x.UnitType.CompareTo(y.UnitType));
            
            ShowUnitsIcons(units);
        }

        private void ShowUnitsIcons(List<UnitFacade> units)
        {
            _descriptionBottom.Clear();

            var iconsToShow = units.Count <= _maximumUnitIconsShowing ? units.Count : _maximumUnitIconsShowing;

            for (var i = 0; i < iconsToShow; i++)
            {
                var unit = units[i];
                var icon = _unitIconComponents[i];

                SetUpIcon(icon);
                SetUpIconClickEvent(unit, i);
                SetUpIconImage(unit, i);
                SetUpIconHealth(unit, i);
            }
        }

        private void SetUpIcon(TemplateContainer icon)
        {
            _descriptionBottom.Add(icon);
        }

        private void SetUpIconClickEvent(UnitFacade unit, int index)
        {
            var iconRoot = _unitIconRoots[index];
            iconRoot.UnregisterCallback<MouseDownEvent, UnitFacade>(IconOnMouseDownEvent);
            iconRoot.RegisterCallback<MouseDownEvent, UnitFacade>(IconOnMouseDownEvent, unit);
        }

        private void SetUpIconImage(UnitFacade unit, int index)
        {
            var iconImage = _unitIconImages[index];
            iconImage.style.backgroundImage = unit.UnitType switch
            {
                UnitType.Traveler => new StyleBackground(_travelerIcon),
                UnitType.Lumberjack => new StyleBackground(_lumberjackIcon),
                UnitType.Mason => new StyleBackground(_masonIcon),
                UnitType.Melee => new StyleBackground(_meleeIcon),
                UnitType.Archer => new StyleBackground(_archerIcon),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void SetUpIconHealth(UnitFacade unit, int index)
        {
            var iconHealth = _unitIconProgressBars[index];
            iconHealth.value = unit.Health;
        }

        private void IconOnMouseDownEvent(MouseDownEvent mouseDownEvent, UnitFacade unit)
        {
            UnitIconClick?.Invoke(unit);
        }
    }
}