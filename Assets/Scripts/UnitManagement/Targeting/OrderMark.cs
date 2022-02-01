﻿using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Entities.Entity;
using Units.Unit;
using UnityEngine;

namespace UnitManagement.Targeting
{
    public class OrderMark : MonoBehaviour
    {
        [SerializeField] private GameObject _targetIndicator;
        [SerializeField] private float _targetIndicatorFlashDuration = 0.8f;

        private readonly List<UnitFacade> _units = new();

        private Coroutine _flashTargetIndicatorCoroutine;

        public Entity Entity { get; private set; }
        public bool Empty => _units.Count == 0;

        public void Add(UnitFacade unit)
        {
            _units.Add(unit);

            unit.DestinationReach += OnDestinationReach;

            UpdateState();
        }

        public void Remove(UnitFacade unit)
        {
            if (!_units.Contains(unit))
            {
                return;
            }

            _units.Remove(unit);

            UpdateState();

            unit.DestinationReach -= OnDestinationReach;
        }

        public void Clear()
        {
            foreach (var unit in _units)
            {
                unit.DestinationReach -= OnDestinationReach;
            }

            _units.Clear();
        }

        public void SetEntity(Entity entity)
        {
            Entity = entity;
        }

        public void ClearEntity()
        {
            Entity = null;
        }

        private void Activate()
        {
            var targetIndicator = Entity != null ? Entity.TargetIndicator : _targetIndicator;

            _flashTargetIndicatorCoroutine = StartCoroutine(FlashTargetIndicator(targetIndicator));
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
            else
            {
                _targetIndicator.SetActive(false);
            }
        }

        private IEnumerator FlashTargetIndicator(GameObject targetIndicator)
        {
            targetIndicator.transform.localScale = Vector3.one;
            targetIndicator.SetActive(true);

            targetIndicator.transform.DOKill();

            yield return targetIndicator.transform.DOScale(new Vector3(0f, 0f, 1f), _targetIndicatorFlashDuration)
                .WaitForCompletion();

            targetIndicator.SetActive(false);
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

        private void OnDestinationReach(UnitFacade unit)
        {
            if (!_units.Contains(unit))
            {
                throw new InvalidOperationException();
            }

            unit.DestinationReach -= OnDestinationReach;

            _units.Remove(unit);

            UpdateState();
        }
    }
}
