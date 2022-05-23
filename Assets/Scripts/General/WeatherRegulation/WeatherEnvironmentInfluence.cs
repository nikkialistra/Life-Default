using General.TimeCycle.Ticking;
using UnityEngine;
using Zenject;
using ITickable = General.TimeCycle.Ticking.ITickable;

namespace General.WeatherRegulation
{
    public class WeatherEnvironmentInfluence : MonoBehaviour, ITickable
    {
        [SerializeField] private int _ticksToShiftTemperature = 15;
        [SerializeField] private int _ticksToShiftLight = 7;

        private float _temperatureValueShiftPerTick;
        private bool _shiftingTemperature;
        private int _temperatureShiftTickCount;

        private float _lightValueShiftPerTick;
        private bool _shiftingLight;
        private int _lightShiftTickCount;
        
        private WeatherEffectsRegistry _weatherEffectsRegistry;

        [Inject]
        public void Construct(WeatherEffectsRegistry weatherEffectsRegistry, TickingRegulator tickingRegulator)
        {
            _weatherEffectsRegistry = weatherEffectsRegistry;
            tickingRegulator.AddToTickables(this);
        }
        
        public float TemperatureSumModifier { get; private set; }

        public float LightSumModifier { get; private set; }

        public void Tick()
        {
            if (_shiftingTemperature)
            {
                ShiftTemperature();
                CheckForTemperatureShiftFinish();
            }

            if (_shiftingLight)
            {
                ShiftLight();
                CheckForLightShiftFinish();
            }
        }

        public void ChangeWeather(Weather weather)
        {
            var weatherEffects = _weatherEffectsRegistry[weather];
            
            ShiftTemperatureBy(weatherEffects.TemperatureChange);
            ShiftLightBy(weatherEffects.LightChange);
        }

        private void ShiftTemperatureBy(int temperatureChange)
        {
            _temperatureValueShiftPerTick =
                (temperatureChange - TemperatureSumModifier) / _ticksToShiftTemperature;
            _shiftingTemperature = true;
        }

        private void ShiftLightBy(int lightChange)
        {
            _lightValueShiftPerTick = (lightChange - LightSumModifier) / _ticksToShiftLight;
            _shiftingLight = true;
        }

        private void ShiftTemperature()
        {
            TemperatureSumModifier += _temperatureValueShiftPerTick;
            _temperatureShiftTickCount++;
        }

        private void CheckForTemperatureShiftFinish()
        {
            if (_temperatureShiftTickCount == _ticksToShiftTemperature)
            {
                _shiftingTemperature = false;
                _temperatureShiftTickCount = 0;
            }
        }

        private void ShiftLight()
        {
            LightSumModifier += _lightValueShiftPerTick;
            _lightShiftTickCount++;
        }

        private void CheckForLightShiftFinish()
        {
            if (_lightShiftTickCount == _ticksToShiftLight)
            {
                _shiftingLight = false;
                _lightShiftTickCount = 0;
            }
        }
    }
}
