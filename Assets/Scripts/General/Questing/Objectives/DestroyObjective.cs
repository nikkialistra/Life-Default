using System;
using Enemies;
using Enemies.Services;
using Units.Enums;
using UnityEngine;

namespace General.Questing.Objectives
{
    [Serializable]
    public class DestroyObjective : IObjective
    {
        [SerializeField] private Fraction _type;
        [SerializeField] private int _quantity;

        private int _destroyed;

        private EnemyRepository _enemyRepository;

        public event Action<string> Update;
        
        public void Activate(QuestServices questServices)
        {
            _enemyRepository = questServices.EnemyRepository;

            _enemyRepository.Remove += IncrementCounter;
        }

        public void Deactivate()
        {
            _enemyRepository.Remove -= IncrementCounter;
        }

        private void IncrementCounter(Enemy _)
        {
            _destroyed++;
            
            Update?.Invoke(ToText());
        }

        public string ToText()
        {
            return $"Get rid of {_quantity} {_type.GetStringForMultiple()}  –  {_destroyed}/{_quantity}";
        }
    }
}
