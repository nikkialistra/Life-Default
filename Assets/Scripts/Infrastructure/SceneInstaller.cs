using Cameras;
using Environment;
using Environment.TemperatureRegulation;
using Environment.TimeCycle.Days;
using Environment.TimeCycle.Seasons;
using Environment.TimeCycle.Ticking;
using Environment.TimeCycle.TimeRegulation;
using ResourceManagement;
using Saving;
using Saving.Serialization;
using Sirenix.OdinInspector;
using Testing;
using UI;
using UI.Game;
using UI.Game.GameLook.Components;
using UI.Menus.Primary;
using Units.Services;
using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class SceneInstaller : MonoInstaller
    {
        [Title("Set Up")]
        [SerializeField] private bool _isSetUpSession;

        [Title("Game Systems")]
        [Required]
        [SerializeField] private TickingRegulator _tickingRegulator;
        [Required]
        [SerializeField] private DayCycle _dayCycle;
        [Required]
        [SerializeField] private SeasonCycle _seasonCycle;
        [Required]
        [SerializeField] private Temperature _temperature;
        [Required]
        [SerializeField] private TimeToggling _timeToggling;
        [Required]
        [SerializeField] private ResourceCounts _resourceCounts;

        [Title("Input")]
        [Required]
        [SerializeField] private GameCursors _gameCursors;
        [Required]
        [SerializeField] private Camera _camera;
        [Required]
        [SerializeField] private CameraMovement _cameraMovement;
        [Required]
        [SerializeField] private FlyCamera _flyCamera;

        [Title("UI")]
        [Required]
        [SerializeField] private GameMenuToggle _gameMenuToggle;
        [Required]
        [SerializeField] private GameViews _gameViews;
        [Required]
        [SerializeField] private TimeTogglingView _timeTogglingView;
        [Required]
        [SerializeField] private TimeWeatherView _timeWeatherView;
        [Required]
        [SerializeField] private MenuPanelView _menuPanelView;
        [Required]
        [SerializeField] private ResourcesView _resourcesView;
        [Required]
        [SerializeField] private InfoPanelView _infoPanelView;
        [Required]
        [SerializeField] private UnitInfoView _unitInfoView;
        [Required]
        [SerializeField] private UnitsInfoView _unitsInfoView;
        [Required]
        [SerializeField] private UnitTypesView _unitTypesView;

        [Title("Saving")]
        [Required]
        [SerializeField] private UnitSaveLoadHandler _unitSaveLoadHandler;
        [Required]
        [SerializeField] private SavingLoadingGame _savingLoadingGame;

        public override void InstallBindings()
        {
            BindGameSystems();
            BindInput();
            BindUi();
            BindSaving();
        }

        private void BindGameSystems()
        {
            Container.BindInstance(_tickingRegulator);
            Container.BindInstance(_dayCycle);
            Container.BindInstance(_seasonCycle);
            Container.BindInstance(_temperature);
            Container.BindInstance(_timeToggling);
            Container.BindInstance(_resourceCounts);
        }

        private void BindInput()
        {
            Container.BindInstance(_gameCursors);

            Container.BindInstance(_camera);

            Container.BindInstance(_cameraMovement);
            Container.BindInstance(_isSetUpSession).WhenInjectedInto<CameraMovement>();

            Container.BindInstance(_flyCamera);
            Container.BindInstance(_isSetUpSession).WhenInjectedInto<FlyCamera>();
        }

        private void BindUi()
        {
            Container.BindInstance(_gameMenuToggle);
            Container.BindInstance(_gameViews);
            Container.BindInstance(_timeTogglingView);
            Container.BindInstance(_timeWeatherView);
            Container.BindInstance(_menuPanelView);
            Container.BindInstance(_resourcesView);
            Container.BindInstance(_infoPanelView);
            Container.BindInstance(_unitInfoView);
            Container.BindInstance(_unitsInfoView);
            Container.BindInstance(_unitTypesView);
        }

        private void BindSaving()
        {
            Container.Bind<SaveData>().AsSingle();
            Container.BindInstance(_unitSaveLoadHandler);
            Container.Bind<Serialization>().AsSingle();
            Container.BindInstance(_savingLoadingGame);
            Container.BindInterfacesTo<UnitResetting>().AsSingle().NonLazy();
        }
    }
}
