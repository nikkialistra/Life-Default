using System;
using System.Collections.Generic;
using Game.Units.Unit;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace Game.UI.Game
{
    [RequireComponent(typeof(UIDocument))]
    public class InfoPanelView : MonoBehaviour
    {
        public VisualElement Root => _infoPanel;

        private VisualElement _tree;

        private UnitDescriptionView _unitDescriptionView;
        private UnitsDescriptionView _unitsDescriptionView;

        private VisualElement _infoPanel;

        [Inject]
        public void Construct(UnitDescriptionView unitDescriptionView, UnitsDescriptionView unitsDescriptionView)
        {
            _unitDescriptionView = unitDescriptionView;
            _unitsDescriptionView = unitsDescriptionView;
        }

        private void Awake()
        {
            _tree = GetComponent<UIDocument>().rootVisualElement;

            _infoPanel = _tree.Q<VisualElement>("info-panel");
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
            ShowInfoPanel();
            ShowOneUnitDescription(unit);
        }

        private void SetMultipleUnits(List<UnitFacade> units)
        {
            ShowInfoPanel();
            ShowUnitsDescription(units);
        }

        private void ShowInfoPanel()
        {
            _infoPanel.RemoveFromClassList("not-displayed");
        }

        private void HideInfoPanel()
        {
            _infoPanel.AddToClassList("not-displayed");
        }

        private void ShowOneUnitDescription(UnitFacade unit)
        {
            _unitsDescriptionView.HideSelf();
            _unitDescriptionView.ShowSelf();
            _unitDescriptionView.FillIn(unit);
        }

        private void ShowUnitsDescription(List<UnitFacade> units)
        {
            _unitDescriptionView.HideSelf();
            _unitsDescriptionView.ShowSelf();
            _unitsDescriptionView.FillIn(units);
        }
    }
}