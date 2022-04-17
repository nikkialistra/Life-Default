using System;
using System.Collections.Generic;
using Colonists;
using Colonists.Services;
using Entities;
using UnityEngine;
using Zenject;

namespace ColonistManagement.OrderMarks
{
    public class OrderMarkPool : MonoBehaviour
    {
        private OrderMark _template;
        private Transform _orderMarkParent;

        private readonly List<OrderMark> _orderMarks = new();

        private ColonistRepository _colonistRepository;

        [Inject]
        public void Construct(OrderMark template, Transform orderMarkParent, ColonistRepository colonistRepository)
        {
            _template = template;
            _orderMarkParent = orderMarkParent;
            _colonistRepository = colonistRepository;
        }

        private void OnEnable()
        {
            _colonistRepository.Remove += OnRemove;
        }

        private void OnDisable()
        {
            _colonistRepository.Remove -= OnRemove;
        }

        public OrderMark PlaceTo(Vector3 position, Entity entity = null)
        {
            var orderMark = GetFromPoolOrCreate();

            orderMark.Entity = entity ? entity : null;
            orderMark.transform.position = position;

            return orderMark;
        }

        public void Link(OrderMark orderMark, Colonist colonist)
        {
            if (!_orderMarks.Contains(orderMark))
            {
                throw new InvalidOperationException();
            }

            RemoveFromOldOrderMark(colonist);
            AddOrderMark(orderMark, colonist);
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

        private void OnRemove(Colonist colonist)
        {
            RemoveFromOldOrderMark(colonist);
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

        private void RemoveFromOldOrderMark(Colonist colonist)
        {
            foreach (var orderMark in _orderMarks)
            {
                orderMark.Remove(colonist);
            }
        }

        private void AddOrderMark(OrderMark orderMark, Colonist colonist)
        {
            _orderMarks.Add(orderMark);
            orderMark.Add(colonist);
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
