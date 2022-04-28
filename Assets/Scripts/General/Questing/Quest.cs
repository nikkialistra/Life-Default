using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace General.Questing
{
    [CreateAssetMenu(fileName = "Quest", menuName = "Quest")]
    public class Quest : ScriptableObject
    {
        [SerializeField] private string _title;
        [TextArea]
        [SerializeField] private string _description;
        
        [ValidateInput("@_objectives.Count <= 3")]
        [SerializeField] private List<Objective> _objectives;

        public string Title => _title;
        public string Description => _description;

        public List<Objective> Objectives => _objectives;
    }
}
