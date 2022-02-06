using System;
using UnityEngine;

namespace UnitManagement.Targeting.Formations
{
    public class RegionFormation : MonoBehaviour
    {
        [SerializeField] private float _unitSize = 1f;
        [Space]
        [SerializeField] private float _distanceMultiplierForPacked = 2f;
        [SerializeField] private float _distanceMultiplierForSparse = 3f;

        private float _distanceMultiplier;

        private int _behindFormation;
        private int _rowsCount;
        private int _columnsCount;
        private int _rowsTotalCount;
        private Vector3 _targetPointFlat;

        private Quaternion _rotation;
        private float _height;

        private float _offsetUpDown;
        private float _offsetLeftRight;

        private Vector3[] _areaFormationPositions;

        public Vector3[] CalculatePositions(int count, Quaternion rotation, Vector3 targetPoint, float height,
            RegionFormationType regionFormationType)
        {
            InitializeParameters(count, rotation, targetPoint, height, regionFormationType);
            SetStartingRow();

            var lastIndex = CalculateFormationPositions();
            CalculateBehindFormationPositionsFrom(lastIndex);

            return _areaFormationPositions;
        }

        private void InitializeParameters(int count, Quaternion rotation, Vector3 targetPoint, float height,
            RegionFormationType regionFormationType)
        {
            _targetPointFlat = new Vector3(targetPoint.x, 0f, targetPoint.z);

            _rotation = rotation;
            _height = height;

            switch (regionFormationType)
            {
                case RegionFormationType.Area:
                    InitializeForMultiRow(count);
                    _distanceMultiplier = _distanceMultiplierForPacked;
                    break;
                case RegionFormationType.Line:
                    InitializeForOneRow(count);
                    _distanceMultiplier = _distanceMultiplierForPacked;
                    break;
                case RegionFormationType.Sparse:
                    InitializeForMultiRow(count);
                    _distanceMultiplier = _distanceMultiplierForSparse;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(regionFormationType), regionFormationType, null);
            }

            _areaFormationPositions = new Vector3[count];
        }

        private void InitializeForMultiRow(int count)
        {
            var sideOfSquare = (int)Mathf.Sqrt(count);
            var notInSquare = count - sideOfSquare * sideOfSquare;

            _behindFormation = notInSquare % sideOfSquare;

            _rowsCount = sideOfSquare;
            _columnsCount = sideOfSquare + notInSquare / sideOfSquare;

            _rowsTotalCount = _behindFormation > 0 ? _rowsCount + 1 : _rowsCount;
        }

        private void InitializeForOneRow(int count)
        {
            _rowsCount = 1;
            _rowsTotalCount = 1;

            _columnsCount = count;

            _behindFormation = 0;
        }

        private void SetStartingRow()
        {
            _offsetUpDown = (float)_rowsTotalCount / 2 - 0.5f;
        }

        private int CalculateFormationPositions()
        {
            var currentPosition = 0;
            for (var i = 0; i < _rowsCount; i++)
            {
                _offsetLeftRight = (-1) * ((float)_columnsCount / 2 - 0.5f);

                for (var j = 0; j < _columnsCount; j++)
                {
                    _areaFormationPositions[currentPosition] = GetUnitPositionInFormation(
                        _targetPointFlat, _rotation, _offsetUpDown, _offsetLeftRight);

                    _offsetLeftRight++;

                    currentPosition++;
                }

                _offsetUpDown--;
            }

            return currentPosition;
        }

        private void CalculateBehindFormationPositionsFrom(int currentPosition)
        {
            if (_behindFormation > 0)
            {
                _offsetLeftRight =
                    -1 * ((float)_behindFormation / 2 - 0.5f);

                for (var j = 0; j < _behindFormation; j++)
                {
                    _areaFormationPositions[currentPosition] = GetUnitPositionInFormation(
                        _targetPointFlat, _rotation, _offsetUpDown, _offsetLeftRight
                    );

                    _offsetLeftRight++;

                    currentPosition++;
                }
            }
        }

        private Vector3 GetUnitPositionInFormation(Vector3 targetPointFlat, Quaternion relativeRotation,
            float offsetUpDown, float offsetLeftRight)
        {
            var targetUnitPositionNotRotated = new Vector3(
                targetPointFlat.x + offsetLeftRight * _unitSize * _distanceMultiplier,
                0f,
                targetPointFlat.z + offsetUpDown * _unitSize * _distanceMultiplier
            );

            var targetUnitPositionRotated =
                targetPointFlat + relativeRotation * (targetUnitPositionNotRotated - targetPointFlat);

            return new Vector3(targetUnitPositionRotated.x, _height, targetUnitPositionRotated.z);
        }
    }
}
