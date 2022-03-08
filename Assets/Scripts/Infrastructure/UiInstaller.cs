using Sirenix.OdinInspector;
using UI.Game;
using UI.Game.GameLook;
using UI.Game.GameLook.Components;
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
        [SerializeField] private TimeTogglingView _timeTogglingView;
        [Required]
        [SerializeField] private TimeWeatherView _timeWeatherView;
        [Required]
        [SerializeField] private MenuPanelView _menuPanelView;
        [Required]
        [SerializeField] private ResourcesView _resourcesView;

        [Title("Info Panels")]
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
            BindTopPanel();
            BindInfoPanels();
        }

        private void BindMain()
        {
            Container.BindInstance(_gameLookView);
            Container.BindInstance(_gameMenuToggle);
            Container.BindInstance(_gameViews);
        }

        private void BindTopPanel()
        {
            Container.BindInstance(_timeTogglingView);
            Container.BindInstance(_timeWeatherView);
            Container.BindInstance(_menuPanelView);
            Container.BindInstance(_resourcesView);
        }

        private void BindInfoPanels()
        {
            Container.BindInstance(_tileInfoView);
            Container.BindInstance(_infoPanelView);
            Container.BindInstance(_colonistInfoView);
            Container.BindInstance(_colonistsInfoView);
        }
    }
}
