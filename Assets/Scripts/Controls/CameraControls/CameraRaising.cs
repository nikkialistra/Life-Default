using System;
using Infrastructure.Settings;
using UnityEngine;
using Zenject;

namespace Controls.CameraControls
{
    public class CameraRaising : MonoBehaviour
    {
        private LayerMask _terrainMask;

        private Vector3 _raycastToTerrainCorrection;

        private float _heightAboveTerrain;

        [Inject]
        public void Construct(RaycastingSettings raycastingSettings)
        {
            _raycastToTerrainCorrection = raycastingSettings.RaycastToTerrainCorrection;
        }

        private void Awake()
        {
            _terrainMask = LayerMask.GetMask("Terrain");
        }

        public Vector3 RaiseAboveTerrain(Vector3 position)
        {
            if (Physics.Raycast(new Ray(position + _raycastToTerrainCorrection, Vector3.down), out var hit,
                100f, _terrainMask))
                position.y = hit.point.y + _heightAboveTerrain;

            return position;
        }

        private void Start()
        {
            _heightAboveTerrain = GetDistanceAboveTerrain();
        }

        private float GetDistanceAboveTerrain()
        {
            if (Physics.Raycast(new Ray(transform.position + _raycastToTerrainCorrection, Vector3.down), out var hit,
                100f, _terrainMask))
                return hit.distance;
            else
                throw new InvalidOperationException("Camera is not above terrain");
        }
    }
}
