using Environment.TileManagement.Tiles;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Environment.TileManagement
{
    public class TileSelecting : MonoBehaviour
    {
        private TileGrid _tileGrid;
        
        private LayerMask _terrainMask;

        private Camera _camera;
        
        private PlayerInput _playerInput;

        private InputAction _selectTileAction;
        private InputAction _mousePositionAction;

        [Inject]
        public void Construct(TileGrid tileGrid, Camera camera, PlayerInput playerInput)
        {
            _tileGrid = tileGrid;
            _camera = camera;
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _terrainMask = LayerMask.GetMask("Terrain");
            
            _selectTileAction = _playerInput.actions.FindAction("Select Tile");
            
            _mousePositionAction = _playerInput.actions.FindAction("Mouse Position");
        }

        private void OnEnable()
        {
            _selectTileAction.started += SelectTile;
        }

        private void OnDisable()
        {
            _selectTileAction.started -= SelectTile;
        }

        private void SelectTile(InputAction.CallbackContext context)
        {
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
            return new Vector2Int(Mathf.RoundToInt(hitPoint.x), Mathf.RoundToInt(hitPoint.z));
        }
    }
}
