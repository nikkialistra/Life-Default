using Animancer;
using Colonists;
using Colonists.Services.Selecting;
using General;
using General.Map;
using General.Questing;
using General.Selecting;
using General.Selecting.Selected;
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
using Units.Ancillaries;
using Units.Appearance;
using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class SceneInstaller : MonoInstaller
    {
        [Title("Set Up")]
        [SerializeField] private bool _isSetUpSession;

        [Title("Selection")]
        [Required]
        [SerializeField] private FrustumSelector _frustumSelector;
        [Required]
        [SerializeField] private Selection _selection;
        [Required]
        [SerializeField] private SelectingOperation _selectingOperation;
        [Required]
        [SerializeField] private SelectingInput _selectingInput;
        [Required]
        [SerializeField] private ColonistChoosing _colonistChoosing;

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
        [SerializeField] private InteractableHovering _interactableHovering;
        [Required]
        [SerializeField] private HumanNames _humanNames;
        [Required]
        [SerializeField] private HumanAppearanceRegistry _humanAppearanceRegistry;

        [Title("Map")]
        [Required]
        [SerializeField] private Terrain _terrain;
        [Required]
        [SerializeField] private MapInitialization _mapInitialization;
        [Required]
        [SerializeField] private TerrainModification _terrainModification;
        [Space]
        [Required]
        [SerializeField] private AstarPath _astarPath;
        [Space]
        [Required]
        [SerializeField] private Transform _resourceChunksParent;

        [Title("Other")]
        [Required]
        [SerializeField] private Transform _pathLineParent;
        [Required]
        [SerializeField] private Transform _linesToTrackedUnitsParent;

        public override void Start()
        {
            AnimancerState.AutomaticallyClearEvents = false;
        }

        public override void InstallBindings()
        {
            BindTimeSystems();
            BindSelection();
            BindTileSystems();
            BindEnvironmentConditionSystems();
            BindResourceSystems();
            BindControls();
            BindEntities();
            BindQuesting();
            BindMap();
            BindMisc();
        }

        private void BindTimeSystems()
        {
            Container.BindInstance(_tickingRegulator);
            Container.BindInstance(_timeToggling);
            Container.BindInstance(_seasonCycle);
            Container.BindInstance(_dayCycle);
        }

        private void BindSelection()
        {
            Container.BindInstance(_frustumSelector).AsSingle();
            Container.BindInstance(_selection).AsSingle();

            Container.Bind<SelectedColonists>().AsSingle();
            Container.Bind<SelectedEnemies>().AsSingle();
            Container.Bind<SelectedEntities>().AsSingle();

            Container.BindInstance(_selectingOperation);
            Container.BindInstance(_selectingInput);
            Container.BindInstance(_colonistChoosing);
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
            Container.BindInstance(_interactableHovering);
            Container.BindInstance(_humanNames);
            Container.BindInstance(_humanAppearanceRegistry);
        }

        private void BindQuesting()
        {
            Container.Bind<QuestServices>().AsSingle();
        }

        private void BindMap()
        {
            Container.BindInstance(_terrain);
            Container.BindInstance(_mapInitialization);
            Container.BindInstance(_terrainModification);
            Container.BindInstance(_astarPath);
            Container.BindInstance(_resourceChunksParent).WithId("ResourceChunksParent");
        }

        private void BindMisc()
        {
            Container.BindInstance(_pathLineParent).WhenInjectedInto<ColonistMeshAgent>();
            Container.BindInstance(_linesToTrackedUnitsParent).WhenInjectedInto<LineToTrackedUnit>();
        }
    }
}
