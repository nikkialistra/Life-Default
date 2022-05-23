﻿using System;
using System.Collections;
using General;
using General.Interfaces;
using General.TimeCycle.Ticking;
using Sirenix.OdinInspector;
using Units.Calculations;
using Units.Humans.Animations;
using Units.Stats;
using UnityEngine;
using Zenject;

namespace Units
{
    public class UnitVitality : MonoBehaviour, IDamageable
    {
        [Required]
        [SerializeField] private HumanAnimations _humanAnimations;

        private VitalityCalculation _vitalityCalculation;

        private float _healthFractionToDecreaseRecoverySpeed;
        private float _recoveryHealthDelayAfterHit;

        private TickingRegulator _tickingRegulator;

        private Coroutine _takingDamageCoroutine;

        public event Action HealthChange;
        public event Action RecoverySpeedChange;
        
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

        private void Awake()
        {
            _vitalityCalculation = new VitalityCalculation();
        }

        private void Start()
        {
            _healthFractionToDecreaseRecoverySpeed = GlobalParameters.Instance.HealthFractionToDecreaseRecoverySpeed;
            _recoveryHealthDelayAfterHit = GlobalParameters.Instance.RecoveryHitDelayAfterHit;
        }

        private void OnEnable()
        {
            _vitalityCalculation.HealthChange += OnHealthChange;
            _vitalityCalculation.RecoverySpeedChange += OnRecoverySpeedChange;
            
            _tickingRegulator.AddToTickablesPerHour(_vitalityCalculation);
        }

        private void OnDisable()
        {
            _vitalityCalculation.HealthChange -= OnHealthChange;
            _vitalityCalculation.RecoverySpeedChange -= OnRecoverySpeedChange;
            
            _tickingRegulator.RemoveFromTickablesPerHour(_vitalityCalculation);
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

            _vitalityCalculation.Initialize(MaxHealth, MaxRecoverySpeed, _healthFractionToDecreaseRecoverySpeed, _recoveryHealthDelayAfterHit);
        }

        public void TakeDamage(float value)
        {
            _vitalityCalculation.TakeDamage(value);
            
            _humanAnimations.Hit();
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
        
        private void OnHealthChange(float value)
        {
            Health = value;
            
            if (!IsAlive)
            {
                StopTakingDamage();
                Wasted?.Invoke();
            }

            HealthChange?.Invoke();
        }

        private void OnRecoverySpeedChange(float value)
        {
            RecoverySpeed = value;
            RecoverySpeedChange?.Invoke();
        }

        private void ChangeMaxHealth(float value)
        {
            MaxHealth = value;
            _vitalityCalculation.ChangeMaxHealth(value);
        }

        private void ChangeMaxRecoverySpeed(float value)
        {
            MaxRecoverySpeed = value;
            _vitalityCalculation.ChangeMaxRecoverySpeed(value);
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
    }
}
