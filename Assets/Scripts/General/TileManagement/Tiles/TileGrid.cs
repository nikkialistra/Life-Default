using General.Map;
using Pathfinding;
using UI.Game.GameLook.Components;
using UnityEngine;
using Zenject;

namespace General.TileManagement.Tiles
{
    public class TileGrid : MonoBehaviour
    {
        private Tile[] _tiles;

        private int _width;
        
        private int _xIndexOffset;
        private int _yIndexOffset;

        private Vector2Int _lastPosition;
        
        private AstarPath _astarPath;
        private MapInitialization _mapInitialization;

        private TileInfoView _tileInfoView;

        [Inject]
        public void Construct(AstarPath astarPath, MapInitialization mapInitialization, TileInfoView tileInfoView)
        {
            _astarPath = astarPath;
            _mapInitialization = mapInitialization;
            _tileInfoView = tileInfoView;
        }

        private void OnEnable()
        {
            _mapInitialization.Load += Initialize;
        }

        private void OnDisable()
        {
            _mapInitialization.Load -= Initialize;
        }

        public void ShowAtPosition(Vector2Int position)
        {
            if (PositionNotChanged(position))
            {
                return;
            }

            if (PositionIsOutOfBounds(position))
            {
                _tileInfoView.Hide();
                return;
            }
            
            var index = GetIndex(position);
            var tile = _tiles[index];
            
            _tileInfoView.ShowFor(tile);
        }

        private bool PositionNotChanged(Vector2Int position)
        {
            if (position == _lastPosition)
            {
                return true;
            }

            _lastPosition = position;
            return false;
        }

        private bool PositionIsOutOfBounds(Vector2Int position)
        {
            return position.x < -_xIndexOffset || position.x > _xIndexOffset - 1 || 
                   position.y < -_yIndexOffset || position.y > _yIndexOffset - 1;
        }

        private void Initialize()
        {
            var graph = _astarPath.data.gridGraph;

            _width = graph.width;
            
            _xIndexOffset = graph.width / 2 - (int)graph.center.x;
            _yIndexOffset = graph.depth / 2 - (int)graph.center.z;

            _tiles = new Tile[graph.width * graph.depth];

            graph.GetNodes(AddNode);
        }

        private void AddNode(GraphNode node)
        {
            var position = GetNodeLeftTopCorner(node);
            var tile = new Tile(position);

            var index = GetIndex(position);
            _tiles[index] = tile;
        }

        private static Vector2Int GetNodeLeftTopCorner(GraphNode node)
        {
            return new Vector2Int((node.position.x - 500) / 1000, (node.position.z - 500) / 1000);
        }

        private int GetIndex(Vector2Int position)
        {
            var xIndex = position.x + _xIndexOffset;
            var yIndex = position.y + _yIndexOffset;

            return yIndex * _width + xIndex;
        }
    }
}
