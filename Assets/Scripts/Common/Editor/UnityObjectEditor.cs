using UnityEditor;

namespace Common.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UnityEngine.Object), true)]
    public class UnityObjectEditor : UnityEditor.Editor
    {
    }
}
