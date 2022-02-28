using UnityEditor;
using UnityEngine;

namespace MapGeneration.Editor
{
    [CustomEditor(typeof(HeightMapPreview))]
    public class HeightMapPreviewEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var heightMapPreview = (HeightMapPreview)target;

            if (DrawDefaultInspector())
            {
                if (heightMapPreview.AutoUpdate)
                {
                    heightMapPreview.DrawInEditor();
                }
            }
            
            if (GUILayout.Button("Export Texture"))
            {
                heightMapPreview.ExportTexture();
            }

            if (GUILayout.Button("Generate"))
            {
                heightMapPreview.DrawInEditor();
            }
        }
    }
}
