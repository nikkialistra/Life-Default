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
        [SerializeField] private GameObject _maleRoot;
        [SerializeField] private GameObject _femaleRoot;

        [SerializeField] private Items _maleItems;
        [SerializeField] private Items _femaleItems;

        [SerializeField] private ItemVariants _maleItemVariants;
        [SerializeField] private ItemVariants _femaleItemVariants;

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
                _maleRoot.SetActive(true);
                _femaleRoot.SetActive(false);
            }
            else
            {
                _maleRoot.SetActive(false);
                _femaleRoot.SetActive(true);
            }
            
            var (items, itemVariants) = gender switch
            {
                Gender.Male => (_maleItems, _maleItemVariants),
                Gender.Female => (_femaleItems, _femaleItemVariants),
                _ => throw new ArgumentOutOfRangeException(nameof(gender), gender, null)
            };

            ActivateItem(items.Head, itemVariants.Head);
            ActivateItem(items.Eyebrows, itemVariants.Eyebrows);
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

            if (gender == Gender.Male)
            {
                ActivateItem(items.FacialHair, itemVariants.FacialHair);
            }
        }

        private void ActivateItem(SkinnedMeshRenderer renderer, List<Mesh> meshVariants)
        {
            var randomMesh = meshVariants[Random.Range(0, meshVariants.Count)];

            renderer.sharedMesh = randomMesh;
        }

        [Serializable]
        private class Items
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
        private class ItemVariants
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
    }
}
