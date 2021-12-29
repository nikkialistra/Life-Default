using System;
using System.Collections;
using Kernel.Types;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Kernel.Entities
{
    public class EntityHealth : MonoBehaviour, IDamageable
    {
        [MinValue(0)]
        [SerializeField] private int _maxHealth;
        [MinValue(0), ValidateInput("@_startHealth <= _maxHealth", "Start health cannon be greater than max health")]
        [SerializeField] private int _startHealth;

        public event Action Die;
        public event Action<int> HealthChange; 
        
        public int Health
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

        public int MaxHealth => _maxHealth;

        private int _health;
        private bool IsAlive => _health > 0;
        
        private Coroutine _takingDamage;
        
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

        public void TakeHealing(int value)
        {
            if (!IsAlive)
            {
                throw new InvalidOperationException("Healing cannot be applied to the died entity");
            }

            _health = Math.Min(_health + _maxHealth, _maxHealth);
        }

        public void TakeDamage(int value)
        {
            CheckTakeDamageValidity(value);

            _health -= value;
            HealthChange?.Invoke(_health);

            if (!IsAlive)
            {
                StopTakingDamage();
                Die?.Invoke();
            }
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

            if (!IsAlive)
            {
                throw new InvalidOperationException("Damage cannot be applied to the died entity");
            }
        }
    }
}