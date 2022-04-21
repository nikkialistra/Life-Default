using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Units.Appearance.ItemVariants;
using Units.Appearance.Pairs;
using UnityEngine;
using UnityEngine.Serialization;

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

        [Title("Not Combinable")]
        [SerializeField] private MeshPairs _notCombinablePairs;

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
            _notCombinablePairs.ClearTaken();
        }

        public Mesh GetElement(GarmentElements garmentElements)
        {
            Mesh mesh;
            
            while (true)
            {
                mesh = GetMeshFor(garmentElements);

                if (TryTakeMesh(mesh))
                {
                    break;
                }
            }

            return mesh;
        }

        public MeshPair GetElementsAtSameIndex(GarmentElementPairs garmentElementPairs)
        {
            MeshPair meshPair;
            
            while (true)
            {
                meshPair = GetMeshPairFor(garmentElementPairs);

                if (TryTakeMesh(meshPair.FirstMesh) || TryTakeMesh(meshPair.SecondMesh))
                {
                    break;
                }
            }

            return meshPair;
        }

        private bool TryTakeMesh(Mesh mesh)
        {
            if (mesh == null)
            {
                return true;
            }

            if (_notCombinablePairs.IsCompatibleWith(mesh))
            {
                _notCombinablePairs.AddToTaken(mesh);
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
                GarmentElements.Hips => Hips.GetRandom(),
                GarmentElements.HipsAttachment => HipsAttachment.GetRandom(),
                _ => throw new ArgumentOutOfRangeException(nameof(garmentElements), garmentElements, null)
            };
        }

        private MeshPair GetMeshPairFor(GarmentElementPairs garmentElementPairs)
        {
            var index = garmentElementPairs switch
            {
                GarmentElementPairs.ArmsUpper => ArmUpperRight.GetRandomIndex(),
                GarmentElementPairs.ArmsLower => ArmLowerRight.GetRandomIndex(),
                GarmentElementPairs.Hands => HandRight.GetRandomIndex(),
                GarmentElementPairs.Legs => LegRight.GetRandomIndex(),
                _ => throw new ArgumentOutOfRangeException(nameof(garmentElementPairs), garmentElementPairs, null)
            };

            return garmentElementPairs switch
            {
                GarmentElementPairs.ArmsUpper => MeshPair.Create(ArmUpperRight.GetAtIndex(index), ArmUpperLeft.GetAtIndex(index)),
                GarmentElementPairs.ArmsLower => MeshPair.Create(ArmLowerRight.GetAtIndex(index), ArmLowerLeft.GetAtIndex(index)),
                GarmentElementPairs.Hands => MeshPair.Create(HandRight.GetAtIndex(index), HandLeft.GetAtIndex(index)),
                GarmentElementPairs.Legs => MeshPair.Create(LegRight.GetAtIndex(index), LegLeft.GetAtIndex(index)),
                _ => throw new ArgumentOutOfRangeException(nameof(garmentElementPairs), garmentElementPairs, null)
            };
        }

        private IItemVariants<Mesh> HeadCoveringHair => _headCoveringHairVariants;

        private IItemVariants<Mesh> Torso => _torsoVariants;
        private IItemVariants<Mesh> BackAttachment => _backAttachmentVariants;

        private IItemVariants<Mesh> ArmUpperRight => _armUpperRightVariants;
        private IItemVariants<Mesh> ArmUpperLeft => _armUpperLeftVariants;
        private IItemVariants<Mesh> ArmLowerRight => _armLowerRightVariants;
        private IItemVariants<Mesh> ArmLowerLeft => _armLowerLeftVariants;

        private IItemVariants<Mesh> HandRight => _handRightVariants;
        private IItemVariants<Mesh> HandLeft => _handLeftVariants;

        private IItemVariants<Mesh> Hips => _hipsVariants;
        private IItemVariants<Mesh> HipsAttachment => _hipsAttachmentVariants;

        private IItemVariants<Mesh> LegRight => _legRightVariants;
        private IItemVariants<Mesh> LegLeft => _legLeftVariants;

        public enum GarmentElements
        {
            HeadCoveringHair,
            Torso,
            BackAttachment,
            Hips,
            HipsAttachment,
        }
        
        public enum GarmentElementPairs
        {
            ArmsUpper,
            ArmsLower,
            Hands,
            Legs
        }
    }
}
