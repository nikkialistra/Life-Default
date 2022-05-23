using System;
using General.TimeCycle.Ticking;
using UnityEngine;

namespace Units.Calculations
{
    public class VitalityCalculation : ITickablePerHour
    {
        private float _health;
        private float _maxHealth;
        
        private float _recoverySpeed;
        private float _maxRecoverySpeed;

        private float _healthFractionToDecreaseRecoverySpeed;
        private float _recoveryHealthDelayAfterHit;

        private float _lastHitTime;

        private bool _initialized;

        public void Initialize(float maxHealth, float maxRecoverySpeed, float healthFractionToDecreaseRecoverySpeed, float recoveryHealthDelayAfterHit)
        {
            _initialized = true;
            
            _health = maxHealth;
            _maxHealth = maxHealth;

            _recoverySpeed = maxRecoverySpeed;
            _maxRecoverySpeed = maxRecoverySpeed;
            
            _healthFractionToDecreaseRecoverySpeed = healthFractionToDecreaseRecoverySpeed;
            _recoveryHealthDelayAfterHit = recoveryHealthDelayAfterHit;
        }

        public event Action<float> HealthChange;
        public event Action<float> RecoverySpeedChange;
        
        public void TickPerHour()
        {
            if (!_initialized || Time.time - _lastHitTime < _recoveryHealthDelayAfterHit)
            {
                return;
            }

            RecoverHealth();
        }

        private float BoundaryHealth => _maxHealth * _healthFractionToDecreaseRecoverySpeed;

        public void TakeDamage(float value)
        {
            CheckTakeDamageValidity(value);
            
            var oldHealth = _health;

            ReduceHealth(value);

            if (_health < BoundaryHealth)
            {
                CalculateRecoverySpeed(oldHealth);
            }
        }

        public void ChangeMaxHealth(float value)
        {
            if (value < 1f)
            {
                throw new ArgumentException("Max health cannot be less than 1");
            }

            var oldHealth = _health;
            
            _health = Mathf.Min(_health, _maxHealth);

            if (_health != oldHealth)
            {
                HealthChange?.Invoke(_health);
            }
        }

        public void ChangeMaxRecoverySpeed(float value)
        {
            if (value < 1f)
            {
                throw new ArgumentException("Max recovery speed cannot be less than 1");
            }

            var oldRecoverySpeed = _recoverySpeed;
            
            _recoverySpeed = Mathf.Min(_recoverySpeed, _maxRecoverySpeed);

            if (_recoverySpeed != oldRecoverySpeed)
            {
                RecoverySpeedChange?.Invoke(_recoverySpeed);
            }
        }

        private void RecoverHealth()
        {
            _health = Math.Min(_health + _recoverySpeed, _maxHealth);
            HealthChange?.Invoke(_health);
        }

        private void ReduceHealth(float value)
        {
            _lastHitTime = Time.time;
            
            _health -= value;
            HealthChange?.Invoke(_health);
        }

        private void CalculateRecoverySpeed(float oldHealth)
        {
            var differenceBelowBoundary = oldHealth > BoundaryHealth ? BoundaryHealth - _health : oldHealth - _health;

            var fraction = differenceBelowBoundary / _maxHealth;

            var newRecoverySpeed = _recoverySpeed - (_maxRecoverySpeed * fraction);

            _recoverySpeed = Mathf.Max(newRecoverySpeed, -_maxRecoverySpeed);
            RecoverySpeedChange?.Invoke(_recoverySpeed);
        }

        private void CheckTakeDamageValidity(float value)
        {
            if (value <= 0)
            {
                throw new ArgumentException("Damage must be more than zero");
            }
        }
    }
}
