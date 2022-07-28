using System;
using Aborigines;
using Aborigines.Services;
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

        private AborigineRepository _aborigineRepository;

        public void Activate(QuestServices questServices)
        {
            _destroyed = 0;

            _aborigineRepository = questServices.AborigineRepository;

            _aborigineRepository.Remove += IncrementCounter;
        }

        public void Deactivate()
        {
            _aborigineRepository.Remove -= IncrementCounter;
        }

        public string ToText()
        {
            return $"Get rid of {_quantity} {_type.GetStringForMultiple()}  –  {_destroyed}/{_quantity}";
        }

        private void IncrementCounter(Aborigine _)
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
