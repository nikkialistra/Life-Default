using System.Collections.Generic;
using General.Questing.Objectives;
using Sirenix.OdinInspector;
using UnityEngine;

namespace General.Questing
{
    [CreateAssetMenu(fileName = "Collect Objective", menuName = "Quest/Quest")]
    public class Quest : ScriptableObject
    {
        [SerializeField] private string _title;
        [TextArea]
        [SerializeField] private string _description;
        
        [ValidateInput("@_objectives.Count <= 3")]
        [SerializeReference] private List<IObjective> _objectives;

        public string Title => _title;
        public string Description => _description;

        public List<IObjective> Objectives => _objectives;

        public bool HasObjectiveAt(int index)
        {
            return index < _objectives.Count;
        }

        public void Activate(QuestServices questServices)
        {
            foreach (var objective in _objectives)
            {
                objective.Activate(questServices);
            }
        }

        public void Deactivate()
        {
            foreach (var objective in _objectives)
            {
                objective.Deactivate();
            }
        }
    }
}
