using System;
using UI;
using UnitManagement.Selection;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace UnitManagement.Movement
{
    [RequireComponent(typeof(MovementInput))]
    public class MovementActionInput : MonoBehaviour
    {
        private MovementAction _movementAction = MovementAction.None;

        private MovementInput _movementInput;

        private PlayerInput _playerInput;
        private SelectionInput _selectionInput;

        private GameCursors _gameCursors;

        private InputAction _moveAction;
        private InputAction _attackAction;
        private InputAction _holdAction;
        private InputAction _patrolAction;

        private InputAction _doAction;
        private InputAction _cancelAction;

        [Inject]
        public void Construct(PlayerInput playerInput, SelectionInput selectionInput, GameCursors gameCursors)
        {
            _playerInput = playerInput;
            _selectionInput = selectionInput;
            _gameCursors = gameCursors;
        }

        private void Awake()
        {
            _movementInput = GetComponent<MovementInput>();

            _moveAction = _playerInput.actions.FindAction("Move");
            _attackAction = _playerInput.actions.FindAction("Attack");
            _holdAction = _playerInput.actions.FindAction("Hold");
            _patrolAction = _playerInput.actions.FindAction("Patrol");

            _doAction = _playerInput.actions.FindAction("Do");
            _cancelAction = _playerInput.actions.FindAction("Cancel");
        }

        private void OnEnable()
        {
            _moveAction.started += Move;
            _attackAction.started += Attack;
            _holdAction.started += Hold;
            _patrolAction.started += Patrol;

            _doAction.started += Do;
            _doAction.canceled += SetDestination;
            _doAction.canceled += AddDestination;

            _cancelAction.started += Cancel;
        }

        private void OnDisable()
        {
            _moveAction.started -= Move;
            _attackAction.started -= Attack;
            _holdAction.started -= Hold;
            _patrolAction.started -= Patrol;

            _doAction.started -= Do;
            _doAction.canceled -= SetDestination;
            _doAction.canceled -= AddDestination;

            _cancelAction.started -= Cancel;
        }

        private void Move(InputAction.CallbackContext context)
        {
            _movementAction = MovementAction.Move;
            PauseAnotherInput();
        }

        private void Attack(InputAction.CallbackContext context)
        {
            _movementAction = MovementAction.Attack;
            PauseAnotherInput();
        }

        private void Hold(InputAction.CallbackContext context)
        {
            _movementAction = MovementAction.Hold;
            PauseAnotherInput();
        }

        private void Patrol(InputAction.CallbackContext context)
        {
            _movementAction = MovementAction.Patrol;
            PauseAnotherInput();
        }

        private void PauseAnotherInput()
        {
            _selectionInput.Deactivated = true;
            _movementInput.UnsubscribeFromActions();

            _gameCursors.SetTargetingCursor();
        }

        private void Do(InputAction.CallbackContext context)
        {
            if (_movementAction == MovementAction.None || !_movementInput.CanTarget)
            {
                return;
            }

            switch (_movementAction)
            {
                case MovementAction.Move:
                    _movementInput.TargetGround();
                    break;
                case MovementAction.Attack:
                    _movementInput.TargetEntity();
                    break;
                case MovementAction.Hold:
                    break;
                case MovementAction.Patrol:
                    break;
                case MovementAction.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Cancel(InputAction.CallbackContext context)
        {
            Complete();
        }

        private void SetDestination(InputAction.CallbackContext context)
        {
            if (Keyboard.current.shiftKey.isPressed)
            {
                return;
            }

            if (_movementAction == MovementAction.None)
            {
                return;
            }

            _movementInput.SetDestination(context);
            Complete();
        }

        private void Complete()
        {
            _movementAction = MovementAction.None;
            ResumeAnotherInput();
        }

        private void AddDestination(InputAction.CallbackContext context)
        {
            if (_movementAction == MovementAction.None)
            {
                return;
            }

            _movementInput.AddDestination(context);
        }

        private void ResumeAnotherInput()
        {
            _selectionInput.Deactivated = false;
            _movementInput.SubscribeToActions();
            _gameCursors.SetDefaultCursor();
        }
    }
}
