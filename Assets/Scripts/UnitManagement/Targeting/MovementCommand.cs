using System.Collections.Generic;
using System.Linq;
using MapGeneration.Map;
using Pathfinding;
using Units.Services.Selecting;
using UnityEngine;
using Zenject;

namespace UnitManagement.Targeting
{
    public class MovementCommand : MonoBehaviour
    {
        private SelectedUnits _selectedUnits;
        private TargetPool _pool;

        private Map _map;
        
        private AstarPath _astarPath;
        private GridGraph _graph;

        [Inject]
        public void Construct(SelectedUnits selectedUnits, TargetPool pool, Map map, AstarPath astarPath)
        {
            _selectedUnits = selectedUnits;
            _pool = pool;
            _map = map;
            _astarPath = astarPath;
        }

        public bool CanAcceptCommand => _selectedUnits.Units.Any();

        private void OnEnable()
        {
            _map.Load += OnMapLoad;
        }

        private void OnDisable()
        {
            _map.Load -= OnMapLoad;
        }

        public void MoveAll(TargetObject targetObject, RaycastHit hit)
        {
            if (targetObject.HasDestinationPoint)
            {
                var destinationPoint = targetObject.GetDestinationPoint();
                var target = _pool.PlaceTo(destinationPoint, targetObject);
                MoveAllTo(target);
            }
            else
            {
                var target = _pool.PlaceTo(hit.point);
                MoveAllTo(target);
            }
        }

        private void OnMapLoad()
        {
            _graph = _astarPath.data.gridGraph;
        }

        private void MoveAllTo(Target target)
        {
            var targetables = GetTargetables().ToList();

            CalculateMovingPositions(targetables, target);
        }

        private IEnumerable<ITargetable> GetTargetables()
        {
            foreach (var unit in _selectedUnits.Units)
            {
                var targetable = unit.GetComponent<ITargetable>();
                if (targetable == null)
                {
                    continue;
                }

                yield return targetable;
            }
        }

        private void CalculateMovingPositions(List<ITargetable> targetables, Target target)
        {
            if (_graph == null)
            {
                return;
            }

            var positions = GetPossiblePositions(target, targetables.Count);

            foreach (var targetable in targetables)
            {
                if (targetable.AcceptTargetPoint(positions.Dequeue()))
                {
                    _pool.Link(target, targetable);
                }
            }
        }

        private Queue<Vector3> GetPossiblePositions(Target target, int count)
        {
            var node = _graph.GetNearest(target.transform.position).node;
            
            var positions = new Queue<Vector3>();
            
            positions.Enqueue((Vector3)node.position);

            node.GetConnections(otherNode =>
            {
                if (otherNode.Walkable)
                {
                    positions.Enqueue((Vector3)otherNode.position);
                }
            });

            return positions;
        }
    }
}