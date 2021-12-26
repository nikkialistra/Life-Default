using System;
using System.Collections.Generic;
using Game.Units.Unit;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI.Game
{
    [RequireComponent(typeof(UIDocument))]
    public class InfoPanelView : MonoBehaviour
    {
        [Title("Previews")]
        [Required]
        [SerializeField] private Texture2D _travelerPreview;
        [Required]
        [SerializeField] private Texture2D _lumberjackPreview;
        [Required]
        [SerializeField] private Texture2D _masonPreview;
        [Required]
        [SerializeField] private Texture2D _meleePreview;
        [Required]
        [SerializeField] private Texture2D _archerPreview;
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

        private VisualElement _infoPanel;
        private VisualElement _infoImage;

        private VisualElement _oneUnitDescription;
        
        private VisualElement _multipleUnitsDescription;
        private VisualElement _multipleUnitsDescriptionBottom;

        private Label _oneUnitType;
        private Label _oneUnitName;
        private ProgressBar _oneUnitHealth;

        private Label _multipleUnitsCount;
        
        private List<TemplateContainer> _unitIconComponents;

        private UnitFacade _unit;
        private List<UnitFacade> _units;

        private void Awake()
        {
            _tree = GetComponent<UIDocument>().rootVisualElement;

            _infoPanel = _tree.Q<VisualElement>("info-panel");
            _infoImage = _tree.Q<VisualElement>("info-image");

            GetDescriptionBlocks();
            GetOneUnitBlockElements();
            GetMultipleUnitsBlockElements();

            InitializeUnitIconComponentPool();
        }

        private void Start()
        {
            _infoPanel.AddToClassList("not-displayed");
            _oneUnitDescription.AddToClassList("not-displayed");
            _multipleUnitsDescription.AddToClassList("not-displayed");
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
            FillInUnitDescription();
        }

        private void GetDescriptionBlocks()
        {
            _oneUnitDescription = _tree.Q<VisualElement>("one-unit-description");
            _multipleUnitsDescription = _tree.Q<VisualElement>("multiple-units-description");
            _multipleUnitsDescriptionBottom = _tree.Q<VisualElement>("multiple-units-description__bottom");
        }

        private void GetOneUnitBlockElements()
        {
            _oneUnitType = _tree.Q<Label>("one-unit-nomination__type");
            _oneUnitName = _tree.Q<Label>("one-unit-nomination__name");
            _oneUnitHealth = _tree.Q<ProgressBar>("one-unit-health__progress-bar");
        }

        private void GetMultipleUnitsBlockElements()
        {
            _multipleUnitsCount = _tree.Q<Label>("multiple-units-nomination__count");
        }

        private void InitializeUnitIconComponentPool()
        {
            _unitIconComponents = new List<TemplateContainer>(_maximumUnitIconsShowing);

            var unitIconComponent = Resources.Load<VisualTreeAsset>("UI/Markup/Components/UnitIcon");
            for (var i = 0; i < _maximumUnitIconsShowing; i++)
            {
                _unitIconComponents.Add(unitIconComponent.CloneTree());
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
            _oneUnitDescription.RemoveFromClassList("not-displayed");
            _multipleUnitsDescription.AddToClassList("not-displayed");
        }

        private void ShowMultipleUnitsDescription()
        {
            _multipleUnitsDescription.RemoveFromClassList("not-displayed");
            _oneUnitDescription.AddToClassList("not-displayed");
        }

        private void FillInUnitDescription()
        {
            FillInUnitPreview();
            FillInUnitProperties();
        }

        private void FillInMultipleUnitsDescription()
        {
            FillInUnitsPreview();
            FillInUnitsProperties();
        }

        private void FillInUnitPreview()
        {
            _infoImage.style.backgroundImage = _unit.UnitType switch
            {
                UnitType.Traveler => new StyleBackground(_travelerPreview),
                UnitType.Lumberjack => new StyleBackground(_lumberjackPreview),
                UnitType.Mason => new StyleBackground(_masonPreview),
                UnitType.Melee => new StyleBackground(_meleePreview),
                UnitType.Archer => new StyleBackground(_archerPreview),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void FillInUnitsPreview()
        {
            _infoImage.style.backgroundImage = new StyleBackground(_multipleUnitsPreview);
        }

        private void FillInUnitProperties()
        {
            _oneUnitType.text = _unit.UnitType.ToString();
            _oneUnitName.text = _unit.Name;
            _oneUnitHealth.value = _unit.Health;
        }

        private void FillInUnitsProperties()
        {
            _multipleUnitsCount.text = $"Units ({_units.Count})";
            
            _units.Sort((x,y) =>
                x.UnitType.CompareTo(y.UnitType));
            
            _multipleUnitsDescriptionBottom.Clear();

            for (var i = 0; i < _units.Count; i++)
            {
                var unit = _units[i];
                var icon = _unitIconComponents[i];
                
                SetUpIcon(icon, unit);
            }
        }

        private void SetUpIcon(TemplateContainer icon, UnitFacade unit)
        {
            _multipleUnitsDescriptionBottom.Add(icon);
            
            var iconImage = icon.Q<VisualElement>("icon__image");
            iconImage.style.backgroundImage = unit.UnitType switch
            {
                UnitType.Traveler => new StyleBackground(_travelerIcon),
                UnitType.Lumberjack => new StyleBackground(_lumberjackIcon),
                UnitType.Mason => new StyleBackground(_masonIcon),
                UnitType.Melee => new StyleBackground(_meleeIcon),
                UnitType.Archer => new StyleBackground(_archerIcon),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            var iconHealth = icon.Q<ProgressBar>("multiple-units-health__progress-bar");
            iconHealth.value = unit.Health;
        }
    }
}