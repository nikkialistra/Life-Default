using System.Collections.Generic;
using System.Linq;
using Entities.Services.Appearance.ItemVariants;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entities.Services.Appearance.Variants
{
    [CreateAssetMenu(fileName = "Human Garment Set Variants", menuName = "Human Appearance/Garment Set Variants", order = 2)]
    public class GarmentSetVariants : ScriptableObject
    {
        [SerializeField] private List<GarmentVariants> _garmentVariants;
        [SerializeField] private List<float> _chances;
        [SerializeField] private List<float> _relativeChances;

        [Button]
        public void CalculateRelativeChancesForVariants()
        {
            if (!_garmentVariants.Any())
            {
                return;
            }
            
            AlignListSizes();

            var sum = _chances.Sum();

            for (int i = 0; i < _relativeChances.Count; i++)
            {
                _relativeChances[i] = _chances[i] / sum;
            }
        }

        public GarmentVariants GetRandom()
        {
            if (!_garmentVariants.Any())
            {
                return default;
            }
            
            var randomValue = Random.Range(0f, 1f);

            for (int i = 0; i < _relativeChances.Count; i++)
            {
                if (randomValue <= _relativeChances[i])
                {
                    return _garmentVariants[i];
                }
            }

            return default;
        }

        private void AlignListSizes()
        {
            if (_chances.Count > _garmentVariants.Count)
            {
                _chances.RemoveRange(_garmentVariants.Count, _chances.Count - _garmentVariants.Count);
            }
            
            if (_relativeChances.Count > _garmentVariants.Count)
            {
                _relativeChances.RemoveRange(_garmentVariants.Count, _relativeChances.Count - _garmentVariants.Count);
            }
            
            for (int i = 1; i <= _garmentVariants.Count; i++)
            {
                if (_chances.Count < i)
                {
                    _chances.Add(1);
                }

                if (_relativeChances.Count < i)
                {
                    _relativeChances.Add(0);
                }
            }
        }
    }
}
