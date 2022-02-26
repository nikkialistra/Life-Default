using Cameras;
using Environment.TemperatureRegulation;
using Environment.TileManagement.Tiles;
using Environment.TimeCycle.Days;
using Environment.TimeCycle.Seasons;
using Environment.TimeCycle.Ticking;
using Environment.TimeCycle.TimeRegulation;
using Environment.WeatherRegulation;
using ResourceManagement;
using Saving;
using Saving.Serialization;
using Sirenix.OdinInspector;
using Testing;
using UI;
using Units.Services;
using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class SceneInstaller : MonoInstaller
    {
        [Title("Set Up")]
        [SerializeField] private bool _isSetUpSession;

        [Title("Time")]
        [Required]
        [SerializeField] private TickingRegulator _tickingRegulator;
        [Required]
        [SerializeField] private TimeToggling _timeToggling;
        [Required]
        [SerializeField] private DayCycle _dayCycle;
        [Required]
        [SerializeField] private SeasonCycle _seasonCycle;
        
        [Title("Tiles")]
        [Required]
        [SerializeField] private TileGrid _tileGrid;
        
        [Title("Environment Conditions")]
        [Required]
        [SerializeField] private WeatherEnvironmentInfluence _weatherEnvironmentInfluence;
        [Required]
        [SerializeField] private WeatherEffectsRegistry _weatherEffectsRegistry;
        [Required]
        [SerializeField] private Temperature _temperature;
        
        [Title("Resources")]
        [Required]
        [SerializeField] private ResourceCounts _resourceCounts;

        [Title("Controls")]
        [Required]
        [SerializeField] private GameCursors _gameCursors;
        [Required]
        [SerializeField] private Camera _camera;
        [Required]
        [SerializeField] private CameraMovement _cameraMovement;
        [Required]
        [SerializeField] private FlyCamera _flyCamera;

        [Title("Saving")]
        [Required]
        [SerializeField] private UnitSaveLoadHandler _unitSaveLoadHandler;
        [Required]
        [SerializeField] private SavingLoadingGame _savingLoadingGame;

        public override void InstallBindings()
        {
            BindTimeSystems();
            BindTileSystems();
            BindEnvironmentConditionSystems();
            BindResourceSystems();
            BindControls();
            BindSaving();
        }

        private void BindTimeSystems()
        {
            Container.BindInstance(_tickingRegulator);
            Container.BindInstance(_timeToggling);
            Container.BindInstance(_seasonCycle);
            Container.BindInstance(_dayCycle);
        }

        private void BindTileSystems()
        {
            Container.BindInstance(_tileGrid);
        }

        private void BindEnvironmentConditionSystems()
        {
            Container.BindInstance(_weatherEnvironmentInfluence);
            Container.BindInstance(_weatherEffectsRegistry);
            Container.BindInstance(_temperature);
        }

        private void BindResourceSystems()
        {
            Container.BindInstance(_resourceCounts);
        }

        private void BindControls()
        {
            Container.BindInstance(_gameCursors);

            Container.BindInstance(_camera);

            Container.BindInstance(_cameraMovement);
            Container.BindInstance(_isSetUpSession).WhenInjectedInto<CameraMovement>();

            Container.BindInstance(_flyCamera);
            Container.BindInstance(_isSetUpSession).WhenInjectedInto<FlyCamera>();
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
