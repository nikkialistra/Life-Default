using System;
using Enemies;
using Enemies.Services;
using Units.Enums;
using UnityEngine;

namespace Questing.Objectives
{
    [Serializable]
    public class DestroyObjective : IObjective
    {
        public event Action<string> Update;
        public event Action<string> Complete;

        [SerializeField] private Faction _type;
        [SerializeField] private int _quantity;

        private int _destroyed;

        private EnemyRepository _enemyRepository;

        public void Activate(QuestServices questServices)
        {
            _destroyed = 0;

            _enemyRepository = questServices.EnemyRepository;

            _enemyRepository.Remove += IncrementCounter;
        }

        public void Deactivate()
        {
            _enemyRepository.Remove -= IncrementCounter;
        }

        public string ToText()
        {
            return $"Get rid of {_quantity} {_type.GetStringForMultiple()}  –  {_destroyed}/{_quantity}";
        }

        private void IncrementCounter(Enemy _)
        {
            _destroyed++;

            Update?.Invoke(ToText());

            CheckForCompletion();
        }

        private void CheckForCompletion()
        {
            if (_destroyed >= _quantity)
            {
                Deactivate();
                Complete?.Invoke($"<s>{ToText()}</s>");
            }
        }
    }
}
