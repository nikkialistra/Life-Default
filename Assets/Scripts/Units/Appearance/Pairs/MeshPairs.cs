using System;
using System.Collections.Generic;
using UnityEngine;

namespace Units.Appearance.Pairs
{
    [Serializable]
    public class MeshPairs
    {
        [SerializeField] private List<MeshPair> _meshPairs;

        public void Clear()
        {
            _meshPairs.Clear();
        }

        public bool IsCompatibleWith(Mesh mesh)
        {
            foreach (var meshPair in _meshPairs)
            {
                if (meshPair.HasMesh(mesh))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
