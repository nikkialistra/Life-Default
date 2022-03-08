using ColonistManagement.OrderMarks;
using ColonistManagement.Selection;
using Colonists.Colonist;
using Colonists.Services;
using Colonists.Services.Selecting;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class ColonistsInstaller : MonoInstaller
    {
        [Title("Selection")]
        [Required]
        [SerializeField] private ColonistSelection _colonistSelection;
        [Required]
        [SerializeField] private SelectionInput _selectionInput;
        [Required]
        [SerializeField] private ColonistChoosing _colonistChoosing;

        [Title("Ordering")]
        [Required]
        [SerializeField] private OrderMarkPool _orderMarkPool;
        [Required]
        [SerializeField] private OrderMark _orderMarkPrefab;
        [Required]
        [SerializeField] private Transform _orderMarksParent;

        [Title("Spawning")]
        [Required]
        [SerializeField] private GameObject _colonistPrefab;
        [Required]
        [SerializeField] private Transform _colonistsParent;

        [Title("Services")]
        [Required]
        [SerializeField] private ColonistRepository _colonistRepository;

        public override void InstallBindings()
        {
            BindUnitSelectionSystem();
            BindTargeting();
            BindColonistServices();
            BindUnitSpawning();
        }

        private void BindUnitSelectionSystem()
        {
            Container.Bind<ColonistSelecting>().AsSingle();
            Container.Bind<SelectedColonists>().AsSingle();
            Container.BindInstance(_colonistSelection);
            Container.BindInstance(_selectionInput);
            Container.BindInstance(_colonistChoosing);
        }

        private void BindTargeting()
        {
            Container.BindInstance(_orderMarkPool);
            Container.BindInstance(_orderMarkPrefab).WhenInjectedInto<OrderMarkPool>();
            Container.BindInstance(_orderMarksParent).WhenInjectedInto<OrderMarkPool>();
        }

        private void BindColonistServices()
        {
            Container.BindInstance(_colonistRepository);
        }

        private void BindUnitSpawning()
        {
            Container.BindFactory<Vector3, ColonistFacade, ColonistFacade.Factory>()
                .FromComponentInNewPrefab(_colonistPrefab)
                .UnderTransform(_colonistsParent);
        }
    }
}
