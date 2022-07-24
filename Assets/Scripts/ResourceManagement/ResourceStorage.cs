using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ResourceManagement
{
    public class ResourceStorage : MonoBehaviour
    {
        public event Action<int> QuantityChange;
        public event Action<int> DurabilityChange;

        public event Action<int, float> ChunkDrop;

        public int Quantity
        {
            get
            {
                var quantity = _storedQuantity + _preservedExtractedQuantity;
                return quantity >= 1f ? (int)Mathf.Floor(_storedQuantity + _preservedExtractedQuantity) : 0;
            }
        }

        public int Durability => (int)Mathf.Round(_durability);

        [MinValue(1)]
        [SerializeField] private int _minExtractedQuantityForDrop;
        [MinValue(1)]
        [SerializeField] private int _maxExtractedQuantityForDrop;

        [MinValue(1)]
        [SerializeField] private float _durability;
        [MinValue(1)]
        [SerializeField] private float _storedQuantity;

        private int _quantityToDrop;
        private float _preservedExtractedQuantity;

        private void Start()
        {
            _quantityToDrop = CalculateNextQuantityToDrop();
        }

        public void Extract(float destructionValue, float extractionEfficiency)
        {
            _preservedExtractedQuantity += ApplyDestruction(destructionValue) * extractionEfficiency;

            if (_preservedExtractedQuantity > _quantityToDrop)
                DropPreservedQuantity();
            else if (_durability <= 0 && _preservedExtractedQuantity >= 1f)
                DropRemainingQuantity();
        }

        public void Reset()
        {
            _durability = 0;
        }

        private void DropPreservedQuantity()
        {
            var sizeMultiplier = CalculateSizeMultiplier(_quantityToDrop, _maxExtractedQuantityForDrop);


            _preservedExtractedQuantity -= _quantityToDrop;

            _quantityToDrop = CalculateNextQuantityToDrop();

            ChunkDrop?.Invoke(_quantityToDrop, sizeMultiplier);
            QuantityChange?.Invoke(Quantity);
        }

        private void DropRemainingQuantity()
        {
            var sizeMultiplier = CalculateSizeMultiplier(_preservedExtractedQuantity, _maxExtractedQuantityForDrop);

            ChunkDrop?.Invoke((int)_preservedExtractedQuantity, sizeMultiplier);
            QuantityChange?.Invoke(Quantity);
        }

        private float CalculateSizeMultiplier(float quantity, int maxQuantity)
        {
            return Mathf.Sqrt(quantity / maxQuantity);
        }

        private int CalculateNextQuantityToDrop()
        {
            return Random.Range(_minExtractedQuantityForDrop, _maxExtractedQuantityForDrop + 1);
        }

        private float ApplyDestruction(float value)
        {
            if (_durability <= 0)
                throw new InvalidOperationException("Making damage cannot be applied to the destroyed resource");

            var quantityToDurabilityFraction = _storedQuantity / _durability;

            float extractedQuantity;

            if (_durability > value)
            {
                extractedQuantity = value * quantityToDurabilityFraction;
                _durability -= value;
            }
            else
            {
                extractedQuantity = _durability * quantityToDurabilityFraction;
                _durability = 0;
            }

            _storedQuantity -= extractedQuantity;

            DurabilityChange?.Invoke(Durability);

            return extractedQuantity;
        }
    }
}
