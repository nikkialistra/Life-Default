using Colonists.Services.Selecting;
using General;
using General.Selecting.Selected;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Testing
{
    public class FieldOfViewToggling : MonoBehaviour
    {
        private SelectedColonists _selectedColonists;
        private InteractableHovering _interactableHovering;

        private PlayerInput _playerInput;
        
        private InputAction _toggleEnemyFieldOfViewAction;
        private InputAction _toggleResourceFieldOfViewAction;

        [Inject]
        public void Construct(SelectedColonists selectedColonists, InteractableHovering interactableHovering, PlayerInput playerInput)
        {
            _selectedColonists = selectedColonists;
            _interactableHovering = interactableHovering;
            
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _toggleEnemyFieldOfViewAction = _playerInput.actions.FindAction("Toggle Enemy Field Of View");
            _toggleResourceFieldOfViewAction = _playerInput.actions.FindAction("Toggle Resource Field Of View");
        }

        private void OnEnable()
        {
            _toggleEnemyFieldOfViewAction.started += ToggleEnemyFieldOfView;
            _toggleResourceFieldOfViewAction.started += ToggleResourceFieldOfView;
        }

        private void OnDisable()
        {
            _toggleEnemyFieldOfViewAction.started -= ToggleEnemyFieldOfView;
            _toggleResourceFieldOfViewAction.started -= ToggleResourceFieldOfView;
        }

        private void ToggleEnemyFieldOfView(InputAction.CallbackContext context)
        {
            foreach (var colonist in _selectedColonists.Colonists)
            {
                colonist.ToggleUnitFieldOfView();
            }
        }

        private void ToggleResourceFieldOfView(InputAction.CallbackContext context)
        {
            foreach (var colonist in _selectedColonists.Colonists)
            {
                colonist.ToggleResourceFieldOfView();
            }
        }
    }
}
