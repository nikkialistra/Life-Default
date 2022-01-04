using System;
using MapGeneration.Data;
using UnityEngine;

namespace MapGeneration
{
    [Serializable]
    public struct LODInfo
    {
        [Range(0, MeshSettings.NumSupportedLODs - 1)]
        [SerializeField] private int _lod;
        [SerializeField] private float _visibleDistanceThreshold;

        public int Lod => _lod;

        public float VisibleDistanceThreshold => _visibleDistanceThreshold;

        public float SqrVisibleDstThreshold => _visibleDistanceThreshold * _visibleDistanceThreshold;
    }
}