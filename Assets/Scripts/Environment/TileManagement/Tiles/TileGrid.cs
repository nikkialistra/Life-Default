using System.Collections.Generic;
using MapGeneration.Map;
using Pathfinding;
using UI.Game.GameLook.Components;
using UnityEngine;
using Zenject;

namespace Environment.TileManagement.Tiles
{
    public class TileGrid : MonoBehaviour
    {
        private NavGraph _graph;

        private readonly List<Tile> _tiles = new();
        
        private AstarPath _astarPath;
        private Map _map;
        
        private TileInfoView _tileInfoView;

        [Inject]
        public void Construct(AstarPath astarPath, Map map, TileInfoView tileInfoView)
        {
            _astarPath = astarPath;
            _map = map;
            _tileInfoView = tileInfoView;
        }

        private void OnEnable()
        {
            _map.Load += Initialize;
        }

        private void OnDisable()
        {
            _map.Load -= Initialize;
        }

        public void ShowAtPosition(Vector2Int position)
        {
            foreach (var tile in _tiles)
            {
                if (tile.Position == position)
                {
                    _tileInfoView.ShowFor(tile);
                    break;
                }
            }
        }

        private void Initialize()
        {
            _graph = _astarPath.graphs[0];
            _graph.GetNodes(AddNode);
        }

        private void AddNode(GraphNode node)
        {
            var position = new Vector2Int(node.position.x / 1000, node.position.z / 1000);
            var tile = new Tile(position);
            _tiles.Add(tile);
        }
    }
}
