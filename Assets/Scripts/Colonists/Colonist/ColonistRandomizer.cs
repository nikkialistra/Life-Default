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
        [SerializeField] private ItemRoots _itemRoots;

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
            var itemVariants = gender == Gender.Male ? _maleItemVariants : _femaleItemVariants;
            
            ActivateItem(_itemRoots.Head, itemVariants.Head);
            ActivateItem(_itemRoots.Eyebrows, itemVariants.Eyebrows);
            ActivateItem(_itemRoots.Torso, itemVariants.Torso);
            ActivateItem(_itemRoots.ArmUpperRight, itemVariants.ArmUpperRight);
            ActivateItem(_itemRoots.ArmUpperLeft, itemVariants.ArmUpperLeft);
            ActivateItem(_itemRoots.ArmLowerRight, itemVariants.ArmLowerRight);
            ActivateItem(_itemRoots.ArmLowerLeft, itemVariants.ArmLowerLeft);
            ActivateItem(_itemRoots.HandRight, itemVariants.HandRight);
            ActivateItem(_itemRoots.HandLeft, itemVariants.HandLeft);
            ActivateItem(_itemRoots.Hips, itemVariants.Hips);
            ActivateItem(_itemRoots.LegRight, itemVariants.LegRight);
            ActivateItem(_itemRoots.LegLeft, itemVariants.LegLeft);

            if (gender == Gender.Male)
            {
                ActivateItem(_itemRoots.FacialHair, itemVariants.FacialHair);
            }
            else
            {
                _itemRoots.FacialHair.sharedMesh = null;
            }
        }

        private void ActivateItem(SkinnedMeshRenderer root, List<Mesh> meshVariants)
        {
            var randomMesh = meshVariants[Random.Range(0, meshVariants.Count)];

            root.sharedMesh = randomMesh;
        }

        [Serializable]
        private class ItemRoots
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
