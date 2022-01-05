using MapGeneration.Generators;
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
                var savePath = $"{MapGenerator.SavePath}/{gameObject.name} Mesh.asset";
                AssetDatabase.CreateAsset(meshFilter.mesh, savePath);
            }
        }
    }
}