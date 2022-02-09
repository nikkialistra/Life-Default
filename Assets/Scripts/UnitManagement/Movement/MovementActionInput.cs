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

        private InputAction _selectMoveAction;
        private InputAction _selectAttackAction;
        private InputAction _selectHoldAction;
        private InputAction _selectPatrolAction;

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

            _selectMoveAction = _playerInput.actions.FindAction("Select Move");
            _selectAttackAction = _playerInput.actions.FindAction("Select Attack");
            _selectHoldAction = _playerInput.actions.FindAction("Select Hold");
            _selectPatrolAction = _playerInput.actions.FindAction("Select Patrol");

            _doAction = _playerInput.actions.FindAction("Do");
            _cancelAction = _playerInput.actions.FindAction("Cancel");
        }

        private void OnEnable()
        {
            _selectMoveAction.started += SelectMove;
            _selectAttackAction.started += SelectAttack;
            _selectHoldAction.started += SelectHold;
            _selectPatrolAction.started += SelectPatrol;

            _doAction.started += Do;
            _doAction.canceled += SetDestination;
            _doAction.canceled += AddDestination;

            _cancelAction.started += Cancel;
        }

        private void OnDisable()
        {
            _selectMoveAction.started -= SelectMove;
            _selectAttackAction.started -= SelectAttack;
            _selectHoldAction.started -= SelectHold;
            _selectPatrolAction.started -= SelectPatrol;

            _doAction.started -= Do;
            _doAction.canceled -= SetDestination;
            _doAction.canceled -= AddDestination;

            _cancelAction.started -= Cancel;
        }

        private void SelectMove(InputAction.CallbackContext context)
        {
            _movementAction = MovementAction.Move;
            PauseAnotherInput();
        }

        private void SelectAttack(InputAction.CallbackContext context)
        {
            _movementAction = MovementAction.Attack;
            PauseAnotherInput();
        }

        private void SelectHold(InputAction.CallbackContext context)
        {
            _movementAction = MovementAction.Hold;
            PauseAnotherInput();
        }

        private void SelectPatrol(InputAction.CallbackContext context)
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

            _movementInput.Move(context);
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
