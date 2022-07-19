using General.Selecting.Selected;
using UI.Game;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace ColonistManagement.Movement
{
    [RequireComponent(typeof(MovementInput))]
    public class MoveActionInput : MonoBehaviour
    {
        private MovementInput _movementInput;

        private PlayerInput _playerInput;

        private InputAction _moveAction;

        [Inject]
        public void Construct(PlayerInput playerInput, RayCasting rayCasting, GameViews gameViews,
            SelectedColonists selectedColonists)
        {
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _movementInput = GetComponent<MovementInput>();

            _moveAction = _playerInput.actions.FindAction("Move");
        }

        private void OnEnable()
        {
            SubscribeToActions();
        }

        private void OnDisable()
        {
            UnsubscribeFromActions();
        }

        public void SubscribeToActions()
        {
            _moveAction.started += _movementInput.SetTarget;
            _moveAction.canceled += _movementInput.Move;
        }

        public void UnsubscribeFromActions()
        {
            _moveAction.started -= _movementInput.SetTarget;
            _moveAction.canceled -= _movementInput.Move;
        }
    }
}
