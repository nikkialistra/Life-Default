using System.IO;

namespace Common
{
    public class SaveUtils
    {
        public const string SavedAssetsPath = "Assets/Resources/SavedAssets";
        public const string SavedResourcesPath = "SavedAssets";

        public static void CreateBaseDirectoriesTo(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}