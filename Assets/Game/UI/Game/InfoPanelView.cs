using System;
using System.Collections.Generic;
using Game.Units.Unit;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI.Game
{
    [RequireComponent(typeof(UIDocument))]
    public class InfoPanelView : MonoBehaviour
    {
        [SerializeField] private Texture2D _travelerPreview;
        [SerializeField] private Texture2D _lumberjackPreview;
        [SerializeField] private Texture2D _masonPreview;
        [SerializeField] private Texture2D _meleePreview;
        [SerializeField] private Texture2D _archerPreview;
        [SerializeField] private Texture2D _multipleUnitsPreview;
        

        private VisualElement _tree;

        private VisualElement _infoPanel;
        private VisualElement _infoImage;

        private VisualElement _oneUnitDescription;
        private VisualElement _multipleUnitsDescription;

        private Label _oneUnitType;
        private Label _oneUnitName;
        private ProgressBar _oneUnitHealth;

        private Label _multipleUnitsTypeAndCount;
        
        private UnitFacade _unit;
        private List<UnitFacade> _units;

        private void Awake()
        {
            _tree = GetComponent<UIDocument>().rootVisualElement;

            _infoPanel = _tree.Q<VisualElement>("info-panel");
            _infoImage = _tree.Q<VisualElement>("info-image");

            _oneUnitDescription = _tree.Q<VisualElement>("one-unit-description");
            _multipleUnitsDescription = _tree.Q<VisualElement>("multiple-units-description");

            _oneUnitType = _tree.Q<Label>("one-unit-nomination__type");
            _oneUnitName = _tree.Q<Label>("one-unit-nomination__name");
            _oneUnitHealth = _tree.Q<ProgressBar>("one-unit-health__progress-bar");

            _multipleUnitsTypeAndCount = _tree.Q<Label>("multiple-units-nomination__type-and-count");
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
            _multipleUnitsTypeAndCount.text = $"Units ({_units.Count})";
        }
    }
}