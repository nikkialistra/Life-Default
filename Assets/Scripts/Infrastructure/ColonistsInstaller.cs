using ColonistManagement.Movement;
using ColonistManagement.OrderMarks;
using ColonistManagement.Selection;
using ColonistManagement.Tasking;
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
        
        [Title("Actions")]
        [Required]
        [SerializeField] private ActionIconsRegistry _actionIconsRegistry;

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
        
        [Title("Commands")]
        [Required]
        [SerializeField] private MovementCommand _movementCommand;
        [Required]
        [SerializeField] private MovementActionInput _movementActionInput;

        [Title("Services")]
        [Required]
        [SerializeField] private ColonistRepository _colonistRepository;

        public override void InstallBindings()
        {
            BindSelection();
            BindActions();
            BindOrdering();
            BindSpawning();
            BindCommands();
            BindServices();
        }

        private void BindSelection()
        {
            Container.Bind<ColonistSelecting>().AsSingle();
            Container.Bind<SelectedColonists>().AsSingle();
            Container.BindInstance(_colonistSelection);
            Container.BindInstance(_selectionInput);
            Container.BindInstance(_colonistChoosing);
        }

        private void BindActions()
        {
            Container.BindInstance(_actionIconsRegistry);
        }

        private void BindOrdering()
        {
            Container.BindInstance(_orderMarkPool);
            Container.BindInstance(_orderMarkPrefab).WhenInjectedInto<OrderMarkPool>();
            Container.BindInstance(_orderMarksParent).WhenInjectedInto<OrderMarkPool>();
        }

        private void BindSpawning()
        {
            Container.BindFactory<ColonistFacade, ColonistFacade.Factory>()
                .FromComponentInNewPrefab(_colonistPrefab)
                .UnderTransform(_colonistsParent);
        }

        private void BindCommands()
        {
            Container.BindInstance(_movementCommand);
            Container.BindInstance(_movementActionInput);
        }

        private void BindServices()
        {
            Container.BindInstance(_colonistRepository);
        }
    }
}
