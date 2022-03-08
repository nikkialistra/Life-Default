using System.Collections.Generic;
using Colonists.Colonist;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    [RequireComponent(typeof(ColonistInfoView))]
    [RequireComponent(typeof(ColonistsInfoView))]
    public class InfoPanelView : MonoBehaviour
    {
        private const string VisualTreePath = "UI/Markup/GameLook/Components/InfoPanel";
        
        private Button _close;

        private ColonistInfoView _colonistInfoView;
        private ColonistsInfoView _colonistsInfoView;

        private void Awake()
        {
            _colonistInfoView = GetComponent<ColonistInfoView>();
            _colonistsInfoView = GetComponent<ColonistsInfoView>();

            Tree = Resources.Load<VisualTreeAsset>(VisualTreePath).CloneTree();

            InfoPanel = Tree.Q<VisualElement>("info-panel");

            _close = Tree.Q<Button>("close");
        }

        public VisualElement Tree { get; private set; }
        public VisualElement InfoPanel { get; private set; }
        
        private void Start()
        {
            HideSelf();
        }

        public void SetUnits(List<ColonistFacade> colonists)
        {
            switch (colonists.Count)
            {
                case 0:
                    HideSelf();
                    break;
                case 1:
                    SetColonist(colonists[0]);
                    break;
                default:
                    SetColonists(colonists);
                    break;
            }
        }

        public void SetColonist(ColonistFacade colonist)
        {
            ShowSelf();
            ShowColonistInfo(colonist);
        }

        public void HideSelf()
        {
            InfoPanel.AddToClassList("not-displayed");
            
            _close.clicked -= HideSelf;
        }

        private void SetColonists(List<ColonistFacade> colonists)
        {
            ShowSelf();
            ShowColonistsInfo(colonists);
        }

        private void ShowSelf()
        {
            InfoPanel.RemoveFromClassList("not-displayed");
            
            _close.clicked += HideSelf;
        }

        private void ShowColonistInfo(ColonistFacade colonist)
        {
            _colonistsInfoView.HideSelf();
            _colonistInfoView.ShowSelf();
            _colonistInfoView.FillIn(colonist);
        }

        private void ShowColonistsInfo(List<ColonistFacade> colonists)
        {
            _colonistInfoView.HideSelf();
            _colonistsInfoView.ShowSelf();
            _colonistsInfoView.FillIn(colonists);
        }
    }
}
