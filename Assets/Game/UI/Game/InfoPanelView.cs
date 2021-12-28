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
        public VisualElement Info => _infoPanel;

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
            HideSelf();
        }

        public void SetUnits(List<UnitFacade> units)
        {
            if (units.Count == 0)
            {
                HideSelf();
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
            ShowSelf();
            ShowOneUnitDescription(unit);
        }

        public void HideSelf()
        {
            _infoPanel.AddToClassList("not-displayed");
        }

        private void SetMultipleUnits(List<UnitFacade> units)
        {
            ShowSelf();
            ShowUnitsDescription(units);
        }

        private void ShowSelf()
        {
            _infoPanel.RemoveFromClassList("not-displayed");
        }

        private void ShowOneUnitDescription(UnitFacade unit)
        {
            _unitsInfoView.HideSelf();
            _unitInfoView.ShowSelf();
            _unitInfoView.FillIn(unit);
        }

        private void ShowUnitsDescription(List<UnitFacade> units)
        {
            _unitInfoView.HideSelf();
            _unitsInfoView.ShowSelf();
            _unitsInfoView.FillIn(units);
        }
    }
}