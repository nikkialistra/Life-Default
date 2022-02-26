using System;

namespace Environment.WeatherRegulation
{
    public static class WeatherExtensions
    {
        public static string GetString(this Weather weather)
        {
            return weather switch {
                Weather.Clear => "Clear",
                Weather.Rain => "Rain",
                Weather.Hail => "Hail",
                Weather.Fog => "Fog",
                Weather.FoggyRain => "Foggy rain",
                Weather.Thunderstorm => "Thunderstorm",
                Weather.RainyThunderstorm => "Rainy thunderstorm",
                Weather.Snow => "Snow",
                Weather.Snowstorm => "Snowstorm",
                Weather.SnowThunderstorm => "Snow thunderstorm",
                Weather.AcidRain => "Acid rain",
                Weather.HeatWave => "Heat wave",
                Weather.MagneticStorm => "Magnetic storm",
                Weather.MeteorShower => "Meteor shower",
                _ => throw new ArgumentOutOfRangeException(nameof(weather), weather, null)
            };
        }
    }
}
