using System;
using System.Collections.Generic;
using Colonists.Colonist.Appearance;
using Common;
using Entities.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Colonists.Colonist
{
    public class ColonistRandomizer : MonoBehaviour
    {
        [Title("Bindings")]
        [SerializeField] private GameObject _male;
        [SerializeField] private GameObject _female;

        [SerializeField] private GenderItems _maleItems;
        [SerializeField] private GenderItems _femaleItems;
        [SerializeField] private AgenderItems _agenderItems;

        [Title("Variants")]
        [SerializeField] private GenderItemVariants _maleItemVariants;
        [SerializeField] private GenderItemVariants _femaleItemVariants;
        [SerializeField] private AgenderItemVariants _agenderItemVariants;

        private void Start()
        {
            RandomizeAppearance();
        }

        public void RandomizeAppearanceWith(HeadVariants headVariants)
        {
            RandomizeItem(_maleItems.Head, headVariants.Head);
            RandomizeItem(_agenderItems.Hair, headVariants.Hair);
            RandomizeItem(_agenderItems.Ears, headVariants.Ears);
            RandomizeItem(_maleItems.Eyebrows, headVariants.Eyebrows);
            RandomizeItem(_maleItems.FacialHair, headVariants.FacialHair);
        }

        [Button(ButtonSizes.Medium)]
        private void RandomizeAppearance()
        {
            var randomGender = EnumUtils.RandomEnumValue<Gender>();

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

        private void RandomizeItem(SkinnedMeshRenderer renderer, List<Mesh> meshVariants)
        {
            if (meshVariants.Count == 0)
            {
                return;
            }
            
            var randomMesh = meshVariants[Random.Range(0, meshVariants.Count)];

            renderer.sharedMesh = randomMesh;
        }

        private void RandomizeItem(SkinnedMeshRenderer maleItemsHead, List<Item> variants)
        {
            
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
