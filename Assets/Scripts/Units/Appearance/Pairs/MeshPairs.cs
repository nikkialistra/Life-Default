using System;
using System.Collections.Generic;
using UnityEngine;

namespace Units.Appearance.Pairs
{
    [Serializable]
    public class MeshPairs
    {
        [SerializeField] private List<MeshPair> _meshPairs;

        private List<Mesh> _takenMeshes = new();
        

        public void ClearTaken()
        {
            _takenMeshes.Clear();
        }

        public bool IsCompatibleWith(Mesh mesh)
        {
            foreach (var meshPair in _meshPairs)
            {
                if (meshPair.HasPairFrom(mesh, _takenMeshes))
                {
                    return false;
                }
            }
            
            return true;
        }

        public void AddToTaken(Mesh mesh)
        {
            _takenMeshes.Add(mesh);
        }
    }
}
