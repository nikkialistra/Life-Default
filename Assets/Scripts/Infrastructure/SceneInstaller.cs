using Cameras;
using Saving;
using Saving.Serialization;
using Sirenix.OdinInspector;
using Testing;
using UI.Game;
using UnitManagement.Selection;
using UnitManagement.Targeting;
using Units.Services;
using Units.Services.Selecting;
using Units.Unit;
using Units.Unit.UnitTypes;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Infrastructure
{
    public class SceneInstaller : MonoInstaller
    {
        [Title("Set Up")]
        [SerializeField] private bool _isSetUpSession;

        [Title("Input")]
        [Required]
        [SerializeField] private Camera _camera;
        [Required]
        [SerializeField] private CameraInputCombination _cameraInputCombination;
        [Required]
        [SerializeField] private FlyCamera _flyCamera;
        [Required]
        [SerializeField] private PlayerInput _playerInput;

        [Title("Selection")]
        [Required]
        [SerializeField] private UnitsSelection _unitsSelection;
        [Required]
        [SerializeField] private SelectionInput _selectionInput;
        [Required]
        [SerializeField] private UnitsChoosing _unitsChoosing;

        [Title("Targeting")]
        [Required]
        [SerializeField] private MovementCommand _movementCommand;
        [Required]
        [SerializeField] private TargetPool _pool;
        [Required]
        [SerializeField] private Target _targetPrefab;
        [Required]
        [SerializeField] private Transform _targetParent;

        [Title("Spawning")]
        [Required]
        [SerializeField] private GameObject _unitPrefab;
        [Required]
        [SerializeField] private int _unitPoolSize;
        [Required]
        [SerializeField] private Transform _unitsParent;

        [Title("Services")]
        [Required]
        [SerializeField] private UnitsRepository _unitsRepository;
        [Required]
        [SerializeField] private UnitsTypeCounts _unitsTypeCounts;
        [Required]
        [SerializeField] private UnitTypeAppearanceRegistry _unitTypeAppearanceRegistry;

        [Title("UI")]
        [Required]
        [SerializeField] private GameViews _gameViews;
        [Required]
        [SerializeField] private UnitTypesView _unitTypesView;
        [Required]
        [SerializeField] private InfoPanelView _infoPanelView;
        [Required]
        [SerializeField] private UnitInfoView _unitInfoView;
        [Required]
        [SerializeField] private UnitsInfoView _unitsInfoView;

        [Title("Saving")]
        [Required]
        [SerializeField] private UnitsSaveLoadHandler _unitsSaveLoadHandler;
        [Required]
        [SerializeField] private SavingLoadingGame _savingLoadingGame;

        public override void InstallBindings()
        {
            BindTesting();
            BindInput();
            BindUnitSelectionSystem();
            BindTargeting();
            BindUnitServices();
            BindUnitSpawning();
            BindUi();
            BindSaving();
        }

        private void BindTesting()
        {
            Container.BindInterfacesTo<UnitsGenerator>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<TogglingCameraMovement>().AsSingle().NonLazy();
        }

        private void BindInput()
        {
            Container.BindInstance(_camera);

            Container.BindInstance(_cameraInputCombination);
            Container.BindInstance(_isSetUpSession).WhenInjectedInto<CameraInputCombination>();

            Container.BindInstance(_flyCamera);
            Container.BindInstance(_isSetUpSession).WhenInjectedInto<FlyCamera>();

            Container.BindInstance(_playerInput);
        }

        private void BindUnitSelectionSystem()
        {
            Container.Bind<UnitsSelecting>().AsSingle();
            Container.Bind<SelectedUnits>().AsSingle();
            Container.BindInstance(_unitsSelection);
            Container.BindInstance(_selectionInput);
            Container.BindInstance(_unitsChoosing);
        }

        private void BindTargeting()
        {
            Container.BindInstance(_movementCommand);
            Container.BindInstance(_pool);
            Container.BindInstance(_targetPrefab).WhenInjectedInto<TargetPool>();
            Container.BindInstance(_targetParent).WhenInjectedInto<TargetPool>();
        }

        private void BindUnitServices()
        {
            Container.BindInstance(_unitsRepository);
            Container.BindInstance(_unitsTypeCounts);
            Container.BindInstance(_unitTypeAppearanceRegistry);
        }

        private void BindUnitSpawning()
        {
            Container.BindFactory<UnitType, Vector3, UnitFacade, UnitFacade.Factory>()
                .FromPoolableMemoryPool<UnitType, Vector3, UnitFacade, UnitFacadePool>(pool => pool
                    .WithInitialSize(_unitPoolSize)
                    .FromComponentInNewPrefab(_unitPrefab)
                    .UnderTransform(_unitsParent));
        }

        private void BindUi()
        {
            Container.BindInstance(_gameViews);
            Container.BindInstance(_unitTypesView);
            Container.BindInstance(_infoPanelView);
            Container.BindInstance(_unitInfoView);
            Container.BindInstance(_unitsInfoView);
        }

        private void BindSaving()
        {
            Container.Bind<SaveData>().AsSingle();
            Container.BindInstance(_unitsSaveLoadHandler);
            Container.Bind<Serialization>().AsSingle();
            Container.BindInstance(_savingLoadingGame);
            Container.BindInterfacesTo<UnitsResetting>().AsSingle().NonLazy();
        }

        private class UnitFacadePool : MonoPoolableMemoryPool<UnitType, Vector3, IMemoryPool, UnitFacade>
        {
        }
    }
}