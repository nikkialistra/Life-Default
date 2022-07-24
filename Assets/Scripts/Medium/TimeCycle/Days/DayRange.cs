using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Medium.TimeCycle.Days
{
    [Serializable]
    public class DayRange
    {
        [Range(0, 23)]
        [SerializeField] private int _lowThreshold;
        [Range(0, 23)]
        [SerializeField] private int _highThreshold;

        public int GetRandomHour()
        {
            return Random.Range(_lowThreshold, _highThreshold);
        }

        public bool Contains(int hour)
        {
            return hour >= _lowThreshold && hour <= _highThreshold;
        }
    }
}
