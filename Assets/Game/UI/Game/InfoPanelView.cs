using System;
using System.Collections.Generic;
using Game.Units.Unit;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI.Game
{
    [RequireComponent(typeof(UIDocument))]
    [RequireComponent(typeof(UnitDescriptionView))]
    public class InfoPanelView : MonoBehaviour
    {
        public event Action<UnitFacade> UnitIconClick;

        public VisualElement Root => _infoPanel;
        
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
        
        private VisualElement _tree;
        
        private UnitDescriptionView _unitDescriptionView;

        private VisualElement _infoPanel;
        private VisualElement _infoImage;

        private VisualElement _multipleUnitsDescription;
        private VisualElement _multipleUnitsDescriptionBottom;

        private Label _multipleUnitsCount;

        private List<TemplateContainer> _unitIconComponents;
        private List<VisualElement> _unitIconRoots;
        private List<VisualElement> _unitIconImages;
        private List<VisualElement> _unitIconHealths;
        private List<ProgressBar> _unitIconProgressBars;

        private UnitFacade _unit;
        private List<UnitFacade> _units;

        private void Awake()
        {
            _tree = GetComponent<UIDocument>().rootVisualElement;

            _unitDescriptionView = GetComponent<UnitDescriptionView>();

            _infoPanel = _tree.Q<VisualElement>("info-panel");
            _infoImage = _tree.Q<VisualElement>("info-image");

            GetDescriptionBlocks();
            GetMultipleUnitsBlockElements();

            InitializeUnitIconComponentPool();
        }

        private void Start()
        {
            _infoPanel.AddToClassList("not-displayed");
        }

        private void OnDestroy()
        {
            for (int i = 0; i < _maximumUnitIconsShowing; i++)
            {
                var iconRoot = _unitIconRoots[i];
                iconRoot.UnregisterCallback<MouseDownEvent, UnitFacade>(IconOnMouseDownEvent);
            }
        }

        public void SetUnits(List<UnitFacade> units)
        {
            if (units.Count == 0)
            {
                HideInfoPanel();
            }
            else if (units.Count == 1)
            {
                SetUnit(units[0]);
            }
            else
            {
                SetMultipleUnits(units);
            }
        }

        public void SetUnit(UnitFacade unit)
        {
            _unit = unit;
            ShowInfoPanel();
            ShowOneUnitDescription();
        }

        private void GetDescriptionBlocks()
        {
            _multipleUnitsDescription = _tree.Q<VisualElement>("multiple-units-description");
            _multipleUnitsDescriptionBottom = _tree.Q<VisualElement>("multiple-units-description__bottom");
        }

        private void GetMultipleUnitsBlockElements()
        {
            _multipleUnitsCount = _tree.Q<Label>("multiple-units-nomination__count");
        }

        private void InitializeUnitIconComponentPool()
        {
            _unitIconComponents = new List<TemplateContainer>(_maximumUnitIconsShowing);
            _unitIconRoots = new List<VisualElement>(_maximumUnitIconsShowing);
            _unitIconImages = new List<VisualElement>(_maximumUnitIconsShowing);
            _unitIconHealths = new List<VisualElement>(_maximumUnitIconsShowing);
            _unitIconProgressBars = new List<ProgressBar>(_maximumUnitIconsShowing);

            var unitIconComponent = Resources.Load<VisualTreeAsset>("UI/Markup/Components/UnitIcon");
            for (var i = 0; i < _maximumUnitIconsShowing; i++)
            {
                var tree = unitIconComponent.CloneTree();
                _unitIconComponents.Add(tree);
                _unitIconRoots.Add(tree.Q<VisualElement>("icon"));
                _unitIconImages.Add(tree.Q<VisualElement>("icon__image"));
                _unitIconHealths.Add(tree.Q<VisualElement>("icon__health"));
                _unitIconProgressBars.Add(tree.Q<ProgressBar>("multiple-units-health__progress-bar"));
            }
        }

        private void SetMultipleUnits(List<UnitFacade> units)
        {
            _units = units;
            ShowInfoPanel();
            ShowMultipleUnitsDescription();
            FillInMultipleUnitsDescription();
        }

        private void ShowInfoPanel()
        {
            _infoPanel.RemoveFromClassList("not-displayed");
        }

        private void HideInfoPanel()
        {
            _infoPanel.AddToClassList("not-displayed");
        }

        private void ShowOneUnitDescription()
        {
            _unitDescriptionView.ShowSelf();
            _unitDescriptionView.FillIn(_unit);
        }

        private void ShowMultipleUnitsDescription()
        {
            _multipleUnitsDescription.RemoveFromClassList("not-displayed");
            _unitDescriptionView.HideSelf();
        }
        
        private void FillInMultipleUnitsDescription()
        {
            FillInUnitsPreview();
            FillInUnitsProperties();
        }
        
        private void FillInUnitsPreview()
        {
            _infoImage.style.backgroundImage = new StyleBackground(_multipleUnitsPreview);
        }

        private void FillInUnitsProperties()
        {
            _multipleUnitsCount.text = $"Units ({_units.Count})";
            
            _units.Sort((x,y) =>
                x.UnitType.CompareTo(y.UnitType));
            
            ShowUnitsIcons();
        }

        private void ShowUnitsIcons()
        {
            _multipleUnitsDescriptionBottom.Clear();

            var iconsToShow = _units.Count <= _maximumUnitIconsShowing ? _units.Count : _maximumUnitIconsShowing;

            for (var i = 0; i < iconsToShow; i++)
            {
                var unit = _units[i];
                var icon = _unitIconComponents[i];

                SetUpIcon(icon);
                SetUpIconClickEvent(unit, i);
                SetUpIconImage(unit, i);
                SetUpIconHealth(unit, i);
            }
        }

        private void SetUpIcon(TemplateContainer icon)
        {
            _multipleUnitsDescriptionBottom.Add(icon);
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