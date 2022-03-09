using System;
using System.Collections;
using Entities.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entities.Creature
{
    public class EntityHealth : MonoBehaviour, IDamageable
    {
        [MinValue(1)]
        [SerializeField] private float _maxVitality;
        [MinValue(1)]
        [SerializeField] private float _maxBlood;

        [Space]
        [ProgressBar(0, "_maxVitality", r: 0.929f, g: 0.145f, b: 0.145f, Height = 20)]
        [SerializeField] private float _startVitality = 100;
        [ProgressBar(0, "_maxBlood", r: 0.929f, g: 0.145f, b: 0.145f, Height = 20)]
        [SerializeField] private float _startBlood = 100;

        private float _vitality;
        private float _blood;

        private Coroutine _takingDamage;

        public event Action Die;
        public event Action<float, float> HealthChange;

        public float Vitality => _vitality / _maxVitality;

        public float Blood => _blood / _maxBlood;

        public int VitalityPercent => (int)(Vitality * 100);
        public int BloodPercent => (int)(Blood * 100);
        
        private bool IsAlive => _vitality > 0 && _blood > 0;

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
            _vitality = _startVitality;
            _blood = _startBlood;
        }

        public void TakeHealing(float value)
        {
            if (!IsAlive)
            {
                throw new InvalidOperationException("Healing cannot be applied to the died entity");
            }

            _vitality = Math.Min(_vitality + value, _maxVitality);
        }

        public void TakeDamage(float value)
        {
            CheckTakeDamageValidity(value);

            _vitality -= value;

            if (!IsAlive)
            {
                StopTakingDamage();
                Die?.Invoke();
            }

            HealthChange?.Invoke(_vitality / _maxVitality, _blood / _maxBlood);
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
