using UnityEngine;

namespace Units.Equipment
{
    [CreateAssetMenu(fileName = "Consumable", menuName = "Consumable")]
    public class Consumable : ScriptableObject
    {
        public GameObject ConsumableGameObject => _consumable;

        [SerializeField] private GameObject _consumable;
    }
}
