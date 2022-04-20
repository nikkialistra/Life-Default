using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Units.Appearance.Variants
{
    [CreateAssetMenu(fileName = "Human Garment Variant", menuName = "Human Appearance/Garment Variant", order = 3)]
    public class GarmentVariants : ScriptableObject
    {
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [SerializeField] private List<Mesh> _headCoveringHairVariants;
        [Space]
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [SerializeField] private List<Mesh> _torsoVariants;
        [Space]
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [SerializeField] private List<Mesh> _backAttachmentVariants;
        [Space]
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [SerializeField] private List<Mesh> _armUpperRightVariants;
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [SerializeField] private List<Mesh> _armUpperLeftVariants;
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [SerializeField] private List<Mesh> _armLowerRightVariants;
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [SerializeField] private List<Mesh> _armLowerLeftVariants;
        [Space]
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [SerializeField] private List<Mesh> _handRightVariants;
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [SerializeField] private List<Mesh> _handLeftVariants;
        [Space]
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [SerializeField] private List<Mesh> _hipsVariants;
        [Space]
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [SerializeField] private List<Mesh> _hipsAttachmentVariants;
        [Space]
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [SerializeField] private List<Mesh> _legRightVariants;
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [SerializeField] private List<Mesh> _legLeftVariants;

        public Mesh HeadCoveringHair => GetRandom(_headCoveringHairVariants);
        
        public Mesh Torso => GetRandom(_torsoVariants);
        public Mesh BackAttachment => GetRandom(_backAttachmentVariants);

        public Mesh ArmUpperRight => GetRandom(_armUpperRightVariants);
        public Mesh ArmUpperLeft => GetRandom(_armUpperLeftVariants);
        public Mesh ArmLowerRight => GetRandom(_armLowerRightVariants);
        public Mesh ArmLowerLeft => GetRandom(_armLowerLeftVariants);
        
        public Mesh HandRight => GetRandom(_handRightVariants);
        public Mesh HandLeft => GetRandom(_handLeftVariants);
        
        public Mesh Hips => GetRandom(_hipsVariants);
        public Mesh HipsAttachment => GetRandom(_hipsAttachmentVariants);
        
        public Mesh LegRight => GetRandom(_legRightVariants);
        public Mesh LegLeft => GetRandom(_legLeftVariants);

        private Mesh GetRandom(List<Mesh> variants)
        {
            if (variants.Count == 0)
            {
                return null;
            }
            
            return variants[Random.Range(0, variants.Count)];
        }
    }
}
