using UnityEngine;

namespace MapGeneration.Settings
{
    public class UpdatableData : ScriptableObject
    {
        [SerializeField] private bool _autoUpdate;

#if UNITY_EDITOR

        public event System.Action OnValuesUpdated;

        protected virtual void OnValidate()
        {
            if (_autoUpdate)
            {
                UnityEditor.EditorApplication.update += NotifyOfUpdatedValues;
            }
        }

        public void NotifyOfUpdatedValues()
        {
            UnityEditor.EditorApplication.update -= NotifyOfUpdatedValues;
            OnValuesUpdated?.Invoke();
        }

#endif
    }
}
