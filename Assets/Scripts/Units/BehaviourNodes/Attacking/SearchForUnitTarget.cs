using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Units.Ancillaries;
using Units.BehaviorVariables;
using UnityEngine;

namespace Units.BehaviourNodes.Attacking
{
    public class SearchForUnitTarget : Action
    {
        public SharedUnit Self;
        
        public SharedUnit UnitTarget;
        public SharedBool NewCommand;
        
        public FieldOfView FieldOfView;

        private float _shortestDistanceToUnit;

        public override void OnStart()
        {
            UnitTarget.Value = null;
            _shortestDistanceToUnit = float.PositiveInfinity;
        }

        public override TaskStatus OnUpdate()
        {
            TryToFind();

            if (UnitTarget.Value != null)
            {
                NewCommand.Value = true;
            }

            return TaskStatus.Success;
        }

        private void TryToFind()
        {
            foreach (var target in FieldOfView.FindVisibleTargets())
            {
                var unit = target.GetComponent<Unit>();
                if (IsUnitTarget(unit))
                {
                    SetIfClosest(unit);
                }
            }
        }

        private bool IsUnitTarget(Unit unit)
        {
            return unit != null || Self.Value.Fraction != unit.Fraction;
        }

        private void SetIfClosest(Unit unit)
        {
            var distanceToEntity = Vector3.Distance(transform.position, unit.transform.position);

            if (distanceToEntity > _shortestDistanceToUnit)
            {
                return;
            }

            _shortestDistanceToUnit = distanceToEntity;
            UnitTarget.Value = unit;
        }
    }
}
