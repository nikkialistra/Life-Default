using System;
using System.Collections;
using General;
using General.Interfaces;
using General.TimeCycle.Ticking;
using Sirenix.OdinInspector;
using Units.Humans.Animations;
using Units.Stats;
using UnityEngine;
using Zenject;

namespace Units
{
    public class UnitVitality : MonoBehaviour, IDamageable, ITickablePerHour
    {
        [Required]
        [SerializeField] private HumanAnimations _humanAnimations;
        
        private float _lastHitTime;
        
        private float _recoveryHealthDelayAfterHit;
        
        private TickingRegulator _tickingRegulator;
        
        private Coroutine _takingDamageCoroutine;
        private Coroutine _recoveryHealthCoroutine;

        public event Action Change;
        public event Action Wasted;

        public float Health { get; private set; }
        public float MaxHealth { get; private set; }

        public float RecoverySpeed { get; private set; }
        public float MaxRecoverySpeed { get; private set; }

        public int HealthPercent => (int)((Health / MaxHealth) * 100);
        public int RecoverySpeedPercent => (int)((RecoverySpeed / MaxRecoverySpeed) * 100);
        
        private bool IsAlive => Health > 0;

        [Inject]
        public void Construct(TickingRegulator tickingRegulator)
        {
            _tickingRegulator = tickingRegulator;
        }

        private void Start()
        {
            _recoveryHealthDelayAfterHit = GlobalParameters.Instance.RecoveryHitDelayAfterHit;
        }

        private void OnEnable()
        {
            _tickingRegulator.AddToTickablesPerHour(this);
        }

        private void OnDisable()
        {
            _tickingRegulator.RemoveFromTickablesPerHour(this);
        }

        public void TickPerHour()
        {
            if (Time.time - _lastHitTime < _recoveryHealthDelayAfterHit)
            {
                return;
            }
            
            TakeHealing(RecoverySpeed);
        }

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

        private void OnDestroy()
        {
            if (_recoveryHealthCoroutine != null)
            {
                StopCoroutine(_recoveryHealthCoroutine);
                _recoveryHealthCoroutine = null;
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

        public void TakeDamage(float value)
        {
            CheckTakeDamageValidity(value);

            ReduceHealth(value);

            if (!IsAlive)
            {
                StopTakingDamage();
                Wasted?.Invoke();
            }

            _humanAnimations.Hit();
            
            Change?.Invoke();
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
        
        private void TakeHealing(float value)
        {
            if (!IsAlive)
            {
                throw new InvalidOperationException("Healing cannot be applied to the died entity");
            }

            Health = Math.Min(Health + value, MaxHealth);
            Change?.Invoke();
        }

        private void ReduceHealth(float value)
        {
            Health -= value;

            _lastHitTime = Time.time;
        }

        private void ChangeMaxHealth(float value)
        {
            if (value < 1f)
            {
                throw new ArgumentException("Max health cannot be less than 1");
            }
            
            MaxHealth = value;

            Health = Mathf.Min(Health, MaxHealth);
            Change?.Invoke();
        }

        private void ChangeMaxRecoverySpeed(float value)
        {
            if (value < 1f)
            {
                throw new ArgumentException("Max recovery speed cannot be less than 1");
            }
            
            MaxRecoverySpeed = value;

            RecoverySpeed = Mathf.Min(RecoverySpeed, MaxHealth);
            Change?.Invoke();
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
