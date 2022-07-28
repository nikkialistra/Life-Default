using System;
using System.Linq;
using ColonistManagement.Targeting.Formations;
using ColonistManagement.Targeting.Formations.Preview;
using Colonists;
using Controls;
using Entities;
using Entities.Types;
using ResourceManagement;
using Selecting.Selected;
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

        public bool MultiCommand { get; private set; }

        public bool CanTarget => _selectedColonists.Colonists.Any() && !_gameViews.MouseOverUi;

        private GroundTargeting _groundTargeting;

        private GameViews _gameViews;

        private SelectedColonists _selectedColonists;

        private bool _firstCommand;

        private RayCasting _rayCasting;

        private PlayerInput _playerInput;

        private InputAction _multiCommandAction;
        private InputAction _stopAction;

        [Inject]
        public void Construct(PlayerInput playerInput, RayCasting rayCasting, GameViews gameViews,
            SelectedColonists selectedColonists)
        {
            _playerInput = playerInput;
            _rayCasting = rayCasting;
            _gameViews = gameViews;
            _selectedColonists = selectedColonists;
        }

        private void Awake()
        {
            _groundTargeting = GetComponent<GroundTargeting>();

            _multiCommandAction = _playerInput.actions.FindAction("Multi Command");

            _stopAction = _playerInput.actions.FindAction("Stop");
        }

        private void OnEnable()
        {
            _multiCommandAction.started += StartMultiCommand;
            _multiCommandAction.canceled += StopMultiCommand;

            _stopAction.started += OnStop;
        }

        private void OnDisable()
        {
            _multiCommandAction.started -= StartMultiCommand;
            _multiCommandAction.canceled -= StopMultiCommand;

            _stopAction.started -= OnStop;
        }

        public void Move(InputAction.CallbackContext context)
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

        public void SetTarget(InputAction.CallbackContext context)
        {
            if (!CanTarget) return;

            if (TryTargetUnit() || TryTargetEntity()) return;

            _groundTargeting.Target(FormationColor.White);
        }

        private bool TryTargetUnit()
        {
            if (Physics.Raycast(_rayCasting.GetRayFromMouse(), out var hit, Mathf.Infinity, _rayCasting.UnitsMask))
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
            if (Physics.Raycast(_rayCasting.GetRayFromMouse(), out var hit, Mathf.Infinity, _rayCasting.EntitiesMask))
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
                case Faction.Aborigines:
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
