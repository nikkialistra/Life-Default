using System;
using System.Collections;
using Entities.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entities.Creature
{
    public class EntityHealth : MonoBehaviour, IDamageable
    {
        [ProgressBar(0, 1, r: 0.929f, g: 0.145f, b: 0.145f, Height = 20)]
        [SerializeField] private float _startHealth = 1;

        private float _health;

        private Coroutine _takingDamage;

        public event Action Die;
        public event Action<float> HealthChange;

        public float Health
        {
            get => _health;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Health));
                }

                _health = value;
            }
        }
        
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
        }

        public void TakeHealing(float value)
        {
            if (!IsAlive)
            {
                throw new InvalidOperationException("Healing cannot be applied to the died entity");
            }

            _health = Math.Min(_health + value, 1f);
        }

        public void TakeDamage(int value)
        {
            CheckTakeDamageValidity(value);

            _health -= value;

            if (!IsAlive)
            {
                StopTakingDamage();
                Die?.Invoke();
            }

            HealthChange?.Invoke(_health);
        }

        public void TakeDamageContinuously(int value, float interval, float time = float.PositiveInfinity)
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

        private IEnumerator TakingDamage(int value, float interval, float time)
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

        private void CheckTakeDamageValidity(int value)
        {
            if (value <= 0)
            {
                throw new ArgumentException("Damage must be more than zero");
            }
        }
    }
}
