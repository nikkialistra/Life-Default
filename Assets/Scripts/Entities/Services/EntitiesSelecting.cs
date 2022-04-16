using System.Collections;
using ColonistManagement.Selection;
using Entities.Interfaces;
using UI.Game;
using UI.Menus.Primary;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Entities.Services
{
    public class EntitiesSelecting : MonoBehaviour
    {
        [SerializeField] private float _hoverRecastingTime = 0.03f;
        
        private bool _canHover = true;

        private Camera _camera;

        private LayerMask _entitiesMask;

        private ISelectable _lastSelectable;

        private Coroutine _hoveringCoroutine;

        private WaitForSeconds _waitPeriod;
        
        private bool _canSelect = true;

        private ColonistSelectionInput _colonistSelectionInput;
        private GameViews _gameViews;
        private GameMenuToggle _gameMenuToggle;

        private PlayerInput _playerInput;

        private InputAction _mousePositionAction;
        private InputAction _selectAction;

        [Inject]
        public void Construct(Camera camera, GameViews gameViews, ColonistSelectionInput colonistSelectionInput,
            GameMenuToggle gameMenuToggle, PlayerInput playerInput)
        {
            _camera = camera;

            _gameViews = gameViews;
            _colonistSelectionInput = colonistSelectionInput;
            _gameMenuToggle = gameMenuToggle;
            
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _entitiesMask = LayerMask.GetMask("Colonists", "Enemies", "Buildings", "Resources");
            
            _mousePositionAction = _playerInput.actions.FindAction("Mouse Position");
            _selectAction = _playerInput.actions.FindAction("Select");
        }

        private void OnEnable()
        {
            _gameMenuToggle.GamePause += StopHovering;
            _gameMenuToggle.GameResume += StartHovering;
            
            _colonistSelectionInput.Selecting += OnColonistSelecting;
            _colonistSelectionInput.SelectingEnd += OnColonistSelectingEnd;

            _selectAction.canceled += OnSelect;
        }

        private void OnDisable()
        {
            _gameMenuToggle.GamePause -= StopHovering;
            _gameMenuToggle.GameResume -= StartHovering;
            
            _colonistSelectionInput.Selecting -= OnColonistSelecting;
            _colonistSelectionInput.SelectingEnd -= OnColonistSelectingEnd;

            _selectAction.canceled -= OnSelect;
        }

        private void Start()
        {
            _waitPeriod = new WaitForSeconds(_hoverRecastingTime);
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

        private void OnColonistSelecting(Rect _)
        {
            StopHovering();
            BlockSelection();
        }

        private void OnColonistSelectingEnd(Rect _)
        {
            StartHovering();
            StartCoroutine(UnblockSelectionAfter());
        }

        private void BlockSelection()
        {
            _canSelect = false;
        }

        private IEnumerator UnblockSelectionAfter()
        {
            yield return null;
            _canSelect = true;
        }

        private void OnSelect(InputAction.CallbackContext context)
        {
            if (_gameViews.MouseOverUi || !_canSelect)
            {
                return;
            }
            
            _lastSelectable?.Deselect();
            _lastSelectable = null;
            
            if (Raycast(out var hit))
            {
                if (hit.transform.TryGetComponent(out ISelectable selectable))
                {
                    selectable.Select();

                    _lastSelectable = selectable;
                }
            }
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
            if (_gameViews.MouseOverUi || !_canHover)
            {
                return;
            }

            if (Raycast(out var hit))
            {
                if (hit.transform.TryGetComponent(out ISelectable selectable))
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
