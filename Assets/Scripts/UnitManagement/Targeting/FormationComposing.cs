using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace UnitManagement.Targeting
{
    public class FormationComposing : MonoBehaviour
    {
        [SerializeField] private FormationType _formationType;
        
        private List<ITargetable> _targetables;
        private Target _target;
        
        private TargetPool _targetPool;

        [Inject]
        public void Construct(TargetPool targetPool)
        {
            _targetPool = targetPool;
        }

        public void MoveTo(List<ITargetable> targetables, Target target)
        {
            _targetables = targetables;
            _target = target;

            CalculateUnitPositions(GenerateFormation(target.transform.position));
        }

        private void CalculateUnitPositions(Vector3[] formationPositions)
        {
            var assignedSpots = new bool[formationPositions.Length];
            var assignedUnits = new bool[formationPositions.Length];

            var currentMiddlePoint = FindMiddlePointBetweenUnits();

            for(var i = 0; i < formationPositions.Length; i++)
            {
                var farthestSpotIndex = _formationType == FormationType.Free
                    ? FarthestSpotIndexFrom(formationPositions, assignedSpots, currentMiddlePoint)
                    : i;

                var closestUnitIndex = ClosestUnitIndexTo(formationPositions[farthestSpotIndex], assignedUnits);

                assignedSpots[farthestSpotIndex] = true;
                assignedUnits[closestUnitIndex] = true;
                

                var accepted = _targetables[closestUnitIndex].AcceptTargetPoint(formationPositions[farthestSpotIndex]);
                if (accepted)
                {
                    _targetPool.Link(_target, _targetables[closestUnitIndex]);
                }
            }
        }

        private static int FarthestSpotIndexFrom(Vector3[] formationPositions, bool[] assignedSpots, Vector3 relativeSpot)
        {
            var farthestSpotDistance = 0f;
            var farthestSpotIndex = 0;
            for(var i = 0; i < assignedSpots.Length; i++)
            {
                if (assignedSpots[i])
                {
                    continue;
                }

                var distanceToSpot = Vector3.Distance(relativeSpot, formationPositions[i]);
                if(distanceToSpot > farthestSpotDistance)
                {
                    farthestSpotDistance = distanceToSpot;
                    farthestSpotIndex = i;
                }
            }
            return farthestSpotIndex;
        }

        private int ClosestUnitIndexTo(Vector3 targetSpot, bool[] assignedUnits)
        {
            var closestUnitDistance = 1000f;
            float distanceToSpot;
            var closestUnitIndex = 0;
            for(var i = 0; i < assignedUnits.Length; i++)
            {
                if (assignedUnits[i])
                {
                    continue;
                }

                distanceToSpot = Vector3.Distance(_targetables[i].Position, targetSpot);
                if (distanceToSpot < closestUnitDistance)
                {
                    closestUnitDistance = distanceToSpot;
                    closestUnitIndex = i;
                }
            }
            return closestUnitIndex;
        }

        private Vector3[] GenerateFormation(Vector3 targetPoint)
        {
            if (_targetables.Count == 0)
            {
                return Array.Empty<Vector3>();
            }
            
            return _formationType switch
            {
                FormationType.Free => GenerateFreeFormation(targetPoint),
                FormationType.Facing => GenerateFacingFormation(targetPoint),
                FormationType.Square => GenerateSquareFormation(targetPoint),
                FormationType.Diamond => GenerateDiamondFormation(targetPoint),
                FormationType.None => GenerateNoFormation(targetPoint),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private Vector3[] GenerateFreeFormation(Vector3 targetPoint)
        {
            var originPoint = FindMiddlePointBetweenUnits();
            
            var difference = targetPoint - originPoint;

            var freeFormationPositions = new Vector3[_targetables.Count];
            for(var i = 0; i < _targetables.Count; i++)
            {
                var targetUnitPosition = _targetables[i].Position + difference;
                freeFormationPositions[i] = new Vector3(
                    targetUnitPosition.x, targetPoint.y, targetUnitPosition.z
                );
            }

            return freeFormationPositions;
        }

        private Vector3[] GenerateFacingFormation(Vector3 targetPoint)
        {
            throw new NotImplementedException();
        }

        private Vector3[] GenerateSquareFormation(Vector3 targetPoint)
        {
            throw new NotImplementedException();
        }

        private Vector3[] GenerateDiamondFormation(Vector3 targetPoint)
        {
            throw new NotImplementedException();
        }

        private Vector3[] GenerateNoFormation(Vector3 targetPoint)
        {
            var noFormationPositions = new Vector3[_targetables.Count];
            for (var i = 0; i < _targetables.Count; i++)
            {
                noFormationPositions[i] = targetPoint;
            }
            return noFormationPositions;
        }

        private Vector3 FindMiddlePointBetweenUnits()
        {
            var minPositionX = _targetables[0].Position.x;
            var maxPositionX = minPositionX;

            var minPositionZ = _targetables[0].Position.z;
            var maxPositionZ = minPositionZ;

            foreach (var unit in _targetables)
            {
                minPositionX = Mathf.Min(minPositionX, unit.Position.x);
                maxPositionX = Mathf.Max(maxPositionX, unit.Position.x);

                minPositionZ = Mathf.Min(minPositionZ, unit.Position.z);
                maxPositionZ = Mathf.Max(maxPositionZ, unit.Position.z);
            }

            var middleSpot = new Vector3(
                (minPositionX + maxPositionX) / 2, 0f, (minPositionZ + maxPositionZ) / 2);
            return middleSpot;
        }
    }
}