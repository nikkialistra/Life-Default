using Entities.Services;
using Entities.Services.Appearance;
using General;
using General.TemperatureRegulation;
using General.TileManagement.Tiles;
using General.TimeCycle.Days;
using General.TimeCycle.Seasons;
using General.TimeCycle.Ticking;
using General.TimeCycle.TimeRegulation;
using General.WeatherRegulation;
using ResourceManagement;
using Sirenix.OdinInspector;
using Testing;
using UI;
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
        
        [Title("Entities")]
        [Required]
        [SerializeField] private HumanNames _humanNames;
        [Required]
        [SerializeField] private HumanAppearance _humanAppearance;
        

        public override void InstallBindings()
        {
            BindTimeSystems();
            BindTileSystems();
            BindEnvironmentConditionSystems();
            BindResourceSystems();
            BindControls();
            BindEntities();
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

        private void BindEntities()
        {
            Container.BindInstance(_humanNames);
            Container.BindInstance(_humanAppearance);
        }
    }
}
