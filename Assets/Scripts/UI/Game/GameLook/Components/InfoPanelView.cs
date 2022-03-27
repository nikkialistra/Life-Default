using System.Collections.Generic;
using Colonists.Colonist;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    [RequireComponent(typeof(ColonistInfoView))]
    [RequireComponent(typeof(ColonistsInfoView))]
    [RequireComponent(typeof(CommandsView))]
    public class InfoPanelView : MonoBehaviour
    {
        private const string VisualTreePath = "UI/Markup/GameLook/Components/InfoPanel";

        private ColonistInfoView _colonistInfoView;
        private ColonistsInfoView _colonistsInfoView;

        private void Awake()
        {
            _colonistInfoView = GetComponent<ColonistInfoView>();
            _colonistsInfoView = GetComponent<ColonistsInfoView>();

            Tree = Resources.Load<VisualTreeAsset>(VisualTreePath).CloneTree();

            InfoPanel = Tree.Q<VisualElement>("info-panel");
        }

        public VisualElement Tree { get; private set; }
        public VisualElement InfoPanel { get; private set; }
        
        private void Start()
        {
            HideSelf();
        }

        public void SetColonists(List<ColonistFacade> colonists)
        {
            ShowSelf();
            
            switch (colonists.Count)
            {
                case 0:
                    HideSelf();
                    break;
                case 1:
                    ShowColonistInfo(colonists[0]);
                    break;
                default:
                    ShowColonistsInfo(colonists.Count);
                    break;
            }
        }

        public void SetColonist(ColonistFacade colonist)
        {
            ShowColonistInfo(colonist);
        }

        public void HideSelf()
        {
            InfoPanel.AddToClassList("not-displayed");
        }

        private void ShowSelf()
        {
            InfoPanel.RemoveFromClassList("not-displayed");
        }

        private void ShowColonistInfo(ColonistFacade colonist)
        {
            HidePanels();
            
            _colonistInfoView.ShowSelf();
            _colonistInfoView.FillIn(colonist);
        }

        private void ShowColonistsInfo(int count)
        {
            HidePanels();
            
            _colonistsInfoView.ShowSelf();
            _colonistsInfoView.SetCount(count);
        }

        private void HidePanels()
        {
            _colonistInfoView.HideSelf();
            _colonistsInfoView.HideSelf();
        }
    }
}
