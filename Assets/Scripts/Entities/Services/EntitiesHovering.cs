using System.Collections;
using ColonistManagement.Selection;
using Entities.Interfaces;
using UI.Menus.Primary;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Entities.Services
{
    public class EntitiesHovering : MonoBehaviour
    {
        [SerializeField] private float _rayRecastingTime = 0.05f;
        
        private bool _canHover = true;

        private Camera _camera;
        private GameMenuToggle _gameMenuToggle;
        
        private SelectionInput _selectionInput;
        
        private LayerMask _entitiesMask;
        
        private Coroutine _hoveringCoroutine;

        private PlayerInput _playerInput;

        private InputAction _mousePositionAction;
        private object _waitPeriod;

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
            _mousePositionAction = _playerInput.actions.FindAction("Mouse Position");
            _entitiesMask = LayerMask.GetMask("Colonists", "Enemies", "Buildings", "Resources");
        }

        private void OnEnable()
        {
            _gameMenuToggle.GamePause += OnGamePause;
            _gameMenuToggle.GameResume += OnGameResume;
            
            _selectionInput.Selecting += OnSelecting;
            _selectionInput.SelectingEnd += OnSelectingEnd;
        }

        private void OnDisable()
        {
            _gameMenuToggle.GamePause -= OnGamePause;
            _gameMenuToggle.GameResume -= OnGameResume;
            
            _selectionInput.Selecting -= OnSelecting;
            _selectionInput.SelectingEnd -= OnSelectingEnd;
        }

        private void Start()
        {
            _waitPeriod = new WaitForSeconds(_rayRecastingTime);
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

            var point = _mousePositionAction.ReadValue<Vector2>();

            var ray = _camera.ScreenPointToRay(point);

            if (Physics.Raycast(ray, out var hit, float.PositiveInfinity, _entitiesMask))
            {
                if (hit.transform.TryGetComponent(out IHoverable hoverable))
                {
                    hoverable.OnHover();
                }
            }
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
