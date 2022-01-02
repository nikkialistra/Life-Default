using Sirenix.OdinInspector;
using Units.Services;
using UnityEngine;
using Zenject;

namespace Units.Unit.UnitTypes
{
    public class UnitModelElements : MonoBehaviour
    {
        [Title("Elements")]
        [Required]
        [SerializeField] private Transform _headEnd;
        [Required]
        [SerializeField] private Transform _head;
        [Required]
        [SerializeField] private Transform _rightHand;
        [Required]
        [SerializeField] private Transform _upperChest;
        [Space]
        [Required]
        [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;

        private GameObject _headEndAccessory;
        private GameObject _headAccessory;
        private GameObject _rightHandAccessory;
        private GameObject _upperChestAccessory;

        private UnitTypeAppearanceRegistry _unitTypeAppearanceRegistry;

        [Inject]
        public void Construct(UnitTypeAppearanceRegistry unitTypeAppearanceRegistry)
        {
            _unitTypeAppearanceRegistry = unitTypeAppearanceRegistry;
        }

        public void SwitchTo(UnitType unitType)
        {
            var appearance = _unitTypeAppearanceRegistry.GetForType(unitType);

            SetAccessory(ref _headEndAccessory, appearance.HeadEndAccessory, _headEnd);
            SetAccessory(ref _headAccessory, appearance.HeadAccessory, _head);
            SetAccessory(ref _rightHandAccessory, appearance.RightHandAccessory, _rightHand);
            SetAccessory(ref _upperChestAccessory, appearance.UpperChestAccessory, _upperChest);

            SetSkin(appearance.Skin);
        }

        private void SetAccessory(ref GameObject accessory, GameObject element, Transform elementParent)
        {
            if (accessory != null)
            {
                Destroy(accessory);
            }

            if (element != null)
            {
                accessory = Instantiate(element, elementParent);
            }
        }

        private void SetSkin(Material skin)
        {
            var materials = _skinnedMeshRenderer.materials;
            materials[0] = skin;
            _skinnedMeshRenderer.materials = materials;
        }
    }
}