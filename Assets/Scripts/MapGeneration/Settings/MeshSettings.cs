using UnityEngine;

namespace MapGeneration.Settings
{
    [CreateAssetMenu]
    public class MeshSettings : UpdatableData
    {
        public const int NumSupportedLODs = 5;

        private const int NumSupportedChunkSizes = 9;
        private const int NumSupportedFlatShadedChunkSizes = 3;
        private static readonly int[] SupportedChunkSizes = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };

        [SerializeField] private float _meshScale = 2.5f;
        [SerializeField] private bool _useFlatShading;

        [Range(0, NumSupportedChunkSizes - 1)]
        [SerializeField] private int _chunkSizeIndex;
        [Range(0, NumSupportedFlatShadedChunkSizes - 1)]
        [SerializeField] private int _flatShadedChunkSizeIndex;

        // num verts per line of mesh rendered at LOD = 0. Includes the 2 extra verts that are excluded from final mesh, but used for calculating normals
        public int NumVertsPerLine =>
            SupportedChunkSizes[(_useFlatShading) ? _flatShadedChunkSizeIndex : _chunkSizeIndex] + 5;

        public float MeshWorldSize => (NumVertsPerLine - 3) * _meshScale;

        public float MeshScale => _meshScale;

        public bool UseFlatShading => _useFlatShading;
    }
}