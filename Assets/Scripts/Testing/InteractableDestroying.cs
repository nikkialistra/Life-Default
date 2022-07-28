using Selecting.Selected;
using Selecting.Selected.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Testing
{
    public class InteractableDestroying : MonoBehaviour
    {
        private SelectedColonists _selectedColonists;
        private SelectedAborigines _selectedAborigines;
        private SelectedEntities _selectedEntities;

        private PlayerInput _playerInput;

        private InputAction _selectAction;
        private InputAction _mousePositionAction;

        private InputAction _destroyInteractableAction;

        [Inject]
        public void Construct(SelectedColonists selectedColonists, SelectedAborigines selectedAborigines,
            SelectedEntities selectedEntities, PlayerInput playerInput)
        {
            _selectedColonists = selectedColonists;
            _selectedAborigines = selectedAborigines;
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
            _selectedAborigines.Destroy();
            _selectedEntities.Destroy();
        }
    }
}
