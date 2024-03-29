﻿using System.Collections.Generic;
using Aborigines;
using Colonists;
using ResourceManagement;
using Sirenix.OdinInspector;
using UI.Game.GameLook.Components.Info.ColonistInfo;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components.Info
{
    [RequireComponent(typeof(ColonistInfoView))]
    [RequireComponent(typeof(ColonistsInfoView))]
    [RequireComponent(typeof(AborigineInfoView))]
    [RequireComponent(typeof(AboriginesInfoView))]
    [RequireComponent(typeof(ResourceInfoView))]
    [RequireComponent(typeof(ResourcesInfoView))]
    [RequireComponent(typeof(ResourceChunkInfoView))]
    [RequireComponent(typeof(ResourceChunksInfoView))]
    [RequireComponent(typeof(CommandsView))]
    public class InfoPanelView : MonoBehaviour
    {
        public VisualElement Tree { get; private set; }
        public VisualElement InfoPanel { get; private set; }

        [Required]
        [SerializeField] private VisualTreeAsset _asset;

        private ColonistInfoView _colonistInfoView;
        private ColonistsInfoView _colonistsInfoView;

        private AborigineInfoView _aborigineInfoView;
        private AboriginesInfoView _aboriginesInfoView;

        private ResourceInfoView _resourceInfoView;
        private ResourcesInfoView _resourcesInfoView;

        private ResourceChunkInfoView _resourceChunkInfoView;
        private ResourceChunksInfoView _resourceChunksInfoView;

        private Colonist _shownColonist;
        private Aborigine _shownAborigine;
        private Resource _shownResource;
        private ResourceChunk _shownResourceChunk;

        private void Awake()
        {
            _colonistInfoView = GetComponent<ColonistInfoView>();
            _colonistsInfoView = GetComponent<ColonistsInfoView>();

            _aborigineInfoView = GetComponent<AborigineInfoView>();
            _aboriginesInfoView = GetComponent<AboriginesInfoView>();

            _resourceInfoView = GetComponent<ResourceInfoView>();
            _resourcesInfoView = GetComponent<ResourcesInfoView>();

            _resourceChunkInfoView = GetComponent<ResourceChunkInfoView>();
            _resourceChunksInfoView = GetComponent<ResourceChunksInfoView>();

            Tree = _asset.CloneTree();

            InfoPanel = Tree.Q<VisualElement>("info-panel");
        }

        private void Start()
        {
            HideSelf();
        }

        public void SetColonists(List<Colonist> colonists)
        {
            switch (colonists.Count)
            {
                case 0:
                    break;
                case 1:
                    SetColonist(colonists[0]);
                    break;
                default:
                    PrepareEmptyPanel();
                    ShowColonistsInfo(colonists.Count);
                    break;
            }
        }

        public void SetAborigines(List<Aborigine> aborigines)
        {
            switch (aborigines.Count)
            {
                case 0:
                    break;
                case 1:
                    SetAborigine(aborigines[0]);
                    break;
                default:
                    PrepareEmptyPanel();
                    ShowAboriginesInfo(aborigines.Count);
                    break;
            }
        }

        public void SetResources(List<Resource> resources)
        {
            switch (resources.Count)
            {
                case 0:
                    break;
                case 1:
                    SetResource(resources[0]);
                    break;
                default:
                    PrepareEmptyPanel();
                    ShowResourcesInfo(resources);
                    break;
            }
        }

        public void SetResourceChunks(List<ResourceChunk> resourceChunks)
        {
            switch (resourceChunks.Count)
            {
                case 0:
                    break;
                case 1:
                    PrepareEmptyPanel();
                    SetResourceChunk(resourceChunks[0]);
                    break;
                default:
                    PrepareEmptyPanel();
                    ShowResourceChunksInfo(resourceChunks);
                    break;
            }
        }

        public void SetColonist(Colonist colonist)
        {
            PrepareEmptyPanel();
            _shownColonist = colonist;
            ShowColonistInfo();
        }

        public void SetAborigine(Aborigine aborigine)
        {
            PrepareEmptyPanel();
            _shownAborigine = aborigine;
            ShowAborigineInfo();
        }

        public void SetResource(Resource resource)
        {
            PrepareEmptyPanel();
            _shownResource = resource;
            ShowResourceInfo();
        }

        public void SetResourceChunk(ResourceChunk resourceChunk)
        {
            PrepareEmptyPanel();
            _shownResourceChunk = resourceChunk;
            ShowResourceChunkInfo();
        }

        public void UnsetColonist(Colonist colonist)
        {
            if (_shownColonist == colonist)
            {
                _shownColonist = null;
                _colonistInfoView.HideSelf();
                HideSelf();
            }
        }

        public void UnsetAborigine(Aborigine aborigine)
        {
            if (_shownAborigine == aborigine)
            {
                _shownAborigine = null;
                _aboriginesInfoView.HideSelf();
                HideSelf();
            }
        }

        public void UnsetResource(Resource resource)
        {
            if (_shownResource == resource)
            {
                _shownResource = null;
                _resourceInfoView.HideSelf();
                HideSelf();
            }
        }

        public void UnsetResourceChunk(ResourceChunk resourceChunk)
        {
            if (_shownResourceChunk == resourceChunk)
            {
                _shownResourceChunk = null;
                _resourceInfoView.HideSelf();
                HideSelf();
            }
        }

        private void ShowColonistsInfo(int count)
        {
            _colonistsInfoView.ShowSelf();
            _colonistsInfoView.SetCount(count);
        }

        public void Hide()
        {
            HidePanels();
            HideSelf();
        }

        private void ShowAboriginesInfo(int count)
        {
            _aboriginesInfoView.ShowSelf();
            _aboriginesInfoView.SetCount(count);
        }

        private void ShowResourcesInfo(List<Resource> resources)
        {
            _resourcesInfoView.ShowSelf();
            _resourcesInfoView.FillIn(resources);
        }

        private void ShowResourceChunksInfo(List<ResourceChunk> resourceChunks)
        {
            _resourceChunksInfoView.ShowSelf();
            _resourceChunksInfoView.FillIn(resourceChunks);
        }

        private void ShowColonistInfo()
        {
            _colonistInfoView.ShowSelf();
            _colonistInfoView.FillIn(_shownColonist);
        }

        private void ShowAborigineInfo()
        {
            _aborigineInfoView.ShowSelf();
            _aborigineInfoView.FillIn(_shownAborigine);
        }

        private void ShowResourceInfo()
        {
            _resourceInfoView.ShowSelf();
            _resourceInfoView.FillIn(_shownResource);
        }

        private void ShowResourceChunkInfo()
        {
            _resourceChunkInfoView.ShowSelf();
            _resourceChunkInfoView.FillIn(_shownResourceChunk);
        }

        private void HideSelf()
        {
            InfoPanel.style.display = DisplayStyle.None;
        }

        private void ShowSelf()
        {
            InfoPanel.style.display = DisplayStyle.Flex;
        }

        private void PrepareEmptyPanel()
        {
            UnsetAll();
            ShowSelf();
            HidePanels();
        }

        private void UnsetAll()
        {
            _shownColonist = null;
            _shownAborigine = null;
            _shownResource = null;
            _shownResourceChunk = null;
        }

        private void HidePanels()
        {
            _colonistInfoView.HideSelf();
            _colonistsInfoView.HideSelf();

            _aborigineInfoView.HideSelf();
            _aboriginesInfoView.HideSelf();

            _resourceInfoView.HideSelf();
            _resourcesInfoView.HideSelf();

            _resourceChunkInfoView.HideSelf();
            _resourceChunksInfoView.HideSelf();
        }
    }
}
