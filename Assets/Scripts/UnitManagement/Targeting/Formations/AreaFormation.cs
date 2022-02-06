using UnityEngine;

namespace UnitManagement.Targeting.Formations
{
    public class AreaFormation : MonoBehaviour
    {
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
