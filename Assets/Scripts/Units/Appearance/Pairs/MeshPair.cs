using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Units.Appearance.Pairs
{
    [Serializable]
    public class MeshPair
    {
        [HorizontalGroup("Split", 100)]
        [VerticalGroup("Split/Left")]
        [HideLabel]
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [SerializeField] private Mesh _firshMesh;
        [VerticalGroup("Split/Right")]
        [HideLabel]
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [SerializeField] private Mesh _secondMesh;

        public bool HasMesh(Mesh mesh)
        {
            return _firshMesh == mesh || _secondMesh == mesh;
        }
    }
}
