using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Units.Appearance.ItemVariants;
using UnityEngine;
using UnityEngine.Serialization;

namespace Units.Appearance.Variants
{
    [CreateAssetMenu(fileName = "Human Garment Set Variants", menuName = "Human Appearance/Garment Set Variants", order = 2)]
    public class GarmentSetVariants : ScriptableObject
    {
        [SerializeField] private List<GarmentSet> _garmentSetVariants;
        [SerializeField] private List<float> _chances;
        [SerializeField] private List<float> _relativeChances;

        [Button]
        public void CalculateRelativeChancesForVariants()
        {
            if (!_garmentSetVariants.Any())
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

        public GarmentSet GetRandom()
        {
            if (!_garmentSetVariants.Any())
            {
                return default;
            }
            
            var randomValue = Random.Range(0f, 1f);

            for (int i = 0; i < _relativeChances.Count; i++)
            {
                if (randomValue <= _relativeChances[i])
                {
                    return _garmentSetVariants[i];
                }
            }

            return default;
        }

        private void AlignListSizes()
        {
            if (_chances.Count > _garmentSetVariants.Count)
            {
                _chances.RemoveRange(_garmentSetVariants.Count, _chances.Count - _garmentSetVariants.Count);
            }
            
            if (_relativeChances.Count > _garmentSetVariants.Count)
            {
                _relativeChances.RemoveRange(_garmentSetVariants.Count, _relativeChances.Count - _garmentSetVariants.Count);
            }
            
            for (int i = 1; i <= _garmentSetVariants.Count; i++)
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
