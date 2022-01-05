using System.IO;
using Common;
using UnityEditor;
using UnityEngine;

namespace MapGeneration
{
    public class MeshSaving : MonoBehaviour
    {
        public void Save()
        {
            var meshFilter = GetComponent<MeshFilter>();
            if (meshFilter)
            {
                SaveUtils.CreateBaseDirectoriesTo(SaveUtils.SavedAssetsPath);

                var path = Path.Combine(SaveUtils.SavedAssetsPath, $"{gameObject.name} Mesh.asset");
                AssetDatabase.CreateAsset(meshFilter.mesh, path);
            }
        }
    }
}