using Sirenix.OdinInspector;
using UnitManagement.Selection;
using UnitManagement.Targeting;
using Units.Services;
using Units.Services.Selecting;
using Units.Unit;
using Units.Unit.UnitTypes;
using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class UnitsInstaller : MonoInstaller
    {
        [Title("Selection")]
        [Required]
        [SerializeField] private UnitSelection _unitSelection;
        [Required]
        [SerializeField] private SelectionInput _selectionInput;
        [Required]
        [SerializeField] private UnitChoosing _unitChoosing;

        [Title("Ordering")]
        [Required]
        [SerializeField] private OrderMarkPool _orderMarkPool;
        [Required]
        [SerializeField] private OrderMark _orderMarkPrefab;
        [Required]
        [SerializeField] private Transform _orderMarksParent;

        [Title("Spawning")]
        [Required]
        [SerializeField] private GameObject _unitPrefab;
        [Required]
        [SerializeField] private int _unitPoolSize;
        [Required]
        [SerializeField] private Transform _unitsParent;

        [Title("Services")]
        [Required]
        [SerializeField] private UnitClassSpecsRepository _unitClassSpecsRepository;
        [Required]
        [SerializeField] private UnitRepository _unitRepository;
        [Required]
        [SerializeField] private UnitTypeCounts _unitTypeCounts;
        [Required]
        [SerializeField] private UnitTypeAppearanceRegistry _unitTypeAppearanceRegistry;

        public override void InstallBindings()
        {
            BindUnitSelectionSystem();
            BindTargeting();
            BindUnitServices();
            BindUnitSpawning();
        }

        private void BindUnitSelectionSystem()
        {
            Container.Bind<UnitSelecting>().AsSingle();
            Container.Bind<SelectedUnits>().AsSingle();
            Container.BindInstance(_unitSelection);
            Container.BindInstance(_selectionInput);
            Container.BindInstance(_unitChoosing);
        }

        private void BindTargeting()
        {
            Container.BindInstance(_orderMarkPool);
            Container.BindInstance(_orderMarkPrefab).WhenInjectedInto<OrderMarkPool>();
            Container.BindInstance(_orderMarksParent).WhenInjectedInto<OrderMarkPool>();
        }

        private void BindUnitServices()
        {
            Container.BindInstance(_unitClassSpecsRepository);
            Container.BindInstance(_unitRepository);
            Container.BindInstance(_unitTypeCounts);
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

        private class UnitFacadePool : MonoPoolableMemoryPool<UnitType, Vector3, IMemoryPool, UnitFacade> { }
    }
}
