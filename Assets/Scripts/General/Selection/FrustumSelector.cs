using System;
using System.Collections;
using pointcache.Frustum;
using UnityEngine;

namespace General.Selection
{
    [RequireComponent(typeof(FrustumMeshCollider))]
    public class FrustumSelector : FrustumCamera
    {
        private FrustumMeshCollider _frustumMeshCollider;
        
        private void Reset() {
            m_config.UseExtents = true;
            m_config.SplitMeshVerts = false;
        }

        protected override void Awake() {
            base.Awake();

            _frustumMeshCollider = GetComponent<FrustumMeshCollider>();
            
            m_config.Active = false;
        }

        public void Select(Rect rect)
        {
            var leftBottomAngle = new Vector2(rect.xMin / Screen.width, rect.yMin / Screen.height);
            var rightUpAngle = new Vector2(rect.xMax / Screen.width, rect.yMax / Screen.height);

            frustumConfig.ExtentsMin = leftBottomAngle;
            frustumConfig.ExtentsMax = rightUpAngle;

            StartCoroutine(ActivateFrustumForOneFrame());
        }

        private IEnumerator ActivateFrustumForOneFrame()
        {
            _frustumMeshCollider.DoGenerate = true;
            m_config.Active = true;

            yield return null;

            m_config.Active = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            
        }
    }
}
