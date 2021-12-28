using System.Collections.Generic;
using Game.Units.Unit;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI.Game
{
    [RequireComponent(typeof(UIDocument))]
    [RequireComponent(typeof(UnitInfoView))]
    [RequireComponent(typeof(UnitsInfoView))]
    public class InfoPanelView : MonoBehaviour
    {
        public VisualElement Root => _infoPanel;
        
        private UnitFacade _lastUnit;

        private VisualElement _tree;

        private UnitInfoView _unitInfoView;
        private UnitsInfoView _unitsInfoView;

        private VisualElement _infoPanel;
        
        private void Awake()
        {
            _unitInfoView = GetComponent<UnitInfoView>();
            _unitsInfoView = GetComponent<UnitsInfoView>();
            
            _tree = GetComponent<UIDocument>().rootVisualElement;

            _infoPanel = _tree.Q<VisualElement>("info-panel");
        }

        private void Start()
        {
            HideInfoPanel();
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
            UnsubscribeFromLastUnit();
            unit.Die += HideInfoPanel;
            
            _unitsInfoView.HideSelf();
            _unitInfoView.ShowSelf();
            _unitInfoView.FillIn(unit);
            
            _lastUnit = unit;
        }

        private void ShowUnitsDescription(List<UnitFacade> units)
        {
            _unitInfoView.HideSelf();
            _unitsInfoView.ShowSelf();
            _unitsInfoView.FillIn(units);
        }
        
        private void UnsubscribeFromLastUnit()
        {
            if (_lastUnit != null)
            {
                _lastUnit.Die -= HideInfoPanel;
            }
        }
    }
}