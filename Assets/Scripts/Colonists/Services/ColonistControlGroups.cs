using System;
using System.Collections.Generic;
using General.Selecting.Selected;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Colonists.Services
{
    public class ColonistControlGroups : MonoBehaviour
    {
        private const int ControlGroupNumber = 9;

        private SelectedColonists _selectedColonists;

        private readonly Dictionary<int, List<Colonist>> _controlGroups = new(ControlGroupNumber);

        private readonly List<Action<Colonist>> _removeFromControlGroup = new(ControlGroupNumber);

        private PlayerInput _playerInput;

        private InputAction _digitAction;

        [Inject]
        public void Construct(SelectedColonists selectedColonists, PlayerInput playerInput)
        {
            _selectedColonists = selectedColonists;
            _playerInput = playerInput;
        }

        private void Awake()
        {
            Initialize();

            _digitAction = _playerInput.actions.FindAction("Digit");
        }

        private void Initialize()
        {
            for (int i = 0; i < ControlGroupNumber; i++)
            {
                var number = i;

                _controlGroups.Add(number, new List<Colonist>());

                _removeFromControlGroup.Add(colonist => { _controlGroups[number].Remove(colonist); });
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
                UnsubscribeFromColonists(number);
        }

        private void OnDigitPress(InputAction.CallbackContext context)
        {
            var digit = Mathf.RoundToInt(context.ReadValue<float>());

            if (!Keyboard.current.ctrlKey.isPressed)
                _selectedColonists.Set(_controlGroups[digit - 1]);
            else
                AddToControlGroup(digit - 1);
        }

        private void AddToControlGroup(int number)
        {
            UnsubscribeFromColonists(number);

            _controlGroups[number] = _selectedColonists.Colonists;

            SubscribeToColonists(number);
        }

        private void SubscribeToColonists(int number)
        {
            foreach (var colonist in _controlGroups[number])
                colonist.ColonistDying += _removeFromControlGroup[number];
        }

        private void UnsubscribeFromColonists(int number)
        {
            foreach (var oldColonist in _controlGroups[number])
                oldColonist.ColonistDying -= _removeFromControlGroup[number];
        }
    }
}
