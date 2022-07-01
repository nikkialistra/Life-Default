using System;
using System.Collections;
using UI.Game;
using UI.Menus.Primary;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace General.Selecting
{
    public class SelectingInput : MonoBehaviour
    {
        public event Action<Rect> Selecting;
        public event Action SelectingArea;
        public event Action<Rect> SelectingEnd;
        public event Action<Rect> SelectingEndAdditive;

        public bool Deactivated { get; set; }

        private Vector2? _startPoint;
        private bool _updatingArea;

        private GameViews _gameViews;
        private GameMenuToggle _gameMenuToggle;

        private Coroutine _areaUpdateCoroutine;

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


        private void OnEnable()
        {
            SubscribeToEvents();

            _gameMenuToggle.GamePause += UnsubscribeFromEvents;
            _gameMenuToggle.GameResume += SubscribeToEvents;
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();

            _gameMenuToggle.GamePause -= UnsubscribeFromEvents;
            _gameMenuToggle.GameResume -= SubscribeToEvents;
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
            if (Keyboard.current.altKey.isPressed || Keyboard.current.ctrlKey.isPressed || _gameViews.MouseOverUi ||
                Deactivated) return;

            _startPoint = _mousePositionAction.ReadValue<Vector2>();

            if (_areaUpdateCoroutine != null)
                StopCoroutine(_areaUpdateCoroutine);

            _areaUpdateCoroutine = StartCoroutine(UpdateArea());
        }

        private IEnumerator UpdateArea()
        {
            _updatingArea = true;

            while (true)
            {
                if (_startPoint == null)
                    throw new InvalidOperationException();

                var rect = GetRect(_startPoint.Value, _mousePositionAction.ReadValue<Vector2>());

                Selecting?.Invoke(rect);

                if (IsArea(rect))
                    SelectingArea?.Invoke();

                yield return null;
            }
        }

        private static bool IsArea(Rect rect)
        {
            return rect.width > 3f && rect.height > 3f;
        }

        private void EndArea(InputAction.CallbackContext context)
        {
            if (!_updatingArea) return;

            if (_startPoint == null || _areaUpdateCoroutine == null) throw new InvalidOperationException();

            if (Keyboard.current.shiftKey.isPressed)
                SelectingEndAdditive?.Invoke(GetRect(_startPoint.Value, _mousePositionAction.ReadValue<Vector2>()));
            else
                SelectingEnd?.Invoke(GetRect(_startPoint.Value, _mousePositionAction.ReadValue<Vector2>()));

            _startPoint = null;
            _updatingArea = false;

            StopCoroutine(_areaUpdateCoroutine);
            _areaUpdateCoroutine = null;
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
