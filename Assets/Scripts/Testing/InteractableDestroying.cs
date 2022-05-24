using General.Selecting.Selected;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Testing
{
    public class InteractableDestroying : MonoBehaviour
    {
        private SelectedColonists _selectedColonists;
        private SelectedEnemies _selectedEnemies;
        private SelectedEntities _selectedEntities;
        
        private PlayerInput _playerInput;

        private InputAction _selectAction;
        private InputAction _mousePositionAction;
        
        private InputAction _destroyInteractableAction;

        [Inject]
        public void Construct(SelectedColonists selectedColonists, SelectedEnemies selectedEnemies,
            SelectedEntities selectedEntities, PlayerInput playerInput)
        {
            _selectedColonists = selectedColonists;
            _selectedEnemies = selectedEnemies;
            _selectedEntities = selectedEntities;

            _playerInput = playerInput;
        }

        private void Awake()
        {
            _destroyInteractableAction = _playerInput.actions.FindAction("Destroy Interactable");
        }

        private void OnEnable()
        {
            _destroyInteractableAction.started += DestroyInteractable;
        }

        private void OnDisable()
        {
            _destroyInteractableAction.started -= DestroyInteractable;
        }

        private void DestroyInteractable(InputAction.CallbackContext context)
        {
            _selectedColonists.Destroy();
            _selectedEnemies.Destroy();
            _selectedEntities.Destroy();
        }
    }
}
