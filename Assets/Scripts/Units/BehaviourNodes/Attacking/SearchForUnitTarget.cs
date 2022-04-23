using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Colonists;
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
            _shortestDistanceToUnit = float.PositiveInfinity;
        }

        public override TaskStatus OnUpdate()
        {
            if (UnitTarget.Value != null)
            {
                NewCommand.Value = true;
                Self.Value.UnitEquipment.EquipWeapon();
                return TaskStatus.Success;
            }
            
            TryToFind();

            if (UnitTarget.Value != null)
            {
                NewCommand.Value = true;
                Self.Value.UnitEquipment.EquipWeapon();
            }
            else
            {
                Self.Value.UnitEquipment.UnequipWeapon();
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
