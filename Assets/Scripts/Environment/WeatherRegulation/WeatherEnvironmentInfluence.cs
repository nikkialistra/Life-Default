using Environment.TimeCycle.Ticking;
using UnityEngine;
using Zenject;

namespace Environment.WeatherRegulation
{
    public class WeatherEnvironmentInfluence : MonoBehaviour, ITicking
    {
        [SerializeField] private int _ticksToShiftTemperature = 15;
        [SerializeField] private int _ticksToShiftLight = 7;

        private float _temperatureValueShiftPerTick;
        private bool _shiftingTemperature;
        private int _temperatureShiftTickCount;

        private float _lightValueShiftPerTick;
        private bool _shiftingLight;
        private int _lightShiftTickCount;

        [Inject]
        public void Construct(TickingRegulator tickingRegulator)
        {
            tickingRegulator.AddToTickables(this);
        }

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

        public float TemperatureSumModifier { get; private set; }

        public float LightSumModifier { get; private set; }

        public void ShiftTemperatureBy(int temperatureDelta)
        {
            TemperatureSumModifier = 0;
            _temperatureValueShiftPerTick = (float)temperatureDelta / _ticksToShiftTemperature;
            _shiftingTemperature = true;
        }

        public void ShiftLightBy(int lightDelta)
        {
            LightSumModifier = 0;
            _lightValueShiftPerTick = (float)lightDelta / _ticksToShiftLight;
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
