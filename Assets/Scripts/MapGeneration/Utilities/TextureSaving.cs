using System.IO;
using UnityEditor;
using UnityEngine;
using static System.IO.File;

namespace MapGeneration.Utilities
{
    public static class TextureSaving
    {
        private const string TexturesPath = "Assets/Resources/HeightMaps";

        public static void ClearSavedTextures()
        {
            if (Directory.Exists(TexturesPath))
            {
                Directory.Delete(TexturesPath, true);
            }

            Directory.CreateDirectory(TexturesPath);
        }


#if UNITY_EDITOR
        
        public static void SaveTexture(Texture2D texture)
        {
            CreateBaseDirectoriesTo(TexturesPath);
            
            var bytes = texture.EncodeToPNG();
            var path = Path.Combine(TexturesPath, "HeightMap.png");
            
            WriteAllBytes(path, bytes);
            AssetDatabase.Refresh();
            Debug.Log($"Texture was saved to {path}");
        }

#endif

        private static void CreateBaseDirectoriesTo(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
