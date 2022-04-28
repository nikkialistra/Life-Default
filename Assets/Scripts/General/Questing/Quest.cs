using System.Collections.Generic;
using UnityEngine;

namespace General.Questing
{
    [CreateAssetMenu(fileName = "Quest", menuName = "Quest")]
    public class Quest : ScriptableObject
    {
        [SerializeField] private string _title;
        [TextArea]
        [SerializeField] private string _description;
        
        [SerializeField] private List<Objective> _objectives;
    }
}
