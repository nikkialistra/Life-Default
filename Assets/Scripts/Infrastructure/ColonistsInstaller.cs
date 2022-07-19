using ColonistManagement.Movement;
using ColonistManagement.Tasking;
using Colonists;
using Colonists.Services;
using Controls;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class ColonistsInstaller : MonoInstaller
    {
        [Title("Actions")]
        [Required]
        [SerializeField] private ActionIconsRegistry _actionIconsRegistry;

        [Title("Spawning")]
        [Required]
        [SerializeField] private GameObject _colonistPrefab;
        [Required]
        [SerializeField] private Transform _colonistsParent;

        [Title("RayCasting")]
        [Required]
        [SerializeField] private RayCasting _rayCasting;

        [Title("Commands")]
        [Required]
        [SerializeField] private MovementCommand _movementCommand;
        [Required]
        [SerializeField] private MovementActionsInput _movementActionsInput;

        [Title("Services")]
        [Required]
        [SerializeField] private ColonistRepository _colonistRepository;

        public override void InstallBindings()
        {
            BindActions();
            BindSpawning();
            BindRayCasting();
            BindCommands();
            BindServices();
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

        private void BindRayCasting()
        {
            Container.BindInstance(_rayCasting);
        }

        private void BindCommands()
        {
            Container.BindInstance(_movementCommand);
            Container.BindInstance(_movementActionsInput);
        }

        private void BindServices()
        {
            Container.BindInstance(_colonistRepository);
        }
    }
}
