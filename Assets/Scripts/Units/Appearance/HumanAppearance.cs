﻿using System;
using System.Collections.Generic;
using Common;
using Sirenix.OdinInspector;
using Units.Appearance.ItemVariants;
using Units.Appearance.Pairs;
using Units.Appearance.Variants;
using UnityEngine;
using static Units.Appearance.GarmentSet;
using Random = UnityEngine.Random;

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

        [Title("Variants")]
        [SerializeField] private GenderItemVariants _maleItemVariants;
        [SerializeField] private GenderItemVariants _femaleItemVariants;
        [SerializeField] private AgenderItemVariants _agenderItemVariants;
        
        private readonly int _color = Shader.PropertyToID("_BaseColor");
        
        public void RandomizeAppearanceWith(Gender gender, HumanAppearanceRegistry humanAppearanceRegistry)
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
            
            RandomizeHeadItems(gender, genderItems,  humanAppearanceRegistry.HeadVariantsFor(gender));
            RandomizeGarmentSet(genderItems, humanAppearanceRegistry.GarmentSetFor(gender));
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
            {
                SetItem(_agenderItems.HeadCoveringHair, garment.GetElement(GarmentElements.HeadCoveringHair));
            }
            else
            {
                SetItemWithReplacement(_agenderItems.HeadCoveringNoHair, _agenderItems.Hair, garment.GetElement(GarmentElements.HeadCoveringNoHair));
            }

            SetItemWithReplacement(_agenderItems.HeadCoveringNoFacialHair, genderItems.FacialHair, garment.GetElement(GarmentElements.HeadCoveringNoFacialHair));

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
            {
                SetColor(genderItems.FacialHair, color);
            }
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
            {
                renderer.sharedMesh = mesh;
            }
        }

        private void SetItem(SkinnedMeshRenderer renderer, Mesh mesh)
        {
            if (mesh != null)
            {
                renderer.sharedMesh = mesh;
            }
        }

        private void SetItemWithReplacement(SkinnedMeshRenderer renderer, SkinnedMeshRenderer rendererToReplace, Mesh mesh)
        {
            if (mesh != null)
            {
                rendererToReplace.sharedMesh = null;
                renderer.sharedMesh = mesh;
            }
        }

        private void SetItemPair(SkinnedMeshRenderer firstRenderer, SkinnedMeshRenderer secondRenderer, MeshPair meshPair)
        {
            if (meshPair.FirstMesh != null)
            {
                firstRenderer.sharedMesh = meshPair.FirstMesh;
            }
            
            if (meshPair.SecondMesh != null)
            {
                secondRenderer.sharedMesh = meshPair.SecondMesh;
            }
        }

        private void SetColor(SkinnedMeshRenderer renderer, Color color)
        {
            renderer.material.SetColor(_color, color);
        }

        private void SetMaterial(SkinnedMeshRenderer renderer, Material material)
        {
            renderer.material = material;
        }

        [Button(ButtonSizes.Medium)]
        private void RandomizeAppearance()
        {
            var randomGender = EnumUtils.RandomValue<Gender>();

            RandomizeAppearanceForGender(randomGender);
        }

        [Button(ButtonSizes.Medium)]
        private void RandomizeAppearanceForGender(Gender gender)
        {
            if (gender == Gender.Male)
            {
                _male.SetActive(true);
                _female.SetActive(false);
            }
            else
            {
                _male.SetActive(false);
                _female.SetActive(true);
            }
            
            var (items, itemVariants) = gender switch
            {
                Gender.Male => (_maleItems, _maleItemVariants),
                Gender.Female => (_femaleItems, _femaleItemVariants),
                _ => throw new ArgumentOutOfRangeException(nameof(gender), gender, null)
            };

            RandomizeGenderItems(items, itemVariants);
            RandomizeAgenderItems();
        }

        private void RandomizeGenderItems(GenderItems items, GenderItemVariants itemVariants)
        {
            RandomizeItem(items.Head, itemVariants.Head);
            RandomizeItem(items.Eyebrows, itemVariants.Eyebrows);
            RandomizeItem(items.FacialHair, itemVariants.FacialHair);
            RandomizeItem(items.Torso, itemVariants.Torso);
            RandomizeItem(items.ArmUpperRight, itemVariants.ArmUpperRight);
            RandomizeItem(items.ArmUpperLeft, itemVariants.ArmUpperLeft);
            RandomizeItem(items.ArmLowerRight, itemVariants.ArmLowerRight);
            RandomizeItem(items.ArmLowerLeft, itemVariants.ArmLowerLeft);
            RandomizeItem(items.HandRight, itemVariants.HandRight);
            RandomizeItem(items.HandLeft, itemVariants.HandLeft);
            RandomizeItem(items.Hips, itemVariants.Hips);
            RandomizeItem(items.LegRight, itemVariants.LegRight);
            RandomizeItem(items.LegLeft, itemVariants.LegLeft);
        }

        private void RandomizeAgenderItems()
        {
            RandomizeItem(_agenderItems.Hair, _agenderItemVariants.Hair);
            RandomizeItem(_agenderItems.Ears, _agenderItemVariants.Ears);
            RandomizeItem(_agenderItems.HeadCoveringHair, _agenderItemVariants.HeadCoveringHair);
            RandomizeItem(_agenderItems.BackAttachment, _agenderItemVariants.BackAttachment);
            RandomizeItem(_agenderItems.HipsAttachment, _agenderItemVariants.HipsAttachment);
        }

        private void RandomizeItem(SkinnedMeshRenderer renderer, List<Mesh> variants)
        {
            if (variants.Count == 0)
            {
                return;
            }

            var itemMesh = variants[Random.Range(0, variants.Count)];

            renderer.sharedMesh = itemMesh;
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

        [Serializable]
        private class GenderItemVariants
        {
            public List<Mesh> Head;
            public List<Mesh> HeadAccessory;
            public List<Mesh> Eyebrows;
            public List<Mesh> FacialHair;
            public List<Mesh> Torso;
            public List<Mesh> ArmUpperRight;
            public List<Mesh> ArmUpperLeft;
            public List<Mesh> ArmLowerRight;
            public List<Mesh> ArmLowerLeft;
            public List<Mesh> HandRight;
            public List<Mesh> HandLeft;
            public List<Mesh> Hips;
            public List<Mesh> LegRight;
            public List<Mesh> LegLeft;
        }
        
        [Serializable]
        private class AgenderItemVariants
        {
            public List<Mesh> Hair;
            public List<Mesh> Ears;
            public List<Mesh> HeadCoveringHair;
            public List<Mesh> HeadCoveringNoHair;
            public List<Mesh> HeadCoveringNoFacialHair;
            public List<Mesh> Helmet;
            public List<Mesh> BackAttachment;
            public List<Mesh> ShoulderAttachmentRight;
            public List<Mesh> ShoulderAttachmentLeft;
            public List<Mesh> ElbowAttachmentRight;
            public List<Mesh> ElbowAttachmentLeft;
            public List<Mesh> HipsAttachment;
            public List<Mesh> KneeAttachmentRight;
            public List<Mesh> KneeAttachmentLeft;
        }
    }
}
