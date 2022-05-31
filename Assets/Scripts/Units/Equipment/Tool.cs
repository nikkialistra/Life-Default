using System;
using ResourceManagement;
using Units.Enums;
using UnityEngine;

namespace Units.Equipment
{
    
    [CreateAssetMenu(fileName = "Tool", menuName = "Tool")]
    public class Tool : ScriptableObject
    {
        [SerializeField] private ToolType _toolType;
        [SerializeField] private GameObject _tool;

        public ToolType ToolType => _toolType;
        public GameObject ToolGameObject => _tool;

        public bool CanExtract(ResourceType resourceType)
        {
            return resourceType switch
            {
                ResourceType.Wood => _toolType == ToolType.Axe,
                ResourceType.Stone => _toolType == ToolType.Pickaxe,
                _ => throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null)
            };
        }
    }
}
