using UI.Game;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace UnitManagement.Targeting
{
    [RequireComponent(typeof(MovementCommand))]
    public class MovementInput : MonoBehaviour
    {
        private Camera _camera;
        
        private GameViews _gameViews;
        
        private MovementCommand _movementCommand;

        private PlayerInput _playerInput;

        private InputAction _setTargetAction;
        private InputAction _positionAction;

        [Inject]
        public void Construct(PlayerInput playerInput, Camera camera, GameViews gameViews)
        {
            _playerInput = playerInput;
            _camera = camera;
            _gameViews = gameViews;
        }

        private void Awake()
        {
            _movementCommand = GetComponent<MovementCommand>();
            
            _setTargetAction = _playerInput.actions.FindAction("SetTarget");
            _positionAction = _playerInput.actions.FindAction("Position");
        }

        private void OnEnable()
        {
            _setTargetAction.started += SetTarget;
        }

        private void OnDisable()
        {
            _setTargetAction.started -= SetTarget;
        }

        private void SetTarget(InputAction.CallbackContext context)
        {
            if (!_movementCommand.CanAcceptCommand || _gameViews.MouseOverUi)
            {
                return;
            }

            var ray = GetRay();
            if (Physics.Raycast(ray, out var hit))
            {
                var targetObject = hit.transform.GetComponentInParent<TargetObject>();
                if (targetObject != null)
                {
                    _movementCommand.MoveAll(targetObject, hit);
                }
            }
        }

        private Ray GetRay()
        {
            var mousePosition = _positionAction.ReadValue<Vector2>();
            var ray = _camera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, _camera.nearClipPlane));
            return ray;
        }
    }
}