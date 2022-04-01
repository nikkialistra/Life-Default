using System;
using System.Collections;
using General.TileManagement.Tiles;
using UI.Menus.Primary;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace General.TileManagement
{
    public class TileSelecting : MonoBehaviour
    {
        [SerializeField] private float _rayRecastingTime = 0.15f;

        private TileGrid _tileGrid;
        
        private LayerMask _terrainMask;

        private Camera _camera;
        
        private Guid _managementMapId;

        private Coroutine _selectingTilesCoroutine;
        private WaitForSeconds _waitPeriod;
        
        private GameMenuToggle _gameMenuToggle;

        private PlayerInput _playerInput;

        private InputAction _mousePositionAction;

        [Inject]
        public void Construct(TileGrid tileGrid, Camera camera, GameMenuToggle gameMenuToggle, PlayerInput playerInput)
        {
            _tileGrid = tileGrid;
            _camera = camera;
            _gameMenuToggle = gameMenuToggle;
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _terrainMask = LayerMask.GetMask("Terrain");

            _mousePositionAction = _playerInput.actions.FindAction("Mouse Position");

            _managementMapId = _playerInput.currentActionMap.id;
        }

        private void OnEnable()
        {
            _gameMenuToggle.GamePause += OnGamePause;
            _gameMenuToggle.GameResume += OnGameResume;
        }

        private void OnDisable()
        {
            _gameMenuToggle.GamePause -= OnGamePause;
            _gameMenuToggle.GameResume -= OnGameResume;
        }

        private void OnGamePause()
        {
            StopSelectingTiles();
        }

        private void OnGameResume()
        {
            StartSelectingTiles();
        }

        private void Start()
        {
            _waitPeriod = new WaitForSeconds(_rayRecastingTime);
            StartSelectingTiles();
        }

        private void StartSelectingTiles()
        {
            _selectingTilesCoroutine = StartCoroutine(SelectingTiles());
        }

        private void StopSelectingTiles()
        {
            if (_selectingTilesCoroutine != null)
            {
                StopCoroutine(_selectingTilesCoroutine);
            }
        }

        private IEnumerator SelectingTiles()
        {
            while (true)
            {
                yield return _waitPeriod;
                SelectTile();
            }
        }

        private void SelectTile()
        {
            if (_playerInput.currentActionMap.id != _managementMapId)
            {
                return;
            }
            
            var position = _mousePositionAction.ReadValue<Vector2>();

            var ray = _camera.ScreenPointToRay(new Vector3(position.x, position.y, _camera.nearClipPlane));
            if (Physics.Raycast(ray, out var hit, float.PositiveInfinity, _terrainMask))
            {
                var tilePosition = ConvertToTilePosition(hit.point);
                _tileGrid.ShowAtPosition(tilePosition);
            }
        }

        private static Vector2Int ConvertToTilePosition(Vector3 hitPoint)
        {
            return new Vector2Int(Mathf.FloorToInt(hitPoint.x), Mathf.FloorToInt(hitPoint.z));
        }
    }
}
