using Sirenix.OdinInspector;
using UnityEngine;

namespace Colonists.Colonist.ColonistTypes
{
    [CreateAssetMenu(fileName = "UnitTypeAppearance", menuName = "Data/Unit Type Appearance")]
    public class ColonistTypeAppearance : ScriptableObject
    {
        [Title("Required Skin")]
        [Required]
        [SerializeField] private Material _skin;

        [Title("Optional Accessories")]
        [SerializeField] private GameObject _headEndAccessory;
        [SerializeField] private GameObject _headAccessory;
        [SerializeField] private GameObject _rightHandAccessory;
        [SerializeField] private GameObject _upperChestAccessory;

        public GameObject HeadEndAccessory => _headEndAccessory;
        public GameObject HeadAccessory => _headAccessory;
        public GameObject RightHandAccessory => _rightHandAccessory;
        public GameObject UpperChestAccessory => _upperChestAccessory;

        public Material Skin => _skin;
    }
}
