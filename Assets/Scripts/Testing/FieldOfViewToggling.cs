using Colonists.Services.Selecting;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Testing
{
    public class FieldOfViewToggling : MonoBehaviour
    {
        private SelectedColonists _selectedColonists;

        private InputAction _toggleEnemyFieldOfViewAction;
        private InputAction _toggleResourceFieldOfViewAction;

        private PlayerInput _playerInput;

        [Inject]
        public void Construct(SelectedColonists selectedColonists, PlayerInput playerInput)
        {
            _playerInput = playerInput;
            _selectedColonists = selectedColonists;
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
            foreach (var unit in _selectedColonists.Colonists)
            {
                unit.ToggleEnemyFieldOfView();
            }
        }

        private void ToggleResourceFieldOfView(InputAction.CallbackContext context)
        {
            foreach (var unit in _selectedColonists.Colonists)
            {
                unit.ToggleResourceFieldOfView();
            }
        }
    }
}
