using System;
using System.Collections.Generic;
using Units.Services.Selecting;
using Units.Unit;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Units.Services
{
    public class UnitControlGroups : MonoBehaviour
    {
        private const int ControlGroupNumber = 9;

        private SelectedUnits _selectedUnits;

        private readonly Dictionary<int, List<UnitFacade>> _controlGroups = new(ControlGroupNumber);

        private readonly List<Action<UnitFacade>> _removeFromControlGroup = new(ControlGroupNumber);

        private PlayerInput _playerInput;

        private InputAction _digitAction;

        [Inject]
        public void Construct(SelectedUnits selectedUnits, PlayerInput playerInput)
        {
            _selectedUnits = selectedUnits;
            _playerInput = playerInput;
        }

        private void Awake()
        {
            Initialize();

            _digitAction = _playerInput.actions.FindAction("Digit");
        }

        private void Initialize()
        {
            for (var i = 0; i < ControlGroupNumber; i++)
            {
                var number = i;

                _controlGroups.Add(number, new List<UnitFacade>());

                _removeFromControlGroup.Add((unit) => { _controlGroups[number].Remove(unit); });
            }
        }

        private void OnEnable()
        {
            _digitAction.started += OnDigitPress;
        }

        private void OnDisable()
        {
            _digitAction.started -= OnDigitPress;
        }

        private void OnDestroy()
        {
            foreach (var number in _controlGroups.Keys)
            {
                UnsubscribeFromUnits(number);
            }
        }

        private void OnDigitPress(InputAction.CallbackContext context)
        {
            var digit = Mathf.RoundToInt(context.ReadValue<float>());

            if (!Keyboard.current.ctrlKey.isPressed)
            {
                _selectedUnits.Set(_controlGroups[digit - 1]);
            }
            else
            {
                AddToControlGroup(digit - 1);
            }
        }

        private void AddToControlGroup(int number)
        {
            UnsubscribeFromUnits(number);

            _controlGroups[number] = _selectedUnits.Units;

            SubscribeToUnits(number);
        }

        private void UnsubscribeFromUnits(int number)
        {
            foreach (var oldUnit in _controlGroups[number])
            {
                oldUnit.UnitDie -= _removeFromControlGroup[number];
            }
        }

        private void SubscribeToUnits(int number)
        {
            foreach (var unit in _controlGroups[number])
            {
                unit.UnitDie += _removeFromControlGroup[number];
            }
        }
    }
}
