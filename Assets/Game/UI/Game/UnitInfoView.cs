using System;
using Game.Units.Unit;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI.Game
{
    [RequireComponent(typeof(InfoPanelView))]
    [RequireComponent(typeof(ChangeColorFractions))]
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

        private UnitFacade _lastUnit;
        
        private InfoPanelView _parent;
        private TemplateContainer _tree;

        private ChangeColorFractions _changeColorFractions;
        
        private VisualElement _image;

        private Label _nominationType;
        private Label _nominationName;
        private ProgressBar _health;

        private void Awake()
        {
            _parent = GetComponent<InfoPanelView>();
            _changeColorFractions = GetComponent<ChangeColorFractions>();
                
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
            
            ChangeHealth();

            SubscribeToUnit(unit);
        }

        private void SubscribeToUnit(UnitFacade unit)
        {
            unit.HealthChange += ChangeHealth;
            unit.Die += HidePanel;
        }

        private void UnsubscribeFromLastUnit()
        {
            if (_lastUnit != null)
            {
                _lastUnit.HealthChange -= ChangeHealth;
                _lastUnit.Die -= HidePanel;
            }
        }

        private void ChangeHealth()
        {
            _health.value = (float) _lastUnit.Health / _lastUnit.MaxHealth;

            SetHealthColor();
        }

        private void SetHealthColor()
        {
            var fraction = _health.value;
            if (fraction > _changeColorFractions.Middle)
            {
                _health.RemoveFromClassList("middle-health");
                _health.RemoveFromClassList("low-health");
            }
            else if (fraction > _changeColorFractions.Low)
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