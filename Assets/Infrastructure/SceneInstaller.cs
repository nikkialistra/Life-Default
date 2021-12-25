using Game.Cameras;
using Game.Units;
using Game.Units.Services;
using Kernel.Saving;
using Kernel.Saving.Serialization;
using Kernel.Selection;
using Kernel.Targeting;
using Kernel.UI;
using Kernel.UI.Game;
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
        [SerializeField] private UnitSelection _unitSelection;
        [Required]
        [SerializeField] private SelectionInput _selectionInput;
        [Required]
        [SerializeField] private RectTransform _selectionRect;
        [Required] 
        [SerializeField] private UnitChoosing _unitChoosing;
        [Required]
        [SerializeField] private Canvas _uiCanvas;

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
        [SerializeField] private UnitTypeCounts _unitTypeCounts;
        
        [Title("Services")]
        [Required]
        [SerializeField] private UnitRepository _unitRepository;

        [Title("Spawning")] 
        [Required] 
        [SerializeField] private Transform _unitRoot;
        
        [Title("UI")]
        [Required]
        [SerializeField] private GameViews _gameViews;
        [Required]
        [SerializeField] private UnitTypesView _unitTypesView;
        [Required] 
        [SerializeField] private InfoPanelView _infoPanelView;

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
            Container.Bind<SelectedUnits>().AsSingle();
            Container.BindInstance(_unitSelection);
            Container.BindInstance(_selectionInput);
            Container.Bind<SelectionArea>().AsSingle().WithArguments(_selectionRect, _uiCanvas);
            Container.BindInstance(_unitChoosing);
        }

        private void BindTargeting()
        {
            Container.Bind<UnitSelecting>().AsSingle();
            Container.BindInstance(_movementCommand);
            Container.BindInstance(_pool).AsSingle();
            Container.BindInstance(_targetPrefab).WhenInjectedInto<TargetPool>();
            Container.BindInstance(_targetParent).WhenInjectedInto<TargetPool>();
        }

        private void BindUnitRepository()
        {
            Container.BindInstance(_unitRepository).AsSingle();
        }

        private void BindUnitTypeCounts()
        {
            Container.BindInstance(_unitTypeCounts);
        }

        private void BindUnitSpawning()
        {
            Container.BindFactory<UnitFacade, UnitFacade.Factory>().FromComponentInNewPrefab(_unitPrefab)
                .UnderTransform(_unitRoot);
            Container.BindInterfacesTo<UnitGenerator>().AsSingle().NonLazy();
        }

        private void BindUi()
        {
            Container.BindInstance(_gameViews);
            Container.BindInstance(_unitTypesView);
            Container.BindInstance(_infoPanelView);
        }

        private void BindSaving()
        {
            Container.Bind<SaveData>().AsSingle();
            Container.BindInstance(_unitsSaveLoadHandler);
            Container.Bind<Serialization>().AsSingle();
            Container.BindInstance(_savingLoadingGame);
            Container.BindInterfacesTo<UnitsResetting>().AsSingle().NonLazy();
        }
    }
}