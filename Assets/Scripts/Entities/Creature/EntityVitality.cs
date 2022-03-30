using System;
using System.Collections;
using Entities.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entities.Creature
{
    public class EntityVitality : MonoBehaviour, IDamageable
    {
        [MinValue(1)]
        [SerializeField] private float _maxHealth = 100;
        [MinValue(1)]
        [SerializeField] private float _maxRecoverySpeed = 3;

        [Space]
        [ProgressBar(0, nameof(_maxHealth), r: 0.929f, g: 0.145f, b: 0.145f, Height = 20)]
        [SerializeField] private float _startHealth = 100;
        [ProgressBar(0, nameof(_maxRecoverySpeed), r: 0.929f, g: 0.145f, b: 0.145f, Height = 20)]
        [SerializeField] private float _startRecoverySpeed = 3;

        private float _health;
        private float _recoverySpeed;

        private Coroutine _takingDamage;

        public event Action Die;
        public event Action<float, float> HealthChange;

        public float Health => _health;

        public float RecoverySpeed => _recoverySpeed;

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
            _health = _startHealth;
            _recoverySpeed = _startRecoverySpeed;
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
            if (_takingDamage != null)
            {
                StopCoroutine(_takingDamage);
            }

            _takingDamage = StartCoroutine(TakingDamage(value, interval, time));
        }

        public void StopTakingDamage()
        {
            if (_takingDamage != null)
            {
                StopCoroutine(_takingDamage);
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
