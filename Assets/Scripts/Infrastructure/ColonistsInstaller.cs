using ColonistManagement.Movement;
using ColonistManagement.Tasking;
using Colonists;
using Colonists.Services;
using Colonists.Services.Selecting;
using General.Selection;
using General.Selection.Selected;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class ColonistsInstaller : MonoInstaller
    {
        [Title("Selection")]
        [Required]
        [SerializeField] private SelectionOperation _selectionOperation;
        [Required]
        [SerializeField] private SelectionInput _selectionInput;
        [Required]
        [SerializeField] private ColonistChoosing _colonistChoosing;
        
        [Title("Actions")]
        [Required]
        [SerializeField] private ActionIconsRegistry _actionIconsRegistry;

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
            BindSpawning();
            BindCommands();
            BindServices();
        }

        private void BindSelection()
        {
            Container.Bind<ObjectSelecting>().AsSingle();
            Container.Bind<SelectedColonists>().AsSingle();
            Container.BindInstance(_selectionOperation);
            Container.BindInstance(_selectionInput);
            Container.BindInstance(_colonistChoosing);
        }

        private void BindActions()
        {
            Container.BindInstance(_actionIconsRegistry);
        }
        
        private void BindSpawning()
        {
            Container.BindFactory<Colonist, Colonist.Factory>()
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
