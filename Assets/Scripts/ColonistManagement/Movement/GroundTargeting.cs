﻿using System;
using System.Collections;
using ColonistManagement.Targeting;
using ColonistManagement.Targeting.Formations;
using UnityEngine;
using Zenject;

namespace ColonistManagement.Movement
{
    public class GroundTargeting : MonoBehaviour
    {
        public event Action<Vector3, FormationColor> PositionSet;
        public event Action<float> RotationUpdate;

        [SerializeField] private float _mouseOffsetForRotationUpdate = 0.5f;

        private bool _isPositionRotating;

        private Vector2 _rotationDirection;
        private Vector2 _perpendicularDirection;

        private Coroutine _positionRotatingCoroutine;

        private Raycasting _raycasting;

        [Inject]
        public void Construct(Raycasting raycasting)
        {
            _raycasting = raycasting;
        }

        public void Target(FormationColor formationColor)
        {
            if (Physics.Raycast(_raycasting.GetRayFromMouse(), out var hit, Mathf.Infinity, _raycasting.RayMask))
            {
                var ground = hit.transform.GetComponentInParent<Ground>();
                if (ground != null)
                {
                    PositionSet?.Invoke(hit.point, formationColor);
                    _isPositionRotating = true;
                    _positionRotatingCoroutine = StartCoroutine(CPositionRotating(hit.point));
                }
            }
        }

        public bool TryFinishRotating()
        {
            if (_isPositionRotating)
            {
                if (_positionRotatingCoroutine != null)
                {
                    StopCoroutine(_positionRotatingCoroutine);
                    _positionRotatingCoroutine = null;
                }

                _isPositionRotating = false;

                return true;
            }

            return false;
        }

        private IEnumerator CPositionRotating(Vector3 position)
        {
            while (GotSufficientMouseOffset(position) == false)
                yield return null;

            while (true)
            {
                UpdateAngle(position);
                yield return null;
            }
        }

        private bool GotSufficientMouseOffset(Vector3 position)
        {
            if (Physics.Raycast(_raycasting.GetRayFromMouse(), out var hit, Mathf.Infinity, _raycasting.TerrainMask))
            {
                var direction = hit.point - position;
                var planeDirection = new Vector2(direction.x, direction.z);

                if (planeDirection.magnitude > _mouseOffsetForRotationUpdate)
                {
                    _rotationDirection = planeDirection;
                    _perpendicularDirection = Vector2.Perpendicular(_rotationDirection);
                    return true;
                }
            }

            return false;
        }

        private void UpdateAngle(Vector3 position)
        {
            if (Physics.Raycast(_raycasting.GetRayFromMouse(), out var hit, Mathf.Infinity, _raycasting.TerrainMask))
            {
                var direction = hit.point - position;
                var planeDirection = new Vector2(direction.x, direction.z);

                var angle = CalculateAngle(_rotationDirection, planeDirection, _perpendicularDirection);

                RotationUpdate?.Invoke(angle);
            }
        }

        private static float CalculateAngle(Vector3 from, Vector3 to, Vector3 left)
        {
            var angle = Vector3.Angle(from, to);
            return (Vector3.Angle(left, to) > 90f) ? angle : 360 - angle;
        }
    }
}
