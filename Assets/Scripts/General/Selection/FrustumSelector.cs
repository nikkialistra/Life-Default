using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using pointcache.Frustum;
using UnityEngine;

namespace General.Selection
{
    [RequireComponent(typeof(FrustumMeshCollider))]
    public class FrustumSelector : FrustumCamera
    {
        private readonly HashSet<Collider> _selectionHashSet = new();

        private FrustumMeshCollider _frustumMeshCollider;

        protected override void Awake() {
            base.Awake();

            _frustumMeshCollider = GetComponent<FrustumMeshCollider>();
            
            m_config.Active = false;
        }

        public event Action<List<Collider>> Selected; 

        private void Reset() {
            m_config.UseExtents = true;
            m_config.SplitMeshVerts = false;
        }

        public void Select(Rect rect)
        {
            _selectionHashSet.Clear();
            
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
            
            Selected?.Invoke(_selectionHashSet.ToList());
        }

        private void OnTriggerEnter(Collider other)
        {
            _selectionHashSet.Add(other);
        }
    }
}
