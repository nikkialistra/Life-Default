using System;
using System.Collections.Generic;
using ColonistManagement.Targeting.Formations.Region;
using Colonists;
using UnityEngine;

namespace ColonistManagement.Targeting.Formations
{
    public class FormationGeneration
    {
        private List<Colonist> _colonists = new();
        private List<Vector3> _colonistPositions = new();

        private Vector3[] _formationPositions;

        private Vector3 _targetPoint;
        private float _relativeYRotation;

        private readonly RegionFormation _regionFormation;

        public FormationGeneration(RegionFormation regionFormation)
        {
            _regionFormation = regionFormation;
        }

        public Vector3[] GenerateFormation(Vector3 targetPoint, List<Colonist> colonists,
            List<Vector3> colonistPositions, FormationType formationType)
        {
            if (colonists.Count == 0)
                return Array.Empty<Vector3>();

            _colonists = colonists;
            _colonistPositions = colonistPositions;

            return formationType switch
            {
                FormationType.Area => GenerateRegionFormation(targetPoint, RegionFormationType.Area),
                FormationType.Line => GenerateRegionFormation(targetPoint, RegionFormationType.Line),
                FormationType.Scattered => GenerateRegionFormation(targetPoint, RegionFormationType.Scattered),
                FormationType.Free => GenerateFreeFormation(targetPoint),
                FormationType.None => GenerateNoFormation(targetPoint),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public Vector3[] GenerateWithRotation(float rotation, List<Colonist> colonists, Vector3[] formationPositions,
            bool isRegion)
        {
            if (colonists.Count == 0)
                return Array.Empty<Vector3>();

            _formationPositions = formationPositions;

            return isRegion ? RotateRegionFormation(rotation) : RotateFreeFormation(rotation);
        }

        private Vector3[] GenerateRegionFormation(Vector3 targetPoint, RegionFormationType regionFormationType)
        {
            _targetPoint = targetPoint;

            var count = _colonists.Count;

            _relativeYRotation =
                FormationUtils.RotationFromOriginToTarget(FormationUtils.FindMiddlePoint(_colonistPositions),
                    targetPoint);

            var relativeRotation = Quaternion.Euler(0f, _relativeYRotation, 0f);

            return _regionFormation.CalculatePositions(count, relativeRotation, targetPoint,
                _targetPoint.y, regionFormationType);
        }

        private Vector3[] GenerateFreeFormation(Vector3 targetPoint)
        {
            var originPoint = FormationUtils.FindMiddlePoint(_colonistPositions);

            var difference = targetPoint - originPoint;

            var freeFormationPositions = new Vector3[_colonists.Count];
            for (int i = 0; i < _colonists.Count; i++)
            {
                var targetPosition = _colonists[i].transform.position + difference;
                freeFormationPositions[i] = new Vector3(
                    targetPosition.x, targetPoint.y, targetPosition.z
                );
            }

            return freeFormationPositions;
        }

        private Vector3[] GenerateNoFormation(Vector3 targetPoint)
        {
            var noFormationPositions = new Vector3[_colonists.Count];

            for (int i = 0; i < _colonists.Count; i++)
                noFormationPositions[i] = targetPoint;

            return noFormationPositions;
        }

        private Vector3[] RotateRegionFormation(float rotation)
        {
            var relativeRotation = Quaternion.Euler(0f, _relativeYRotation + rotation, 0f);

            return _regionFormation.RotatePositions(relativeRotation);
        }

        private Vector3[] RotateFreeFormation(float rotation)
        {
            var relativeRotation = Quaternion.Euler(0f, _relativeYRotation + rotation, 0f);
            var centerPoint = FormationUtils.FindCenterPoint(_formationPositions);

            var rotatedPositions = new Vector3[_formationPositions.Length];
            for (int i = 0; i < _formationPositions.Length; i++)
            {
                var targetUnitPositionRotated =
                    centerPoint + relativeRotation * (_formationPositions[i] - centerPoint);

                rotatedPositions[i] =
                    new Vector3(targetUnitPositionRotated.x, _targetPoint.y, targetUnitPositionRotated.z);
            }

            return rotatedPositions;
        }
    }
}
