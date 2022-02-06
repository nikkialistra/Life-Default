using Units.Services.Selecting;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Testing
{
    public class FieldOfViewToggling : MonoBehaviour
    {
        private SelectedUnits _selectedUnits;

        private InputAction _toggleEnemyFieldsOfViewAction;
        private InputAction _toggleResourceFieldsOfViewAction;

        private PlayerInput _playerInput;

        [Inject]
        public void Construct(SelectedUnits selectedUnits, PlayerInput playerInput)
        {
            _playerInput = playerInput;
            _selectedUnits = selectedUnits;
        }

        private void Awake()
        {
            _toggleEnemyFieldsOfViewAction = _playerInput.actions.FindAction("ToggleEnemyFieldsOfView");
            _toggleResourceFieldsOfViewAction = _playerInput.actions.FindAction("ToggleResourceFieldsOfView");
        }

        private void OnEnable()
        {
            _toggleEnemyFieldsOfViewAction.started += ToggleEnemyFieldsOfView;
            _toggleResourceFieldsOfViewAction.started += ToggleResourceFieldsOfView;
        }

        private void OnDisable()
        {
            _toggleEnemyFieldsOfViewAction.started -= ToggleEnemyFieldsOfView;
            _toggleResourceFieldsOfViewAction.started -= ToggleResourceFieldsOfView;
        }

        private void ToggleEnemyFieldsOfView(InputAction.CallbackContext context)
        {
            foreach (var unit in _selectedUnits.Units)
            {
                unit.ToggleEnemyFieldOfView();
            }
        }

        private void ToggleResourceFieldsOfView(InputAction.CallbackContext context)
        {
            foreach (var unit in _selectedUnits.Units)
            {
                unit.ToggleResourceFieldOfView();
            }
        }
    }
}
