using Sirenix.OdinInspector;
using Units.Appearance.ItemVariants;
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
    }
}
