using System.Collections.Generic;
using Units.Unit;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    [RequireComponent(typeof(UnitInfoView))]
    [RequireComponent(typeof(UnitsInfoView))]
    public class InfoPanelView : MonoBehaviour
    {
        private UnitInfoView _unitInfoView;
        private UnitsInfoView _unitsInfoView;

        private void Awake()
        {
            _unitInfoView = GetComponent<UnitInfoView>();
            _unitsInfoView = GetComponent<UnitsInfoView>();

            Tree = Resources.Load<VisualTreeAsset>("UI/Markup/GameLook/Components/InfoPanel").CloneTree();

            InfoPanel = Tree.Q<VisualElement>("info-panel");
        }

        public VisualElement Tree { get; private set; }
        public VisualElement InfoPanel { get; private set; }

        private void Start()
        {
            HideSelf();
        }

        public void SetUnits(List<UnitFacade> units)
        {
            switch (units.Count)
            {
                case 0:
                    HideSelf();
                    break;
                case 1:
                    SetUnit(units[0]);
                    break;
                default:
                    SetMultipleUnits(units);
                    break;
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
