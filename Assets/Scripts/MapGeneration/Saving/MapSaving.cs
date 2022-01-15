using System.IO;
using UnityEditor;
using UnityEngine;

namespace MapGeneration.Saving
{
    public static class MapSaving
    {
        public const string SavedAssetsPath = "Assets/Resources/SavedAssets";

        private const string SavedResourcesPath = "SavedAssets";

        public static void CreateBaseDirectoriesTo(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static GameObject GetSavedPrefab(string name)
        {
            var path = Path.Combine(SavedResourcesPath, name);
            var prefab = Resources.Load<GameObject>(path);
            return prefab;
        }

        public static void ClearSavedAssets()
        {
            if (Directory.Exists(SavedAssetsPath))
            {
                Directory.Delete(SavedAssetsPath, true);
            }

            Directory.CreateDirectory(SavedAssetsPath);
        }


#if UNITY_EDITOR

        public static void SaveMesh(Mesh mesh, string name)
        {
            CreateBaseDirectoriesTo(SavedAssetsPath);

            var path = Path.Combine(MapSaving.SavedAssetsPath, name);
            AssetDatabase.CreateAsset(mesh, path);
        }

        public static void SavePrefab(GameObject gameObject, string name)
        {
            CreateBaseDirectoriesTo(SavedAssetsPath);

            var path = Path.Combine(SavedAssetsPath, name);
            PrefabUtility.SaveAsPrefabAsset(gameObject, path);
        }

#endif
    }
}
