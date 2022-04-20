using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Units
{
    public class UnitVitality : MonoBehaviour, IDamageable
    {
        [MinValue(1)]
        [SerializeField] private float _maxHealth = 100;
        [MinValue(1)]
        [SerializeField] private float _maxRecoverySpeed = 3;

        [ValidateInput("@_possibleMaxHealths.Count > 0", "Max health variant count should be greater than zero")]
        [SerializeField] private List<int> _possibleMaxHealths;
        
        [ValidateInput("@_possibleRecoverySpeeds.Count > 0", "Recovery speed variant count should be greater than zero")]
        [SerializeField] private List<float> _possibleRecoverySpeeds;

        private float _health;
        private float _recoverySpeed;

        private Coroutine _takingDamageCoroutine;

        public event Action Die;
        public event Action<float, float> HealthChange;

        public float Health => _health;
        public float MaxHealth => _maxHealth;

        public float RecoverySpeed => _recoverySpeed;
        public float MaxRecoverySpeed => _maxRecoverySpeed;

        public int HealthPercent => (int)((Health / _maxHealth) * 100);
        public int RecoverySpeedPercent => (int)((RecoverySpeed / _maxRecoverySpeed) * 100);
        
        private bool IsAlive => _health > 0;

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
            _maxHealth = _possibleMaxHealths[Random.Range(0, _possibleMaxHealths.Count)];
            _health = _maxHealth;
            
            _maxRecoverySpeed = _possibleRecoverySpeeds[Random.Range(0, _possibleRecoverySpeeds.Count)];
            _recoverySpeed = _maxRecoverySpeed;
        }

        public void TakeHealing(float value)
        {
            if (!IsAlive)
            {
                throw new InvalidOperationException("Healing cannot be applied to the died entity");
            }

            _health = Math.Min(_health + value, _maxHealth);
        }

        public void TakeDamage(float value)
        {
            CheckTakeDamageValidity(value);

            _health -= value;

            if (!IsAlive)
            {
                StopTakingDamage();
                Die?.Invoke();
            }

            HealthChange?.Invoke(_health / _maxHealth, _recoverySpeed / _maxRecoverySpeed);
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
