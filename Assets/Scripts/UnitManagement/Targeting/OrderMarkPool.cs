using System;
using System.Collections.Generic;
using Entities;
using Entities.Entity;
using Units.Services;
using Units.Unit;
using UnityEngine;
using Zenject;

namespace UnitManagement.Targeting
{
    public class OrderMarkPool : MonoBehaviour
    {
        private OrderMark _template;
        private Transform _orderMarkParent;

        private readonly List<OrderMark> _orderMarks = new();

        private UnitRepository _unitRepository;

        [Inject]
        public void Construct(OrderMark template, Transform orderMarkParent, UnitRepository unitRepository)
        {
            _template = template;
            _orderMarkParent = orderMarkParent;
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

        public OrderMark PlaceTo(Vector3 position, Entity entity = null)
        {
            var orderMark = GetFromPoolOrCreate();
            if (entity)
            {
                orderMark.SetEntity(entity);
            }
            else
            {
                orderMark.ClearEntity();
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

            RemoveFromOldOrderMark(orderable);
            AddOrderMark(orderMark, orderable);
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
            RemoveFromOldOrderMark(unit.Orderable);
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

        private void RemoveFromOldOrderMark(IOrderable orderable)
        {
            foreach (var orderMark in _orderMarks)
            {
                orderMark.Remove(orderable);
            }
        }

        private void AddOrderMark(OrderMark orderMark, IOrderable orderable)
        {
            _orderMarks.Add(orderMark);
            orderMark.Add(orderable);
        }

        private OrderMark CreateNew()
        {
            var orderMark = Instantiate(_template, Vector3.zero, Quaternion.identity, _orderMarkParent);
            orderMark.Deactivate();

            _orderMarks.Add(orderMark);

            return orderMark;
        }
    }
}
