using System;
using System.Collections.Generic;
using ColonistManagement.Tasking;
using Common;
using Sirenix.OdinInspector;
using Units.Appearance.ItemVariants;
using Units.Appearance.Pairs;
using UnityEngine;

namespace Units.Appearance
{
    [CreateAssetMenu(fileName = "Human Garment Set", menuName = "Human Appearance/Garment Set", order = 3)]
    public class GarmentSet : ScriptableObject
    {
        [SerializeField] private ItemObjectVariants<Mesh> _headCoveringHairVariants;
        [Space]
        [SerializeField] private ItemObjectVariants<Mesh> _torsoVariants;
        [Space]
        [SerializeField] private ItemObjectVariants<Mesh> _backAttachmentVariants;
        [Space]
        [SerializeField] private ItemObjectVariants<Mesh> _armUpperRightVariants;
        [SerializeField] private ItemObjectVariants<Mesh> _armUpperLeftVariants;
        [SerializeField] private ItemObjectVariants<Mesh> _armLowerRightVariants;
        [SerializeField] private ItemObjectVariants<Mesh> _armLowerLeftVariants;
        [Space]
        [SerializeField] private ItemObjectVariants<Mesh> _handRightVariants;
        [SerializeField] private ItemObjectVariants<Mesh> _handLeftVariants;
        [Space]
        [SerializeField] private ItemObjectVariants<Mesh> _hipsVariants;
        [Space]
        [SerializeField] private ItemObjectVariants<Mesh> _hipsAttachmentVariants;
        [Space]
        [SerializeField] private ItemObjectVariants<Mesh> _legRightVariants;
        [SerializeField] private ItemObjectVariants<Mesh> _legLeftVariants;
        
        [Title("Not Combined")]
        [SerializeField] private List<MeshPair> _notCombinedPairs;

        private readonly MeshPairs _takenElements = new();

        [Button]
        private void CalculateAllRelativeChances()
        {
            HeadCoveringHair.CalculateRelativeChancesForVariants();
            
            Torso.CalculateRelativeChancesForVariants();
            BackAttachment.CalculateRelativeChancesForVariants();
            
            ArmUpperRight.CalculateRelativeChancesForVariants();
            ArmUpperLeft.CalculateRelativeChancesForVariants();
            ArmLowerRight.CalculateRelativeChancesForVariants();
            ArmLowerLeft.CalculateRelativeChancesForVariants();
            
            HandRight.CalculateRelativeChancesForVariants();
            HandLeft.CalculateRelativeChancesForVariants();
            
            Hips.CalculateRelativeChancesForVariants();
            
            LegRight.CalculateRelativeChancesForVariants();
            LegLeft.CalculateRelativeChancesForVariants();
        }

        public void ResetTakeHistory()
        {
            _takenElements.Clear();
        }

        public Mesh GetElement(GarmentElements garmentElements)
        {
            Mesh mesh;
            
            while (true)
            {
                mesh = GetMeshFor(garmentElements);

                if (IsMeshEmptyOrCompatible(mesh))
                {
                    break;
                }
            }

            return mesh;
        }

        private bool IsMeshEmptyOrCompatible(Mesh mesh)
        {
            if (mesh == null || !_takenElements.IsCompatibleWith(mesh))
            {
                return true;
            }

            return false;
        }

        private Mesh GetMeshFor(GarmentElements garmentElements)
        {
            return garmentElements switch
            {
                GarmentElements.HeadCoveringHair => HeadCoveringHair.GetRandom(),
                GarmentElements.Torso => Torso.GetRandom(),
                GarmentElements.BackAttachment => BackAttachment.GetRandom(),
                GarmentElements.ArmUpperRight => ArmUpperRight.GetRandom(),
                GarmentElements.ArmUpperLeft => ArmUpperLeft.GetRandom(),
                GarmentElements.ArmLowerRight => ArmLowerRight.GetRandom(),
                GarmentElements.ArmLowerLeft => ArmLowerLeft.GetRandom(),
                GarmentElements.HandRight => HandRight.GetRandom(),
                GarmentElements.HandLeft => HandLeft.GetRandom(),
                GarmentElements.Hips => Hips.GetRandom(),
                GarmentElements.HipsAttachment => HipsAttachment.GetRandom(),
                GarmentElements.LegRight => LegRight.GetRandom(),
                GarmentElements.LegLeft => LegLeft.GetRandom(),
                _ => throw new ArgumentOutOfRangeException(nameof(garmentElements), garmentElements, null)
            };
        }

        public IItemVariants<Mesh> HeadCoveringHair => _headCoveringHairVariants;

        public IItemVariants<Mesh> Torso => _torsoVariants;
        public IItemVariants<Mesh> BackAttachment => _backAttachmentVariants;

        public IItemVariants<Mesh> ArmUpperRight => _armUpperRightVariants;
        public IItemVariants<Mesh> ArmUpperLeft => _armUpperLeftVariants;
        public IItemVariants<Mesh> ArmLowerRight => _armLowerRightVariants;
        public IItemVariants<Mesh> ArmLowerLeft => _armLowerLeftVariants;

        public IItemVariants<Mesh> HandRight => _handRightVariants;
        public IItemVariants<Mesh> HandLeft => _handLeftVariants;

        public IItemVariants<Mesh> Hips => _hipsVariants;
        public IItemVariants<Mesh> HipsAttachment => _hipsAttachmentVariants;

        public IItemVariants<Mesh> LegRight => _legRightVariants;
        public IItemVariants<Mesh> LegLeft => _legLeftVariants;

        public enum GarmentElements
        {
            HeadCoveringHair,
            Torso,
            BackAttachment,
            ArmUpperRight,
            ArmUpperLeft,
            ArmLowerRight,
            ArmLowerLeft,
            HandRight,
            HandLeft,
            Hips,
            HipsAttachment,
            LegRight,
            LegLeft
        }
    }
}
