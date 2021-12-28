using System;
using System.Collections.Generic;
using Game.Units.Unit;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI.Game
{
    [RequireComponent(typeof(InfoPanelView))]
    public class UnitsInfoView : MonoBehaviour
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
        
        [Title("Health Changing Color Fractions")] 
        [Range(0, 1)]
        [SerializeField] private float _middleFraction;
        [Range(0, 1)]
        [SerializeField] private float _lowFraction;

        [Title("Other")] 
        [SerializeField] private int _maximumUnitIconsShowing;

        private int _count;
        
        private InfoPanelView _parent;
        private TemplateContainer _tree;

        private VisualElement _image;

        private VisualElement _descriptionBottom;
        private Label _unitCount;

        private List<UnitFacade> _lastUnits;

        private List<TemplateContainer> _unitIconComponents;
        private List<VisualElement> _unitIconRoots;
        private List<VisualElement> _unitIconImages;
        private List<ProgressBar> _unitIconProgressBars;

        private List<Action<int>> _changeHealthAtIndexActions;
        private List<Action> _removeUnitAtIndexActions;

        private void Awake()
        {
            _parent = GetComponent<InfoPanelView>();
            
            _tree = Resources.Load<VisualTreeAsset>("UI/Markup/Components/UnitsInfo").CloneTree();

            _image = _tree.Q<VisualElement>("info-image");
            
            _descriptionBottom = _tree.Q<VisualElement>("units-description__bottom");
            
            _unitCount = _tree.Q<Label>("units-nomination-count");
            
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
            _lastUnits = new List<UnitFacade>(_maximumUnitIconsShowing);
            InitializeChangeHealthActions();
            InitializeRemoveUnitActions();

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

        private void InitializeChangeHealthActions()
        {
            _changeHealthAtIndexActions = new List<Action<int>>();
            
            for (var i = 0; i < _maximumUnitIconsShowing; i++)
            {
                var index = i;
                var action = new Action<int>(delegate(int health)
                {
                    ChangeHealth(_unitIconProgressBars[index], health, index);
                });
                _changeHealthAtIndexActions.Add(action);
            }
        }

        private void InitializeRemoveUnitActions()
        {
            _removeUnitAtIndexActions = new List<Action>();

            for (var i = 0; i < _maximumUnitIconsShowing; i++)
            {
                var index = i;
                var action = new Action(delegate
                {
                    RemoveUnit(index);
                });
                _removeUnitAtIndexActions.Add(action);
            }
        }

        private void RemoveUnit(int index)
        {
            _count--;
            UpdateCountText();
            _unitIconRoots[index].AddToClassList("not-displayed");
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
            UnsubscribeFromUnits();
            
            _descriptionBottom.Clear();

            var iconsToShow = units.Count <= _maximumUnitIconsShowing ? units.Count : _maximumUnitIconsShowing;

            for (var i = 0; i < iconsToShow; i++)
            {
                var unit = units[i];
                var icon = _unitIconComponents[i];
                
                SubscribeToUnit(unit, i);

                SetUpIcon(icon);
                SetUpIconClickEvent(unit, i);
                SetUpIconImage(unit, i);
                SetUpIconHealth(unit, i);
            }
        }

        private void SubscribeToUnit(UnitFacade unit, int i)
        {
            unit.HealthChange += _changeHealthAtIndexActions[i];
            unit.Die += _removeUnitAtIndexActions[i];
            
            if (_lastUnits.Count > i)
            {
                _lastUnits[i] = unit;
            }
            else
            {
                _lastUnits.Add(unit);
            }
        }

        private void UnsubscribeFromUnits()
        {
            for (var i = 0; i < _lastUnits.Count; i++)
            {
                _lastUnits[i].HealthChange -= _changeHealthAtIndexActions[i];
                _lastUnits[i].Die -= _removeUnitAtIndexActions[i];
            }
        }

        private void SetUpIcon(TemplateContainer icon)
        {
            _descriptionBottom.Add(icon);
        }

        private void SetUpIconClickEvent(UnitFacade unit, int index)
        {
            var iconRoot = _unitIconRoots[index];
            iconRoot.RemoveFromClassList("not-displayed");
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
            ChangeHealth(iconHealth, unit.Health, index);
        }
        
        private void ChangeHealth(ProgressBar iconHealth, int health, int index)
        {
            iconHealth.value = (float) health / _lastUnits[index].MaxHealth;

            SetHealthColor(iconHealth);
        }

        private void SetHealthColor(ProgressBar iconHealth)
        {
            var fraction = iconHealth.value;
            if (fraction > _middleFraction)
            {
                iconHealth.RemoveFromClassList("middle-health");
                iconHealth.RemoveFromClassList("low-health");
            }
            else if (fraction > _lowFraction)
            {
                iconHealth.AddToClassList("middle-health");
                iconHealth.RemoveFromClassList("low-health");
            }
            else
            {
                iconHealth.RemoveFromClassList("middle-health");
                iconHealth.AddToClassList("low-health");
            }

        } 

        private void IconOnMouseDownEvent(MouseDownEvent mouseDownEvent, UnitFacade unit)
        {
            UnitIconClick?.Invoke(unit);
        }
    }
}