using System;
using System.Collections;
using Selecting.Controls;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Kernel.Selection
{
    public class SelectionInput : MonoBehaviour
    {
        public event Action<Rect> Selecting;
        public event Action<Rect> SelectingEnded;

        private Control _control;

        private Vector2? _startPoint;
        
        private Coroutine _areaUpdateCourotine;

        private void Awake() => _control = new Control();

        private void OnEnable()
        {
            _control.Enable();
            _control.Selection.Select.started += StartArea;
            _control.Selection.Select.canceled += EndArea;
        }
        
        private void OnDisable()
        {
            _control.Selection.Select.started -= StartArea;
            _control.Selection.Select.canceled -= EndArea;
            _control.Disable();
        }

        private void StartArea(InputAction.CallbackContext context)
        {
            if (Keyboard.current.ctrlKey.isPressed)
                return;
            
            _startPoint = _control.Selection.Position.ReadValue<Vector2>();
            
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

                Selecting?.Invoke(GetRect(_startPoint.Value, _control.Selection.Position.ReadValue<Vector2>()));

                yield return null;
            }
        }

        private void EndArea(InputAction.CallbackContext context)
        {
            if (_startPoint == null)
                return;
            
            if (_areaUpdateCourotine == null)
                throw new InvalidOperationException();

            SelectingEnded?.Invoke(GetRect(_startPoint.Value, _control.Selection.Position.ReadValue<Vector2>()));
            
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