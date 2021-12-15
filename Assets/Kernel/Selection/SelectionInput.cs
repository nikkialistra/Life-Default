using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Kernel.Selection
{
    public class SelectionInput : MonoBehaviour
    {
        public event Action<Rect> Selecting;
        public event Action<Rect> SelectingEnd;
        
        private Vector2? _startPoint;
        
        private Coroutine _areaUpdateCourotine;
        
        private PlayerInput _playerInput;
        
        private InputAction _selectAction;
        private InputAction _positionAction;

        [Inject]
        public void Construct(PlayerInput playerInput)
        {
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _selectAction = _playerInput.actions.FindAction("Select");
            _positionAction = _playerInput.actions.FindAction("Position");
        }

        private void OnEnable()
        {
            _selectAction.started += StartArea;
            _selectAction.canceled += EndArea;
        }
        
        private void OnDisable()
        {
            _selectAction.started -= StartArea;
            _selectAction.canceled -= EndArea;
        }

        private void StartArea(InputAction.CallbackContext context)
        {
            if (Keyboard.current.ctrlKey.isPressed)
            {
                return;
            }

            _startPoint = _positionAction.ReadValue<Vector2>();
            
            if (_areaUpdateCourotine != null)
                StopCoroutine(_areaUpdateCourotine);
            _areaUpdateCourotine = StartCoroutine(UpdateArea());
        }

        private IEnumerator UpdateArea()
        {
            while (true)
            {
                if (_startPoint == null)
                    throw new InvalidOperationException();

                Selecting?.Invoke(GetRect(_startPoint.Value, _positionAction.ReadValue<Vector2>()));

                yield return null;
            }
        }

        private void EndArea(InputAction.CallbackContext context)
        {
            if (Keyboard.current.ctrlKey.isPressed)
            {
                return;
            }
            
            if (_startPoint == null || _areaUpdateCourotine == null)
            {
                throw new InvalidOperationException();
            }

            SelectingEnd?.Invoke(GetRect(_startPoint.Value, _positionAction.ReadValue<Vector2>()));
            
            _startPoint = null;

            StopCoroutine(_areaUpdateCourotine);
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