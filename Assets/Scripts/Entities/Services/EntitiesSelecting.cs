using System.Collections;
using Entities.Interfaces;
using General.Selection;
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

        private Camera _camera;

        private LayerMask _entitiesMask;

        private ISelectableEntity _lastEntity;

        private Coroutine _hoveringCoroutine;

        private WaitForSecondsRealtime _waitPeriod;
        
        private bool _canSelect = true;

        private SelectionInput _selectionInput;
        private GameViews _gameViews;
        private GameMenuToggle _gameMenuToggle;

        private PlayerInput _playerInput;

        private InputAction _mousePositionAction;
        private InputAction _selectAction;

        [Inject]
        public void Construct(Camera camera, GameViews gameViews, SelectionInput selectionInput,
            GameMenuToggle gameMenuToggle, PlayerInput playerInput)
        {
            _camera = camera;

            _gameViews = gameViews;
            _selectionInput = selectionInput;
            _gameMenuToggle = gameMenuToggle;
            
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _entitiesMask = LayerMask.GetMask("Colonists", "Enemies", "Buildings", "Resources", "Items");
            
            _mousePositionAction = _playerInput.actions.FindAction("Mouse Position");
            _selectAction = _playerInput.actions.FindAction("Select");
        }

        private void OnEnable()
        {
            _gameMenuToggle.GamePause += StopHovering;
            _gameMenuToggle.GameResume += StartHovering;
            
            _selectionInput.SelectingArea += OnSelectingArea;
            _selectionInput.SelectingEnd += OnSelectingEnd;

            _selectAction.canceled += OnSelect;
        }

        private void OnDisable()
        {
            _gameMenuToggle.GamePause -= StopHovering;
            _gameMenuToggle.GameResume -= StartHovering;
            
            _selectionInput.SelectingArea -= OnSelectingArea;
            _selectionInput.SelectingEnd -= OnSelectingEnd;

            _selectAction.canceled -= OnSelect;
        }

        private void Start()
        {
            _waitPeriod = new WaitForSecondsRealtime(_hoverRecastingTime);
            StartHovering();
        }

        public void DeselectEntity()
        {
            _lastEntity?.Deselect();
            _lastEntity = null;
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
            StopHovering();
            BlockSelection();
        }

        private void OnSelectingEnd(Rect _)
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
            if (SpawnKeyIsPressed() || _gameViews.MouseOverUi || !_canSelect)
            {
                return;
            }

            DeselectEntity();
            
            if (Raycast(out var hit))
            {
                if (hit.transform.TryGetComponent(out ISelectableEntity entity))
                {
                    entity.Select();

                    _lastEntity = entity;
                }
            }
        }

        private static bool SpawnKeyIsPressed()
        {
            return Keyboard.current.altKey.isPressed || Keyboard.current.ctrlKey.isPressed;
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
