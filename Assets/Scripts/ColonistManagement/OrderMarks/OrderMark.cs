using System;
using System.Collections;
using System.Collections.Generic;
using Colonists.Colonist;
using DG.Tweening;
using Entities;
using UnityEngine;

namespace ColonistManagement.OrderMarks
{
    public class OrderMark : MonoBehaviour
    {
        [SerializeField] private float _targetIndicatorFlashDuration = 0.8f;

        private readonly List<ColonistFacade> _colonists = new();

        private Coroutine _flashTargetIndicatorCoroutine;

        private bool _activated;

        public Entity Entity { get; set; }
        public bool Empty => _colonists.Count == 0;

        public void Add(ColonistFacade colonist)
        {
            _colonists.Add(colonist);

            colonist.DestinationReach += OnDestinationReach;

            UpdateState();
        }

        public void Remove(ColonistFacade colonist)
        {
            if (!_colonists.Contains(colonist))
            {
                return;
            }

            _colonists.Remove(colonist);

            UpdateState();

            colonist.DestinationReach -= OnDestinationReach;
        }

        public void Clear()
        {
            foreach (var colonist in _colonists)
            {
                colonist.DestinationReach -= OnDestinationReach;
            }

            _colonists.Clear();
        }

        private void Activate()
        {
            if (_activated)
            {
                return;
            }

            _activated = true;

            if (Entity != null)
            {
                _flashTargetIndicatorCoroutine = StartCoroutine(CollapseTargetIndicator(Entity.TargetIndicator));
            }
        }

        public void Deactivate()
        {
            if (_flashTargetIndicatorCoroutine != null)
            {
                StopCoroutine(_flashTargetIndicatorCoroutine);
            }

            if (Entity != null)
            {
                Entity.TargetIndicator.SetActive(false);
            }

            _activated = false;
        }

        private IEnumerator CollapseTargetIndicator(GameObject targetIndicator)
        {
            targetIndicator.transform.localScale = Vector3.one;
            targetIndicator.SetActive(true);

            targetIndicator.transform.DOKill();

            yield return targetIndicator.transform.DOScale(new Vector3(0f, 0f, 1f), _targetIndicatorFlashDuration)
                .WaitForCompletion();

            if (Entity.TargetIndicator != null)
            {
                Entity.TargetIndicator.SetActive(false);
            }
        }

        private void UpdateState()
        {
            if (!Empty)
            {
                Activate();
            }
            else
            {
                Deactivate();
            }
        }

        private void OnDestinationReach(ColonistFacade colonist)
        {
            if (!_colonists.Contains(colonist))
            {
                throw new InvalidOperationException();
            }

            colonist.DestinationReach -= OnDestinationReach;

            _colonists.Remove(colonist);

            UpdateState();
        }
    }
}
