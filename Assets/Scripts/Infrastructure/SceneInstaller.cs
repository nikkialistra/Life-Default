using Entities.Services;
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
using Units;
using Units.Appearance;
using UnityEngine;
using UnityEngine.Serialization;
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
        [SerializeField] private HumanAppearanceRegistry _humanAppearanceRegistry;
        
        [Title("Map")]
        [Required]
        [SerializeField] private Map _map;
        [Space]
        [Required]
        [SerializeField] private AstarPath _astarPath;
        [Space]
        [Required]
        [SerializeField] private Transform _resourceChunksParent;

        public override void InstallBindings()
        {
            BindTimeSystems();
            BindTileSystems();
            BindEnvironmentConditionSystems();
            BindResourceSystems();
            BindControls();
            BindEntities();
            BindMap();
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
            Container.BindInstance(_humanAppearanceRegistry);
        }
        
        private void BindMap()
        {
            Container.BindInstance(_map);
            Container.BindInstance(_astarPath);
            Container.BindInstance(_resourceChunksParent).WithId("ResourceChunksParent");
        }
    }
}
