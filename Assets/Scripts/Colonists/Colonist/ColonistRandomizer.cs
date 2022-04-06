using System;
using System.Collections.Generic;
using Common;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Colonists.Colonist
{
    public class ColonistRandomizer : MonoBehaviour
    {
        [SerializeField] private GameObject _male;
        [SerializeField] private GameObject _female;

        [SerializeField] private GenderItems _maleItems;
        [SerializeField] private GenderItems _femaleItems;
        [SerializeField] private AgenderItems _agenderItems;

        [SerializeField] private GenderItemVariants _maleItemVariants;
        [SerializeField] private GenderItemVariants _femaleItemVariants;
        [SerializeField] private AgenderItemVariants _agenderItemVariants;

        private enum Gender { Male, Female }

        private void Start()
        {
            RandomizeAppearance();
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

            ActivateGenderItems(items, itemVariants);
            ActivateAgenderItems();
        }

        private void ActivateGenderItems(GenderItems items, GenderItemVariants itemVariants)
        {
            ActivateItem(items.Head, itemVariants.Head);
            ActivateItem(items.Eyebrows, itemVariants.Eyebrows);
            ActivateItem(items.FacialHair, itemVariants.FacialHair);
            ActivateItem(items.Torso, itemVariants.Torso);
            ActivateItem(items.ArmUpperRight, itemVariants.ArmUpperRight);
            ActivateItem(items.ArmUpperLeft, itemVariants.ArmUpperLeft);
            ActivateItem(items.ArmLowerRight, itemVariants.ArmLowerRight);
            ActivateItem(items.ArmLowerLeft, itemVariants.ArmLowerLeft);
            ActivateItem(items.HandRight, itemVariants.HandRight);
            ActivateItem(items.HandLeft, itemVariants.HandLeft);
            ActivateItem(items.Hips, itemVariants.Hips);
            ActivateItem(items.LegRight, itemVariants.LegRight);
            ActivateItem(items.LegLeft, itemVariants.LegLeft);
        }

        private void ActivateAgenderItems()
        {
            ActivateItem(_agenderItems.Hair, _agenderItemVariants.Hair);
            ActivateItem(_agenderItems.HeadCoveringHair, _agenderItemVariants.HeadCoveringHair);
            ActivateItem(_agenderItems.BackAttachment, _agenderItemVariants.BackAttachment);
            ActivateItem(_agenderItems.HipsAttachment, _agenderItemVariants.HipsAttachment);
            ActivateItem(_agenderItems.Ears, _agenderItemVariants.Ears);
        }

        private void ActivateItem(SkinnedMeshRenderer renderer, List<Mesh> meshVariants)
        {
            if (renderer == null)
            {
                return;
            }
            
            if (meshVariants.Count == 0)
            {
                return;
            }
            
            var randomMesh = meshVariants[Random.Range(0, meshVariants.Count)];

            renderer.sharedMesh = randomMesh;
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
