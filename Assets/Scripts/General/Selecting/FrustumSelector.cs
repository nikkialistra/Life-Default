using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using pointcache.Frustum;
using UnityEngine;

namespace General.Selecting
{
    [RequireComponent(typeof(FrustumMeshCollider))]
    public class FrustumSelector : FrustumCamera
    {
        public event Action<List<Collider>> Selected;
        public event Action<List<Collider>> AdditiveSelected;

        [SerializeField] private Vector2 _extentsMinForAreaSelection = new(0.2f, 0.2f);
        [SerializeField] private Vector2 _extentsMaxForAreaSelection = new(0.8f, 0.8f);

        private readonly HashSet<Collider> _selectionHashSet = new();

        private FrustumMeshCollider _frustumMeshCollider;

        private bool _additive;

        protected override void Awake()
        {
            base.Awake();

            _frustumMeshCollider = GetComponent<FrustumMeshCollider>();

            m_config.Active = false;
        }

        private void Reset()
        {
            m_config.UseExtents = true;
            m_config.SplitMeshVerts = false;
        }

        public void Select(Rect rect)
        {
            _additive = false;
            ActivateFrustum(rect);
        }

        public void SelectAdditive(Rect rect)
        {
            _additive = true;
            ActivateFrustum(rect);
        }

        public void SelectFromAreaAround()
        {
            _additive = true;
            ActivateFrustumForAreaSelection();
        }

        private void ActivateFrustum(Rect rect)
        {
            _selectionHashSet.Clear();
            GenerateFrustumFromRect(rect);
            StartCoroutine(FlashFrustum());
        }

        private void ActivateFrustumForAreaSelection()
        {
            _selectionHashSet.Clear();
            GenerateFrustumForAreaSelection();
            StartCoroutine(FlashFrustum());
        }

        private void GenerateFrustumFromRect(Rect rect)
        {
            var leftBottomAngle = new Vector2(rect.xMin / Screen.width, rect.yMin / Screen.height);
            var rightUpAngle = new Vector2(rect.xMax / Screen.width, rect.yMax / Screen.height);

            frustumConfig.ExtentsMin = leftBottomAngle;
            frustumConfig.ExtentsMax = rightUpAngle;
        }

        private void GenerateFrustumForAreaSelection()
        {
            frustumConfig.ExtentsMin = _extentsMinForAreaSelection;
            frustumConfig.ExtentsMax = _extentsMaxForAreaSelection;
        }

        private IEnumerator FlashFrustum()
        {
            _frustumMeshCollider.DoGenerate = true;
            m_config.Active = true;

            yield return new WaitForFixedUpdate();

            m_config.Active = false;

            if (_additive)
                AdditiveSelected?.Invoke(_selectionHashSet.ToList());
            else
                Selected?.Invoke(_selectionHashSet.ToList());
        }

        private void OnTriggerEnter(Collider other)
        {
            _selectionHashSet.Add(other);
        }
    }
}
