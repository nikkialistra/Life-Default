﻿using System;
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
        private readonly HashSet<Collider> _selectionHashSet = new();

        private FrustumMeshCollider _frustumMeshCollider;
        
        private bool _additive;

        protected override void Awake() {
            base.Awake();

            _frustumMeshCollider = GetComponent<FrustumMeshCollider>();
            
            m_config.Active = false;
        }

        public event Action<List<Collider>> Selected; 
        public event Action<List<Collider>> AdditiveSelected; 

        private void Reset() {
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

        private void ActivateFrustum(Rect rect)
        {
            _selectionHashSet.Clear();

            var leftBottomAngle = new Vector2(rect.xMin / Screen.width, rect.yMin / Screen.height);
            var rightUpAngle = new Vector2(rect.xMax / Screen.width, rect.yMax / Screen.height);

            frustumConfig.ExtentsMin = leftBottomAngle;
            frustumConfig.ExtentsMax = rightUpAngle;

            StartCoroutine(FlashFrustum());
        }

        private IEnumerator FlashFrustum()
        {
            _frustumMeshCollider.DoGenerate = true;
            m_config.Active = true;

            // OnTriggerEnter catches all colliders only within 3 frames
            yield return null;
            yield return null;
            yield return null;

            m_config.Active = false;

            if (_selectionHashSet.Count == 0)
            {
                yield break;
            }

            if (_additive)
            {
                Selected?.Invoke(_selectionHashSet.ToList());
            }
            else
            {
                AdditiveSelected?.Invoke(_selectionHashSet.ToList());
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            _selectionHashSet.Add(other);
        }
    }
}
