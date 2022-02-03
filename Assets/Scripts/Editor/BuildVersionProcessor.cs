using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Editor
{
    public class BuildVersionProcessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            UpdateVersion();
        }

        private static void UpdateVersion()
        {
            var currentVersion = PlayerSettings.bundleVersion;

            if (int.TryParse(currentVersion, out var version))
            {
                version++;
                PlayerSettings.bundleVersion = $"{version}";
            }
        }
    }
}
