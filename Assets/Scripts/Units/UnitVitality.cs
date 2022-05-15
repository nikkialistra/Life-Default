using System;
using System.Collections;
using System.Collections.Generic;
using General.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Units
{
    public class UnitVitality : MonoBehaviour, IDamageable
    {
        [MinValue(1)]
        [SerializeField] private float _maxRecoverySpeed = 3;

        [ValidateInput("@_possibleMaxHealths.Count > 0", "Max health variant count should be greater than zero")]
        [SerializeField] private List<int> _possibleMaxHealths;
        
        [ValidateInput("@_possibleRecoverySpeeds.Count > 0", "Recovery speed variant count should be greater than zero")]
        [SerializeField] private List<float> _possibleRecoverySpeeds;

        private Coroutine _takingDamageCoroutine;

        public event Action Wasted;
        public event Action<float, float> HealthChange;

        public float Health { get; private set; }
        public float MaxHealth { get; private set; }

        public float RecoverySpeed { get; private set; }
        public float MaxRecoverySpeed => _maxRecoverySpeed;

        public int HealthPercent => (int)((Health / MaxHealth) * 100);
        public int RecoverySpeedPercent => (int)((RecoverySpeed / _maxRecoverySpeed) * 100);
        
        private bool IsAlive => Health > 0;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IHittable hittable))
            {
                TakeDamageContinuously(hittable.Damage, hittable.Interval);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out IHittable _))
            {
                StopTakingDamage();
            }
        }

        public void Initialize()
        {
            MaxHealth = _possibleMaxHealths[Random.Range(0, _possibleMaxHealths.Count)];
            Health = MaxHealth;
            
            _maxRecoverySpeed = _possibleRecoverySpeeds[Random.Range(0, _possibleRecoverySpeeds.Count)];
            RecoverySpeed = _maxRecoverySpeed;
        }

        public void TakeHealing(float value)
        {
            if (!IsAlive)
            {
                throw new InvalidOperationException("Healing cannot be applied to the died entity");
            }

            Health = Math.Min(Health + value, MaxHealth);
        }

        public void TakeDamage(float value)
        {
            CheckTakeDamageValidity(value);

            Health -= value;

            if (!IsAlive)
            {
                StopTakingDamage();
                Wasted?.Invoke();
            }

            HealthChange?.Invoke(Health / MaxHealth, RecoverySpeed / _maxRecoverySpeed);
        }

        public void TakeDamageContinuously(float value, float interval, float time = float.PositiveInfinity)
        {
            if (_takingDamageCoroutine != null)
            {
                StopCoroutine(_takingDamageCoroutine);
            }

            _takingDamageCoroutine = StartCoroutine(TakingDamage(value, interval, time));
        }

        public void StopTakingDamage()
        {
            if (_takingDamageCoroutine != null)
            {
                StopCoroutine(_takingDamageCoroutine);
                _takingDamageCoroutine = null;
            }
        }

        private IEnumerator TakingDamage(float value, float interval, float time)
        {
            yield return null;

            var elapsedTime = 0.0f;

            while (elapsedTime < time)
            {
                TakeDamage(value);
                yield return new WaitForSeconds(interval);
                elapsedTime += interval;
            }
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
