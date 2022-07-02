using System;
using Sirenix.OdinInspector;
using Units.Appearance.ItemVariants;
using Units.Appearance.Pairs;
using Units.Appearance.Variants;
using Units.Enums;
using UnityEngine;
using static Units.Appearance.GarmentSet;
using static Units.Appearance.HumanAppearanceRegistry;

namespace Units.Appearance
{
    public class HumanAppearance : MonoBehaviour
    {
        [Title("Bindings")]
        [SerializeField] private GameObject _male;
        [SerializeField] private GameObject _female;
        [Space]
        [SerializeField] private GenderItems _maleItems;
        [SerializeField] private GenderItems _femaleItems;
        [SerializeField] private AgenderItems _agenderItems;

        private readonly int _color = Shader.PropertyToID("_BaseColor");

        public void RandomizeAppearanceWith(Gender gender, HumanType humanType,
            HumanAppearanceRegistry humanAppearanceRegistry)
        {
            GenderItems genderItems;
            if (gender == Gender.Male)
            {
                genderItems = _maleItems;

                _male.SetActive(true);
                _female.SetActive(false);
            }
            else
            {
                genderItems = _femaleItems;

                _male.SetActive(false);
                _female.SetActive(true);
            }

            ResetComplementaryItems();

            RandomizeHeadItems(gender, genderItems, humanAppearanceRegistry.HeadVariantsFor(gender));
            RandomizeGarmentSet(genderItems, humanAppearanceRegistry.GarmentSetFor(gender, humanType));
            RandomizeColors(gender, genderItems, humanAppearanceRegistry.ColorVariants);
        }

        private void ResetComplementaryItems()
        {
            _maleItems.FacialHair.sharedMesh = null;

            _agenderItems.Hair.sharedMesh = null;
            _agenderItems.Ears.sharedMesh = null;
            _agenderItems.HeadCoveringHair.sharedMesh = null;
            _agenderItems.HeadCoveringNoHair.sharedMesh = null;
            _agenderItems.HeadCoveringNoFacialHair.sharedMesh = null;
            _agenderItems.Helmet.sharedMesh = null;
            _agenderItems.BackAttachment.sharedMesh = null;
            _agenderItems.ShoulderAttachmentRight.sharedMesh = null;
            _agenderItems.ShoulderAttachmentLeft.sharedMesh = null;
            _agenderItems.ElbowAttachmentRight.sharedMesh = null;
            _agenderItems.ElbowAttachmentLeft.sharedMesh = null;
            _agenderItems.HipsAttachment.sharedMesh = null;
            _agenderItems.KneeAttachmentRight.sharedMesh = null;
            _agenderItems.KneeAttachmentLeft.sharedMesh = null;
        }

        private void RandomizeHeadItems(Gender gender, GenderItems genderItems, HeadVariants headVariants)
        {
            RandomizeItem(genderItems.Head, headVariants.Head);

            RandomizeItem(_agenderItems.Hair, headVariants.Hair);

            RandomizeItem(_agenderItems.Ears, headVariants.Ears);
            RandomizeItem(genderItems.Eyebrows, headVariants.Eyebrows);

            if (gender == Gender.Male)
            {
                RandomizeItem(genderItems.FacialHair, headVariants.FacialHair);
            }
        }

        private void RandomizeGarmentSet(GenderItems genderItems, IItemVariants<GarmentSet> garmentSets)
        {
            var garment = garmentSets.GetRandom();
            garment.ResetTakeHistory();

            if (garment.ShouldHeadCoveringReplaceHair())
                SetItem(_agenderItems.HeadCoveringHair, garment.GetElement(GarmentElements.HeadCoveringHair));
            else
                SetItemWithReplacement(_agenderItems.HeadCoveringNoHair, _agenderItems.Hair,
                    garment.GetElement(GarmentElements.HeadCoveringNoHair));

            SetItemWithReplacement(_agenderItems.HeadCoveringNoFacialHair, genderItems.FacialHair,
                garment.GetElement(GarmentElements.HeadCoveringNoFacialHair));

            SetItem(genderItems.Torso, garment.GetElement(GarmentElements.Torso));
            SetItem(_agenderItems.BackAttachment, garment.GetElement(GarmentElements.BackAttachment));

            SetItemPair(_agenderItems.ShoulderAttachmentRight, _agenderItems.ShoulderAttachmentLeft,
                garment.GetElementsAtSameIndex(GarmentElementPairs.ShoulderAttachments));

            SetItemPair(genderItems.ArmUpperRight, genderItems.ArmUpperLeft,
                garment.GetElementsAtSameIndex(GarmentElementPairs.ArmsUpper));
            SetItemPair(genderItems.ArmLowerRight, genderItems.ArmLowerLeft,
                garment.GetElementsAtSameIndex(GarmentElementPairs.ArmsLower));

            SetItemPair(genderItems.HandRight, genderItems.HandLeft,
                garment.GetElementsAtSameIndex(GarmentElementPairs.Hands));

            SetItem(genderItems.Hips, garment.GetElement(GarmentElements.Hips));
            SetItem(_agenderItems.HipsAttachment, garment.GetElement(GarmentElements.HipsAttachment));

            SetItemPair(genderItems.LegRight, genderItems.LegLeft,
                garment.GetElementsAtSameIndex(GarmentElementPairs.Legs));
        }

        private void RandomizeColors(Gender gender, GenderItems genderItems, ColorVariants colorVariants)
        {
            var skinMaterial = colorVariants.SkinColorMaterials.GetRandom();
            var color = colorVariants.HairColors.GetRandom();

            SetSkinMaterial(genderItems, skinMaterial);
            SetColor(_agenderItems.Hair, color);

            if (gender == Gender.Male)
                SetColor(genderItems.FacialHair, color);
        }

        private void SetSkinMaterial(GenderItems genderItems, Material skinMaterial)
        {
            SetMaterial(genderItems.Head, skinMaterial);
            SetMaterial(_agenderItems.Ears, skinMaterial);

            SetMaterial(genderItems.Torso, skinMaterial);
            SetMaterial(genderItems.ArmUpperRight, skinMaterial);
            SetMaterial(genderItems.ArmUpperLeft, skinMaterial);
            SetMaterial(genderItems.ArmLowerRight, skinMaterial);
            SetMaterial(genderItems.ArmLowerLeft, skinMaterial);

            SetMaterial(genderItems.HandRight, skinMaterial);
            SetMaterial(genderItems.HandLeft, skinMaterial);

            SetMaterial(genderItems.Hips, skinMaterial);

            SetMaterial(genderItems.LegRight, skinMaterial);
            SetMaterial(genderItems.LegLeft, skinMaterial);
        }

        private void RandomizeItem(SkinnedMeshRenderer renderer, IItemVariants<Mesh> meshVariants)
        {
            var mesh = meshVariants.GetRandom();

            if (mesh != null)
                renderer.sharedMesh = mesh;
        }

        private void SetItem(SkinnedMeshRenderer renderer, Mesh mesh)
        {
            if (mesh != null)
                renderer.sharedMesh = mesh;
        }

        private void SetItemWithReplacement(SkinnedMeshRenderer renderer, SkinnedMeshRenderer rendererToReplace,
            Mesh mesh)
        {
            if (mesh != null)
            {
                rendererToReplace.sharedMesh = null;
                renderer.sharedMesh = mesh;
            }
        }

        private void SetItemPair(SkinnedMeshRenderer firstRenderer, SkinnedMeshRenderer secondRenderer,
            MeshPair meshPair)
        {
            if (meshPair.FirstMesh != null)
                firstRenderer.sharedMesh = meshPair.FirstMesh;

            if (meshPair.SecondMesh != null)
                secondRenderer.sharedMesh = meshPair.SecondMesh;
        }

        private void SetColor(SkinnedMeshRenderer renderer, Color color)
        {
            renderer.material.SetColor(_color, color);
        }

        private void SetMaterial(SkinnedMeshRenderer renderer, Material material)
        {
            renderer.material = material;
        }

        [Serializable]
        private class GenderItems
        {
            public SkinnedMeshRenderer Head;
            public SkinnedMeshRenderer HeadAccessory;
            public SkinnedMeshRenderer Eyebrows;
            public SkinnedMeshRenderer FacialHair;
            public SkinnedMeshRenderer Torso;
            public SkinnedMeshRenderer ArmUpperRight;
            public SkinnedMeshRenderer ArmUpperLeft;
            public SkinnedMeshRenderer ArmLowerRight;
            public SkinnedMeshRenderer ArmLowerLeft;
            public SkinnedMeshRenderer HandRight;
            public SkinnedMeshRenderer HandLeft;
            public SkinnedMeshRenderer Hips;
            public SkinnedMeshRenderer LegRight;
            public SkinnedMeshRenderer LegLeft;
        }

        [Serializable]
        private class AgenderItems
        {
            public SkinnedMeshRenderer Hair;
            public SkinnedMeshRenderer Ears;
            public SkinnedMeshRenderer HeadCoveringHair;
            public SkinnedMeshRenderer HeadCoveringNoHair;
            public SkinnedMeshRenderer HeadCoveringNoFacialHair;
            public SkinnedMeshRenderer Helmet;
            public SkinnedMeshRenderer BackAttachment;
            public SkinnedMeshRenderer ShoulderAttachmentRight;
            public SkinnedMeshRenderer ShoulderAttachmentLeft;
            public SkinnedMeshRenderer ElbowAttachmentRight;
            public SkinnedMeshRenderer ElbowAttachmentLeft;
            public SkinnedMeshRenderer HipsAttachment;
            public SkinnedMeshRenderer KneeAttachmentRight;
            public SkinnedMeshRenderer KneeAttachmentLeft;
        }
    }
}
