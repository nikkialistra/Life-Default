using System.Collections;
using Entities.Interfaces;
using General.Selecting;
using UI.Game;
using UI.Menus.Primary;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Entities.Services
{
    public class InteractableHovering : MonoBehaviour
    {
        [SerializeField] private float _hoverRecastingTime = 0.03f;

        private Camera _camera;

        private LayerMask _entitiesMask;

        private ISelectableEntity _lastEntity;

        private Coroutine _hoveringCoroutine;

        private WaitForSecondsRealtime _waitPeriod;
        
        private bool _canSelect = true;

        private SelectingInput _selectingInput;
        private GameViews _gameViews;
        private GameMenuToggle _gameMenuToggle;

        private PlayerInput _playerInput;

        private InputAction _mousePositionAction;

        [Inject]
        public void Construct(Camera camera, GameViews gameViews, SelectingInput selectingInput,
            GameMenuToggle gameMenuToggle, PlayerInput playerInput)
        {
            _camera = camera;

            _gameViews = gameViews;
            _selectingInput = selectingInput;
            _gameMenuToggle = gameMenuToggle;
            
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _entitiesMask = LayerMask.GetMask("Colonists", "Enemies", "Buildings", "Resources", "Items");
            
            _mousePositionAction = _playerInput.actions.FindAction("Mouse Position");
        }

        private void OnEnable()
        {
            _gameMenuToggle.GamePause += StopHovering;
            _gameMenuToggle.GameResume += StartHovering;
            
            _selectingInput.SelectingArea += OnSelectingArea;
            _selectingInput.SelectingEnd += OnSelectingEnd;
        }

        private void OnDisable()
        {
            _gameMenuToggle.GamePause -= StopHovering;
            _gameMenuToggle.GameResume -= StartHovering;
            
            _selectingInput.SelectingArea -= OnSelectingArea;
            _selectingInput.SelectingEnd -= OnSelectingEnd;
        }

        private void Start()
        {
            _waitPeriod = new WaitForSecondsRealtime(_hoverRecastingTime);
            StartHovering();
        }
        
        private void StartHovering()
        {
            _hoveringCoroutine = StartCoroutine(Hovering());
        }

        private void StopHovering()
        {
            if (_hoveringCoroutine != null)
            {
                StopCoroutine(_hoveringCoroutine);
                _hoveringCoroutine = null;
            }
        }

        private void OnSelectingArea()
        {
            _canSelect = false;
        }

        private void OnSelectingEnd(Rect _)
        {
            StartCoroutine(UnblockSelectionAfter());
        }

        private IEnumerator UnblockSelectionAfter()
        {
            yield return null;
            _canSelect = true;
        }

        private IEnumerator Hovering()
        {
            while (true)
            {
                yield return _waitPeriod;
                Hover();
            }
        }

        private void Hover()
        {
            if (!_canSelect || _gameViews.MouseOverUi)
            {
                return;
            }

            if (Raycast(out var hit))
            {
                if (hit.transform.TryGetComponent(out ISelectableEntity selectable))
                {
                    selectable.Hover();
                }
            }
        }

        private bool Raycast(out RaycastHit hit)
        {
            var point = _mousePositionAction.ReadValue<Vector2>();
            var ray = _camera.ScreenPointToRay(point);

            return Physics.Raycast(ray, out hit, float.PositiveInfinity, _entitiesMask);
        }
    }
}
