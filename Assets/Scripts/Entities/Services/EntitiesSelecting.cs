using System.Collections;
using ColonistManagement.Selection;
using Entities.Interfaces;
using UI.Menus.Primary;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Entities.Services
{
    public class EntitiesSelecting : MonoBehaviour
    {
        [SerializeField] private float _hoverRecastingTime = 0.05f;
        
        private bool _canHover = true;

        private Camera _camera;
        private GameMenuToggle _gameMenuToggle;
        
        private SelectionInput _selectionInput;
        
        private LayerMask _entitiesMask;
        
        private ISelectable _lastSelectable;
        
        private Coroutine _hoveringCoroutine;

        private WaitForSeconds _waitPeriod;
        
        private PlayerInput _playerInput;

        private InputAction _mousePositionAction;
        private InputAction _selectAction;

        [Inject]
        public void Construct(Camera camera, GameMenuToggle gameMenuToggle, SelectionInput selectionInput, PlayerInput playerInput)
        {
            _camera = camera;
            _gameMenuToggle = gameMenuToggle;
            
            _selectionInput = selectionInput;
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
            _gameMenuToggle.GamePause += OnGamePause;
            _gameMenuToggle.GameResume += OnGameResume;
            
            _selectionInput.Selecting += OnSelecting;
            _selectionInput.SelectingEnd += OnSelectingEnd;

            _selectAction.canceled += OnSelect;
        }

        private void OnDisable()
        {
            _gameMenuToggle.GamePause -= OnGamePause;
            _gameMenuToggle.GameResume -= OnGameResume;
            
            _selectionInput.Selecting -= OnSelecting;
            _selectionInput.SelectingEnd -= OnSelectingEnd;

            _selectAction.canceled -= OnSelect;
        }

        private void Start()
        {
            _waitPeriod = new WaitForSeconds(_hoverRecastingTime);
            StartHovering();
        }

        private void OnGamePause()
        {
            StopHovering();
        }

        private void OnGameResume()
        {
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
            }
        }

        private void OnSelect(InputAction.CallbackContext context)
        {
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
            if (!_canHover)
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

        private void OnSelecting(Rect _)
        {
            _canHover = false;
        }

        private void OnSelectingEnd(Rect _)
        {
            _canHover = true;
        }
    }
}
