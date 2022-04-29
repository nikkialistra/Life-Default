using Sirenix.OdinInspector;
using UI.Game;
using UI.Game.GameLook;
using UI.Game.GameLook.Components;
using UI.Game.GameLook.Components.Info;
using UI.Game.GameLook.Components.Info.ColonistInfo;
using UI.Game.GameLook.Components.Stock;
using UI.Menus.Primary;
using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class UiInstaller : MonoInstaller
    {
        [Title("Main")]
        [Required]
        [SerializeField] private GameLookView _gameLookView;
        [Required]
        [SerializeField] private GameMenuToggle _gameMenuToggle;
        [Required]
        [SerializeField] private GameViews _gameViews;
        
        [Title("Top Panels")]
        [Required]
        [SerializeField] private ResourcesView _resourcesView;
        [Required]
        [SerializeField] private QuestsView _questsView;
        [Required]
        [SerializeField] private NotificationsView _notificationsView;
        [Required]
        [SerializeField] private ColonistIconsView _colonistIconsView;
        [Required]
        [SerializeField] private TimeWeatherView _timeWeatherView;
        [Required]
        [SerializeField] private TimeTogglingView _timeTogglingView;

        [Title("Bottom Panels")]
        [Required]
        [SerializeField] private TileInfoView _tileInfoView;
        [Required]
        [SerializeField] private InfoPanelView _infoPanelView;
        [Required]
        [SerializeField] private ColonistInfoView _colonistInfoView;
        [Required]
        [SerializeField] private ColonistsInfoView _colonistsInfoView;

        public override void InstallBindings()
        {
            BindMain();
            BindTopPanels();
            BindBottomPanels();
        }

        private void BindMain()
        {
            Container.BindInstance(_gameLookView);
            Container.BindInstance(_gameMenuToggle);
            Container.BindInstance(_gameViews);
        }

        private void BindTopPanels()
        {
            Container.BindInstance(_resourcesView);
            Container.BindInstance(_questsView);
            Container.BindInstance(_notificationsView);
            Container.BindInstance(_colonistIconsView);
            Container.BindInstance(_timeWeatherView);
            Container.BindInstance(_timeTogglingView);
        }

        private void BindBottomPanels()
        {
            Container.BindInstance(_tileInfoView);
            Container.BindInstance(_infoPanelView);
            Container.BindInstance(_colonistInfoView);
            Container.BindInstance(_colonistsInfoView);
        }
    }
}
