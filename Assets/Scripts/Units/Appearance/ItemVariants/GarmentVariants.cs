using Sirenix.OdinInspector;
using UnityEngine;

namespace Units.Appearance.ItemVariants
{
    [CreateAssetMenu(fileName = "Human Garment Variant", menuName = "Human Appearance/Garment Variant", order = 3)]
    public class GarmentVariants : ScriptableObject
    {
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [SerializeField] private Mesh _torso;
        [Space]
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [SerializeField] private Mesh _armUpperRight;
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [SerializeField] private Mesh _armUpperLeft;
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [SerializeField] private Mesh _armLowerRight;
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [SerializeField] private Mesh _armLowerLeft;
        [Space]
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [SerializeField] private Mesh _handRight;
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [SerializeField] private Mesh _handLeft;
        [Space]
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [SerializeField] private Mesh _hips;
        [Space]
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [SerializeField] private Mesh _legRight;
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [SerializeField] private Mesh _legLeft;

        public Mesh Torso => _torso;
        
        public Mesh ArmUpperRight => _armUpperRight;
        public Mesh ArmUpperLeft => _armUpperLeft;
        public Mesh ArmLowerRight => _armLowerRight;
        public Mesh ArmLowerLeft => _armLowerLeft;
        
        public Mesh HandRight => _handRight;
        public Mesh HandLeft => _handLeft;
        
        public Mesh Hips => _hips;
        
        public Mesh LegRight => _legRight;
        public Mesh LegLeft => _legLeft;
    }
}
