using Colonists.Services;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Colonists.Colonist.ColonistTypes
{
    public class ColonistAppearance : MonoBehaviour
    {
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

        private ColonistTypeAppearanceRegistry _colonistTypeAppearanceRegistry;

        [Inject]
        public void Construct(ColonistTypeAppearanceRegistry colonistTypeAppearanceRegistry)
        {
            _colonistTypeAppearanceRegistry = colonistTypeAppearanceRegistry;
        }

        public void SwitchTo(ColonistType colonistType)
        {
            var appearance = _colonistTypeAppearanceRegistry.GetForType(colonistType);

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
