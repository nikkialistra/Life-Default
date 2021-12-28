using System;
using Game.Units.Unit;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI.Game
{
    [RequireComponent(typeof(InfoPanelView))]
    public class UnitInfoView : MonoBehaviour
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

        [Title("Health Changing Color Fractions")] 
        [Range(0, 1)]
        [SerializeField] private float _middleFraction;
        [Range(0, 1)]
        [SerializeField] private float _lowFraction;

        private UnitFacade _lastUnit;
        
        private InfoPanelView _parent;
        private TemplateContainer _tree;

        private VisualElement _image;

        private Label _nominationType;
        private Label _nominationName;
        private ProgressBar _health;

        private void Awake()
        {
            _parent = GetComponent<InfoPanelView>();
                
            _tree = Resources.Load<VisualTreeAsset>("UI/Markup/Components/UnitInfo").CloneTree();

            _image = _tree.Q<VisualElement>("info-image");
            
            _nominationType = _tree.Q<Label>("unit-nomination__type");
            _nominationName = _tree.Q<Label>("unit-nomination__name");
            _health = _tree.Q<ProgressBar>("unit-health__progress-bar");
        }

        private void OnDestroy()
        {
            UnsubscribeFromLastUnit();
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

        public void FillIn(UnitFacade unit)
        {
            FillInPreview(unit);
            FillInProperties(unit);
        }

        private void FillInPreview(UnitFacade unit)
        {
            _image.style.backgroundImage = unit.UnitType switch
            {
                UnitType.Traveler => new StyleBackground(_travelerPreview),
                UnitType.Lumberjack => new StyleBackground(_lumberjackPreview),
                UnitType.Mason => new StyleBackground(_masonPreview),
                UnitType.Melee => new StyleBackground(_meleePreview),
                UnitType.Archer => new StyleBackground(_archerPreview),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void FillInProperties(UnitFacade unit)
        {
            UnsubscribeFromLastUnit();
            _lastUnit = unit;
            
            _nominationType.text = unit.UnitType.ToString();
            _nominationName.text = unit.Name;
            
            ChangeHealth(unit.Health);

            SubscribeToUnit(unit);
        }

        private void SubscribeToUnit(UnitFacade unit)
        {
            unit.HealthChange += ChangeHealth;
        }

        private void UnsubscribeFromLastUnit()
        {
            if (_lastUnit != null)
            {
                _lastUnit.HealthChange -= ChangeHealth;
            }
        }

        private void ChangeHealth(int value)
        {
            _health.value = (float) value / _lastUnit.MaxHealth;

            SetHealthColor();
        }

        private void SetHealthColor()
        {
            var fraction = _health.value;
            if (fraction > _middleFraction)
            {
                _health.RemoveFromClassList("middle-health");
                _health.RemoveFromClassList("low-health");
            }
            else if (fraction > _lowFraction)
            {
                _health.AddToClassList("middle-health");
                _health.RemoveFromClassList("low-health");
            }
            else
            {
                _health.RemoveFromClassList("middle-health");
                _health.AddToClassList("low-health");
            }
        }
    }
}