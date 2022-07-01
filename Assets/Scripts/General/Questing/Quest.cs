using System;
using System.Collections.Generic;
using General.Questing.Objectives;
using Sirenix.OdinInspector;
using UnityEngine;

namespace General.Questing
{
    [CreateAssetMenu(fileName = "Quest", menuName = "Quest")]
    public class Quest : ScriptableObject
    {
        public event Action<Quest> Complete;

        public string Title => _title;
        public string Description => _description;

        public List<IObjective> Objectives => _objectives;

        [SerializeField] private string _title;
        [TextArea]
        [SerializeField] private string _description;

        [ValidateInput("@_objectives.Count <= 3")]
        [SerializeReference] private List<IObjective> _objectives;

        private int _objectivesCompleted;

        public bool HasObjectiveAt(int index)
        {
            return index < _objectives.Count;
        }

        public void Activate(QuestServices questServices)
        {
            _objectivesCompleted = 0;

            foreach (var objective in _objectives)
            {
                objective.Activate(questServices);
                objective.Complete += CheckQuestCompletion;
            }
        }

        private void CheckQuestCompletion(string _)
        {
            _objectivesCompleted++;

            if (_objectivesCompleted == _objectives.Count)
            {
                Deactivate();
                Complete?.Invoke(this);
            }
        }

        private void Deactivate()
        {
            foreach (var objective in _objectives)
                objective.Complete -= CheckQuestCompletion;
        }
    }
}
