using System;
using UnityEngine;

namespace CompositionRoot.Settings
{
    [Serializable]
    public class RaycastingSettings
    {
        public Vector3 RaycastToTerrainCorrection = Vector3.up * 10f;
    }
}
