using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> _keys = new();

        [SerializeField]
        private List<TValue> _values = new();

        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();

            foreach (var (key, value) in this)
            {
                _keys.Add(key);
                _values.Add(value);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();

            if (_keys.Count != _values.Count)
            {
                throw new Exception(string.Format(
                    "there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));
            }

            for (var i = 0; i < _keys.Count; i++)
            {
                Add(_keys[i], _values[i]);
            }
        }
    }
}