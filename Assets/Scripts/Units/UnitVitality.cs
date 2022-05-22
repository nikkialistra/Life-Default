using System;
using System.Collections;
using System.Collections.Generic;
using General.Interfaces;
using Sirenix.OdinInspector;
using Units.Humans.Animations;
using Units.Stats;
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

        [Required]
        [SerializeField] private HumanAnimations _humanAnimations;

        public event Action Wasted;
        public event Action<float, float> HealthChange;

        public float Health { get; private set; }
        public float MaxHealth { get; private set; }

        public float RecoverySpeed { get; private set; }
        public float MaxRecoverySpeed { get; private set; }

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

        public void BindStats(Stat maxHealth, Stat maxRecoverySpeed)
        {
            MaxHealth = maxHealth.Value;
            MaxRecoverySpeed = maxRecoverySpeed.Value;

            maxHealth.ValueChange += ChangeMaxHealth;
            maxRecoverySpeed.ValueChange += ChangeMaxRecoverySpeed;
        }

        public void UnbindStats(Stat maxHealth, Stat maxRecoverySpeed)
        {
            maxHealth.ValueChange -= ChangeMaxHealth;
            maxRecoverySpeed.ValueChange -= ChangeMaxRecoverySpeed;
        }
        
        public void SetInitialValues()
        {
            Health = MaxHealth;
            RecoverySpeed = MaxRecoverySpeed;
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

            _humanAnimations.Hit();
            
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
        
        private void ChangeMaxHealth(float value)
        {
            if (value < 1f)
            {
                throw new ArgumentException("Max health cannot be less than 1");
            }
            
            MaxHealth = value;
        }

        private void ChangeMaxRecoverySpeed(float value)
        {
            if (value < 1f)
            {
                throw new ArgumentException("Max recovery speed cannot be less than 1");
            }
            
            MaxRecoverySpeed = value;
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
