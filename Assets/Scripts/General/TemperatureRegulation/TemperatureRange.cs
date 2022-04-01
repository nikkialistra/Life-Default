using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace General.TemperatureRegulation
{
    [Serializable]
    public class TemperatureRange
    {
        [Range(-273, 273)]
        [SerializeField] private int _lowThreshold;
        [Range(-273, 273)]
        [SerializeField] private int _highThreshold;

        public int GetRandomTemperature()
        {
            return Random.Range(_lowThreshold, _highThreshold);
        }

        public bool Contains(int temperature)
        {
            return temperature >= _lowThreshold && temperature <= _highThreshold;
        }
    }
}
