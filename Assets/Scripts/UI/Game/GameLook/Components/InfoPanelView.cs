using System;
using System.Collections.Generic;
using Colonists.Colonist;
using Entities;
using Entities.Types;
using ResourceManagement;
using UI.Game.GameLook.Components.ColonistInfo;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    [RequireComponent(typeof(ColonistInfoView))]
    [RequireComponent(typeof(ColonistsInfoView))]
    [RequireComponent(typeof(EntityInfoView))]
    [RequireComponent(typeof(CommandsView))]
    public class InfoPanelView : MonoBehaviour
    {
        private const string VisualTreePath = "UI/Markup/GameLook/Components/InfoPanel";

        private ColonistInfoView _colonistInfoView;
        private ColonistsInfoView _colonistsInfoView;
        private EntityInfoView _entityInfoView;
        
        private void Awake()
        {
            _colonistInfoView = GetComponent<ColonistInfoView>();
            _colonistsInfoView = GetComponent<ColonistsInfoView>();
            _entityInfoView = GetComponent<EntityInfoView>();

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
            PrepareEmptyPanel();

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
            PrepareEmptyPanel();
            
            ShowColonistInfo(colonist);
        }

        public void SetResource(Resource resource)
        {
            PrepareEmptyPanel();
            
            ShowResourceInfo(resource);
        }

        private void ShowResourceInfo(Resource resource)
        {
            _entityInfoView.ShowSelf();
            
            _entityInfoView.FillIn(resource);
        }

        public void HideSelf()
        {
            InfoPanel.style.display = DisplayStyle.None;
        }

        private void ShowSelf()
        {
            InfoPanel.style.display = DisplayStyle.Flex;
        }

        private void PrepareEmptyPanel()
        {
            ShowSelf();
            HidePanels();
        }

        private void ShowColonistInfo(ColonistFacade colonist)
        {
            _colonistInfoView.ShowSelf();
            _colonistInfoView.FillIn(colonist);
        }

        private void ShowColonistsInfo(int count)
        {
            _colonistsInfoView.ShowSelf();
            _colonistsInfoView.SetCount(count);
        }

        private void HidePanels()
        {
            _colonistInfoView.HideSelf();
            _colonistsInfoView.HideSelf();
            _entityInfoView.HideSelf();
        }
    }
}
