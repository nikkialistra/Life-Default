using Selecting.Selected;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Testing
{
    public class FieldOfViewToggling : MonoBehaviour
    {
        private SelectedColonists _selectedColonists;
        private SelectedAborigines _selectedAborigines;

        private PlayerInput _playerInput;

        private InputAction _toggleUnitFieldOfViewAction;
        private InputAction _toggleResourceFieldOfViewAction;

        [Inject]
        public void Construct(SelectedColonists selectedColonists, SelectedAborigines selectedAborigines,
            PlayerInput playerInput)
        {
            _selectedColonists = selectedColonists;
            _selectedAborigines = selectedAborigines;

            _playerInput = playerInput;
        }

        private void Awake()
        {
            _toggleUnitFieldOfViewAction = _playerInput.actions.FindAction("Toggle Unit Field Of View");
            _toggleResourceFieldOfViewAction = _playerInput.actions.FindAction("Toggle Resource Field Of View");
        }

        private void OnEnable()
        {
            _toggleUnitFieldOfViewAction.started += ToggleUnitFieldOfView;
            _toggleResourceFieldOfViewAction.started += ToggleResourceFieldOfView;
        }

        private void OnDisable()
        {
            _toggleUnitFieldOfViewAction.started -= ToggleUnitFieldOfView;
            _toggleResourceFieldOfViewAction.started -= ToggleResourceFieldOfView;
        }

        private void ToggleUnitFieldOfView(InputAction.CallbackContext context)
        {
            foreach (var colonist in _selectedColonists.Colonists)
                colonist.ToggleUnitFieldOfView();

            foreach (var aborigine in _selectedAborigines.Aborigines)
                aborigine.ToggleUnitFieldOfView();
        }

        private void ToggleResourceFieldOfView(InputAction.CallbackContext context)
        {
            foreach (var colonist in _selectedColonists.Colonists)
                colonist.ToggleResourceFieldOfView();
        }
    }
}
