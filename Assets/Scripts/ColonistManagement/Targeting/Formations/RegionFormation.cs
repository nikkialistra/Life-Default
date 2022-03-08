using System;
using UnityEngine;

namespace ColonistManagement.Targeting.Formations
{
    public class RegionFormation : MonoBehaviour
    {
        [SerializeField] private float _colonistSize = 1f;
        [Space]
        [SerializeField] private float _distanceMultiplierForPacked = 2f;
        [SerializeField] private float _distanceMultiplierForSparse = 3f;

        [SerializeField] private float _distanceForArrow = 1f;

        private float _distanceMultiplier;

        private int _behindFormation;
        private int _rowsCount;
        private int _columnsCount;
        private int _rowsTotalCount;
        private float _rowsOffset;

        private Vector3 _targetPointFlat;
        private Quaternion _rotation;
        private float _height;

        private float _leftIndent;

        private Vector3[] _areaFormationPositions;

        public float CurrentYRotation => _rotation.eulerAngles.y;

        public Vector3[] CalculatePositions(int count, Quaternion rotation, Vector3 targetPoint, float height,
            RegionFormationType regionFormationType)
        {
            InitializeParameters(count, rotation, targetPoint, height, regionFormationType);

            var lastIndex = CalculateFormationPositions();
            CalculateBehindFormationPositionsFrom(lastIndex);

            return RotateFormationPositions();
        }

        public Vector3[] RotatePositions(Quaternion rotation)
        {
            _rotation = rotation;

            return RotateFormationPositions();
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
                case RegionFormationType.Scattered:
                    InitializeForMultiRow(count);
                    _distanceMultiplier = _distanceMultiplierForSparse;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(regionFormationType), regionFormationType, null);
            }

            _rowsOffset = (float)_rowsTotalCount / 2 - 0.5f;
            _areaFormationPositions = new Vector3[count + 1];
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

        private int CalculateFormationPositions()
        {
            var currentPosition = 1;
            for (var i = 0; i < _rowsCount; i++)
            {
                _leftIndent = (-1) * ((float)_columnsCount / 2 - 0.5f);

                for (var j = 0; j < _columnsCount; j++)
                {
                    _areaFormationPositions[currentPosition] = GetUnitPositionInFormation(
                        _targetPointFlat, _rowsOffset, _leftIndent);

                    _leftIndent++;

                    currentPosition++;
                }

                _rowsOffset--;
            }

            _areaFormationPositions[0] = CalculateArrowPosition();

            return currentPosition;
        }

        private Vector3 CalculateArrowPosition()
        {
            var horizontalPosition = GetFirstRowMiddlePosition();
            var verticalPosition = IndentFromFirstPosition();

            return new Vector3(horizontalPosition, 0, verticalPosition);
        }

        private float GetFirstRowMiddlePosition()
        {
            if (_columnsCount % 2 == 1)
            {
                return _areaFormationPositions[_columnsCount / 2 + 1].x;
            }
            else
            {
                return (_areaFormationPositions[_columnsCount / 2].x +
                        _areaFormationPositions[_columnsCount / 2 + 1].x) / 2;
            }
        }

        private float IndentFromFirstPosition()
        {
            return _areaFormationPositions[1].z + _distanceForArrow;
        }

        private void CalculateBehindFormationPositionsFrom(int currentPosition)
        {
            if (_behindFormation > 0)
            {
                _leftIndent =
                    -1 * ((float)_behindFormation / 2 - 0.5f);

                for (var j = 0; j < _behindFormation; j++)
                {
                    _areaFormationPositions[currentPosition] = GetUnitPositionInFormation(
                        _targetPointFlat, _rowsOffset, _leftIndent
                    );

                    _leftIndent++;

                    currentPosition++;
                }
            }
        }

        private Vector3[] RotateFormationPositions()
        {
            var rotatedPositions = new Vector3[_areaFormationPositions.Length];
            for (var i = 0; i < _areaFormationPositions.Length; i++)
            {
                var targetUnitPositionRotated =
                    _targetPointFlat + _rotation * (_areaFormationPositions[i] - _targetPointFlat);

                rotatedPositions[i] =
                    new Vector3(targetUnitPositionRotated.x, _height, targetUnitPositionRotated.z);
            }

            return rotatedPositions;
        }

        private Vector3 GetUnitPositionInFormation(Vector3 targetPointFlat,
            float rowsOffset, float leftIndent)
        {
            var targetUnitPosition = new Vector3(
                targetPointFlat.x + leftIndent * _colonistSize * _distanceMultiplier,
                0f,
                targetPointFlat.z + rowsOffset * _colonistSize * _distanceMultiplier
            );

            return targetUnitPosition;
        }
    }
}
