using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Units;
using Units.Enums;
using UnityEngine;

namespace Enemies.BehaviorNodes
{
    public class EscapeToSafety : Action
    {
        public float SafeDistance = 15f;
        public float DistanceForOneRetreat = 5f;
        
        public LayerMask _targetMask;
        public float TimeToRescan = 0.2f;
        
        public EnemyMeshAgent EnemyMeshAgent;

        public SharedBool Escape;
        
        private float _nextTimeToScan;

        public override void OnStart()
        {
            EnemyMeshAgent.StopMoving();
        }

        public override TaskStatus OnUpdate()
        {
            if (Time.time < _nextTimeToScan)
            {
                return TaskStatus.Running;
            }
            
            _nextTimeToScan += TimeToRescan;
            
            var opponents = FindOpponents();

            if (opponents.Count == 0)
            {
                return TaskStatus.Failure;
            }
            else
            {
                RetreatFromClosestOpponent(opponents);
                return TaskStatus.Running;
            }
        }

        public override void OnEnd()
        {
            Escape.Value = false;
        }

        private List<Unit> FindOpponents()
        {
            var colliders = Physics.OverlapSphere(transform.position, SafeDistance, _targetMask);

            var opponents = new List<Unit>();

            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent(out Unit unit))
                {
                    if (unit.Fraction == Fraction.Colonists)
                    {
                        opponents.Add(unit);
                    }
                }
            }

            return opponents;
        }

        private void RetreatFromClosestOpponent(List<Unit> opponents)
        {
            var shortestDistance = float.PositiveInfinity;
            Unit closestOpponent = default;

            foreach (var opponent in opponents)
            {
                var distance = Vector3.Distance(EnemyMeshAgent.transform.position, opponent.transform.position);
                
                if (distance < shortestDistance)
                {
                    closestOpponent = opponent;
                    shortestDistance = distance;
                }
            }

            if (closestOpponent == null)
            {
                return;
            }

            EnemyMeshAgent.RunFrom(closestOpponent, DistanceForOneRetreat);
        }
    }
}
