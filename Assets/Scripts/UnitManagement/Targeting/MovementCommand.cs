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
        private TargetMarkPool _targetMarkPool;

        private AstarPath _astarPath;
        private FormationMovement _formationMovement;

        private MovementInput _movementInput;

        [Inject]
        public void Construct(SelectedUnits selectedUnits, TargetMarkPool markPool, Map map, AstarPath astarPath)
        {
            _selectedUnits = selectedUnits;
            _targetMarkPool = markPool;
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
            if (target.HasDestinationPoint)
            {
                var destinationPoint = target.GetDestinationPoint();
                var targetMark = _targetMarkPool.PlaceTo(destinationPoint, target);
                MoveAllTo(targetMark);
            }
            else
            {
                var targetMark = _targetMarkPool.PlaceTo(hit.point);
                MoveAllTo(targetMark);
            }
        }

        private void MoveAllTo(TargetMark targetMark)
        {
            var targetables = GetTargetables().ToList();
            _formationMovement.MoveTo(targetables, targetMark);
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
