using Game.Units;
using Game.Units.Services;
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
        [SerializeField] private SelectionInput _selectionInput;
        [Required]
        [SerializeField] private RectTransform _selectionRect;
        [Required]
        [SerializeField] private Canvas _uiCanvas;

        [Title("Targeting")]
        [Required]
        [SerializeField] private MovementCommand _movementCommand;

        [Title("Units")] 
        [Required]
        [SerializeField] private GameObject _unitPrefab;

        [Title("Unit Handling")]
        [Required]
        [SerializeField] private UnitRepository _unitRepository;
        [Required]
        [SerializeField] private PointObjectPool _pool;
        [Required]
        [SerializeField] private GameObject _pointPrefab;
        
        public override void InstallBindings()
        {
            BindBase();
            BindUnitSelectionSystem();
            BindTargeting();
            BindUnitHandling();
            BindUnitSpawning();
        }

        private void BindBase()
        {
            Container.BindInstance(_camera).AsSingle();
            Container.BindInstance(_playerInput).AsSingle();
        }

        private void BindUnitSelectionSystem()
        {
            Container.BindInterfacesAndSelfTo<UnitSelection>().AsSingle().NonLazy();
            Container.BindInstance(_selectionInput);
            Container.Bind<SelectionArea>().AsSingle().WithArguments(_selectionRect, _uiCanvas);
        }

        private void BindTargeting()
        {
            Container.Bind<ProjectionSelector>().AsSingle();
            Container.BindInstance(_movementCommand);
        }

        private void BindUnitHandling()
        {
            Container.BindInstance(_unitRepository).AsSingle();
            Container.BindInstance(_pool).AsSingle();
            Container.BindInstance(_pointPrefab).WhenInjectedInto<PointObjectPool>();
        }

        private void BindUnitSpawning()
        {
            Container.BindFactory<UnitFacade, UnitFacade.Factory>().FromComponentInNewPrefab(_unitPrefab);
            Container.BindInterfacesTo<UnitGenerator>().AsSingle().NonLazy();
        }
    }
}