using UnityEngine;

namespace MapGeneration.Data
{
    public class UpdatableData : ScriptableObject
    {
        [SerializeField] private bool _autoUpdate;
        public event System.Action OnValuesUpdated;

#if UNITY_EDITOR

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