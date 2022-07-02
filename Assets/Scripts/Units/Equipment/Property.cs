using UnityEngine;

namespace Units.Equipment
{
    [CreateAssetMenu(fileName = "Property", menuName = "Property")]
    public class Property : ScriptableObject
    {
        public GameObject PropertyGameObject => _property;

        [SerializeField] private GameObject _property;
    }
}
