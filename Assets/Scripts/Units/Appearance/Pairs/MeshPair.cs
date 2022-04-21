using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Units.Appearance.Pairs
{
    [Serializable]
    public class MeshPair
    {
        [HorizontalGroup("Split", 100)]
        [VerticalGroup("Split/Left")]
        [HideLabel]
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [SerializeField] private Mesh _firstMesh;
        [VerticalGroup("Split/Right")]
        [HideLabel]
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [SerializeField] private Mesh _secondMesh;

        public Mesh FirstMesh=> _firstMesh;
        
        public Mesh SecondMesh => _secondMesh;

        public static MeshPair Create(Mesh firstMesh, Mesh secondMesh)
        {
            var meshPair = new MeshPair
            {
                _firstMesh = firstMesh,
                _secondMesh = secondMesh
            };

            return meshPair;
        }

        public bool HasPairFrom(Mesh mesh, List<Mesh> takenMeshes)
        {
            var sameFirstMesh = mesh == _firstMesh;
            var sameSecondMesh = mesh == _secondMesh;

            if (!sameFirstMesh && !sameSecondMesh)
            {
                return false;
            }

            if (sameFirstMesh)
            {
                return takenMeshes.Contains(_secondMesh);
            }
            else
            {
                return takenMeshes.Contains(_firstMesh);
            }
        }
    }
}
