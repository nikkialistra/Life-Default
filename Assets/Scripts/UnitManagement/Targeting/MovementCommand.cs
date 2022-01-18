using System.Collections.Generic;
using System.Linq;
using MapGeneration.Map;
using Units.Services.Selecting;
using UnityEngine;
using Zenject;

namespace UnitManagement.Targeting
{
    [RequireComponent(typeof(MovementInput))]
    [RequireComponent(typeof(FormationMovement))]
    public class MovementCommand : MonoBehaviour
    {
        private SelectedUnits _selectedUnits;
        private OrderMarkPool _orderMarkPool;

        private AstarPath _astarPath;
        private FormationMovement _formationMovement;

        private MovementInput _movementInput;

        [Inject]
        public void Construct(SelectedUnits selectedUnits, OrderMarkPool markPool, Map map, AstarPath astarPath)
        {
            _selectedUnits = selectedUnits;
            _orderMarkPool = markPool;
        }

        private void Awake()
        {
            _movementInput = GetComponent<MovementInput>();
            _formationMovement = GetComponent<FormationMovement>();
        }

        private void OnEnable()
        {
            _movementInput.TargetSet += MoveAll;
        }

        private void OnDisable()
        {
            _movementInput.TargetSet -= MoveAll;
        }

        private void MoveAll(Target target, RaycastHit hit)
        {
            if (target.IsEntity)
            {
                var destinationPoint = target.GetDestinationPoint();
                var orderMark = _orderMarkPool.PlaceTo(destinationPoint, target);
                MoveAllTo(orderMark);
            }
            else
            {
                var orderMark = _orderMarkPool.PlaceTo(hit.point);
                MoveAllTo(orderMark);
            }
        }

        private void MoveAllTo(OrderMark orderMark)
        {
            var orderables = GetOrderables().ToList();
            _formationMovement.MoveTo(orderables, orderMark);
        }

        private IEnumerable<IOrderable> GetOrderables()
        {
            foreach (var unit in _selectedUnits.Units)
            {
                var orderable = unit.GetComponent<IOrderable>();
                if (orderable == null)
                {
                    continue;
                }

                yield return orderable;
            }
        }
    }
}
