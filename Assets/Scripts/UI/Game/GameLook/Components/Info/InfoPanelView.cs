using System.Collections.Generic;
using Colonists;
using Enemies;
using ResourceManagement;
using UI.Game.GameLook.Components.Info.ColonistInfo;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components.Info
{
    [RequireComponent(typeof(ColonistInfoView))]
    [RequireComponent(typeof(ColonistsInfoView))]
    [RequireComponent(typeof(EnemyInfoView))]
    [RequireComponent(typeof(ResourceInfoView))]
    [RequireComponent(typeof(ResourceChunkInfoView))]
    [RequireComponent(typeof(CommandsView))]
    public class InfoPanelView : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset _asset;

        private ColonistInfoView _colonistInfoView;
        private ColonistsInfoView _colonistsInfoView;
        private EnemyInfoView _enemyInfoView;
        private ResourceInfoView _resourceInfoView;
        private ResourceChunkInfoView _resourceChunkInfoView;

        private Colonist _shownColonist;
        private Enemy _shownEnemy;
        private Resource _shownResource;
        private ResourceChunk _shownResourceChunk;

        private void Awake()
        {
            _colonistInfoView = GetComponent<ColonistInfoView>();
            _colonistsInfoView = GetComponent<ColonistsInfoView>();
            _enemyInfoView = GetComponent<EnemyInfoView>();
            _resourceInfoView = GetComponent<ResourceInfoView>();
            _resourceChunkInfoView = GetComponent<ResourceChunkInfoView>();

            Tree = _asset.CloneTree();

            InfoPanel = Tree.Q<VisualElement>("info-panel");
        }

        public VisualElement Tree { get; private set; }
        public VisualElement InfoPanel { get; private set; }

        private void Start()
        {
            HideSelf();
        }

        public void SetColonists(List<Colonist> colonists)
        {
            switch (colonists.Count)
            {
                case 0:
                    if (NotShowingEntityInfo())
                    {
                        HidePanels();
                        HideSelf();
                    }
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

        public void SetEnemies(List<Enemy> enemies)
        {
            switch (enemies.Count)
            {
                case 0:
                    if (NotShowingEntityInfo())
                    {
                        HidePanels();
                        HideSelf();
                    }
                    break;
                case 1:
                    SetEnemy(enemies[0]);
                    break;
                default:
                    PrepareEmptyPanel();
                    ShowEnemiesInfo(enemies.Count);
                    break;
            }
        }

        public void SetResources(List<Resource> resources)
        {
            
        }

        public void SetResourceChunks(List<ResourceChunk> resourceChunks)
        {
            
        }

        public void SetColonist(Colonist colonist)
        {
            PrepareEmptyPanel();
            _shownColonist = colonist;
            ShowColonistInfo();
        }

        public void SetEnemy(Enemy enemy)
        {
            PrepareEmptyPanel();
            _shownEnemy = enemy;
            ShowEnemyInfo();
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

        public void UnsetEnemy(Enemy enemy)
        {
            
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

        private bool NotShowingEntityInfo()
        {
            return _shownResource == null && _shownResourceChunk == null;
        }

        private void ShowColonistInfo()
        {
            _colonistInfoView.ShowSelf();
            _colonistInfoView.FillIn(_shownColonist);
        }

        private void ShowEnemyInfo()
        {
            _enemyInfoView.ShowSelf();
            _enemyInfoView.FillIn(_shownEnemy);
        }

        private void ShowColonistsInfo(int count)
        {
            _colonistsInfoView.ShowSelf();
            _colonistsInfoView.SetCount(count);
        }
        
        private void ShowEnemiesInfo(int count)
        {
            
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
            _shownEnemy = null;
            _shownResource = null;
            _shownResourceChunk = null;
        }

        private void HidePanels()
        {
            _colonistInfoView.HideSelf();
            _colonistsInfoView.HideSelf();
            _enemyInfoView.HideSelf();
            _resourceInfoView.HideSelf();
            _resourceChunkInfoView.HideSelf();
        }
    }
}
