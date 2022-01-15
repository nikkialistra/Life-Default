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
        private TargetPool _targetPool;

        private AstarPath _astarPath;
        private FormationMovement _formationMovement;

        private MovementInput _movementInput;

        [Inject]
        public void Construct(SelectedUnits selectedUnits, TargetPool pool, Map map, AstarPath astarPath)
        {
            _selectedUnits = selectedUnits;
            _targetPool = pool;
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

        private void MoveAll(TargetObject targetObject, RaycastHit hit)
        {
            if (targetObject.HasDestinationPoint)
            {
                var destinationPoint = targetObject.GetDestinationPoint();
                var target = _targetPool.PlaceTo(destinationPoint, targetObject);
                MoveAllTo(target);
            }
            else
            {
                var target = _targetPool.PlaceTo(hit.point);
                MoveAllTo(target);
            }
        }

        private void MoveAllTo(Target target)
        {
            var targetables = GetTargetables().ToList();
            _formationMovement.MoveTo(targetables, target);
        }

        private IEnumerable<ITargetable> GetTargetables()
        {
            foreach (var unit in _selectedUnits.Units)
            {
                var targetable = unit.GetComponent<ITargetable>();
                if (targetable == null)
                {
                    continue;
                }

                yield return targetable;
            }
        }
    }
}
