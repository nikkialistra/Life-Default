using System.Collections.Generic;
using System.Linq;
using MapGeneration.Map;
using Units.Services.Selecting;
using UnityEngine;
using Zenject;

namespace UnitManagement.Targeting
{
    [RequireComponent(typeof(FormationComposing))]
    public class MovementCommand : MonoBehaviour
    {
        private SelectedUnits _selectedUnits;
        private TargetPool _targetPool;

        private AstarPath _astarPath;
        private FormationComposing _formationComposing;

        [Inject]
        public void Construct(SelectedUnits selectedUnits, TargetPool pool, Map map, AstarPath astarPath)
        {
            _selectedUnits = selectedUnits;
            _targetPool = pool;
        }

        private void Awake()
        {
            _formationComposing = GetComponent<FormationComposing>();
        }

        public bool CanAcceptCommand => _selectedUnits.Units.Any();

        public void MoveAll(TargetObject targetObject, RaycastHit hit)
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
            _formationComposing.MoveTo(targetables, target);
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