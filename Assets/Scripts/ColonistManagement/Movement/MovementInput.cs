using System;
using System.Collections;
using System.Linq;
using ColonistManagement.Targeting.Formations;
using Colonists;
using Entities;
using Entities.Types;
using General.Selecting.Selected;
using ResourceManagement;
using UI.Game;
using Units;
using Units.Enums;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace ColonistManagement.Movement
{
    [RequireComponent(typeof(GroundTargeting))]
    public class MovementInput : MonoBehaviour
    {
        public event Action<bool, FormationColor> DestinationSet;

        public event Action MultiCommandReset;

        public event Action<Colonist> ColonistSet;
        public event Action<Unit> UnitTargetSet;
        public event Action<Resource> ResourceSet;

        public event Action Stop;

        public bool CanTarget => _selectedColonists.Colonists.Any() && !_gameViews.MouseOverUi;

        public bool MultiCommand { get; private set; }

        private GroundTargeting _groundTargeting;

        private GameViews _gameViews;

        private SelectedColonists _selectedColonists;

        private bool _firstCommand;

        private Raycasting _raycasting;

        private PlayerInput _playerInput;

        private InputAction _multiCommandAction;
        private InputAction _moveAction;
        private InputAction _stopAction;

        [Inject]
        public void Construct(PlayerInput playerInput, Raycasting raycasting, GameViews gameViews,
            SelectedColonists selectedColonists)
        {
            _playerInput = playerInput;
            _raycasting = raycasting;
            _gameViews = gameViews;
            _selectedColonists = selectedColonists;
        }

        private void Awake()
        {
            _groundTargeting = GetComponent<GroundTargeting>();

            _multiCommandAction = _playerInput.actions.FindAction("Multi Command");

            _moveAction = _playerInput.actions.FindAction("Move");

            _stopAction = _playerInput.actions.FindAction("Stop");
        }

        private void OnEnable()
        {
            SubscribeToActions();

            _multiCommandAction.started += StartMultiCommand;
            _multiCommandAction.canceled += StopMultiCommand;

            _stopAction.started += OnStop;
        }

        private void OnDisable()
        {
            UnsubscribeFromActions();

            _multiCommandAction.started -= StartMultiCommand;
            _multiCommandAction.canceled -= StopMultiCommand;

            _stopAction.started -= OnStop;
        }

        public void SubscribeToActions()
        {
            _moveAction.started += SetTarget;
            _moveAction.canceled += Move;
        }

        public void UnsubscribeFromActions()
        {
            _moveAction.started -= SetTarget;
            _moveAction.canceled -= Move;
        }

        private void Move(InputAction.CallbackContext context)
        {
            Move(FormationColor.White);
        }

        public void Move(FormationColor formationColor)
        {
            if (_groundTargeting.TryFinishRotating())
            {
                var additional = MultiCommand && !_firstCommand;
                DestinationSet?.Invoke(additional, formationColor);

                _firstCommand = false;
            }
        }

        private void SetTarget(InputAction.CallbackContext context)
        {
            if (!CanTarget) return;

            if (TryTargetUnit() || TryTargetEntity()) return;

            _groundTargeting.Target(FormationColor.White);
        }

        private bool TryTargetUnit()
        {
            if (Physics.Raycast(_raycasting.GetRayFromMouse(), out var hit, Mathf.Infinity, _raycasting.RayMask))
            {
                var unit = hit.transform.GetComponentInParent<Unit>();
                if (unit != null)
                {
                    ChooseActionBasedOnUnitFraction(unit);

                    return true;
                }
            }

            return false;
        }

        private bool TryTargetEntity()
        {
            if (Physics.Raycast(_raycasting.GetRayFromMouse(), out var hit, Mathf.Infinity, _raycasting.RayMask))
            {
                var entity = hit.transform.GetComponentInParent<Entity>();
                if (entity != null)
                {
                    ChooseActionBasedOnUnitType(entity);

                    return true;
                }
            }

            return false;
        }

        private void ChooseActionBasedOnUnitFraction(Unit unit)
        {
            switch (unit.Faction)
            {
                case Faction.Colonists:
                    ColonistSet?.Invoke(unit.Colonist);
                    break;
                case Faction.Enemies:
                    UnitTargetSet?.Invoke(unit);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ChooseActionBasedOnUnitType(Entity entity)
        {
            switch (entity.EntityType)
            {
                case EntityType.Resource:
                    ResourceSet?.Invoke(entity.Resource);
                    break;
                case EntityType.Building:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void StartMultiCommand(InputAction.CallbackContext context)
        {
            MultiCommand = true;
            _firstCommand = true;
        }

        private void StopMultiCommand(InputAction.CallbackContext context)
        {
            MultiCommand = false;
            MultiCommandReset?.Invoke();
        }

        private void OnStop(InputAction.CallbackContext context)
        {
            Stop?.Invoke();
        }
    }
}
