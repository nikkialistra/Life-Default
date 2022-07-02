using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Units.Ancillaries.Fields;
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
        public FieldOfHearing FieldOfHearing;

        public float TimeToRescan = 0.2f;

        private float _nextTimeToScan;

        private float _shortestDistanceToUnit;

        public override void OnStart()
        {
            _shortestDistanceToUnit = float.PositiveInfinity;

            _nextTimeToScan = Time.time + TimeToRescan;
        }

        public override TaskStatus OnUpdate()
        {
            if (Time.time < _nextTimeToScan)
                return TaskStatus.Running;

            _nextTimeToScan += TimeToRescan;

            if (UnitTarget.Value != null)
            {
                NewCommand.Value = true;
                Self.Value.UnitEquipment.EquipWeapon();
                return TaskStatus.Running;
            }

            TryToFind();
            UpdateEquipment();

            return TaskStatus.Running;
        }

        private void TryToFind()
        {
            foreach (var target in GetVisibleTargets())
            {
                var unit = target.GetComponent<Unit>();
                if (IsUnitTarget(unit))
                    SetIfClosest(unit);
            }
        }

        private void UpdateEquipment()
        {
            if (UnitTarget.Value != null)
            {
                NewCommand.Value = true;
                Self.Value.UnitEquipment.EquipWeapon();
            }
        }

        private IEnumerable<Transform> GetVisibleTargets()
        {
            return FieldOfView.FindVisibleTargets().Concat(FieldOfHearing.FindVisibleTargets());
        }

        private bool IsUnitTarget(Unit unit)
        {
            return unit != null && unit.Alive && Self.Value.Faction != unit.Faction;
        }

        private void SetIfClosest(Unit unit)
        {
            var distanceToEntity = Vector3.Distance(transform.position, unit.transform.position);

            if (distanceToEntity > _shortestDistanceToUnit) return;

            _shortestDistanceToUnit = distanceToEntity;
            UnitTarget.Value = unit;
        }
    }
}
