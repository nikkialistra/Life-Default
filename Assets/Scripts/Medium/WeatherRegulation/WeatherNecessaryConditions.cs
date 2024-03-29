﻿using System;
using Medium.TemperatureRegulation;
using Medium.TimeCycle.Seasons;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Medium.WeatherRegulation
{
    [Serializable]
    public class WeatherNecessaryConditions
    {
        [EnumToggleButtons]
        [SerializeField] private Season _seasons;
        [SerializeField] private TemperatureRange _temperatureRange;

        public bool SuitableWith(Season season, int dayTemperature)
        {
            return _seasons.HasFlag(season) && _temperatureRange.Contains(dayTemperature);
        }

        public bool SuitableWith(int dayTemperature)
        {
            return _temperatureRange.Contains(dayTemperature);
        }
    }
}
