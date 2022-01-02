using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Saving.Serialization.Surrogates;
using UnityEngine;

namespace Saving.Serialization
{
    public class Serialization
    {
        private BinaryFormatter _formatter;

        public void SaveToFile(string saveName, object saveData)
        {
            _formatter ??= GetBinaryFormatter();

            var basePath = Path.Combine(Application.persistentDataPath, "saves");
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            var path = Path.Combine(basePath, saveName);
            var file = File.Create(path);

            try
            {
                _formatter.Serialize(file, saveData);
            }
            catch (SerializationException exception)
            {
                Debug.Log("Failed to serialize. Reason: " + exception.Message);
                return;
            }
            finally
            {
                file.Close();
            }
        }

        public object LoadFromFile(string loadName)
        {
            var path = Path.Combine(Application.persistentDataPath, "saves", loadName);

            if (!File.Exists(path))
            {
                throw new ArgumentException(nameof(loadName));
            }

            _formatter ??= GetBinaryFormatter();

            var file = File.Open(path, FileMode.Open);

            try
            {
                var save = _formatter.Deserialize(file);
                return save;
            }
            catch
            {
                throw new FileLoadException($"Failed to load file at {loadName}");
            }
            finally
            {
                file.Close();
            }
        }

        private static BinaryFormatter GetBinaryFormatter()
        {
            var formatter = new BinaryFormatter();

            var selector = new SurrogateSelector();

            var vector3Surrogate = new Vector3SerializationSurrogate();
            var quaternionSurrogate = new QuaternionSerializationSurrogate();

            selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3Surrogate);
            selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All),
                quaternionSurrogate);

            formatter.SurrogateSelector = selector;

            return formatter;
        }
    }
}