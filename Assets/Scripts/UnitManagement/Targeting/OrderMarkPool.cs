using System;
using System.Collections.Generic;
using Units.Services;
using Units.Unit;
using UnityEngine;
using Zenject;

namespace UnitManagement.Targeting
{
    public class OrderMarkPool : MonoBehaviour
    {
        private OrderMark _template;
        private Transform _targetParent;

        private readonly List<OrderMark> _orderMarks = new();

        private UnitRepository _unitRepository;

        [Inject]
        public void Construct(OrderMark template, Transform targetParent, UnitRepository unitRepository)
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

        public OrderMark PlaceTo(Vector3 position, Target target = null)
        {
            var orderMark = GetFromPoolOrCreate();
            if (target)
            {
                orderMark.SetTarget(target);
            }
            else
            {
                orderMark.ClearTarget();
            }

            orderMark.transform.position = position;

            return orderMark;
        }

        public void Link(OrderMark orderMark, IOrderable orderable)
        {
            if (!_orderMarks.Contains(orderMark))
            {
                throw new InvalidOperationException();
            }

            RemoveFromOldTarget(orderable);
            AddTarget(orderMark, orderable);
        }

        public void OffAll()
        {
            foreach (var orderMark in _orderMarks)
            {
                orderMark.Clear();
                _orderMarks.Remove(orderMark);
                Destroy(orderMark.gameObject);
            }
        }

        private void OnRemove(UnitFacade unit)
        {
            RemoveFromOldTarget(unit.Orderable);
        }

        private OrderMark GetFromPoolOrCreate()
        {
            foreach (var orderMark in _orderMarks)
            {
                if (orderMark.Empty)
                {
                    return orderMark;
                }
            }

            return CreateNew();
        }

        private void RemoveFromOldTarget(IOrderable orderable)
        {
            foreach (var orderMark in _orderMarks)
            {
                orderMark.Remove(orderable);
            }
        }

        private void AddTarget(OrderMark orderMark, IOrderable orderable)
        {
            _orderMarks.Add(orderMark);
            orderMark.Add(orderable);
        }

        private OrderMark CreateNew()
        {
            var orderMark = Instantiate(_template, Vector3.zero, Quaternion.identity, _targetParent);
            orderMark.Deactivate();

            _orderMarks.Add(orderMark);

            return orderMark;
        }
    }
}
