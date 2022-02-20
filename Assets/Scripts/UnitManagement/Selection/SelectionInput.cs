using System;
using System.Collections;
using UI.Game;
using UI.Menus.Primary;
using UnitManagement.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace UnitManagement.Selection
{
    public class SelectionInput : MonoBehaviour
    {
        private Vector2? _startPoint;
        private bool _updatingArea;

        private GameViews _gameViews;
        private GameMenuToggle _gameMenuToggle;

        private Coroutine _areaUpdateCouroutine;

        private PlayerInput _playerInput;

        private InputAction _selectAction;
        private InputAction _mousePositionAction;

        [Inject]
        public void Construct(PlayerInput playerInput, GameViews gameViews, GameMenuToggle gameMenuToggle)
        {
            _playerInput = playerInput;
            _gameViews = gameViews;
            _gameMenuToggle = gameMenuToggle;
        }

        private void Awake()
        {
            _selectAction = _playerInput.actions.FindAction("Select");
            _mousePositionAction = _playerInput.actions.FindAction("Mouse Position");
        }

        public bool Deactivated { get; set; }

        public event Action<Rect> Selecting;
        public event Action<Rect> SelectingEnd;

        private void OnEnable()
        {
            SubscribeToEvents();

            _gameMenuToggle.Pausing += UnsubscribeFromEvents;
            _gameMenuToggle.Resuming += SubscribeToEvents;
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();

            _gameMenuToggle.Pausing -= UnsubscribeFromEvents;
            _gameMenuToggle.Resuming -= SubscribeToEvents;
        }

        private void SubscribeToEvents()
        {
            _selectAction.started += StartArea;
            _selectAction.canceled += EndArea;
        }

        private void UnsubscribeFromEvents()
        {
            _selectAction.started -= StartArea;
            _selectAction.canceled -= EndArea;
        }

        private void StartArea(InputAction.CallbackContext context)
        {
            if (Keyboard.current.IsModifierKeyPressed() || _gameViews.MouseOverUi || Deactivated)
            {
                return;
            }

            _startPoint = _mousePositionAction.ReadValue<Vector2>();

            if (_areaUpdateCouroutine != null)
            {
                StopCoroutine(_areaUpdateCouroutine);
            }

            _areaUpdateCouroutine = StartCoroutine(UpdateArea());
        }

        private IEnumerator UpdateArea()
        {
            _updatingArea = true;

            while (true)
            {
                if (_startPoint == null)
                {
                    throw new InvalidOperationException();
                }

                Selecting?.Invoke(GetRect(_startPoint.Value, _mousePositionAction.ReadValue<Vector2>()));

                yield return null;
            }
        }

        private void EndArea(InputAction.CallbackContext context)
        {
            if (!_updatingArea)
            {
                return;
            }

            if (_startPoint == null || _areaUpdateCouroutine == null)
            {
                throw new InvalidOperationException();
            }

            SelectingEnd?.Invoke(GetRect(_startPoint.Value, _mousePositionAction.ReadValue<Vector2>()));

            _startPoint = null;
            _updatingArea = false;

            StopCoroutine(_areaUpdateCouroutine);
        }

        private Rect GetRect(Vector2 a, Vector2 b)
        {
            var minCorner = new Vector2(a.x < b.x ? a.x : b.x, a.y < b.y ? a.y : b.y);
            var maxCorner = new Vector2(a.x > b.x ? a.x : b.x, a.y > b.y ? a.y : b.y);
            var size = new Vector2(maxCorner.x - minCorner.x, maxCorner.y - minCorner.y);

            return new Rect(minCorner, size);
        }
    }
}
