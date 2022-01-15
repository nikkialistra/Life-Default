using System.Collections.Generic;
using Units.Unit;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game
{
    [RequireComponent(typeof(UIDocument))]
    [RequireComponent(typeof(UnitInfoView))]
    [RequireComponent(typeof(UnitsInfoView))]
    public class InfoPanelView : MonoBehaviour
    {
        private VisualElement _tree;

        private UnitInfoView _unitInfoView;
        private UnitsInfoView _unitsInfoView;

        private void Awake()
        {
            _unitInfoView = GetComponent<UnitInfoView>();
            _unitsInfoView = GetComponent<UnitsInfoView>();

            _tree = GetComponent<UIDocument>().rootVisualElement;

            InfoPanel = _tree.Q<VisualElement>("info-panel");
        }

        public VisualElement InfoPanel { get; private set; }

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
            InfoPanel.AddToClassList("not-displayed");
        }

        private void SetMultipleUnits(List<UnitFacade> units)
        {
            ShowSelf();
            ShowUnitsDescription(units);
        }

        private void ShowSelf()
        {
            InfoPanel.RemoveFromClassList("not-displayed");
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
