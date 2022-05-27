using UnityEngine;

namespace Units.Equipment
{
    [CreateAssetMenu(fileName = "Consumable", menuName = "Consumable")]
    public class Consumable : ScriptableObject
    {
        [SerializeField] private GameObject _consumable;
        
        public GameObject ConsumableGameObject => _consumable;
    }
}
