using System.Collections.Generic;
using Units.Unit;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    [RequireComponent(typeof(ColonistInfoView))]
    [RequireComponent(typeof(UnitsInfoView))]
    public class InfoPanelView : MonoBehaviour
    {
        private const string VisualTreePath = "UI/Markup/GameLook/Components/InfoPanel";
        
        private ColonistInfoView _colonistInfoView;
        private UnitsInfoView _unitsInfoView;

        private void Awake()
        {
            _colonistInfoView = GetComponent<ColonistInfoView>();
            _unitsInfoView = GetComponent<UnitsInfoView>();

            Tree = Resources.Load<VisualTreeAsset>(VisualTreePath).CloneTree();

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
            ShowColonistInfo(unit);
        }

        public void HideSelf()
        {
            InfoPanel.AddToClassList("not-displayed");
        }

        private void SetMultipleUnits(List<UnitFacade> units)
        {
            ShowSelf();
            ShowColonistsInfo(units);
        }

        private void ShowSelf()
        {
            InfoPanel.RemoveFromClassList("not-displayed");
        }

        private void ShowColonistInfo(UnitFacade unit)
        {
            _unitsInfoView.HideSelf();
            _colonistInfoView.ShowSelf();
            _colonistInfoView.FillIn(unit);
        }

        private void ShowColonistsInfo(List<UnitFacade> units)
        {
            _colonistInfoView.HideSelf();
            _unitsInfoView.ShowSelf();
            _unitsInfoView.FillIn(units);
        }
    }
}
