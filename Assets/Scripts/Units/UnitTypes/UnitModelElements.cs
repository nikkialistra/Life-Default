using System;
using Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Units.UnitTypes
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
        
        [Title("Types")]
        [ValidateInput("@_unitTypeAppearances.Count == 5", "Unit type appearances dictionary should have 5 elements for all 5 unit types.")]
        [SerializeField] private UnitTypeAppearancesDictionary _unitTypeAppearances;
        
        [Serializable] public class UnitTypeAppearancesDictionary : SerializableDictionary<UnitType, UnitTypeAppearance> {}

        public void SwitchTo(UnitType unitType)
        {
            // ResetAppearance();
            SetUpAppearance(unitType);
        }

        private void ResetAppearance()
        {
            DeleteChildrenFor(_headEnd);
            DeleteChildrenFor(_head);
            DeleteChildrenFor(_rightHand);
            DeleteChildrenFor(_upperChest);
        }

        private static void DeleteChildrenFor(Transform element)
        {
            foreach (Transform child in element)
            {
                Destroy(child.gameObject);
            }
        }

        private void SetUpAppearance(UnitType unitType)
        {
            var appearance = _unitTypeAppearances[unitType];

            SetElement(appearance.HeadEndAccessory, _headEnd);
            SetElement(appearance.HeadAccessory, _head);
            SetElement(appearance.RightHandAccessory, _rightHand);
            SetElement(appearance.UpperChestAccessory, _upperChest);

            SetSkin(appearance.Skin);
        }

        private void SetElement(GameObject element, Transform elementParent)
        {
            if (element != null)
            {
                Instantiate(element, elementParent);
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