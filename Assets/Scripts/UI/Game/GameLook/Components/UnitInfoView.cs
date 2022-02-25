using System;
using Sirenix.OdinInspector;
using Units.Unit;
using Units.Unit.UnitTypes;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    [RequireComponent(typeof(InfoPanelView))]
    [RequireComponent(typeof(ChangeColorFractions))]
    public class UnitInfoView : MonoBehaviour
    {
        private const string VisualTreePath = "UI/Markup/GameLook/Components/UnitInfo";
        
        [Title("Previews")]
        [Required]
        [SerializeField] private Texture2D _scoutPreview;
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

            _tree = Resources.Load<VisualTreeAsset>(VisualTreePath).CloneTree();

            _image = _tree.Q<VisualElement>("image");

            _nominationType = _tree.Q<Label>("nomination__type");
            _nominationName = _tree.Q<Label>("nomination__name");
            _health = _tree.Q<ProgressBar>("health__progress-bar");
        }

        private void OnDestroy()
        {
            UnsubscribeFromLastUnit();
        }

        public void ShowSelf()
        {
            _parent.InfoPanel.Add(_tree);
        }

        public void HideSelf()
        {
            if (_parent.InfoPanel.Contains(_tree))
            {
                _parent.InfoPanel.Remove(_tree);
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
                UnitType.Scout => new StyleBackground(_scoutPreview),
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
            _health.value = (float)_lastUnit.Health / _lastUnit.MaxHealth;

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
