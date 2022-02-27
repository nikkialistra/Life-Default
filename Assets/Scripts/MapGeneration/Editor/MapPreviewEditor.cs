using MapGeneration;
using UnityEditor;
using UnityEngine;

namespace Map.Editor
{
    [CustomEditor(typeof(HeightMapPreview))]
    public class HeightMapPreviewEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var mapPreview = (HeightMapPreview)target;

            if (DrawDefaultInspector())
            {
                if (mapPreview.AutoUpdate)
                {
                    mapPreview.DrawMapInEditor();
                }
            }

            if (GUILayout.Button("Generate"))
            {
                mapPreview.DrawMapInEditor();
            }
        }
    }
}
