using UnityEngine;

namespace Units.Equipment
{
    [CreateAssetMenu(fileName = "Property", menuName = "Property")]
    public class Property : ScriptableObject
    {
        [SerializeField] private GameObject _property;
        
        public GameObject PropertyGameObject => _property;
    }
}
