using UnityEngine;

namespace UnitManagement.Targeting
{
    public class AreaFormation : MonoBehaviour
    {
        [SerializeField] private bool _startFromMiddleRow;
        [SerializeField] private bool _startFromMiddleColumn;

        [Space]
        [SerializeField] private float _distanceMultiplier = 2f;
        [SerializeField] private float _unitSize = 1f;

        private int _sideOfSquare;
        private int _notInSquare;
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

        public Vector3[] CalculatePositions(int count, Quaternion rotation, Vector3 targetPoint, float height)
        {
            Initialize(count, rotation, targetPoint, height);

            _areaFormationPositions = new Vector3[count];

            SetStartingRow();

            var lastIndex = CalculateFormationPositions();
            CalculateBehindFormationPositionsFrom(lastIndex);

            return _areaFormationPositions;
        }

        private void SetStartingRow()
        {
            if (_startFromMiddleRow)
            {
                _offsetUpDown = _rowsCount % 2 == 1 ? 0f : -0.5f;
            }
            else
            {
                _offsetUpDown = (float)_rowsTotalCount / 2 - 0.5f;
            }
        }

        private int CalculateFormationPositions()
        {
            var currentPosition = 0;
            for (var i = 0; i < _rowsCount; i++)
            {
                if (_startFromMiddleColumn)
                {
                    _offsetLeftRight = _columnsCount % 2 == 1 ? 0f : -0.5f;
                }
                else
                {
                    _offsetLeftRight = (-1) * ((float)_columnsCount / 2 - 0.5f);
                }

                for (var j = 0; j < _columnsCount; j++)
                {
                    _areaFormationPositions[currentPosition] = GetUnitPositionInFormation(
                        _targetPointFlat, _rotation, _offsetUpDown, _offsetLeftRight);

                    if (_startFromMiddleColumn)
                    {
                        _offsetLeftRight = _offsetLeftRight < 0f ? -_offsetLeftRight : -_offsetLeftRight - 1f;
                    }
                    else
                    {
                        _offsetLeftRight++;
                    }

                    currentPosition++;
                }

                if (_startFromMiddleRow)
                {
                    _offsetUpDown = _offsetUpDown < 0f ? -_offsetUpDown : -_offsetUpDown - 1f;
                }
                else
                {
                    _offsetUpDown--;
                }
            }

            return currentPosition;
        }

        private void CalculateBehindFormationPositionsFrom(int currentPosition)
        {
            if (_behindFormation > 0)
            {
                if (_startFromMiddleRow)
                {
                    _offsetUpDown = (-1) * ((float)_rowsTotalCount / 2 - 0.5f);
                }

                if (_startFromMiddleColumn)
                {
                    _offsetLeftRight =
                        _behindFormation % 2 == 1 ? 0f : -0.5f;
                }
                else
                {
                    _offsetLeftRight =
                        -1 * ((float)_behindFormation / 2 - 0.5f);
                }

                for (var j = 0; j < _behindFormation; j++)
                {
                    _areaFormationPositions[currentPosition] = GetUnitPositionInFormation(
                        _targetPointFlat, _rotation, _offsetUpDown, _offsetLeftRight
                    );

                    if (_startFromMiddleColumn)
                    {
                        _offsetLeftRight = _offsetLeftRight < 0f ? -_offsetLeftRight : -_offsetLeftRight - 1f;
                    }
                    else
                    {
                        _offsetLeftRight++;
                    }

                    currentPosition++;
                }
            }
        }

        private void Initialize(int count, Quaternion rotation, Vector3 targetPoint, float height)
        {
            _sideOfSquare = (int)Mathf.Sqrt(count);
            _notInSquare = count - _sideOfSquare * _sideOfSquare;
            _behindFormation = _notInSquare % _sideOfSquare;

            _rowsCount = _sideOfSquare;
            _columnsCount = _sideOfSquare + _notInSquare / _sideOfSquare;

            _rowsTotalCount = _behindFormation > 0 ? _rowsCount + 1 : _rowsCount;

            _targetPointFlat = new Vector3(targetPoint.x, 0f, targetPoint.z);

            _rotation = rotation;
            _height = height;
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