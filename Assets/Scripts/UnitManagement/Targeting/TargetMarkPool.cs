using System;
using System.Collections.Generic;
using Units.Services;
using Units.Unit;
using UnityEngine;
using Zenject;

namespace UnitManagement.Targeting
{
    public class TargetMarkPool : MonoBehaviour
    {
        private TargetMark _template;
        private Transform _targetParent;

        private readonly List<TargetMark> _targetMarks = new();

        private UnitRepository _unitRepository;

        [Inject]
        public void Construct(TargetMark template, Transform targetParent, UnitRepository unitRepository)
        {
            _template = template;
            _targetParent = targetParent;
            _unitRepository = unitRepository;
        }

        private void OnEnable()
        {
            _unitRepository.Remove += OnRemove;
        }

        private void OnDisable()
        {
            _unitRepository.Remove -= OnRemove;
        }

        public TargetMark PlaceTo(Vector3 position, Target target = null)
        {
            var targetMark = GetFromPoolOrCreate();
            if (target)
            {
                targetMark.SetTargetObject(target);
            }
            else
            {
                targetMark.ClearTargetObject();
            }

            targetMark.transform.position = position;

            return targetMark;
        }

        public void Link(TargetMark targetMark, ITargetable targetable)
        {
            if (!_targetMarks.Contains(targetMark))
            {
                throw new InvalidOperationException();
            }

            RemoveFromOldTarget(targetable);
            AddTarget(targetMark, targetable);
        }

        public void OffAll()
        {
            foreach (var targetMark in _targetMarks)
            {
                targetMark.Clear();
                _targetMarks.Remove(targetMark);
                Destroy(targetMark.gameObject);
            }
        }

        private void OnRemove(UnitFacade unit)
        {
            RemoveFromOldTarget(unit.Targetable);
        }

        private TargetMark GetFromPoolOrCreate()
        {
            foreach (var targetMark in _targetMarks)
            {
                if (targetMark.Empty)
                {
                    return targetMark;
                }
            }

            return CreateNew();
        }

        private void RemoveFromOldTarget(ITargetable targetable)
        {
            foreach (var targetMark in _targetMarks)
            {
                targetMark.Remove(targetable);
            }
        }

        private void AddTarget(TargetMark targetMark, ITargetable targetable)
        {
            _targetMarks.Add(targetMark);
            targetMark.Add(targetable);
        }

        private TargetMark CreateNew()
        {
            var targetMark = Instantiate(_template, Vector3.zero, Quaternion.identity, _targetParent);
            targetMark.Deactivate();

            _targetMarks.Add(targetMark);

            return targetMark;
        }
    }
}
