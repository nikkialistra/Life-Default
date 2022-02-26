using System;
using Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Environment.WeatherRegulation
{
    public class WeatherEffectsRegistry : MonoBehaviour
    {
        [ValidateInput("EveryWeatherHasEffects", "Not every weather has effects")]
        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
        [SerializeField] private WeatherEffectsDictionary _weatherEffects;
        
        private bool EveryWeatherHasEffects(WeatherEffectsDictionary effects, ref string errorMessage)
        {
            foreach (var weatherValue in (Weather[])Enum.GetValues(typeof(Weather)))
            {
                if (!effects.ContainsKey(weatherValue))
                {
                    errorMessage = $"{weatherValue} don't have effects";
                    return false;
                }
            }

            return true;
        }
        
        [Serializable] public class WeatherEffectsDictionary : SerializableDictionary<Weather, WeatherEffects> { }
    }
}
