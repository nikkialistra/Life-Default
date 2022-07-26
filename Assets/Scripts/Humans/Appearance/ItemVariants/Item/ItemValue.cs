using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Humans.Appearance.ItemVariants.Item
{
    [Serializable]
    public class ItemValue<T> : IItem<T>
    {
        public float RelativeChance
        {
            get => _relativeChance;
            set => _relativeChance = value;
        }

        public T Value => _value;
        public int Chance => _chance;

        [HideLabel]
        [HorizontalGroup("Split", 100)]
        [VerticalGroup("Split/Left")]
        [SerializeField] private T _value;

        [VerticalGroup("Split/Right")]
        [SerializeField] private int _chance = 1;

        [VerticalGroup("Split/Right")]
        [ReadOnly]
        [SerializeField] private float _relativeChance;
    }
}
