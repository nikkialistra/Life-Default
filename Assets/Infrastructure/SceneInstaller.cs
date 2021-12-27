using Game.UI;
using Game.UI.Game;
using Game.Units.Selecting;
using Game.Units.Services;
using Game.Units.Unit;
using Kernel.Saving;
using Kernel.Saving.Serialization;
using Kernel.Selection;
using Kernel.Targeting;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Infrastructure
{
    public class SceneInstaller : MonoInstaller
    {
        [Title("Base")]
        [SerializeField] private Camera _camera;
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
        [SerializeField] private GameObject _targetPrefab;
        [Required]
        [SerializeField] private Transform _targetParent;

        [Title("Units")] 
        [Required]
        [SerializeField] private GameObject _unitPrefab;
        [Required]
        [SerializeField] private UnitsTypeCounts _unitsTypeCounts;

        [Title("Services")]
        [Required]
        [SerializeField] private UnitsRepository _unitsRepository;

        [Title("Spawning")] 
        [Required]
        [SerializeField] private int _unitPoolSize;
        [SerializeField] private Transform _unitsParent;
        
        [Title("UI")]
        [Required]
        [SerializeField] private GameViews _gameViews;
        [Required]
        [SerializeField] private UnitTypesView _unitTypesView;
        [Required] 
        [SerializeField] private InfoPanelView _infoPanelView;
        [Required] 
        [SerializeField] private UnitDescriptionView _unitDescriptionView;
        [Required] 
        [SerializeField] private UnitsDescriptionView _unitsDescriptionView;

        [Title("Saving")]
        [Required]
        [SerializeField] private UnitsSaveLoadHandler _unitsSaveLoadHandler;
        [Required] 
        [SerializeField] private SavingLoadingGame _savingLoadingGame;

        public override void InstallBindings()
        {
            BindBase();
            BindUnitSelectionSystem();
            BindTargeting();
            BindUnitRepository();
            BindUnitTypeCounts();
            BindUnitSpawning();
            BindUi();
            BindSaving();
        }

        private void BindBase()
        {
            Container.BindInstance(_camera).AsSingle();
            Container.BindInstance(_playerInput).AsSingle();
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
            Container.BindInstance(_pool).AsSingle();
            Container.BindInstance(_targetPrefab).WhenInjectedInto<TargetPool>();
            Container.BindInstance(_targetParent).WhenInjectedInto<TargetPool>();
        }

        private void BindUnitRepository()
        {
            Container.BindInstance(_unitsRepository).AsSingle();
        }

        private void BindUnitTypeCounts()
        {
            Container.BindInstance(_unitsTypeCounts);
        }

        private void BindUnitSpawning()
        {
            Container.BindFactory<Vector3, UnitFacade, UnitFacade.Factory>()
                .FromPoolableMemoryPool<Vector3, UnitFacade, UnitFacadePool>(pool => pool
                    .WithInitialSize(_unitPoolSize)
                    .FromComponentInNewPrefab(_unitPrefab)
                    .UnderTransform(_unitsParent));
            
            Container.BindInterfacesTo<UnitsGenerator>().AsSingle().NonLazy();
        }

        private void BindUi()
        {
            Container.BindInstance(_gameViews);
            Container.BindInstance(_unitTypesView);
            Container.BindInstance(_infoPanelView);
            Container.BindInstance(_unitDescriptionView);
            Container.BindInstance(_unitsDescriptionView);
        }

        private void BindSaving()
        {
            Container.Bind<SaveData>().AsSingle();
            Container.BindInstance(_unitsSaveLoadHandler);
            Container.Bind<Serialization>().AsSingle();
            Container.BindInstance(_savingLoadingGame);
            Container.BindInterfacesTo<UnitsResetting>().AsSingle().NonLazy();
        }
        
        private class UnitFacadePool : MonoPoolableMemoryPool<Vector3, IMemoryPool, UnitFacade>
        {
        }
    }
}