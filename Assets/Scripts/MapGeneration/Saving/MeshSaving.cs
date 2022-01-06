using UnityEngine;

namespace MapGeneration.Saving
{
    public class MeshSaving : MonoBehaviour
    {
#if UNITY_EDITOR

        public void Save()
        {
            var meshFilter = GetComponent<MeshFilter>();
            if (meshFilter)
            {
                MapSaving.SaveMesh(meshFilter.mesh, $"{gameObject.name} Mesh.asset");
            }
        }

#endif
    }
}