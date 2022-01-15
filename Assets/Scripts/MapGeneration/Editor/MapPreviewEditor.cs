using MapGeneration.Map;
using UnityEditor;
using UnityEngine;

namespace MapGeneration.Editor
{
    [CustomEditor(typeof(MapPreview))]
    public class MapPreviewEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var mapPreview = (MapPreview)target;

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
