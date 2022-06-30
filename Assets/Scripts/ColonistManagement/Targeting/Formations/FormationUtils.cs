using System;
using System.Collections.Generic;
using Colonists;
using UnityEngine;

namespace ColonistManagement.Targeting.Formations
{
    public static class FormationUtils
    {
        public static Vector3 FindMiddlePoint(List<Vector3> positions)
        {
            if (positions.Count == 0) throw new InvalidOperationException();

            var minPositionX = positions[0].x;
            var maxPositionX = minPositionX;

            var minPositionZ = positions[0].z;
            var maxPositionZ = minPositionZ;

            foreach (var position in positions)
            {
                minPositionX = Mathf.Min(minPositionX, position.x);
                maxPositionX = Mathf.Max(maxPositionX, position.x);

                minPositionZ = Mathf.Min(minPositionZ, position.z);
                maxPositionZ = Mathf.Max(maxPositionZ, position.z);
            }

            var middlePoint = new Vector3(
                (minPositionX + maxPositionX) / 2, 0f, (minPositionZ + maxPositionZ) / 2);
            return middlePoint;
        }

        public static Vector3 FindCenterPoint(Vector3[] positions)
        {
            if (positions.Length == 0) throw new InvalidOperationException();

            var centerPoint = Vector3.zero;

            foreach (var position in positions)
                centerPoint += position;

            centerPoint /= positions.Length;

            return centerPoint;
        }

        public static int FarthestPointIndexFrom(Vector3[] formationPositions, bool[] assignedPositionsBitmask,
            Vector3 originPoint)
        {
            var farthestPointDistance = 0f;
            var farthestPointIndex = 0;

            for (int i = 0; i < assignedPositionsBitmask.Length; i++)
            {
                if (assignedPositionsBitmask[i]) continue;

                var distanceToPoint = Vector3.Distance(originPoint, formationPositions[i]);
                if (distanceToPoint > farthestPointDistance)
                {
                    farthestPointDistance = distanceToPoint;
                    farthestPointIndex = i;
                }
            }

            return farthestPointIndex;
        }

        public static int ClosestUnitIndexTo(List<Colonist> colonists, Vector3 targetPoint,
            bool[] assignedColonistsBitmask)
        {
            var closestUnitDistance = 1000f;
            var closestUnitIndex = 0;

            for (int i = 0; i < assignedColonistsBitmask.Length; i++)
            {
                if (assignedColonistsBitmask[i]) continue;

                var distanceToPoint = Vector3.Distance(colonists[i].transform.position, targetPoint);
                if (distanceToPoint < closestUnitDistance)
                {
                    closestUnitDistance = distanceToPoint;
                    closestUnitIndex = i;
                }
            }

            return closestUnitIndex;
        }

        public static float RotationFromOriginToTarget(Vector3 originPoint, Vector3 targetPoint)
        {
            var distToTarget = Vector3.Distance(originPoint, targetPoint);
            var differenceVector = targetPoint - originPoint;
            differenceVector.y = 0f;

            var angleBetween = Vector3.Angle(Vector3.forward * distToTarget, differenceVector);

            return targetPoint.x > originPoint.x ? angleBetween : 360f - angleBetween;
        }
    }
}
