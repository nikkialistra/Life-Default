using System.Collections.Generic;
using MapGeneration.Map;
using Pathfinding;
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

        [Inject]
        public void Construct(AstarPath astarPath, Map map)
        {
            _astarPath = astarPath;
            _map = map;
        }

        private void OnEnable()
        {
            _map.Load += Initialize;
        }

        private void OnDisable()
        {
            _map.Load -= Initialize;
        }

        public void ShowAtCoordinate(Vector3 coordinate)
        {
            var position = new Int3(coordinate);

            foreach (var tile in _tiles)
            {
                if (tile.Position.Equals(position))
                {
                    Debug.Log(tile);
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
            var tile = new Tile(node.position);
            _tiles.Add(tile);
        }
    }
}
